using System;
using System.Net;

namespace Logging.Server
{
    public class WebClient : IDisposable //: WebClient
    {
        private int _timeout;

        /// <summary>
        /// 超时时间(毫秒)
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        public WebHeaderCollection Headers { get; set; }

        public WebClient()
        {
            this._timeout = 60000;
        }

        public WebClient(int timeout)
        {
            this._timeout = timeout;
        }

        public void UploadString(Uri address, string str)
        {
            GetWebRequest(address, "POST");
        }

        public void UploadStringAsync(Uri address, string str)
        {
            GetWebRequest(address, "POST");
        }

        public string GetWebRequest(Uri address, string method = "GET")
        {

            var req = HttpWebRequest.CreateHttp(address);
            req.ContinueTimeout = this._timeout;
            req.Method = method;
            req.Headers = Headers;
            var resp = req.GetResponseAsync().Result;
            var stream = resp.GetResponseStream();
            byte[] b = new byte[stream.Length];
            stream.Read(b, 0, (int)stream.Length);

            var result = System.Text.Encoding.UTF8.GetString(b);
            return result;
        }




        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}