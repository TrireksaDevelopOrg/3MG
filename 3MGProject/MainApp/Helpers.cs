using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using DataAccessLayer.DataModels;
using MainApp.Reports.Forms;
using Microsoft.Reporting.WinForms;

namespace MainApp
{
    public class Helpers
    {
        public static user UserLogin { get; internal set; }

        internal static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal static void ShowMessage(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void  ShowChild(Window parent, Window child)
        {
            child.Owner = parent;
            child.ShowDialog();
        }




        private static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //
        public static int GetIntValue(string KeyName)
        {
            int vResult = 0;

            if (KeyExists(KeyName))
            {
                AppSettingsReader asr = new AppSettingsReader();
                vResult = (int)asr.GetValue(KeyName, typeof(int));
            }
            return vResult;
        }

        public static string GetStringValue(string KeyName)
        {
            string vResult = string.Empty;
            if (KeyExists(KeyName))
            {
                AppSettingsReader asr = new AppSettingsReader();
                vResult = (string)asr.GetValue(KeyName, typeof(string));
            }
            return vResult;
        }

        public static void UpdateKey(string strKey, string newValue)
        {
            if (!KeyExists(strKey))
            {
                // Add an Application Setting.
                config.AppSettings.Settings.Add(strKey, newValue);
                config.Save(ConfigurationSaveMode.Modified, true);
                // Save the configuration file.
                // Force a reload of a changed section.
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                // Add an Application Setting.

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                foreach (XmlElement element in xmlDoc.DocumentElement)
                {
                    if (element.Name.Equals("appSettings"))
                    {
                        foreach (XmlNode node in element.ChildNodes)
                        {
                            if (node.Attributes[0].Value.Equals(strKey))
                            {
                                node.Attributes[1].Value = newValue;
                            }
                        }
                    }
                }

                xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public static bool KeyExists(string strKey)
        {
            bool IsExists = false;
            foreach (string item in config.AppSettings.Settings.AllKeys)
            {
                if (item == strKey)
                {
                    IsExists = true;
                }
            }
            return IsExists;
        }

        internal static void PrintWithFormActionTwoSource(ReportDataSource dataHeader, ReportDataSource items, string layout, ReportParameter[] parameters)
        {
            var content = new ReportContent(dataHeader, items, layout, parameters);
            var dlg = new Window
            {
                Content = content,
                Title = "",
                ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip,
                WindowState = WindowState.Maximized,
            };

            dlg.ShowDialog();
        }

        internal static void PrintPreviewWithFormAction(ReportDataSource source, string layout, ReportParameter[] parameters)
        {
            //TrireksaApp.Reports.Layouts.Nota.rdlc"
            var content = new ReportContent(source, layout, parameters);
            var dlg = new Window
            {
                Content = content,
                Title = "",
                ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip,
                WindowState = WindowState.Maximized,
            };

            dlg.ShowDialog();
        }

    }
}
