using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA.Framework.Analysis
{
    [Serializable]
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
        public uint block_index;
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

        public void Merge(BasicBlock bb)
        {
            if(bb.block_index != this.block_index)
            {
                throw new Exception("Cannot merge with a basic block which has a different block index.");
            }
            //update start
            if(this.start_line > bb.start_line)
            {
                this.start_line = bb.start_line;
                this.start_col = bb.start_col;
            } else if(this.start_line == bb.start_line)
            {
                if(this.start_col > bb.start_col)
                {
                    this.start_col = bb.start_col;
                }
            }

            //update end
            if(this.end_line < bb.end_line)
            {
                this.end_line = bb.end_line;
                this.end_col = bb.end_col;
            } else if( this.end_line == bb.end_line)
            {
                if(this.end_col < bb.end_col)
                {
                    this.end_col = bb.end_col;
                }
            }
        }
    }
}
