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

            // Force the CWD to be the directory in which the exe is located
            Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
            //Application.ThreadException += new ThreadExceptionEventHandler(HandleException);
            Application.Run(new MainForm(args));
        }

        private static void HandleException(object sender, ThreadExceptionEventArgs t)
        {
            throw t.Exception;
        }
    }
}
