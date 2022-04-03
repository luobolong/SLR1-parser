using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLR1
{
    public class Move
    {
        public State From { get; set; }
        public State To { get; set; }
        public Symbol By { get; set; }

        public override string ToString()
        {
            return From.Name + " —" + By.Value + "→ " + To.Name;
        }
    }
}
