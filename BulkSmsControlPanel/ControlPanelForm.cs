using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BulkSms;
using System.Security.Permissions;
using System.Data.OleDb;
using System.Net;
using System.Net.Sockets;

namespace BulkSmsControlPanel 
{
    public partial class ControlPanelForm : Form
    {
        private int MIN_ROWS_IN_DATAGRIDVIEW = 30;
        private List<Thread> workerThreads = new List<Thread>();

        public ControlPanelForm()
        {
            InitializeComponent();
        }

        private void ControlPanelForm_Load(object sender, EventArgs e)
        {
            try
            {

                button5.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;

                this.panel1.Location = new Point((this.ClientSize.Width - panel1.Width) / 2, (this.ClientSize.Height - panel1.Height) / 2);

                this.panel2.Width = this.panel1.Width;
                this.panel2.Height = this.panel1.Height - this.panel3.Height;
                this.panel2.Location = new Point(0, 0);

                this.dataGridView1.Width = this.panel2.Width;
                this.dataGridView1.Height = this.panel2.Height;
                this.dataGridView1.Location = new Point(0, 0);

                this.panel3.Location = new Point((this.panel1.Width - this.panel3.Width) / 2, this.panel1.Height - this.panel3.Height);

                this.label3.Location = new Point(this.panel1.Location.X, this.panel1.Location.Y - this.label3.Height - 3);

                button3.PerformClick();

            }
            catch (Exception exp)
            {
                Utils.Log(exp);
            }
        }

        public static Dictionary<String, int> GetDataGridViewColumnsWidthLocalReport()
        {
            Dictionary<String, int> outs = new Dictionary<String, int>();
            outs["ID"] = 200;
            outs["Mobile Number"] =200;
            outs["Message"] = 200;
            outs["Status"] = 200;
            return outs;
        }

        public static Dictionary<String, int> GetDataGridViewColumnsWidthOnlineReport()
        {
            Dictionary<String, int> outs = new Dictionary<String, int>();
            outs["Mobile Number"] = 200;
            outs["Date"] = 200;
            outs["Status"] = 200;
            outs["Request ID"] = 200;
            return outs;
        }

        public void InitializeDataGridviewLocalReport()
        {
            try
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                Dictionary<String, int> all_fields = GetDataGridViewColumnsWidthLocalReport();
                dataGridView1.ColumnCount = all_fields.Count;

                for (int i = 0; i < all_fields.Keys.Count; i++)
                {
                    dataGridView1.Columns[i].Name = all_fields.Keys.ElementAt(i);
                    dataGridView1.Columns[i].Width = (dataGridView1.Width - SystemInformation.VerticalScrollBarWidth - 1) / all_fields.Count;
                    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                setupGridView();

            }
            catch (Exception exp)
            {
                Utils.Log(exp);
            }
        }

        public void InitializeDataGridviewOnlineReport()
        {
            try
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                Dictionary<String, int> all_fields = GetDataGridViewColumnsWidthOnlineReport();
                dataGridView1.ColumnCount = all_fields.Count;

                for (int i = 0; i < all_fields.Keys.Count; i++)
                {
                    dataGridView1.Columns[i].Name = all_fields.Keys.ElementAt(i);
                    dataGridView1.Columns[i].Width = (dataGridView1.Width - SystemInformation.VerticalScrollBarWidth - 1) / all_fields.Count;
                    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                setupGridView();

            }
            catch (Exception exp)
            {
                Utils.Log(exp);
            }
        }

