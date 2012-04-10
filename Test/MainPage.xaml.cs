using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using SevenZip.Compression.LZMA.WindowsPhone;

namespace Test
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            StreamResourceInfo input = Application.GetResourceStream(new Uri("/Test;component/test.lzma", UriKind.Relative));
            IsolatedStorageFileStream output = new IsolatedStorageFileStream("test.out", FileMode.Create, store);
            StreamDecoder d = new StreamDecoder();
            d.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(d_ProgressChanged);
            d.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(d_RunWorkerCompleted);
            d.DecodeAsync(input.Stream, output);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            StreamResourceInfo input = Application.GetResourceStream(new Uri("/Test;component/test.lzma", UriKind.Relative));
            IsolatedStorageDecoder d = new IsolatedStorageDecoder();
            d.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(d_ProgressChanged);
            d.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(d_RunWorkerCompleted);
            d.DecodeAsync(input.Stream, "test2.out");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            StreamResourceInfo input = Application.GetResourceStream(new Uri("/Test;component/test.lzma", UriKind.Relative));
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageFileStream output = new IsolatedStorageFileStream("test.lzma", FileMode.Create, store);
                int size = 65536;
                byte[] data = new byte[size];
                while ((size = input.Stream.Read(data, 0, data.Length)) > 0)
                {
                    output.Write(data, 0, size);
                }
                output.Close();
            }
            IsolatedStorageDecoder d = new IsolatedStorageDecoder();
            d.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(d_ProgressChanged);
            d.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(d_RunWorkerCompleted);
            d.DecodeAsync("test.lzma", "test3.out");
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            WebClient2IsolatedStorageDecoder d = new WebClient2IsolatedStorageDecoder();
            d.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(d_ProgressChanged);
            d.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(d_RunWorkerCompleted);
            d.DecodeAsync(new Uri("http://localhost/test.lzma"), "test4.out");
        }

        void d_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ProgressText.Text = String.Format("Decompressing...{0}%", e.ProgressPercentage);
            if (Progress.Value <= Progress.Maximum)
                Progress.Value = e.ProgressPercentage;
        }

        void d_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            ProgressText.Text = "Decompression...SUCCESS :)";
        }
    }
}