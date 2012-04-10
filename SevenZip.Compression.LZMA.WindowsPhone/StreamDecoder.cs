using System;
using System.ComponentModel;
using System.Diagnostics;
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
    public class StreamDecoder : BackgroundWorker, ICodeProgress
    {
        Stream outStream;
        protected long outSize;

        /// <summary>
        /// Encapsulates the decompression function of the LZMA SDK; implemented as a BackgroundWorker.
        /// </summary>
        /// <remarks>Register your own ProgressChanged and RunWorkerCompleted event handlers, e.g.</remarks>
        /// <example>
        /// using SevenZip.Compression.LZMA.WindowsPhone;
        /// ...
        /// StreamDecoder decoder = new StreamDecoder();
        /// decoder.ProgressChanged += new ProgressChangedEventHandler(decoder_ProgressChanged);
        /// decoder.RunWorkerCompleted += new RunWorkerCompletedEventHandler(decoder_RunWorkerCompleted);
        /// decoder.DecodeAsync(inStream, outStream);
        /// ...
        /// void decoder_ProgressChanged(object sender, ProgressChangedEventArgs e)
        /// {
        ///     // TODO: do something with e.ProgressPercentage
        /// ...
        /// void decoder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        /// {
        ///     // TODO: if (e.Error) ... else ...
        /// ...
        /// </example>
        /// <see cref="System.ComponentModel.BackgroundWorker"/>
        public StreamDecoder()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = false;
            DoWork += new DoWorkEventHandler(decoder_DoWork);
        }

        /// <summary>
        /// Starts the asynchronous LZMA decompression operation with I/O streams.
        /// </summary>
        /// <param name="inStream">The System.IO.Stream from which to read the compressed data.</param>
        /// <param name="outStream">The System.IO.Stream to which the decompressed data should be written.</param>
        /// <remarks>Make sure your RunWorkerCompletedEventHandler is attached before calling this method.</remarks>
        /// <see cref="System.ComponentModel.BackgroundWorker.RunWorkerCompleted"/>
        public void DecodeAsync(Stream inStream, Stream outStream)
        {
            this.outStream = outStream;
            RunWorkerAsync(inStream);
        }

        /// <summary>
        /// Internal wrapper implementation for SevenZip.Compression.LZMA.Decoder.
        /// </summary>
        /// <remarks>Implementation taken from LzmaAlone.cs (in the LZMA SDK).</remarks>
        void decoder_DoWork(object sender, DoWorkEventArgs e)
        {
            StreamDecoder decoder = (StreamDecoder)sender;
            Stream inStream = (Stream)e.Argument;
            DateTime start = DateTime.Now;
            
            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw new ArgumentException("LZMA input data is too short");

            Compression.LZMA.Decoder lzmaDecoder = new Compression.LZMA.Decoder();
            lzmaDecoder.SetDecoderProperties(properties);

            outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                if (v < 0)
                    throw new ArgumentException("LZMA input data is empty/unreadable");
                outSize |= ((long)(byte)v) << (8 * i);
            }
            decoder.FreeSpaceRequired(outSize);

            long compressedSize = inStream.Length - inStream.Position;
            lzmaDecoder.Code(inStream, outStream, compressedSize, outSize, this);

            TimeSpan elapsed = DateTime.Now - start;
            int speed = (int)(outSize / 1024 / (int)elapsed.TotalSeconds);
            Debug.WriteLine(String.Format("LZMA decompression took {0}s. for {1}(c.)/{2}(u.) bytes at {3}KB/s", elapsed.TotalSeconds, compressedSize, outSize, speed));
        }

        /// <summary>
        /// Called after reading the LZMA header and before starting decompression.
        /// </summary>
        /// <param name="outSize">Number of bytes that will be written to the outStream.</param>
        /// <remarks>Intended to be overridden by extending classes to handle free space requirements.</remarks>
        protected virtual void FreeSpaceRequired(long outSize)
        {
        }

        #region ICodeProgress interface
        /// <summary>
        /// ICodeProgress callback method, used internally by SevenZip.Compression.LZMA.Decoder. Don't call this from client code!
        /// </summary>
        /// <remarks>May be overridden by extending classes which perform multiple background tasks.</remarks>
        public virtual void SetProgress(long inProgress, long outProgress)
        {
            ReportProgress((int)(100 * outProgress / outSize));
        }
        #endregion
    }
}
