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

        public static async Task Record(string message,LogLevel logLevel)
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            string filePath = "Logs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
            if(File.Exists(filePath))
            {
                if(Stream==null)
                    Stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                
            }
            else
            {
                Stream?.Dispose();
                Stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,FileShare.Read);
                Stream.Write(Encoding.UTF8.GetBytes("日期,详情,级别\n"));
                await Stream.FlushAsync();
            }

            Stream.Write(Encoding.UTF8.GetBytes($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},{message},{GetLogLevelString(logLevel)}    \n"));

            await Stream.FlushAsync();
            
        }

        public static void RecordAsync(string message, LogLevel logLevel)
        {
            Task.Run(async () => { await Record(message, logLevel); });
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.ERROR: return "ERROR";
                case LogLevel.WARN: return "WARN";
                case LogLevel.INFO: return "INFO";
            }
            return "";
        }
    }
}
