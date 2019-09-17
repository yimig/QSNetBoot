using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QSNetBoot
{
    partial class ConnectService : ServiceBase
    {
        public ConnectService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MessageBox.Show("Hello!");
        }

        protected override void OnStop()
        {
            MessageBox.Show("GoodBye!");
        }
    }
}
