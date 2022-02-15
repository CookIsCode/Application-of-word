using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
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

namespace Application_of_word.Page
{
    /// <summary>
    /// PortandExport.xaml 的交互逻辑
    /// </summary>
    public partial class PortAndExport : System.Windows.Controls.Page
    {
        public PortAndExport()
        {
            InitializeComponent();
            Database db = new Database(@"./Database/Word.db");
            var obj = db.FindTables();
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
                if ((bool)(this.PortMode1.IsChecked))
                {

                }
                else if ((bool)(this.PortMode2.IsChecked))
                {

                }
                else if ((bool)(this.PortMode3.IsChecked))
                {
                    this.DataGridSheet.DataContext = ReadXmlFile(obj.FileName);
                }
            }
        }

        //处理有道词典中导出数据 数据格式为xml
        private ObservableCollection<NoteBookWord> ReadXmlFile(string _path)
        {
            var list =new ObservableCollection<NoteBookWord>();
            if (File.Exists(_path))
            {
                int i = 1;
                XDocument xmldoc = XDocument.Load(_path);
                try
                {
                    foreach (var item in xmldoc.Element("wordbook").Elements("item"))
                    {
                        list.Add(new NoteBookWord(i++, item.Element("word").Value, item.Element("trans").Value, item.Element("phonetic").Value));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+"\n 无法识别的xml文档,请使用有道词典导出的xml文档");
                    return null;
                }
                return list;
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
                if(item.Name !=(e.OriginalSource as CheckBox).Name)
                {
                    item.IsChecked = false;
                }
            }
            e.Handled = true;
        }
    }

    public class NoteBookWord
    {
        public int ID { get; set; }
        public string Word { get; set; }
        public string Translation { get; set; }
        public string PhoneticSymbol { get; set; }
        public NoteBookWord(int _id, string _word, string _translation, string _phoneticsymbol="")
        {
            ID = _id;
            Word = _word;
            Translation = _translation;
            PhoneticSymbol = _phoneticsymbol;
        }
    }
}
