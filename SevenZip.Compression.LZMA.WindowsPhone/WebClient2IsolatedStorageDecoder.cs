using System;
using System.ComponentModel;
using System.IO;
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
    public class WebClient2IsolatedStorageDecoder : IsolatedStorageDecoder
    {
        WebClient webClient;
        string outFile;

        /// <summary>
        /// Extends IsolatedStorageDecoder to download a compressed file and decompress to a file in IsolatedStorage.
        /// </summary>
        /// <see cref="SevenZip.Compression.LZMA.WindowsPhone.IsolatedStorageDecoder"/>
        public WebClient2IsolatedStorageDecoder()
            : base()
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
        }

        /// <summary>
        /// Starts the URI-to-file asynchronous download and decompression operation.
        /// </summary>
        /// <param name="inFile">The URI to the compressed file to download.</param>
        /// <param name="outFile">The path to the file (to be created) to which the decompressed data should be written.</param>
        /// <exception cref="WebClient.OpenReadAsync" />
        public void DecodeAsync(Uri inUri, string outFile)
        {
            this.outFile = outFile;
            webClient.OpenReadAsync(inUri); // download
        }

        /// <summary>
        /// Reports progress during the download.
        /// </summary>
        /// <remarks>Download is operation 1 of 2 so the progress is 0-50, not 0-100.</remarks>
        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ReportProgress(e.ProgressPercentage / 2); // 0-50%
        }

        /// <summary>
        /// Checks that the download completed successfully and kicks off the decompression, or aborts.
        /// </summary>
        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.OnRunWorkerCompleted(new RunWorkerCompletedEventArgs(null, e.Error, false));
            }
            else
            {
                base.DecodeAsync((Stream)e.Result, outFile); // decompress
            }
        }

        /// <summary>
        /// Overrides StreamDecoder.SetProgress to report progress during decompression.
        /// </summary>
        /// <remarks>Decompression is operation 2 of 2 so the progress is 50-100, not 0-100.</remarks>
        public override void SetProgress(long inProgress, long outProgress)
        {
            ReportProgress(50 + (int)(100 * outProgress / outSize) / 2); // 50-100% progress is decompression
        }
    }
}
