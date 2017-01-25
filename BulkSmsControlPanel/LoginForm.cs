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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception exp)
            {
                Utils.Log(exp);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == null || String.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("Please enter username", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox2.Text == null || String.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Please enter password", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            String username = textBox1.Text.Trim();
            String password = textBox2.Text.Trim();
            
            if(!(username.Equals(Constants.LOGIN_USERNAME) && password.Equals(Constants.LOGIN_PASSWORD)))
            {
                MessageBox.Show("Invalid authentication", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ControlPanelForm form = new ControlPanelForm();
            form.Owner = this;
            form.Show();
            this.Hide();
        }
    }
}
