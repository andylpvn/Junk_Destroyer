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

using JunkDestroyer.JSON;
using System.Collections.ObjectModel;
using System.Reflection;

namespace JunkDestroyer
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        List<appName> updateFullName = new List<appName>();
        

        public UpdateWindow()
        {
            InitializeComponent();
        }

        //Refresh button
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            lbAllApps.Items.Clear();
            populateApp();
        }

        private void populateApp()
        {
            //PowerShell ps = PowerShell.Create();

            //string param1 = "Get-AppxPackage | Select Name";
            ////string param2 = "";
            ////string param3 = "";


            ////concatenate all the PS scripts
            //ps.AddScript(param1);
            ////ps.AddScript(param2);
            ////...

            ////invoke the PS script
            //var appUpdates = ps.Invoke();

            var appUpdates = PowerShell.Create().AddScript("Get-AppxPackage | Select Name").Invoke();

            //add app list to the listBox
            foreach (var app in appUpdates)
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
            // lbCustomList.ClearValue(ItemsControl.ItemsSourceProperty);
            // lbCustomList.ItemsSource = null;
            // String selectedItem = lbAllApps.SelectedValue.ToString();
            if (lbAllApps.SelectedIndex != -1)
            {

                if (lbCustomList.Items.Contains(lbAllApps.SelectedItem.ToString()))
                {
                    MessageBox.Show("The item is already on the list. Let's try with another application");
                }
                else
                {
                    lbCustomList.Items.Add(lbAllApps.SelectedValue);
                    lbAllApps.Items.Remove(lbAllApps.SelectedValue);
                }
            }
        }
        //Remove button
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //lbCustomList.ItemsSource = null;
            lbCustomList.ClearValue(ItemsControl.ItemsSourceProperty);

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

            //call root path where the application is located
            string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            //check if Database folder already existed or not in root
            if (!Directory.Exists($@"{currentDirectory}\Database"))
            {
                Directory.CreateDirectory($@"{currentDirectory}\Database"); // create a new Database folder in root folder
            }

            //get value from combobox
            String dbName = cbAppList.Text;

            //create a file path for the database  
            var path = $@"{currentDirectory}\Database\{dbName}.json";
         
            //assign all variables in the ListBox to the ArrayList
            foreach (var item in lbCustomList.Items)
            {

                appName name = new appName()
                {
                    Name = item.ToString() //convert each item in the ListBox to string, and assign to appName class in Applist.cs
                };

                updateFullName.Add(name); //add items from the above object to the ArrayList in class Applist.cs

            }

            //Serialize the ArrayList to JSON format and write to JSON file
            var appList = JsonConvert.SerializeObject(updateFullName);
         
            File.WriteAllText(path, appList);


            //show a notification to user
            MessageBox.Show("You have successfully updated the " + dbName + " database in " + path);
        }

        private void cbAppList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //call root path where the application is located
            string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
           
            var CustomPath = $@"{currentDirectory}\Database\Custom.json";
            int index = cbAppList.SelectedIndex;

            if (index == 0)
            {
                //clear Listbox
                lbCustomList.Items.Clear();

                string jsonCustom = File.ReadAllText(CustomPath);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(jsonCustom);
                foreach (var name in appList)
                {
                    String n = name.ToString();
                    lbCustomList.Items.Add(n);
                }

            }
            
        }
    }
}
