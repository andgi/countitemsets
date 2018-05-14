using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            IDictionary<long, int> result1tuples = unitUnderTest.Get1TupleResult(computeCmdQueue);
            CollectionAssert.AreEquivalent(reader.AllEANs.ToArray(), result1tuples.Keys.ToArray());
            CollectionAssert.AreEquivalent(reader.EANFrequencies.ToArray(), result1tuples.ToArray());
            IDictionary<string, int> result2tuples = unitUnderTest.GetNTupleResult(2, computeCmdQueue);
            CollectionAssert.AreEquivalent(reader.EAN2TupleFrequencies.ToArray(), result2tuples.ToArray());
            IDictionary<string, int> result3tuples = unitUnderTest.GetNTupleResult(3, computeCmdQueue);
            CollectionAssert.AreEquivalent(reader.EAN3TupleFrequencies.ToArray(), result3tuples.ToArray());
        }

        private GPUTransactionFrequentItemsetGenerator.GPUAlgorithm unitUnderTest;
        private FakeTransactionReader reader;
    }
}
