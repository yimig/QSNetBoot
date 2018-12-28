using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;


namespace QSNetBoot
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        private bool isWlanAvailable=true;
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeNotifyIcon();
            //检测是否为首次启动，若为首次启动，仅进行配置不进行连接
            if (Setting.Default.isFirstOpen)
            {
                this.Visibility = Visibility.Visible;
                Setting.Default.isFirstOpen = false;
                Setting.Default.Save();
            }
            else
            {
                //检测是否已经连接校园网，如果已经连接则直接发送数据，未连接则先连接
                if (!CheckCurrentNet())
                {
                    WifiControl wifiControl = new WifiControl();
                    ConnectToNet(wifiControl);
                }
                //从注册表启动时每次都在这里停止，弹出窗口显示isWlan的值时会有五秒左右的线程休眠，然后程序终止。原因不明。
                //检测网络连接是否可用
                if (isWlanAvailable)
                {
                    try
                    {
                        Thread.Sleep(500);
                        //查询是否直送数据不等待回复
                        if (Setting.Default.isContentDriect)
                        {
                            SendData.SentInfo(URLBox.Text, UserNameBox.Text, PasswordBox.Password);
                            CloseApp("登录信息已发送！");
                            return;
                        }
                        else
                        {
                            //如果等待回复，则对回复进行简单判断
                            string response =
                                SendData.GetResponse(SendData.SentInfo(URLBox.Text, UserNameBox.Text,
                                    PasswordBox.Password));
                            if (response.Contains("我要下线"))
                            {
                                CloseApp("已连接");
                                return;
                            }
                            else if (response.Contains("参数错误"))
                            {
                                CloseApp("账户名或密码错误，请单击此气泡修改");
                                return;
                            }
                            else if (response.Contains("接入设备"))
                            {
                                CloseApp("设备认证失败，请单击此气泡并尝试修改登录页面地址");
                                return;
                            }
                            else
                            {
                                //特殊回复则输出提示框中的文字
                                string[] rawreturndata =
                                    response.Split(new string[] {"('", "')"}, StringSplitOptions.RemoveEmptyEntries);
                                CloseApp(rawreturndata[1]);
                                return;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        CloseApp("与服务器连接失败，请尝试修改登录页面地址。\n Code="+e.Message);
                        return;
                    }
                }
            }

        }

        /// <summary>
        /// 连接到校园网
        /// </summary>
        /// <param name="wifiControl"></param>
        private void ConnectToNet(WifiControl wifiControl)
        {
            wifiControl.ScanSSID();
            if (wifiControl.ssids.Count == 0)
            {
                CloseApp("当前环境中没有任何可连接的网络");
                return;
            }
            else
            {
                var connectList = wifiControl.ssids.Where(p => p.SSID == "QLNU-5.8G").ToList();
                if (connectList.Count != 0) wifiControl.ConnectToSSID(connectList[0]);
                else
                {
                    connectList = wifiControl.ssids.Where(p => p.SSID == "QLNU-5.8G").ToList();
                    if (connectList.Count != 0) wifiControl.ConnectToSSID(connectList[0]);
                    else
                    {
                        CloseApp("当前环境中好像没有校园网");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 判断当前网络的连接情况
        /// </summary>
        /// <returns>true：已连接校园网，需进一步发送数据；false：未连接任何网络</returns>
        private bool CheckCurrentNet()
        {
            bool statue=false;
            string CurrentNet=null;
            try
            {
                CurrentNet = WifiControl.GetCurrentConnection();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                CloseApp("没有开启Wlan连接，请手动开启后重启本程序");
                isWlanAvailable = false;
                return true;
            }
            catch(Exception e)
            {
                CloseApp("错误："+e.Message+"\n请联系开发者协助解决该问题");
                return true;
            }

            if (CurrentNet.Contains("QLNU")) statue = true;
            else if (CurrentNet == "") statue = false;
            else
            {
                CloseApp("已连接非校园网");
                return true;
            }
            return statue;
        }

        private void InitializeNotifyIcon()
        {
            Icon icon = new Icon(@"Icon\NotifyIcon.ico");
            this.notifyIcon = new NotifyIcon();
            //从注册表启动时不可使用相对路径
            //this.notifyIcon.Icon = new Icon(@"C:\Users\zhang\Downloads\Icon\NotifyIcon.ico");
            this.notifyIcon.Text = "QSNetBoot";
            this.notifyIcon.Icon = icon;
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.BalloonTipClicked += NotifyIcon_Click;
        }

        private void InitializeData()
        {
            UserNameBox.Text = Setting.Default.userName;
            PasswordBox.Password = Setting.Default.passWord;
            isBootBox.IsChecked = Setting.Default.isBootWithWindows;
            isDriectBox.IsChecked = Setting.Default.isContentDriect;
            URLBox.Text = Setting.Default.URL;
        }

        private void CloseApp(string showtext)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.notifyIcon.BalloonTipText = showtext;
                    this.notifyIcon.ShowBalloonTip(0);
                });

                //线程休眠5秒
                Thread.Sleep(5000);

                Dispatcher.Invoke(() =>
                {
                    if (this.Visibility == Visibility.Hidden)
                        Environment.Exit(0);
                });
            });


        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Setting.Default.URL = URLBox.Text;
            Setting.Default.userName = UserNameBox.Text;
            Setting.Default.passWord = PasswordBox.Password;
            Setting.Default.Save();
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
    }
}
