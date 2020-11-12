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
    /// FirstFollowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FirstFollowWindow : Window
    {
        public FirstFollowWindow()
        {
            InitializeComponent();
            ShowFirstnFollow();
        }

        private string ListToString(List<Symbol> symbols)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var symbol in symbols)
            {
                sb.Append(symbol.ToString() + "  ");
            }
            return sb.ToString();
        }

        private void ShowFirstnFollow()
        {
            if(MainWindow.analyser == null || MainWindow.firstDict == null || MainWindow.followDict == null)
            {
                return;
            }
            listviewFirst.Items.Clear();
            listviewFollow.Items.Clear();
            foreach (var k in MainWindow.firstDict)
            {
                listviewFirst.Items.Add(new { Sym = k.Key.ToString(), Str = ListToString(k.Value) });
            }
            foreach (var k in MainWindow.followDict)
            {
                listviewFollow.Items.Add(new { Sym = k.Key.ToString(), Str = ListToString(k.Value) });
            }
        }
    }
}
