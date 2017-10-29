using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalProcessing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //var walsh = SignalForm.BuildWalshMatrix(8);
            //StringBuilder walshMatrix = new StringBuilder();
            //for (int i = 0; i < walsh.Length; i++)
            //{
            //    walshMatrix.Append(string.Join(",", walsh[i]));
            //    walshMatrix.Append("\n");
            //}
            //Debug.Write(walshMatrix.ToString());


            Application.Run(new SignalForm());
        }
    }
}
