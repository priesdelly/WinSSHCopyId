using Renci.SshNet;
using System;
using System.Threading.Tasks;

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

        public void Copy()
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
                        return;
                    }

                    var appendCommand = $"echo \"{PublicKey}\" >> ~/.ssh/authorized_keys";
                    var appendResult = client.RunCommand(appendCommand);

                    if (appendResult.ExitStatus == 0)
                    {
                        Log("Public key successfully added to authorized_keys.");
                        return;
                    }

                    Log($"Failed to add public key to authorized_keys: {appendResult.Error}");
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        public async Task CopyAsync()
        {
            await Task.Run(Copy);
        }

        private void Log(string msg)
        {
            if (LogEventHandler == null)
            {
                return;
            }

            LogEventHandler(msg.Trim());
        }
    }
}