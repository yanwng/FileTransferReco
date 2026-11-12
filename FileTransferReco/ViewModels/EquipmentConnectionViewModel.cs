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

        private async Task ReceiveEquipmentReply()
        {
            await Task.Run(() =>
            {

            });
        }

        /// <summary>
        /// 发送一次广播，将当前局域网IP传过去
        /// </summary>
        /// <returns></returns>
        private void FindEquipment()
        {
            Task.Run(() =>
              {
                  List<RemoteInfo> list = new List<RemoteInfo>();
                  UdpClient client = new UdpClient();
                  client.EnableBroadcast = true;
                  //广播发送服务器地址
                  string message = $"{_AppConfig.FindEquipmentServerAddr}-{_AppConfig.ServerPort}";
                  byte[] data = Encoding.UTF8.GetBytes(message);
                  //发送广播
                  client.SendAsync(data, data.Length,
                      new System.Net.IPEndPoint(IPAddress.Parse(_AppConfig.FindEquipmentServerAddr),
                      _AppConfig.FindEquipmentServerPort));
                  LogHelper.RecordAsync("发送广播" + message, LogLevel.INFO);

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
