using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace FileTransferReco.ViewModels
{
    public class EquipmentConnectionViewModel : BindableBase,IRegionAware,IRegionMemberLifetime
    {
        public ObservableCollection<RemoteInfo> ConnectionSocket { get; set; } = new ObservableCollection<RemoteInfo>();

        public bool KeepAlive => false;

        public EquipmentConnectionViewModel()
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            foreach (RemoteInfo remote in SocketHelper.ConnectedSocket)
            {
                ConnectionSocket.Add(remote);
            }
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
