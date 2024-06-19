using System;
using System.Windows.Forms;
using WinSSHCopyId.Engine;
using WinSSHCopyId.Repository;

namespace WinSSHCopyId
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                return new CLIEngine().Run(args);
            }

            WindowsAPI.FreeConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            return 1;
        }
    }
}