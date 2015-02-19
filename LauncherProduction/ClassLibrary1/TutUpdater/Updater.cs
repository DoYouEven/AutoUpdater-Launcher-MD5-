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
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;
using System.IO;
using NxtLauncher.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Windows.Forms;
using NxtLauncher.UI;

namespace NxtLauncher
{
    public class Updater : Component
    {
        private const String LocalUpdateFile = @".\UpdateInfo.dat";

        public String UpdateUrl { get; set; }
        private Version Check;
        public void CheckForUpdates()
        {
            try
            {
                CleanUp();
                if (File.Exists(LocalUpdateFile))
                {
                    UpdateSaveFile CheckVersion = DecodeSaveFile(LocalUpdateFile);


                    Check = Version.Parse(CheckVersion.VersionString);
                }
                else
                   Version.TryParse("0.0.0.0", out Check);
                    WebClient downloadClient = new WebClient();
                downloadClient.DownloadFile(UpdateUrl, LocalUpdateFile);
                downloadClient.Dispose();

                if (!File.Exists(LocalUpdateFile))
                    throw new FileNotFoundException("The local update file is missing!", LocalUpdateFile);

                UpdateSaveFile localFile = DecodeSaveFile(LocalUpdateFile);

                //Version localVersion = Assembly.GetEntryAssembly().GetName().Version;
                Version onlineVersion = Version.Parse(localFile.VersionString);
                
                if (onlineVersion > Check)
                {
                    if (MessageBox.Show(String.Format("Version {0} available,\nInstall it now?", onlineVersion.ToString()), "NXT Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        frmUpdater updateForm = new frmUpdater(localFile, GetPath(UpdateUrl));
                        updateForm.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("You already have the latest version!", "NXT  Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error checking for updates\ntry again later!\n\nError Message:" + e.Message, "NXT Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetPath(string UpdateUrl)
        {
            StringBuilder sb = new StringBuilder();
            String[] updatePieces = UpdateUrl.Split('\\');
            for (int i = 0; i < updatePieces.Length -1; i++)
            {
                sb.Append(updatePieces[i] + '\\');
            }
            return sb.ToString();
        }

        private UpdateSaveFile DecodeSaveFile(string LocalUpdateFile)
        {
            FileStream localFileStream = null;
                BinaryFormatter decoder = null;
                try
                {
                    localFileStream = File.Open(LocalUpdateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    decoder = new BinaryFormatter();
                     
                    return (UpdateSaveFile)decoder.Deserialize(localFileStream);
                }
                catch (Exception e)
                {
                    throw new InvalidDataException("The local update info file is corrupt!", e);
                }
                finally
                {
                    if (localFileStream != null)
                        localFileStream.Close();
                }

        }

        private void CleanUp()
        {
            DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
            foreach (FileInfo fi in di.GetFiles("*.old", SearchOption.TopDirectoryOnly))
            {
                fi.Delete();
            }
           
        }
    }
}
