using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Cloo;

namespace CountItemSets
{
    class GPUTransactionFrequentItemsetGenerator : IFrequentItemsetGenerator
    {
        public IDictionary<long, int> Level1 { get { return dictionaryLevel1; } }
        public IDictionary<string, int> Level2 { get { return dictionaryLevel2; } }
        public IDictionary<string, int> Level3 { get { return dictionaryLevel3; } }
        public IDictionary<string, int> Level4 { get { return dictionaryLevel4; } }
        public IDictionary<string, int> Level5 { get { return dictionaryLevel5; } }

        private IDictionary<long, int> dictionaryLevel1;
        private IDictionary<string, int> dictionaryLevel2;
        private IDictionary<string, int> dictionaryLevel3 = new Dictionary<string, int>();
        private IDictionary<string, int> dictionaryLevel4 = new Dictionary<string, int>();
        private IDictionary<string, int> dictionaryLevel5 = new Dictionary<string, int>();

        HashSet<long> pruningExcludeItems = new HashSet<long>();

        private string fileNameTransaction; // Should be handled by separate class TransactionReader
        private int transactionCount; // Should be handled by separate class TransactionReader
        private Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>(); // Should be handled by separate class TransactionContext
        private string fileNameExcludeItems; // Should be handled by separate class

        private Stopwatch stopwatch = new Stopwatch();
        private double pruningMinSupport = 0.0001;
        private int maxNrTransactions = 1000000000;
        private GPUAlgorithm gpuAlg;

        public void SetMaxNrTransactions(int maxNrTransactions)
        {
            this.maxNrTransactions = maxNrTransactions;
        }

        public void SetPruningMinSupport(double minSupport)
        {
            pruningMinSupport = minSupport;
        }

        public int GetTransactionCount()
        {
            return transactionCount;
        }

        public int GetProgess()
        {
            return gpuAlg == null ? 0 : gpuAlg.Progress;
        }

        public Stopwatch GetStopWatch()
        {
            return stopwatch;
        }

        public Dictionary<long, int> GetDictionaryEANtoVGR()
        {
            return dictionaryEANtoVGR;
        }

        public void BeginGenerate(string fileNameTransaction, GenerateCallBack callBack)
        {
            this.fileNameTransaction = fileNameTransaction;
            Thread thread = new Thread(new ParameterizedThreadStart(GenerateThread));
            thread.Name = "GenaratorThread";
            thread.Start(callBack);
        }
        public void Generate(string fileNameTransaction)
        {
            this.fileNameTransaction = fileNameTransaction;
            GenerateThread(null);
        }

        private void GenerateThread(object obj)
        {
            ComputeContext gpuContext;
            ComputeCommandQueue gpuCmdQueue;
            SetUpOpenCL(out gpuContext, out gpuCmdQueue);

            if (gpuContext == null)
            {
                throw new ApplicationException("no GPU with OpenCL support available.");
            }

            GenerateCallBack callBack = obj as GenerateCallBack;
            stopwatch.Restart();

            TransactionReader reader = new TransactionReader(fileNameTransaction, dictionaryEANtoVGR);
            reader.SetMaxNrTransactions(maxNrTransactions);

            // Level 1
            reader.Begin();
            gpuAlg = new GPUAlgorithm(gpuContext, reader,
                                      pruningMinSupport); // First pass through the transactions.

            // E1
            // V1
            reader.Begin();
            gpuAlg.Run(gpuCmdQueue, reader); // Second and third pass through the transactions.
            // ...
            dictionaryLevel1 = new ConcurrentDictionary<long, int>(gpuAlg.Get1TupleResult(gpuCmdQueue).Where(item => (!pruningExcludeItems.Contains(item.Key))));
            transactionCount = gpuAlg.GetNoTransactions();
            Console.Out.WriteLine("Found " + dictionaryLevel1.Count + " interesting 1-tuples.");

            // Level 2
            // E1 E2
            // E1 V1
            // V1 V2
            dictionaryLevel2 = gpuAlg.GetNTupleResult(2, gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel2.Count + " interesting 2-tuples.");

            // Level 3
            // E1 E2 E3
            // E1 E2 V1
            // E1 V1 V2
            // V1 V2 V3
            dictionaryLevel3 = gpuAlg.GetNTupleResult(3, gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel3.Count + " interesting 3-tuples.");

            // Level 4
            // E1 E2 E3 E4
            // E1 E2 E3 V1
            // E1 E2 V1 V2
            // E1 V1 V2 V3
            // V1 V2 V3 V4
            dictionaryLevel4 = gpuAlg.GetNTupleResult(4, gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel4.Count + " interesting 4-tuples.");

            // Level 5
            // E1 E2 E3 E4 E5
            // V1 V2 V3 V4 V5
            dictionaryLevel5 = gpuAlg.GetNTupleResult(5, gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel5.Count + " interesting 5-tuples.");

            stopwatch.Stop();
            //textBoxTime.Text = stopwatch.Elapsed.ToString();

            if (callBack != null) callBack();

            gpuAlg.Dispose();
            gpuCmdQueue.Dispose();
            gpuContext.Dispose();
        }

