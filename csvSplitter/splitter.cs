using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splitbot;

    class Program
    {
        static void Main(string[] args)
        {
            // Read the csv as plain text
            string rawCSV = System.IO.File.ReadAllText(@"C:\Users\jacob\OneDrive\Desktop\csvSplitter\installed_full.csv");
            rawTextBox.Text(rawCSV);
          

            Console.ReadKey();
        }
    }