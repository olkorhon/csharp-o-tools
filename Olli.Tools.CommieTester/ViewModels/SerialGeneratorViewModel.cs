using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Olli.Tools.CommieTester.Utility;

namespace Olli.Tools.CommieTester.ViewModels
{
    public class SerialGeneratorViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Task generateDataTask;
        private bool serialToggleChecked;

        public SerialGeneratorViewModel(string defaultComPort)
        {
            generateDataTask = new Task(GenerateDataLoop);

            SPortWrapper = new SerialPortWrapper(defaultComPort);
            BufferViewModel = new RingBufferViewModel(SPortWrapper.InputBuffer);

            generateDataTask.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public SerialPortWrapper SPortWrapper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBufferViewModel BufferViewModel { get; set; }

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
        private void GenerateDataLoop()
        {
            while (true)
            {
                if (!SerialToggleChecked)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                SPortWrapper.InputBuffer.Buffer($"{Guid.NewGuid()}" + Environment.NewLine);
                Thread.Sleep(1000);
            }
        }
    }
}
