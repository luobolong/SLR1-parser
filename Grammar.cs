using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace SLR1
{
    public class Grammar
    {
        public static Symbol P { get; } = new Symbol() { Type = "V", Value = "[Point]" };
        public static Symbol E { get; } = new Symbol() { Type = "E", Value = "[Null]" };
        public static Symbol D { get; } = new Symbol() { Type = "V", Value = "[Dollar]" };
        public List<Rule> Rules { get; private set; } = new List<Rule>();
        public List<Symbol> Symbols { get; private set; } = new List<Symbol>();
        public List<Symbol> N_Symbols { get; private set; } = new List<Symbol>(); // 非终结符
        public List<Symbol> V_Symbols { get; private set; } = new List<Symbol>(); // 终结符

        public Grammar(string filename)
        {
            readFile(filename);
        }

        public Grammar(Grammar grammar)
        {
            foreach (var rule in grammar.Rules)
            {
                Rules.Add(new Rule(rule));
            }
            Symbols.AddRange(grammar.Symbols);
        }

        private void readFile(string filename)
        {
            string json = File.ReadAllText(filename);
            Rules.AddRange(Rule.FromJson(json));

            List<Symbol> temp = new List<Symbol>();
            // 找出所有的符号
            foreach (var rule in Rules)
            {
                temp.Add(rule.Left);
                temp.AddRange(rule.Right);
            }
            Symbols.AddRange(temp.Distinct().ToList());
            foreach(var sym in Symbols)
            {
                if (sym.Type == "N")
                {
                    N_Symbols.Add(sym);
                }
                if (sym.Type == "V")
                {
                    V_Symbols.Add(sym);
                }
            }
            Symbols.Add(D);
        }
    }
}