        private void SetUpOpenCL(out ComputeContext context, out ComputeCommandQueue cmdQueue)
        {
            ComputePlatform platform = null;
            ComputeDevice gpu = null;
            Console.Out.WriteLine("OpenCL platforms:");
            foreach (ComputePlatform p in ComputePlatform.Platforms)
            {
                Console.Out.WriteLine("  " + p.Name + ", " + p.Profile + ", " + p.Vendor);
                foreach (ComputeDevice d in p.Devices)
                {
                    Console.Out.WriteLine("    " + d.Name + ", " + d.Type + ", " + d.MaxComputeUnits +
                                          ", " + d.OpenCLCVersionString + ", " + d.Available);
                    if (d.Type == ComputeDeviceTypes.Gpu)
                    {
                        platform = p;
                        gpu = d;
                    }
                }
            }
            if (gpu != null)
            {
                context = new ComputeContext(new ComputeDevice[]{ gpu },
                                             new ComputeContextPropertyList(platform), null, IntPtr.Zero);
                cmdQueue = new ComputeCommandQueue(context, gpu, ComputeCommandQueueFlags.None);
                Console.Out.WriteLine("Selected OpenCL platform: " + context.Platform.Name + " and device: " + gpu.Name + ".");
            }
            else
            {
                context = null;
                cmdQueue = null;
            }
            Console.Out.Flush();
        }

        private class GPUAlgorithm : IDisposable
        {
            int MAX_TUPLE_SIZE = 5;
            double pruningMinSupport = 0.0;
            ComputeProgram program;
            ComputeKernel[,] kernel;
            ComputeBuffer<uint> transactionsArg, tuple1FrequenciesArg;
            System.Runtime.InteropServices.GCHandle tuple1Handle;
            OpenCLHashTable[] tupleDictionary;
            long[] EANfromID; // NOTE: index/ID 0 is unassigned (and EOT marker in transactionItems).
            Dictionary<long, int> IDfromEAN;
            int noTransactions;
            uint[] transactionItems;
            int lastIdx;
            uint[] itemFrequencies; // NOTE: index 0 is unassigned.
            string[] keyVar;
            char[] loopVar;

            public int Progress { get; private set; }

            public GPUAlgorithm(ComputeContext context, TransactionReader reader,
                                double pruningMinSupport)
            {
                Progress = 0;
                this.pruningMinSupport = pruningMinSupport;
                SetUpTranslations(reader);
                Progress = 5;
                transactionItems = new uint[16 * 1024 * 1024];
                itemFrequencies  = new uint[EANfromID.Length];
                tupleDictionary = new OpenCLHashTable[] {
                        null,
                        new OpenCLHashTable(context, 2, 16 * 1024 * 1024),
                        new OpenCLHashTable(context, 3, 16 * 1024 * 1024),
                        new OpenCLHashTable(context, 4, 16 * 1024 * 1024),
                        new OpenCLHashTable(context, 5, 16 * 1024 * 1024)
                    };
                CheckGPUMemory(context);
                SetUpProgram(context);
            }

