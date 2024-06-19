using System.Runtime.InteropServices;

namespace WinSSHCopyId.Repository
{
    public static class WindowsAPI
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int FreeConsole();
    }
}