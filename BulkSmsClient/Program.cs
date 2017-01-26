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
                try
                {
                    client.Connect(IPAddress.Parse(Constants.SERVER_IP), Convert.ToInt32(Constants.SERVER_PORT));
                } catch(Exception exp)
                {
                    MessageBox.Show(String.Format("BulkSMS Server is not running at port {0}, please srart", Convert.ToInt32(Constants.SERVER_PORT)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception();
                }
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
