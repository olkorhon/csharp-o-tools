using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using Ponsse.Olli.Buffering;

namespace Olli.Tools.CommieTester.Utility
{
    public class SerialPortWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public static Collection<int> ValidBaudRates { get; } = new ObservableCollection<int> {
            4800, 9600, 14400, 19200, 38400, 57600, 115200
        };

        /// <summary>
        /// 
        /// </summary>
        public static Collection<Handshake> ValidHandshakes { get; } = new ObservableCollection<Handshake> {
            Handshake.None, Handshake.RequestToSend, Handshake.RequestToSendXOnXOff, Handshake.XOnXOff
        };

        private Thread sendThread;
        private SerialPort? serialPort;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultComPort"></param>
        public SerialPortWrapper(string defaultComPort) : this(
            defaultComPort, new RingBuffer<string>(1024), new RingBuffer<string>(1024)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultComPort"></param>
        /// <param name="outputBuffer"></param>
        public SerialPortWrapper(string defaultComPort, RingBuffer<string> inputBuffer, RingBuffer<string> outputBuffer)
        {
            sendThread = new Thread(SendLoop);
            serialPort = null;

            ComPort       = defaultComPort;
            BaudRate      = ValidBaudRates[1];
            UsedHandshake = ValidHandshakes[0];
            
            InputBuffer  = inputBuffer;
            OutputBuffer = outputBuffer;

            FailedToSendMessage = string.Empty;
            sendThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ComPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Handshake UsedHandshake { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBuffer<string> InputBuffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RingBuffer<string> OutputBuffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string FailedToSendMessage { get; set; }

        /// <summary>
        /// Open serial port connection if serial port connection is closed
        /// </summary>
        public bool Open()
        {
            try
            {
                if (serialPort != null)
                    return false;

                serialPort = new SerialPort(ComPort, BaudRate, Parity.None, 8, StopBits.One);
                serialPort.Handshake = UsedHandshake;
                serialPort.DataReceived += ReceiveSerialData;
                serialPort.WriteTimeout = 1000;
                serialPort.Open();
                return true;
            }
            catch (Exception e)
            {
                string msg = $"{e.Message}\r\n\r\n{e.StackTrace}";
                MessageBox.Show("Failed to open serial connection", msg, MessageBoxButton.OK);
                return false;
            }
        }

        /// <summary>
        /// Close serial port connection if connection is open
        /// </summary>
        public void Close()
        {
            try
            {
                if (serialPort == null)
                    return;

                serialPort.DataReceived -= ReceiveSerialData;
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort = null;
            }
            catch (Exception e)
            {
                string msg = $"{e.Message}\r\n\r\n{e.StackTrace}";
                MessageBox.Show("Failed to close serial connection", msg, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendLoop()
        {
            while (true)
            {
                string msgToSend = string.IsNullOrEmpty(FailedToSendMessage)
                    ? InputBuffer.Read()
                    : FailedToSendMessage;

                if (string.IsNullOrEmpty(msgToSend))
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (!SendData(msgToSend))
                    Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private bool SendData(string data)
        {
            if (serialPort == null)
                return false;

            try
            {
                serialPort.WriteLine(data);
                FailedToSendMessage = string.Empty;
                return true;
            }
            catch (TimeoutException e)
            {
                FailedToSendMessage = data;
                return true;
            }
            catch (Exception e)
            {
                string msg = $"{e.Message}\r\n\r\n{e.StackTrace}";
                MessageBox.Show(msg, "Write error", MessageBoxButton.OK);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveSerialData(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null)
                return;

            try
            {
                string serialDataBlock = serialPort.ReadExisting();
                OutputBuffer.Buffer(serialDataBlock);
            }
            catch (OperationCanceledException oce)
            {
                Close();

                string msg = $"{oce.Message}\r\n\r\n{oce.StackTrace}";
                MessageBox.Show(msg, "Serial read cancelled", MessageBoxButton.OK);
            }
        }
    }
}
