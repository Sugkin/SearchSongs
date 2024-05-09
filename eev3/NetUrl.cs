using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eev3
{
    public class NetUrl
    {
        /// <summary>
        /// 默认请求UA
        /// </summary>
        public static string defUA = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
        /// <summary>
        /// GET请求异步
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url, bool AllowAutoRedirect = false)
        {
            try
            {
                string result = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "application/json, text/plain, */*";
                request.UserAgent = defUA;
                request.AllowAutoRedirect = AllowAutoRedirect;
                request.ContentType = "application/json; charset=UTF-8";
                request.ServicePoint.Expect100Continue = false;
                WebResponse response = await request.GetResponseAsync();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                result = sr.ReadToEnd();
                return result;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Post请求from数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="value">请求值</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, string value, string Referer,  bool AllowAutoRedirect = false)
        {
            try
            {
                string result = "";
                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] byteArray = myEncoding.GetBytes(value);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Accept = "application/json, text/plain, */*";
                request.UserAgent = defUA;
                request.AllowAutoRedirect = AllowAutoRedirect;
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.ContentLength = byteArray.Length;
                request.ServicePoint.Expect100Continue = false;
                request.Referer = Referer;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length); //写入参数 
                }
                WebResponse response = await request.GetResponseAsync();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                result = sr.ReadToEnd();
                return result;
            }
            catch (Exception l)
            {
                Console.WriteLine(l.Data);
                return "";
            }
        }


    }
}
