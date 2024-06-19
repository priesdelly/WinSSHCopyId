using Renci.SshNet;
using System;
using System.Text;

namespace WinSSHCopyId.Engine
{
    public class SSHCopyEngine
    {
        public delegate void RaiseLogEvent(string msg);

        public event RaiseLogEvent LogEventHandler;

        private string Host { get; }
        private string Username { get; }
        private string Password { get; }
        private string PublicKey { get; }

        private readonly StringBuilder _sb = new StringBuilder();

        public SSHCopyEngine(string host, string username, string password, string publicKey)
        {
            Host = host;
            Username = username;
            Password = password;
            PublicKey = publicKey;
        }

        public Exception Copy()
        {
            try
            {
                using (var client = new SshClient(Host, Username, Password))
                {
                    client.Connect();
                    Log("Successfully connected to the server.");

                    var cmd = client.RunCommand("echo 'A connection was successfully established with the server' ");
                    Log(cmd.Result);

                    client.RunCommand("mkdir -p ~/.ssh");
                    var checkCommand = $"grep -q \"{PublicKey}\" ~/.ssh/authorized_keys";
                    var checkResult = client.RunCommand(checkCommand);

                    if (checkResult.ExitStatus == 0)
                    {
                        Log("Public key already exists in authorized_keys.");
                    }
                    else
                    {
                        var appendCommand = $"echo \"{PublicKey}\" >> ~/.ssh/authorized_keys";
                        var appendResult = client.RunCommand(appendCommand);

                        Log(appendResult.ExitStatus == 0
                            ? "Public key successfully added to authorized_keys."
                            : $"Failed to add public key to authorized_keys: {appendResult.Error}");
                    }
                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        private void Log(string msg)
        {
            _sb.Clear();
            _sb.Append(DateTime.Now.ToString("[HH:mm:ss.fff]"));
            _sb.Append(" - ");
            _sb.Append(msg.Trim());
            LogEventHandler?.Invoke(_sb.ToString());
        }
    }
}