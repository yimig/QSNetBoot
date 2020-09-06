using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace QSNetBoot
{
    class NotOpenBrowser
    {
        private bool isChanged;

        public NotOpenBrowser(bool isNotOpenBrowser)
        {
            if (!isAdmin())
            {
                DialogResult result = MessageBox.Show("更改此项需给予管理员权限，是否继续？", "权限不足", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    SwitchToAdmin();
                    isChanged = true;
                }
                else isChanged = false;
            }
            else
            {
                NotOpen(isNotOpenBrowser);
                isChanged = true;
            }
        }

        public bool IsChanged
        {
            get { return isChanged; }
        }

        public bool isAdmin()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        public void SwitchToAdmin()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);}

            Environment.Exit(0);
        }

        public static void NotOpen(bool isNotOpenBrowser)
        {
            if (isNotOpenBrowser)
            {
                SetReg();
            }
            else
            {
                DelReg();
            }
        }

        private static void SetReg()
        {
            try
            {
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey runKey = hkml.OpenSubKey(@"SYSTEM\CurrentControlSet\services\NlaSvc\Parameters\Internet", true);
                runKey.SetValue( "EnableActiveProbing", Convert.ToString(0, 16), RegistryValueKind.DWord);
                runKey.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void DelReg()
        {
            try
            {
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey runKeys = hkml.OpenSubKey(@"SYSTEM\CurrentControlSet\services\NlaSvc\Parameters\Internet", true);
                runKeys.SetValue( "EnableActiveProbing", Convert.ToString(1, 16), RegistryValueKind.DWord);
                runKeys.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
