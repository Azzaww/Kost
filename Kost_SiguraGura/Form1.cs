using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace Kost_SiguraGura
{
    public partial class Form1 : Form
    {
        Kost_RahmatEntities1 db = new Kost_RahmatEntities1();
        public Form1()
        {
            InitializeComponent();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
           
        }

        private void btnAuth_Click_1(object sender, EventArgs e)
        {

        }

        private void btnAuth_Click_2(object sender, EventArgs e)
        {
            var username = txtUsername.Text;
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username dan Password wajib diisi!");
                return;
            }

            var user = db.User.FirstOrDefault(x => x.username == username && x.password == password);

            if (user == null)
            {
                MessageBox.Show("Username dan Password tidak ditemukan!");
                return;
            }

            Session.UserId = user.id;
            Session.Username = user.username;
            Session.UserRole = user.role;

            this.Hide();


            if (user.role == "admin")
            {
                new Sidebar().Show();
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
