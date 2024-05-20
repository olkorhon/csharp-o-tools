using System;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ponsse.Olli.Buffering;

namespace Olli.Tools.CommieTester.Utility
{
    public class UdpWriter
    {
        internal delegate void MessageEvent(string msgData);
        internal event MessageEvent? messageSent;

        private Thread udpSenderThread;

        private IPAddress address;
        private IPEndPoint endpoint;
        private UdpClient? udpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sendBuffer"></param>
        public UdpWriter(int port, RingBuffer<string> sendBuffer)
        {
            address = new IPAddress(new byte[] { 127, 0, 0, 1 });
            endpoint = new IPEndPoint(address, port);
            udpClient = null;

            SendBuffer = sendBuffer;

            udpSenderThread = new Thread(UDPSenderLoop);
            udpSenderThread.IsBackground = true;
            udpSenderThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Port {
            get { return endpoint.Port; }
            set { endpoint.Port = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RingBuffer<string> SendBuffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOperational
        {
            get { return udpClient?.Client?.Connected == true; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Open(int port)
        {
            try
            {
                if (udpClient != null)
                    return;

                endpoint.Port = port;

                udpClient = new UdpClient();
                udpClient.Connect(endpoint);
            }
            catch (Exception e)
            {
                string msg = $"{e.Message}\r\n\r\n{e.StackTrace}";
                MessageBox.Show("Failed to open UDP connection", msg, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Close()
        {
            try
            {
                if (udpClient == null)
                    return;

                if (udpClient.Client?.Connected == true)
                    udpClient.Close();

                udpClient = null;
            }
            catch (Exception e)
            {
                string msg = $"{e.Message}\r\n\r\n{e.StackTrace}";
                MessageBox.Show("Failed to close UDP connection", msg, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void UDPSenderLoop()
        {
            while (true)
            {
                if (!IsOperational)
                {
                    await Task.Delay(500);
                    continue;
                }

                try   { UnloadSendBuffer(); }
                catch { Close(); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void UnloadSendBuffer()
        {
            string msgToSend = SendBuffer.Read();
            if (string.IsNullOrEmpty(msgToSend))
            {
                await Task.Delay(100);
                return;
            }

            if (udpClient?.Client?.Connected == true)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msgToSend);
                udpClient.Send(bytes);
                messageSent?.Invoke(msgToSend);
            }
            else
            {
                messageSent?.Invoke($"[Failed] {msgToSend}");
            }
        }
    }
}
