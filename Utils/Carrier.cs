using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSNetBoot
{

    enum ConnectError { Connected, ReConnected,VerifyUserError, VerifyDeviceError, OtherError }

    class Carrier
    {
        private string url, username, password;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public void StartSendInNoResponse(string url,string userName,string passWord)
        {
            SendData.SentInfo(url, userName, passWord);
        }

        public InfoAnalyser StartSend(string url, string userName, string passWord)
        {
            return new InfoAnalyser(SendData.SentInfo(url,userName,passWord));
        }
    }

    class InfoAnalyser
    {
        private string rawInfo;
        private bool isConnected;
        private ConnectError errorCode;
        private string otherInfo;
        private bool sendDriectly;

        /// <summary>
        /// 网页返回的原始信息
        /// </summary>
        public string RawInfo
        {
            get { return rawInfo; }
        }

        /// <summary>
        /// 当前是否已连接
        /// </summary>
        public bool IsConnected
        {
            get { return isConnected; }
        }

        /// <summary>
        /// 返回连接错误代码
        /// </summary>
        public ConnectError ErrorCode
        {
            get { return errorCode; }
        }

        /// <summary>
        /// 如果是特殊情况导致的连接失败，尝试导出其提示信息
        /// </summary>
        public string OtherInfo
        {
            get { return otherInfo; }
        }

        /// <summary>
        /// 是否直接连接不等待回送
        /// </summary>
        public bool SendDriectly
        {
            get { return sendDriectly; }
        }

        /// <summary>
        /// 不等待会送的构造方法
        /// </summary>
        public InfoAnalyser()
        {
            sendDriectly = true;
        }

        /// <summary>
        /// 等待回送的构造方法
        /// </summary>
        /// <param name="webResponse"></param>
        public InfoAnalyser(HttpWebResponse webResponse)
        {
            isConnected = false;
            sendDriectly = false;
            rawInfo=SendData.GetResponse(webResponse);
            StartAnalize();
        }

        /// <summary>
        /// 检测回送结果
        /// </summary>
        private void StartAnalize()
        {
            if (rawInfo.Contains("我要下线"))
            {
                isConnected = true;
                errorCode = ConnectError.Connected;
            }
            else if (rawInfo.Contains("您当前已使用")) errorCode = ConnectError.ReConnected;
            else if (rawInfo.Contains("参数错误")) errorCode = ConnectError.VerifyUserError;
            else if (rawInfo.Contains("接入设备")) errorCode = ConnectError.VerifyDeviceError;
            else
            {
                errorCode = ConnectError.OtherError;
                otherInfo = rawInfo.Split(new string[] { "('", "')" }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }
    }
}
