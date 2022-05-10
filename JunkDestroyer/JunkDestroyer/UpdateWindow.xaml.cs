using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
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

using System.Web.Script.Serialization;

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



        //Refresh button
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            populateApp();
        }

        private void populateApp()
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
                // trim the string to get specific app names
                String s = app.ToString();
                int start = s.IndexOf("=") + 1;
                int end = s.IndexOf("}", start);
                string final = s.Substring(start, end - start);

                //add names to the ListBox                
                lbAllApps.Items.Add(final);
            }
        }

        //Add button
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (lbAllApps.SelectedIndex != -1)
            {
                lbCustomList.Items.Add(lbAllApps.SelectedValue);
                lbAllApps.Items.Remove(lbAllApps.SelectedValue);
            }
        }
        //Remove button
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbCustomList.SelectedIndex != -1)
            {
                lbAllApps.Items.Add(lbCustomList.SelectedValue);
                lbCustomList.Items.Remove(lbCustomList.SelectedValue);
            }
        }

        //Save button
        private void bntSave_Click(object sender, RoutedEventArgs e)
        {
            populateApp();







            //check if Temp folder already existed or not
            if (!Directory.Exists(@"C:\Temp"))
            {
                Directory.CreateDirectory(@"C:\Temp"); // create a new Temp folder
            }

            //get value from combobox
            String dbName = cbAppList.Text;

            //create a file path for the database
            var path = $@"C:\Temp\{dbName}.json";

            // Create a new JavaScriptSerializer to convert our object to and from a json string
            JavaScriptSerializer jss = new JavaScriptSerializer();
            // Use the JavaScriptSerializer to convert the ListBox items into a Json string
            string writeJson = jss.Serialize(lbCustomList.Items);
            // Write this string to file
            File.WriteAllText(path, writeJson);


            //show a notification to user
            MessageBox.Show("You have successfully updated the " + dbName + " database in " + path);
        }


    }
}
