//
// Copyright (c) 2010-2012, MatthiWare
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software. 
// You shall include 'MatthiWare' in the credits/about section of the program
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NxtLauncher.Data;
using System.Net;
using System.IO;

namespace NxtLauncher.UI
{
    public partial class frmUpdater : Form
    {
        int index = 0;
        private UpdateSaveFile localInfoFile;
        private String baseUrl;

        public frmUpdater(UpdateSaveFile file, String baseUrl)
        {
            InitializeComponent();

            localInfoFile = file;

            pbInstall.Maximum = (file.UpdateFileCollection.Count)*100;

            this.baseUrl = baseUrl;

            foreach (UpdateFileInfo fileInfo in file.UpdateFileCollection)
            {
                ListViewItem lvItem = new ListViewItem(new String[] { fileInfo.Name, "Waiting...", fileInfo.Description });
                lvItems.Items.Add(lvItem);
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (btnInstall.Text == "Finish")
            {
                Application.Exit();
            }
            else
            {
                btnInstall.Enabled = false;
                DownloadFile();
            }
        }

        private void SetStatus(String p)
        {
            lblStatus.Text = String.Format("Status: {0}", p);
        }

        private void DownloadFile()
        {
            WebClient downloadClient = new WebClient();

            downloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadClient_DownloadProgressChanged);
            downloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadClient_DownloadFileCompleted);

            ListViewItem currItem = lvItems.Items[index];

            String name = currItem.SubItems[0].Text;

            SetStatus("Downloading " + name + "...");
            currItem.SubItems[1].Text = "Downloading...";

            String local = String.Format(@".\{0}", name);
            String online = String.Format("{0}{1}", baseUrl, name);

            if (File.Exists(local))
            {
                if (File.Exists(local + ".old"))
                    File.Delete(local + ".old");

                File.Move(local, local + ".old");
            }


            downloadClient.DownloadFileAsync(new Uri(online), local);
        }

        void downloadClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ListViewItem currItem = lvItems.Items[index];
            currItem.SubItems[1].Text = "Downloaded";

            pbInstall.Increment(100);
            pbDownload.Value = 0;

            index += 1;

            if (lvItems.Items.Count - 1 >= index)
            {

                DownloadFile();
            }
            else
            {
                SetStatus("Finished!");
                btnInstall.Text = "Finish";
                btnInstall.Enabled = true;
            }
        }

        void downloadClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbDownload.Value = e.ProgressPercentage;
        }
    }
}
