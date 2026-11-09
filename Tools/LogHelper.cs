using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 记录日志
    /// </summary>
    public static class LogHelper
    {
        public static FileStream Stream { get; set; }

        public static void Record(string message)
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            string filePath = "Logs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            if(File.Exists(filePath))
            {
                if(Stream==null)
                    Stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                
            }
            else
            {
                Stream?.Dispose();
                Stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,FileShare.Read);
            }

            Stream.Write(Encoding.UTF8.GetBytes($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：{message}\n"));

            Stream.FlushAsync();
            
        }

        public static void RecordAsync(string message)
        {
            Task.Run(() => { Record(message); });
        }
    }
}
