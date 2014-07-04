using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Reflection;
using System.IO;
using FIFA.Framework.Test;
using FIFA.Framework;

namespace FIFA.Test
{
    public class TestDiscovererProxy:MarshalByRefObject
    {
        public List<TestCase> GetTestCases(string ass_path)
        {
            List<TestCase> test_case_list = new List<TestCase>();
            var ass = Assembly.LoadFrom(ass_path);
            int test_case_index = 0;
            foreach (var module in ass.GetModules())
            {
                foreach (var type in module.GetTypes())
                {
                    //get container
                    TestContainerAttribute container = type.GetCustomAttribute<TestContainerAttribute>();
                    if (container == null)
                    {
                        continue;
                    }
                    
                    foreach (var method in type.GetMethods())
                    {
                        List<TestCase> tmp_list = create_test_cases(ass_path, module, type, method, ref test_case_index);
                        test_case_list.AddRange(tmp_list);
                    }

                }

            }
           
            return test_case_list;
        }

        public List<string> GetRelevantAssemblies(IEnumerable<TestCase> test_case_collection)
        {

            var ass_path_list = test_case_collection.Select(test => test.SourceFile).Distinct();
            List<string> result = new List<string>();
            foreach (var ass_path in ass_path_list)
            {
                result.Add(ass_path);
                var ass = Assembly.LoadFrom(ass_path);
                foreach (var ref_ass in ass.GetReferencedAssemblies())
                {
                    var info = Directory.GetParent(ass_path);

                    if (File.Exists(info.FullName + @"\" + ref_ass.Name + ".pdb"))
                    {
                        if (File.Exists(info.FullName + @"\" + ref_ass.Name + ".dll"))
                        {
                            result.Add(info.FullName + @"\" + ref_ass.Name + ".dll");
                        }
                        else if (File.Exists(info.FullName + @"\" + ref_ass.Name + ".exe"))
                        {
                            result.Add(info.FullName + @"\" + ref_ass.Name + ".exe");
                        }
                    }
                }
            }

            return result;
             
        }

        List<TestCase> create_test_cases(
            string source,
            Module module, 
            Type type,
            MethodInfo method,
            ref int test_case_index)
        {
            //process TestAtrribute
            List<TestCase> test_case_list = new List<TestCase>();
            object[] attr_array = method.GetCustomAttributes(false);
            for (int i = 0; i < attr_array.Length; i++)
            {
                int index = -1;
                if(attr_array[i] is TestAttribute)
                {
                    index = i;
                }
                if(index < 0)
                {
                    continue;
                }
                TestCase tc = new TestCase();
                tc.AttributeIndex = index;
                tc.MethodName = method.Name;
                tc.TypeFullName = type.FullName;
                tc.ModuleName = module.Name;
                tc.SourceFile = source;
                tc.TestIndex = test_case_index;
                test_case_index++;
                test_case_list.Add(tc);
            }

            return test_case_list;
        }


        
    }
}