            public void Run(ComputeCommandQueue queue, TransactionReader reader)
            {
                Progress = 5;
                lastIdx = ReadTransactions(reader);
                Progress = 10;

                // Set up the transaction items argument.
                transactionsArg =
                    new ComputeBuffer<uint>(kernel[0,0].Context,
                                            ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                                            transactionItems);
                for (int tupleSize = 1; tupleSize <= MAX_TUPLE_SIZE; tupleSize++)
                {
                    queue.AddBarrier();
                    RunPhase(tupleSize, queue);
                    Progress += 80/MAX_TUPLE_SIZE;
                }
                queue.Finish();
                Progress = 100;
            }

            public int GetNoTransactions()
            {
                return noTransactions;
            }

            public IDictionary<long, int> Get1TupleResult(ComputeCommandQueue queue)
            {
                queue.Read<uint>(tuple1FrequenciesArg,
                                 true,
                                 0,
                                 tuple1FrequenciesArg.Count,
                                 tuple1Handle.AddrOfPinnedObject(),
                                 null);
                uint max = 0;
                int maxIdx = -1;
                double avg = 0.0;
                for (int i = 1; i < itemFrequencies.Length; i++)
                {
                    avg += itemFrequencies[i] / (double)itemFrequencies.Length;
                    if (itemFrequencies[i] > max)
                    {
                        max = itemFrequencies[i];
                        maxIdx = i;
                    }
                    //Console.Out.WriteLine(" " + i + ": " + itemFrequencies[i]);
                }
                Console.Out.WriteLine("Average frequency: " + avg);
                Console.Out.WriteLine("Most frequent item: " + maxIdx + ": " + max);

                Dictionary<long, int> result = new Dictionary<long, int>();
                for (int i = 1; i < itemFrequencies.Length; i++)
                {
                    if (itemFrequencies[i] >= pruningMinSupport * noTransactions)
                    {
                        result.Add(EANfromID[i], (int)itemFrequencies[i]);
                    }
                }
                return result;
            }

            public IDictionary<string, int> GetNTupleResult(int n, ComputeCommandQueue queue)
            {
                if (1 < n && n <= MAX_TUPLE_SIZE)
                {
                    return tupleDictionary[n - 1].AsDictionary(queue, EANfromID);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(n), n,
                                                          "the tuple size must be between 2 and " +
                                                          MAX_TUPLE_SIZE + ".");
                }
            }

            private void CheckGPUMemory(ComputeContext context)
            {
                long bytesRequired = 0;

                bytesRequired += sizeof(uint) * transactionItems.Length;
                bytesRequired += sizeof(uint) * itemFrequencies.Length;
                foreach (OpenCLHashTable d in tupleDictionary.Where(d => d != null))
                {
                    bytesRequired += d.MaxSizeInBytes;
                }
                Console.Out.WriteLine("Available GPU memory: " + context.Devices.First().GlobalMemorySize + " bytes.");
                Console.Out.WriteLine("Required GPU memory: " + bytesRequired + " bytes.");
                if (bytesRequired > context.Devices.First().GlobalMemorySize)
                {
                    throw new InvalidOperationException("insufficient GPU memory.");
                }
            }

