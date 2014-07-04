using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIFA.Framework.Analysis;

namespace FIFA.Analysis
{
    public class SuspCalculator
    {
        public static string[] MethodList = new string[] { "op1", "ochiai" };

        public SuspCalculator()
        {
            
        }

        public double Calc(BasicBlockFeature feature, string method)
        {
            switch(method)
            {
                case "op1":
                    return op1(feature);
                case "ochiai":
                    return ochiai(feature);
                default:
                    throw new Exception("method named " + method + " not heard.");
            }

        }

        public double Calc(BasicBlock bb, double f, double p, string method)
        {
            return Calc(new BasicBlockFeature(bb, f, p), method);
        }

        public void Calc(IEnumerable<BasicBlock> bb_list,double f, double p, string method)
        {
            foreach(var bb in bb_list)
            {
                bb.susp = Calc(bb, f, p, method);
            }
        }
        double op1(BasicBlockFeature a)
        {
            double result;
            try
            {
                result = a.ef - a.ep / (a.f + a.p);
            } catch(Exception)
            {
                result = -1;
            }
            if (double.IsNaN(result))
            {
                result = -1;
            }
            return result;
        }

        double ochiai(BasicBlockFeature a)
        {
            double result;
            try
            {
                result = a.ef / Math.Sqrt(a.f*(a.ef + a.ep));
            }
            catch (Exception)
            {
                result = -1;
            }
            if (double.IsNaN(result))
            {
                result = -1;
            }
            return result;
        }
        
    }
}
