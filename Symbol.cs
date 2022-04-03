using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SLR1
{
    public partial class Symbol
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public Symbol()
        {

        }
        public Symbol(Symbol s)
        {
            Type = s.Type;
            Value = s.Value;
        }
        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (obj.GetType() == typeof(Symbol))
            {
                return ((Symbol)obj).Value == this.Value;
            }
            if (obj.GetType() == typeof(string))
            {
                return (string)obj == this.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<string>.Default.GetHashCode(Type) + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            if (Value == "[Dollar]")
            {
                return "$";
            }
            if (Type == "E" && Value == "[Null]")
            {
                return "ε";
            }
            if (Value == "[Point]")
            {
                return "•";
            }
            return Value;
        }
    }
}
