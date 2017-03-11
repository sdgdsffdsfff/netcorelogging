using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Logging.Server
{
    public class WebClient
    {
        public string UploadString(Uri address, string content)
        {
            string resp = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpContent c = new StringContent(content);
                var ret = client.PostAsync(address, c);
                resp = ret.Result.Content.ReadAsStringAsync().Result;
            }
            return resp;
        }

        public async Task<HttpResponseMessage> UploadStringAsync(Uri address, string content)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpContent c = new StringContent(content);
                return await client.PostAsync(address, c);
            }
        }
    }
}