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

        private int progressGenerate = 0;
        private Stopwatch stopwatch = new Stopwatch();
        private double pruningMinSupport = 0.0001;
        private int maxNrTransactions = 1000000000;

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
            return progressGenerate;
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
            progressGenerate = 0;
            stopwatch.Restart();

            TransactionReader reader = new TransactionReader(fileNameTransaction, dictionaryEANtoVGR);
            reader.SetMaxNrTransactions(maxNrTransactions);

            // Level 1
            reader.Begin();
            GPUAlgorithm gpuAlg = new GPUAlgorithm(gpuContext, reader,
                                                   pruningMinSupport); // First pass through the transactions.
            progressGenerate = 10;

            // E1
            // V1
            reader.Begin();
            gpuAlg.Run(gpuCmdQueue, reader); // Second and third pass through the transactions.
            // ...
            dictionaryLevel1 = new ConcurrentDictionary<long, int>(gpuAlg.Get1TupleResult(gpuCmdQueue).Where(item => (!pruningExcludeItems.Contains(item.Key))));
            transactionCount = gpuAlg.GetNoTransactions();
            Console.Out.WriteLine("Found " + dictionaryLevel1.Count + " interesting 1-tuples.");
            progressGenerate = 20;

            // Level 2
            // E1 E2
            // E1 V1
            // V1 V2
            dictionaryLevel2 = gpuAlg.Get2TupleResult(gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel2.Count + " interesting 2-tuples.");
            progressGenerate = 40;

            // Level 3
            // E1 E2 E3
            // E1 E2 V1
            // E1 V1 V2
            // V1 V2 V3
            dictionaryLevel3 = gpuAlg.Get3TupleResult(gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel3.Count + " interesting 3-tuples.");
            progressGenerate = 60;

            // Level 4
            // E1 E2 E3 E4
            // E1 E2 E3 V1
            // E1 E2 V1 V2
            // E1 V1 V2 V3
            // V1 V2 V3 V4
            dictionaryLevel4 = gpuAlg.Get4TupleResult(gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel4.Count + " interesting 4-tuples.");
            progressGenerate = 80;

            // Level 5
            // E1 E2 E3 E4 E5
            // V1 V2 V3 V4 V5
            dictionaryLevel5 = gpuAlg.Get5TupleResult(gpuCmdQueue);
            Console.Out.WriteLine("Found " + dictionaryLevel5.Count + " interesting 5-tuples.");

            stopwatch.Stop();
            //textBoxTime.Text = stopwatch.Elapsed.ToString();
            progressGenerate = 100;

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
        }

        private class GPUAlgorithm : IDisposable
        {
            double pruningMinSupport = 0.0;
            ComputeProgram program;
            ComputeKernel phase1aKernel;
            ComputeKernel phase1bKernel;
            ComputeKernel phase2aKernel;
            ComputeKernel phase2bKernel;
            ComputeKernel phase3aKernel;
            ComputeKernel phase3bKernel;
            ComputeKernel phase4aKernel;
            ComputeKernel phase4bKernel;
            ComputeKernel phase5aKernel;
            ComputeKernel phase5bKernel;
            ComputeBuffer<uint> transactionsArg, tuple1FrequenciesArg;
            System.Runtime.InteropServices.GCHandle tuple1Handle;
            OpenCLHashTable tuple2Dictionary;
            OpenCLHashTable tuple3Dictionary;
            OpenCLHashTable tuple4Dictionary;
            OpenCLHashTable tuple5Dictionary;
            long[] EANfromID; // NOTE: index/ID 0 is unassigned (and EOT marker in transactionItems).
            Dictionary<long, int> IDfromEAN;
            int noTransactions;
            uint[] transactionItems;
            int lastIdx;
            uint[] itemFrequencies; // NOTE: index 0 is unassigned.

            public GPUAlgorithm(ComputeContext context, TransactionReader reader,
                                double pruningMinSupport)
            {
                this.pruningMinSupport = pruningMinSupport;
                SetUpTranslations(reader);
                transactionItems = new uint[16 * 1024 * 1024];
                itemFrequencies  = new uint[EANfromID.Length];
                tuple2Dictionary = new OpenCLHashTable(context, 2, 16 * 1024 * 1024);
                tuple3Dictionary = new OpenCLHashTable(context, 3, 16 * 1024 * 1024);
                tuple4Dictionary = new OpenCLHashTable(context, 4, 16 * 1024 * 1024);
                tuple5Dictionary = new OpenCLHashTable(context, 5, 16 * 1024 * 1024);
                SetUpProgram(context);
            }

            public void Run(ComputeCommandQueue queue, TransactionReader reader)
            {
                lastIdx = ReadTransactions(reader);

                // Set up the transaction items argument.
                transactionsArg =
                    new ComputeBuffer<uint>(phase1aKernel.Context,
                                            ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                                            transactionItems);
                RunPhase1(queue);
                queue.AddBarrier();
                RunPhase2(queue);
                queue.AddBarrier();
                RunPhase3(queue);
                queue.AddBarrier();
                RunPhase4(queue);
                queue.AddBarrier();
                RunPhase5(queue);
                queue.Finish();
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

            public IDictionary<string, int> Get2TupleResult(ComputeCommandQueue queue)
            {
                return tuple2Dictionary.AsDictionary(queue, EANfromID);
            }

            public IDictionary<string, int> Get3TupleResult(ComputeCommandQueue queue)
            {
                return tuple3Dictionary.AsDictionary(queue, EANfromID);
            }

            public IDictionary<string, int> Get4TupleResult(ComputeCommandQueue queue)
            {
                return tuple4Dictionary.AsDictionary(queue, EANfromID);
            }

            public IDictionary<string, int> Get5TupleResult(ComputeCommandQueue queue)
            {
                return tuple5Dictionary.AsDictionary(queue, EANfromID);
            }

            private void SetUpProgram(ComputeContext context)
            {
                program = new ComputeProgram(context,
                                             new string[] {
                                                            tuple2Dictionary.SourceOpenCL,
                                                            tuple3Dictionary.SourceOpenCL,
                                                            tuple4Dictionary.SourceOpenCL,
                                                            tuple5Dictionary.SourceOpenCL,
                                                            constantsOpenCL,
                                                            Phase1aOpenCL, Phase1bOpenCL,
                                                            Phase2aOpenCL, Phase2bOpenCL,
                                                            Phase3aOpenCL, Phase3bOpenCL,
                                                            Phase4aOpenCL, Phase4bOpenCL,
                                                            Phase5aOpenCL, Phase5bOpenCL
                                                          });
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
                phase1aKernel = program.CreateKernel("count_1tuple_frequencies");
                phase1bKernel = program.CreateKernel("clip_1tuple_frequencies");

                phase2aKernel = program.CreateKernel("count_2tuple_frequencies");
                phase2bKernel = program.CreateKernel("clip_2tuple_frequencies");

                phase3aKernel = program.CreateKernel("count_3tuple_frequencies");
                phase3bKernel = program.CreateKernel("clip_3tuple_frequencies");

                phase4aKernel = program.CreateKernel("count_4tuple_frequencies");
                phase4bKernel = program.CreateKernel("clip_4tuple_frequencies");

                phase5aKernel = program.CreateKernel("count_5tuple_frequencies");
                phase5bKernel = program.CreateKernel("clip_5tuple_frequencies");

                // Create some of the arguments.
                tuple1Handle =
                    System.Runtime.InteropServices.GCHandle.Alloc(itemFrequencies,
                                                                  System.Runtime.InteropServices.GCHandleType.Pinned);
                tuple1FrequenciesArg =
                    new ComputeBuffer<uint>(context,
                                            ComputeMemoryFlags.WriteOnly| ComputeMemoryFlags.UseHostPointer,
                                            itemFrequencies);

                Console.Out.WriteLine(tuple3Dictionary.SourceOpenCL);
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

            private void RunPhase1(ComputeCommandQueue queue)
            {
                Console.Out.WriteLine("Executing " + phase1aKernel.FunctionName + " on " + lastIdx +
                                      " GPU threads.");
                phase1aKernel.SetMemoryArgument(0, transactionsArg);
                phase1aKernel.SetMemoryArgument(1, tuple1FrequenciesArg);
                queue.Execute(phase1aKernel, new long[] { 0 }, new long[] { lastIdx }, null, null);
                queue.AddBarrier();

                Console.Out.WriteLine("Executing " + phase1bKernel.FunctionName + " on " +
                                      itemFrequencies.Length + " GPU threads.");
                phase1bKernel.SetMemoryArgument(0, tuple1FrequenciesArg);
                phase1bKernel.SetValueArgument<uint>(1, (uint)(pruningMinSupport * noTransactions));
                queue.Execute(phase1bKernel, new long[] { 0 }, new long[] { itemFrequencies.Length }, null, null);
            }

            private void RunPhase2(ComputeCommandQueue queue)
            {
                Console.Out.WriteLine("Executing " + phase2aKernel.FunctionName + " on " + lastIdx +
                                      " GPU threads.");
                phase2aKernel.SetMemoryArgument(0, transactionsArg);
                phase2aKernel.SetMemoryArgument(1, tuple1FrequenciesArg);
                tuple2Dictionary.SetAsArgument(phase2aKernel, 2);
                queue.Execute(phase2aKernel, new long[] { 0 }, new long[] { lastIdx }, null, null);
                queue.AddBarrier();

                Console.Out.WriteLine("Executing " + phase2bKernel.FunctionName + " on " +
                    tuple2Dictionary.MaxSize + " GPU threads.");
                tuple2Dictionary.SetAsArgument(phase2bKernel, 0);
                phase2bKernel.SetValueArgument<uint>(1, (uint)(pruningMinSupport * noTransactions));
                queue.Execute(phase2bKernel, new long[] { 0 }, new long[] { tuple2Dictionary.MaxSize }, null, null);
            }

            private void RunPhase3(ComputeCommandQueue queue)
            {
                Console.Out.WriteLine("Executing " + phase3aKernel.FunctionName + " on " + lastIdx +
                                      " GPU threads.");
                phase3aKernel.SetMemoryArgument(0, transactionsArg);
                phase3aKernel.SetMemoryArgument(1, tuple1FrequenciesArg);
                tuple2Dictionary.SetAsArgument(phase3aKernel, 2);
                tuple3Dictionary.SetAsArgument(phase3aKernel, 3);
                queue.Execute(phase3aKernel, new long[] { 0 }, new long[] { lastIdx }, null, null);
                queue.AddBarrier();

                Console.Out.WriteLine("Executing " + phase3bKernel.FunctionName + " on " +
                    tuple3Dictionary.MaxSize + " GPU threads.");
                tuple3Dictionary.SetAsArgument(phase3bKernel, 0);
                phase3bKernel.SetValueArgument<uint>(1, (uint)(pruningMinSupport * noTransactions));
                queue.Execute(phase3bKernel, new long[] { 0 }, new long[] { tuple3Dictionary.MaxSize }, null, null);
            }

            private void RunPhase4(ComputeCommandQueue queue)
            {
                Console.Out.WriteLine("Executing " + phase4aKernel.FunctionName + " on " + lastIdx +
                                      " GPU threads.");
                phase4aKernel.SetMemoryArgument(0, transactionsArg);
                phase4aKernel.SetMemoryArgument(1, tuple1FrequenciesArg);
                tuple2Dictionary.SetAsArgument(phase4aKernel, 2);
                tuple3Dictionary.SetAsArgument(phase4aKernel, 3);
                tuple4Dictionary.SetAsArgument(phase4aKernel, 4);

                queue.Execute(phase4aKernel, new long[] { 0 }, new long[] { lastIdx }, null, null);
                queue.AddBarrier();

                Console.Out.WriteLine("Executing " + phase4bKernel.FunctionName + " on " +
                    tuple4Dictionary.MaxSize + " GPU threads.");
                tuple4Dictionary.SetAsArgument(phase4bKernel, 0);
                phase4bKernel.SetValueArgument<uint>(1, (uint)(pruningMinSupport * noTransactions));
                queue.Execute(phase4bKernel, new long[] { 0 }, new long[] { tuple4Dictionary.MaxSize }, null, null);
            }

            private void RunPhase5(ComputeCommandQueue queue)
            {
                Console.Out.WriteLine("Executing " + phase5aKernel.FunctionName + " on " + lastIdx +
                                      " GPU threads.");
                phase5aKernel.SetMemoryArgument(0, transactionsArg);
                phase5aKernel.SetMemoryArgument(1, tuple1FrequenciesArg);
                tuple2Dictionary.SetAsArgument(phase5aKernel, 2);
                tuple3Dictionary.SetAsArgument(phase5aKernel, 3);
                tuple4Dictionary.SetAsArgument(phase5aKernel, 4);
                tuple5Dictionary.SetAsArgument(phase5aKernel, 5);

                queue.Execute(phase5aKernel, new long[] { 0 }, new long[] { lastIdx }, null, null);
                queue.AddBarrier();

                Console.Out.WriteLine("Executing " + phase5bKernel.FunctionName + " on " +
                    tuple4Dictionary.MaxSize + " GPU threads.");
                tuple5Dictionary.SetAsArgument(phase5bKernel, 0);
                phase5bKernel.SetValueArgument<uint>(1, (uint)(pruningMinSupport * noTransactions));
                queue.Execute(phase5bKernel, new long[] { 0 }, new long[] { tuple5Dictionary.MaxSize }, null, null);
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


            private string constantsOpenCL =
                @"
";
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

            private string Phase2bOpenCL = PhaseNb(2);

            private string Phase3aOpenCL =
    @"
__kernel void count_3tuple_frequencies(__global /*__read_only*/  uint* transaction_items,
                                       __global /*__read_only*/  uint* frequencies_1tuple,
                                       __global /*__read_only*/  uint* frequencies_2tuple,
                                       __global /*__read_write*/ uint* frequencies_3tuple)
{
    // Vector element index: denotes item of a transaction.
    int i = get_global_id(0);
    uint key1 = transaction_items[i];
    if (key1 > 0 && frequencies_1tuple[key1] > 0) {
        int j = i + 1;
        while (transaction_items[j] > 0) {
            uint key2 = transaction_items[j];
            if (ht2_lookup(frequencies_2tuple, key1, key2) > 0) {
                int k = j + 1;
                while (transaction_items[k] > 0) {
                    uint key3 = transaction_items[k];
                    if (ht2_lookup(frequencies_2tuple, key1, key3) > 0 &&
                        ht2_lookup(frequencies_2tuple, key2, key3) > 0) {
                        ht3_add_or_increase(frequencies_3tuple, key1, key2, transaction_items[k]);
                    }
                    k++;
                }
            }
            j++;
        }
    }
}";

            private string Phase3bOpenCL = PhaseNb(3);

            private string Phase4aOpenCL =
@"
__kernel void count_4tuple_frequencies(__global /*__read_only*/  uint* transaction_items,
                                       __global /*__read_only*/  uint* frequencies_1tuple,
                                       __global /*__read_only*/  uint* frequencies_2tuple,
                                       __global /*__read_only*/  uint* frequencies_3tuple,
                                       __global /*__read_write*/ uint* frequencies_4tuple)
{
    // Vector element index: denotes item of a transaction.
    int i = get_global_id(0);
    uint key1 = transaction_items[i];
    if (key1 > 0 && frequencies_1tuple[key1] > 0) {
        int j = i + 1;
        while (transaction_items[j] > 0) {
            uint key2 = transaction_items[j];
            if (ht2_lookup(frequencies_2tuple, key1, key2) > 0) {
                int k = j + 1;
                while (transaction_items[k] > 0) {
                    uint key3 = transaction_items[k];
                    if (ht3_lookup(frequencies_3tuple, key1, key2, key3) > 0) {
                        int l = k + 1;
                        while (transaction_items[l] > 0) {
                            uint key4 = transaction_items[l];
                            if (ht3_lookup(frequencies_3tuple, key1, key3, key4) > 0 &&
                                ht3_lookup(frequencies_3tuple, key1, key2, key4) > 0 &&
                                ht3_lookup(frequencies_3tuple, key2, key3, key4) > 0) {
                                ht4_add_or_increase(frequencies_4tuple, key1, key2, key3, key4);
                            }
                            l++;
                        }
                    }
                    k++;
                }
            }
            j++;
        }
    }
}";

            private string Phase4bOpenCL = PhaseNb(4);

            private string Phase5aOpenCL =
@"
__kernel void count_5tuple_frequencies(__global /*__read_only*/  uint* transaction_items,
                                       __global /*__read_only*/  uint* frequencies_1tuple,
                                       __global /*__read_only*/  uint* frequencies_2tuple,
                                       __global /*__read_only*/  uint* frequencies_3tuple,
                                       __global /*__read_only*/  uint* frequencies_4tuple,
                                       __global /*__read_write*/ uint* frequencies_5tuple)
{
    // Vector element index: denotes item of a transaction.
    int i = get_global_id(0);
    uint key1 = transaction_items[i];
    if (key1 > 0 && frequencies_1tuple[key1] > 0) {
        int j = i + 1;
        while (transaction_items[j] > 0) {
            uint key2 = transaction_items[j];
            if (ht2_lookup(frequencies_2tuple, key1, key2) > 0) {
                int k = j + 1;
                while (transaction_items[k] > 0) {
                    uint key3 = transaction_items[k];
                    if (ht3_lookup(frequencies_3tuple, key1, key2, key3) > 0) {
                        int l = k + 1;
                        while (transaction_items[l] > 0) {
                            uint key4 = transaction_items[l];
                            if (ht4_lookup(frequencies_4tuple, key1, key2, key3, key4) > 0) {
                                int m = l + 1;
                                while (transaction_items[m] > 0) {
                                    uint key5 = transaction_items[m];
                                    if (ht4_lookup(frequencies_4tuple, key1, key3, key4, key5) > 0 &&
                                        ht4_lookup(frequencies_4tuple, key1, key2, key4, key5) > 0 &&
                                        ht4_lookup(frequencies_4tuple, key1, key2, key3, key5) > 0 &&
                                        ht4_lookup(frequencies_4tuple, key2, key3, key4, key5) > 0) {
                                        ht5_add_or_increase(frequencies_5tuple, key1, key2, key3, key4, key5);
                                    }
                                    m++;
                                }
                            }
                            l++;
                        }
                    }
                    k++;
                }
            }
            j++;
        }
    }
}";

            private string Phase5bOpenCL = PhaseNb(5);

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
                        tuple2Dictionary.Dispose();
                        tuple3Dictionary.Dispose();
                        tuple4Dictionary.Dispose();
                        tuple5Dictionary.Dispose();
                        phase1aKernel.Dispose();
                        phase1bKernel.Dispose();
                        phase2aKernel.Dispose();
                        phase2bKernel.Dispose();
                        phase3aKernel.Dispose();
                        phase3bKernel.Dispose();
                        phase4aKernel.Dispose();
                        phase4bKernel.Dispose();
                        phase5aKernel.Dispose();
                        phase5bKernel.Dispose();
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
