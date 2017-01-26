using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkSms;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace BulkSmsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TcpClient client = null;
            NetworkStream ns = null;
            try
            {

                client = new TcpClient();
                client.Connect(IPAddress.Parse(Constants.SERVER_IP), Convert.ToInt32(Constants.SERVER_PORT));
                ns = client.GetStream();
                ns.WriteByte((byte)Constants.SIGNAL_NORMAL_UPDATE);
                ns.ReadByte();

                try
                {
                    ns.Close();
                    client.Close();
                }
                catch (Exception) { }
            }  catch(Exception exp)
            {
                try
                {
                    if (ns != null)
                        ns.Close();
                    if (client != null)
                        client.Close();
                }
                catch (Exception) { }
                Utils.Log(exp);
            }
        }
    }
}
