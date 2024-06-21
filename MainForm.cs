using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WinSSHCopyId.Engine;

namespace WinSSHCopyId
{
    public partial class MainForm : Form
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly string _filePath;
        private readonly SSHCopyIdEngine sshCopyEngine;

        public MainForm()
        {
            InitializeComponent();

            _filePath = Path.Combine(Path.GetTempPath(), "WinSSHCopyId.txt");

            sshCopyEngine = new SSHCopyIdEngine();
            sshCopyEngine.LogEventHandler -= SshCopyEngine_LogEventHandler;
            sshCopyEngine.LogEventHandler += SshCopyEngine_LogEventHandler;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadFormData();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            ClearLog();

            if (AreInputsEmpty())
            {
                Log("Validation failed: Host, Username, and Public Key fields must not be empty.");
                return;
            }

            var host = txtHost.Text.Trim();
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();
            var publicKey = txtPublicKey.Text.Trim();

            try
            {
                sshCopyEngine.Host = host;
                sshCopyEngine.Username = username;
                sshCopyEngine.Password = password;
                sshCopyEngine.PublicKey = publicKey;

                var err = sshCopyEngine.Copy();
                if (err != null)
                {
                    throw err;
                }

                SaveFormData(host, username, publicKey);
            }
            catch (Exception ex)
            {
                Log($"Exception occurred: {ex.Message}");
            }
        }

        private void SshCopyEngine_LogEventHandler(string msg)
        {
            Log(msg);
        }

        private void Log(string msg)
        {
            _sb.Clear();
            if (!string.IsNullOrWhiteSpace(txtConsole.Text))
            {
                _sb.Append(Environment.NewLine);
            }
            _sb.Append(msg.Trim());
            txtConsole.AppendText(_sb.ToString());
            txtConsole.ScrollToCaret();
        }

        private void LoadFormData()
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
                Log($"Error loading form data: {ex.Message}");
            }
        }

        private void SaveFormData(string host, string username, string publicKey)
        {
            try
            {
                File.WriteAllText(_filePath, $"{host}\n{username}\n{publicKey}");
                Log("Form data saved successfully.");
            }
            catch (Exception ex)
            {
                Log($"Error saving form data: {ex.Message}");
            }
        }

        private bool AreInputsEmpty()
        {
            return txtHost.Text.Trim().Length == 0
                   || txtUsername.Text.Trim().Length == 0
                   || txtPublicKey.Text.Trim().Length == 0;
        }

        private void ClearLog()
        {
            txtConsole.Clear();
        }
    }
}