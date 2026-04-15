namespace Kost_SiguraGura
{
    partial class TenantDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelProfile = new System.Windows.Forms.Panel();
            this.pictureBoxProfile = new System.Windows.Forms.PictureBox();
            this.lblTenantTitle = new System.Windows.Forms.Label();

            this.lblId = new System.Windows.Forms.Label();
            this.lblIdValue = new System.Windows.Forms.Label();
            this.lblNama = new System.Windows.Forms.Label();
            this.lblNamaValue = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblEmailValue = new System.Windows.Forms.Label();
            this.lblNoHp = new System.Windows.Forms.Label();
            this.lblNoHpValue = new System.Windows.Forms.Label();
            this.lblNik = new System.Windows.Forms.Label();
            this.lblNikValue = new System.Windows.Forms.Label();
            this.lblTglLahir = new System.Windows.Forms.Label();
            this.lblTglLahirValue = new System.Windows.Forms.Label();
            this.lblAlamat = new System.Windows.Forms.Label();
            this.lblAlamatValue = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblGenderValue = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblRoleValue = new System.Windows.Forms.Label();

            this.lblPaymentHistory = new System.Windows.Forms.Label();
            this.lblPaymentHistoryValue = new System.Windows.Forms.Label();
            this.dataGridViewPayments = new System.Windows.Forms.DataGridView();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnSuspend = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            this.panelMain.SuspendLayout();
            this.panelProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPayments)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // panelMain
            this.panelMain.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);
            this.panelMain.Size = new System.Drawing.Size(700, 850);
            this.panelMain.TabIndex = 0;
            this.panelMain.Controls.Add(this.panelButtons);
            this.panelMain.Controls.Add(this.dataGridViewPayments);
            this.panelMain.Controls.Add(this.lblPaymentHistoryValue);
            this.panelMain.Controls.Add(this.lblPaymentHistory);
            this.panelMain.Controls.Add(this.lblRoleValue);
            this.panelMain.Controls.Add(this.lblRole);
            this.panelMain.Controls.Add(this.lblGenderValue);
            this.panelMain.Controls.Add(this.lblGender);
            this.panelMain.Controls.Add(this.lblAlamatValue);
            this.panelMain.Controls.Add(this.lblAlamat);
            this.panelMain.Controls.Add(this.lblTglLahirValue);
            this.panelMain.Controls.Add(this.lblTglLahir);
            this.panelMain.Controls.Add(this.lblNikValue);
            this.panelMain.Controls.Add(this.lblNik);
            this.panelMain.Controls.Add(this.lblNoHpValue);
            this.panelMain.Controls.Add(this.lblNoHp);
            this.panelMain.Controls.Add(this.lblEmailValue);
            this.panelMain.Controls.Add(this.lblEmail);
            this.panelMain.Controls.Add(this.lblNamaValue);
            this.panelMain.Controls.Add(this.lblNama);
            this.panelMain.Controls.Add(this.lblIdValue);
            this.panelMain.Controls.Add(this.lblId);
            this.panelMain.Controls.Add(this.panelProfile);

            // panelProfile
            this.panelProfile.BackColor = System.Drawing.SystemColors.Window;
            this.panelProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelProfile.Controls.Add(this.lblTenantTitle);
            this.panelProfile.Controls.Add(this.pictureBoxProfile);
            this.panelProfile.Location = new System.Drawing.Point(20, 20);
            this.panelProfile.Name = "panelProfile";
            this.panelProfile.Size = new System.Drawing.Size(660, 140);
            this.panelProfile.TabIndex = 0;
            this.panelProfile.Controls.SetChildIndex(this.pictureBoxProfile, 0);
            this.panelProfile.Controls.SetChildIndex(this.lblTenantTitle, 0);

            // pictureBoxProfile
            this.pictureBoxProfile.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pictureBoxProfile.Location = new System.Drawing.Point(10, 10);
            this.pictureBoxProfile.Name = "pictureBoxProfile";
            this.pictureBoxProfile.Size = new System.Drawing.Size(110, 110);
            this.pictureBoxProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxProfile.TabIndex = 0;
            this.pictureBoxProfile.TabStop = false;

            // lblTenantTitle
            this.lblTenantTitle.AutoSize = true;
            this.lblTenantTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTenantTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTenantTitle.Location = new System.Drawing.Point(130, 25);
            this.lblTenantTitle.Name = "lblTenantTitle";
            this.lblTenantTitle.Size = new System.Drawing.Size(150, 25);
            this.lblTenantTitle.TabIndex = 1;
            this.lblTenantTitle.Text = "Tenant Detail";

            // ID
            this.lblId.AutoSize = true;
            this.lblId.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblId.Location = new System.Drawing.Point(20, 170);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(25, 15);
            this.lblId.TabIndex = 1;
            this.lblId.Text = "ID:";

            this.lblIdValue.AutoSize = true;
            this.lblIdValue.Location = new System.Drawing.Point(150, 170);
            this.lblIdValue.Name = "lblIdValue";
            this.lblIdValue.Size = new System.Drawing.Size(25, 13);
            this.lblIdValue.TabIndex = 2;
            this.lblIdValue.Text = "-";

            // Nama Lengkap
            this.lblNama.AutoSize = true;
            this.lblNama.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNama.Location = new System.Drawing.Point(20, 200);
            this.lblNama.Name = "lblNama";
            this.lblNama.Size = new System.Drawing.Size(110, 15);
            this.lblNama.TabIndex = 1;
            this.lblNama.Text = "Nama Lengkap:";

            this.lblNamaValue.AutoSize = true;
            this.lblNamaValue.Location = new System.Drawing.Point(150, 200);
            this.lblNamaValue.Name = "lblNamaValue";
            this.lblNamaValue.Size = new System.Drawing.Size(25, 13);
            this.lblNamaValue.TabIndex = 2;
            this.lblNamaValue.Text = "-";

            // Email
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmail.Location = new System.Drawing.Point(20, 230);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(45, 15);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "Email:";

            this.lblEmailValue.AutoSize = true;
            this.lblEmailValue.Location = new System.Drawing.Point(150, 230);
            this.lblEmailValue.Name = "lblEmailValue";
            this.lblEmailValue.Size = new System.Drawing.Size(25, 13);
            this.lblEmailValue.TabIndex = 2;
            this.lblEmailValue.Text = "-";

            // No HP
            this.lblNoHp.AutoSize = true;
            this.lblNoHp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNoHp.Location = new System.Drawing.Point(20, 260);
            this.lblNoHp.Name = "lblNoHp";
            this.lblNoHp.Size = new System.Drawing.Size(60, 15);
            this.lblNoHp.TabIndex = 1;
            this.lblNoHp.Text = "No. HP:";

            this.lblNoHpValue.AutoSize = true;
            this.lblNoHpValue.Location = new System.Drawing.Point(150, 260);
            this.lblNoHpValue.Name = "lblNoHpValue";
            this.lblNoHpValue.Size = new System.Drawing.Size(25, 13);
            this.lblNoHpValue.TabIndex = 2;
            this.lblNoHpValue.Text = "-";

            // NIK
            this.lblNik.AutoSize = true;
            this.lblNik.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNik.Location = new System.Drawing.Point(20, 290);
            this.lblNik.Name = "lblNik";
            this.lblNik.Size = new System.Drawing.Size(30, 15);
            this.lblNik.TabIndex = 1;
            this.lblNik.Text = "NIK:";

            this.lblNikValue.AutoSize = true;
            this.lblNikValue.Location = new System.Drawing.Point(150, 290);
            this.lblNikValue.Name = "lblNikValue";
            this.lblNikValue.Size = new System.Drawing.Size(25, 13);
            this.lblNikValue.TabIndex = 2;
            this.lblNikValue.Text = "-";

            // Tanggal Lahir
            this.lblTglLahir.AutoSize = true;
            this.lblTglLahir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTglLahir.Location = new System.Drawing.Point(20, 320);
            this.lblTglLahir.Name = "lblTglLahir";
            this.lblTglLahir.Size = new System.Drawing.Size(90, 15);
            this.lblTglLahir.TabIndex = 1;
            this.lblTglLahir.Text = "Tanggal Lahir:";

            this.lblTglLahirValue.AutoSize = true;
            this.lblTglLahirValue.Location = new System.Drawing.Point(150, 320);
            this.lblTglLahirValue.Name = "lblTglLahirValue";
            this.lblTglLahirValue.Size = new System.Drawing.Size(25, 13);
            this.lblTglLahirValue.TabIndex = 2;
            this.lblTglLahirValue.Text = "-";

            // Alamat
            this.lblAlamat.AutoSize = true;
            this.lblAlamat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAlamat.Location = new System.Drawing.Point(20, 350);
            this.lblAlamat.Name = "lblAlamat";
            this.lblAlamat.Size = new System.Drawing.Size(80, 15);
            this.lblAlamat.TabIndex = 1;
            this.lblAlamat.Text = "Alamat Asal:";

            this.lblAlamatValue.AutoSize = true;
            this.lblAlamatValue.Location = new System.Drawing.Point(150, 350);
            this.lblAlamatValue.Name = "lblAlamatValue";
            this.lblAlamatValue.Size = new System.Drawing.Size(25, 13);
            this.lblAlamatValue.TabIndex = 2;
            this.lblAlamatValue.Text = "-";
            this.lblAlamatValue.MaximumSize = new System.Drawing.Size(500, 0);
            this.lblAlamatValue.AutoEllipsis = true;

            // Gender
            this.lblGender.AutoSize = true;
            this.lblGender.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGender.Location = new System.Drawing.Point(20, 380);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(80, 15);
            this.lblGender.TabIndex = 1;
            this.lblGender.Text = "Jenis Kelamin:";

            this.lblGenderValue.AutoSize = true;
            this.lblGenderValue.Location = new System.Drawing.Point(150, 380);
            this.lblGenderValue.Name = "lblGenderValue";
            this.lblGenderValue.Size = new System.Drawing.Size(25, 13);
            this.lblGenderValue.TabIndex = 2;
            this.lblGenderValue.Text = "-";

            // Role
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRole.Location = new System.Drawing.Point(20, 410);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(40, 15);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "Role:";

            this.lblRoleValue.AutoSize = true;
            this.lblRoleValue.Location = new System.Drawing.Point(150, 410);
            this.lblRoleValue.Name = "lblRoleValue";
            this.lblRoleValue.Size = new System.Drawing.Size(25, 13);
            this.lblRoleValue.TabIndex = 2;
            this.lblRoleValue.Text = "-";

            // Payment History
            this.lblPaymentHistory.AutoSize = true;
            this.lblPaymentHistory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPaymentHistory.Location = new System.Drawing.Point(20, 450);
            this.lblPaymentHistory.Name = "lblPaymentHistory";
            this.lblPaymentHistory.Size = new System.Drawing.Size(125, 19);
            this.lblPaymentHistory.TabIndex = 1;
            this.lblPaymentHistory.Text = "Riwayat Pembayaran";

            this.lblPaymentHistoryValue.AutoSize = true;
            this.lblPaymentHistoryValue.Location = new System.Drawing.Point(20, 480);
            this.lblPaymentHistoryValue.Name = "lblPaymentHistoryValue";
            this.lblPaymentHistoryValue.Size = new System.Drawing.Size(150, 13);
            this.lblPaymentHistoryValue.TabIndex = 2;
            this.lblPaymentHistoryValue.Text = "Loading payment history...";

            // dataGridViewPayments
            this.dataGridViewPayments.AllowUserToAddRows = false;
            this.dataGridViewPayments.AllowUserToDeleteRows = false;
            this.dataGridViewPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPayments.Location = new System.Drawing.Point(20, 480);
            this.dataGridViewPayments.Name = "dataGridViewPayments";
            this.dataGridViewPayments.ReadOnly = true;
            this.dataGridViewPayments.RowHeadersVisible = false;
            this.dataGridViewPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPayments.Size = new System.Drawing.Size(660, 200);
            this.dataGridViewPayments.TabIndex = 3;
            this.dataGridViewPayments.Visible = false;

            // panelButtons
            this.panelButtons.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 750);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(700, 100);
            this.panelButtons.TabIndex = 4;
            this.panelButtons.Controls.Add(this.btnSuspend);
            this.panelButtons.Controls.Add(this.btnClose);

            // btnSuspend
            this.btnSuspend.BackColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.btnSuspend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSuspend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuspend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSuspend.ForeColor = System.Drawing.Color.White;
            this.btnSuspend.Location = new System.Drawing.Point(200, 20);
            this.btnSuspend.Name = "btnSuspend";
            this.btnSuspend.Size = new System.Drawing.Size(150, 40);
            this.btnSuspend.TabIndex = 0;
            this.btnSuspend.Text = "Suspend";
            this.btnSuspend.UseVisualStyleBackColor = false;

            // btnClose
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.Location = new System.Drawing.Point(380, 20);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 40);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;

            // TenantDetailForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 850);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TenantDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tenant Detail";

            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelProfile.ResumeLayout(false);
            this.panelProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPayments)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelProfile;
        private System.Windows.Forms.PictureBox pictureBoxProfile;
        private System.Windows.Forms.Label lblTenantTitle;

        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblIdValue;
        private System.Windows.Forms.Label lblNama;
        private System.Windows.Forms.Label lblNamaValue;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblEmailValue;
        private System.Windows.Forms.Label lblNoHp;
        private System.Windows.Forms.Label lblNoHpValue;
        private System.Windows.Forms.Label lblNik;
        private System.Windows.Forms.Label lblNikValue;
        private System.Windows.Forms.Label lblTglLahir;
        private System.Windows.Forms.Label lblTglLahirValue;
        private System.Windows.Forms.Label lblAlamat;
        private System.Windows.Forms.Label lblAlamatValue;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Label lblGenderValue;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblRoleValue;

        private System.Windows.Forms.Label lblPaymentHistory;
        private System.Windows.Forms.Label lblPaymentHistoryValue;
        private System.Windows.Forms.DataGridView dataGridViewPayments;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnSuspend;
        private System.Windows.Forms.Button btnClose;
    }
}
