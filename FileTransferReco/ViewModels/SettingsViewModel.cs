using FileTransferReco.models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tools;

namespace FileTransferReco.ViewModels
{
    public class SettingsViewModel : BindableBase, IRegionAware, IRegionMemberLifetime
    {

        public DelegateCommand<string> UpdataAppConfig { get; set; }
        public DelegateCommand<TextBox> UpdataServerPort { get; set; }

        public AppConfig _AppConfig { get; set; }

        public bool KeepAlive => false;

        public SettingsViewModel(AppConfig appConfig)
        {
            //依赖注入
            _AppConfig = appConfig;

            //命令初始化
            UpdataAppConfig = new DelegateCommand<string>(UpdataAppConfig_Method);
            UpdataServerPort = new DelegateCommand<TextBox>(UpdataServerPort_Method);
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="name">属性名</param>
        private void UpdataAppConfig_Method(string name)
        {
            switch (name)
            {
                case nameof(_AppConfig.DownLoadFileDir): UpdataDownLoadDir(); break;
            }
        }

        /// <summary>
        /// 更新端口号
        /// </summary>
        private void UpdataServerPort_Method(TextBox textBox)
        {
            int port;
            try
            {
                port = int.Parse(textBox.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("端口号数值或格式不正确");
                textBox.Text = _AppConfig.ServerPort+"";
                return;
            }
            LogHelper.RecordAsync($"修改服务器{_AppConfig.ServerPort}端口为{port}", LogLevel.WARN);
            _AppConfig.ServerPort = port;


        }

        /// <summary>
        /// 更新下载文件的路径
        /// </summary>
        private void UpdataDownLoadDir()
        {
            OpenFolderDialog folder = new OpenFolderDialog();
            folder.Multiselect = false;
            folder.ShowDialog();
            string path = folder.FolderName;
            if(path != "")
            {
                _AppConfig.DownLoadFileDir = path;
            }

        }



        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
