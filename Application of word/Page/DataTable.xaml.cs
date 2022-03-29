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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Collections.ObjectModel;

namespace Application_of_word.Page
{
    /// <summary>
    /// DataTable.xaml 的交互逻辑
    /// </summary>
    public partial class DataTable:System.Windows.Controls.Page
    {
        List<string> Choses = new List<string> { "n", "v", "vi", "vt", "adj", "adv", "prep", "conj", "int", "aux", "num" };
        public DataTable()
        {
            InitializeComponent();
            InitalControls();
        }

        private void DataTablePage_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void InitalControls()
        {
            InitialSlectTable();
            InitialSearchMode();
            InitialSelectBook();
            InitialComboxOfSearchWordType();
            InitialSearboxBingding();
        }

        private void InitialDataGridSheet()
        {
            this.DataGridSheet.Columns.Clear();
            var ColName = new List<string> { "Word", "UKPhoneticSymbol" };
            foreach (var item in SearchWordType.SelectedItems)
            {
                ColName.Add(item as string);
            }    
            var IsGold=new DataGridCheckBoxColumn();
            IsGold.Header = "IsGold";
            IsGold.Binding = new Binding("IsGold");
            this.DataGridSheet.Columns.Add(IsGold);
            var IDcol = new DataGridTextColumn();
            IDcol.Header = "ID";
            IDcol.Binding = new Binding("ID");
            this.DataGridSheet.Columns.Add(IDcol);
            for (int i = 0; i < ColName.Count; i++)
            {
                var obj = new DataGridTextColumn();
                obj.Header = ColName[i];
                obj.Binding = new Binding($"WordData[{i}]");
                this.DataGridSheet.Columns.Add(obj);
            }
        }
        private string MakeSqlStatement()
        {
            string sql = "SELECT Word,UKPhoneticSymbol,";
            foreach (var item in SearchWordType.SelectedItems)
                sql += (string)item + ',';
            sql=sql.Remove(sql.Length - 1);
            if (SelectBook.SelectedIndex != 1)
            {
                if(SearchMode.SelectedIndex==0)
                    sql += $" FROM AllWord WHERE ((Word LIKE \'%{SearchBox.Text}%\') AND (Book =\'{SelectBook.SelectedItem as string}\')) AND (";
                else if(SearchMode.SelectedIndex==1)
                    sql += $" FROM AllWord WHERE ((Word LIKE \'{SearchBox.Text}%\') AND (Book =\'{SelectBook.SelectedItem as string}\')) AND (";
                else if(SelectBook.SelectedIndex==2)
                    sql += $" FROM AllWord WHERE ((Word LIKE \'%{SearchBox.Text}\') AND (Book =\'{SelectBook.SelectedItem as string}\')) AND (";
            }
            else
            {
                if(SearchMode.SelectedIndex==0)
                    sql += $" FROM AllWord WHERE (Word LIKE \'%{SearchBox.Text}%\') AND (";
                else if(SearchMode.SelectedIndex == 1)
                    sql += $" FROM AllWord WHERE (Word LIKE \'{SearchBox.Text}%\') AND (";
                else if (SearchMode.SelectedIndex == 2)
                    sql += $" FROM AllWord WHERE (Word LIKE \'%{SearchBox.Text}\') AND (";
            }
            foreach (var item in SearchWordType.SelectedItems)
            {
                sql += $"({(string)item} IS NOT NULL) OR ";
            }
            sql=sql.Remove(sql.Length - 4);
            sql += ");";
            return sql;
        }

        private void InitialComboxOfSearchWordType()
        {
            List<string> Chose = new List<string> {"n", "v", "vi", "vt", "adj", "adv", "prep", "conj", "int", "aux", "num" };
            SearchWordType.ItemsSource = Chose;
            SearchWordType.SelectAll();
        }
        private void InitialSearboxBingding()
        {
            this.SearchBox.Command = ApplicationCommands.New;
            SearchBox.CommandTarget = SearchBox;
            var cb = new CommandBinding();
            cb.Command = ApplicationCommands.New;
            cb.CanExecute += SearchBox_CanExecute;
            this.SearchBox.CommandBindings.Add(cb);
        }

        private void InitialSelectBook()
        {
            Database db = new Database(@"./Database/Word.db");
            //根据用于单词库的选择来显示数据
            if (this.SelectTable.SelectedIndex == 0)
            {
                this.SelectBook.DataContext = db.FindBooksName("AllWord");
                this.SelectBook.SelectedIndex=0;
            }
            else
            {
                this.SelectBook.DataContext=db.FindTables(true);
            }
        }
        private void InitialSlectTable()
        {
            List<string> Selections= new List<string> { "内置单词库","自定义词库","测试用词库"};
            this.SelectTable.ItemsSource = Selections;
            this.SelectTable.SelectedIndex = 0;
        }

        private void InitialSearchMode()
        {
            List<string> Selections = new List<string> { "任意位置", "开头位置", "末尾位置" };
            this.SearchMode.ItemsSource = Selections;
            SearchMode.SelectedIndex = 0;
        }

        private void ChooseAll_Click(object sender, RoutedEventArgs e)
        {
            var items = this.DataGridSheet.Items;
            if(this.DataGridSheet.DataContext!=null)
            {
                foreach(var item in items)
                {
                    (item as Word).IsGold = (bool)this.ChooseAll.IsChecked ;
                }
                items.Refresh();
            }
            e.Handled = true;
        }

        private void Review_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.SearchWordType.SelectedItems.Count.ToString());
        }

        private void SearchBox_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            ParameterizedThreadStart action = (_confige) =>
            {   
                object[] Confige = _confige as object[]; 
                Database db = new Database(@"./Database/Word.db");
                var DataBaseData = db.ReadDatabase(Confige[0] as string);
                if (DataBaseData == null) return;
                var data = new ObservableCollection<Word>();
                int i = 1;int count = (int)Confige[1];
                while(DataBaseData.Read())
                {
                    data.Add(new Word(DataBaseData, i++,count));
                }
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                {
                    //如果全选则显示前初始化为true
                    if (this.ChooseAll.IsChecked == true)
                        foreach (var item in data)
                        {
                            item.IsGold = true;
                        }
                    this.DataGridSheet.DataContext = data;
                });
            };
            InitialDataGridSheet();
            object[] confige = new object[] { MakeSqlStatement(), this.SearchWordType.SelectedItems.Count + 2 };
            Thread thread = new Thread(action);
            thread.Start(confige);
            
        }

        private void SearchBox_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectTable.SelectedIndex == 0 && SearchWordType.SelectedItems.Count > 0&&SelectBook.SelectedIndex>=1)
                e.CanExecute = true;
            else { e.CanExecute = false; }
            e.Handled = true;
        }

        private void SelectTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //选择内置单词库
            if (this.SelectTable.SelectedIndex == 0) { this.SearchWordType.IsEnabled=true; InitialSelectBook(); }
            else { this.SearchWordType.IsEnabled=false;  InitialSelectBook(); }//选择自定义单词库
        }

        private void SelectBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectBook.SelectedIndex==0)
                this.DataGridSheet.DataContext = null;
        }
    }
}