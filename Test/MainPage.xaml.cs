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
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            StreamResourceInfo input = Application.GetResourceStream(new Uri("/Test;component/test.lzma", UriKind.Relative));
            IsolatedStorageFileStream output = new IsolatedStorageFileStream("test.out", FileMode.Create, store);
            Decoder d = new Decoder();
            d.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(d_ProgressChanged);
            d.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(d_RunWorkerCompleted);
            d.DecodeAsync(input.Stream, output);
        }

        void d_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ProgressText.Text = String.Format("Decompressing...{0}%", e.ProgressPercentage);
            if (Progress.Value <= Progress.Maximum)
                Progress.Value = e.ProgressPercentage;
        }

        void d_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Decompression complete!");
        }
    }
}