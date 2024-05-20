using System.ComponentModel;
using Olli.Tools.CommieTester.Utility;

namespace Olli.Tools.CommieTester.ViewModels
{
    public class SerialReaderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool serialToggleChecked;

        public SerialReaderViewModel(string defaultComPort)
        {
            serialToggleChecked = false;

            SPortWrapper = new SerialPortWrapper(defaultComPort);
            ReaderViewModel = new RingBufferViewModel(SPortWrapper.OutputBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public SerialPortWrapper SPortWrapper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBufferViewModel ReaderViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SerialToggleChecked
        {
            get
            {
                return serialToggleChecked;
            }
            set
            {
                serialToggleChecked = value;

                if (serialToggleChecked)
                    SPortWrapper.Open();
                else
                    SPortWrapper.Close();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SerialToggleChecked)));
            }
        }
    }
}
