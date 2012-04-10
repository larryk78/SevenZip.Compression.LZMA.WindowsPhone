using System;
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

        public WebClient2IsolatedStorageDecoder()
            : base()
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
        }

        public void DecodeAsync(Uri inUri, string outFile)
        {
            this.outFile = outFile;
            webClient.OpenReadAsync(inUri);
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ReportProgress(e.ProgressPercentage / 2); // 0-50% progress is download
        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            base.DecodeAsync((Stream)e.Result, outFile);
        }

        public override void SetProgress(long inProgress, long outProgress)
        {
            ReportProgress(50 + (int)(100 * outProgress / outSize) / 2); // 50-100% progress is decompression
        }
    }
}
