using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace FileTransferReco.models
{
    public class FileDetail
    {
        public string FilePath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string  FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 文件总字节数
        /// </summary>
        public long    FileSize { get; set; }

        /// <summary>
        /// 是否是发送的文件(不是发送的文件就是接收的文件)
        /// </summary>
        public bool IsReceive { get; set; }

        /// <summary>
        /// 接收的字节数
        /// </summary>
        public long ReceiveSize { get; set; }

        /// <summary>
        /// 发送的字节数
        /// </summary>
        public long SendSize { get; set; }

        /// <summary>
        /// 发送此文件的RemoteInfo
        /// </summary>
        public RemoteInfo Owner { get; set; }

        /// <summary>
        /// 发送/接收文件是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
