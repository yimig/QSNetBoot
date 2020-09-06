using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleWifi;

namespace QSNetBoot
{
    class SchoolNet
    {
        private const string NET1 = "QLNU-5.8G";
        private const string NET2 = "QLNU-2.4G";
        Wifi wifi = new Wifi();

        /// <summary>
        /// 检查目前是否已连接无线网
        /// </summary>
        /// <returns></returns>
        public bool CheckWifi()
        {
            return ((int)wifi.ConnectionStatus == 1) ? true : false;
        }

        /// <summary>
        /// 检查当前环境中是否存在校园网
        /// </summary>
        /// <returns></returns>
        public bool CheckSchoolNet()
        {
            foreach (var ap in wifi.GetAccessPoints())
            {
                if (ap.Name == NET1 || ap.Name == NET2) return true;
            }
            return false;
        }

        /// <summary>
        /// 检查当前连接的网络是否为校园网
        /// </summary>
        /// <returns></returns>
        public bool IsSchoolNet()
        {

            foreach (var ap in wifi.GetAccessPoints())
            {
                if (ap.Name == NET1 && ap.IsConnected || ap.Name == NET2 && ap.IsConnected) return true;
            }
            return false;
        }

        /// <summary>
        /// 连接到校园网
        /// </summary>
        public void ConnectToSchoolNet()
        {
            foreach (var ap in wifi.GetAccessPoints())
            {
                if (ap.Name == NET1 && !ap.IsConnected)
                {
                    ap.Connect(new AuthRequest(ap));
                    break;
                }
                else if (ap.Name == NET2 && !ap.IsConnected)
                {
                    ap.Connect(new AuthRequest(ap));
                    break;
                }
            }
        }
    }
}
