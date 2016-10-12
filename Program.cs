using RTFree.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RTFree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 引数
            string mail = "", pass = "", station_id = "", from = "", to = "", filename = "";
            for(int i=0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-m":
                        mail = args[++i];
                        break;

                    case "-p":
                        pass = args[++i];
                        break;

                    case "-s":
                        station_id = args[++i];
                        break;

                    case "-f":
                        from = args[++i];
                        break;

                    case "-t":
                        to = args[++i];
                        break;

                    case "-n":
                        filename = args[++i];
                        break;
                }
            }

            if(string.IsNullOrWhiteSpace(station_id) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                Console.WriteLine("-s, -f, -t is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = station_id + "_" + from + "_" + to;
            }

            Task task = Task.Factory.StartNew(async () =>
            {
                bool login   = await Utility.Login(mail, pass);
                Console.WriteLine("premium:" + (login ? "enabled" : "disabled"));
                string token = await Utility.GetToken();
                Console.WriteLine("token:" + token);
                string m3u8  = await Utility.GetPlayList(token, station_id, from, to);

                if (!string.IsNullOrWhiteSpace(m3u8))
                {
                    Console.WriteLine("start ffmpeg");
                    Process p = new Process();
                    p.StartInfo.FileName = Define.Ffmpeg;
                    p.StartInfo.Arguments = string.Format(Define.FfmpegArgs, m3u8, filename);
                    p.Start();
                    p.WaitForExit();
                }
                else
                {
                    Console.WriteLine("couldn't get the m3u8");
                }

            }).Unwrap();
            task.Wait();
        }

        
    }
}
