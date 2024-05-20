using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Ponsse.Olli.Buffering;
using Olli.Tools.CommieTester.Utility;

namespace Olli.Tools.CommieTester.ViewModels
{
    public class UdpGeneratorViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Task generateDataTask;

        private int port;
        private bool udpToggleChecked;

        public UdpGeneratorViewModel(int defaultUdpPort)
        {
            generateDataTask = new Task(GenerateDataLoop);

            Port = defaultUdpPort;
            udpToggleChecked = false;

            Writer = new UdpWriter(defaultUdpPort, new RingBuffer<string>(1024));
            BufferViewModel = new RingBufferViewModel(Writer.SendBuffer);

            generateDataTask.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Port)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UdpWriter Writer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBufferViewModel BufferViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UdpToggleChecked
        {
            get { return udpToggleChecked; }
            set
            {
                udpToggleChecked = value;

                if (udpToggleChecked)
                    Writer.Open(Port);
                else
                    Writer.Close();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UdpToggleChecked)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateDataLoop()
        {
            while (true)
            {
                if (!UdpToggleChecked)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                Writer.SendBuffer.Buffer($"{Guid.NewGuid()}" + Environment.NewLine);
                Thread.Sleep(1000);
            }
        }
    }
}
