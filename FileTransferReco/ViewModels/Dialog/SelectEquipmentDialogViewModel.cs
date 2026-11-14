using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace FileTransferReco.ViewModels.Dialog
{
    public class SelectEquipmentDialogViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "选择设备";

        private ObservableCollection<RemoteInfo> canSendSockets;

        /// <summary>
        /// 能够选择的Socket
        /// </summary>
        public ObservableCollection<RemoteInfo> CanSendSockets
        {
            get { return canSendSockets; }
            set { SetProperty(ref canSendSockets, value); }
        }

        public List<RemoteInfo> Remotes { get; set; } = new List<RemoteInfo>();


        public DialogCloseListener RequestClose { get; set; }

        public SelectEquipmentDialogViewModel()
        {
            
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add("selectedRemotes",Remotes);
            DialogResult result = new DialogResult();
            result.Parameters = parameters;
            RequestClose.Invoke(result);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            foreach (RemoteInfo remote in SocketHelper.ConnectedSocket)
            {
                if(remote.IsConnected)
                    canSendSockets.Add(remote);
            }
        }
    }
}
