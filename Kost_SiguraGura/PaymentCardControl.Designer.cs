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
            this.panelCard = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.lblTenantName = new System.Windows.Forms.Label();
            this.lblRoomInfo = new System.Windows.Forms.Label();
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.lblAmount = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.btnReceipt = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnReject = new System.Windows.Forms.Button();

            this.panelCard.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelMiddle.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();

            // panelCard - Main container (horizontal layout)
            this.panelCard.BackColor = System.Drawing.SystemColors.Window;
            this.panelCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCard.Location = new System.Drawing.Point(0, 0);
            this.panelCard.Name = "panelCard";
            this.panelCard.Size = new System.Drawing.Size(1000, 100);
            this.panelCard.TabIndex = 0;
            this.panelCard.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);

            // panelLeft - Tenant info column
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(15, 10);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(300, 80);
            this.panelLeft.TabIndex = 0;
            this.panelLeft.AutoScroll = false;
            this.panelLeft.Controls.Add(this.lblRoomInfo);
            this.panelLeft.Controls.Add(this.lblTenantName);

            // lblTenantName
            this.lblTenantName.AutoSize = true;
            this.lblTenantName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTenantName.ForeColor = System.Drawing.Color.Black;
            this.lblTenantName.Location = new System.Drawing.Point(0, 5);
            this.lblTenantName.Name = "lblTenantName";
            this.lblTenantName.Size = new System.Drawing.Size(100, 20);
            this.lblTenantName.TabIndex = 0;
            this.lblTenantName.Text = "Tenant Name";

            // lblRoomInfo
            this.lblRoomInfo.AutoSize = true;
            this.lblRoomInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRoomInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblRoomInfo.Location = new System.Drawing.Point(0, 30);
            this.lblRoomInfo.Name = "lblRoomInfo";
            this.lblRoomInfo.Size = new System.Drawing.Size(100, 15);
            this.lblRoomInfo.TabIndex = 1;
            this.lblRoomInfo.Text = "Kamar A9 - transfer";

            // panelMiddle - Amount column
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMiddle.Location = new System.Drawing.Point(315, 10);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(250, 80);
            this.panelMiddle.TabIndex = 1;
            this.panelMiddle.Controls.Add(this.lblDate);
            this.panelMiddle.Controls.Add(this.lblAmount);

            // lblAmount
            this.lblAmount.AutoSize = true;
            this.lblAmount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAmount.ForeColor = System.Drawing.Color.FromArgb(245, 158, 11); // Orange
            this.lblAmount.Location = new System.Drawing.Point(0, 5);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(80, 20);
            this.lblAmount.TabIndex = 2;
            this.lblAmount.Text = "Rp 100.000";

            // lblDate
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDate.ForeColor = System.Drawing.Color.Gray;
            this.lblDate.Location = new System.Drawing.Point(0, 30);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(80, 15);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "15/04/2024";

            // panelRight - Buttons column
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(565, 10);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(420, 80);
            this.panelRight.TabIndex = 2;
            this.panelRight.Controls.Add(this.btnReject);
            this.panelRight.Controls.Add(this.btnConfirm);
            this.panelRight.Controls.Add(this.btnReceipt);

            // btnReceipt - Show receipt button
            this.btnReceipt.BackColor = System.Drawing.SystemColors.Control;
            this.btnReceipt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReceipt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnReceipt.ForeColor = System.Drawing.Color.Gray;
            this.btnReceipt.Location = new System.Drawing.Point(10, 25);
            this.btnReceipt.Name = "btnReceipt";
            this.btnReceipt.Size = new System.Drawing.Size(80, 30);
            this.btnReceipt.TabIndex = 0;
            this.btnReceipt.Text = "Receipt";
            this.btnReceipt.UseVisualStyleBackColor = true;
            this.btnReceipt.Visible = false;
            this.btnReceipt.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnConfirm - Confirm button (green)
            this.btnConfirm.BackColor = System.Drawing.Color.FromArgb(52, 211, 153); // Green
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(100, 25);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(90, 30);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Visible = false;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatAppearance.BorderSize = 0;

            // btnReject - Reject button (red)
            this.btnReject.BackColor = System.Drawing.Color.FromArgb(239, 68, 68); // Red
            this.btnReject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnReject.ForeColor = System.Drawing.Color.White;
            this.btnReject.Location = new System.Drawing.Point(200, 25);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(90, 30);
            this.btnReject.TabIndex = 2;
            this.btnReject.Text = "Reject";
            this.btnReject.UseVisualStyleBackColor = false;
            this.btnReject.Visible = false;
            this.btnReject.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReject.FlatAppearance.BorderSize = 0;

            this.panelCard.Controls.Add(this.panelRight);
            this.panelCard.Controls.Add(this.panelMiddle);
            this.panelCard.Controls.Add(this.panelLeft);

            // PaymentCardControl
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
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelCard;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label lblTenantName;
        private System.Windows.Forms.Label lblRoomInfo;
        private System.Windows.Forms.Panel panelMiddle;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Button btnReceipt;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnReject;
    }
}
