using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHARRandomizerFrontend
{
    public partial class Settings : Form
    {
        clsSettings CSettings;

        public Settings(clsSettings csettings)
        {
            InitializeComponent();
            CSettings = csettings;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CSettings.APPath = tbxPath.Text;
            SettingsManager.Save(CSettings);
            this.Hide();
        }
    }
}
