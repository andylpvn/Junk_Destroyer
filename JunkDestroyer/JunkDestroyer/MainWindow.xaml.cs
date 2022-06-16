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
using System.Data;
using System.Web.Script.Serialization;
//for getting Windows users
using System.DirectoryServices;
using System.Collections;

using JunkDestroyer.JSON;

using System.Linq;
using System.ComponentModel;

namespace JunkDestroyer
{
    public partial class MainWindow : Window
    {

       // List<string> appNameList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
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
                //add names to the ListBox  

                //string outputRow = $"{final,0} {name,100} ";


            }
          
        }

        //find all windows users in the computer
        private void findWindowsUser()
        {
            cbWindowsUser.Items.Clear();
            cbWindowsAdmin.Items.Clear();

            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName); //can use a specific machineName to login to that computer

            //getting all account in Admins group
            DirectoryEntry admGroup = localMachine.Children.Find("administrators", "group");
            object adms = admGroup.Invoke("members", null);
            foreach (object groupMember in (IEnumerable)adms)
            {
                DirectoryEntry adm = new DirectoryEntry(groupMember);
                cbWindowsAdmin.Items.Add(adm.Name);
            }
            //getting all account in Users group
            DirectoryEntry userGroup = localMachine.Children.Find("users", "group");
            object users = userGroup.Invoke("members", null);
            foreach (object groupMember in (IEnumerable)users)
            {
                DirectoryEntry user = new DirectoryEntry(groupMember);
                cbWindowsUser.Items.Add(user.Name);
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
            rdAll.IsChecked = false;
            rdPersonal.IsChecked = false;
            rdBusiness.IsChecked = false;
            rdCustom.IsChecked = false;

            //uncheck the "check all" checkbox
            cbSelectAll.IsChecked = false;

            //clear Notification
            lbNotifications.Items.Clear();

            //call methods
            populateApp();
            findWindowsUser();

            lbApps.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));

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
            //get value from combobox
            String cbUser = cbWindowsUser.Text;
            //get current logged in user
            string winUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            

            //loop all the selected item in the listbox           
            foreach (var item in lbApps.SelectedItems)
            {
                String s = item.ToString(); //convert each item to string
                                            //PowerShell.Create().AddScript($"Get-AppxPackage {s} | Remove-AppxPackage").Invoke(); //assign the string to PS script to remove the app

               
                

                string final = s.Split(',').Last();

                // PowerShell.Create().AddScript($"Get-AppxPackage -user {cbUser} {s} | Remove-AppxPackage").Invoke(); //assign the string to PS script to remove the app

                String notification = $"{final} from the user: {cbUser} >>> processed"; //show notification each run             
                lbNotifications.Items.Add(notification);
                lbNotifications.Foreground = Brushes.Red;
                //log file
                Logger.Log("WinUser: {0} | Application: {1} | Process: {2}", winUser, s , "has been processed"); //add each run to log file 
            }
            pBar.Value = pBar.Maximum;           
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
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            

            if (File.Exists(@"C:\Temp\Personal.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Personal.json";
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

            lbApps.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));

        }

        private void rdBusiness_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;

            if (File.Exists(@"C:\Temp\Business.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Business.json";
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

            lbApps.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
        }

       

        private void rdMaster_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;

            if (File.Exists(@"C:\Temp\Master.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Master.json";
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
                rdAll.IsChecked = false;
            }
        }

        private void rdCustom_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;

            if (File.Exists(@"C:\Temp\Custom.json"))
            {
                //clear ArrayString ItemsSource
                lbApps.ClearValue(ItemsControl.ItemsSourceProperty);
                //clear Listbox
                lbApps.Items.Clear();

                var path = $@"C:\Temp\Custom.json";
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


        private void rdAll_Checked(object sender, RoutedEventArgs e)
        {
            //clear notification listbox
            lbNotifications.Items.Clear();
            //reset progress bar
            pBar.Value = pBar.Minimum;
            //cal the function 
            populateApp();

        }





        //log file class
        public static class Logger
        {
            public static void Log(string format, params object[] args)
            {
                using (var streamWriter = new StreamWriter("C:\\Temp\\Log.txt", true))
                {
                    streamWriter.WriteLine("{0} | {1}", DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"), string.Format(format, args));
                }
            }
        }

        
    }
}