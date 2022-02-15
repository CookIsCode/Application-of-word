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
using Application_of_word.Page;

namespace Application_of_word
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Controls.Page CurrentPage=null;
        private System.Windows.Controls.Page DataTablePage=null;
        private System.Windows.Controls.Page PortAndExportPage=null;
        public MainWindow()
        {
            InitializeComponent();
            
        }
                       
        private void PageBrowse_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CurrentPage != null)
            {
                CurrentPage.Height = this.PageBrowse.ActualHeight;
                CurrentPage.Width = this.PageBrowse.ActualWidth;
            }
               
        }

        private void LinkDataTable_Click(object sender, RoutedEventArgs e)
        {
            if (!(CurrentPage is DataTable))
            {
                if (PortAndExportPage == null)
                {
                    PortAndExportPage = new DataTable();   
                }
                CurrentPage = PortAndExportPage;
                this.PageBrowse.Navigate(CurrentPage);
            }
            
        }

        private void BackHome_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage = null;
            this.PageBrowse.Navigate(null);
        }

        private void LinkToPort_Click(object sender, RoutedEventArgs e)
        {
            if (!(CurrentPage is PortAndExport))
            {
                if (DataTablePage == null)
                {
                    DataTablePage = new PortAndExport();
                }
                CurrentPage = DataTablePage;
                this.PageBrowse.Navigate(CurrentPage);
            }
        }
    }
}
