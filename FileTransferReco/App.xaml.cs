using FileTransferReco.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FileTransferReco
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //导航页面注册 
            containerRegistry.RegisterForNavigation<FileTransferView>(); //文件传输界面
            containerRegistry.RegisterForNavigation<EquipmentManagementView>();    //设备管理界面
            containerRegistry.RegisterForNavigation<EquipmentConnectionView>();    //设备界面
            containerRegistry.RegisterForNavigation<SettingsView>();     //设置界面
        }
    }

}
