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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csvSplitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 1. Create lists for the csv data
            var appNames = new List<string>();
            var appPackageFullNames = new List<string>();

            // 2. Collect the rows of the CSV file
            string[] csvRows = System.IO.File.ReadAllLines(@"C:\Users\jacob\OneDrive\Desktop\csvSplitter\installed_full.csv");

            // 3. For each row:
            for (int i = 2; i < csvRows.Length; i++)
            {
                // Collect its data into an array (0=names, 1=fullPackagenames)
                string[] rowData = csvRows[i].Split(',');

                // Add the row's data to its respective list
                appNames.Add(rowData[0]);
                appPackageFullNames.Add(rowData[1]);
            }

            // 4. Populate listboxes with data
            for (int i = 0; i < appNames.Count; i++)
            {
                namesView.Items.Add(appNames[i]);
            }

            for (int i = 0; i < appPackageFullNames.Count; i++)
            {
                packageFullNamesView.Items.Add(appPackageFullNames[i]);
            }

        }
    }
}
