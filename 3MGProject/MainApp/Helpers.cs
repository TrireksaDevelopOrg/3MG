using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static Task SendeEmail(string email,string file,string subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("trimg.jayapura@gmail.com");
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = "Lihat Attachment";
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(file);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("trimg.jayapura", "Sony@7777");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                ShowMessage("Terkirim");
                return Task.FromResult(1);
            }
            catch (Exception)
            {
                ShowErrorMessage("Tidak Terkirim");
                return Task.FromResult(0);
            }
        }

        public static Task<string> ExportReportToPDF(ReportViewer ReportViewer1, string reportName)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            byte[] bytes = ReportViewer1.LocalReport.Render(
               "PDF", null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            Random rand = new Random();

            string filename = Path.Combine(Path.GetTempPath(), reportName+rand.Next(0,10)+"."+filenameExtension);

            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            return Task.FromResult(filename);
        }

        public static bool IsValidEmail(string strIn)
        {
          var  invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                throw new SystemException();
            }
            return match.Groups[1].Value + domainName;
        }
        
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
