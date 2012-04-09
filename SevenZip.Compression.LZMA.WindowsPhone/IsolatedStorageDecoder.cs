using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SevenZip.Compression.LZMA.WindowsPhone
{
    public class IsolatedStorageDecoder : Decoder
    {
        IsolatedStorageFile store;

        public IsolatedStorageDecoder()
            : base()
        {
            RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(IsolatedStorageDecoder_RunWorkerCompleted);
        }

        public void DecodeAsync(string inputFile, string outputFile)
        {
            store = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream inStream = new IsolatedStorageFileStream(inputFile, FileMode.Open, store);
            DecodeAsync(inStream, outputFile);
        }

        public void DecodeAsync(Stream inStream, string outputFile)
        {
            try
            {
                store = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream outStream = new IsolatedStorageFileStream(outputFile, FileMode.Create, store);
                DecodeAsync(inStream, outStream);
            }
            catch (IsolatedStorageException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void FreeSpaceRequired(long bytes)
        {
            if (store.AvailableFreeSpace < bytes)
            {
                store.IncreaseQuotaTo(store.Quota + bytes);
            }
        }

        void IsolatedStorageDecoder_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            store.Dispose();
        }
    }
}
