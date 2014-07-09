using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FIFA.Framework.Test;
using FIFA.Framework.Analysis;
using FIFA.Test;
using FIFA.Analysis;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class Test
    {
        string target_path = @"C:\Users\Administrator\Desktop\coding_space\FIFA\TestTarget\bin\Debug\TestTarget.dll";
        [TestMethod]
        public void TestDiscoverer()
        {
            TestDiscoverer discoverer = new TestDiscoverer();
            discoverer.LoadDomain();
            List<TestCase> test_case_list = discoverer.Proxy.GetTestCases(target_path);
            discoverer.UnloadDomain();
        }

        [TestMethod]
        public void TestExecutor()
        {
            TestDiscoverer discoverer = new TestDiscoverer();
            discoverer.LoadDomain();
            List<TestCase> test_case_list = discoverer.Proxy.GetTestCases(target_path);
            discoverer.UnloadDomain();

            


            TestSetting setting = new TestSetting();
            string file_path = Environment.GetEnvironmentVariable("PROGRAMFILES");
            setting.InstrumentTool = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\vsinstr.exe";
            setting.PerformanceCmder = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VSPerfCmd.exe";
            setting.IsDebugging = false;
            setting.CoverageStoreDirectory = @".";
            TestExecutor executor = new TestExecutor(setting);
            executor.Initialize(test_case_list);
            executor.Execute();
            executor.Finish();
        }

        [TestMethod]
        public void TestAnalysis()
        {
            TestDiscoverer discoverer = new TestDiscoverer();
            discoverer.LoadDomain();
            List<TestCase> test_case_list = discoverer.Proxy.GetTestCases(target_path);
            discoverer.UnloadDomain();




            TestSetting setting = new TestSetting();
            string file_path = Environment.GetEnvironmentVariable("PROGRAMFILES");
            setting.InstrumentTool = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\vsinstr.exe";
            setting.PerformanceCmder = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VSPerfCmd.exe";
            setting.IsDebugging = false;
            setting.CoverageStoreDirectory = @".";
            TestExecutor executor = new TestExecutor(setting);
            executor.Initialize(test_case_list);
            executor.Execute();
            LocatorSetting locator_setting = new LocatorSetting();
            locator_setting.Method = FLMethod.ochiai;
            FaultLocator locator = new FaultLocator(locator_setting);
            List<BasicBlock> bb_list = locator.GetRankList(executor.Results);
            foreach(var bb in bb_list)
            {
                Console.WriteLine(bb);
            }
            executor.Finish();
        }
    }
}
