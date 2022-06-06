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

namespace JunkDestroyer
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        List<appName> updateList = new List<appName>();

        public UpdateWindow()
        {
            InitializeComponent();
        }

        //show combobox for each value



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
             lbCustomList.ClearValue(ItemsControl.ItemsSourceProperty);
           // lbCustomList.ItemsSource = null;
          // String selectedItem = lbAllApps.SelectedValue.ToString();
            if (lbAllApps.SelectedIndex != -1)
            {
                //lbCustomList.Items.Add(selectedItem);
               lbCustomList.Items.Add(lbAllApps.SelectedValue);
               lbAllApps.Items.Remove(lbAllApps.SelectedValue);
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

            //check if Temp folder already existed or not
            if (!Directory.Exists(@"C:\Temp"))
            {
                Directory.CreateDirectory(@"C:\Temp"); // create a new Temp folder
            }

            //get value from combobox
            String dbName = cbAppList.Text;

            //create a file path for the database
            var path = $@"C:\Temp\{dbName}.json";

            //assign all variables in the ListBox to the ArrayList
            foreach (var item in lbCustomList.Items)
            {

                appName name = new appName()
                {
                    Name = item.ToString() //convert each item in the ListBox to string, and assign to appName class in Applist.cs
                };

                updateList.Add(name); //add items from the above object to the ArrayList in class Applist.cs

            }

            //Serialize the ArrayList to JSON format and write to JSON file
            var appList = JsonConvert.SerializeObject(updateList);
            File.WriteAllText(path, appList);

            //show a notification to user
            MessageBox.Show("You have successfully updated the " + dbName + " database in " + path);
        }

        private void cbAppList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //clear ArrayString ItemsSource
            lbCustomList.ClearValue(ItemsControl.ItemsSourceProperty);

            var PersonalPath = $@"C:\Temp\Personal.json";
            var BusinessPath = $@"C:\Temp\Business.json";
            var CustomPath = $@"C:\Temp\Custom.json";
     
            int index = cbAppList.SelectedIndex;
                    
            if (index == 0)
            {           
                //clear Listbox
                lbCustomList.Items.Clear();

                string jsonPersonal = File.ReadAllText(PersonalPath);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(jsonPersonal);
               
                foreach (var name in appList)
                {
                    lbCustomList.Items.Add(name);
                }

            }
            else if (index == 1)
            {             
                //clear Listbox
                lbCustomList.Items.Clear();

                string jsonBusiness = File.ReadAllText(BusinessPath);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(jsonBusiness);
                foreach (var name in appList)
                {
                    lbCustomList.Items.Add(name);
                }
            }
            else if (index == 2)
            {              
                //clear Listbox
                lbCustomList.Items.Clear();

                string jsonCustom = File.ReadAllText(CustomPath);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(jsonCustom);
                foreach (var name in appList)
                {
                    lbCustomList.Items.Add(name);
                }
            }

        }
    }
}
