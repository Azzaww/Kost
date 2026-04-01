using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class Sidebar : Form
    {
        public Sidebar()
        {
            InitializeComponent();
            lblName.Text = Session.Username;
        }

        private void Sidebar_Load(object sender, EventArgs e)
        {
            ShowUc(new BerandaPage());
        }

        void ShowUc (UserControl uc)
        {
            guna2Panel2.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            guna2Panel2.Controls.Add(uc);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ShowUc(new BerandaPage());
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ShowUc(new DataKamar());
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            ShowUc(new DataPenyewa());
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            ShowUc(new PembayaranForm());
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            ShowUc(new GalleryForm());
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            ShowUc(new Report());
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            this.Close();
            new Form1().Show();
        }
    }
}
