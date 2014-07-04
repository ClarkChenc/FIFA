using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA.Framework.Analysis
{
    public class BasicBlock
    {
        public string module_name;
        public string source_file_path;
        public string namespace_name;
        public string class_name;
        public string method_name;
        public uint start_line;
        public uint end_line;
        public uint start_col;
        public uint end_col;
        public double susp;
        public uint passed_covered;
        public uint failed_covered;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(namespace_name);
            sb.Append(".");
            sb.Append(class_name);
            sb.Append(".");
            sb.Append(method_name);
            sb.Append(" ");
            sb.Append(start_line);
            sb.Append(":");
            sb.Append(start_col);
            sb.Append(",");
            sb.Append(end_line);
            sb.Append(":");
            sb.Append(end_col);
            sb.Append(" ef:");
            sb.Append(failed_covered);
            sb.Append(" ep:");
            sb.Append(passed_covered);
            sb.Append("  ");
            sb.Append(susp);
            return sb.ToString();
        }
    }
}
