using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Extensions.Standard;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace MiniDiagnostics.UnitTest
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HelperUnitTest
    {
        [TestMethod]
        public void Guid()
        {
            var guid = Helper.ExecutingAssemblyGuid;
            if (Properties.Settings.Default.ConsoleLog)
            {
                Trace.WriteLine($"First trial: GUID = {guid}");
            }
            var guid2 = Helper.ExecutingAssemblyGuid;
            if (Properties.Settings.Default.ConsoleLog)
            {
                Trace.WriteLine($"Second trial: GUID = {guid2}");
            }
            Assert.AreEqual(guid, guid2);
        }

        [TestMethod]
        public void OsFriendlyName()
        {
            var watch = new Stopwatch();
            watch.Start();
            // ex.: Microsoft Windows 7 Professional Service Pack 1
            var fromRegistry = Helper.OsName;
            if (Properties.Settings.Default.ConsoleLog)
                Trace.WriteLine($"Fetching from registry took {watch.ElapsedMilliseconds} ms, result = {fromRegistry}");
            watch.Restart();
            // ex.: Microsoft Windows NT 6.1.7601 Service Pack 1
            var fromEnvironment = Environment.OSVersion.VersionString;
            if (Properties.Settings.Default.ConsoleLog)
                Trace.WriteLine($"Fetching from Environment took {watch.ElapsedMilliseconds} ms, result = {fromEnvironment}");
            watch.Restart();
            // ex.: Microsoft Windows 7 Professional
            var fromComputerInfo = new ComputerInfo().OSFullName;
            if (Properties.Settings.Default.ConsoleLog)
                Trace.WriteLine($"Fetching from Diagnostics.DiagnosticTools.ComputerInfo.OSFullName took {watch.ElapsedMilliseconds} ms, result = {fromComputerInfo}");
            watch.Restart();
            // ex.: Microsoft Windows 7 Professional
            var fromNewComputerInfo = new ComputerInfo().OSFullName;
            if (Properties.Settings.Default.ConsoleLog)
                Trace.WriteLine($"Fetching from new ComputerInfo.OSFullName took {watch.ElapsedMilliseconds} ms, result = {fromNewComputerInfo}");
        }

        [TestMethod]
        public void ExecutingAssemblyNameDifferentWays()
        {
            var executingAssemblyName = Helper.ExecutingAssemblyName;
            if (Properties.Settings.Default.ConsoleLog)
            {
                Trace.WriteLine($"ExecutingAssemblyName: {executingAssemblyName}");
                Trace.WriteLine($"Path.GetFileName(Assembly.GetExecutingAssembly().Location): {Path.GetFileName(Assembly.GetExecutingAssembly().Location)}");
                Trace.WriteLine($"System.AppDomain.CurrentDomain.FriendlyName: {AppDomain.CurrentDomain.FriendlyName}");
                //Trace.WriteLine("Application.StartupPath: {0}", Application.StartupPath);// requires winforms
                //Trace.WriteLine("Application.ExecutablePath: {0}", Application.ExecutablePath);// requires winforms
                Trace.WriteLine($"System.IO.Directory.GetCurrentDirectory(): {Directory.GetCurrentDirectory()}");
            }
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            if (Properties.Settings.Default.ConsoleLog)
                Trace.WriteLine($"Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path): {Path.GetDirectoryName(path)}");
            //Trace.WriteLine("new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath: {0}", (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
        }

        [TestMethod]
        public void ExpandAllEnvironmentVariablesTime()
        {
            var temp = Environment.GetEnvironmentVariables();
            if (Properties.Settings.Default.ConsoleLog)
            {
                foreach (DictionaryEntry item in temp)
                {
                    Trace.WriteLine($"{item.Key}: {item.Value}");
                }
            }
        }

        [TestMethod]
        public void TimeFromStartup()
        {
            var watch = new Stopwatch();
            watch.Start();
            var fromEnv = Helper.TimeFromStartupEnvironment;
            if (Properties.Settings.Default.ConsoleLog)
                Trace.WriteLine($"Fetching time from env took { watch.ElapsedMilliseconds} ms, result = {fromEnv.AsTime()}");
        }

        [TestMethod]
        public void GuidTest()
        {
            var theGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;
            var theGuidDiagnostics = Helper.ExecutingAssemblyGuid;
            if (Properties.Settings.Default.ConsoleLog)
            {
                Trace.WriteLine($"Guid from test location = {theGuid}");
                Trace.WriteLine($"Guid from diagnostics = {theGuidDiagnostics}");
            }
        }

        [TestMethod]
        public void ThreadPoolStatusReturnsString()
        {
            var result = Helper.ThreadPoolStatus;

        }

        [TestMethod]
        public void IsAdminElevatedTest()
        {
            var result = Helper.IsAdminElevated();

        }

        [TestMethod]
        public void DrivesInfoTest()
        {
            var result = string.Join(Environment.NewLine, Helper.DrivesInfo);

        }
    }
}
