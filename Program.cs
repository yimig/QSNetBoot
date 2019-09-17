using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QSNetBoot
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length!=0&&args[0] == "-c") new App().Run(new ConnectWindow());
            else new App().Run(new MainWindow());
        }
    }
}
