using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkSms
{
    class Test
    {
        public void op()
        {
            Utils.SendBulkSmsToClients();

            /*try
            {
                OleDbConnection ole_db_conn = null;////hhkh
                OleDbCommand ole_db_comm = null;
                OleDbDataReader ole_db_reader = null;
                //
                ole_db_conn = new OleDbConnection(String.Format(@"Provider=VFPOLEDB.1; Data Source={0};", Constants.DATA_PATH));
                ole_db_conn.Open();


                for(int i=1000; i<10000; i++)
                {
                    ole_db_comm = ole_db_conn.CreateCommand();
                    ole_db_comm.CommandText = String.Format("insert into {0} (ID, MOBILE, SMS, STATUS) values ({1}, 8001329415, 'test', '')", Constants.DBF_FILE_NAME, i);
                    ole_db_comm.ExecuteNonQuery();
                }

                ole_db_conn.Close();

            } catch(Exception exp)
            {
                Utils.Log(exp);
            }*/
        }
    }
}
