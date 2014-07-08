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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using FIFA.Analysis;
using System.Threading;

namespace FIFATestAdapter
{
    class FIFATestExecutionHandler
    {
        TestResult current_result;
        TestCase[] test_case_array;
        IFrameworkHandle frameworkHandle;
        CoverageCollector cc;
        int f;
        int p;
        public FIFATestExecutionHandler(IFrameworkHandle handle, TestCase[] array)
        {
            this.frameworkHandle = handle;
            this.test_case_array = array;
        }
        public void OneTestStart(FIFA.Framework.Test.TestCase fifa_tc)
        {
            int index = fifa_tc.Marker;
            current_result = new TestResult(test_case_array[index]);
            current_result.StartTime = DateTime.Now;
        }

        public void OneTestEnded(FIFA.Framework.Test.TestCase fifa_tc, FIFA.Framework.Test.TestResult fifa_tr)
        {
            TestOutcome tout;
            if (fifa_tr.Outcome == FIFA.Framework.Test.TestOutcome.Failed)
            {
                tout = TestOutcome.Failed;
            }
            else if (fifa_tr.Outcome == FIFA.Framework.Test.TestOutcome.Passed)
            {
                tout = TestOutcome.Passed;
            }
            else
            {
                tout = TestOutcome.Skipped;
            }
            current_result.Outcome = tout;
            current_result.ErrorMessage = fifa_tr.ErrorMessage;
            current_result.ErrorStackTrace = fifa_tr.StackTrace;
            current_result.EndTime = DateTime.Now;
            current_result.Duration = current_result.EndTime - current_result.StartTime;
            frameworkHandle.RecordEnd(current_result.TestCase, current_result.Outcome);
            frameworkHandle.RecordResult(current_result);
        }

        public void OneCoverageGenerated(FIFA.Framework.Test.TestCase fifa_tc, FIFA.Framework.Test.TestResult fifa_tr)
        {
            if(fifa_tr.Outcome == FIFA.Framework.Test.TestOutcome.Passed)
            {
                p += 1;
                ThreadPool.QueueUserWorkItem(CollectThread, fifa_tr);
            } else if(fifa_tr.Outcome == FIFA.Framework.Test.TestOutcome.Failed)
            {
                f += 1;
                ThreadPool.QueueUserWorkItem(CollectThread, fifa_tr);
                
            }
        }

        void CollectThread(object obj)
        {
            lock (this)
            {
                FIFA.Framework.Test.TestResult fifa_tr = obj as FIFA.Framework.Test.TestResult;
                cc.MergeFromFile(fifa_tr);
            }
        }

        
        public void TestStated(IEnumerable<FIFA.Framework.Test.TestCase> fifa_tc_list)
        {
            cc = new CoverageCollector();
            f = 0;
            p = 0;
            FLGlobalService.SendMessage("Test Stated.");
        }

        public void TestEnded(IEnumerable<FIFA.Framework.Test.TestCase> fifa_tc_list,
                              IEnumerable<FIFA.Framework.Test.TestResult> fifa_tr_list)
        {
            FIFA.Framework.Analysis.LocatorSetting setting = new FIFA.Framework.Analysis.LocatorSetting();
            setting.Method = "ochiai";
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "FL Method: ochiai.");
            FaultLocator locator = new FaultLocator(setting);
            FLGlobalService.SendMessage("Test Ended. Calculating suspiciousness...");
            List<FIFA.Framework.Analysis.BasicBlock> list;
            lock (this)
            {
                list = locator.GetRankList(cc.BasicBlockList, f, p);
            }
            FLGlobalService.SendMessage("Delivering results...");
            FLGlobalService.SendRank(list);
            FLGlobalService.SendMessage("Ready");
        }

    }
}
