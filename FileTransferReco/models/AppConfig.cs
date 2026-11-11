using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferReco.models
{
    public class AppConfig:INotifyPropertyChanged
    {
        /// <summary>
        /// 文件下载所保存的路径
        /// </summary>
        private string downLoadFileDir;

        public string DownLoadFileDir
        {
            get { return downLoadFileDir; }
            set { downLoadFileDir = value; 
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(downLoadFileDir)));
            }
        }

        /// <summary>
        /// 服务器响应的端口
        /// </summary>
        public int ServerPort { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
