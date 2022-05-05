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


        //private void CreateCSVFiles()
        //// This function runs a powershell script which creates two .csv files.
        //// One is for Names, the other for PackageFullNames.
        //{
        //var process = new Process();
        //    process.StartInfo.FileName = "powershell.exe";
        //    process.StartInfo.Arguments = @"C:\\JunkDestroyer\\apps_getInstalled.ps1";
        //    process.StartInfo.CreateNoWindow = false; //hide script window
        //    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //hide script window

        //    process.Start();
        //    process.WaitForExit();
        //    // ...
        //}

        //private void UpdateAppList(object sender, RoutedEventArgs e)
        //// This is called when the user clicks 'Update list'
        //// The function loops through the installed_names.csv file.
        //// First, it calls CreateCSVFiles to get an updated version of the list
        //// Then, it adds an item to the list view for each line in the file. (excluding the header)
        //{
        //    CreateCSVFiles(); 

        //    int count = 0;
        //    foreach (string line in System.IO.File.ReadLines(@"C:\\JunkDestroyer\\installed_names.csv"))
        //    {
        //        count++;

        //        // omit the header
        //        if (count != 1 && count != 2)
        //        {
        //            lbApps.Items.Add(line);
        //        }
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // This function runs the apps_getInstalled
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
                    lbApps.Items.Add(line);
                }
            }
        }

        private void UpdateAppList(object sender, RoutedEventArgs e)
        {
            // use powershell to open the list of installed apps
            //var process = new Process();
            //process.StartInfo.FileName = "powershell.exe";
            //process.StartInfo.Arguments = @"C:\\JunkDestroyer\\installed_full.csv";
            //process.StartInfo.CreateNoWindow = true; //not shown

            //process.Start();
            //process.WaitForExit();
            // ...

            UpdateWindow updateWindow = new UpdateWindow();
            updateWindow.Show();
        }

        private void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            // use powershell to remove the installed UWP apps
            var process = new Process();
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = @"C:\\JunkDestroyer\\apps-mockUninstall.ps1";
            process.StartInfo.CreateNoWindow = false; //shown

            process.Start();
            process.WaitForExit();
            // ...
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (cbSelectAll.IsChecked == true)
            {
                lbApps.SelectAll();
                lbApps.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (cbSelectAll.IsChecked == false)
            {
                lbApps.UnselectAll();
                lbApps.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}