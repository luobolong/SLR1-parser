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
            var dict = new Dictionary<State, Dictionary<Symbol, TableCell>>(MainWindow.analyser.analysisTable);
            List<Symbol> symbols = new List<Symbol>(MainWindow.analyser.grammar.Symbols.ToArray());
            List<Symbol> v_symbols = new List<Symbol>(MainWindow.analyser.grammar.V_Symbols.ToArray());
            v_symbols.Add(Grammar.D);
            List<Symbol> n_symbols = new List<Symbol>(MainWindow.analyser.grammar.N_Symbols.ToArray());

            DataTable analysisDataTable = new DataTable();
            DataColumn column1 = new DataColumn();
            column1.ColumnName = "State";
            column1.DataType = System.Type.GetType("System.String");
            analysisDataTable.Columns.Add(column1);

            foreach(var symbol in v_symbols)
            {
                DataColumn action_column = new DataColumn();
                action_column.ColumnName = symbol.ToString();
                action_column.DataType = System.Type.GetType("System.String");
                analysisDataTable.Columns.Add(action_column);
            }
            foreach(var symbol in n_symbols)
            {
                DataColumn goto_column = new DataColumn();
                goto_column.ColumnName = symbol.ToString();
                goto_column.DataType = System.Type.GetType("System.String");
                analysisDataTable.Columns.Add(goto_column);
            }

            foreach(var key in dict.Keys)
            {
                DataRow row = analysisDataTable.NewRow();
                row["State"] = key.Name;
                foreach(var column in analysisDataTable.Columns)
                {
                    string column_name = column.ToString();
                    if (column_name.Equals("State"))
                    {
                        continue;
                    }
                    var cell = dict[key][symbols.Find(s => s.ToString().Equals(column_name))];
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
