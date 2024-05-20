using System.ComponentModel;
using Ponsse.Olli.Buffering;
using Olli.Tools.CommieTester.Utility;

namespace Olli.Tools.CommieTester.ViewModels
{
    public class ConverterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool serialToggleChecked;
        private bool udpToggleChecked;

        /// <summary>
        /// 
        /// </summary>
        public ConverterViewModel(string defaultComPort, int defaultUdpPort)
        {
            serialToggleChecked = false;
            udpToggleChecked = false;

            RingBuffer<string> sendDataBuffer = new RingBuffer<string>(1024);
            RingBuffer<string> receiveDataBuffer = new RingBuffer<string>(1024);
            
            SPortWrapper = new SerialPortWrapper(defaultComPort, sendDataBuffer, receiveDataBuffer);
            UDPPortWrapper = new UdpWriter(defaultUdpPort, receiveDataBuffer);
            BufferViewModel = new RingBufferViewModel(receiveDataBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public int UDPPort {
            get
            {
                return UDPPortWrapper.Port;
            }
            set
            {
                UDPPortWrapper.Port = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UDPPort)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SerialPortWrapper SPortWrapper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UdpWriter UDPPortWrapper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBufferViewModel BufferViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SerialToggleChecked {
            get
            {
                return serialToggleChecked;
            }
            set
            {
                serialToggleChecked = value;

                if (serialToggleChecked)
                {
                    if (!SPortWrapper.Open())
                        serialToggleChecked = false;
                }
                else
                    SPortWrapper.Close();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SerialToggleChecked)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UDPToggleChecked {
            get
            {
                return udpToggleChecked;
            }
            set
            {
                udpToggleChecked = value;

                if (udpToggleChecked)
                    UDPPortWrapper.Open(UDPPort);
                else
                    UDPPortWrapper.Close();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UDPToggleChecked)));
            }
        }
    }
}
