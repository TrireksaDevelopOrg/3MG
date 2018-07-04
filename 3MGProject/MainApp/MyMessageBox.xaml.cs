using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MyMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {
        private MyMessageViewModel vm;
        public MessageBoxResult MessageResult { get; set; }

        public MyMessageBox(string title, string message, MessageBoxButton type)
        {
            InitializeComponent();
            vm = new MyMessageViewModel() { WindowClose = this.Close };
            vm.MyTitle = title;
            this.textMessage.Text = message;
            this.DataContext = vm;
            switch (type)
            {
                case MessageBoxButton.OK:
                   buttonPanel.Children.Add(new Button { Content = "OK" });
                    break;
                case MessageBoxButton.YesNo:
                    buttonPanel.Children.Add(new Button { Content = "Ya" });
                    buttonPanel.Children.Add(new Button { Content = "Tidak" });
                    break;
                case MessageBoxButton.YesNoCancel:
                    buttonPanel.Children.Add(new Button { Content = "Ya" });
                    buttonPanel.Children.Add(new Button { Content = "Tidak" });
                    buttonPanel.Children.Add(new Button { Content = "Batal" });
                    break;
                case MessageBoxButton.OKCancel:
                    buttonPanel.Children.Add(new Button { Content = "OK" });
                    buttonPanel.Children.Add(new Button { Content = "Batal" });
                    break;
                default:
                    break;
            }


            foreach(Button item in buttonPanel.Children)
            {
                item.Click += Item_Click;
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            var result = (Button) sender;
            this.MessageResult = GetButtonResult(result.Content.ToString());
            this.Close();
        }

        private MessageBoxResult GetButtonResult(string result)
        {
            if (result == "OK")
                return MessageBoxResult.OK;
            if (result == "Ya")
                return MessageBoxResult.Yes;
            if (result == "Tidak")
                return MessageBoxResult.No;
            if (result == "Batal")
                return MessageBoxResult.Cancel;

            return MessageBoxResult.None;
        }
    }

    


    public class MyMessageViewModel:BaseNotify
    {
        public MyMessageViewModel()
        {
            MyTitle = "Info";
            Buttons = new List<Button>();
            
        }

       

        public List<Button> Buttons { get; set; }
        public Action WindowClose { get;  set; }
    }



    public static class MyMessage
    {
        public static MessageBoxResult Show(string message, string title, MessageBoxButton type)
        {
            SystemSounds.Beep.Play();
            var forms = new MyMessageBox(title, message, type);
            forms.ShowDialog();
            return forms.MessageResult;
        }
    }
}
