using System;
using System.Collections.Generic;
using System.Linq;

using Cloo;

namespace CountItemSets
{
    public class OpenCLHashTable : IDisposable
    {
        int keyParts;
        uint[] hashtable;
        System.Runtime.InteropServices.GCHandle hashtableHandle;
        ComputeBuffer<uint> hashtableArg;
        const uint P = 4294967291;
        const uint A = 628346;
        const uint B = 94967291;
        const uint MAX_RETRIES = 8;

        public int MaxSize
        {
            get; private set;
        }

        public int MaxSizeInBytes
        {
            get {
                return  (keyParts + 1) * sizeof(uint) * MaxSize;
            }
        }

        public string SourceOpenCL
        {
            get
            {
                return SourceAddOrIncrease + SourceLookup;
            }
        }

        public string AddOrIncreaseFunctionName
        {
            get
            {
                return SourcePrefix + "add_or_increase";
            }
        }

        public string LookupFunctionName
        {
            get
            {
                return SourcePrefix + "lookup";
            }
        }

        protected string SourcePrefix
        {
            get
            {
                return "ht" + keyParts + "_";
            }
        }

        protected string SourceKeyFormalArguments
        {
            get
            {
                string args = "";
                for (int i = 1; i <= keyParts; i++)
                {
                    args += ", uint key" + i;
                }
                return args;
            }
        }

        protected string SourceKeyActualArguments
        {
            get
            {
                string args = "";
                for (int i = 1; i <= keyParts; i++)
                {
                    args += ", key" + i;
                }
                return args;
            }
        }

        protected string SourceComputeHash
        {
            get
            {
                string hash = "((((1";
                for (int i = 1; i <= keyParts; i++)
                {
                    hash += "+key" + i;
                }
                hash += ") ^ " + A + " + " + B + ") % " + P + ") % " + MaxSize + ")";
                return hash;
            }
        }

        protected string SourceProbe
        {
            get
            {
                return
                    //"idx = (idx + (" + keyParts + @" + 1)*1) % ((" + keyParts + @" + 1)*" + MaxSize + @"); // Linear probing.";
                    "idx = (idx + (" + keyParts + @" + 1) * t) % ((" + keyParts + @" + 1) * " + MaxSize + @"); // ? probing.";
            }
        }

