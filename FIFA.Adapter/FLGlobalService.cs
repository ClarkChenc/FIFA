using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FIFA.Framework.Analysis;

namespace FIFATestAdapter
{
    public class FLGlobalService
    {
        public static void SendMessage(string msg)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(IPAddress.Loopback, Constant.IPPort);
            } catch(Exception e)
            {
                return;
            }
            NetworkStream ns = client.GetStream();
            BinaryFormatter bformatter = new BinaryFormatter();

            bformatter.Serialize(ns, msg);

            ns.Flush();
            ns.Close();
            client.Close();
        }

        public static void SendRank(List<BasicBlock> list)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(IPAddress.Loopback, Constant.IPPort);
            }
            catch (Exception e)
            {
                return;
            }
            NetworkStream ns = client.GetStream();
            BinaryFormatter bformatter = new BinaryFormatter();

            bformatter.Serialize(ns, "list_head");
            bformatter.Serialize(ns, list.Count);
            foreach (var bb in list)
            {
                bformatter.Serialize(ns, bb);
            }
            ns.Flush();
            ns.Close();
            client.Close();
        }
    }
}
