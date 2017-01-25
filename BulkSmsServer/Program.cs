using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using BulkSms;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BulkSmsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread th = new Thread(new ThreadStart(() => {
                try
                {
                    TcpListener listener = new TcpListener(IPAddress.Any, Convert.ToInt32(Constants.SERVER_PORT));
                    try
                    {
                        listener.Start();
                        MessageBox.Show("BulkSMS Server is running on port: " + Convert.ToInt32(Constants.SERVER_PORT), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        try
                        {
                            while (true)
                            {
                                TcpClient client = listener.AcceptTcpClient();
                                HandleRequest hr = new HandleRequest(client);
                                new Thread(new ThreadStart(() => {
                                    hr.Handle();
                                })).Start();
                            }
                        } catch(Exception)
                        {
                            MessageBox.Show("BulkSMS Server is stopped, please try to restart", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show(String.Format("Probably BulkSMS Server is already running or the port {0} is not available", Convert.ToInt32(Constants.SERVER_PORT)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                } catch(Exception exp)
                {
                    Utils.Log(exp);
                }
            }));
            th.Start();

        }
    }
}
