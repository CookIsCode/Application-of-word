using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
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

namespace Application_of_word.Page
{
    /// <summary>
    /// PortandExport.xaml 的交互逻辑
    /// </summary>
    public partial class PortAndExport : System.Windows.Controls.Page
    {
        Database? db=null;
        public PortAndExport()
        {
            InitializeComponent();
            db = new Database(@"./Database/Word.db");
            this.SelectTable.DataContext = db.FindTables(true); 
        }

        private void Brower_Click(object sender, RoutedEventArgs e)
        {
            var obj = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "文本文件|*.txt;*.xml"
            };
            if ((bool)obj.ShowDialog())
            {
                this.SelectFileName.Text = obj.FileName;
                this.DataGridSheet.DataContext = null;
                //针对TXT
                if ((bool)(this.PortMode1.IsChecked))
                {
                }
                //针对欧路词典
                else if ((bool)(this.PortMode2.IsChecked))
                {
                    this.DataGridSheet.DataContext = ReadTxtFile(obj.FileName);
                }
                //针对有道词典
                else if ((bool)(this.PortMode3.IsChecked))
                {
                    this.DataGridSheet.DataContext = ReadXmlFile(obj.FileName);
                }
            }
        }

        //处理有道词典中导出数据 数据格式为xml
        private ObservableCollection<NoteBookWord> ReadXmlFile(string _path)
        {
            if (File.Exists(_path))
            {
                int i = 1;
                var list = new ObservableCollection<NoteBookWord>();
                try
                {
                    XDocument xmldoc = XDocument.Load(_path);
                    foreach (var item in xmldoc.Element("wordbook").Elements("item"))
                    {
                        list.Add(new NoteBookWord(i++, item.Element("word").Value, item.Element("trans").Value, item.Element("phonetic").Value));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n 无法识别的xml文档,请使用有道词典导出的xml文档");
                    return null;
                }
                return list;
            }
            return null;
        }
        private ObservableCollection<NoteBookWord> ReadTxtFile(string _path)
        {
            if (File.Exists(_path))
            {
                var list = new ObservableCollection<NoteBookWord>();
                int i = 1;
                MatchCollection matches = Regex.Matches(File.ReadAllText(_path), @"@(.+)@(.+)");
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        list.Add(new NoteBookWord(i++, match.Groups[1].Value, match.Groups[2].Value));
                    }
                    return list;
                }
                MessageBox.Show("无法识别的欧陆字典的导出,请使用欧陆词典导出的txt文档");
            }
            return null;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PortMode_Checked(object sender, RoutedEventArgs e)
        {
            List<CheckBox> obj = new List<CheckBox>();
            obj.Add(this.PortMode1); obj.Add(this.PortMode2); obj.Add(this.PortMode3);
            foreach (var item in obj)
            {
                if (item.Name != (e.OriginalSource as CheckBox).Name)
                {
                    item.IsChecked = false;
                }
            }
            e.Handled = true;
        }

        private void PortSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.DataGridSheet.DataContext!=null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
            e.Handled = true;
        }

        private void PortSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Match match = Regex.Match(this.SelectFileName.Text, @".+\\(.+)[.]");
            var obj = new Application_of_word.Messagewindows.SubmitDatabase
                (new Messagewindows.MessageBoxConfig(Messagewindows.MessageBoxMode.Save, match.Groups[1].Value,
                this.DataGridSheet.DataContext as ObservableCollection<NoteBookWord>));
            obj.WindowStartupLocation = WindowStartupLocation.CenterOwner;//设置消息框显示的位置
            obj.Owner = Window.GetWindow(this);//设置消息框的父窗口
            obj.ShowDialog();
            this.SelectTable.DataContext =db.FindTables(true);
            e.Handled=true;
        }

        private void DeleteTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.DataGridSheet.DataContext != null && this.SelectTable.SelectedItem != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
            e.Handled = true;
        }

        private void DeleteTable_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string Tip= $"请确认是否删除{((string)this.SelectTable.SelectedItem)}词书";
            var State = MessageBox.Show(Tip,"警告信息",MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if(State == MessageBoxResult.Yes)
            {
                db.ExecuteSqlCommand($"DROP TABLE {(string)(this.SelectTable.SelectedItem)}");
                this.SelectTable.DataContext = db.FindTables(true);
                this.DataGridSheet.DataContext = null;
            }
            e.Handled = true;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            //this.DataGridSheet.CanUserAddRows = true;
            var obj = this.DataGridSheet.DataContext as ObservableCollection<NoteBookWord>;
            obj.Add(new NoteBookWord(9999, "", ""));
            //this.DataGridSheet.CanUserAddRows = false;
        }

        private void SelectTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.SelectTable.SelectedItem != null)
            {
                ParameterizedThreadStart action = (str) =>
                {
                    var QueryResult=ShowDataBaseData( db.ReadDatabase($"SELECT * FROM {str as string}"));
                    //使用WPF自带任务调动来刷新页面
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,()=> this.DataGridSheet.DataContext= QueryResult);
                };
                Thread QueryThread = new Thread(action);
                QueryThread.Priority = ThreadPriority.Normal;
                QueryThread.Start(this.SelectTable.SelectedItem);
            }
        }

        //将数据库查询结果转换为可显示的对象
        private ObservableCollection<NoteBookWord> ShowDataBaseData(Microsoft.Data.Sqlite.SqliteDataReader _queryresult)
        {
            ObservableCollection<NoteBookWord> data = new ObservableCollection<NoteBookWord>();
            while (_queryresult.Read())
            {
                data.Add(new NoteBookWord(_queryresult.GetInt32(0)
               , _queryresult.GetString(1), _queryresult.GetString(2)));
            }
            return data;
        }
    }

    public class NoteBookWord
    {
        public int ID { get; set; }
        public string Word { get; set; }
        public string Translation { get; set; }
        public string PhoneticSymbol { get; set; }
        public NoteBookWord(int _id, string _word, string _translation, string _phoneticsymbol = "")
        {
            ID = _id;
            Word = _word;
            Translation = _translation;
            PhoneticSymbol = _phoneticsymbol;
        }
    }
}
