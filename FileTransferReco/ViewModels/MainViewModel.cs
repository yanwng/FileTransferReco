using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferReco.ViewModels
{
    public class MainViewModel:BindableBase
    {
        private IRegionManager _regionManager;
        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            
        }
    }
}
