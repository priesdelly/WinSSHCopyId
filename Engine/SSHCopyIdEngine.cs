using System;
using System.Text;
using Renci.SshNet;

namespace WinSSHCopyId.Engine
{
    public class SSHCopyIdEngine
    {
        public delegate void RaiseLogEvent(string message);

        public event RaiseLogEvent LogEventHandler;

        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PublicKey { get; set; }

        private readonly StringBuilder _sb = new StringBuilder();

        public SSHCopyIdEngine()
        {
        }

        public SSHCopyIdEngine(string host, string username, string password, string publicKey)
        {
            Host = host;
            Username = username;
            Password = password;
            PublicKey = publicKey;
        }

        public Exception Copy()
        {
            using (var client = new SshClient(Host, Username, Password))
            {
                try
                {
                    client.Connect();
                    Log("Successfully connected to the server.");

                    var cmd = client.RunCommand("echo 'A connection was successfully established with the server'");
                    Log(cmd.Result);

                    client.RunCommand("mkdir -p ~/.ssh");
                    var checkCommand = $"grep -q \"{PublicKey}\" ~/.ssh/authorized_keys";
                    var checkResult = client.RunCommand(checkCommand);

                    if (checkResult.ExitStatus == 0)
                    {
                        Log("Public key already exists in authorized_keys.");
                        return null;
                    }

                    var appendCommand = $"echo \"{PublicKey}\" >> ~/.ssh/authorized_keys";
                    var appendResult = client.RunCommand(appendCommand);

                    if (appendResult.ExitStatus == 0)
                    {
                        Log("Public key successfully added to authorized_keys.");
                        return null;
                    }

                    Log($"Failed to add public key to authorized_keys: {appendResult.Error}");
                }
                catch (Exception ex)
                {
                    return ex;
                }
                finally
                {
                    client.Disconnect();
                }

                return null;
            }
        }

        private void Log(string msg)
        {
            if (LogEventHandler == null)
            {
                return;
            }

            _sb.Clear();
            _sb.Append(DateTime.Now.ToString("[HH:mm:ss.fff]"));
            _sb.Append(" - ");
            _sb.Append(msg.Trim());
            LogEventHandler(_sb.ToString());
        }
    }
}