using FileTransferReco.models;
using FileTransferReco.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using Tools;

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
            //加载注册App配置
            _ = LoadAppConfig(containerRegistry);

            //导航页面注册 
            containerRegistry.RegisterForNavigation<FileTransferView>(); //文件传输界面
            containerRegistry.RegisterForNavigation<EquipmentManagementView>();    //设备管理界面
            containerRegistry.RegisterForNavigation<EquipmentConnectionView>();    //设备界面
            containerRegistry.RegisterForNavigation<SettingsView>();     //设置界面

        }

        /// <summary>
        /// 加载并注册配置
        /// </summary>
        /// <param name="containerRegistry"></param>
        private async Task LoadAppConfig(IContainerRegistry containerRegistry)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppConfig));
            //配置文件存在
            if (File.Exists("AppConfig.xml"))
            {

                using (FileStream stream = new FileStream("AppConfig.xml", FileMode.Open))
                {

                    AppConfig? appConfig = (AppConfig?)xmlSerializer.Deserialize(stream);
                    if (appConfig == null)
                    {
                        await LogHelper.Record("配置文件为空", LogLevel.ERROR);
                        MessageBox.Show("配置文件缺失", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Shutdown();

                    }
                    else
                    {
                        containerRegistry.RegisterInstance(appConfig);
                    }
                }
            }
            else
            {
                //首次启动，创建配置文件
                AppConfig appConfig = new AppConfig();
                appConfig.DownLoadFileDir = Directory.GetCurrentDirectory()+"\\downLoad";
                appConfig.ServerPort = 8080;

                using (FileStream stream = new FileStream("AppConfig.xml", FileMode.Create))
                {
                    try
                    {
                        xmlSerializer.Serialize(stream, appConfig);
                        containerRegistry.RegisterInstance(appConfig);

                    }
                    catch (Exception ex)
                    {
                        await LogHelper.Record("创建配置文件失败，message：" + ex.Message, LogLevel.ERROR);
                        this.Shutdown();
                    }
                }

            }
        }
    }

}
