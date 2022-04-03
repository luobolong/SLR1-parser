using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SLR1
{
    public class Rule
    {
        [JsonProperty("left")]
        public Symbol Left { get; set; }

        [JsonProperty("right")]
        public List<Symbol> Right { get; set; }

        public Rule() { }

        public Rule(Rule rule)
        {
            Left = rule.Left;
            Right = new List<Symbol>(rule.Right.ToArray());
        }

        public Rule(Symbol L, List<Symbol> R)
        {
            Left = L;
            Right = new List<Symbol>(R.ToArray());
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj.GetType() == typeof(Rule))
            {
                var rule = (Rule)obj;
                if (rule.Left.Equals(this.Left))
                {
                    if (rule.Right.Count == this.Right.Count)
                    {
                        for (int i = 0; i < rule.Right.Count; i++)
                        {
                            if (!rule.Right[i].Equals(this.Right[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Left} → ");
            foreach (var s in Right)
            {
                sb.Append($"{s}");
            }
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Symbol>.Default.GetHashCode(Left) + EqualityComparer<List<Symbol>>.Default.GetHashCode(Right);
        }

        public static List<Rule> FromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Rule>> (json, new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None
            });
        }
    }
}
