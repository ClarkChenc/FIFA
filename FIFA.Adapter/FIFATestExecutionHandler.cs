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

namespace FIFATestAdapter
{
    class FIFATestExecutionHandler
    {
        TestResult current_result;
        TestCase[] test_case_array;
        IFrameworkHandle frameworkHandle;
        public FIFATestExecutionHandler(IFrameworkHandle handle)
        {
            this.frameworkHandle = handle;
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

    }
}
