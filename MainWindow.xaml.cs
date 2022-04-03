using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SLR1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Analyser analyser { get; set; }
        public static Dictionary<Symbol, List<Symbol>> firstDict { get; set; }
        public static Dictionary<Symbol, List<Symbol>> followDict { get; set; }
        private bool isGrammarAnalyzed { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            btnShowAnalysis.IsEnabled = false;
            btnAnalyzeInput.IsEnabled = false;
            isGrammarAnalyzed = false;
        }

        private void clear()
        {
            analyser = null;
            firstDict = null;
            followDict = null;
            dgSteps.ItemsSource = null;
            dgSteps.Items.Clear();
            listviewN.Items.Clear();
            listviewV.Items.Clear();
            listviewP.Items.Clear();
            btnShowAnalysis.IsEnabled = false;
            btnAnalyzeInput.IsEnabled = false;
            isGrammarAnalyzed = false;
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = false };
            openFileDialog.ShowDialog();
            string filename = openFileDialog.FileName;
            if (filename == "")
                return;
            clear();
            try
            {
                analyser = new Analyser(filename);
                foreach(var rule in analyser.grammar.Rules)
                {
                    listviewP.Items.Add(new ListViewItem() { Content = rule });
                }
                foreach(var sym in analyser.grammar.N_Symbols)
                {
                    listviewN.Items.Add(new ListViewItem() { Content = sym });
                }
                foreach(var sym in analyser.grammar.V_Symbols)
                {
                    listviewV.Items.Add(new ListViewItem() { Content = sym });
                }
            }
            catch(Exception ex)
            {
                analyser = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void btnAnalyzeGrammar_Click(object sender, RoutedEventArgs e)
        {
            if(analyser == null)
            {
                MessageBox.Show("未读取到文法");
                return;
            }
            if (isGrammarAnalyzed)
            {
                return;
            }
            firstDict = analyser.getAllFirst();
            followDict = analyser.getAllFollow();
            analyser.analyzeGrammar();

            btnShowAnalysis.IsEnabled = true;
            btnAnalyzeInput.IsEnabled = true;
            isGrammarAnalyzed = true;
            MessageBox.Show("分析成功");
        }

        private void btnShowAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (analyser == null)
            {
                return;
            }
            var index = cbChoices.SelectedIndex;
            switch (index)
            {
                case 0:
                    FirstFollowWindow ffWindow = new FirstFollowWindow();
                    ffWindow.Show();
                    break;
                case 1:
                    DFAWindow dfaWindow = new DFAWindow();
                    dfaWindow.Show();
                    break;
                case 2:
                    AnalysisTableWindow atWindow = new AnalysisTableWindow();
                    try
                    {
                        atWindow.Show();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                default:
                    break;
            }
        }

        private void btnAnalyzeInput_Click(object sender, RoutedEventArgs e)
        {
            if (analyser == null)
            {
                return;
            }
            string inputString = tbInput.Text;
            DataTable table = new DataTable();
            try
            {
                bool isAccepted = analyser.analyseInput(inputString, out table);
                dgSteps.ItemsSource = table.DefaultView;
                tbResult.Text = isAccepted ? "输入串符合文法" : "输入串不符合文法";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
