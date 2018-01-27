using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Standard;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiniDiagnostics.UnitTest
{
    [TestClass]
    public class PerformanceUnitTest
    {
        [TestMethod]
        public void PerformanceCounterForProcessorIsLazilyCreated()
        {
            var read1 = Performance.CpuCurrentUsage();
            var read2 = Performance.CpuCurrentUsage();
        }
        [TestMethod]
        public void PerformanceCounterForRamIsLazilyCreated()
        {
            var read1 = Performance.RamFreeB().AsMemory();
            var read2 = Performance.RamFreeB().AsMemory();
        }
        [TestMethod]
        public void RamFreePerformanceCountersReturnCongruentValues()
        {
            const float delta = 1024 * 1024 * 1024;
            var info = new ComputerInfo();
            var expected = info.AvailablePhysicalMemory;

            var read1 = Performance.RamFreeB();
            var read2 = Performance.RamFreeKb() * 1024;
            var read3 = Performance.RamFreeMb() * 1024 * 1024;
            var read4 = Performance.RamFreeGb() * 1024 * 1024 * 1024;

            Assert.AreEqual(expected, read1, delta);
            Assert.AreEqual(expected, read2, delta);
            Assert.AreEqual(expected, read3, delta);
            Assert.AreEqual(expected, read4, delta);
        }
        [TestMethod]
        public void RamOccupiedPerformanceCountersReturnCongruentValues()
        {
            const float delta = 1024 * 1024 * 1024;
            var info = new ComputerInfo();
            var expected = info.TotalPhysicalMemory - info.AvailablePhysicalMemory;
            var read1 = Performance.RamUsedB();
            var read2 = Performance.RamUsedKb() * 1024;
            var read3 = Performance.RamUsedMb() * 1024 * 1024;
            var read4 = Performance.RamUsedGb() * 1024 * 1024 * 1024;

            Assert.AreEqual(expected, read1, delta);
            Assert.AreEqual(expected, read2, delta);
            Assert.AreEqual(expected, read3, delta);
            Assert.AreEqual(expected, read4, delta);
        }
        [TestMethod]
        public void VirtualMemoryFreeReturnCongruentValues()
        {
            const float delta = 1024 * 1024 * 1024;
            var info = new ComputerInfo();
            var expected = info.AvailableVirtualMemory;
            var read1 = Performance.VirtualFreeB;
            var read2 = Performance.VirtualFreeKb * 1024;
            var read3 = Performance.VirtualFreeMb * 1024 * 1024;
            var read4 = Performance.VirtualFreeGb * 1024 * 1024 * 1024;

            Assert.AreEqual(expected, read1, delta);
            Assert.AreEqual(expected, read2, delta);
            Assert.AreEqual(expected, read3, delta);
            Assert.AreEqual(expected, read4, delta);
        }
        [TestMethod]
        public void VirtualMemoryUsedReturnCongruentValues()
        {
            const float delta = 1024 * 1024 * 1024;
            var info = new ComputerInfo();
            var expected = info.TotalVirtualMemory - info.AvailableVirtualMemory;
            var read1 = Performance.VirtualUsedB;
            var read2 = Performance.VirtualUsedKb * 1024;
            var read3 = Performance.VirtualUsedMb * 1024 * 1024;
            var read4 = Performance.VirtualUsedGb * 1024 * 1024 * 1024;
            //Trace.WriteLine($"Expected {expected.AsMemory()} received {read1.AsMemory()}");
            Assert.AreEqual(expected, read1, delta);
            Assert.AreEqual(expected, read2, delta);
            Assert.AreEqual(expected, read3, delta);
            Assert.AreEqual(expected, read4, delta);
        }

        [TestMethod]
        public void TotalRamKbTest()
        {
            double expected = 0;
            double received = 0;
            Parallel.Invoke(() => expected = Performance.RamTotalB / 1024.0, () => received = Performance.RamTotalKb);
            Assert.AreEqual(expected, received, double.Epsilon);
        }
        [TestMethod]
        public void TotalRamMbTest()
        {
            var expected = Performance.RamTotalB / (1024.0 * 1024);
            var received = Performance.RamTotalMb;
            Assert.AreEqual(expected, received, double.Epsilon);
        }
        [TestMethod]
        public void TotalRamGbTest()
        {
            var expected = Performance.RamTotalB / (1024.0 * 1024 * 1024);
            var received = Performance.RamTotalGb;
            Assert.AreEqual(expected, received, double.Epsilon);
        }

        //[TestMethod]
        //public void GetAllPerformanceCountersTest()
        //{
        //    const string Category = "Processor";
        //    const string InstanceName = "_Total";
        //    var result = Helper.GetAllPerformanceCountersFor(Category, InstanceName);
        //}

        [TestMethod]
        public void RamPercentFreeReturnsValidValue()
        {
            var tested = Performance.RamPercentFree();
            Assert.IsTrue(tested.InOpenRange(0.0, 100.0));
        }
        [TestMethod]
        public void RamPercentOccupiedReturnsValidValue()
        {
            var tested = Performance.RamPercentUsed();
            Assert.IsTrue(tested.InOpenRange(0.0, 100.0));
        }
        [TestMethod]
        public void VirtualPercentFreeReturnsValidValue()
        {
            var tested = Performance.VirtualPercentFree();
            Assert.IsTrue(tested.InOpenRange(0.0, 100.0));
        }
        [TestMethod]
        public void VirtualPercentOccupiedReturnsValidValue()
        {
            var tested = Performance.VirtualPercentUsed();
            Assert.IsTrue(tested.InOpenRange(0.0, 100.0));
        }

    }
}
