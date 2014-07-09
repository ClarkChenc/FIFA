using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA.Framework.Analysis
{
    public enum FLMethod
    {
        ochiai,
        op1,
    }
    [Serializable]
    public class LocatorSetting
    {
        public FLMethod Method { set; get; }
    }
}