            private void SetUpProgram(ComputeContext context)
            {
                kernel = new ComputeKernel[MAX_TUPLE_SIZE, 2];
                keyVar = new string[MAX_TUPLE_SIZE];
                loopVar = new char[MAX_TUPLE_SIZE];
                for (int i = 0; i < MAX_TUPLE_SIZE; i++)
                {
                    keyVar[i] = "key" + (i + 1);
                    loopVar[i] = (char)('i' + i);
                }

                IEnumerable<string> htSource = from d in tupleDictionary
                                               where d != null
                                               select d.SourceOpenCL;
                List<string> code =
                    new List<string>(htSource.Concat(new string[] { Phase1aOpenCL, Phase1bOpenCL,
                                                                    Phase2aOpenCL, PhaseNb(2) }));
                for (int p = 3; p <= MAX_TUPLE_SIZE; p++)
                {
                    code.Add(PhaseNa(p));
                    code.Add(PhaseNb(p));
                }
                program = new ComputeProgram(context, code.ToArray());
                // Compile the program.
                try
                {
                    program.Build(null, null, null, IntPtr.Zero);
                }
                catch (BuildProgramFailureComputeException)
                {
                    string message = program.GetBuildLog(context.Devices[0]);
                    throw new ArgumentException(message);
                }
                for (int p = 0; p < MAX_TUPLE_SIZE; p++)
                {
                    kernel[p, 0] = program.CreateKernel("count_" + (p+1) + "tuple_frequencies");
                    kernel[p, 1] = program.CreateKernel("clip_" + (p+1) + "tuple_frequencies");
                }

                // Create some of the arguments.
                tuple1Handle =
                    System.Runtime.InteropServices.GCHandle.Alloc(itemFrequencies,
                                                                  System.Runtime.InteropServices.GCHandleType.Pinned);
                tuple1FrequenciesArg =
                    new ComputeBuffer<uint>(context,
                                            ComputeMemoryFlags.WriteOnly| ComputeMemoryFlags.UseHostPointer,
                                            itemFrequencies);

                // Write out representative OpenCL code.
                if (false) {
                    Console.Out.WriteLine(Phase1aOpenCL);
                    Console.Out.WriteLine(Phase1bOpenCL);
                    Console.Out.WriteLine(Phase2aOpenCL);
                    Console.Out.WriteLine(PhaseNb(2));
                    Console.Out.WriteLine(tupleDictionary[1].SourceOpenCL);
                    foreach (int p in new int[] {3, 4, 5})
                    {
                        Console.Out.WriteLine(PhaseNa(p));
                        Console.Out.WriteLine(PhaseNb(p));
                        Console.Out.WriteLine(tupleDictionary[p-1].SourceOpenCL);
                    }
                }
            }

            private void SetUpTranslations(TransactionReader reader)
            {
                // There are EAN codes not present in the EAN translation table so
                // get all codes from the transactions, instead.
                IDfromEAN = new Dictionary<long, int>();
                List<TransactionReader.Transaction> transactions;
                while ((transactions = reader.ReadList(1000)) != null)
                {
                    foreach (TransactionReader.Transaction t in transactions)
                    {
                        foreach (long ean in t.EANCodes)
                        {
                            IDfromEAN[ean] = -1;
                        }
                    }
                }

                EANfromID = new long[IDfromEAN.Count+1];

                var sorted = from ean in IDfromEAN.Keys
                             orderby ean ascending
                             select ean;
                int next = 1;
                foreach (long ean in sorted)
                {
                    EANfromID[next] = ean;
                    IDfromEAN[ean] = next;
                    next++;
                }
            }

