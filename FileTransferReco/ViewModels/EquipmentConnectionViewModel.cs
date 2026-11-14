using FileTransferReco.models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Tools;

namespace FileTransferReco.ViewModels
{
    public class EquipmentConnectionViewModel : BindableBase, IRegionAware, IRegionMemberLifetime
    {
        /// <summary>
        /// 扫描到的可建立连接的设备，不包含已连接的设备
        /// </summary>
        public ObservableCollection<RemoteInfo> CanConnectionSocket { get; set; } = new ObservableCollection<RemoteInfo>();

        public DelegateCommand FlushEquipment { get; set; }

        public AppConfig _AppConfig { get; set; }

        public bool KeepAlive => true;

        public EquipmentConnectionViewModel(AppConfig appConfig)
        {

            //依赖注入
            _AppConfig = appConfig;
            //初始化命令
            FlushEquipment = new DelegateCommand(FlushEquipment_Mehtod);



        }

        /// <summary>
        /// 更新扫描到的设备
        /// </summary>
        private async void FlushEquipment_Mehtod()
        {

            UpdataCanConnectionSocket();
            //发送广播查找设备
            FindEquipment();

            //接收广播 只接收3s
            await ReceiveEquipmentReply();

        }

        /// <summary>
        /// 接收3秒回复
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveEquipmentReply()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            UdpClient replyClient = new UdpClient(_AppConfig.EquipmentServerAddr, _AppConfig.EquipmentServerPort);
            _ = Task.Run(() =>
            {
                try
                {


                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
                        try
                        {

                            byte[] data = replyClient.Receive(ref remoteIP);

                        }
                        catch (Exception ex)
                        {
                            LogHelper.RecordAsync("Udp接收设备已关闭", LogLevel.INFO);
                        }

                        if (remoteIP.Address.ToString() != "0.0.0.0" && !IsInCanConnectionSocket(remoteIP))
                        {
                            RemoteInfo remoteInfo = new RemoteInfo();
                            remoteInfo.IsConnected = false;
                            remoteInfo.RemoteIP = remoteIP.ToString();
                            remoteInfo.RemoteName = remoteIP.ToString();
                            remoteInfo.RemoteSocket = null;
                            LogHelper.RecordAsync($"查找到设备：{remoteIP}", LogLevel.INFO);
                            PrismApplication.Current.Dispatcher.Invoke(() => { CanConnectionSocket.Add(remoteInfo); });

                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.RecordAsync(ex.Message, LogLevel.ERROR);
                }

            }, cancellationTokenSource.Token);

            await Task.Delay(3000);
            LogHelper.RecordAsync("关闭Udp接受设备线程", LogLevel.WARN);
            cancellationTokenSource.Cancel();
            replyClient.Dispose();
        }

        /// <summary>
        /// 该IP是否已经在集合中
        /// </summary>
        /// <param name="remoteIP"></param>
        /// <returns></returns>
        private bool IsInCanConnectionSocket(IPEndPoint remoteIP)
        {
       
            foreach (RemoteInfo remoteInfo in CanConnectionSocket)
            {
                if (remoteInfo.RemoteIP == remoteIP.Address.ToString())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 发送一次广播，查找设备
        /// </summary>
        /// <returns></returns>
        private void FindEquipment()
        {
            Task.Run(() =>
              {
                  UdpClient client = new UdpClient();
                  client.EnableBroadcast = true;
                  //广播查找设备
                  string message = _AppConfig.UdpFindEquipmentData;
                  byte[] data = Encoding.UTF8.GetBytes(message);

                  //发送IP和端口
                  client.SendAsync(data, data.Length,
                      new System.Net.IPEndPoint(IPAddress.Parse(_AppConfig.FindEquipmentServerAddr),
                      _AppConfig.FindEquipmentServerPort));
                  LogHelper.RecordAsync("发送广播：" + message, LogLevel.INFO);

              });


        }


        /// <summary>
        /// 更新可以连接但是未连接的Socket
        /// </summary>
        private void UpdataCanConnectionSocket()
        {
            CanConnectionSocket.Clear();
            foreach (RemoteInfo remote in SocketHelper.ConnectedSocket)
            {
                if (!remote.IsConnected)
                    CanConnectionSocket.Add(remote);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UpdataCanConnectionSocket();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
