using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Http;
using Newtonsoft.Json;

namespace Kost_SiguraGura
{
    public partial class GalleryForm : UserControl
    {
        public GalleryForm()
        {
            InitializeComponent();
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            AddGallery form = new AddGallery();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Logic refresh gallery bisa ditaruh di sini
            }
        }

        private void GalleryForm_Load(object sender, EventArgs e)
        {
            // Panggil API Gallery di sini nantinya
        }
    }
}
