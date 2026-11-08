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
        public static void Record(string message)
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            string filePath = "Logs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            FileStream stream;
           if(File.Exists(filePath))
            {
                stream = new FileStream(filePath,FileMode.Append,FileAccess.Write);
            }
            else
            {
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            }


            stream.Write(Encoding.UTF8.GetBytes($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}：{message}"));
            stream.Flush();
            stream.Dispose();
        }

        public static void RecordAsync(string message)
        {
            Task.Run(() => { Record(message); });
        }
    }
}
