using NxtLauncher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;

namespace LauncherV1._0
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }
       

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            updater1.CheckForUpdates();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = Updater.GetPath(Application.ExecutablePath);
            if(!File.Exists(filePath + @"AlphaVersion0.1\AlphaVersion0.1.exe"))
            ZipFile.ExtractToDirectory(filePath + "AlphaVersion0.1.zip", filePath);
            Process process = new Process();
            process.StartInfo.WorkingDirectory = filePath;
            process.StartInfo.FileName = @"AlphaVersion0.1\AlphaVersion0.1.exe";
            process.Start();
            Application.Exit();
        }

    }
}
