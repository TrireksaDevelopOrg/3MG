using FoxLearn.License;
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
    /// Interaction logic for RegistrationProduct.xaml
    /// </summary>
    public partial class RegistrationProduct : Window
    {
        public bool Success { get; private set; }

        public RegistrationProduct()
        {
            InitializeComponent();
            Loaded += RegistrationProduct_Loaded;
        }

        private void RegistrationProduct_Loaded(object sender, RoutedEventArgs e)
        {
            textProductId.Text = ComputerInfo.GetComputerId();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {

            KeyManager key = new KeyManager(textProductId.Text);
            string productKey = textSerialKey.Text;
            
            try
            {
                if (key.ValidKey(ref productKey))
                {
                    KeyValuesClass kv = new KeyValuesClass();
                    if (key.DisassembleKey(productKey, ref kv))
                    {
                        LicenseInfo lic = new LicenseInfo();
                        lic.FullName = "TRIMG AIR LINES";
                        lic.ProductKey = productKey;
                       if( key.SaveSuretyFile("Key.lic", lic))
                        {
                            MessageBox.Show("Sukes Registrasi");
                            this.Success = true;
                            this.Close();
                        }else
                        {
                            throw new SystemException("Serial Key Tidak Tersimpan !");
                        }
                       
                    }
                    else
                        throw new SystemException("Invalid Registration !");
                }
                else
                    throw new SystemException("Serial Key Invalid !");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Success = false;
            this.Close();
        }
    }
}
