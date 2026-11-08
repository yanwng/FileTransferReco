using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferReco.models
{
    public class FileDetail
    {

        public string  FileName { get; set; }
        public long    FileSize { get; set; }

        public bool IsReceive { get; set; }

        public long ReceiveSize { get; set; }

        public long SendSize { get; set; }

        public string Owner { get; set; }
    }
}
