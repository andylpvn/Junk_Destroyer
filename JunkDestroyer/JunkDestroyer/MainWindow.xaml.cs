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
//for getting Windows users
using System.DirectoryServices;
using System.Collections;
//calling the Applists class
using JunkDestroyer.JSON;

namespace JunkDestroyer
{
    public partial class MainWindow : Window
    {

     
        public MainWindow()
        {
            InitializeComponent();
        }

        //show installed apps by powershell scripts
        private void populateApp()
        {
            lbApps.Items.Clear();

            //intergrate PS script
            var appNames = PowerShell.Create().AddScript("Get-AppxPackage | Select Name").Invoke();

            //get each value in the PS result
            foreach (var name in appNames)
            {
                // trim the string to get specific app names
                String s = name.ToString();
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

            //clear Notification
            lbNotifications.Items.Clear();

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
            //clear ArrayString ItemsSource
            lbNotifications.ClearValue(ItemsControl.ItemsSourceProperty);
            lbNotifications.Items.Add("Uninstallation is in progress ...");
            lbNotifications.Foreground = new SolidColorBrush(Colors.Red);
            //loop all the selected item in the listbox
            foreach (var item in lbApps.SelectedItems)
            {
                String s = item.ToString(); //convert each item to string
                PowerShell.Create().AddScript($"Get-AppxPackage {s} | Remove-AppxPackage").Invoke(); //assign the string to PS script to remove the app
                String notification = $"{s} >>> has been uninstalled"; //show notofication each run             
                lbNotifications.Items.Add(notification);
                
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
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Personal.json";
                string json = File.ReadAllText(path);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);
                lbApps.ItemsSource = appList;

            }
            else
            {
                MessageBox.Show("You have to create a new Personal list first");
               
            }


        }

        private void rdBusiness_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\Temp\Business.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Business.json";
                string json = File.ReadAllText(path);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);
                lbApps.ItemsSource = appList;


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
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Custom.json";
                string json = File.ReadAllText(path);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);
                lbApps.ItemsSource = appList;
            }
            else
            {
                MessageBox.Show("You have to create a new Custom list first");
               // lbApps.Items.Clear();
            }
        }

        private void rdAll_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\Temp\Master.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Master.json";
                string json = File.ReadAllText(path);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);
                lbApps.ItemsSource = appList;
            }
            else
            {
                MessageBox.Show("You have to create a new Master list first");
                // lbApps.Items.Clear();
            }

        }

    }
}