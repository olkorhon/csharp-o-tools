using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using Ponsse.Olli.Buffering;

namespace Olli.Tools.CommieTester.Utility
{
    public class UdpReader
    {
        internal delegate void MessageEvent(string msgData);
        internal event MessageEvent? messageReceived;

        private IPAddress address;
        private IPEndPoint endpoint;
        private UdpClient? udpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="receiveBuffer"></param>
        public UdpReader(int port, RingBuffer<string> receiveBuffer)
        {
            address = new IPAddress(new byte[] { 127, 0, 0, 1 });
            endpoint = new IPEndPoint(address, port);
            udpClient = null;

            ReceiveBuffer = receiveBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        public RingBuffer<string> ReceiveBuffer { get; set; }

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
                udpClient = new UdpClient(endpoint);
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
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
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);

            if (udpClient == null)
                return;

            // Complete the receive operation
            byte[] receiveBytes = udpClient.EndReceive(ar, ref e);
            
            string msg = Encoding.UTF8.GetString(receiveBytes);
            ReceiveBuffer.Buffer(msg);
            messageReceived?.Invoke(msg);

            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
        }
    }
}
