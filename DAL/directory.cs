using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    static class directory
    {
        public static DirectoryInfo getWorkingDirectory()
        {
            return Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName);
        }
    }
}
