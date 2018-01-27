using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Standard;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using MiniDiagnostics.Properties;

namespace MiniDiagnostics
{
    public static class Helper
    {

        public static string GetHklmValue(string path, string key)
        {
            using (var rKey = Registry.LocalMachine.OpenSubKey(path, RegistryKeyPermissionCheck.ReadSubTree))
            {
                return (string)rKey?.GetValue(key);
            }
        }

        internal static string GetHklmValues(RegistryKey subKey)
        {
            var stb = new StringBuilder();
            if (subKey != null)
                foreach (var valueName in subKey.GetValueNames())
                {
                    stb.AppendLine(subKey.GetValueKind(valueName) == RegistryValueKind.Binary
                        ? $"{valueName}: {string.Join("", (byte[])subKey.GetValue(valueName))}"
                        : $"{valueName}: {subKey.GetValue(valueName)}");
                }

            return stb.ToString();
        }

        internal static string ReadHklmSubTree(string path)
        {
            var stb = new StringBuilder();
            using (var rKey = Registry.LocalMachine.OpenSubKey(path, RegistryKeyPermissionCheck.ReadSubTree))
            {
                if (rKey == null)
                {
                    return null;
                }

                if (rKey.ValueCount != 0)
                {
                    stb.Append(GetHklmValues(rKey));
                }

                if (rKey.SubKeyCount != 0)
                {
                    foreach (var subKeyName in rKey.GetSubKeyNames())
                    {
                        stb.Append(ReadHklmSubTree(Path.Combine(path, subKeyName)));
                    }
                }
            }

            return stb.ToString();
        }

        /// <summary>
        ///     Get executing assembly globally unique identifier.
        /// </summary>
        public static string ExecutingAssemblyGuid
            =>
                ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0])
                .Value;

        public static string ExecutingAssemblyName
        {
            get
            {
                string result;
                try
                {
                    result = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = AppDomain.CurrentDomain.FriendlyName;
                    }
                }
                catch (Exception ex) when (ex is ArgumentException || ex is AppDomainUnloadedException ||
                                           ex is NotSupportedException)
                {
                    result = Settings.Default.InfoUnavailable;
                }

                return result;
            }
        }

        public static string OsNameFromRegistry
        {
            get
            {
                string result = Settings.Default.InfoUnavailable;
                try
                {
                    result = GetHklmValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
                    if (!string.IsNullOrEmpty(result))
                    {
                        var csdVersion = GetHklmValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
                        return string.Concat(
                            result.StartsWith("Microsoft") ? string.Empty : "Microsoft ",
                            result,
                            string.IsNullOrEmpty(csdVersion) ? string.Empty : string.Concat(" ", csdVersion));
                    }
                }
                catch (Exception ex) when (ex is ObjectDisposedException || ex is SecurityException ||
                                           ex is UnauthorizedAccessException)
                {
                    return Settings.Default.InfoUnavailable;
                }

                return result;
            }
        }

        internal static string GetAllPerformanceCountersFor(string Category, string InstanceName = null)
        {
            var stb = new StringBuilder();
            var performanceCounterCategories = PerformanceCounterCategory.GetCategories().FirstOrDefault(cat => cat.CategoryName == Category);
            PerformanceCounter[] counters = null;
            if (InstanceName != null)
            {
                counters = performanceCounterCategories.GetCounters(InstanceName);
            }
            else
            {
                counters = performanceCounterCategories.GetCounters();
            }
            stb.AppendLine($"Displaying performance counters for {Category} category:\n");
            foreach (PerformanceCounter performanceCounter in counters)
            {
                stb.AppendLine(performanceCounter.CounterName);
            }

            return stb.ToString();
        }
        /// <summary>
        ///     Try fetching operating system (OS) name from various locations until attempts fail or possibilities exhaust.
        /// </summary>
        public static string OsName
        {
            get
            {
                string result;
                try
                {
                    // Try fetching from registry...
                    result = OsNameFromRegistry;
                }
                catch (Exception)
                {
                    result = Settings.Default.InfoUnavailable;
                }

                if (result != Settings.Default.InfoUnavailable)
                    return result;

                // Try fetching from VB.ComputerInfo...
                try
                {
                    result = Performance.ComputerInfo.Value.OSFullName;
                }
                catch (SecurityException)
                {
                    result = Settings.Default.InfoUnavailable;
                }

                if (result != Settings.Default.InfoUnavailable)
                    return result;

                // Try fetching from Environment...
                try
                {
                    result = Environment.OSVersion.VersionString;
                }
                catch (InvalidOperationException)
                {
                    result = Settings.Default.InfoUnavailable;
                }

                if (result != Settings.Default.InfoUnavailable)
                    return result;
                // Give up..
                return Settings.Default.InfoUnavailable;
            }
        }

        public static string ProcessorName
        {
            get
            {
                try
                {
                    return GetHklmValue(@"Hardware\Description\System\CentralProcessor\0", "ProcessorNameString");
                }
                catch (Exception)
                {
                    return Settings.Default.InfoUnavailable;
                }
            }
        }

        public static IEnumerable<string> DrivesInfo
        {
            get
            {
                var drives = new List<string>();
                foreach (var driveInfo in DriveInfo.GetDrives())
                {
                    var infoBuilder = new StringBuilder();
                    infoBuilder.AppendFormat("Drive: {0}{1}\t\t DriveType: {2}{1}",
                        driveInfo.Name, Environment.NewLine, driveInfo.DriveType);
                    if (driveInfo.IsReady)
                    {
                        infoBuilder.AppendFormat("\t\t Format: {0}{1}\t\t Size: {2}{1}\t\t Free space: {3}{1}",
                            driveInfo.DriveFormat, Environment.NewLine, driveInfo.TotalSize.AsMemory(),
                            driveInfo.AvailableFreeSpace.AsMemory());
                    }

                    drives.Add(infoBuilder.ToString());
                }

                return drives;
            }
        }

        public static TimeSpan TimeFromStartupEnvironment =>
            TimeSpan.FromMilliseconds(Environment.TickCount & int.MaxValue);


        public static string ThreadPoolStatus
        {
            get
            {
                ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
                ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
                ThreadPool.GetAvailableThreads(out var avWorkerThreads, out var avCompletionPortThreads);
                return
                    $"Thread pool with minimum of {minWorkerThreads} threads ({minCompletionPortThreads} CompletionPortThreads) and maximum of {maxWorkerThreads} threads ({maxCompletionPortThreads} CompletionPortThreads), of which {avWorkerThreads} are available ({avCompletionPortThreads} CompletionPortThreads)";
            }
        }

        /// <summary>
        ///     Returns true if current user is elevated admin. See http://stackoverflow.com/a/1089061/3922292 for reference.
        /// </summary>
        /// <returns></returns>
        public static bool IsAdminElevated()
        {
            bool isAdmin;
            WindowsIdentity user = null;
            try
            {
                user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            finally
            {
                user?.Dispose();
            }

            return isAdmin;
        }
    }
}

