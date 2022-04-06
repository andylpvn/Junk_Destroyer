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
using System.Diagnostics;
using System.IO;

namespace UWPApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void get_installed_names(object sender, EventArgs e)
        {
            // This function runs the apps_getInstalled
            var process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = @"C:\\Users\\jacob\\OneDrive\\Desktop\\UWPApp\\apps_getInstalled.ps1";

            process.Start();
            process.WaitForExit();

            populate_appListLV();
            // ...
        }

        private void populate_appListLV()
            // This function loops through the installed_names csv file
            // It populates the appList list view with everything but the header.
        {
            int count = 0;

            foreach (string line in System.IO.File.ReadLines(@"C:\\Users\\jacob\\OneDrive\\Desktop\\UWPApp\\installed_names.csv"))
            {
                count++;      
                
                // omit the header
                if (count != 1 && count != 2)
                {
                    appsLV.Items.Add(line);
                }    
            }
        }

        private void open_installed(object sender, EventArgs e)
        {
            // use powershell to open the list of installed apps
            var process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = @"C:\\Users\\jacob\\OneDrive\\Desktop\\UWPApp\\installed_full.csv";

            process.Start();
            process.WaitForExit();
            // ...
        }

        private void remove_installed(object sender, RoutedEventArgs e)
        {
            // use powershell to remove the installed UWP apps
            var process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = @"C:\\Users\\jacob\\OneDrive\\Desktop\\UWPApp\\apps-mockUninstall.ps1";

            process.Start();
            process.WaitForExit();
            // ...
        }
    }
}
