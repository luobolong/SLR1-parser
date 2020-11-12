using System;
using System.Collections.Generic;
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
    /// DFA.xaml 的交互逻辑
    /// </summary>
    public partial class DFAWindow : Window
    {
        public DFAWindow()
        {
            InitializeComponent();
            ShowDFA();
        }
        private void ShowDFA()
        {
            if(MainWindow.analyser == null)
            {
                return;
            }
            listviewStates.Items.Clear();
            listviewMoves.Items.Clear();
            foreach (var state in MainWindow.analyser.states)
            {
                var sList = state.ToStringList();
                listviewStates.Items.Add(new {Name = sList[0], Productions = sList[1] });
            }
            foreach(var move in MainWindow.analyser.moves)
            {
                listviewMoves.Items.Add(new {From = move.From.Name, By = move.By, To = move.To.Name });
            }
        }
    }
}
