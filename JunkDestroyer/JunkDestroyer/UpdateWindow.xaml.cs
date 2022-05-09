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
                lbAllApps.Items.Add(app);
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
            if (!Directory.Exists(@"C:\Temp")) //check if the Temp folder already exist or not
            {
               Directory.CreateDirectory(@"C:\Temp"); //create a new Temp folder
            }
      
            //get value from combobox
            String dbName = cbAppList.Text;

            //create a file path for the database
            var path = $@"C:\Temp\{dbName}.json";

            //export listbox to a JSON file
            foreach (var item in lbCustomList.Items)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(item));

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, item);
                }
            }
            //show a notification to user
            MessageBox.Show("You have successfully updated the " + dbName + " database in " + path);
        }


    }   
}
