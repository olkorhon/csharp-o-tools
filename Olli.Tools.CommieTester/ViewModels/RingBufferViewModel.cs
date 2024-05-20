using System.ComponentModel;
using System.Linq;
using System.Text;
using Ponsse.Olli.Buffering;
using Olli.Tools.CommieTester.Utility;

namespace Olli.Tools.CommieTester.ViewModels
{
    public class RingBufferViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object bufferLock = new object();

        private bool logMessagesChecked;

        public RingBufferViewModel(RingBuffer<string> ringBuffer)
        {
            ClearLogAction = new CommandHandler(() => {
                BufferEventLog = string.Empty;
                InvokePropertyChanged(nameof(BufferEventLog));
            });

            DataBuffer = ringBuffer;
            BufferEventLog = string.Empty;

            LogMessagesChecked = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandHandler ClearLogAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBuffer<string> DataBuffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BufferEventLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LogMessagesChecked {
            get
            {
                return logMessagesChecked;
            }
            set
            {
                // Unregister listeners
                try
                {
                    DataBuffer.ElemBuffered -= HandleElemBuffered;
                    DataBuffer.ElemRead     -= HandleElemRead;
                }
                catch { }

                // Re-add listeners if needed
                if (value == true)
                {
                    DataBuffer.ElemBuffered += HandleElemBuffered;
                    DataBuffer.ElemRead     += HandleElemRead;
                }

                logMessagesChecked = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ringBuffer"></param>
        public void ChangeBuffer(RingBuffer<string> ringBuffer)
        {
            try
            {
                DataBuffer.ElemBuffered -= HandleElemBuffered;
                DataBuffer.ElemRead     -= HandleElemRead;
            }
            catch { }

            DataBuffer = ringBuffer;
            DataBuffer.ElemBuffered += HandleElemBuffered;
            DataBuffer.ElemRead     += HandleElemRead;
            InvokePropertyChanged(nameof(DataBuffer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem"></param>
        private void HandleElemBuffered(string elem)
        {
            lock (bufferLock) { BufferEventLog += SplitDataToLines('+', elem); }
            InvokePropertyChanged(nameof(BufferEventLog));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem"></param>
        private void HandleElemRead(string elem)
        {
            lock (bufferLock) { BufferEventLog += SplitDataToLines('-', elem); }
            InvokePropertyChanged(nameof(BufferEventLog));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        private string SplitDataToLines(char label, string elem)
        {
            StringBuilder bldr = new StringBuilder();

            string[] parts = elem
                .Split('\r', '\n')
                .Where(elem => !string.IsNullOrEmpty(elem))
                .ToArray();

            // Shortcut for single line prints
            if (parts.Length == 1)
            {
                bldr.Append($"{label} ─{parts[0]}\r\n");
                return bldr.ToString();
            }
            
            // Multiple lines to print
            for (int i = 0; i < parts.Length; i++)
            {
                if      (i == 0)               bldr.Append($"{label} ┬{parts[i]}\r\n");
                else if (i < parts.Length - 1) bldr.Append(      $"  │{parts[i]}\r\n");
                else                           bldr.Append(      $"  └{parts[i]}\r\n");
            }
            bldr.Append("\r\n");
            return bldr.ToString();
        }

        /// <summary>
        /// Inform property watchers that a property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
