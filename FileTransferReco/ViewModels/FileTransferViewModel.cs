using FileTransferReco.models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FileTransferReco.ViewModels
{

    public class FileTransferViewModel:BindableBase
    {
        public DelegateCommand<TextBlock> OpenSelecteFileDialog {  get; set; }
        public FileDetail  SelectedFile { get; set; }


        public FileTransferViewModel()
        {
            OpenSelecteFileDialog  = new DelegateCommand<TextBlock>(SelecteFile_Method);
        }
        

        private  void SelecteFile_Method(TextBlock block)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
            selectFileDialog.Filter="全部文件(*.*)|*.*";
            selectFileDialog.ShowDialog();
            string filePath =  selectFileDialog.FileName;
            if (filePath == null||filePath=="") return;

            FileInfo fileInfo = new FileInfo(filePath);
            SelectedFile = new FileDetail();
            SelectedFile.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.Length - filePath.LastIndexOf('\\') - 1);
            SelectedFile.FileSize = fileInfo.Length;
            SelectedFile.IsReceive = false;
            SelectedFile.Owner = "localhost";

            
            block.Text = SelectedFile.FileName;
        }
        
    }
}
