using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NxtLauncher.Data;

namespace UpdateMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       

      

        private void Add(object sender, EventArgs e)
        {
            ListViewItem lvItem = new ListViewItem(new String[] { txtName.Text, txtDesc.Text });
            listView1.Items.Add(lvItem);
        }

        private void Generate(object sender, EventArgs e)
        {
            UpdateSaveFile file = new UpdateSaveFile(txtVersion.Text);
            foreach (ListViewItem item in listView1.Items)
            {
                file.UpdateFileCollection.Add(new UpdateFileInfo(item.SubItems[0].Text, item.SubItems[1].Text));
            }
            BinaryFormatter bf = new BinaryFormatter();

            FileStream bfStream = File.Open(@".\UpdateInfo.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            bf.Serialize(bfStream, file);
            bfStream.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].Remove();

        }



       

    
    }
}
