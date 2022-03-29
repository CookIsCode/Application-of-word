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
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace Application_of_word.Messagewindows
{
    /// <summary>
    /// SubmitDatabase.xaml 的交互逻辑
    /// </summary>
    public enum MessageBoxMode { Save, Delete };
    public partial class SubmitDatabase : Window
    {
        private MessageBoxConfig? ConfigData { get; set; }
        private Database? Db{ get; set; }
        public SubmitDatabase()
        {
            InitializeComponent();
        }
        public SubmitDatabase(MessageBoxConfig _config)
        {
            InitializeComponent();
            ConfigData = _config; Db = null;
            this.TableName.Text = _config.ConveyData;
        }

        private void TableName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
           if(CheckTableName(5,30,TableName.Text))
            {
                this.Check.Foreground = Brushes.Green;
                this.Check.Text = "命名符合要求";
            }
            else
            {
               
                this.Check.Foreground = Brushes.Red;
                this.Check.Text = "请不要使用特殊字符以及中文命名\r\n,不能用数字开头,字符数大于4小于30";
            }
        }

        private bool CheckTableName(int _min,int _max,string input)
        {
            if(input.Length>=_min&&input.Length<_max&&!(Regex.IsMatch(input, @"^[0-9]|[^A-Za-z0-9-_]")))
            {
                return true;
            }
            return false;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.TableName.Clear();
            this.TableName.Focus();
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Db == null)
            {
                Db = new Database(@"./Database/Word.db");
            }
            SaveTableDataIntoDataBase();
            this.Close();
        }

        private void SaveTableDataIntoDataBase()
        {
            int i = 0;
            List<string> Sqllist = new List<string>();
            string SqlCommand = @" CREATE TABLE IF NOT EXISTS " + this.TableName.Text +
            @"(ID INT PRIMARY KEY, Word VARCHAR(100) NOT NULL,
            Trans VARCHAR(100), SystemTrans VARCHAR(100))";
            Sqllist.Add(SqlCommand);//建立单词表
            Sqllist.Add($"DELETE FROM {this.TableName.Text};");//清空表中数据
            foreach (var item in this.ConfigData.TableData)
            {
                i++;
                SqlCommand = $"INSERT INTO {this.TableName.Text}" +
                @" (ID,Word,Trans)" + " VALUES" + $"({i},\'{item.Word}\',\'{item.Translation}\');";
                Sqllist.Add(SqlCommand);
            }
            Db.AsynExecuteSqlCommand(Sqllist);//插入数据
        }
    }
    /// <summary>
    /// 用于配置消息框
    /// </summary>
    public class MessageBoxConfig
    {
        
        public string ConveyData { get; set; }
        public MessageBoxMode Mode { get; set; }
        public ObservableCollection<Application_of_word.Page.NoteBookWord>? TableData { get; set; }
        public MessageBoxConfig(MessageBoxMode _mode,string _conveydata, ObservableCollection<Application_of_word.Page.NoteBookWord>? _tabledata)
        {
            ConveyData = _conveydata;
            TableData = _tabledata;
            Mode = _mode;
        }
        public MessageBoxConfig(MessageBoxMode _mode,string _conveydata)
        {
            ConveyData = _conveydata;
            TableData = null;
            Mode = _mode;
        }

    }
}
