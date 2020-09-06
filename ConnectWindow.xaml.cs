using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using SimpleWifi;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;

namespace QSNetBoot
{
    /// <summary>
    /// ConnectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectWindow : Window
    {
        private NotifyIcon notifyIcon;
        private DispatcherTimer timer;


        public ConnectWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            try
            {
                LinkStart();
            }
            catch (Exception e)
            {
                CloseApp(false, e.Message, e);
            }
        }

        private void SetEndTimer()
        {
            //设置定时器          
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(50000000);   //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);
            //开启定时器          
            timer.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void LinkStart()
        {
            new Wifi().ConnectionStatusChanged += ConnectWindow_ConnectionStatusChanged;
            SchoolNet schoolNet=new SchoolNet();
            if (schoolNet.CheckWifi())
            {
                if(schoolNet.IsSchoolNet())StartSend();
                else CloseApp(true,"当前已连接非校园网");
            }
            else
            {
                if (schoolNet.CheckSchoolNet())
                {
                    schoolNet.ConnectToSchoolNet();
                    StartSend();
                }
                else CloseApp(false,"当前环境中没有校园网");
            }
        }

        private void ConnectWindow_ConnectionStatusChanged(object sender, WifiStatusEventArgs e)
        {
            LinkStart();
        }

        private void StartSend()
        {
            try
            {
                CheckAnalyzer(StartSendOption());
            }
            catch (Exception e)
            {
                CloseApp(false,e.Message,e);
            }
        }

        private InfoAnalyser StartSendOption()
        {
            var carrier=new Carrier();
            if (Setting.Default.isContentDriect)
            {
                carrier.StartSendInNoResponse(Setting.Default.URL, Setting.Default.userName, Setting.Default.passWord);
                return new InfoAnalyser();
            }
            else return carrier.StartSend(Setting.Default.URL, Setting.Default.userName, Setting.Default.passWord);
        }

        private void CheckAnalyzer(InfoAnalyser analyser)
        {
            if(analyser.SendDriectly)CloseApp(true,"登陆信息已发送");
            else
            {
                if (analyser.IsConnected) CloseApp(true,"连接成功");
                else if(analyser.ErrorCode==ConnectError.ReConnected)CloseApp(true,"您已经连接校园网，无需重复连接",analyser);
                else if (analyser.ErrorCode == ConnectError.VerifyUserError) CloseApp(false, "账户名或密码错误，请打开本软件进行修改", analyser);
                else if (analyser.ErrorCode == ConnectError.VerifyDeviceError) CloseApp(false, "设备认证失败，请重新输入登陆页面地址", analyser);
                else CloseApp(false, analyser.OtherInfo, analyser);
            }
        }

        void CloseApp(bool isConnected, string closeString)
        {
            if (isConnected)ShowInfo(isConnected,"已连接",closeString);
            else ShowInfo(isConnected,"连接失败","未知错误"+closeString);
            CloseApp();
        }

        void CloseApp(bool isConnected, string closeString, Exception e)
        {
            if(isConnected) ShowInfo(isConnected,"已连接", closeString);
            else
            {
                ShowInfo(isConnected,"连接失败",closeString);
                logWriter(e.ToString());
            }
            CloseApp();
        }

        void CloseApp(bool isConnected, string closeString, InfoAnalyser analyser)
        {
            if (isConnected) ShowInfo(isConnected,"已连接", closeString);
            else
            {
                ShowInfo(isConnected,"连接失败",closeString);
                logWriter(analyser.RawInfo);
            }
            CloseApp();
        }

        private void CloseApp()
        {
            SetEndTimer();
        }

        private void logWriter(string log)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\错误日志.log"))
            {
                if(new FileInfo(Environment.CurrentDirectory + "\\错误日志.log").Length/(1024*8)>200)File.Delete(Environment.CurrentDirectory + "\\错误日志.log");
            }
            StreamWriter writer=new StreamWriter("错误日志.log",true);
            writer.Write("\n=============="+DateTime.Now+"===============\n"+log);
            writer.Close();
        }

        private void ShowInfo(bool isConnected,string title,string info)
        {
            //MessageBox.Show(info);
            notifyIcon.ShowBalloonTip(0, title,info, isConnected?ToolTipIcon.Info:ToolTipIcon.Error);
        }

        private void InitializeNotifyIcon()
        {
            Icon icon = (Icon)(ResourceFile.ResourceManager.GetObject("NotifyIcon"));
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Visible = true;
            //从注册表启动时不可使用相对路径
            //this.notifyIcon.Icon = new Icon(@"C:\Users\zhang\Downloads\Icon\NotifyIcon.ico");
            this.notifyIcon.Text = "QSNetBoot";
            this.notifyIcon.Icon = icon;
            this.notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.BalloonTipClicked += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            timer?.Stop();
            new MainWindow().Show();
            this.Close();
        }
    }
}
