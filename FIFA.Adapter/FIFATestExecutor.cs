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
        FIFATestExecutionHandler handler;
        TestCase[] test_case_array;
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

        FIFA.Framework.Test.TestCase ToFIFATestCase(TestCase test_case, int marker)
        {
            FIFA.Framework.Test.TestCase fifa_tc = new FIFA.Framework.Test.TestCase();
            string[] tokens = test_case.FullyQualifiedName.Split('#');
            fifa_tc.ModuleName = tokens[0];
            fifa_tc.TypeFullName = tokens[1];
            fifa_tc.MethodName = tokens[2];
            fifa_tc.AttributeIndex = int.Parse(tokens[3]);
            fifa_tc.SourceFile = test_case.Source;
            fifa_tc.Marker = marker;
            return fifa_tc;
            
        }

        
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            FLGlobalService.SendMessage("clear");
            //setting
            FIFA.Framework.Test.TestSetting setting = new FIFA.Framework.Test.TestSetting();
            string file_path = Environment.GetEnvironmentVariable("PROGRAMFILES");
            setting.InstrumentTool = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\vsinstr.exe";
            setting.PerformanceCmder = file_path + @"\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\VSPerfCmd.exe";
            setting.IsDebugging = runContext.IsBeingDebugged;
            Guid guid = Guid.NewGuid();
            String cov_dir = runContext.TestRunDirectory + "\\" + guid;
            System.IO.Directory.CreateDirectory(cov_dir);
            setting.CoverageStoreDirectory = cov_dir;
            //generate test cases
            test_case_array = tests.ToArray();
            FIFA.Framework.Test.TestCase[] fifa_test_case_array = new FIFA.Framework.Test.TestCase[test_case_array.Length];
            for (int i = 0; i < test_case_array.Length; i++)
            {
                fifa_test_case_array[i] = ToFIFATestCase(test_case_array[i], i);
            }
            
            FIFATestExecutionHandler handler = new FIFATestExecutionHandler(frameworkHandle);
            executor = new FIFA.Test.TestExecutor(setting);
            //events
            executor.OneTestStarted += handler.OneTestStart;
            executor.OneTestEnded += handler.OneTestEnded;
            //start
            executor.Initialize(fifa_test_case_array);
            executor.Execute();

            //finish
            executor.Finish();
            
            
        }
    }
}
