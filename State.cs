using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLR1
{
    public class State
    {
        public string Name { get; set; }
        public List<Rule> Rules { get; private set; } = new List<Rule>();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"State:{Name}");
            foreach (var rule in Rules)
            {
                sb.AppendLine($"  {rule}");
            }
            return sb.ToString();
        }

        public List<string> ToStringList()
        {
            List<string> sList = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach(var rule in Rules)
            {
                sb.Append($"{rule}   ");
            }
            sList.Add(Name);
            sList.Add(sb.ToString().Trim());
            return sList;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj.GetType() == typeof(State))
            {
                return ((State)obj).Rules.All(Rules.Contains) && ((State)obj).Rules.Count == Rules.Count;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(Name) + EqualityComparer<List<Rule>>.Default.GetHashCode(Rules);
        }
    }
}
