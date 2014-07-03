using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIFA.Framework.Test;

namespace FIFA.Test
{
    public class TestExecutor
    {
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
        }

        /// <summary>
        /// Instrument relevant assemblies.
        /// </summary>
        /// <param name="test_case_collection">All the tests that will be executed later.</param>
        public void Initialize(IEnumerable<TestCase> test_case_collection)
        {
            Results.Clear();
            //instr
        }

        public TestResult Execute(TestCase test_case)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restore assemblies that have been instrumented.
        /// Before finalization, coverage infromation should be processed first.
        /// </summary>
        public void Finalize()
        {
            
        }
    }
}
