namespace Kost_SiguraGura
{
    partial class PaymentCardControl
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
            this.panelCard = new Guna.UI2.WinForms.Guna2Panel();
            this.panelLeft = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTenantName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRoomInfo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panelMiddle = new Guna.UI2.WinForms.Guna2Panel();
            this.lblDate = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblAmount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panelRigt = new Guna.UI2.WinForms.Guna2Panel();
            this.btnReceipt = new Guna.UI2.WinForms.Guna2Button();
            this.btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            this.btnReject = new Guna.UI2.WinForms.Guna2Button();
            this.panelCard.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelMiddle.SuspendLayout();
            this.panelRigt.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCard
            // 
            this.panelCard.BackColor = System.Drawing.Color.Transparent;
            this.panelCard.BorderColor = System.Drawing.Color.Black;
            this.panelCard.BorderRadius = 8;
            this.panelCard.BorderThickness = 1;
            this.panelCard.Controls.Add(this.panelRigt);
            this.panelCard.Controls.Add(this.panelMiddle);
            this.panelCard.Controls.Add(this.panelLeft);
            this.panelCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCard.FillColor = System.Drawing.SystemColors.Window;
            this.panelCard.Location = new System.Drawing.Point(0, 0);
            this.panelCard.Name = "panelCard";
            this.panelCard.Size = new System.Drawing.Size(1000, 100);
            this.panelCard.TabIndex = 0;
            // 
            // panelLeft
            // 
            this.panelLeft.BorderColor = System.Drawing.Color.White;
            this.panelLeft.BorderRadius = 8;
            this.panelLeft.Controls.Add(this.lblRoomInfo);
            this.panelLeft.Controls.Add(this.lblTenantName);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.FillColor = System.Drawing.SystemColors.Window;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(300, 100);
            this.panelLeft.TabIndex = 0;
            // 
            // lblTenantName
            // 
            this.lblTenantName.BackColor = System.Drawing.Color.Transparent;
            this.lblTenantName.Font = new System.Drawing.Font("Inter", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenantName.Location = new System.Drawing.Point(17, 13);
            this.lblTenantName.Name = "lblTenantName";
            this.lblTenantName.Size = new System.Drawing.Size(101, 24);
            this.lblTenantName.TabIndex = 0;
            this.lblTenantName.Text = "Tenant Name";
            // 
            // lblRoomInfo
            // 
            this.lblRoomInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblRoomInfo.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoomInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblRoomInfo.Location = new System.Drawing.Point(17, 34);
            this.lblRoomInfo.Name = "lblRoomInfo";
            this.lblRoomInfo.Size = new System.Drawing.Size(116, 19);
            this.lblRoomInfo.TabIndex = 1;
            this.lblRoomInfo.Text = "Kamar A9 - transfer";
            // 
            // panelMiddle
            // 
            this.panelMiddle.BorderColor = System.Drawing.Color.White;
            this.panelMiddle.BorderRadius = 8;
            this.panelMiddle.Controls.Add(this.lblDate);
            this.panelMiddle.Controls.Add(this.lblAmount);
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMiddle.FillColor = System.Drawing.SystemColors.Window;
            this.panelMiddle.Location = new System.Drawing.Point(300, 0);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(250, 100);
            this.panelMiddle.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.Gray;
            this.lblDate.Location = new System.Drawing.Point(17, 34);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(69, 19);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "15/04/2024";
            // 
            // lblAmount
            // 
            this.lblAmount.BackColor = System.Drawing.Color.Transparent;
            this.lblAmount.Font = new System.Drawing.Font("Inter", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.lblAmount.Location = new System.Drawing.Point(17, 13);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(87, 24);
            this.lblAmount.TabIndex = 0;
            this.lblAmount.Text = "Rp 100.000";
            // 
            // panelRigt
            // 
            this.panelRigt.BorderColor = System.Drawing.Color.White;
            this.panelRigt.BorderRadius = 8;
            this.panelRigt.Controls.Add(this.btnReject);
            this.panelRigt.Controls.Add(this.btnConfirm);
            this.panelRigt.Controls.Add(this.btnReceipt);
            this.panelRigt.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRigt.FillColor = System.Drawing.SystemColors.Window;
            this.panelRigt.Location = new System.Drawing.Point(550, 0);
            this.panelRigt.Name = "panelRigt";
            this.panelRigt.Size = new System.Drawing.Size(450, 100);
            this.panelRigt.TabIndex = 2;
            // 
            // btnReceipt
            // 
            this.btnReceipt.BorderColor = System.Drawing.Color.Gray;
            this.btnReceipt.BorderRadius = 8;
            this.btnReceipt.BorderThickness = 1;
            this.btnReceipt.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReceipt.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReceipt.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReceipt.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReceipt.FillColor = System.Drawing.Color.White;
            this.btnReceipt.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReceipt.ForeColor = System.Drawing.Color.Gray;
            this.btnReceipt.Location = new System.Drawing.Point(60, 33);
            this.btnReceipt.Name = "btnReceipt";
            this.btnReceipt.Size = new System.Drawing.Size(90, 30);
            this.btnReceipt.TabIndex = 0;
            this.btnReceipt.Text = "Receipt";
            // 
            // btnConfirm
            // 
            this.btnConfirm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnConfirm.BorderRadius = 8;
            this.btnConfirm.BorderThickness = 1;
            this.btnConfirm.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnConfirm.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnConfirm.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnConfirm.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnConfirm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnConfirm.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(180, 33);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(90, 30);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "Confirm";
            // 
            // btnReject
            // 
            this.btnReject.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnReject.BorderRadius = 8;
            this.btnReject.BorderThickness = 1;
            this.btnReject.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReject.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReject.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReject.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReject.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnReject.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReject.ForeColor = System.Drawing.Color.White;
            this.btnReject.Location = new System.Drawing.Point(298, 33);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(90, 30);
            this.btnReject.TabIndex = 2;
            this.btnReject.Text = "Reject";
            // 
            // PaymentCardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panelCard);
            this.Name = "PaymentCardControl";
            this.Size = new System.Drawing.Size(1000, 100);
            this.panelCard.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelMiddle.ResumeLayout(false);
            this.panelMiddle.PerformLayout();
            this.panelRigt.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2Panel panelCard;
        private Guna.UI2.WinForms.Guna2Panel panelLeft;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTenantName;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRoomInfo;
        private Guna.UI2.WinForms.Guna2Panel panelMiddle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDate;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAmount;
        private Guna.UI2.WinForms.Guna2Panel panelRigt;
        private Guna.UI2.WinForms.Guna2Button btnReceipt;
        private Guna.UI2.WinForms.Guna2Button btnReject;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
    }
}
