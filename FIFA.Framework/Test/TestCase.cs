using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIFA.Framework;

namespace FIFA.Framework.Test
{
    [Serializable]
    public class TestCase
    {
        public string MethodName { set; get; }
        public string TypeFullName { set; get; }
        public string ModuleName { set; get; }
        public string SourceFile { set; get; }
        public int AttributeIndex { set; get; }

       
    }
}
