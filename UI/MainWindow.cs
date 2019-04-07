using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SaveEditor.Core;

namespace SaveEditor.UI
{
    public partial class MainWindow : Form
    {
        private ROM ROM_;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadRom();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadRom()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "GBA ROMs|*.gba|All Files|*.*",
                Title = "Select TMC ROM"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                ROM_ = new ROM(ofd.FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (ROM.Instance.version.Equals(RegionVersion.None))
            {
                MessageBox.Show("Invalid TMC ROM. Please Open a valid ROM.", "Incorrect ROM", MessageBoxButtons.OK);
                statusText.Text = "Unable to determine ROM.";
                return;
            }


            statusText.Text = "Loaded: " + ROM.Instance.path;
        }
    }
}
