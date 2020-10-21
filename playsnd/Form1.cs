using System;
using System.IO;
using System.Windows.Forms;

namespace playsnd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Media.SoundPlayer sndplayr = new System.Media.SoundPlayer(playsnd.Properties.Resources.tile);
            this.Hide();
            sndplayr.PlaySync();
            this.Close();
        }
    }
}
