using FileTransferReco.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace FileTransferReco.ViewModels
{
    public class MainViewModel:BindableBase
    {
        private IRegionManager _regionManager;
        public AppConfig _AppConfig { get; set; }
        //导航到页面
        public DelegateCommand<string> RegionNavigation { get; set; }


        //接受连接
        public DelegateCommand<bool?> AcceptSocket { get; set; }

        public MainViewModel(IRegionManager regionManager,AppConfig appConfig)
        {
            //依赖注入
            _regionManager = regionManager;
            _AppConfig= appConfig;

            //Command初始化
            RegionNavigation = new DelegateCommand<string>(RegionNavigation_Method);
            AcceptSocket = new DelegateCommand<bool?>(AcceptSocket_Method);


            
        }

        private void AcceptSocket_Method(bool? isChecked)
        {
            if (isChecked == true)
            {
                //初始化服务端Socket，持续接受连接(8080端口)
                SocketHelper.StartAccept(_AppConfig.ServerPort);
                LogHelper.RecordAsync($"服务端{_AppConfig.ServerPort}端口已打开",LogLevel.INFO);
            }
            else
            {
                SocketHelper.StopAccept();
                LogHelper.RecordAsync($"服务端{_AppConfig.ServerPort}端口已关闭", LogLevel.INFO);
            }
        }

        /// <summary>
        /// 导航viewName界面
        /// </summary>
        /// <param name="viewName">需要导航到的页面注册名</param>
        private void RegionNavigation_Method(string viewName)
        {
            _regionManager.RequestNavigate("MainRegion", viewName);
            LogHelper.RecordAsync("MainRegion导航到"+viewName, LogLevel.INFO);
        }
    }
}
