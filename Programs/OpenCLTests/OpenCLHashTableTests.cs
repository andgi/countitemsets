using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Cloo;
using CountItemSets;

namespace OpenCLTests
{
    [TestClass]
    public class OpenCLHashTableTests
    {
        public OpenCLHashTableTests()
        {
            SetUpOpenCL();
        }

        public void SetUpOpenCL()
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
                computeContext = new ComputeContext(new ComputeDevice[] { gpu },
                                             new ComputeContextPropertyList(platform), null, IntPtr.Zero);
                computeCmdQueue = new ComputeCommandQueue(computeContext, gpu, ComputeCommandQueueFlags.None);
                Console.Out.WriteLine("Selected OpenCL platform: " + computeContext.Platform.Name + " and device: " + gpu.Name + ".");
            }
            else
            {
                computeContext = null;
                computeCmdQueue = null;
            }
            Console.Out.Flush();
        }

        [TestMethod]
        public void OCLHT_CanCreate()
        {
            unitUnderTest = new OpenCLHashTable(computeContext, TUPLE_SIZE, MAX_SIZE);
            Assert.IsNotNull(unitUnderTest);
        }

        [TestMethod]
        public void OCLHT_CanReadEmpty()
        {
            OCLHT_CanCreate();
            IDictionary<string, int> dict = unitUnderTest.AsDictionary(computeCmdQueue, new long[0]);
            Assert.IsNotNull(dict);
            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void OCLHT_CanCompile()
        {
            OCLHT_CanCreate();
            Console.Out.WriteLine(unitUnderTest.SourceOpenCL + OpenCLcode);
            computeProgram = new ComputeProgram(computeContext,
                                                unitUnderTest.SourceOpenCL + OpenCLcode);
            try {
                computeProgram.Build(null, null, null, IntPtr.Zero);
            } catch (Exception e) {
                Console.Out.WriteLine(e);
                throw;
            }
            computeKernel = computeProgram.CreateKernel("insert_tuples");
        }

        [TestMethod]
        public void OCLHT_CanRun()
        {
            OCLHT_CanCompile();

            unitUnderTest.SetAsArgument(computeKernel, 0);
            Console.Out.WriteLine("Executing " + computeKernel.FunctionName + " on " +
                                  NO_THREADS + " GPU threads.");
            computeCmdQueue.Execute(computeKernel, new long[] { 0 }, new long[] { NO_THREADS }, null, null);
            computeCmdQueue.Finish();
        }

        [TestMethod]
        public void OCLHT_CanReadFilled()
        {
            OCLHT_CanRun();
            IDictionary<string, int> dict = unitUnderTest.AsDictionary(computeCmdQueue,
                                                                       GetTranslation(NO_THREADS));
            Assert.IsNotNull(dict);
            Assert.AreEqual(MAX_SIZE, dict.Count); // We fill it completely..
            for (int t = 1; t <= MAX_SIZE; t++) {
                string key = t.ToString();
                for (int k = 1; k < TUPLE_SIZE; k++) {
                    key += "," + t;
                }
                Assert.AreEqual(dict[key], NO_THREADS / MAX_SIZE + (t == 1 ? NO_THREADS : 0));
            }
        }

        private long[] GetTranslation(int length)
        {
            long[] translation = new long[length + 1];
            for (int i = 0; i < translation.Length; i++) {
                translation[i] = i;
            }
            return translation;
        }

        private string OpenCLcode {
            get {
                string code = @"
__kernel void insert_tuples(__global /*__read_write*/ uint* hashtable)
{
    // Vector element index: denotes keys of an entry.
    uint idx = get_global_id(0) % " + MAX_SIZE + @" + 1;
    ht" + TUPLE_SIZE + @"_add_or_increase(hashtable";
                for (int i = 0; i < TUPLE_SIZE; i++) {
                    code += ", 1";
                }
                code += @");
    ht" + TUPLE_SIZE + @"_add_or_increase(hashtable";
                for (int i = 0; i < TUPLE_SIZE; i++)
                {
                    code += ", idx";
                }
                code += @");
}
";
                return code;
            }
        }

        private const int TUPLE_SIZE = 2;
        private const int MAX_SIZE = 4 * 1024 * 1024;
        private const int NO_THREADS = 2*MAX_SIZE;
        private ComputeContext computeContext;
        private ComputeCommandQueue computeCmdQueue;
        private OpenCLHashTable unitUnderTest;
        private ComputeProgram computeProgram;
        private ComputeKernel computeKernel;
    }
}
