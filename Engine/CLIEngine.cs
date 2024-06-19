using System;
using System.IO;
using System.Text;

namespace WinSSHCopyId.Engine
{
    public class CLIEngine
    {
        private StringBuilder _sb = new StringBuilder();

        public int Run(string[] args)
        {
            string host = null;
            string username = null;
            string password = null;
            string publicKeyPath = null;
            string publicKey = null;

            try
            {
                // Argument parsing
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-i" && i + 1 < args.Length)
                    {
                        publicKeyPath = args[++i];
                    }
                    else if (args[i].Contains("@"))
                    {
                        var parts = args[i].Split('@');
                        if (parts.Length != 2)
                        {
                            continue;
                        }

                        username = parts[0];
                        host = parts[1];
                    }
                }

                if (string.IsNullOrWhiteSpace(host)
                    || string.IsNullOrWhiteSpace(username)
                    || string.IsNullOrWhiteSpace(publicKeyPath))
                {
                    Console.WriteLine("Usage: WinSSHCopyId.exe -i <PublicKeyPath: C:\\Users\\Peter\\.ssh\\id_rsa.pub> <username>@<host>");
                    return 0;
                }

                publicKey = File.ReadAllText(publicKeyPath);

                Console.Write("Enter password: ");
                password = ReadPassword();

                var sshCopyEngine = new SSHCopyEngine(host, username, password, publicKey);
                sshCopyEngine.LogEventHandler -= SshCopyEngine_LogEventHandler;
                sshCopyEngine.LogEventHandler += SshCopyEngine_LogEventHandler;
                var err = sshCopyEngine.Copy();
                if (err != null)
                {
                    throw err;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

            return 1;
        }

        private void SshCopyEngine_LogEventHandler(string msg)
        {
            Console.WriteLine(msg);
        }

        #region "Method"

        private string ReadPassword()
        {
            _sb.Clear();
            while (true)
            {
                var info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (info.Key == ConsoleKey.Backspace)
                {
                    if (_sb.Length <= 0)
                    {
                        continue;
                    }
                    _sb.Remove(_sb.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else
                {
                    _sb.Append(info.KeyChar);
                    Console.Write("*");
                }
            }
            return _sb.ToString();
        }

        #endregion "Method"
    }
}