﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace QSNetBoot
{
    static class BootWithWindows
    {
        static string filepath = Application.ExecutablePath;
        static string runName = "QSNetBoot";

        public static void Boot(bool isBoot)
        {
            if (isBoot)
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

                RegistryKey hkml = Registry.CurrentUser;
                RegistryKey runKey = hkml.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                runKey.SetValue(runName, filepath);
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
                string runName = "QSNetBoot";
                RegistryKey hkml = Registry.CurrentUser;
                RegistryKey runKeys = hkml.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                runKeys.DeleteValue(runName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
