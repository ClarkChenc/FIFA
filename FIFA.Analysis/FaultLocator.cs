using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIFA.Framework.Analysis;
using FIFA.Framework.Test;

namespace FIFA.Analysis
{
    public class FaultLocator
    {
        public LocatorSetting Setting { set; get; }
        public FaultLocator(LocatorSetting setting)
        {
            this.Setting = setting;
        }

        public List<BasicBlock> GetRankList(IEnumerable<TestResult> result_list)
        {
            CoverageCollector collector = new CoverageCollector();
            int f = 0;
            int p = 0;
            foreach(var result in result_list)
            {
                if(result.Outcome == TestOutcome.Passed)
                {
                    p += 1;
                } else if(result.Outcome == TestOutcome.Failed)
                {
                    f += 1;
                } else
                {
                    continue;
                }
                collector.MergeFromFile(result);
            }



            return GetRankList(collector.BasicBlockList, f, p);

        }

        public List<BasicBlock> GetRankList(List<BasicBlock> bb_list, double f, double p )
        {
            SuspCalculator calculator = new SuspCalculator();
            calculator.Calc(bb_list, f, p, Setting.Method);
            bb_list.Sort(new Comparison<BasicBlock>(
                (x, y) => -x.susp.CompareTo(y.susp)
                ));

            return bb_list;
        }
    }
}
