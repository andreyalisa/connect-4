using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Connect4.Network
{
    public class Server
    {
        private Server()
        {
            mainListenerThread = new Thread(new ThreadStart(Receiver));
            mainListenerThread.Start();
        }

        private static Server instance;

        public static Server getInstance()
        {
            if (instance == null) { instance = new Server(); }
            return instance;
        }

        public void Abort()
        {
            listener.Stop();
            mainListenerThread.Abort();
            instance = null;
        }
        private Thread mainListenerThread;

        private TcpListener listener;

        private int port = 10000;

        User connectedUser;
        User server;

        public bool IsServerCreated()
        {
            if (server == null) return false;
            return true;
        }

        public bool IsUserConnected()
        {
            bool result = true;
            if (connectedUser == null) result =  false;
            return result;
        }

        public User getServerUser() { return server; }

        private void Receiver()
        {
            string _ip = "";
            bool serverturn = false;
            do
            {
                try
                {
                    var ips = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    _ip = ips.Last().ToString();
                    IPAddress ip = IPAddress.Parse(_ip);
                    //Начинаем прослушку
                    listener = new TcpListener(ip, port);
                    listener.Start();
                    serverturn = true;
                }
                catch
                {
                    port--; //Порт не свободен
                }
            } while (!serverturn);

            server = new User(_ip, port);
            string recstring = "";

            //и заведем заранее сокет
            Socket ReceiveSocket;
            while (true)
            {
                try  //поток может находиться в процессе прерывания, во время отключения 
                {
                    //Пришло сообщение
                    ReceiveSocket = listener.AcceptSocket();
                    Byte[] Receive = new Byte[256];
                    //Читать сообщение будем в поток
                    using (MemoryStream MessageR = new MemoryStream())
                    {
                        //Количество считанных байт
                        int ReceivedBytes;
                        do
                        {//Собственно читаем
                            ReceivedBytes = ReceiveSocket.Receive(Receive, Receive.Length, 0);
                            //и записываем в поток
                            MessageR.Write(Receive, 0, ReceivedBytes);
                            //Читаем до тех пор, пока в очереди не останется данных
                        } while (ReceiveSocket.Available > 0);

                        recstring = Encoding.Default.GetString(MessageR.ToArray());
                        string probst = recstring.Substring(0, 4);
                        switch (probst)
                        {
                            case "0001":
                                comm01_newUserConnected(recstring); //connects a new user
                                break;
                            case "0002":
                                comm02_makeMove(recstring);
                                    break; //user makes move
                            case "0003": break; //user disconnected
                        }

                    }
                }
                catch
                {

                }

            }
        }
        
        private void SendMessage(string ip, int port, string sendingtext)   //отправляется text на данный ip / port
        {
            try
            {
                //Создаем сокет, коннектимся
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                Socket Connector = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Connector.Connect(EndPoint);


                //Проверяем входной объект на соответствие строке                
                //Создаем сокет, коннектимся               
                //Отправляем сообщение

                Byte[] SendBytes = Encoding.Default.GetBytes(sendingtext);
                Connector.Send(SendBytes);

                Connector.Close();
                //Изменяем поле сообщений (уведомляем, что отправили сообщение)
                //  Console.WriteLine(MessageText);

            }
            catch
            {
                onCantConnect();
            }
        }

        public void Connect(string ip, string port)
        {
            try
            {
                int iport = int.Parse(port);
                if (iport == server.Port && ip.Equals(server.IP)) throw new Exception();  //connecting to myself
                Thread send = new Thread(() => SendMessage(ip, iport, "0001&" + server.IP + "&" + server.Port.ToString()));
                send.Start();                
                connectedUser = new User(ip, int.Parse(port));
                onConnected();
            }
            catch
            {
                onCantConnect();
            }
        }

        public void SendMove(int team, int column)
        {
            try {
                Thread send = new Thread(() => SendMessage(connectedUser.IP, connectedUser.Port, "0002&" + team + "&" + column));
                send.Start();
            }
            catch
            {
                onCantConnect();
            }
        }

        public Action onUserConnected;

        /// <summary>
        /// first argument is the team, second is the column
        /// </summary>
        public Action<int, int> onMakeMove;
        public Action onCantConnect;
        public Action onConnected;


        private void comm01_newUserConnected(string message)
        {
            try {
                var messages = message.Split('&');
                connectedUser = new User(messages[1], int.Parse(messages[2]));
                onUserConnected();
            }catch { }
        }

        private void comm02_makeMove(string message)
        {
            try
            {
                var messages = message.Split('&');
                onMakeMove(int.Parse(messages[1]), int.Parse(messages[2]));
            }
            catch { }
        }

    }
}
