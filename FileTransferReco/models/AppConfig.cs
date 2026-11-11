using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferReco.models
{
    public class AppConfig
    {
        /// <summary>
        /// 文件下载所保存的路径
        /// </summary>
        public string DownLoadFileDir;
        /// <summary>
        /// 服务器响应的端口
        /// </summary>
        public int ServerPort { get; set; }


    }
}
