﻿using Renci.SshNet;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WinSSHCopyId
{
    public partial class MainForm : Form
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly string _filePath;

        public MainForm()
        {
            InitializeComponent();
            _filePath = Path.Combine(Path.GetTempPath(), "WinSSHCopyId.txt");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return;
                }
                var content = File.ReadAllLines(_filePath);
                if (content.Length > 0) txtHost.Text = content[0];
                if (content.Length > 1) txtUsername.Text = content[1];
                if (content.Length > 2) txtPublicKey.Text = content[2];
            }
            catch (Exception ex)
            {
                Log($"Error loading text: {ex.Message}");
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            ClearLog();

            if (txtHost.Text.Trim().Length == 0
                || txtUsername.Text.Trim().Length == 0
                || txtPublicKey.Text.Trim().Length == 0)
            {
                return;
            }

            var host = txtHost.Text.Trim();
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();
            var publicKey = txtPublicKey.Text.Trim();
            try
            {
                var client = new SshClient(host, username, password);
                client.Connect();

                var cmd = client.RunCommand("echo 'A connection was successfully established with the server' ");
                Log(cmd.Result);

                client.RunCommand("mkdir -p ~/.ssh");
                var checkCommand = $"grep -q \"{publicKey}\" ~/.ssh/authorized_keys";
                var checkResult = client.RunCommand(checkCommand);

                if (checkResult.ExitStatus == 0)
                {
                    Log("Public key already exists in authorized_keys.");
                }
                else
                {
                    var appendCommand = $"echo \"{publicKey}\" >> ~/.ssh/authorized_keys";
                    var appendResult = client.RunCommand(appendCommand);

                    Log(appendResult.ExitStatus == 0
                        ? "Public key successfully added to authorized_keys."
                        : $"Error: {appendResult.Error}");
                }
                client.Disconnect();

                File.WriteAllText(_filePath, $"{host}\n{username}\n{publicKey}");
            }
            catch (Exception ex)
            {
                Log("ERR: " + ex.Message);
            }
        }

        #region "METHOD"

        private void Log(string msg)
        {
            _sb.Clear();
            if (txtConsole.Text.Trim().Length > 0)
            {
                _sb.Append(Environment.NewLine);
            }
            _sb.Append(DateTime.Now.ToString("[HH:mm:ss-fff]"));
            _sb.Append(" - ");
            _sb.Append(msg.Trim());
            txtConsole.AppendText(_sb.ToString());
            txtConsole.ScrollToCaret();
        }

        private void ClearLog()
        {
            txtConsole.Clear();
        }

        #endregion "METHOD"
    }
}