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
            throw new NotImplementedException();
        }
    }
}
