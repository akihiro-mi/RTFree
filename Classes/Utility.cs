using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RTFree.Classes
{
    public class Utility
    {
        /// <summary>
        /// プレミアムにログインおよび、ログイン状態確認
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        static async public Task<bool> Login(string mail = "", string pass = "")
        {
            bool res = false;

            if (!string.IsNullOrWhiteSpace(mail) && !string.IsNullOrWhiteSpace(pass))
            {
                // メール・パスワードがあればログイン
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Define.Login);

                    // ヘッダー
                    req.CookieContainer = Globals.Cookie;
                    req.Method          = "post";
                    req.Accept          = "application/json";

                    // post
                    string post         = "mail" + "=" + WebUtility.UrlEncode(mail) + "&" + "pass" + "=" + WebUtility.UrlEncode(pass);
                    byte[] data         = Encoding.ASCII.GetBytes(post);

                    req.ContentType              = "application/x-www-form-urlencoded";
                    req.Headers["ContentLength"] = Convert.ToString(data.Length);

                    using (Stream stream = await req.GetRequestStreamAsync())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse webres = (HttpWebResponse)(await req.GetResponseAsync());
                    using (var r = new StreamReader(webres.GetResponseStream(), Encoding.UTF8))
                    {
                        var ss = r.ReadToEnd();
                    }

                }
                catch (Exception e)
                {

                }
            }

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Define.LoginCheck);
                req.CookieContainer = Globals.Cookie;
                HttpWebResponse webres = (HttpWebResponse)(await req.GetResponseAsync());
                using (var r = new StreamReader(webres.GetResponseStream(), Encoding.UTF8))
                {
                    var json = r.ReadToEnd();
                    if (json.Contains("areafree"))
                    {
                        res = true;
                    }

                }
            }
            catch (Exception e)
            {

            }
            return res;
        }

        static async public Task<string> GetToken()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Task[] tasks = new Task[] {
                GetToken(data),
                GetImage(data)
            };
            Task.WaitAll(tasks);

            string token = (string)data["token"];
            int key_length = (int)data["key_length"];
            int key_offset = (int)data["key_offset"];
            string image = (string)data["image"];
            string partial_key = "";

            if (!string.IsNullOrWhiteSpace(token) && key_length > 0 && key_offset > 0 && !string.IsNullOrWhiteSpace(image) && File.Exists(image))
            {
                using (FileStream st = new FileStream(image, FileMode.Open, FileAccess.Read))
                {
                    st.Seek(key_offset, SeekOrigin.Begin);
                    var a = st.Length;
                    byte[] tmp = new byte[key_length];
                    st.Read(tmp, 0, key_length);
                    partial_key = Convert.ToBase64String(tmp);
                }
            }

            if (!string.IsNullOrWhiteSpace(partial_key))
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Define.Auth2);

                // ヘッダー
                req.ContentType = "application/x-www-form-urlencoded";
                req.Headers["pragma"] = "no-cache";
                req.Headers["X-Radiko-App"] = "pc_ts";
                req.Headers["X-Radiko-App-Version"] = "4.0.0";
                req.Headers["X-Radiko-User"] = "test-stream";
                req.Headers["X-Radiko-Device"] = "pc";
                req.Headers["X-Radiko-AuthToken"] = token;
                req.Headers["X-Radiko-Partialkey"] = partial_key;


                req.CookieContainer = Globals.Cookie;

                req.Method = "post";

                HttpWebResponse webres = (HttpWebResponse)(await req.GetResponseAsync());
                var res = webres.StatusCode;

                using (var r = new StreamReader(webres.GetResponseStream(), Encoding.UTF8))
                {
                    var ss = r.ReadToEnd();
                }


            }
            return token;
        }

        /// <summary>
        /// トークン取得
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static async private Task GetToken(Dictionary<string, object> data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Define.Auth1);

            // ヘッダー
            req.Headers["pragma"]               = "no-cache";
            req.Headers["X-Radiko-App"]         = "pc_ts";
            req.Headers["X-Radiko-App-Version"] = "4.0.0";
            req.Headers["X-Radiko-User"]        = "test-stream";
            req.Headers["X-Radiko-Device"]      = "pc";


            req.CookieContainer = Globals.Cookie;
            req.Method = "post";

            HttpWebResponse webres = (HttpWebResponse)(await req.GetResponseAsync());
            data.Add("token", webres.Headers["x-radiko-authtoken"]);
            data.Add("key_length", int.Parse(webres.Headers["x-radiko-keylength"]));
            data.Add("key_offset", int.Parse(webres.Headers["x-radiko-keyoffset"]));

        }

        static async private Task GetImage(Dictionary<string, object> data)
        {
            Directory.CreateDirectory(Define.WorkDir);
            string swf    = Path.Combine(Define.WorkDir, "player.swf");
            string image  = Path.Combine(Define.WorkDir, "image.png");
            data["image"] = image;

            // 存在チェック
            if (!File.Exists(swf) || (DateTime.Now - File.GetLastWriteTime(swf)).TotalHours > 24)
            {
                HttpClient hc = new HttpClient();
                HttpResponseMessage res = await hc.GetAsync(Define.Swf, HttpCompletionOption.ResponseHeadersRead);

                using (var fileStream = File.Create(swf))
                {
                    using (var httpStream = await res.Content.ReadAsStreamAsync())
                    {
                        httpStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                }
            }

            if (File.Exists(swf) && (!File.Exists(image) || (DateTime.Now - File.GetLastWriteTime(image)).TotalHours > 24))
            {
                Process p = new Process();
                p.StartInfo.CreateNoWindow  = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments       = "-b 12 \"" + swf + "\" -o \"" + (string)data["image"] + "\"";
                p.StartInfo.FileName        = Define.SwfExtract;
                p.Start();

                p.WaitForExit();
            }
        }

      
        static async public Task<string> GetPlayList(string token, string station_id, string from, string to)
        {
            string res = "";
            string url = string.Format(Define.PlayList, station_id, from, to);


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            // ヘッダー
            req.Headers["pragma"]             = "no-cache";
            req.Headers["X-Radiko-AuthToken"] = token;

            req.CookieContainer = Globals.Cookie;

            req.Method = "post";

            HttpWebResponse webres = (HttpWebResponse)(await req.GetResponseAsync());

            using (var r = new StreamReader(webres.GetResponseStream(), Encoding.UTF8))
            {
                string line = "";
                while ((line = r.ReadLine()) != null)
                {
                    if (line.Contains("http"))
                    {
                        res = line;
                    }
                }
            }

            return res;


        }
    }
}
