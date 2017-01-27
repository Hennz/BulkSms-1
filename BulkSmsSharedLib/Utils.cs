using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Web;

namespace BulkSms
{
    public class Utils
    {

        public static void Log(Object obj)
        {
            String logs = (obj == null) ? ("null") : (obj.ToString());
            MessageBox.Show(logs);
        }

        public static String dateTimeToStr(DateTime dt)
        {
            return String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
        }

        public static int SendBulkSmsToClients()
        {
            int outs = 0;
            try
            {
                OleDbConnection ole_db_conn = null;
                OleDbCommand ole_db_comm = null;
                OleDbDataReader ole_db_reader = null;

                ole_db_conn = new OleDbConnection(String.Format(@"Provider=VFPOLEDB.1; Data Source={0};", Constants.DATA_PATH));
                ole_db_conn.Open();
                ole_db_comm = ole_db_conn.CreateCommand();
                ole_db_comm.CommandText = String.Format("select * from {0} where STATUS='' or STATUS='0' or STATUS=' ' ", Constants.DBF_FILE_NAME);
                ole_db_reader = ole_db_comm.ExecuteReader();
                if (!ole_db_reader.HasRows)
                {
                    try
                    {
                        ole_db_reader.Close();
                        ole_db_conn.Close();
                    }
                    catch (Exception) { }
                    return 1;
                }


                List<ulong> rowIds = new List<ulong>();
                List<String> intlNumbersWithCode = new List<string>();
                List<string> messages = new List<string>();
                while (ole_db_reader.Read())
                {
                    String strNumber = Convert.ToUInt64(ole_db_reader.GetDecimal(ole_db_reader.GetOrdinal("MOBILE"))).ToString();
                    if(strNumber.Length <= 10)
                    {
                        strNumber = "91" + strNumber;
                    }
                    intlNumbersWithCode.Add(strNumber);
                    messages.Add(ole_db_reader.GetString(ole_db_reader.GetOrdinal("SMS")));
                    rowIds.Add(Convert.ToUInt64(ole_db_reader.GetDecimal(ole_db_reader.GetOrdinal("ID"))));
                }
                ole_db_reader.Close();

                String requestID = Utils.SendBulkSms(intlNumbersWithCode, messages);
                //String requestID = "";
                if (requestID == null)
                {
                    throw new Exception("Could not send due to network error");
                }

                foreach(ulong id in rowIds)
                {
                    try
                    {
                        ole_db_comm = ole_db_conn.CreateCommand();
                        ole_db_comm.CommandText = String.Format("update {0} set STATUS='1' where ID={1}", Constants.DBF_FILE_NAME, id);
                        ole_db_comm.ExecuteNonQuery();
                    } catch(Exception exp) { }
                }
                try
                {
                    ole_db_conn.Close();
                } catch(Exception exp) { }

                outs = 1;

            } catch(Exception exp)
            {
                outs = 0;
                Utils.Log(exp);
            }
            return outs;
        }

        public static String SendBulkSms(List<String> intlNumbersWithCode, List<String> messages)
        {
            String outs = null;
            try
            {

                if(intlNumbersWithCode.Count <= 0 || messages.Count < intlNumbersWithCode.Count)
                {
                    throw new Exception("Insufficeint arguments");
                }

                XElement messageElem = new XElement("MESSAGE");

                messageElem.Add(new XElement("AUTHKEY", Constants.BULK_SMS_AUTH_KEY.ToString()));
                messageElem.Add(new XElement("SENDER", Constants.SENDER_ID.ToString()));
                messageElem.Add(new XElement("ROUTE", Constants.BULK_SMS_ROUTE.ToString()));
                messageElem.Add(new XElement("CAMPAIGN", Constants.BULK_SMS_CAMPAIGN.ToString()));
                messageElem.Add(new XElement("COUNTRY", Constants.BULK_SMS_COUNTRY.ToString()));

                for(int i = 0; i<intlNumbersWithCode.Count; i++)
                {
                    messageElem.Add(new XElement("SMS", new XAttribute("TEXT", messages[i].ToString()), new XElement("ADDRESS", new XAttribute("TO", intlNumbersWithCode[i].ToString()))));
                }


                String data = messageElem.ToString();
                String dataWillBeSent = String.Format("data={0}", HttpUtility.UrlEncode(data));

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(Constants.BULK_SMS_SERVER_URL_TO_SEND_MESSAGE);
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytesData = encoding.GetBytes(dataWillBeSent.ToString());
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = bytesData.Length;
                Stream stream = httpWReq.GetRequestStream();
                stream.Write(bytesData, 0, bytesData.Length);
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String requestID = reader.ReadToEnd();
                try
                {
                    reader.Close();
                    response.Close();
                } catch(Exception) { }

                outs = requestID;

            } catch(Exception exp)
            {
                outs = null;
                Utils.Log(exp);
            }
            return outs;
        }

    }
}
