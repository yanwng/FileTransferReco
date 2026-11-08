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
    public class EquipmentConnectionViewModel : BindableBase
    {
        public ObservableCollection<RemoteInfo> ConnectionSocket { get; set; } = new ObservableCollection<RemoteInfo>();

        public EquipmentConnectionViewModel()
        {
            
        }
    }
}
