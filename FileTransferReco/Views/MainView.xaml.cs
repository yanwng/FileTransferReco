using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileTransferReco.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IRegionManager regionManager)
        {
            InitializeComponent();

            //扫描该页面Region并注册
            RegionManager.SetRegionManager(this, regionManager);
            RegionManager.UpdateRegions();

            //加载初始界面
            regionManager.RequestNavigate("MainRegion", "FileTransferView");
        }

       
    }
}
