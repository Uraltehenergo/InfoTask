using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;

namespace AuditMonitor
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>

        public static ClassParams Params = new ClassParams();
        //public static readonly ClassAdamNet Net = new ClassAdamNet();
        public static readonly NewNet Net = new NewNet();
        //public static readonly ClassArchive Archive = new ClassArchive();
        //public static readonly ClassAuditArchive Archive = new ClassAuditArchive();
        public static readonly NewAuditArchive Archive = new NewAuditArchive();
        public static FormMonitor FmMonitor;
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FmMonitor = new FormMonitor();
            Application.Run(FmMonitor);
        }
    }
}
