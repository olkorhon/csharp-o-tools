using System.ComponentModel;
using System.Threading.Tasks;
using Ponsse.Olli.Buffering;
using Olli.Tools.CommieTester.Utility;

namespace Olli.Tools.CommieTester.ViewModels
{
    public class UdpReaderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int port;
        private bool udpToggleChecked;

        public UdpReaderViewModel(int defaultUdpPort)
        {
            Port = defaultUdpPort;
            udpToggleChecked = false;

            Reader = new UdpReader(defaultUdpPort, new RingBuffer<string>(16));
            ReaderViewModel = new RingBufferViewModel(Reader.ReceiveBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Port {
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
        public UdpReader Reader { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBufferViewModel ReaderViewModel { get; set; }

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
                    Reader.Open(Port);
                else
                    Reader.Close();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UdpToggleChecked)));
            }
        }
    }
}
