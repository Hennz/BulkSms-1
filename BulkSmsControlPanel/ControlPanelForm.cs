using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BulkSms;

namespace BulkSmsControlPanel 
{
    public partial class ControlPanelForm : Form
    {
        private int MIN_ROWS_IN_DATAGRIDVIEW = 30;

        public ControlPanelForm()
        {
            InitializeComponent();
        }

        private void ControlPanelForm_Load(object sender, EventArgs e)
        {
            try
            {

                this.panel1.Location = new Point((this.ClientSize.Width - panel1.Width) / 2, (this.ClientSize.Height - panel1.Height) / 2);

                this.panel2.Width = this.panel1.Width;
                this.panel2.Height = this.panel1.Height - this.panel3.Height;
                this.panel2.Location = new Point(0, 0);

                this.dataGridView1.Width = this.panel2.Width;
                this.dataGridView1.Height = this.panel2.Height;
                this.dataGridView1.Location = new Point(0, 0);

                this.panel3.Location = new Point((this.panel1.Width - this.panel3.Width) / 2, this.panel1.Height - this.panel3.Height);


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
            outs["Time"] = 200;
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
                    dataGridView1.Columns[i].Width = all_fields[all_fields.Keys.ElementAt(i)];
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
                    dataGridView1.Columns[i].Width = all_fields[all_fields.Keys.ElementAt(i)];
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
