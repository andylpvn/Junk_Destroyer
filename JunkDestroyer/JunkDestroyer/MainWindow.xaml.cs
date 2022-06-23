using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
//for running PS scripts inside .NET
using System.Management.Automation;
//for JSON
using Newtonsoft.Json;
using JunkDestroyer.JSON;
using System.ComponentModel;
using System.Reflection;

namespace JunkDestroyer
{
    public partial class MainWindow : Window
    {

        
        //call root path where the application is located
        string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        
        public MainWindow()
        {
            InitializeComponent();
            rdCurrentUser.IsChecked = true;
        }
        //show currently logged in windows user
        private void txtBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            txtBoxLogin.Text = userName.ToString();
        }

        //show installed apps by powershell scripts
        private void populateApp()
        {
            lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
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

                lbApps.Items.Add(final);
               


            }
          
        }

        //find all windows users in the computer
        //private void findWindowsUser()
        //{
        //    cbWindowsUser.Items.Clear();
           

        //    DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName); //can use a specific machineName to login to that computer

            
        //    //getting all account in Users group
        //    DirectoryEntry userGroup = localMachine.Children.Find("users", "group");
        //    object users = userGroup.Invoke("members", null);
        //    foreach (object groupMember in (IEnumerable)users)
        //    {
        //        DirectoryEntry user = new DirectoryEntry(groupMember);
        //        cbWindowsUser.Items.Add(user.Name);
        //    }

        //    ////another way to find users
        //    //SelectQuery query = new SelectQuery("Win32_UserAccount");
        //    //ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
        //    //foreach (ManagementObject user in searcher.Get())
        //    //{
        //    //    //get a specific last part of windows user string
        //    //    String s = user.ToString();
        //    //    var last = s.Split('=').Last();

        //    //    //add user into the combobox
        //    //    comboBoxWindowsUser.Items.Add(last.ToString());
        //    //    comboBoxWindowsUser.SelectedIndex = 0;
        //    //}
            
        //}

        //Refresh button
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            //uncheck buttons
           
            rdPersonal.IsChecked = false;
            rdBusiness.IsChecked = false;
            rdCustom.IsChecked = false;
            rdMaster.IsChecked = false;
            UninstallBtn.IsEnabled = false;          
            cbSelectAll.IsChecked = false;

            //clear Notification
            lbNotifications.Items.Clear();

            //call methods
            populateApp();
           
            //sort the listbox 
            lbApps.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));

        }

        // "Select all" check box 
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (cbSelectAll.IsChecked == true)
            {
                lbApps.SelectAll();
                lbApps.Foreground = Brushes.Red;
            }
            else if (cbSelectAll.IsChecked == false)
            {
                lbApps.UnselectAll();
                lbApps.Foreground = Brushes.Black;
            }
        }

        // Uninstall Button
        private void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            //show notification
            MessageBox.Show("Uninstall is in progress. Click OK to continue");          
            
            //get current logged in user
            string winUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;


            if (rdCurrentUser.IsChecked == true)
            {
                foreach (var item in lbApps.SelectedItems)
                {
                    String s = item.ToString(); //convert each item to string

                    string packageFullName = s.Split(',').Last();

                     PowerShell.Create().AddScript($"Get-AppxPackage {packageFullName} | Remove-AppxPackage").Invoke(); //assign the string to PS script to remove the app

                    String notification = $"{s} from the user: {winUser} >>> processed"; //show notification each run             
                    lbNotifications.Items.Add(notification);
                    lbNotifications.Foreground = Brushes.Red;
                    //log file
                    Logger.Log("WinUser: {0} | Application: {1} | Process: {2}", winUser, s, "has been processed"); //add each run to log file 
                }
            }

            //else if (rdAllUser.IsChecked == true)
            //{
                
            //    foreach (var item in lbApps.SelectedItems)
            //    {
            //        String s = item.ToString(); //convert each item to string

            //        string packageFullName = s.Split(',').Last();
                    
            //         //PowerShell.Create().AddScript($"Get-AppxPackage -AllUsers {packageFullName} | Remove-AppxPackage").Invoke(); //assign the string to PS script to remove the app

            //        String notification = $"{s} from all the Windows users have been processed"; //show notification each run             
            //        lbNotifications.Items.Add(notification);
            //        lbNotifications.Foreground = Brushes.Red;
            //        //add log file to the log.txt
            //        Logger.Log("WinUser: {0} | Application: {1} | Process: {2}", winUser, s, "has been processed"); //add each run to log file  
            //    }
            //}

    
            pBar.Value = pBar.Maximum;           
        }

        // Update button - open a new window
        private void UpdateAppList(object sender, RoutedEventArgs e)

        {
            UpdateWindow updateWindow = new UpdateWindow();
            updateWindow.Show();
        }

        

        //for the 3 radio buttons
        private void rdPersonal_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            //reset uninstall button
            UninstallBtn.IsEnabled = true;

            if (File.Exists($@"{currentDirectory}\Personal.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"{currentDirectory}\Database\Personal.json";
                string json = File.ReadAllText(path);
                List<commonAppName> commmonAppList = JsonConvert.DeserializeObject<List<commonAppName>>(json);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);
           
                //combine the two ArrayList together to match commonName with fullAppName
                foreach (var combineName in commmonAppList.Zip(appList, Tuple.Create))
                {
                    //trim to get specific combination name
                    String s = combineName.ToString();
                    int start = s.IndexOf("(") + 1;
                    int end = s.IndexOf(")", start);
                    string final = s.Substring(start, end - start);
                    //add the specific app name to the list box
                    lbApps.Items.Add(final);                  
                }
            }

            //sort the listbox
            

            else
            {
                //lbNotifications.ClearValue(ItemsControl.ItemsSourceProperty);
                MessageBox.Show("You have to create a new Personal list first");
                rdPersonal.IsChecked = false;
            }

            //sort the listbox 
            lbApps.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));

        }

        private void rdBusiness_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            //reset uninstall button
            UninstallBtn.IsEnabled = true;

            if (File.Exists($@"{currentDirectory}\Database\Business.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"{currentDirectory}\Database\Business.json";
                string json = File.ReadAllText(path);
                List < commonAppName > commmonAppList = JsonConvert.DeserializeObject<List<commonAppName>>(json);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);

                //combine the two ArrayList together to match commonName with fullAppName
                foreach (var combineName in commmonAppList.Zip(appList, Tuple.Create))
                {
                    //trim to get specific combination name
                    String s = combineName.ToString();
                    int start = s.IndexOf("(") + 1;
                    int end = s.IndexOf(")", start);
                    string final = s.Substring(start, end - start);
                    //add the specific app name to the list box
                    lbApps.Items.Add(final);
                }
            }
            else
            {
                //lbNotifications.ClearValue(ItemsControl.ItemsSourceProperty);
                MessageBox.Show("You have to create a new Business list first");
                rdBusiness.IsChecked = false;
            }

            //sort the listbox 
            lbApps.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

       

        private void rdMaster_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            //reset uninstall button
            UninstallBtn.IsEnabled = true;

            if (File.Exists($@"{currentDirectory}\Database\Master.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"{currentDirectory}\Database\Master.json";
                string json = File.ReadAllText(path);
                List<commonAppName> commmonAppList = JsonConvert.DeserializeObject<List<commonAppName>>(json);
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);

                //combine the two ArrayList together to match commonName with fullAppName
                foreach (var combineName in commmonAppList.Zip(appList, Tuple.Create))
                {
                    //trim to get specific combination name
                    String s = combineName.ToString();
                    int start = s.IndexOf("(") + 1;
                    int end = s.IndexOf(")", start);
                    string final = s.Substring(start, end - start);
                    //add the specific app name to the list box
                    lbApps.Items.Add(final);
                }
            }
            else
            {
                //lbNotifications.ClearValue(ItemsControl.ItemsSourceProperty);
                MessageBox.Show("You have to create a new Master list first");
                
            }
            //sort the listbox 
            lbApps.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        private void rdCustom_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            //reset uninstall button
            UninstallBtn.IsEnabled = true;

            if (File.Exists($@"{currentDirectory}\Database\Custom.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"{currentDirectory}\Database\Custom.json";
                string json = File.ReadAllText(path);
               
                List<appName> appList = JsonConvert.DeserializeObject<List<appName>>(json);

                //combine the two ArrayList together to match commonName with fullAppName
                foreach (var app in appList)
                {
                    
                    //add the specific app name to the list box
                    lbApps.Items.Add(app);
                }
            }
            else
            {
                //lbNotifications.ClearValue(ItemsControl.ItemsSourceProperty);
                MessageBox.Show("You have to create a new Custom list first");
                rdCustom.IsChecked = false;
            }
        }


        //log file class
        public static class Logger
        {
            public static void Log(string format, params object[] args)
            {
                if (!Directory.Exists("C:\\Temp"))
                {
                    Directory.CreateDirectory("C:\\Temp"); // create a new Database folder in root folder
                }

                using (var streamWriter = new StreamWriter("C:\\Temp\\JunkDestroyer_Log.txt", true))
                {
                    streamWriter.WriteLine("{0} | {1}", DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"), string.Format(format, args));
                }
            }
        }

       
    }
}