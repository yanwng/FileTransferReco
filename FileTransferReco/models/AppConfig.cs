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


        /// <summary>
        /// 寻找设备广播的网段
        /// </summary>
        public string FindEquipmentServerAddr { get; set; } = "10.81.76.255";
        /// <summary>
        /// 寻找设备广播的端口
        /// </summary>
        public int FindEquipmentServerPort { get; set; } = 23499;

        /// <summary>
        /// 广播回复IP
        /// </summary>
        public string EquipmentServerAddr { get; set; } = "10.81.76.178";
        /// <summary>
        /// 广播回复端口
        /// </summary>
        public int EquipmentServerPort { get; set; } = 22499;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
