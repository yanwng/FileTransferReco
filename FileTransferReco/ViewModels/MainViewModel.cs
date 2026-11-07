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
        //导航到页面
        public DelegateCommand<string> RegionNavigation { get; set; }
        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            //Command初始化

            RegionNavigation = new DelegateCommand<string>(RegionNavigation_Method);

            //初始化服务端Socket，持续接受连接(8080端口)
            SocketHelper.StartAccept();
        }

        /// <summary>
        /// 导航viewName界面
        /// </summary>
        /// <param name="viewName">需要导航到的页面注册名</param>
        private void RegionNavigation_Method(string viewName)
        {
            _regionManager.RequestNavigate("MainRegion", viewName);
        }
    }
}
