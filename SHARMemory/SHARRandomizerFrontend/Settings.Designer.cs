namespace SHARRandomizerFrontend
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            btnSave = new Button();
            tbxPath = new TextBox();
            lblAPPath = new Label();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.Location = new Point(307, 56);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // tbxPath
            // 
            tbxPath.Location = new Point(12, 27);
            tbxPath.Name = "tbxPath";
            tbxPath.Size = new Size(370, 23);
            tbxPath.TabIndex = 3;
            // 
            // lblAPPath
            // 
            lblAPPath.AutoSize = true;
            lblAPPath.Location = new Point(12, 9);
            lblAPPath.Name = "lblAPPath";
            lblAPPath.Size = new Size(146, 15);
            lblAPPath.TabIndex = 4;
            lblAPPath.Text = "Path to Archipelago Install";
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(394, 95);
            Controls.Add(lblAPPath);
            Controls.Add(tbxPath);
            Controls.Add(btnSave);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Settings";
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnSave;
        private TextBox tbxPath;
        private Label lblAPPath;
    }
}