using Ocph.DAL;
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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    /// 

    public delegate void CariAction(string message);
    public partial class SearchView : Window
    {
        public event CariAction OnCari;
        public SearchView()
        {
            InitializeComponent();
            this.DataContext = new SerachViewModel();
            Loaded += SearchView_Loaded;
        }

        private void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            searchBox.Focus();
        }

        private void SearchView_OnCari(string message)
        {
            throw new NotImplementedException();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if(e.Key== Key.Escape)
            {
              //  OnCari?.Invoke(string.Empty);
                this.Close();
            }
            if(e.Key== Key.Enter || e.Key== Key.Return)
            {
                OnCari?.Invoke(searchBox.Text);
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }
    }



    public class SerachViewModel :BaseNotify
    {
        public SerachViewModel()
        {
            MyTitle = "Search Box";
        }
       
    }
}
