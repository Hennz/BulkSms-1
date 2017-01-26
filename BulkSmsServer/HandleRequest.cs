using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BulkSms;

namespace BulkSmsServer
{
    class HandleRequest
    {
        private TcpClient client;
        public static Object sync_locking = new Object();

        public HandleRequest(TcpClient client)
        {
            this.client = client;
        }

        public void Handle()
        {
            try
            {
                NetworkStream ns = client.GetStream();
                int handshakeSignal = ns.ReadByte();
                if(handshakeSignal == -1)
                {
                    throw new Exception();
                }

                if(handshakeSignal == Constants.SIGNAL_NORMAL_UPDATE)
                {
                    try
                    {
                        ns.WriteByte((byte)Constants.SIGNAL_NORMAL_UPDATE);
                        ns.Close();
                        this.client.Close();
                    } catch(Exception) {
                        try
                        {
                            this.client.Close();
                        }
                        catch (Exception) { }
                    }

                    lock(HandleRequest.sync_locking)
                    {
                        Utils.SendBulkSmsToClients();
                        Thread.Sleep(10000);
                    }
                } else if(handshakeSignal == Constants.SIGNAL_SYNC_UPDATE)
                {
                    int res = 0;
                    lock (HandleRequest.sync_locking)
                    {
                       res = Utils.SendBulkSmsToClients();
                    }
                    try
                    {
                        if(res == 1)
                        {
                            ns.WriteByte((byte)Constants.SIGNAL_SUCCESS_UPDATE);
                        } else
                        {
                            ns.WriteByte((byte)Constants.SIGNAL_FAILURE_UPDATE);
                        }
                        try
                        {
                            ns.ReadByte();
                        } catch(Exception) { }
                        ns.Close();
                        this.client.Close();
                    }
                    catch (Exception) {
                        try
                        {
                            this.client.Close();
                        }
                        catch (Exception) { }
                    }
                } else
                {
                    ns.Close();
                    this.client.Close();
                }

                try
                {
                    ns.Close();
                    this.client.Close();
                }
                catch (Exception) { }

            } catch(Exception exp)
            {
                try
                {
                    this.client.Close();
                } catch(Exception){}
                
            }
        }
    }
}
