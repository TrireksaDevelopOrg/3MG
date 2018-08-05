using DataAccessLayer.Models;
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
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : Window
    {
        public HistoryView(List<History> list, string title)
        {
            InitializeComponent();
            this.DataContext = new HistoryViewModel(list.OrderBy(O=>O.CreatedDate).ToList()) { WindowClose=Close,MyTitle=title};
        }
    }

    public class HistoryViewModel:BaseNotify
    {
        public HistoryViewModel(List<History> list)
        {
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(list);
            CancelCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = x => WindowClose() };
            
        }

        public CollectionView SourceView { get; }
        public CommandHandler CancelCommand { get; }
        public Action WindowClose { get; internal set; }
    }
}
