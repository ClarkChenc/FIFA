using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoverageHelper;

namespace FLTestAdapter
{
    public class BasicBlockView
    {
        public double Susp { set; get; }
        public string Source { set; get; }
        public string Method {set; get;}
        public string Class {set; get;}
        public string Namespace { set; get; }

        public BasicBlockCov Cov { set; get; }

        public BasicBlockView(BasicBlockCov cov)
        {
            this.Cov = cov;
            this.Source = (new System.IO.DirectoryInfo(cov.source_file_path)).Name + " line " + cov.start_line;
            this.Method = cov.method_name;
            this.Class = cov.class_name;
            this.Namespace = cov.namespace_name;
            this.Susp = cov.susp;
        }

        public BasicBlockView()
        {

        }
    }
}
