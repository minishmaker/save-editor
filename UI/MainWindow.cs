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
        private SRAM SRAM_;

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
                Filter = "GBA Save Files|*.sav|All Files|*.*",
                Title = "Select TMC Save File"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                SRAM_ = new SRAM(ofd.FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (SRAM.Instance.version.Equals(RegionVersion.None))
            {
                MessageBox.Show("Invalid TMC save file. Please open a valid save.", "Invalid save", MessageBoxButtons.OK);
                statusText.Text = "Unable to determine save file.";
                return;
            }


            statusText.Text = "Loaded: " + SRAM.Instance.path;
        }
    }
}
