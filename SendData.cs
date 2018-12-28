using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QSNetBoot
{
    class SendData
    {
        /// <summary>
        /// 上传用户信息
        /// </summary>
        /// <param name="deviceinfo">设备信息</param>
        /// <param name="userinfo">用户登录信息</param>
        /// <returns></returns>
        static public HttpWebResponse SentInfo(string url, string username,string password)
        {
            string[] rawdeviceinfo = url.Split(new char[] {'&'});
            string deviceinfo = null;
            for (int i = 1; i < rawdeviceinfo.Length - 1; i++) deviceinfo += rawdeviceinfo[i] + '&';
            deviceinfo += rawdeviceinfo[rawdeviceinfo.Length-1];
            string userinfo= "is_auto_land=false&usernameHidden="+username+"&username_tip=Username&username="+username+"&strTypeAu=&uuidQrCode=&authorMode=&pwd_tip=Password&pwd="+password+"&net_access_type=%BB%A5%C1%AA%CD%F8";
            //转换输入参数的编码类型，获取bytep[]数组 
            byte[] byteArray = Encoding.UTF8.GetBytes(userinfo);
            //初始化新的webRequst
            //1． 创建httpWebRequest对象
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri("http://10.240.20.2:8080/eportal/webGateModeV2.do?method=login&param=true&" + deviceinfo));
            webRequest.KeepAlive = false;
            webRequest.Host = "10.240.20.2:8080";
            //webRequest.ContentLength = 184;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            webRequest.Referer = "http://10.240.20.2:8080/eportal/index.jsp?" + deviceinfo;
            webRequest.Headers.Add("Cache-Control", "max-age=0");
            webRequest.Headers.Add("Origin", "http://10.240.20.2:8080");
            webRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            webRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            //webRequest.Headers.Add("Cookie", cookie);
            //2． 初始化HttpWebRequest对象
            webRequest.Method = "POST";
            webRequest.ContentLength = byteArray.Length;
            Stream newStream = webRequest.GetRequestStream(); //创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            HttpWebResponse web = (HttpWebResponse)webRequest.GetResponse();
            return web;
        }

        /// <summary>
        /// 获得服务器返回的数据;
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static string GetResponse(HttpWebResponse web)
        {
            //4． 读取服务器的返回信息
            Stream responseStream = UnzipStream(web);
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
            string result = reader.ReadToEnd();
            responseStream.Close();
            return result;
        }

        /// <summary>
        /// 解压网页回传的数据流
        /// </summary>
        /// <param name="web">请求数据</param>
        /// <returns>解压后的数据流</returns>
        static Stream UnzipStream(HttpWebResponse web)
        {
            Stream responseStream = web.GetResponseStream();
            if (web.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (web.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
            return responseStream;
        }
    }
}
