using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SLR1
{
    /// <summary>
    /// tableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AnalysisTableWindow : Window
    {
        public AnalysisTableWindow()
        {
            InitializeComponent();
            ShowTable();
        }

        private void ShowTable()
        {
            if(MainWindow.analyser == null)
            {
                return;
            }
            datagrid.Items.Clear();
            var dict = MainWindow.analyser.analysisTable;
            List<Symbol> symbols = MainWindow.analyser.grammar.Symbols;
            List<Symbol> v_n_symbols = new List<Symbol>();
            v_n_symbols.AddRange(MainWindow.analyser.grammar.V_Symbols);
            v_n_symbols.Add(Grammar.D);
            v_n_symbols.AddRange(MainWindow.analyser.grammar.N_Symbols);

            DataTable analysisDataTable = new DataTable();
            analysisDataTable.Columns.Add(new DataColumn() { ColumnName = "State", DataType = System.Type.GetType("System.String") });

            for(int i = 0; i < v_n_symbols.Count; i++)
            {
                analysisDataTable.Columns.Add(new DataColumn() { ColumnName = $"S{i + 1}", DataType = System.Type.GetType("System.String") });
            }

            DataRow symbols_row = analysisDataTable.NewRow();
            for(int i = 0; i < v_n_symbols.Count; i++)
            {
                symbols_row[$"S{i + 1}"] = v_n_symbols[i].ToString();
            }
            analysisDataTable.Rows.Add(symbols_row);

            foreach(var key in dict.Keys)
            {
                DataRow row = analysisDataTable.NewRow();
                row["State"] = key.Name;
                for(int i = 1; i < analysisDataTable.Columns.Count; i++)
                {
                    string column_name = analysisDataTable.Columns[i].ToString();
                    string symbol_name = v_n_symbols[i - 1].ToString();
                    var cell = dict[key][symbols.Find(s => s.ToString().Equals(symbol_name))];
                    switch (cell.Type)
                    {
                        case TableCell.Types.ACC:
                            row[column_name] = "ACC";
                            break;
                        case TableCell.Types.REDUCE:
                            row[column_name] = $"R{cell.Value}";
                            break;
                        case TableCell.Types.SHIFT:
                            row[column_name] = $"S{cell.Value}";
                            break;
                        case TableCell.Types.GOTO:
                            row[column_name] = $"{cell.Value}";
                            break;
                        case TableCell.Types.NULL:
                            row[column_name] = "";
                            break;
                    }
                }
                analysisDataTable.Rows.Add(row);
            }
            datagrid.ItemsSource = analysisDataTable.DefaultView;
        }
    }
}
