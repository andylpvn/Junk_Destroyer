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
using System.Diagnostics;
using System.IO;

//for running PS scripts inside .NET
using System.Management.Automation;

//for JSON
using Newtonsoft.Json;


namespace JunkDestroyer
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       
        //for Refresh button
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            PowerShell ps = PowerShell.Create();

            string param1 = "Get-AppxPackage | Select Name";
            //string param2 = "";
            //string param3 = "";

            //concatenate all the PS scripts
            ps.AddScript(param1);
            //ps.AddScript(param2);
            //...

            //invoke the PS script
            var result = ps.Invoke();

            //add app list to the listBox
            foreach (var app in result)
            {
                lbApps.Items.Add(app);
            }

            // serialize JSON to a string and then write string to a file
            String path = @"c:\appList.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(result));

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, result);
            }
        }


        public void populate_appListLV()
        // This function create a PS script and run the script to output to the listBox
      
        {
            PowerShell ps = PowerShell.Create();

            string param1 = "Get-AppxPackage | Select Name";
            //string param2 = "";
            //string param3 = "";


            //concatenate all the PS scripts
            ps.AddScript(param1);
            //ps.AddScript(param2);
            //...

            //invoke the PS script
            var result = ps.Invoke();
         
            //add app list to the listBox
            foreach (var app in result)
            { 
                lbApps.Items.Add(app); 
            }
        }


       //for "Select all""check box 
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

        //for Uninstall Button
        private void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {

            //mockup function to test the uninstall button
            if (lbApps.SelectedItems.Count != 0)
            {
                while (lbApps.SelectedIndex != -1)
                {
                    lbApps.Items.RemoveAt(lbApps.SelectedIndex);
                }
            }
        }

        //for Update button - open a new window
        private void UpdateAppList(object sender, RoutedEventArgs e)

        {
            UpdateWindow updateWindow = new UpdateWindow();
            updateWindow.Show();
        }

        //for radio button
        




    }
}