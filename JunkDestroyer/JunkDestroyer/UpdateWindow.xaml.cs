using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace JunkDestroyer
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = @"C:\\JunkDestroyer\\apps_getInstalled.ps1";
            process.StartInfo.CreateNoWindow = true; //not shown

            process.Start();
            process.WaitForExit();

            populate_appListLV();
        }

        private void populate_appListLV()
        // This function loops through the installed_names csv file
        // It populates the appList list view with everything but the header.
        {
            int count = 0;

            foreach (string line in System.IO.File.ReadLines(@"C:\\JunkDestroyer\\installed_names.csv"))
            {
                count++;

                // omit the header
                if (count != 1 && count != 2)
                {
                    lbUpdateList.Items.Add(line);
                }
            }
        }
    }
}
