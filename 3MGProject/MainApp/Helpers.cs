using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Xml;
using DataAccessLayer.DataModels;
using MainApp.Reports.Forms;
using Microsoft.Reporting.WinForms;
using Ocph.DAL;

namespace MainApp
{
    public class Helpers
    {
        public static user UserLogin { get; internal set; }

        internal static MessageBoxResult ShowErrorMessage(string message)
        {
            return  MyMessage.Show(message, "Error", MessageBoxButton.OK);
        }

        internal static MessageBoxResult ShowMessage(string message)
        {
           return MyMessage.Show(message, "Info", MessageBoxButton.OK);
        }

        internal static MessageBoxResult ShowMessage(string message, string title)
        {
           return MyMessage.Show(message, title, MessageBoxButton.OK);
        }

        public static void  ShowChild(Window parent, Window child)
        {
            child.Owner = parent;
            child.ShowDialog();
        }

      

        internal static void PrintWithFormActionTwoSource(string title, ReportDataSource dataHeader, ReportDataSource items, string layout, ReportParameter[] parameters)
        {
            var content = new ReportContent(dataHeader, items, layout, parameters);
            Style style = Application.Current.FindResource("WindowKey") as Style;
            var dlg = new Window
            {
                Style=style,
                Content = content, AllowsTransparency=false,
                Title = "",
                ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip,
                WindowState = WindowState.Maximized,
            };

            content.WindowClose = dlg.Close;
            var vm = new BaseNotify();
            vm.MyTitle = title;
            dlg.DataContext = vm;
            dlg.ShowDialog();
        }

        internal static void PrintPreviewWithFormAction(string title, ReportDataSource source, string layout, ReportParameter[] parameters)
        {
            //TrireksaApp.Reports.Layouts.Nota.rdlc"
            var content = new ReportContent(source, layout, parameters);
            Style style = Application.Current.FindResource("WindowKey") as Style;
            var dlg = new Window
            {
                Style=style, AllowsTransparency=false,
                Content = content,
                Title = "",
                ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip,
                WindowState = WindowState.Maximized,
            };

            content.WindowClose = dlg.Close;
            var vm = new BaseNotify();
            vm.MyTitle = title;
            dlg.DataContext = vm;
            dlg.ShowDialog();
        }

    }
}
