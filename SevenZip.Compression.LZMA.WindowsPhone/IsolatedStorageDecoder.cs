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
    public class IsolatedStorageDecoder : StreamDecoder
    {
        IsolatedStorageFile store = null;
        IsolatedStorageFileStream outStream;

        /// <summary>
        /// Extends StreamDecoder to decompress LZMA to a file in IsolatedStorage.
        /// </summary>
        /// <see cref="SevenZip.Compression.LZMA.WindowsPhone.StreamDecoder"/>
        public IsolatedStorageDecoder()
            : base()
        {
            RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(IsolatedStorageDecoder_RunWorkerCompleted);
        }

        /// <summary>
        /// Starts the file-to-file asynchronous LZMA decompression operation.
        /// </summary>
        /// <param name="inFile">The path to the file from which to read the compressed data.</param>
        /// <param name="outFile">The path to the file (to be created) to which the decompressed data should be written.</param>
        /// <exception cref="System.IO.IsolatedStorage.IsolatedStorageException" />
        public void DecodeAsync(string inFile, string outFile)
        {
            store = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream inStream = new IsolatedStorageFileStream(inFile, FileMode.Open, store);
            DecodeAsync(inStream, outFile);
        }

        /// <summary>
        /// Starts the stream-to-file asynchronous LZMA decompression operation.
        /// </summary>
        /// <param name="inStream">The stream from which to read the compressed data.</param>
        /// <param name="outFile">The path to the file (to be created) to which the decompressed data should be written.</param>
        /// <exception cref="System.IO.IsolatedStorage.IsolatedStorageException" />
        public void DecodeAsync(Stream inStream, string outFile)
        {
            if (store == null)
                store = IsolatedStorageFile.GetUserStoreForApplication();
            outStream = new IsolatedStorageFileStream(outFile, FileMode.Create, store);
            DecodeAsync(inStream, outStream);
        }

        /// <summary>
        /// Overrides StreamDecoder.FreeSpaceRequired to extend the IsolatedStorage quota, if necessary.
        /// </summary>
        /// <see cref="SevenZip.Compression.LZMA.StreamDecoder.FreeSpaceRequired"/>
        protected override bool FreeSpaceRequired(long bytes)
        {
            if (store.AvailableFreeSpace >= bytes)
                return true;
            return store.IncreaseQuotaTo(store.Quota + bytes);
        }

        /// <summary>
        /// Clean up resources on completion of decompression.
        /// </summary>
        void IsolatedStorageDecoder_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (outStream != null)
                outStream.Close();
            if (store != null)
                store.Dispose();
        }
    }
}
