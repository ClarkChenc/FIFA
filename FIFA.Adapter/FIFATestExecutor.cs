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


namespace FLTestAdapter
{

    [ExtensionUri(Constant.ExecutorUriString)]
    public class FIFATestExecutor:ITestExecutor
    {
        #region Constants

        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>


        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>
        public static readonly Uri ExecutorUri = new Uri(Constant.ExecutorUriString);

        #endregion

        FIFA.Test.TestExecutor executor;
        public void Cancel()
        {
            if(executor != null)
            {
                executor.Cancelled = true;
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var tests = FIFATestDiscoverer.GetTests(sources, frameworkHandle as IMessageLogger);
            RunTests(tests, runContext, frameworkHandle);
        }

        FIFA.Framework.Test.TestCase ToFIFATestCase(TestCase test_case)
        {
            FIFA.Framework.Test.TestCase fifa_tc = new FIFA.Framework.Test.TestCase();
            string[] tokens = test_case.FullyQualifiedName.Split('#');
            fifa_tc.TypeFullName = tokens[0];
            fifa_tc.MethodName = tokens[1];
            fifa_tc.AttributeIndex = int.Parse(tokens[2]);
            fifa_tc.SourceFile = test_case.Source;
            return fifa_tc;
            
        }

        IFrameworkHandle frameworkHandle;
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            FLGlobalService.SendMessage("clear");
            this.frameworkHandle = frameworkHandle;
            FIFA.Framework.Test.TestSetting setting = new FIFA.Framework.Test.TestSetting();
            string file_path = Environment.GetEnvironmentVariable("PROGRAMFILES");
            setting.InstrumentTool = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\vsinstr.exe";
            setting.PerformanceCmder = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VSPerfCmd.exe";
            setting.IsDebugging = runContext.IsBeingDebugged;
            Guid guid = Guid.NewGuid();
            String cov_dir = runContext.TestRunDirectory + "\\" + guid;
            System.IO.Directory.CreateDirectory(cov_dir);
            setting.CoverageStoreDirectory = cov_dir;
            executor = new FIFA.Test.TestExecutor(setting);
     
            //get test cases
            List<FIFA.Framework.Test.TestCase> fifa_tr_list = new List<FIFA.Framework.Test.TestCase>();
            foreach (TestCase test in tests)
            {
                fifa_tr_list.Add(ToFIFATestCase(test));
            }

            executor.Initialize(fifa_tr_list);
            executor.CoverageGenerated += ProcessOneResult;
            executor.Execute();

            executor.Finish();
            
            foreach (TestCase test in tests) 
            {
                if (m_cancelled)
                {
                    break;
                }
                // Setup the test result as indicated by the test case.
                frameworkHandle.RecordStart(test);
                var testResult = new TestResult(test);
                testResult.StartTime = DateTime.Now;
                FLTestCase fl_tc = new FLTestCase();
                
                string[] tokens = test.FullyQualifiedName.Split('#');
                fl_tc.TypeFullName = tokens[0];
                fl_tc.MethodName = tokens[1];
                fl_tc.Source = test.Source;
                FLTestResult fl_tr = runner.Run(fl_tc, cov_dir);
                fl_tr_list.Add(fl_tr);
                
                testResult.Outcome = fl_tr.Passed ? TestOutcome.Passed : TestOutcome.Failed;
                testResult.ErrorMessage = fl_tr.ErrorMessage;
                testResult.ErrorStackTrace = fl_tr.StackTrace;
                testResult.EndTime = DateTime.Now;
                testResult.Duration = testResult.EndTime - testResult.StartTime;
                frameworkHandle.RecordEnd(test, testResult.Outcome);
                frameworkHandle.RecordResult(testResult);


            }
            /*
            foreach(var tr in fl_tr_list)
            {
                ProgramCov cov = CoveragePaser.LoadFromFile(tr.CoverageFile);
                foreach(var bb in cov.basic_blocks)
                {
                    FLLogger.WriteLine(bb.start_line.ToString(), 1, 0);
                    Console.WriteLine(bb.start_line.ToString());
                }
            }*/
            FLGlobalService.SendMessage("Analysing Coverage...");
            runner.RunFinalize(fl_tr_list);
            if (!runContext.IsBeingDebugged)
            {
                FLGlobalService.SendRank(runner.BasicBlockCoverageList);
            }
            FLGlobalService.SendMessage("FL Method: Op1");
            FLLogger.WriteLine(this.ToString() + ".RunTests tests out", 1, -1);
        }


        public void ProcessOneResult(FIFA.Framework.Test.TestResult fifa_test_result)
        {

        }
    }
}
