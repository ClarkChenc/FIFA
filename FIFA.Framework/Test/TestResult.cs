using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA.Framework.Test
{
    public enum TestOutcome
    {
        Passed,
        Failed,
        NotRun
    }
    [Serializable]
    public class TestResult
    {
        public TestOutcome Outcome { set; get; }

        public string ErrorMessage { set; get; }

        public string StackTrace { set; get; }

        public string CoverageFile { set; get; }

        public TestCase CorrespondingTestCase { set; get; }
    }
}
