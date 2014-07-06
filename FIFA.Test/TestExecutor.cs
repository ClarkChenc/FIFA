using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using FIFA.Framework.Test;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using FIFA.Framework;

namespace FIFA.Test
{
    public class TestExecutor
    {
       
        #region events
        public event Action<IEnumerable<TestCase>> TestStated;
        public event Action<IEnumerable<TestCase>, List<TestResult>> TestEnded;
        public event Action<List<string>> InstrumentStarted;
        public event Action<List<string>> InstrumentEnded;
        public event Action<TestCase, TestResult> OneCoverageGenerated;
        public event Action<TestCase> OneTestStarted;
        public event Action<TestCase, TestResult> OneTestEnded;
        #endregion
        //We should remember the assemblies that we have instrumented, so that we can restore them later.
        List<string> instrumented_assemblies;

        IEnumerable<TestCase> test_case_collection;

        public bool Cancelled { set; get; }
        /// <summary>
        /// Set or get test setting.
        /// </summary>
        public TestSetting Setting { set; get; }
        /// <summary>
        /// Test results will be stored here.
        /// </summary>
        public List<TestResult> Results { get; private set; }

        public TestExecutor(TestSetting setting)
        {
            this.Setting = setting;
            Results = new List<TestResult>();
            instrumented_assemblies = new List<string>();
        }

        /// <summary>
        /// Instrument relevant assemblies.
        /// </summary>
        /// <param name="test_case_collection">All the tests that will be executed later.</param>
        public void Initialize(IEnumerable<TestCase> test_case_collection)
        {
            Cancelled = false;
            instrumented_assemblies.Clear();
            Results.Clear();
            this.test_case_collection = test_case_collection;
            if(Setting.IsDebugging)
            {
                return;
            }
            //instrument
            TestDiscoverer discoverer = new TestDiscoverer();
            discoverer.LoadDomain();
            List<string> source_list = discoverer.Proxy.GetRelevantAssemblies(test_case_collection);
            discoverer.UnloadDomain();
            if(InstrumentStarted != null )
            {
                InstrumentStarted(source_list);
            }
            Instrument(source_list);
            if(InstrumentEnded!=null)
            {
                InstrumentEnded(source_list);
            }
        }

        void Instrument(IEnumerable<string> ass_list)
        {
            List<Process> p_list = new List<Process>();
            foreach (var ass in ass_list)
            {
                Process p = new Process();
                p.StartInfo.FileName = Setting.InstrumentTool;
                p.StartInfo.Arguments = "/coverage \"" + ass + "\"";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p_list.Add(p);
                instrumented_assemblies.Add(ass);
            }
            for (int i = 0; i < p_list.Count; i++ )
            {
                Process p = p_list[i];
                p.WaitForExit();
                //if instrument failed, we remove the item
                if (p.ExitCode != 0)
                {
                    instrumented_assemblies[i] = null;
                    Console.WriteLine(p.StandardError.ReadToEnd());
                }
                p.Close();
            }

        }


        /// <summary>
        /// This is intended to be called independently of setting
        /// </summary>
        /// <param name="test_case">Test case to be run</param>
        /// <returns>Test result</returns>
        public static TestResult ExecuteStatic(TestCase test_case)
        {
            Assembly ass = Assembly.LoadFrom(test_case.SourceFile);

            //assume test will pass
            TestResult test_result = new TestResult();
            test_result.Outcome = TestOutcome.Passed;
            //find the test
            var module = ass.GetModule(test_case.ModuleName);
            var type = module.GetType(test_case.TypeFullName);
            var method = type.GetMethod(test_case.MethodName);
            var attr = method.GetCustomAttributes(false)[test_case.AttributeIndex];
            if (attr is TestAttribute)
            {
                object obj = ass.CreateInstance(type.FullName);
                try
                {
                    method.Invoke(obj, null);
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    //test failed.
                    test_result.ErrorMessage = ex.InnerException.Message;
                    test_result.StackTrace = ex.InnerException.StackTrace;
                    test_result.Outcome = TestOutcome.Failed;
                }
            }   //here we can put more attributes.
            else
            {
                test_result.ErrorMessage = "The attribute can not be recognized.";
                test_result.StackTrace = null;
                test_result.Outcome = TestOutcome.NotRun;
            }
            
            return test_result;
        }
        void Execute(TestCase test_case)
        {
            if(OneTestStarted!= null)
            {
                OneTestStarted(test_case);
            }
            TestResult test_result;
            if (Setting.IsDebugging)
            {
                test_result = TestExecutor.ExecuteStatic(test_case);
                if (OneTestEnded != null)
                {
                    OneTestEnded(test_case, test_result);
                }
                Results.Add(test_result);
                return;
            }
            //start monitor
            string cov_file = GetCovFilePath(Setting.CoverageStoreDirectory, test_case);
            StartMonitor(cov_file);
            //execute test
            Process p = new Process();
            p.StartInfo.FileName = (typeof(TestExecutor)).Assembly.Location;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(p.StandardInput.BaseStream, test_case);
            p.WaitForExit();
            test_result = (TestResult)formatter.Deserialize(p.StandardOutput.BaseStream);
            p.Close();
            //stop monitor
            StopMonitor();
            //keep in track of the coverage file
            test_result.CoverageFile = cov_file + ".coverage";
            //record the result
            Results.Add(test_result);
            //broadcast the result
            if (OneCoverageGenerated!=null)
            {
                OneCoverageGenerated(test_case, test_result);
            }
            if (OneTestEnded != null)
            {
                OneTestEnded(test_case, test_result);
            }
        }
        string GetCovFilePath(string dir, TestCase test_case)
        {
            return dir + @"\" + test_case.MethodName +test_case.AttributeIndex.ToString();
        }
        void StartMonitor(string cov_file)
        {
            Process p = new Process();
            p.StartInfo.FileName = Setting.PerformanceCmder;
            p.StartInfo.Arguments = "-start:coverage -output:\"" + cov_file + "\"";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            p.Close();


        }
        void StopMonitor()
        {
            Process p = new Process();
            p.StartInfo.FileName = Setting.PerformanceCmder;
            p.StartInfo.Arguments = "-shutdown";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            p.Close();

        }

        public void Execute()
        {
            //later, we might consider about execution orders among test cases.
            if(TestStated != null)
            {
                TestStated(test_case_collection);
            }
            List<TestResult> test_result_list = new List<TestResult>();
            foreach(var test_case in test_case_collection)
            {
                if (Cancelled)
                {
                    break;
                }
                Execute(test_case);
            }
            if (TestEnded != null)
            {
                TestEnded(test_case_collection, test_result_list);
            }

        }

        /// <summary>
        /// Restore assemblies that have been instrumented.
        /// Before finalization, coverage infromation should be processed first.
        /// </summary>
        public void Finish()
        {
            //restore instrumented assemblies.
            Restore(instrumented_assemblies);
        }

        void Restore(IEnumerable<string> ass_list)
        {
            foreach (var ass in ass_list)
            {
                if(ass == null)
                {
                    continue;
                }
                File.Delete(ass);
                File.Move(ass + ".orig", ass);
                File.Delete(ass.Substring(0, ass.Length - 3) + "instr.pdb");
            }
        }
    }
}
