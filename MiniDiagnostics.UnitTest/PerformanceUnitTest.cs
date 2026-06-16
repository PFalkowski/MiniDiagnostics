using System;
using System.Threading.Tasks;
using Extensions.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiniDiagnostics.UnitTest
{
    [TestClass]
    public class PerformanceUnitTest
    {
        [TestMethod]
        public void PerformanceCounterForProcessorIsLazilyCreated()
        {
            _ = Performance.CpuCurrentUsage();
            _ = Performance.CpuCurrentUsage();
        }

        [TestMethod]
        public void PerformanceCounterForRamIsLazilyCreated()
        {
            _ = Performance.RamFreeB().AsMemory();
            _ = Performance.RamFreeB().AsMemory();
        }

        [TestMethod]
        public void RamFreeUnitConversionsAreCongruent()
        {
            const double delta = 1024.0 * 1024 * 1024;
            var freeB = (double)Performance.RamFreeB();
            Assert.AreEqual(freeB / 1024, (double)Performance.RamFreeKb(), delta);
            Assert.AreEqual(freeB / (1024 * 1024), (double)Performance.RamFreeMb(), delta);
            Assert.AreEqual(freeB / (1024 * 1024 * 1024), (double)Performance.RamFreeGb(), delta);
        }

        [TestMethod]
        public void RamUsedUnitConversionsAreCongruent()
        {
            const double delta = 1024.0 * 1024 * 1024;
            var usedB = (double)Performance.RamUsedB();
            Assert.AreEqual(usedB / 1024, (double)Performance.RamUsedKb(), delta);
            Assert.AreEqual(usedB / (1024 * 1024), (double)Performance.RamUsedMb(), delta);
            Assert.AreEqual(usedB / (1024 * 1024 * 1024), (double)Performance.RamUsedGb(), delta);
        }

        [TestMethod]
        public void VirtualMemoryFreeUnitConversionsAreCongruent()
        {
            const double delta = 1024.0 * 1024 * 1024;
            var freeB = (double)Performance.VirtualFreeB;
            Assert.AreEqual(freeB / 1024, (double)Performance.VirtualFreeKb, delta);
            Assert.AreEqual(freeB / (1024 * 1024), (double)Performance.VirtualFreeMb, delta);
            Assert.AreEqual(freeB / (1024 * 1024 * 1024), (double)Performance.VirtualFreeGb, delta);
        }

        [TestMethod]
        public void VirtualMemoryUsedUnitConversionsAreCongruent()
        {
            const double delta = 1024.0 * 1024 * 1024;
            var usedB = (double)Performance.VirtualUsedB;
            Assert.AreEqual(usedB / 1024, (double)Performance.VirtualUsedKb, delta);
            Assert.AreEqual(usedB / (1024 * 1024), (double)Performance.VirtualUsedMb, delta);
            Assert.AreEqual(usedB / (1024 * 1024 * 1024), (double)Performance.VirtualUsedGb, delta);
        }

        [TestMethod]
        public void TotalRamKbTest()
        {
            double expected = 0;
            double received = 0;
            Parallel.Invoke(() => expected = Performance.RamTotalB / 1024.0, () => received = Performance.RamTotalKb);
            Assert.AreEqual(expected, received, 1.0);
        }

        [TestMethod]
        public void TotalRamMbTest()
        {
            var expected = Performance.RamTotalB / (1024.0 * 1024);
            var received = (double)Performance.RamTotalMb;
            Assert.AreEqual(expected, received, 1.0);
        }

        [TestMethod]
        public void TotalRamGbTest()
        {
            var expected = Performance.RamTotalB / (1024.0 * 1024 * 1024);
            var received = (double)Performance.RamTotalGb;
            Assert.AreEqual(expected, received, 0.01);
        }

        [TestMethod]
        public void RamPercentFreeReturnsValidValue()
        {
            var tested = Performance.RamPercentFree();
            Assert.IsTrue(tested.InRangeInclusive(0.0, 100.0));
        }

        [TestMethod]
        public void RamPercentOccupiedReturnsValidValue()
        {
            var tested = Performance.RamPercentUsed();
            Assert.IsTrue(tested.InRangeInclusive(0.0, 100.0));
        }

        [TestMethod]
        public void VirtualPercentFreeReturnsValidValue()
        {
            var tested = Performance.VirtualPercentFree();
            Assert.IsTrue(tested.InRangeInclusive(0.0, 100.0));
        }

        [TestMethod]
        public void VirtualPercentOccupiedReturnsValidValue()
        {
            var tested = Performance.VirtualPercentUsed();
            Assert.IsTrue(tested.InRangeInclusive(0.0, 100.0));
        }

        [TestMethod]
        public void RamTotalIsPositive()
        {
            Assert.IsTrue(Performance.RamTotalB > 0);
        }

        [TestMethod]
        public void VirtualTotalIsPositive()
        {
            Assert.IsTrue(Performance.VirtualTotalB > 0);
        }
    }
}
