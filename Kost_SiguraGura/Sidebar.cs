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
            SetActiveButton(guna2Button1);
        }

        private void SetActiveButton(object sender)
        {
            Color amberYellow = ColorTranslator.FromHtml("#FBBF24");

            foreach (Control c in guna2Panel1.Controls)
            {
                if (c is Guna.UI2.WinForms.Guna2Button btn)
                {
                    // Reset ke Default
                    btn.FillColor = Color.Transparent;
                    btn.ForeColor = Color.Gray;
                    btn.CustomBorderThickness = new Padding(0, 0, 0, 0);

                    // Kembalikan ikon ke warna abu-abu (sesuaikan dengan nama resource kamu)
                    if (btn.Name == "guna2Button1") btn.Image = Properties.Resources.home_gray;
                    if (btn.Name == "guna2Button2") btn.Image = Properties.Resources.room_gray;
                    if (btn.Name == "guna2Button3") btn.Image = Properties.Resources.tenant_gray;
                    if (btn.Name == "guna2Button4") btn.Image = Properties.Resources.payment_gray;
                    if (btn.Name == "guna2Button5") btn.Image = Properties.Resources.report_gray;
                    if (btn.Name == "guna2Button6") btn.Image = Properties.Resources.gallery_gray;
                }
            }

            Guna.UI2.WinForms.Guna2Button selectedBtn = (Guna.UI2.WinForms.Guna2Button)sender;
            selectedBtn.FillColor = Color.FromArgb(40, amberYellow);
            selectedBtn.ForeColor = amberYellow;
            selectedBtn.CustomBorderColor = amberYellow;
            selectedBtn.CustomBorderThickness = new Padding(4, 0, 0, 0);

            //// Ganti ikon tombol yang diklik ke versi kuning
            if (selectedBtn.Name == "guna2Button1") selectedBtn.Image = Properties.Resources.home_yellow;
            if (selectedBtn.Name == "guna2Button2") selectedBtn.Image = Properties.Resources.room_yellow;
            if (selectedBtn.Name == "guna2Button3") selectedBtn.Image = Properties.Resources.tenant_yellow;
            if (selectedBtn.Name == "guna2Button4") selectedBtn.Image = Properties.Resources.payment_yellow;
            if (selectedBtn.Name == "guna2Button5") selectedBtn.Image = Properties.Resources.report_yellow;
            if (selectedBtn.Name == "guna2Button6") selectedBtn.Image = Properties.Resources.gallery_yellow;
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
            SetActiveButton(sender);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ShowUc(new DataKamar());
            SetActiveButton(sender);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            ShowUc(new DataPenyewa());
            SetActiveButton(sender);
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            ShowUc(new PembayaranForm());
            SetActiveButton(sender);
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            ShowUc(new GalleryForm());
            SetActiveButton(sender);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            ShowUc(new Report());
            SetActiveButton(sender);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            this.Close();
            new Form1().Show();
        }
    }
}
