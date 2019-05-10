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

        private SaveFile currentSaveFile_;

        public MainWindow()
        {
            InitializeComponent();

            saveCombo.Items.Clear();
            saveCombo.Items.Add("File 1");
            saveCombo.Items.Add("File 2");
            saveCombo.Items.Add("File 3");
            saveCombo.SelectedIndex = 0;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadRom();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
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
            /*
            if (SRAM.Instance.version.Equals(RegionVersion.None))
            {
                MessageBox.Show("Invalid TMC save file. Please open a valid save.", "Invalid save", MessageBoxButtons.OK);
                statusText.Text = "Unable to determine save file.";
                return;
            }
            */
            currentSaveFile_ = SRAM_.GetSaveFile(0);
            LoadData();
        }

        private void LoadData()
        {
            saveCombo.Enabled = true;
            fileNameTextBox.Enabled = true;

            fileNameTextBox.Text = currentSaveFile_.GetName();

            statusText.Text = "Loaded: " + SRAM.Instance.path;
        }

        private void Save()
        {
            if (SRAM_ != null)
            {
                currentSaveFile_.WriteName(fileNameTextBox.Text);

                SRAM_.SaveFile(currentSaveFile_, saveCombo.SelectedIndex);
            }
            else
            {
                statusText.Text = "Save failed: No save file loaded";
            }
        }

        private void saveCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SRAM_ != null)
            {
                // check to see if data changed, then load based on new index.
                currentSaveFile_ = SRAM_.GetSaveFile(saveCombo.SelectedIndex);
                LoadData();
            }
           
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Closing");
        }
    }
}
