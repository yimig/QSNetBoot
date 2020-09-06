using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using QSNetBoot.Utils;


/* Copyright [Upane Web]

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

namespace QSNetBoot
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            UserNameBox.Text = Setting.Default.userName;
            PasswordBox.Password = Setting.Default.passWord;
            isBootBox.IsChecked = Setting.Default.isBootWithWindows;
            isDriectBox.IsChecked = Setting.Default.isContentDriect;
            NotOpenBrowserBox.IsChecked = Setting.Default.isNotOpenBrowser;
            URLBox.Text = Setting.Default.URL;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Setting.Default.URL = URLBox.Text;
            Setting.Default.userName = UserNameBox.Text;
            Setting.Default.passWord = PasswordBox.Password;
            Setting.Default.Save();
            this.Close();
            new ConnectWindow().Show();
        }

        private void IsBootBox_OnClick(object sender, RoutedEventArgs e)
        {
            BootWithWindows.Boot((bool)isBootBox.IsChecked);
            Setting.Default.isBootWithWindows = (bool) isBootBox.IsChecked;
            Setting.Default.Save();
        }

        private void IsDriectBox_OnClick(object sender, RoutedEventArgs e)
        {
            Setting.Default.isContentDriect = (bool) isDriectBox.IsChecked;
            Setting.Default.Save();
        }

        private void NotOpenBrowserBox_OnClick(object sender, RoutedEventArgs e)
        {
            var nob = new NotOpenBrowser((bool) NotOpenBrowserBox.IsChecked);
            NotOpenBrowserBox.IsChecked = nob.IsChanged;
            Setting.Default.isNotOpenBrowser = (bool) NotOpenBrowserBox.IsChecked;
            Setting.Default.Save();
        }

        private void JumpToProjectPageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/yimig/QSNetBoot");
        }

        private void BtnCreateShotcut_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Utils.ShortcutCreator.CreateShortcutOnDesktop("一键连接校园网",Environment.CurrentDirectory+ "\\QSNetBoot.exe","-c","使用QSNetBoot一键连接校园网");
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：" + ex.Message, "创建快捷方式失败");
            }
        }
    }
}