            private int ReadTransactions(TransactionReader reader)
            {
                noTransactions = 0;
                int idx = 0;
                int transactionMaxItems = 0;
                List<TransactionReader.Transaction> transactions;
                while ((transactions = reader.ReadList(1000)) != null)
                {
                    foreach (TransactionReader.Transaction t in transactions)
                    {
                        int first = idx;
                        for (int i = 0; i < t.EANCodes.Count; i++)
                        {
                            transactionMaxItems = Math.Max(transactionMaxItems, t.EANCodes.Count);
                            if (idx >= transactionItems.Length)
                            {
                                throw new ArgumentException("buffer overflow");
                            }
                            transactionItems[idx++] = (uint)IDfromEAN[t.EANCodes[i]];
                        }
                        Array.Sort(transactionItems, first, idx - first);
                        transactionItems[idx++] = 0;
                        noTransactions++;
                    }
                }
                Console.Out.WriteLine("Found " + noTransactions +
                                      " transactions containing a total of " +
                                      (idx - noTransactions) + " item EANs.");
                Console.Out.WriteLine("The number of unique item EANs is " + (EANfromID.Length - 1));
                Console.Out.WriteLine("Maximum transaction size " + transactionMaxItems + " items.");
                return idx;
            }

            private void RunPhase(int tupleSize, ComputeCommandQueue queue)
            {
                ComputeKernel kernelA = kernel[tupleSize-1, 0];
                ComputeKernel kernelB = kernel[tupleSize-1, 1];
                Console.Out.WriteLine("Executing " + kernelA.FunctionName + " on " + lastIdx +
                                      " GPU threads.");
                kernelA.SetMemoryArgument(0, transactionsArg);
                kernelA.SetMemoryArgument(1, tuple1FrequenciesArg);
                for (int argNo = 2; argNo <= tupleSize; argNo++)
                {
                    // The first tuple size is 2 which is at index 1.
                    tupleDictionary[argNo-1].SetAsArgument(kernelA, argNo);
                }
                queue.Execute(kernelA, new long[] { 0 }, new long[] { lastIdx }, null, null);
                queue.AddBarrier();

                int noThreads;
                if (tupleSize == 1)
                {
                    noThreads = (int)tuple1FrequenciesArg.Count;
                    kernelB.SetMemoryArgument(0, tuple1FrequenciesArg);
                }
                else
                {
                    noThreads = tupleDictionary[tupleSize-1].MaxSize;
                    tupleDictionary[tupleSize-1].SetAsArgument(kernelB, 0);
                }
                Console.Out.WriteLine("Executing " + kernelB.FunctionName + " on " +
                                      noThreads + " GPU threads.");
                kernelB.SetValueArgument<uint>(1, (uint)(pruningMinSupport * noTransactions));
                queue.Execute(kernelB, new long[] { 0 }, new long[] { noThreads }, null, null);
            }

