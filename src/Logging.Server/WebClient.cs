using System;
using System.IO;
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

        private Stream _respStream;

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
            Request(address, "POST");
        }

        public void UploadStringAsync(Uri address, string str)
        {
            Request(address, "POST");
        }

        public string Request(Uri address, string method = "GET")
        {

            var req = HttpWebRequest.CreateHttp(address);
            req.ContinueTimeout = this._timeout;
            req.Method = method;
            req.Headers = Headers;
            var resp = req.GetResponseAsync().Result;
             _respStream = resp.GetResponseStream();
            byte[] b = new byte[_respStream.Length];
            _respStream.Read(b, 0, (int)_respStream.Length);

            var result = System.Text.Encoding.UTF8.GetString(b);
            _respStream?.Dispose();
            return result;
        }




        public void Dispose()
        {
            _respStream?.Dispose();
            // throw new NotImplementedException();
        }
    }
}