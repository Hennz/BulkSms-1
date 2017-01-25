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
            LoginForm form = (LoginForm)this.Owner;
            form.Show();
        }
    }
}
