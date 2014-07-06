using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA.Framework.Analysis
{
    [Serializable]
    public class BasicBlockFeature
    {
        public double ef;
        public double ep;
        public double nf;
        public double np;
        public double f;
        public double p;

        public BasicBlockFeature(BasicBlock bb, double f, double p)
        {
            this.ef = bb.failed_covered;
            this.ep = bb.passed_covered;
            this.nf = f - this.ef;
            this.np = p - this.ep;
            this.f = f;
            this.p = p;
        }
    }
}
