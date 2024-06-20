using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WinSSHCopyId.Engine
{
    public class CLIEngine
    {
        private readonly StringBuilder _sb = new StringBuilder();

        private string _host = null;
        private string _username = null;
        private string _password = null;
        private string _publicKeyPath = null;
        private string _publicKey = null;

        public int Run(string[] args)
        {
            try
            {
                Console.WriteLine("=== WinSSHCopyId ===");
                Console.WriteLine("Argument: {0}", FlattedArgs(args));

                ArgumentParsing(args);

                if (string.IsNullOrWhiteSpace(_host)
                    || string.IsNullOrWhiteSpace(_username)
                    || string.IsNullOrWhiteSpace(_publicKeyPath))
                {
                    Console.WriteLine("Invalid Arguments!");
                    Console.WriteLine("[1] Usage: WinSSHCopyId.exe -i <PublicKeyPath: C:\\Users\\John\\.ssh\\id_rsa.pub> <username>@<host>");
                    Console.WriteLine("[2] Usage: WinSSHCopyId.exe -q -i <PublicKeyPath: C:\\Users\\John\\.ssh\\id_rsa.pub> <username>:<password>@<host>");
                    return 0;
                }

                _publicKey = File.ReadAllText(_publicKeyPath);

                if (!args.Contains("-q"))
                {
                    Console.Write("Enter password: ");
                    _password = ReadPassword();
                }

                var sshCopyEngine = new SSHCopyIdEngine(_host, _username, _password, _publicKey);
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
            finally
            {
                if (!args.Contains("-q"))
                {
                    Console.WriteLine("Press any key to continue ...");
                    Console.ReadKey();
                }
            }

            return 1;
        }

        private string FlattedArgs(string[] args)
        {
            _sb.Clear();
            foreach (var item in args)
            {
                _sb.AppendFormat("{0} ", item);
            }

            return _sb.ToString();
        }

        private void ArgumentParsing(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-i" && i + 1 < args.Length)
                {
                    _publicKeyPath = args[++i];
                }
                else if (args[i].Contains("@"))
                {
                    var parts = args[i].Split('@');
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    if (parts[0].Contains(":"))
                    {
                        var authPath = parts[0].Split(':');
                        _password = authPath[1];
                        _username = authPath[0];
                    }
                    else
                    {
                        _username = parts[0];
                    }

                    _host = parts[1];
                }
            }
        }

        private void SshCopyEngine_LogEventHandler(string msg)
        {
            Console.WriteLine(msg);
        }

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
    }
}