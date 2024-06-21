using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WinSSHCopyId.Engine
{
    public class CLIEngine
    {
        private readonly StringBuilder _sb = new StringBuilder();

        private string _host;
        private string _username;
        private string _password;
        private string _publicKeyPath;
        private string _publicKey;

        public void Run(string[] args)
        {
            try
            {
                args = CleanUp(args);

                Log("=== WinSSHCopyId ===");
                Log($"Argument: {FlattedArgs(args)}");

                ArgumentParsing(args);

                if (string.IsNullOrWhiteSpace(_host)
                    || string.IsNullOrWhiteSpace(_username)
                    || string.IsNullOrWhiteSpace(_publicKeyPath))
                {
                    Log("Invalid Arguments!");
                    Log("[1] Usage: WinSSHCopyId.exe -i <PublicKeyPath: C:\\Users\\John\\.ssh\\id_rsa.pub> <username>@<host>");
                    Log("[2] Usage: WinSSHCopyId.exe -q -i <PublicKeyPath: C:\\Users\\John\\.ssh\\id_rsa.pub> <username>:<password>@<host>");
                    return;
                }

                _publicKey = File.ReadAllText(_publicKeyPath);

                if (!args.Contains("-q"))
                {
                    Console.Write($"{CurTimeString()} - Enter password: ");
                    _password = ReadPassword();
                }

                var sshCopyEngine = new SSHCopyIdEngine(_host, _username, _password, _publicKey);
                sshCopyEngine.LogEventHandler -= SshCopyEngine_LogEventHandler;
                sshCopyEngine.LogEventHandler += SshCopyEngine_LogEventHandler;
                sshCopyEngine.Copy();
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            finally
            {
                if (!args.Contains("-q"))
                {
                    Log("Press any key to continue ...");
                    Console.ReadKey();
                }
            }
        }

        private static string CurTimeString()
        {
            return DateTime.Now.ToString("[HH:mm:ss.fff]");
        }

        private void SshCopyEngine_LogEventHandler(string msg)
        {
            Log(msg);
        }

        private void Log(string msg)
        {
            _sb.Clear();
            _sb.Append(CurTimeString());
            _sb.Append(" - ");
            _sb.Append(msg.Trim());
            Console.WriteLine(_sb.ToString());
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

        private static string[] CleanUp(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim().Replace("\r\n", "");
            }
            return args;
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
    }
}