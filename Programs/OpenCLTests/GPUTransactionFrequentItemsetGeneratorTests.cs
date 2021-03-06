﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using Cloo;
using CountItemSets;

namespace OpenCLTests
{
    [TestClass]
    public class GPUTransactionFrequentItemsetGeneratorTests : AbstractOpenCLTest
    {
        [TestMethod]
        public void GPUAlg_CanCreate()
        {
            reader = new FakeTransactionReader();
            unitUnderTest =
                new GPUTransactionFrequentItemsetGenerator.GPUAlgorithm(computeContext,
                                                                        reader,
                                                                        0.0);
            Assert.IsNotNull(unitUnderTest);
        }

        [TestMethod]
        public void GPUAlg_CanRun()
        {
            GPUAlg_CanCreate();
            reader.Begin(); // Must reset the ITransactionReader!
            unitUnderTest.Run(computeCmdQueue, reader);

            Assert.AreEqual<int>(reader.NoTransactions,
                                 unitUnderTest.GetNoTransactions());
            System.Console.WriteLine("The number of transactions are correct.");

            IDictionary<long, int> result1tuples = unitUnderTest.Get1TupleResult(computeCmdQueue);
            CollectionAssert.AreEquivalent(reader.AllEANs.ToArray(),
                                           result1tuples.Keys.ToArray(),
                                           " The set of EANs is wrong.");
            CollectionAssert.AreEquivalent(reader.EANFrequencies.ToArray(),
                                           result1tuples.ToArray(),
                                           " The set of EAN 1-tuple frequencies is wrong.");
            System.Console.WriteLine("The set of EAN 1-tuple frequencies is correct.");

            IDictionary<string, int> result2tuples = unitUnderTest.GetNTupleResult(2, computeCmdQueue);
            var difference2tuple = reader.EAN2TupleFrequencies.Where(kv => !result2tuples.Keys.Contains(kv.Key));
            System.Console.WriteLine("The set of EAN 2-tuple missing keys:");
            foreach (var kv in difference2tuple) {
                System.Console.WriteLine("  Missing in result2tuples: " + kv.Key + ": " + kv.Value);
            }
            System.Console.WriteLine("The set of all EAN 2-tuples from the TransactionReader:");
            foreach (var kv in reader.EAN2TupleFrequencies.OrderBy(kv => uint.Parse(kv.Key.Split(',')[0]))) {
                int fromResult;
                result2tuples.TryGetValue(kv.Key, out fromResult);
                System.Console.WriteLine("  EAN2TupleFrequencies[" + kv.Key + "]: " + kv.Value +
                                         " In result2tuples[" + kv.Key + "]: " + fromResult);
            }
            CollectionAssert.AreEquivalent(reader.EAN2TupleFrequencies.ToArray(),
                                           result2tuples.ToArray(),
                                           " The set of EAN 2-tuple frequencies is wrong.");
            System.Console.WriteLine("The set of EAN 2-tuple frequencies is correct.");

            IDictionary<string, int> result3tuples = unitUnderTest.GetNTupleResult(3, computeCmdQueue);
            var difference3tuple = reader.EAN3TupleFrequencies.Where(kv => !result3tuples.Keys.Contains(kv.Key));
            System.Console.WriteLine("The set of EAN 3-tuple missing keys:");
            foreach (var kv in difference3tuple) {
                System.Console.WriteLine("  Missing in result3tuples: " + kv.Key + ": " + kv.Value);
            }
            System.Console.WriteLine("The set of all EAN 3-tuples from the TransactionReader:");
            foreach (var kv in reader.EAN3TupleFrequencies.OrderBy(kv => uint.Parse(kv.Key.Split(',')[0]))) {
                int fromResult;
                result3tuples.TryGetValue(kv.Key, out fromResult);
                System.Console.WriteLine("  EAN3TupleFrequencies[" + kv.Key + "]: " + kv.Value +
                                         " In result3tuples[" + kv.Key + "]: " + fromResult);
            }
            CollectionAssert.AreEquivalent(reader.EAN3TupleFrequencies.ToArray(),
                                           result3tuples.ToArray(),
                                           " The set of EAN 3-tuple frequencies is wrong.");
            System.Console.WriteLine("The set of EAN 3-tuple frequencies is correct.");
        }

        private GPUTransactionFrequentItemsetGenerator.GPUAlgorithm unitUnderTest;
        private FakeTransactionReader reader;
    }
}