        protected string SourceAddOrIncrease
        {
            get
            {
                return @"
void " + AddOrIncreaseFunctionName + @"(__global /*__read_write*/  uint* hashtable" +
                                        SourceKeyFormalArguments + @")
{
    int idx = (" + keyParts + @" + 1) * " + SourceComputeHash + @";
    for (int t = 1; t < " + MAX_RETRIES + @"; t++) {
        if (" + SourceIsEmpty("hashtable", "idx") + @") {
            if (" + SourceCASFirstKeyPart("hashtable", "idx") + @") {
                " + SourceStoreOtherKeyParts("hashtable", "idx") + @"
                atomic_inc(&hashtable[idx + " + keyParts + @"]);
                return;
            }
            t--;
        } else if (" + SourceIsIncomplete("hashtable", "idx") + @") {
            t--;
        } else if (" + SourceKeysMatch("hashtable", "idx") + @") {
            atomic_inc(&hashtable[idx + " + keyParts + @"]);
            return;
        } else {
            " + SourceProbe + @"
        }
    }
    /* This update was lost. Increase the counter for lost updates. */
    atomic_inc(&hashtable[(" + keyParts + " + 1)*" + MaxSize + @"]);
}";
            }
        }

        protected string SourceLookup
        {
            get
            {
                return @"
uint " + LookupFunctionName + @"(__global /*__read_write*/  uint* hashtable" +
                                 SourceKeyFormalArguments + @")
{
    int idx = (" + keyParts + @" + 1) * " + SourceComputeHash + @";
    for (int t = 1; t < " + MAX_RETRIES + @"; t++) {
        if (" + SourceIsEmpty("hashtable", "idx") + @") {
            return 0;
        } else if (" + SourceKeysMatch("hashtable", "idx") + @") {
            return hashtable[idx + " + keyParts + @"];
        } else {
            " + SourceProbe + @"
        }
    }
    return 0;
}";
            }
        }

        public OpenCLHashTable(ComputeContext context, int keyParts, int maxSize)
        {
            this.keyParts = keyParts;
            MaxSize = maxSize;
            hashtable = new uint[MaxSize * (keyParts + 1) + 1]; // One extra cell to count lost updates.
            hashtableHandle =
                System.Runtime.InteropServices.GCHandle.Alloc(hashtable,
                                                              System.Runtime.InteropServices.GCHandleType.Pinned);
            hashtableArg =
                new ComputeBuffer<uint>(context,
                                        ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer,
                                        hashtable);
            Console.Out.WriteLine("OpenCLHashTable<" + keyParts + "> Created size " + MaxSize +
                                  " entries, " + MaxSizeInBytes + " bytes.");
        }

        public void SetAsArgument(ComputeKernel kernel, int index)
        {
            kernel.SetMemoryArgument(index, hashtableArg);
        }

        public IDictionary<string, int> AsDictionary(ComputeCommandQueue queue, long[] GPUtoCPU)
        {
            queue.Read<uint>(hashtableArg,
                             true,
                             0,
                             hashtableArg.Count,
                             hashtableHandle.AddrOfPinnedObject(),
                             null);
            Dictionary<string, int> result = new Dictionary<string, int>();
            long[] CPUKeys = new long[keyParts];
            int badBuckets = 0;
            for (int i = 0; i < (keyParts + 1)*MaxSize; i += keyParts + 1)
            {
                if (hashtable[i + keyParts] > 0)
                {
                    // FIXME: Why needed?!
                    if (hashtable[i] == 0 || hashtable[i + 1] == 0 ||
                        hashtable[i] >= GPUtoCPU.Length || hashtable[i + 1] >= GPUtoCPU.Length)
                    {
                        badBuckets++;
                        continue;
                    }
                    for (int k = 0; k < keyParts; k++)
                    {
                        CPUKeys[k] = GPUtoCPU[hashtable[i + k]];
                    }
                    Array.Sort(CPUKeys);
                    string key = CPUKeys[0].ToString();
                    for (int k = 1; k < keyParts; k++)
                    {
                        key += "," + CPUKeys[k];
                    }
                    result[key] = (int)hashtable[i + keyParts];
                }
            }
            Console.Out.WriteLine("OpenCLHashTable<" + keyParts + ">: Lost updates " +
                                  hashtable[(keyParts + 1) * MaxSize] + ", " +
                                  "Bad buckets " + badBuckets + ".");
            return result;
        }

        protected string SourceCASFirstKeyPart(string hashtable, string index)
        {
            return "(0 == atomic_cmpxchg(&" + hashtable + "[" + index + "], 0, key1))";
        }

        protected string SourceStoreOtherKeyParts(string hashtable, string index)
        {
            string statements = "";
            for (int i = 1; i < keyParts; i++)
            {
                statements += hashtable + "[" + index + " + " + i + "] = key" + (i+1) + "; ";
            }
            return statements;
        }

        protected string SourceIsEmpty(string hashtable, string index)
        {
            return "(!" + hashtable + "[" + index + "])";
        }

        protected string SourceIsIncomplete(string hashtable, string index)
        {
            string condition = "(0";
            for (int i = 1; i < keyParts; i++)
            {
                condition += " || !" + hashtable + "[" + index + " + " + i + "]";
            }
            condition += ")";
            return condition;
        }

        protected string SourceKeysMatch(string hashtable, string index)
        {
            string condition = "(1";
            for (int i = 0; i < keyParts; i++)
            {
                condition += " && " + hashtable + "[" + index + " + " + i + "] == " + "key" + (i + 1);
            }
            condition += ")";
            return condition;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    hashtableHandle.Free();
                    hashtableArg.Dispose();
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
