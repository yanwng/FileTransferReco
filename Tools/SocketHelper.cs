using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{

    /// <summary>
    /// 远程连接信息
    /// </summary>
    public class RemoteInfo
    {
        public string? RemoteName { get; set; }
        public string RemoteIP { get; set; }
        public Socket? RemoteSocket { get; set; }

        public bool IsConnected { get; set; }

    }

    public static class SocketHelper
    {
        /// <summary>
        /// 当前APP作为服务端所有建立连接的Socket
        /// </summary>
        public static ConcurrentBag<RemoteInfo> ConnectedSocket = new ConcurrentBag<RemoteInfo>();
    
        /// <summary>
        /// 本地服务端Socket
        /// </summary>
        private static Socket? Master { get; set; }

        /// <summary>
        /// 是否处于接受连接状态
        /// </summary>
        private static bool IsStartAccept { get; set; } = false;

        /// <summary>
        /// 控制Master是否继续接受连接
        /// </summary>
        private static CancellationTokenSource? AcceptTokenSource;

        /// <summary>
        /// 控制维护连接状态的token,用于程序退出时取消线程
        /// </summary>
        public static CancellationTokenSource? ConnectedStatusTokenSource;

        /// <summary>
        /// 开始接受连接
        /// </summary>
        /// <param name="master">外部创建服务端</param>
        private static void StartAccept(Socket master)
        {
            //判断是否已经在接受连接
            if (IsStartAccept) return;

            //以外部设置的Sokect连接
            Master = master;
            //重置线程令牌
            AcceptTokenSource = new CancellationTokenSource();
            //设置状态
            IsStartAccept = true;

            //开始维护连接

            ConnectionStatus();
            
            Task.Run(async () => 
            {
                //判断是否取消线程
                while (!AcceptTokenSource.Token.IsCancellationRequested)
                {
                    //接受连接
                    try
                    {
                        Socket socket = await Master.AcceptAsync();
                        //查看是否为曾连接过的IP
                        RemoteInfo result = FindByRemoteEndPoint(socket.RemoteEndPoint);
                        if (result != null) 
                        {
                            result.IsConnected = true;
                            result.RemoteSocket = socket;
                            continue;
                        }

                        IPEndPoint remoteEp = socket.RemoteEndPoint as IPEndPoint;
                        string remoteIP = "";
                        if (remoteEp != null)
                        {
                             remoteIP = remoteEp.Address.ToString();
                        }

                        RemoteInfo remote = new RemoteInfo()
                        {
                            RemoteName = socket.RemoteEndPoint?.ToString(),
                            RemoteIP = remoteIP,
                            RemoteSocket = socket,
                            IsConnected = true
                        };

                        if (!AcceptTokenSource.Token.IsCancellationRequested)
                            ConnectedSocket.Add(remote);
                    }
                    catch (Exception ex) 
                    {
                        LogHelper.RecordAsync(ex.Message+" Socket退出接受连接",LogLevel.INFO);
                    }
                }
                //设置退出接受状态
                IsStartAccept = false;            

            }, AcceptTokenSource.Token);
        }

        /// <summary>
        /// 指定IPEndPoint创建服务端
        /// </summary>
        /// <param name="iPEndPoint"></param>
        public static void StartAccept(IPEndPoint iPEndPoint)
        {
            if (IsStartAccept) return;
            Socket master = new Socket(SocketType.Stream, ProtocolType.Tcp);

            //可能发生异常，向调用者抛出
            master.Bind(iPEndPoint);

            master.Listen();

            StartAccept(master);
        }


        /// <summary>
        /// 监听所有网卡
        /// </summary>
        /// <param name="port"></param>
        public static void StartAccept(int port)
        {
            if(IsStartAccept) return;

            Socket master = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            master.Bind(new IPEndPoint(IPAddress.Any, port));
            master.Listen(10);
            StartAccept(master);
        }

        /// <summary>
        /// 创建以8080端口的服务端接受连接
        /// </summary>
        public static void StartAccept()
        {
            StartAccept(8080);
        }

        /// <summary>
        /// 停止当前APP的服务端接受连接
        /// </summary>
        public static void StopAccept()
        {
            try
            {
                AcceptTokenSource?.Cancel();
            }catch(Exception ex)
            {
                LogHelper.RecordAsync(ex.Message,LogLevel.ERROR);
            }
            Master?.Dispose();
        }

        /// <summary>
        /// 维护连接
        /// </summary>
        private static void ConnectionStatus()
        {
            ConnectedStatusTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!ConnectedStatusTokenSource.Token.IsCancellationRequested)
                {
                    //判断所有维护的连接
                    foreach (RemoteInfo remoteInfo in ConnectedSocket)
                    {
                        if (remoteInfo.RemoteSocket!.Poll(0, SelectMode.SelectRead) && remoteInfo.RemoteSocket.Available == 0)
                        {
                            remoteInfo.IsConnected = false;
                        }
                        else
                        {
                            remoteInfo.IsConnected = true;
                        }
                    }

                    //一秒判断集合一次
                    await Task.Delay(1000);
                }
            }, ConnectedStatusTokenSource.Token);
        }

        /// <summary>
        /// 根据EndPoint在维护的连接中查找
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private static RemoteInfo FindByRemoteEndPoint(EndPoint endPoint)
        {
            IPEndPoint remoteEp = endPoint as IPEndPoint;
            string remoteIP = "";
            if (remoteEp != null)
            {
                remoteIP = remoteEp.Address.ToString();
            }
            return ConnectedSocket.FirstOrDefault<RemoteInfo>(remote=>remote.RemoteIP== remoteIP);
            
        }
    }
}
