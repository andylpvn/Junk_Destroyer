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

//for getting local Windows users
using System.Management;

//for running PS scripts inside .NET
using System.Management.Automation;

//for JSON
using Newtonsoft.Json;
using System.Data;
using System.Web.Script.Serialization;

using System.DirectoryServices;
using System.Collections;

namespace JunkDestroyer
{



    public partial class MainWindow : Window
    {

        public class Findings
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

        }

        public class FindingsList
        {
            [JsonProperty("Findings")]
            public IList<Findings> Findings { get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        //show installed apps by powershell scripts
        private void populateApp()
        {
            lbApps.Items.Clear();

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
                lbApps.Items.Add(final);
            }
        }

        //find all windows users in the computer
        private void findWindowsUser()
        {
            comboBoxWindowsUser.Items.Clear();
            comboBoxWindowsAdmin.Items.Clear();

            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName); //can use a specific machineName to login to that computer

            //getting all account in Admins group
            DirectoryEntry admGroup = localMachine.Children.Find("administrators", "group");
            object adms = admGroup.Invoke("members", null);
            foreach (object groupMember in (IEnumerable)adms)
            {
                DirectoryEntry adm = new DirectoryEntry(groupMember);
                comboBoxWindowsAdmin.Items.Add(adm.Name);
            }
            //getting all account in Users group
            DirectoryEntry userGroup = localMachine.Children.Find("users", "group");
            object users = userGroup.Invoke("members", null);
            foreach (object groupMember in (IEnumerable)users)
            {
                DirectoryEntry user = new DirectoryEntry(groupMember);
                comboBoxWindowsUser.Items.Add(user.Name);
            }

            ////another way to find users
            //SelectQuery query = new SelectQuery("Win32_UserAccount");
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            //foreach (ManagementObject user in searcher.Get())
            //{
            //    //get a specific last part of windows user string
            //    String s = user.ToString();
            //    var last = s.Split('=').Last();

            //    //add user into the combobox
            //    comboBoxWindowsUser.Items.Add(last.ToString());
            //    comboBoxWindowsUser.SelectedIndex = 0;
            //}

        }

        //Refresh button
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            //check the "All Applications" radio button
            rdAll.IsChecked = true;

            //uncheck the "check all" checkbox
            cbSelectAll.IsChecked = false;

            //call methods
            populateApp();
            findWindowsUser();

        }

        // "Select all" check box 
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

        // Uninstall Button
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

        // Update button - open a new window
        private void UpdateAppList(object sender, RoutedEventArgs e)

        {
            UpdateWindow updateWindow = new UpdateWindow();
            updateWindow.Show();
        }

        //show currently logged in windows user
        private void txtBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            txtBoxLogin.Text = userName.ToString();
        }

        //for the 3 radio buttons
        private void rdPersonal_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\Temp\Personal.json"))
            {
                lbApps.Items.Clear();
                var path = $@"C:\Temp\Personal.json";
                var json = File.ReadAllText(path);

                lbApps.Items.Add(json);



                //lbApps.ItemsSource = json;
                //lbApps.DisplayMemberPath = "Name";



            }
            else
            {
                MessageBox.Show("You have to creat a new Personal list first");
                lbApps.Items.Clear();
            }
        }

        private void rdBusiness_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\Temp\Business.json"))
            {
                lbApps.Items.Clear();
                var path = $@"C:\Temp\Business.json";
                var json = File.ReadAllText(path);

                //var json = JsonConvert.DeserializeObject(File.ReadAllText(path));
                lbApps.Items.Add(json);

                // lbApps.ItemsSource = json;
                //lbApps.DisplayMemberPath = "Name";

            }
            else
            {
                MessageBox.Show("You have to create a new Business list first");
                lbApps.Items.Clear();
            }
        }

        private void rdCustom_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\Temp\Custom.json"))
            {
                lbApps.Items.Clear();
                var path = $@"C:\Temp\Custom.json";
                var json = File.ReadAllText(path);
                lbApps.Items.Add(json);
            }
            else
            {
                MessageBox.Show("You have to create a new Custom list first");
                lbApps.Items.Clear();
            }
        }

        private void rdAll_Checked(object sender, RoutedEventArgs e)
        {
            lbApps.Items.Clear();
            populateApp();

        }

    }
}