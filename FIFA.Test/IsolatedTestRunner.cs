using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FIFA.Framework.Test;
using FIFA.Framework;

namespace FIFA.Test
{
    class IsolatedTestRunner
    {
        static void Main(string[] args)
        {
            BinaryFormatter formater = new BinaryFormatter();
            TestCase test_case = (TestCase)formater.Deserialize(Console.OpenStandardInput());
            TestResult test_result = TestExecutor.ExecuteStatic(test_case);
            Stream st = Console.OpenStandardOutput();
            formater.Serialize(st, test_result);
            st.Flush();
            st.Close();
        }
    }
}
