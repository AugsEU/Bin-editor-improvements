using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataReader;

namespace SMSManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-yaz0dec")
                {
                    string dest = args[1] + ".arc";
                    if (args.Length > 2)
                        dest = args[2];
                    GCN.Yaz0Dec(args[1], dest);
                    return;
                }
                if (args[0] == "-rarcdump" || args[0] == "-arcdump")
                {
                    string dest = args[1];
                    if (args.Length > 2)
                        dest = args[2];
                    Console.WriteLine("Extracting " + args[1] + " to " + dest);
                    GCN.ExtractArc(args[1], dest);
                    return;
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
