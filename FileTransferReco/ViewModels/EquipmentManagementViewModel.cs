using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace FileTransferReco.ViewModels
{
    public class EquipmentManagementViewModel:BindableBase,INavigationAware
    {
        /// <summary>
        /// 所有设备
        /// </summary>
        public ObservableCollection<RemoteInfo> ConnectedSocket { get; set; } = new ObservableCollection<RemoteInfo>();

        public EquipmentManagementViewModel()
        {
            
           
        }


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ConnectedSocket.Clear();
            foreach (var item in SocketHelper.ConnectedSocket)
            {
                ConnectedSocket.Add(item);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }
    }
}
