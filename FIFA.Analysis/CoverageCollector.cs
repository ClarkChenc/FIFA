using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Coverage.Analysis;
using FIFA.Framework.Test;
using FIFA.Framework.Analysis;

namespace FIFA.Analysis
{
    public class CoverageCollector
    {
        bool need_update;
        List<BasicBlock> basic_block_list;
        List<ModuleCov> module_list;

        public List<BasicBlock> BasicBlockList
        {
            get
            {
                if(need_update)
                {
                    basic_block_list = new List<BasicBlock>();
                    foreach(var module in module_list)
                    {
                        basic_block_list.AddRange(module.BasicBlockList);
                    }
                    need_update = false;
                    
                } 
                return basic_block_list;
                
            }
        }

        public CoverageCollector()
        {
            module_list = new List<ModuleCov>();
            need_update = true;
        }

        /// <summary>
        /// this function should only be called when test_result.OutCome == passed or failed
        /// </summary>
        /// <param name="file">coverage file</param>
        /// <param name="test_result">test result</param>
        public void MergeFromFile(TestResult test_result)
        {
            need_update = true;
            if ((test_result.Outcome != TestOutcome.Failed) &&
                (test_result.Outcome != TestOutcome.Passed))
            {
                return;
            }
            List<ModuleCov> module_cov_tmp_list = new List<ModuleCov>();
            using (CoverageInfo info = CoverageInfo.CreateFromFile(test_result.CoverageFile))
            {
                List<BlockLineRange> lines = new List<BlockLineRange>();
                foreach (ICoverageModule module in info.Modules)
                {
                    byte[] coverageBuffer = module.GetCoverageBuffer(null);

                    ModuleCov module_cov = new ModuleCov();
                    module_cov.Name = module.Name;
                    using (ISymbolReader reader = module.Symbols.CreateReader())
                    {
                        uint methodId;
                        string methodName;
                        string undecoratedMethodName;
                        string className;
                        string namespaceName;


                        lines.Clear();
                        while (reader.GetNextMethod(
                            out methodId,
                            out methodName,
                            out undecoratedMethodName,
                            out className,
                            out namespaceName,
                            lines))
                        {
                            foreach (var line in lines)
                            {

                                BasicBlock bb = new BasicBlock();
                                bb.class_name = className;
                                bb.namespace_name = namespaceName;
                                bb.method_name = methodName;
                                bb.source_file_path = line.SourceFile;
                                bb.module_name = module.Name;
                                bb.start_col = line.StartColumn;
                                bb.end_col = line.EndColumn;
                                bb.start_line = line.StartLine;
                                bb.end_line = line.EndLine;
                                if (test_result.Outcome == TestOutcome.Passed)
                                {
                                    bb.passed_covered = coverageBuffer[line.BlockIndex] > 0 ? (uint)1 : (uint)0;
                                }
                                else
                                {
                                    bb.failed_covered = coverageBuffer[line.BlockIndex] > 0 ? (uint)1 : (uint)0;
                                }
                                module_cov.BasicBlockList.Add(bb);
                            }
                            lines.Clear();
                        }
                    }

                    module_cov_tmp_list.Add(module_cov);

                }
            }

            Merge(module_cov_tmp_list);
        }

        public void MergeFromFile(IEnumerable<TestResult> test_result_list)
        {
            foreach(var test_result in test_result_list)
            {
                this.MergeFromFile(test_result);
            }
        }

        void Merge(List<ModuleCov> module_cov_tmp_list)
        {
            foreach(var m in module_cov_tmp_list)
            {
                ModuleCov m_in_list = module_list.Find(new Predicate<ModuleCov>(x => x.Name == m.Name));
                if(m_in_list != null)
                {
                    if (m_in_list.BasicBlockList.Count != m.BasicBlockList.Count)
                    {
                        throw new Exception("module's basic block list size not match.");
                    }
                    for (int i = 0; i < m_in_list.BasicBlockList.Count; i++)
                    {
                        m_in_list.BasicBlockList[i].passed_covered += m.BasicBlockList[i].passed_covered;
                        m_in_list.BasicBlockList[i].failed_covered += m.BasicBlockList[i].failed_covered;
                    }
                } else
                {
                    module_list.Add(m);
                }
            }
        }
    }
}
