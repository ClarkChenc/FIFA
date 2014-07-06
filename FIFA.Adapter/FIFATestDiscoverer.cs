using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Reflection;
using System.IO;
using FIFA.Test;
namespace FIFATestAdapter
{
    [DefaultExecutorUri(Constant.ExecutorUriString)]
    public class FIFATestDiscoverer:ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var tests = GetTests(sources, logger);
            foreach (var tc in tests)
            {
                discoverySink.SendTestCase(tc);
            }
            FLGlobalService.SendMessage("clear");
        }


        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, IMessageLogger logger)
        {

            var tests = new List<TestCase>();
            TestDiscoverer discoverer = new TestDiscoverer();
            discoverer.LoadDomain();
            foreach(var source in sources)
            {
                IEnumerable<FIFA.Framework.Test.TestCase> fifa_tc_list = discoverer.Proxy.GetTestCases(source);
                foreach (var fifa_tc in fifa_tc_list)
                {
                    TestCase tc = new TestCase(
                        fifa_tc.ModuleName + "#" 
                        + fifa_tc.TypeFullName + "#" 
                        + fifa_tc.MethodName + "#" 
                        + fifa_tc.AttributeIndex,
                        FIFATestExecutor.ExecutorUri,
                        source);
                    tc.DisplayName = fifa_tc.MethodName + fifa_tc.AttributeIndex;
                    tests.Add(tc);
                }
            }
            discoverer.UnloadDomain();
            return tests;
        }
    }
}
