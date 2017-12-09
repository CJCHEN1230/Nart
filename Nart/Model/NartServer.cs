using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Windows.Forms;

namespace Nart
{
    public class NartServer
    {
        /// <summary>
        /// client端的連線列表
        /// </summary>
        private List<Socket> _clientSockets;
        /// <summary>
        /// client端的buffer列表
        /// </summary>
        private List<byte[]> _bufferList;
        /// <summary>
        ///本地端的socket
        /// </summary>
        private Socket _serverSocket;
        /// <summary>
        ///開啟Server
        /// </summary>
        public NartServer()
        {
            try
            {
                _clientSockets = new List<Socket>();
                _bufferList = new List<byte[]>();
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 59979));//IPEndPoint為一個定義完整的server位置，包含ip跟port
                _serverSocket.Listen(10);//一個等待連線的queue長度，不是只能10個連線
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket); //AsyncCallback(AcceptCallback),一旦連接上後的回調函數為AcceptCallback。當系統調用這個函數時，自動賦予的輸入參數為IAsyncResult類型變量ar。
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
        /// <summary>
        ///accept的callback內容
        /// </summary>
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var clientSocket = _serverSocket.EndAccept(ar); //完成連接，並返回此時的socket通道
                _clientSockets.Add(clientSocket);

                byte[]  buffer = new byte[clientSocket.ReceiveBufferSize];
                _bufferList.Add(buffer);

                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                Console.WriteLine("收到一客戶端");
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }

            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
        /// <summary>
        ///receive的callback內容
        /// </summary>
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket current = (Socket)ar.AsyncState;
            CheckIfConnected(_clientSockets);
            try
            {
                MyCallback callback = new MyCallback(UpdataTb);
                int received = current.EndReceive(ar);// 結束非同步讀取，並回傳收到幾個Byte                                
                if (received == 0)
                {
                    return;
                }
                int index = _clientSockets.IndexOf(current);//取得此client是列表中的哪個
                int code = BitConverter.ToInt16(_bufferList[index], 0);

                //加車
                if (code == -70)
                {
                    Point point = new Point();
                    point.X = BitConverter.ToInt16(_bufferList[index], 2);
                    point.Y = BitConverter.ToInt16(_bufferList[index], 4);

                }
                Console.WriteLine("接收到東西");
                _bufferList[index] = new byte[current.ReceiveBufferSize];
                current.BeginReceive(_bufferList[index], 0, _bufferList[index].Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), current);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
                current.Close();
                _clientSockets.Remove(current);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
        /// <summary>
        ///send的callback內容
        /// </summary>
        private void SendCallback(IAsyncResult ar)
        {
            Socket current = (Socket)ar.AsyncState;
            try
            {
                current.EndSend(ar);//終止Send
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
        /// <summary>
        ///發送內容
        /// </summary>
        public void SendMessage()
        {
            List<Byte> byteMessageList = new List<byte>();
            byteMessageList.AddRange(BitConverter.GetBytes((short)-70));
     
            byte[] sendData = byteMessageList.ToArray();

            //byte[] byData = System.Text.Encoding.ASCII.GetBytes("un:" + username + ";pw:" + password);

            foreach (Socket eachSocket in _clientSockets)
            {            
               eachSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(SendCallback), eachSocket/*nullptr*/);                
            }
        }        
        /// <summary>
        ///檢查連線是否存在
        /// </summary>
        public void CheckIfConnected(List<Socket> clientSockets)
        {
            for (int i = 0; i < clientSockets.Count; i++)
            {  //不用foreach 是因為foreach是唯讀的
                if (clientSockets[i].Poll(1, SelectMode.SelectRead) && clientSockets[i].Available == 0)
                {
                    clientSockets.Remove(clientSockets[i]);
                }
            }

            foreach (Socket eachSocket in clientSockets)
            {
                if (eachSocket.Poll(100, SelectMode.SelectRead) || eachSocket.Available == 0)
                {
                    clientSockets.Remove(eachSocket);
                    eachSocket.Shutdown(SocketShutdown.Both);
                    eachSocket.Close();
                }
            }
        }
        public delegate void MyCallback(String message);
        public void UpdataTb(String message)
        {
            //ChatRoomTB->AppendText(message + Environment::NewLine);
            //KeyInTB.Clear();
            //ChatRoomTB.Text += message + Environment::NewLine;

        }
        private static void ShowErrorDialog(String message)
        {
            System.Windows.Forms.MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
