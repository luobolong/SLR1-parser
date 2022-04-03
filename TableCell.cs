using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLR1
{
    public class TableCell
    {
        public enum Types { NULL, SHIFT, REDUCE, GOTO, ACC};
        public Types Type { get; set; }
        public int Value { get; set; }
        public TableCell()
        {
            Type = Types.NULL;
            Value = -1;
        }
        public TableCell(Types type, int value)
        {
            Type = type;
            Value = value;
        }
        public override string ToString()
        {
            return $"{Type} {Value}";
        }
    }
}
