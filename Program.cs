using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinSSHCopyId.Engine;

namespace WinSSHCopyId
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                new CLIEngine().Run(args);
                return;
            }

            FreeConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}