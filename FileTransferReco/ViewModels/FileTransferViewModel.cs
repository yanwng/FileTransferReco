using FileTransferReco.models;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Tools;

namespace FileTransferReco.ViewModels
{

    public class FileTransferViewModel:BindableBase
    {
        /// <summary>
        /// 选择文件
        /// </summary>
        public DelegateCommand<TextBlock> OpenSelecteFileDialog {  get; set; }
        /// <summary>
        /// 发送文件
        /// </summary>
        public DelegateCommand<TextBlock> SendFile {  get; set; }
        /// <summary>
        /// 选择的文件的详细信息
        /// </summary>
        public FileDetail  SelectedFile { get; set; }

        /// <summary>
        /// 发送/接收文件汇总
        /// </summary>
        public ObservableCollection<FileDetail> FileDetailList { get; set; }

        /// <summary>
        /// 选择需要发送到的RemoteInfo
        /// </summary>
        public ConcurrentBag<RemoteInfo> SelectedRemoteInfo { get; set; }

        public FileTransferViewModel()
        {
            //数据初始化
            FileDetailList = new ObservableCollection<FileDetail>();
            SelectedRemoteInfo = new ConcurrentBag<RemoteInfo>();


            //命令初始化
            OpenSelecteFileDialog  = new DelegateCommand<TextBlock>(SelecteFile_Method);
            SendFile = new DelegateCommand<TextBlock>(SendFile_Method);
        }


        /// <summary>
        /// 发送选择的文件
        /// </summary>
        /// <param name="block"></param>
        private void SendFile_Method(TextBlock block)
        {
           // if (SelectedFile == null || SelectedRemoteInfo.Count == 0) return;

            string json = JsonSerializer.Serialize(SelectedFile);
            byte[] header =  Encoding.UTF8.GetBytes(json);
            int headerSize = header.Length;
            foreach (RemoteInfo remote in SelectedRemoteInfo)
            {
                Task.Run(async () => 
                {
                    //发送文件头大小以及文件头
                   await remote.RemoteSocket!.SendAsync(BitConverter.GetBytes(headerSize));
                   await remote.RemoteSocket!.SendAsync(header);

                    using(FileStream stream = 
                    new FileStream(SelectedFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long sendCount = 0;
                        int readLen = 0;
                        byte[] buffer = new byte[30*1024];
                        while (sendCount < SelectedFile.FileSize)
                        {
                            readLen = await stream.ReadAsync(buffer, 0, buffer.Length);
                            sendCount+= readLen;
                            remote.RemoteSocket.Send(buffer,0,readLen,System.Net.Sockets.SocketFlags.None);
                            SelectedFile.SendSize = sendCount;
                        }
                    }
                    
                });
            }


        }


        /// <summary>
        /// 选择需要发送的文件
        /// </summary>
        /// <param name="block">显示选择的文件</param>
        private  void SelecteFile_Method(TextBlock block)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
            selectFileDialog.Filter="全部文件(*.*)|*.*";
            selectFileDialog.ShowDialog();
            string filePath =  selectFileDialog.FileName;
            if (filePath == null||filePath==""||!File.Exists(filePath)) return;

            FileInfo fileInfo = new FileInfo(filePath);
            SelectedFile = new FileDetail();
            SelectedFile.FilePath = filePath;
            SelectedFile.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.Length - filePath.LastIndexOf('\\') - 1);
            SelectedFile.FileSize = fileInfo.Length;
            SelectedFile.FileType= filePath.Substring(filePath.LastIndexOf('.'), filePath.Length - filePath.LastIndexOf('.')); 
            SelectedFile.IsReceive = false;
            SelectedFile.Owner = null; 

            
            block.Text = SelectedFile.FileName;
        }
        
    }
}
