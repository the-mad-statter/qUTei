using System;
using System.Drawing;
using System.Windows.Forms;

namespace qUTei
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            Bitmap b = new Bitmap(this.BackgroundImage);
            b.MakeTransparent(b.GetPixel(1, 1));
            this.BackgroundImage = b;
        }

        private void Splash_Load(object sender, EventArgs e)
        {
        }
    }
}
