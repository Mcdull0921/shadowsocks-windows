using HtmlAgilityPack;
using Shadowsocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks
{
    class GetServers
    {
        static GetServers()
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
        }

        static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {   // 总是接受 认证平台 服务器的证书
            return true;
        }
        public static List<Server> Download()
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string result = client.DownloadString("https://c.ishadowx.com/");
                return load(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Server> LoadLocate(string path)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(path);
                string result = sr.ReadToEnd();
                sr.Dispose();
                return load(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static List<Server> load(string result)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            List<Server> list = new List<Server>();
            var root = doc.DocumentNode.SelectSingleNode("//div[@id='portfolio']");
            var items = root.SelectNodes(@".//div[@class='hover-text']");
            var split = ":".ToCharArray();
            var trim = "\r\n".ToCharArray();
            foreach (HtmlNode item in items)
            {
                var contents = item.SelectNodes("h4").Select(p => p.InnerText).ToArray();
                if (contents[0].Contains("IP"))
                {
                    list.Add(new Server
                    {
                        server = contents[0].Split(split, StringSplitOptions.RemoveEmptyEntries)[1].Trim(trim).Trim(),
                        server_port = int.Parse(contents[1].Split(split, StringSplitOptions.RemoveEmptyEntries)[1].Trim(trim).Trim()),
                        password = contents[2].Split(split, StringSplitOptions.RemoveEmptyEntries)[1].Trim(trim).Trim(),
                        method = contents[3].Split(split, StringSplitOptions.RemoveEmptyEntries)[1].Trim(trim).Trim(),
                        timeout = 5
                    });
                }
            }
            return list;
        }
    }
}
