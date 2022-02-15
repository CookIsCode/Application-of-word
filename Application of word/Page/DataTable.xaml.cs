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

namespace Application_of_word.Page
{
    /// <summary>
    /// DataTable.xaml 的交互逻辑
    /// </summary>
    public partial class DataTable:System.Windows.Controls.Page
    {
        public DataTable()
        {
            InitializeComponent();
            InitialSelectBook();
            InitialDataGridSheet();
        }

        private void DataTablePage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InitialDataGridSheet()
        {
            List<string> ColName = new List<string> { "Word", "USPhoneticSymbol", "UKPhoneticSymbol", "n", "v", "vi", "vt", "adj", "adv", "prep", "conj", "int", "aux", "num" };
            var IDcol = new DataGridTextColumn();
            IDcol.Header = "ID";
            this.DataGridSheet.Columns.Add(IDcol);
            for (int i = 0; i < ColName.Count; i++)
            {
                var obj = new DataGridTextColumn();
                obj.Header = ColName[i];
                obj.Binding = new Binding($"WordData[{i}]");
                this.DataGridSheet.Columns.Add(obj);
            }
        }

        private void InitialSelectBook()
        {
            Database db = new Database(@"./Database/Word.db");
            this.SelectBook.ItemsSource = db.FindBooksName("AllWord");
            this.SelectBook.SelectedItem = SelectBook.Items[0];
        }

        private void SelectBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectBook.SelectedItem as string != "None")
            {
                (DataGridSheet.Columns[0] as DataGridTextColumn).Binding = new Binding(this.SelectBook.SelectedItem == "All" ? "ID" : "WordID");
                Database db = new Database(@"./Database/Word.db");
                this.DataGridSheet.DataContext = this.DataGridSheet.DataContext = db.ReadTableData("AllWord", this.SelectBook.SelectedItem as string);
            }
            else
            {
                this.DataGridSheet.DataContext = null;
            }
        }
    }
}
