using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Collections;

namespace uninstall
{
    public partial class Form1 : Form
    {

        BackgroundWorker bw_AppLister = null;

        public Form1()
        {
            InitializeComponent();

            bw_AppLister = new BackgroundWorker();
           
            //create our background worker event handlers
            bw_AppLister.DoWork+=new DoWorkEventHandler(bw_AppLister_DoWork);
            bw_AppLister.RunWorkerCompleted+=new RunWorkerCompletedEventHandler(bw_AppLister_RunWorkerCompleted);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //display all installed applications asynchronously
            ListProgramsAsync();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //display all installed applications asynchronously
            ListProgramsAsync();
        }     
               

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            try
            {
                int index = this.listView1.SelectedIndices[0];
                string sProgram = this.listView1.Items[index].Text;
                if(UninstallProgram(sProgram))
                {
                    MessageBox.Show(sProgram + " was uninstalled!");
                    ListProgramsAsync();
                }
                else
                    MessageBox.Show("Error uninstalling " + sProgram);
            }
            catch { }
        }

        #region "Methods"

        private void EnableUI(bool enable)
        {
            this.btnRefresh.Enabled = enable;
            this.btnUninstall.Enabled = enable;
        }

        /// <summary>
        /// asynchronous method
        /// </summary>
        private void ListProgramsAsync()
        {
            //is backgroundworker running?
            if (bw_AppLister.IsBusy)
                return;

            this.listView1.Items.Clear();
            this.listView1.Items.Add("Collecting applications, Please wait...");

            //lock controls
            EnableUI(false);

            Application.DoEvents();

            //start the asynchronous method
            this.bw_AppLister.RunWorkerAsync();
        }

        private List<string> ListPrograms()
        {
            List<string> programs = new List<string>();
            try
            {
                //query to get all installed products
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        //more properties:
                        //http://msdn.microsoft.com/en-us/library/windows/desktop/aa394378(v=vs.85).aspx
                        programs.Add(mo["Name"].ToString());

                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property
                    }
                }
                return programs;
            }
            catch (Exception ex)
            {
                return programs;
            }
        }

        private bool UninstallProgram(string ProgramName)
        {
            try
            {
                //load the query string
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                //get the specified proram(s)
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        //make sure that we are uninstalling the correct application
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            //call to Win32_Product Uninstall method, no parameters needed
                            object hr = mo.InvokeMethod("Uninstall", null);
                            return (bool)hr;
                        }
                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property, so an exception will be thrown
                    }
                }
                //was not found...
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region "BackgroundWorker"

        private void bw_AppLister_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> programs = ListPrograms();
            //pass the results to RunWorkerCompleted
            e.Result = programs;
        }

        private void bw_AppLister_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.listView1.Items.Clear();
            //cast the object result to IEnumerable, this will cast the result object to an enumerable object
            IEnumerable enumerable = e.Result as IEnumerable;
            foreach (object item in enumerable)
            {
                this.listView1.Items.Add(item.ToString());
            }

            EnableUI(true);
        }

        #endregion

    }
}