            private string PhaseNa(int n)
            {
                string source = @"
__kernel void count_" + n + @"tuple_frequencies(__global /*__read_only*/  uint* transaction_items,
" + PhaseNaFormalArgs(n) +
@"                                       __global /*__read_write*/ uint* frequencies_" + n + @"tuple)
{
    // Vector element index: denotes item of a transaction.
    int i = get_global_id(0);
    uint key1 = transaction_items[i];
    if (key1 > 0 && frequencies_1tuple[key1] > 0) {
";
                for (int i = 1; i < n; i++)
                {
                    source += indent(2*i) + "int " + loopVar[i] + " = " + loopVar[i - 1] + " + 1;\n";
                    source += indent(2*i) + "while (transaction_items[" + loopVar[i] + "] > 0) {\n";
                    source += indent(2*i + 1) + "uint " + keyVar[i] + " = transaction_items[" + loopVar[i] + "];\n";
                    if (i == n - 1) continue;
                    source += indent(2*i + 1) + "if (ht" + (i+1) + "_lookup(frequencies_" + (i+1) + "tuple, " +
                                                     KeyArgs(i+1) + ") > 0) {\n";
                }
                source += indent(2*n - 1) + "if (" + CheckSubTuples(n) + ") {\n";
                source += indent(2 * n) + "ht" + n + "_add_or_increase(frequencies_" + n + "tuple, " +
                                                                       KeyArgs(n) + ");\n";
                for (int i = n-1; i > 0; i--)
                {
                    source += indent(2*i + 1) + "}\n";
                    source += indent(2*i + 1) + loopVar[i] + "++;\n";
                    source += indent(2*i) + "}\n";
                }
                source += @"
    }
}
";
                return source;
            }

            private static string indent(int level)
            {
                string indent = "";
                for (int i = 0; i < level; i++)
                {
                    indent += "    ";
                }
                return indent;
            }

            private static string PhaseNaFormalArgs(int n)
            {
                string source = "";
                for (int i = 1; i < n; i++)
                {
                    source +=
@"                                       __global /*__read_only*/  uint * frequencies_" + i + @"tuple,
";
                }
                return source;
            }

            private string KeyArgs(int n)
            {
                string source = "";
                for (int i = 0; i < n-1; i++)
                {
                    source += keyVar[i] + ", ";
                }
                source += keyVar[n-1];
                return source;
            }

            private string CheckSubTuples(int n)
            {
                string source = "1";
                for (int without = n-2; without >= 0; without--)
                {
                    source += " &&\n";
                    source += indent(2 * n) + "ht" + (n - 1) + "_lookup(frequencies_" + (n - 1) + "tuple, " +
                              GetKeySubTuple(n-1, without) + "key" + n + ") > 0";
                }
                return source;
            }

            private string GetKeySubTuple(int max, int without)
            {
                string keys = "";
                for (int i = 0; i < max; i++)
                {
                    if (i != without)
                    {
                        keys += keyVar[i] + ", ";
                    }
                }
                return keys;
            }

            private static string PhaseNb(int n)
            {
                return @"
__kernel void clip_" + n + @"tuple_frequencies(__global /*__read_write*/ uint* frequencies,
                                               /*__read_only*/  uint  limit)
{
    // Vector element index: index into hashtable denotes frequency of item-pair i.
    int i = " + (n + 1) + @"*get_global_id(0) + " + n + @";
    frequencies[i] *= (uint)step((float)limit, (float)frequencies[i]);
}";
            }

            private string Phase1aOpenCL =
                @"
__kernel void count_1tuple_frequencies(__global /*__read_only*/  uint* transaction_items,
                                       __global /*__read_write*/ uint* frequencies)
{
    // Vector element index: denotes item of a transaction.
    int idx = get_global_id(0);
    atomic_inc(&frequencies[transaction_items[idx]]);
}";
            private string Phase1bOpenCL =
                @"
__kernel void clip_1tuple_frequencies(__global /*__read_write*/ uint* frequencies,
                                               /*__read_only*/  uint  limit)
{
    // Vector element index: denotes frequency of item ID i.
    int i = get_global_id(0);
    frequencies[i] *= (uint)step(0.5f, (float)i) * (uint)step((float)limit, (float)frequencies[i]);
}";

            private string Phase2aOpenCL =
                @"
__kernel void count_2tuple_frequencies(__global /*__read_only*/  uint* transaction_items,
                                       __global /*__read_only*/  uint* frequencies_1tuple,
                                       __global /*__read_write*/ uint* frequencies_2tuple)
{
    // Vector element index: denotes item of a transaction.
    int i = get_global_id(0);
    uint key1 = transaction_items[i];
    if (key1 > 0 && frequencies_1tuple[key1] > 0) {
        int j = i + 1;
        while (transaction_items[j] > 0) {
            if (frequencies_1tuple[transaction_items[j]] > 0) {
                ht2_add_or_increase(frequencies_2tuple, key1, transaction_items[j]);
            }
            j++;
        }
    }
}";

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        transactionsArg.Dispose();
                        tuple1Handle.Free();
                        tuple1FrequenciesArg.Dispose();
                        foreach (OpenCLHashTable d in tupleDictionary.Where(d => d != null))
                        {
                            d.Dispose();
                        }
                        foreach (ComputeKernel k in kernel)
                        {
                            k.Dispose();
                        }
                        program.Dispose();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
            }
            #endregion
        }
    }
}
