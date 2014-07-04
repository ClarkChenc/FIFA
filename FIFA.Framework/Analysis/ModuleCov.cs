using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA.Framework.Analysis
{
    public class ModuleCov
    {
        public List<BasicBlock> BasicBlockList { private set; get; }

        public ModuleCov()
        {
            BasicBlockList = new List<BasicBlock>();
        }
        public string Name { set; get; }

        public override bool Equals(object obj)
        {
            ModuleCov other = obj as ModuleCov;
            if(other != null && other.Name == this.Name)
            {
                return true;
            }
            return false;
        }
    }
}
