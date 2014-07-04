using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Collections.ObjectModel;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Controls;

namespace FLTestAdapter
{
    class DataCollectionListener<T>
    {
        TcpListener tcp_listener;
        Action<IEnumerable<T>> finish_callback;
        Action<String> message_callback;
        bool cmd_stop;
        public DataCollectionListener(int port, Action<IEnumerable<T>> finish_callback, Action<String> message_callback)
        {
            tcp_listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            this.finish_callback = finish_callback;
            this.message_callback = message_callback;
        }

        public void Start()
        {
            tcp_listener.Start();
            cmd_stop = false;
            ThreadStart ts = new ThreadStart(worker);
            Thread tr = new Thread(ts);
            tr.Start();
        }

        void worker()
        {
            while (!cmd_stop)
            {
                using (Socket socket = tcp_listener.AcceptSocket())
                {
                    NetworkStream ns = new NetworkStream(socket);
                    BinaryFormatter bf = new BinaryFormatter();
                    string msg = (string)bf.Deserialize(ns);
                    if(msg == "list_head")
                    {
                        int count = (int)bf.Deserialize(ns);
                        T[] array = new T[count];
                        for (int i = 0; i < count; i++)
                        {
                            array[i] = (T)bf.Deserialize(ns);
                        }
                        finish_callback(array);

                    }
                    else if (msg == "clear")
                    {
                        finish_callback(new T[0]);
                    } else
                    {
                        message_callback(msg);
                    }
                    ns.Close();
                    socket.Disconnect(true);
                   
                }
                
            }

        }

        void Stop()
        {
            cmd_stop = true;
        }
    }
}
