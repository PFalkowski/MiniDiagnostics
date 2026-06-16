using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace MiniDiagnostics
{
    public static class Performance
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private sealed class MemoryStatusEx
        {
            public uint dwLength = (uint)Marshal.SizeOf<MemoryStatusEx>();
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);

        private static MemoryStatusEx GetMemoryStatus()
        {
            var status = new MemoryStatusEx();
            GlobalMemoryStatusEx(status);
            return status;
        }

        internal static Lazy<PerformanceCounter> Cpu { get; } = new Lazy<PerformanceCounter>(
            () => new PerformanceCounter("Processor", "% Processor Time", "_Total", true),
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// Getting total RAM from WMI takes 10 times longer; PerformanceCounter "Available Bytes" is faster
        internal static Lazy<PerformanceCounter> Ram { get; } = new Lazy<PerformanceCounter>(
            () => new PerformanceCounter("Memory", "Available Bytes", true),
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>% of overall CPU time. The first readout is always zero.</summary>
        public static float CpuCurrentUsage() => Cpu.Value.NextValue();

        public static float RamUsedB() => RamTotalB - Ram.Value.NextValue();
        public static float RamUsedKb() => RamUsedB() / 1024;
        public static float RamUsedMb() => RamUsedKb() / 1024;
        public static float RamUsedGb() => RamUsedMb() / 1024;

        public static float RamFreeB() => Ram.Value.NextValue();
        public static float RamFreeKb() => RamFreeB() / 1024;
        public static float RamFreeMb() => RamFreeKb() / 1024;
        public static float RamFreeGb() => RamFreeMb() / 1024;

        /// <summary>Total physical memory seen by OS (from kernel32.dll GlobalMemoryStatusEx).</summary>
        public static ulong RamTotalB => GetMemoryStatus().ullTotalPhys;
        public static float RamTotalKb => (float)RamTotalB / 1024;
        public static float RamTotalMb => RamTotalKb / 1024;
        public static float RamTotalGb => RamTotalMb / 1024;

        public static ulong VirtualTotalB => GetMemoryStatus().ullTotalVirtual;
        public static ulong VirtualTotalKb => VirtualTotalB / 1024;
        public static ulong VirtualTotalMb => VirtualTotalKb / 1024;
        public static ulong VirtualTotalGb => VirtualTotalMb / 1024;

        public static ulong VirtualFreeB => GetMemoryStatus().ullAvailVirtual;
        public static ulong VirtualFreeKb => VirtualFreeB / 1024;
        public static ulong VirtualFreeMb => VirtualFreeKb / 1024;
        public static ulong VirtualFreeGb => VirtualFreeMb / 1024;

        public static ulong VirtualUsedB => VirtualTotalB - VirtualFreeB;
        public static ulong VirtualUsedKb => VirtualUsedB / 1024;
        public static ulong VirtualUsedMb => VirtualUsedKb / 1024;
        public static ulong VirtualUsedGb => VirtualUsedMb / 1024;

        public static double RamPercentFree() => (double)RamFreeB() / RamTotalB * 100.0;
        public static double RamPercentUsed() => 100 - RamPercentFree();

        public static double VirtualPercentFree() => (double)VirtualFreeB / VirtualTotalB * 100.0;
        public static double VirtualPercentUsed() => 100 - VirtualPercentFree();
    }
}
