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

namespace JunkDestroyer
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


        private void CreateCSVFiles()
        // This function runs a powershell script which creates two .csv files.
        // One is for Names, the other for PackageFullNames.
        {
        var process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = @"C:\\Users\\jacob\\OneDrive\\Desktop\\UWPApp\\apps_getInstalled.ps1";
            process.StartInfo.CreateNoWindow = false; //hide script window
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //hide script window

            process.Start();
            process.WaitForExit();
            // ...
        }

        private void UpdateAppList(object sender, RoutedEventArgs e)
        // This is called when the user clicks 'Update list'
        // The function loops through the installed_names.csv file.
        // First, it calls CreateCSVFiles to get an updated version of the list
        // Then, it adds an item to the list view for each line in the file. (excluding the header)
        {
            CreateCSVFiles(); 

            int count = 0;
            foreach (string line in System.IO.File.ReadLines(@"C:\\Users\\jacob\\OneDrive\\Desktop\\UWPApp\\installed_names.csv"))
            {
                count++;

                // omit the header
                if (count != 1 && count != 2)
                {
                    lbApps.Items.Add(line);
                }
            }
        }

    }
}