using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Cloo;
using CountItemSets;

namespace OpenCLTests
{
    [TestClass]
    public class OpenCLHashTableTests : AbstractOpenCLTest
    {
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
            Console.Out.WriteLine(unitUnderTest.SourceOpenCL + OpenCLcodeInsert);
            computeProgram = new ComputeProgram(computeContext,
                                                unitUnderTest.SourceOpenCL +
                                                OpenCLcodeInsert + OpenCLcodeLookup);
            try {
                computeProgram.Build(null, null, null, IntPtr.Zero);
            } catch (Exception e) {
                Console.Out.WriteLine(e);
                throw;
            }
            computeKernelInsert = computeProgram.CreateKernel("insert_tuples");
            computeKernelLookup = computeProgram.CreateKernel("lookup_tuples");
        }

        [TestMethod]
        public void OCLHT_CanRunInsert()
        {
            OCLHT_CanCompile();

            unitUnderTest.SetAsArgument(computeKernelInsert, 0);
            Console.Out.WriteLine("Executing " + computeKernelInsert.FunctionName + " on " +
                                  NO_THREADS + " GPU threads.");
            computeCmdQueue.Execute(computeKernelInsert, new long[] { 0 }, new long[] { NO_THREADS }, null, null);
            computeCmdQueue.Finish();
        }

        [TestMethod]
        public void OCLHT_CanRunLookup()
        {
            OCLHT_CanRunInsert();
            unitUnderTest.SetAsArgument(computeKernelLookup, 0);
            Console.Out.WriteLine("Executing " + computeKernelLookup.FunctionName + " on " +
                                  MAX_SIZE + " GPU threads.");
            computeCmdQueue.Execute(computeKernelLookup, new long[] { 0 }, new long[] { MAX_SIZE }, null, null);
            computeCmdQueue.Finish();
        }

        [TestMethod]
        public void OCLHT_CanReadFilled()
        {
            OCLHT_CanRunLookup();
            IDictionary<string, int> dict = unitUnderTest.AsDictionary(computeCmdQueue,
                                                                       GetTranslation(NO_THREADS));
            Assert.IsNotNull(dict);
            Assert.AreEqual(MAX_SIZE, dict.Count); // We filled it completely..
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

        private string OpenCLcodeInsert {
            get {
                string code = @"
__kernel void insert_tuples(__global /*__read_write*/ uint* hashtable)
{
    // Vector element index: denotes keys of an entry.
    uint idx = get_global_id(0) % " + MAX_SIZE + @" + 1;
    " + unitUnderTest.AddOrIncreaseFunctionName + "(hashtable";
                for (int i = 0; i < TUPLE_SIZE; i++) {
                    code += ", 1";
                }
                code += @");
    " + unitUnderTest.AddOrIncreaseFunctionName + "(hashtable";
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

        private string OpenCLcodeLookup
        {
            get
            {
                string code = @"
__kernel void lookup_tuples(__global /*__read_write*/ uint* hashtable)
{
    // Vector element index: denotes keys of an entry.
    uint idx = get_global_id(0) % " + MAX_SIZE + @" + 1;
    uint count = " + unitUnderTest.LookupFunctionName + "(hashtable";
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
        private const int MAX_SIZE = 1 * 1024 * 1024;
        private const int NO_THREADS = 2*MAX_SIZE;
        private OpenCLHashTable unitUnderTest;
        private ComputeProgram computeProgram;
        private ComputeKernel computeKernelInsert;
        private ComputeKernel computeKernelLookup;
    }
}
