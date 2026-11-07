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
            //文件传输界面 
            containerRegistry.RegisterForNavigation<FileTransferView>();
        }
    }

}