        private void setupGridView()
        {
            try
            {
                if (dataGridView1.RowCount < MIN_ROWS_IN_DATAGRIDVIEW)
                {
                    dataGridView1.Rows.Add(MIN_ROWS_IN_DATAGRIDVIEW - dataGridView1.RowCount);
                }

                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (i % 2 == 0)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(227, 255, 246);
                    }
                    else
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                    }
                }
            } catch(Exception exp)
            {
                Utils.Log(exp);
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ControlPanelForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult r = MessageBox.Show("Are you sure to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(r == DialogResult.No || r == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if(this.Owner != null)
            {
                LoginForm form = (LoginForm)this.Owner;
                form.textBox2.Text = "";
                form.Show();
            }
            killAllThreads();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                label3.Text = "Local Report";
                InitializeDataGridviewLocalReport();
                button5.Enabled = false;
                button3.Enabled = false;
                button2.Enabled = false;
                button3.Text = "Loading....";

                Thread th = new Thread(new ThreadStart(() => {
                    try
                    {
                        OleDbConnection ole_db_conn = null;
                        OleDbCommand ole_db_comm = null;
                        OleDbDataReader ole_db_reader = null;

                        try
                        {
                            ole_db_conn = new OleDbConnection(String.Format(@"Provider=VFPOLEDB.1; Data Source={0};", Constants.DATA_PATH));
                            ole_db_conn.Open();
                            ole_db_comm = ole_db_conn.CreateCommand();
                            ole_db_comm.CommandText = String.Format("select * from {0}", Constants.DBF_FILE_NAME);
                            ole_db_reader = ole_db_comm.ExecuteReader();
                            if (!ole_db_reader.HasRows)
                            {
                                try
                                {
                                    if (ole_db_reader != null)
                                        ole_db_reader.Close();
                                    if (ole_db_conn != null)
                                        ole_db_conn.Close();
                                }
                                catch (Exception) { }
                                
                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    MessageBox.Show("No local report found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    button5.Enabled = false;
                                    button3.Enabled = true;
                                    button2.Enabled = true;
                                    button3.Text = "Local Report";
                                }));
                                return;
                            }

                            int rIndex = 0;
                            while(ole_db_reader.Read())
                            {
                               try
                                {
                                    List<String> row_data = new List<string>();
                                    row_data.Add(Convert.ToUInt64(ole_db_reader.GetDecimal(ole_db_reader.GetOrdinal("ID"))).ToString());
                                    row_data.Add(Convert.ToUInt64(ole_db_reader.GetDecimal(ole_db_reader.GetOrdinal("MOBILE"))).ToString());
                                    row_data.Add(ole_db_reader.GetString(ole_db_reader.GetOrdinal("SMS")));
                                    String status = ole_db_reader.GetString(ole_db_reader.GetOrdinal("STATUS")).Trim();
                                    if (status.Equals("1"))
                                    {
                                        row_data.Add("Sent");
                                    }
                                    else
                                    {
                                        row_data.Add("Not sent");
                                    }
                                    BeginInvoke(new MethodInvoker(() =>
                                    {
                                        dataGridView1.Rows.Insert(rIndex, row_data.ToArray());
                                        setupGridView();
                                        rIndex++;
                                    }));
                                } catch(Exception) { }
                            }

                            try
                            {
                                if (ole_db_reader != null)
                                    ole_db_reader.Close();
                                if (ole_db_conn != null)
                                    ole_db_conn.Close();
                            }
                            catch (Exception) { }
                            
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                button5.Enabled = true;
                                button3.Enabled = true;
                                button2.Enabled = true;
                                button3.Text = "Local Report";
                            }));

                        }
                        catch(ThreadAbortException exp)
                        {
                            try
                            {
                                if (ole_db_reader != null)
                                    ole_db_reader.Close();
                                if (ole_db_conn != null)
                                    ole_db_conn.Close();
                            }
                            catch (Exception) { }
                            throw new Exception();
                        }
                        catch (ThreadInterruptedException exp)
                        {
                            try
                            {
                                if (ole_db_reader != null)
                                    ole_db_reader.Close();
                                if (ole_db_conn != null)
                                    ole_db_conn.Close();
                            }
                            catch (Exception) { }
                            throw new Exception();
                        }
                        catch (Exception exp)
                        {
                            try
                            {
                                if (ole_db_reader != null)
                                    ole_db_reader.Close();
                                if (ole_db_conn != null)
                                    ole_db_conn.Close();
                            }
                            catch (Exception) { }
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                MessageBox.Show("Local Report could not be loaded", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                button5.Enabled = false;
                                button3.Enabled = true;
                                button2.Enabled = true;
                                button3.Text = "Local Report";
                            }));
                            Utils.Log(exp);
                        }


                    } catch(Exception exp)
                    {
                        //Nothing
                    }
                }));
                workerThreads.Add(th);
                th.Start();

            } catch(Exception exp)
            {    
                Utils.Log(exp);
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        private void killAllThreads()
        {
            foreach (Thread th in workerThreads)
            {
                try
                {
                    th.Abort();
                }
                catch (Exception exp)
                {
                    Utils.Log(exp);
                }
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {

                button5.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button5.Text = "Resending...";

                Thread th = new Thread(new ThreadStart(() => {
                    try
                    {
                        TcpClient client = null;
                        NetworkStream ns = null;
                        try
                        {

                            client = new TcpClient();
                            try
                            {
                                client.Connect(IPAddress.Parse(Constants.SERVER_IP), Convert.ToInt32(Constants.SERVER_PORT));
                            }
                            catch (Exception exp)
                            {
                                try
                                {
                                    if (ns != null)
                                        ns.Close();
                                    if (client != null)
                                        client.Close();
                                }
                                catch (Exception) { }
                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    MessageBox.Show(String.Format("BulkSMS Server is not running at port {0}, please srart", Convert.ToInt32(Constants.SERVER_PORT)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    button5.Enabled = true;
                                    button3.Enabled = true;
                                    button2.Enabled = true;
                                    button5.Text = "Resend";
                                }));
                                return;
                            }
                            ns = client.GetStream();
                            ns.WriteByte((byte)Constants.SIGNAL_SYNC_UPDATE);
                            int status = ns.ReadByte();
                            if(status == -1)
                            {
                                throw new Exception();
                            }
                            try
                            {
                                ns.WriteByte((byte)status);
                                ns.Close();
                                client.Close();
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    client.Close();
                                }
                                catch (Exception) { }
                            }


                            try
                            {
                                ns.Close();
                                client.Close();
                            }
                            catch (Exception) { }


                            if (status == Constants.SIGNAL_SUCCESS_UPDATE)
                            {
                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    MessageBox.Show("SMS is sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    button5.Enabled = true;
                                    button3.Enabled = true;
                                    button2.Enabled = true;
                                    button5.Text = "Resend";
                                    button3.PerformClick();
                                }));
                            } else
                            {
                                throw new Exception();
                            }

                        }
                        catch (ThreadAbortException exp)
                        {
                            try
                            {
                                if (ns != null)
                                    ns.Close();
                                if (client != null)
                                    client.Close();
                            }
                            catch (Exception) { }
                            throw new Exception();
                        }
                        catch (ThreadInterruptedException exp)
                        {
                            try
                            {
                                if (ns != null)
                                    ns.Close();
                                if (client != null)
                                    client.Close();
                            }
                            catch (Exception) { }
                            throw new Exception();
                        }
                        catch (Exception exp)
                        {
                            try
                            {
                                if (ns != null)
                                    ns.Close();
                                if (client != null)
                                    client.Close();
                            }
                            catch (Exception) { }
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                MessageBox.Show("SMS could not be resent", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                button5.Enabled = true;
                                button3.Enabled = true;
                                button2.Enabled = true;
                                button5.Text = "Resend";
                            }));
                            Utils.Log(exp);
                        }
                       
                    } catch(Exception exp)
                    {
                       //Nothing
                    }
                }));
                workerThreads.Add(th);
                th.Start();

             } catch(Exception exp)
            {
                Utils.Log(exp);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
