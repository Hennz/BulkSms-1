using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkSms;
using System.Windows.Forms;

namespace BulkSmsControlPanel  
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            test();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }

        public static void test()
        {
            try
            {


                



            }
            catch (Exception exp)
            {
                Utils.Log(exp);
            }
        }


    }
}
