using Newtonsoft.Json;
using SHARRandomizer;
using SHARRandomizer.Classes;
using System.Diagnostics;

namespace SHARRandomizerFrontend
{
    public partial class Main : Form
    {
        string VERSION = "Beta 0.4.5";
        private ArchipelagoClient? _ac;
        private MemoryManip? _mm;
        private bool _connected;

        private clsSettings cSettings;
        private Form settings;


        public Main()
        {
            InitializeComponent();

            Common.LogMessageReceived += OnLogMessage;

            cSettings = SettingsManager.Load();
            settings = new Settings(cSettings);
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            if (!await CheckVersion())
            {
                Application.Exit();
                return;
            }

            this.Text = $"Simpsons Hit & Run Archipelago {VERSION}";
            Common.WriteLog($"SHARRandomizer.exe version: {VERSION}", "Main");

            string? URI = null;
            string? SLOTNAME = null;
            string? PASSWORD = null;

            tbURL.Text = cSettings.prevURL;
            tbPort.Text = cSettings.prevPort;
            tbSlot.Text = cSettings.prevSlot;
            tbPass.Text = cSettings.prvPass;
        }

        public async Task<bool> CheckVersion()
        {
            try
            {
                using HttpClient _http = new();
                _http.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/vnd.github+json");
                _http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "archipelago");
                var latestReleaseJson = await _http.GetStringAsync("https://api.github.com/repos/nmize1/AP-SHARRandomizer/releases/latest");
                var latestRelease = JsonConvert.DeserializeObject<GitHubRelease>(latestReleaseJson);
                if (latestRelease != null && latestRelease.Name != VERSION)
                {
                    Common.WriteLog("ARE YOU ON THE LATEST VERSION?", "GitHub");
                    Common.WriteLog($"YOU ARE RUNNING VERSION: {VERSION}.", "GitHub");
                    Common.WriteLog($"THE LATEST VERSION ON GITHUB IS: {latestRelease.Name}", "GitHub");
                    var result = MessageBox.Show
                    (
                        $"ARE YOU ON THE LATEST VERSION?\nYOU ARE RUNNING VERSION: {VERSION}\n" +
                        $"THE LATEST VERSION ON GITHUB IS {latestRelease.Name}\nDo you want to continue?",
                        "Version Check",
                        MessageBoxButtons.YesNo
                    );

                    if (result == DialogResult.No)
                    {
                        MessageBox.Show("Opening latest release on GitHub...\nClient will close.");
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://github.com/nmize1/AP-SHARRandomizer/releases/latest",
                            UseShellExecute = true
                        });
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLog($"Error checking latest version: {ex}", "GitHub");
            }

            return true;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (tbURL.Text == "" || tbPort.Text == "" || tbSlot.Text == "")
            {
                MessageBox.Show("Missing required connection info.");
                return;
            }

            if (_connected)
            {
                btnConnect.Enabled = false;
                _connected = false;

                _ac?.Disconnect();
                _mm?.Stop();

                await Task.Delay(100);

                btnConnect.Text = "Connect";
                btnConnect.Enabled = true;
                tbSlot.Enabled = true;
                tbPort.Enabled = true;
                tbURL.Enabled = true;
                tbPass.Enabled = true;
                txbLog.Clear();
            }
            else
            {
                btnConnect.Enabled = true;

                _ac = new ArchipelagoClient();
                _ac.URI = $"{tbURL.Text}:{tbPort.Text}";
                _ac.SLOTNAME = tbSlot.Text;
                _ac.PASSWORD = tbPass.Text;

                _mm = new MemoryManip(_ac);
                _ac.mm = _mm;
                _ac.ConnectionSucceeded += OnConnect;
                _ac.ConnectionFailed += OnDisconnect;

                _ = Task.Run(() => _ac.Connect());
                _ = Task.Run(() => _mm.MemoryStart());

                btnConnect.Text = "Disconnect";
                btnConnect.Enabled = true;
                tbSlot.Enabled = false;
                tbPort.Enabled = false;
                tbURL.Enabled = false;
                tbPass.Enabled = false;

                cSettings.prevURL = tbURL.Text;
                cSettings.prevPort = tbPort.Text;
                cSettings.prevSlot = tbSlot.Text;
                cSettings.prvPass = tbPass.Text;
                SettingsManager.Save(cSettings);
            }
        }

        void OnConnect()
        {
            this.Invoke(new Action(() =>
            {
                _connected = true;
            }));
        }

        void OnDisconnect()
        {
            this.Invoke(new Action(() =>
            {
                _connected = false;
            }));

            MessageBox.Show("Failed to connect. Double check your connection info.");
        }

        void OnLogMessage(string message, string method)
        {
            if (InvokeRequired)
            {
                Invoke(() => AddLog(message, method));
            }
            else
                AddLog(message, method);
        }

        private void AddLog(string message, string method)
        {
            if (method == "ArchipelagoClient::Session_OnMessageReceived" || cSettings.ShowFullLog)
            {
                txbLog.AppendText(message + Environment.NewLine);
                txbLog.ScrollToCaret();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            settings.ShowDialog();
        }
    }
}
