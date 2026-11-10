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

    public class FileTransferViewModel : BindableBase
    {
        /// <summary>
        /// 选择文件
        /// </summary>
        public DelegateCommand<TextBlock> OpenSelecteFileDialog { get; set; }
        /// <summary>
        /// 发送文件
        /// </summary>
        public DelegateCommand<TextBlock> SendFile { get; set; }
        /// <summary>
        /// 选择的文件的详细信息
        /// </summary>
        public FileDetail SelectedFile { get; set; }

        /// <summary>
        /// 发送/接收文件汇总
        /// </summary>
        public ObservableCollection<FileDetail> FileDetailList { get; set; }

        /// <summary>
        /// 选择需要发送到的RemoteInfo
        /// </summary>
        public ConcurrentBag<RemoteInfo> SelectedRemoteInfo { get; set; }

        /// <summary>
        /// 正在接收数据中的socket
        /// </summary>
        public ConcurrentDictionary<RemoteInfo, CancellationTokenSource> ReceivingSocket;

        public FileTransferViewModel()
        {
            //数据初始化
            FileDetailList = new ObservableCollection<FileDetail>();
            SelectedRemoteInfo = new ConcurrentBag<RemoteInfo>();
            ReceivingSocket = new ConcurrentDictionary<RemoteInfo, CancellationTokenSource>();

            //命令初始化
            OpenSelecteFileDialog = new DelegateCommand<TextBlock>(SelecteFile_Method);
            SendFile = new DelegateCommand<TextBlock>(SendFile_Method);

            //开始接收已连接的socket数据
            ReceiveData();
        }

        /// <summary>
        /// 对所有连接的Socket后台接收文件
        /// </summary>
        private void ReceiveData()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    //判断所有已连接的socket是否在接收数据，没在则开始接收数
                    foreach (RemoteInfo remote in SocketHelper.ConnectedSocket)
                    {
                        if (remote.IsConnected && !ReceivingSocket.ContainsKey(remote))
                        {
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            _ = Task.Run(async () =>
                            {
                                while (!cancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    //读取header大小
                                    byte[] headerSize = new byte[4];
                                    if (!SocketHelper.ReceiveFullToBytes(remote, headerSize))
                                    {
                                        LogHelper.RecordAsync($"接收{remote.RemoteIP}文件头大小失败", LogLevel.INFO);
                                        break;
                                    }

                                    //读取文件头
                                    byte[] header = new byte[BitConverter.ToInt32(headerSize)];
                                    if (!SocketHelper.ReceiveFullToBytes(remote, header))
                                    {
                                        LogHelper.RecordAsync($"接收{remote.RemoteIP}文件头失败", LogLevel.INFO);
                                        break;
                                    }
                                    FileDetail? fileDetail;
                                    try
                                    {
                                        fileDetail = JsonSerializer.Deserialize<FileDetail>(Encoding.UTF8.GetString(header));
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.RecordAsync($"{remote.RemoteIP}序列化文件头失败", LogLevel.ERROR);
                                        break;
                                    }

                                    FileDetailList.Add(fileDetail);
                                    int readLen = 0;
                                    byte[] buffer = new byte[30 * 1024];
                                    string dir = "download\\";
                                    using (FileStream stream = new FileStream(dir + fileDetail.FileName, FileMode.Create))
                                        while (fileDetail.ReceiveSize < fileDetail.FileSize)
                                        {
                                            try
                                            {
                                                readLen = await remote.RemoteSocket.ReceiveAsync(buffer);
                                                await stream.WriteAsync(buffer, 0, readLen);

                                            }
                                            catch (Exception ex)
                                            {
                                                LogHelper.RecordAsync(ex.Message + $"--{remote.RemoteIP}接收或写入{fileDetail.FileName}文件异常", LogLevel.ERROR);
                                                break;

                                            }
                                            fileDetail.ReceiveSize += readLen;
                                        }

                                }


                            }, cancellationTokenSource.Token).ContinueWith(task =>
                            {
                                ReceivingSocket.Remove(remote, out var cancelSource);
                                cancelSource!.Cancel();
                            });

                            ReceivingSocket.AddOrUpdate(remote, cancellationTokenSource, (k, v) => v);
                        }
                    }
                    //一秒扫描一次连接过的socket
                    await Task.Delay(1000);
                }

            });
        }


        /// <summary>
        /// 发送选择的文件
        /// </summary>
        /// <param name="block"></param>
        private void SendFile_Method(TextBlock block)
        {
            // if (SelectedFile == null || SelectedRemoteInfo.Count == 0) return;

            string json = JsonSerializer.Serialize(SelectedFile);
            byte[] header = Encoding.UTF8.GetBytes(json);
            int headerSize = header.Length;
            foreach (RemoteInfo remote in SelectedRemoteInfo)
            {
                Task.Run(async () =>
                {
                    //发送文件头大小以及文件头
                    await remote.RemoteSocket!.SendAsync(BitConverter.GetBytes(headerSize));
                    await remote.RemoteSocket!.SendAsync(header);

                    using (FileStream stream =
                    new FileStream(SelectedFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long sendCount = 0;
                        int readLen = 0;
                        byte[] buffer = new byte[30 * 1024];
                        while (sendCount < SelectedFile.FileSize)
                        {
                            readLen = await stream.ReadAsync(buffer, 0, buffer.Length);
                            sendCount += readLen;
                            remote.RemoteSocket.Send(buffer, 0, readLen, System.Net.Sockets.SocketFlags.None);
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
        private void SelecteFile_Method(TextBlock block)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
            selectFileDialog.Filter = "全部文件(*.*)|*.*";
            selectFileDialog.ShowDialog();
            string filePath = selectFileDialog.FileName;
            if (filePath == null || filePath == "" || !File.Exists(filePath)) return;

            FileInfo fileInfo = new FileInfo(filePath);
            SelectedFile = new FileDetail();
            SelectedFile.FilePath = filePath;
            SelectedFile.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.Length - filePath.LastIndexOf('\\') - 1);
            SelectedFile.FileSize = fileInfo.Length;
            SelectedFile.FileType = filePath.Substring(filePath.LastIndexOf('.'), filePath.Length - filePath.LastIndexOf('.'));
            SelectedFile.IsReceive = false;
            SelectedFile.Owner = null;


            block.Text = SelectedFile.FileName;
        }

    }
}
