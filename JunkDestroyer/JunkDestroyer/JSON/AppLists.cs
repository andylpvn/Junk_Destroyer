﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunkDestroyer.JSON
{
    public class AppLists
    {
        public List<appName> appName { get; set; }
    }

    public class appName
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format(Name);
        }
    }



}
