using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Connect4.Network
{
    public class User
    {
        public string IP //IP пользователя
        { get; set; }
        public int Port //Порт, через который подключен пользователь
        { get; set; }

        public User(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

    }
}
