using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SLR1
{
    public class Analyser
    {
        public Grammar grammar; // 文法
        public Grammar pGrammar; // 包含Point的文法
        public List<State> states = new List<State>(); // 状态集合
        public List<Move> moves = new List<Move>(); // 状态转移集合
        public Dictionary<State, Dictionary<Symbol, TableCell>> analysisTable = new Dictionary<State, Dictionary<Symbol, TableCell>>();
        public Analyser(string filename)
        {
            grammar = new Grammar(filename);
            pGrammar = new Grammar(grammar);
        }
        /// <summary>
        /// 判断是否是空产生式集
        /// </summary>
        /// <param name="symbol">非终结符</param>
        /// <returns>返回是否是空产生式集的结果</returns>
        private bool isRightEmpty(Symbol symbol)
        {
            foreach (var rule in grammar.Rules.Where(r => r.Left.Equals(symbol)))
            {
                var rightFirst = rule.Right.First();
                if (rightFirst.Type == "E")
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 返回去掉空串的符号集合
        /// </summary>
        /// <param name="symbols">符号集合</param>
        /// <returns></returns>
        private List<Symbol> deleteEmpty(List<Symbol> symbols)
        {
            List<Symbol> res = new List<Symbol>();
            foreach (var sym in symbols.Where(s => s.Type != "E"))
                res.Add(sym);
            return res;
        }
        /// <summary>
        /// 求非终结符的First集
        /// </summary>
        /// <param name="symbol">非终结符</param>
        /// <returns>返回First集的List</returns>
        private List<Symbol> getFirst(Symbol symbol)
        {
            List<Symbol> firstList = new List<Symbol>();
            foreach(var rule in grammar.Rules.Where(r => r.Left.Equals(symbol)))
            {
                var rightFirst = rule.Right.First();
                // 如果存在空产生式或产生式右部第一个是终结符，则将其加入First集中
                if(rightFirst.Type == "V" || rightFirst.Type == "E")
                {
                    firstList.Add(rightFirst);
                }
                // 如果产生式右部第一个是非终结符，X->Y1Y2...Yk
                else if (rightFirst.Type == "N" && !rightFirst.Equals(symbol))
                {
                    // 遍历这个产生式中的每一项
                    for(int index = 0; index < rule.Right.Count; index++)
                    {
                        var sym = rule.Right[index];
                        // 对于j(1≤j≤k)，若ε∈FIRST(Yj)，
                        // 则将到 FIRST(Yj)-{ε} 加入 FIRST(X)中，
                        // 其中若j=k，则将ε加入 FIRST(X)中
                        if (isRightEmpty(sym))
                        {
                            if(index == rule.Right.Count - 1)
                            {
                                firstList.AddRange(getFirst(sym));
                            }
                            else
                            {
                                firstList.AddRange(deleteEmpty(getFirst(sym)));
                            }
                        }
                        else
                        {
                            // 否则，将FIRST(Yj)加入 FIRST(X)中，
                            // 直接结束算法
                            firstList.AddRange(getFirst(sym));
                            break;
                        }
                    }
                }
            }
            return firstList.Distinct().ToList();
        }
        /// <summary>
        /// 求非终结符的Follow集
        /// </summary>
        /// <param name="symbol">非终结符</param>
        /// <returns>返回Follow集的List</returns>
        private List<Symbol> getFollow(Symbol symbol)
        {
            List<Symbol> followList = new List<Symbol>();
            // 对起始终结符 S ，将 $ 加入 FOLLOW(S)
            if (symbol == grammar.Symbols.First())
            {
                followList.Add(Grammar.D);
            }
            else
            {
                foreach (var rule in grammar.Rules.Where(r => r.Right.Contains(symbol)))
                {
                    var sIndex = rule.Right.IndexOf(symbol);
                    // 对于产生式：A->aBC，将除去空串的 FIRST(C) 加入 FOLLOW(B) 中; 
                    if (++sIndex < rule.Right.Count)
                    {
                        if (rule.Right[sIndex].Type == "V")
                        {
                            followList.Add(rule.Right[sIndex]);
                        }
                        else if(rule.Right[sIndex].Type == "N")
                        {
                            followList.AddRange(deleteEmpty(getFirst(rule.Right[sIndex])));
                        }
                        // 其中如果 C 可以推导出空串，则将 FOLLOW(A) 加入 FOLLOW(B) 中
                        if (isRightEmpty(rule.Right[sIndex]))
                        {
                            if(rule.Left != symbol)
                            {
                                followList.AddRange(getFollow(rule.Left));
                            }
                        }
                    }
                    // 对于产生式：A->aB，则将 FOLLOW(A) 加入 FOLLOW(B) 中
                    else
                    {
                        if (rule.Left != symbol)
                        {
                            followList.AddRange(getFollow(rule.Left));
                        }
                    }
                }
            }
            return followList.Distinct().ToList();
        }
        /// <summary>
        /// 得到所有的非终结符的First集字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<Symbol, List<Symbol>> getAllFirst()
        {
            Dictionary<Symbol, List<Symbol>> result = new Dictionary<Symbol, List<Symbol>>();
            foreach(var symbol in grammar.N_Symbols)
            {
                result.Add(symbol, getFirst(symbol));
            }
            return result;
        }
        /// <summary>
        /// 得到所有的非终结符的Follow集字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<Symbol, List<Symbol>> getAllFollow()
        {
            Dictionary<Symbol, List<Symbol>> result = new Dictionary<Symbol, List<Symbol>>();
            foreach (var symbol in grammar.N_Symbols)
            {
                result.Add(symbol, getFollow(symbol));
            }
            return result;
        }
        /// <summary>
        /// 得到以目标非终结符号为左部的所有产生式
        /// </summary>
        /// <param name="symbol">目标非终结符号</param>
        /// <returns>以目标非终结符号为左部的所有产生式集合</returns>
        private List<Rule> getRulesStartingWith(Symbol symbol)
        {
            List<Rule> rules = new List<Rule>();
            foreach(var rule in pGrammar.Rules.Where(r => r.Left.Equals(symbol)))
            {
                rules.Add(rule);
            }
            return rules;
        }
        /// <summary>
        /// 对目标规则集求闭包，直至集合不再增大
        /// </summary>
        /// <param name="rules">目标规则集</param>
        /// <returns></returns>
        private State getClosure(List<Rule> rules)
        {
            State state = new State();
            state.Rules.AddRange(rules);
            int rule_count = -1;
            while(state.Rules.Count != rule_count)
            {
                rule_count = state.Rules.Count;
                // 遍历目标规则集中的所有规则
                foreach(var rule in state.Rules)
                {
                    var pIndex = rule.Right.IndexOf(Grammar.P);
                    if(++pIndex < rule.Right.Count)
                    {
                        var symbol = rule.Right[pIndex];
                        if (symbol.Type == "N") // 如果是非终结符
                        {
                            List<Rule> temp = getRulesStartingWith(symbol).Where(t => !state.Rules.Contains(t)).ToList(); // 避免重复
                            if (temp.Count > 0)
                            {
                                state.Rules.AddRange(temp);
                                break;
                            }
                        }
                    }
                }
            }
            return state;
        }
        /// <summary>
        /// 源状态经由输入符号生成转换后的新状态
        /// </summary>
        /// <param name="from">原状态</param>
        /// <param name="by">输入符号</param>
        /// <returns>新状态</returns>
        public State moveState(State from, Symbol by)
        {
            List<Rule> to_rules = new List<Rule>();
            foreach (var rule in from.Rules)
            {
                var pIndex = rule.Right.IndexOf(Grammar.P);
                if(++pIndex < rule.Right.Count && rule.Right[pIndex].Equals(by))
                {
                    var _right = new List<Symbol>(rule.Right.ToArray());

                    var temp = _right[pIndex];
                    _right[pIndex] = _right[pIndex - 1];
                    _right[pIndex - 1] = temp;

                    to_rules.Add(new Rule(rule.Left, _right));
                }
            }
            return getClosure(to_rules);
        }
        /// <summary>
        /// 对目标文法生成DFA
        /// </summary>
        private void generateDFA()
        {
            foreach(var rule in pGrammar.Rules)
            {
                rule.Right.Insert(0, Grammar.P);
                if(rule.Right.Contains(Grammar.E))
                {
                    rule.Right.Remove(Grammar.E);
                }
            }

            // 初始化第一个状态
            int iCount = 1; // 状态计数
            var firstRule = pGrammar.Rules.First(); // 假设第一条规则是初始非终结符的规则
            State firstState = getClosure(new List<Rule>() { firstRule });
            firstState.Name = "I0";
            states.Add(firstState);

            for(int i = 0; i < states.Count; i++)
            {
                List<Symbol> afterP = new List<Symbol>();

                // 遍历规则集中的所有规则，找出点后的所有符号
                foreach (var rule in states[i].Rules)
                {
                    var pIndex = rule.Right.IndexOf(Grammar.P);
                    if (++pIndex < rule.Right.Count && !rule.Right[pIndex].Type.Equals("E"))
                    {
                        afterP.Add(rule.Right[pIndex]);
                    }
                }
                // 得到新状态
                foreach (var a in afterP.Distinct())
                {
                    State _state = moveState(states[i], a);
                    if (_state.Rules.Count > 0)
                    {
                        // 如果新状态已经出现过
                        if (states.Contains(_state))
                        {
                            // 更新状态转移表
                            moves.Add(new Move()
                            {
                                From = states[i],
                                To = states.Where(s => s.Equals(_state)).Single(),
                                By = a
                            });
                        }
                        // 如果新状态未出现过
                        else
                        {
                            _state.Name = $"I{iCount++}";
                            states.Add(_state);
                            moves.Add(new Move()
                            {
                                From = states[i],
                                To = _state,
                                By = a
                            });
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 生成分析表
        /// </summary>
        private void generateAnalysisTable()
        {
            // 先处理无转移状态的情况
            foreach (var state in states)
            {
                // 初始化 analysisTable
                List<Symbol> symbols = new List<Symbol>(grammar.Symbols.ToArray());
                if (symbols.Contains(Grammar.E))
                {
                    symbols.Remove(Grammar.E);
                }
                analysisTable.Add(state, new Dictionary<Symbol, TableCell>(symbols.Count));
                foreach (var symbol in symbols)
                {
                    analysisTable[state][symbol] = new TableCell();
                }

                // 如果 [Point] 在其中一个状态的所有产生式右部的最后
                if (state.Rules.All(r => r.Right.Last() == Grammar.P))
                {
                    // 遍历该状态下的全部产生式
                    foreach(var rule in state.Rules)
                    {
                        Rule _rule = new Rule(rule);
                        _rule.Right.Remove(Grammar.P);
                        if(_rule.Right.Count == 0)
                        {
                            _rule.Right.Add(Grammar.E);
                        }
                        // 找到这条产生式的编号
                        var index = grammar.Rules.FindIndex(r => r.Equals(_rule));
                        var followSet = getFollow(rule.Left); // 求出这条产生式左部的FOLLOW集
                        foreach(var follow in followSet) // 在相应位置上标记为规约
                        {
                            analysisTable[state][follow].Type = TableCell.Types.REDUCE;
                            analysisTable[state][follow].Value = index;
                        }
                    }
                }
            }
            // 遍历转移表
            foreach (var move in moves)
            {
                switch (move.By.Type)
                {
                    // 如果转移条件是终结符
                    case "V":
                        // 对于源状态不含有 [Point] 在产生式右部的最后的规则，标记为移进
                        foreach(var rule in move.From.Rules.Where(r => r.Right.Last() != Grammar.P))
                        {
                            analysisTable[move.From][move.By].Type = TableCell.Types.SHIFT;
                            analysisTable[move.From][move.By].Value = states.IndexOf(move.To);
                        }
                        // 对于源状态含有 [Point] 在产生式右部的最后的规则，标记为规约
                        foreach (var rule in move.From.Rules.Where(r => r.Right.Last() == Grammar.P))
                        {
                            Rule _rule = new Rule(rule);
                            _rule.Right.Remove(Grammar.P);
                            if (_rule.Right.Count == 0)
                            {
                                _rule.Right.Add(Grammar.E);
                            }
                            var index = grammar.Rules.FindIndex(t => t.Equals(_rule));
                            List<Symbol> followSet = getFollow(rule.Left);
                            foreach (var follow in followSet)
                            {
                                if (analysisTable[move.From][follow].Type != TableCell.Types.NULL) // 产生冲突
                                {
                                    throw new Exception("该文法不是SLR(1)文法");
                                }
                                analysisTable[move.From][follow].Type = TableCell.Types.REDUCE;
                                analysisTable[move.From][follow].Value = index;
                            }
                        }
                        break;
                    // 如果转移条件是非终结符，它对应GO TO表
                    case "N":
                        analysisTable[move.From][move.By].Type = TableCell.Types.GOTO;
                        analysisTable[move.From][move.By].Value = states.IndexOf(move.To);
                        break;
                    default:
                        analysisTable[move.From][move.By].Type = TableCell.Types.NULL;
                        break;
                }
            }
            // (I1, $) 在分析表上对应ACCEPT
            analysisTable[states[1]][Grammar.D].Type = TableCell.Types.ACC;
        }
        /// <summary>
        /// 分析文法
        /// </summary>
        public void analyzeGrammar()
        {
            generateDFA();
            generateAnalysisTable();
        }

        /// <summary>
        /// 将输入字符串转化成符号队列
        /// </summary>
        /// <param name="sInput">输入字符串</param>
        /// <param name="qInput">符号队列</param>
        private void StringToQueue(string sInput, out Queue<Symbol> qInput)
        {
            qInput = new Queue<Symbol>();
            foreach (string s in sInput.Trim().Split(' '))
            {
                bool flag = true;
                foreach (var symbol in grammar.Symbols)
                {
                    if (symbol.Equals(s))
                    {
                        flag = false;
                        qInput.Enqueue(symbol);
                        break;
                    }
                }
                if (flag)
                    throw new Exception($"输入了未定义的符号 {s}");
            }
            qInput.Enqueue(Grammar.D);
        }
        /// <summary>
        /// 分析输入串是否符合文法
        /// </summary>
        /// <param name="v">输入串</param>
        /// <param name="output">输出数据表</param>
        /// <returns>返回是否分析成功</returns>
        public bool analyseInput(string inputString, out DataTable output)
        {
            int stepNumber = 1;
            Stack<State> stateStack = new Stack<State>();
            Stack<Symbol> symbolStack = new Stack<Symbol>();
            Queue<Symbol> inputQueue = new Queue<Symbol>();
            List<string> actionList = new List<string>();
            bool isAccepted = false;
            bool isEnded = false;

            output = new DataTable();
            output.Columns.Add("Step", Type.GetType("System.Int32"));
            output.Columns.Add("State Stack", Type.GetType("System.String"));
            output.Columns.Add("Symbol Stack", Type.GetType("System.String"));
            output.Columns.Add("Input String", Type.GetType("System.String"));
            output.Columns.Add("Action", Type.GetType("System.String"));

            symbolStack.Push(Grammar.D);
            stateStack.Push(states.Find(t => t.Name == "I0"));
            try
            {
                StringToQueue(inputString, out inputQueue);
            }
            catch(Exception e)
            {
                throw e;
            }

            for(; ; )
            {
                Symbol inputSymbol = inputQueue.Peek();
                State topState = stateStack.Peek();
                TableCell cell = analysisTable[topState][inputSymbol];
                if (isEnded)
                {
                    if (isAccepted)
                    {
                        actionList.Add("ACCEPTED");
                    }
                    else
                    {
                        actionList.Add("NOT ACCEPTED");
                    }
                    break;
                }
                DataRow row = output.NewRow();
                row["Step"] = stepNumber;

                StringBuilder sbState = new StringBuilder();
                foreach (var state in stateStack.Reverse())
                {
                    sbState.Append(state.Name);
                }
                row["State Stack"] = sbState.ToString();

                StringBuilder sbSymbol = new StringBuilder();
                foreach (var symbol in symbolStack.Reverse())
                {
                    sbSymbol.Append(symbol);
                }
                row["Symbol Stack"] = sbSymbol.ToString();

                StringBuilder sbInput = new StringBuilder();
                foreach (var i in inputQueue)
                {
                    sbInput.Append(i);
                }
                row["Input String"] = sbInput.ToString();
                output.Rows.Add(row);
                switch (cell.Type)
                {
                    case TableCell.Types.SHIFT:
                        stateStack.Push(states[cell.Value]);
                        symbolStack.Push(inputSymbol);
                        inputQueue.Dequeue();
                        actionList.Add(cell.ToString());
                        stepNumber++;
                        break;
                    case TableCell.Types.REDUCE:
                        Rule rule = grammar.Rules[cell.Value];
                        if (rule.Right[0].Equals(Grammar.E))
                        {
                            symbolStack.Push(Grammar.E);
                            stateStack.Push(stateStack.Peek());
                        }
                        for(int i = 0; i < rule.Right.Count; i++)
                        {
                            symbolStack.Pop();
                            stateStack.Pop();
                        }
                        symbolStack.Push(rule.Left);
                        var nextState = stateStack.Peek();
                        var nextSymbol = symbolStack.Peek();
                        var _cell = analysisTable[nextState][nextSymbol];
                        if (_cell.Type != TableCell.Types.GOTO)
                        {
                            isEnded = true;
                        }
                        else
                        {
                            stateStack.Push(states[_cell.Value]);
                        }
                        actionList.Add(cell.ToString());
                        stepNumber++;
                        break;
                    case TableCell.Types.ACC:
                        isEnded = true;
                        isAccepted = true;
                        stepNumber++;
                        break;
                    default:
                        isEnded = true;
                        stepNumber++;
                        break;
                }
            }
            for(int i =0; i < output.Rows.Count; i++)
            {
                output.Rows[i]["Action"] = actionList[i];
            }
            return isAccepted;
        }
    }
}
