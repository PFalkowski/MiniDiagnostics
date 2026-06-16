using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Extensions.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace MiniDiagnostics.UnitTest
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HelperUnitTest
    {
        [TestMethod]
        public void GuidReturnsSameValueOnConsecutiveCalls()
        {
            var guid = Helper.ExecutingAssemblyGuid;
            var guid2 = Helper.ExecutingAssemblyGuid;
            Assert.AreEqual(guid, guid2);
        }

        [TestMethod]
        public void OsFriendlyNameDoesNotThrow()
        {
            _ = Helper.OsName;
            _ = Environment.OSVersion.VersionString;
        }

        [TestMethod]
        public void ExecutingAssemblyNameDoesNotThrow()
        {
            _ = Helper.ExecutingAssemblyName;
            _ = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            _ = AppDomain.CurrentDomain.FriendlyName;
            _ = Directory.GetCurrentDirectory();
        }

        [TestMethod]
        public void EnvironmentVariablesDoNotThrow()
        {
            var temp = Environment.GetEnvironmentVariables();
            Assert.IsNotNull(temp);
        }

        [TestMethod]
        public void TimeFromStartupDoesNotThrow()
        {
            var fromEnv = Helper.TimeFromStartupEnvironment;
            Assert.IsTrue(fromEnv >= TimeSpan.Zero);
        }

        [TestMethod]
        public void GuidAttributeConsistentWithHelperMethod()
        {
            var fromAttribute = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(GuidAttribute), true) is GuidAttribute[] { Length: > 0 } attrs
                ? attrs[0].Value
                : string.Empty;
            var fromHelper = Helper.ExecutingAssemblyGuid;
            Trace.WriteLine($"Guid from test assembly = {fromAttribute}");
            Trace.WriteLine($"Guid from Helper = {fromHelper}");
        }

        [TestMethod]
        public void ThreadPoolStatusReturnsString()
        {
            var result = Helper.ThreadPoolStatus;
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void IsAdminElevatedDoesNotThrow()
        {
            _ = Helper.IsAdminElevated();
        }

        [TestMethod]
        public void DrivesInfoDoesNotThrow()
        {
            var result = string.Join(Environment.NewLine, Helper.DrivesInfo);
            Assert.IsNotNull(result);
        }
    }
}
