namespace SHARRandomizerFrontend
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            lbURL = new Label();
            tbURL = new TextBox();
            tbPort = new TextBox();
            lblPort = new Label();
            tbPass = new TextBox();
            lblPassword = new Label();
            tbSlot = new TextBox();
            lblSlot = new Label();
            pnConnection = new Panel();
            btnSettings = new Button();
            btnConnect = new Button();
            pnLog = new Panel();
            tlpSendMessage = new TableLayoutPanel();
            tbMessage = new TextBox();
            btnSend = new Button();
            txbLog = new RichTextBox();
            pnConnection.SuspendLayout();
            pnLog.SuspendLayout();
            tlpSendMessage.SuspendLayout();
            SuspendLayout();
            // 
            // lbURL
            // 
            lbURL.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbURL.AutoSize = true;
            lbURL.Location = new Point(3, 9);
            lbURL.Name = "lbURL";
            lbURL.Size = new Size(42, 15);
            lbURL.TabIndex = 0;
            lbURL.Text = "Server:";
            // 
            // tbURL
            // 
            tbURL.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbURL.Location = new Point(51, 6);
            tbURL.Name = "tbURL";
            tbURL.Size = new Size(130, 23);
            tbURL.TabIndex = 1;
            tbURL.Text = "archipelago.gg";
            // 
            // tbPort
            // 
            tbPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbPort.Location = new Point(252, 6);
            tbPort.Name = "tbPort";
            tbPort.Size = new Size(130, 23);
            tbPort.TabIndex = 3;
            // 
            // lblPort
            // 
            lblPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblPort.AutoSize = true;
            lblPort.Location = new Point(214, 9);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(32, 15);
            lblPort.TabIndex = 2;
            lblPort.Text = "Port:";
            // 
            // tbPass
            // 
            tbPass.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbPass.Location = new Point(252, 35);
            tbPass.Name = "tbPass";
            tbPass.Size = new Size(130, 23);
            tbPass.TabIndex = 7;
            // 
            // lblPassword
            // 
            lblPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(186, 38);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(60, 15);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "Password:";
            // 
            // tbSlot
            // 
            tbSlot.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbSlot.Location = new Point(51, 35);
            tbSlot.Name = "tbSlot";
            tbSlot.Size = new Size(130, 23);
            tbSlot.TabIndex = 5;
            // 
            // lblSlot
            // 
            lblSlot.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblSlot.AutoSize = true;
            lblSlot.Location = new Point(15, 38);
            lblSlot.Name = "lblSlot";
            lblSlot.Size = new Size(30, 15);
            lblSlot.TabIndex = 4;
            lblSlot.Text = "Slot:";
            // 
            // pnConnection
            // 
            pnConnection.Controls.Add(btnSettings);
            pnConnection.Controls.Add(btnConnect);
            pnConnection.Controls.Add(lbURL);
            pnConnection.Controls.Add(tbPass);
            pnConnection.Controls.Add(tbURL);
            pnConnection.Controls.Add(lblPassword);
            pnConnection.Controls.Add(lblPort);
            pnConnection.Controls.Add(tbSlot);
            pnConnection.Controls.Add(tbPort);
            pnConnection.Controls.Add(lblSlot);
            pnConnection.Location = new Point(12, 12);
            pnConnection.Name = "pnConnection";
            pnConnection.Size = new Size(558, 69);
            pnConnection.TabIndex = 8;
            // 
            // btnSettings
            // 
            btnSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSettings.Location = new Point(450, 6);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(102, 23);
            btnSettings.TabIndex = 9;
            btnSettings.Text = "Client Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // btnConnect
            // 
            btnConnect.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnConnect.Location = new Point(450, 35);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(102, 23);
            btnConnect.TabIndex = 8;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // pnLog
            // 
            pnLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnLog.Controls.Add(tlpSendMessage);
            pnLog.Controls.Add(txbLog);
            pnLog.Location = new Point(12, 104);
            pnLog.Name = "pnLog";
            pnLog.Size = new Size(558, 445);
            pnLog.TabIndex = 9;
            // 
            // tlpSendMessage
            // 
            tlpSendMessage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tlpSendMessage.ColumnCount = 2;
            tlpSendMessage.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpSendMessage.ColumnStyles.Add(new ColumnStyle());
            tlpSendMessage.Controls.Add(tbMessage, 0, 0);
            tlpSendMessage.Controls.Add(btnSend, 1, 0);
            tlpSendMessage.Location = new Point(3, 412);
            tlpSendMessage.Name = "tlpSendMessage";
            tlpSendMessage.RowCount = 1;
            tlpSendMessage.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSendMessage.Size = new Size(552, 30);
            tlpSendMessage.TabIndex = 11;
            // 
            // tbMessage
            // 
            tbMessage.Dock = DockStyle.Fill;
            tbMessage.Location = new Point(3, 3);
            tbMessage.Name = "tbMessage";
            tbMessage.Size = new Size(465, 23);
            tbMessage.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(474, 3);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txbLog
            // 
            txbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txbLog.BackColor = Color.White;
            txbLog.Location = new Point(0, 0);
            txbLog.Name = "txbLog";
            txbLog.ReadOnly = true;
            txbLog.Size = new Size(558, 410);
            txbLog.TabIndex = 0;
            txbLog.Text = "";
            // 
            // Main
            // 
            AcceptButton = btnConnect;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 561);
            Controls.Add(pnLog);
            Controls.Add(pnConnection);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 600);
            Name = "Main";
            Text = "Simpsons Hit & Run Archipelago";
            Load += Main_Load;
            pnConnection.ResumeLayout(false);
            pnConnection.PerformLayout();
            pnLog.ResumeLayout(false);
            tlpSendMessage.ResumeLayout(false);
            tlpSendMessage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lbURL;
        private TextBox tbURL;
        private TextBox tbPort;
        private Label lblPort;
        private TextBox tbPass;
        private Label lblPassword;
        private TextBox tbSlot;
        private Label lblSlot;
        private Panel pnConnection;
        private Button btnConnect;
        private Panel pnLog;
        private RichTextBox txbLog;
        private Button btnSettings;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnSend;
        private TextBox tbMessage;
        private TableLayoutPanel tlpSendMessage;
    }
}
