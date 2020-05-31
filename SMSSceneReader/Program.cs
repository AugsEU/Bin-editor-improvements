using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SMSReader;
using System.Globalization;

namespace SMSSceneReader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("", false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.ThreadException += new ThreadExceptionEventHandler(HandleException);
            Application.Run(new MainForm(args));
        }

        private static void HandleException(object sender, ThreadExceptionEventArgs t)
        {
            throw t.Exception;
        }
    }
}
