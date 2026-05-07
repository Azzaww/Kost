namespace Kost_SiguraGura
{
    partial class Report
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cleanup DataSyncManager first
                if (disposing && _dataSyncManager != null)
                {
                    try
                    {
                        _dataSyncManager.StopAutoSync();
                        _dataSyncManager.PaymentsRefreshed -= DataSyncManager_PaymentsRefreshed;
                        _dataSyncManager.RoomsRefreshed -= DataSyncManager_RoomsRefreshed;
                        _dataSyncManager.SyncStatusChanged -= DataSyncManager_SyncStatusChanged;
                        _dataSyncManager.SyncErrorOccurred -= DataSyncManager_SyncErrorOccurred;
                        _dataSyncManager.Dispose();
                    }
                    catch { }
                    finally
                    {
                        _dataSyncManager = null;
                    }
                }

                // Then cleanup UI components
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lblWelcome = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.guna2Panel4 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel18 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel15 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel11 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.TotalRevenueHtmlLabel12 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel17 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel14 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel10 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel16 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel13 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel9 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.guna2Panel7 = new Guna.UI2.WinForms.Guna2Panel();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.guna2HtmlLabel7 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel8 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel6 = new Guna.UI2.WinForms.Guna2Panel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.guna2HtmlLabel6 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel5 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.guna2Panel8 = new Guna.UI2.WinForms.Guna2Panel();
            this.chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.guna2HtmlLabel20 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel21 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.guna2Panel5 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel13 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel31 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel32 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel33 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel12 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel28 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel29 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel30 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel11 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel24 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel25 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel27 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel19 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel9 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel10 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel26 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel22 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel4 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel23 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.timeReportButton2 = new Guna.UI2.WinForms.Guna2Button();
            this.exportButton1 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2HtmlLabel34 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.guna2Panel4.SuspendLayout();
            this.guna2Panel1.SuspendLayout();
            this.guna2Panel3.SuspendLayout();
            this.guna2Panel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.guna2Panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.guna2Panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.guna2Panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.guna2Panel5.SuspendLayout();
            this.guna2Panel13.SuspendLayout();
            this.guna2Panel12.SuspendLayout();
            this.guna2Panel11.SuspendLayout();
            this.guna2Panel9.SuspendLayout();
            this.guna2Panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.Font = new System.Drawing.Font("Inter", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.Gray;
            this.lblWelcome.Location = new System.Drawing.Point(31, 64);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(4);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(397, 28);
            this.lblWelcome.TabIndex = 4;
            this.lblWelcome.Text = "Comprehensive financial analytics and reports";
            this.lblWelcome.UseWaitCursor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel2, 1, 0);
            this.tableLayoutPanel1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(27, 116);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1247, 160);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // guna2Panel4
            // 
            this.guna2Panel4.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel4.BorderRadius = 10;
            this.guna2Panel4.BorderThickness = 1;
            this.guna2Panel4.Controls.Add(this.guna2HtmlLabel18);
            this.guna2Panel4.Controls.Add(this.guna2HtmlLabel15);
            this.guna2Panel4.Controls.Add(this.guna2HtmlLabel11);
            this.guna2Panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel4.FillColor = System.Drawing.Color.White;
            this.guna2Panel4.Location = new System.Drawing.Point(940, 6);
            this.guna2Panel4.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.guna2Panel4.Name = "guna2Panel4";
            this.guna2Panel4.Size = new System.Drawing.Size(300, 148);
            this.guna2Panel4.TabIndex = 6;
            // 
            // guna2HtmlLabel18
            // 
            this.guna2HtmlLabel18.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel18.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel18.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel18.Location = new System.Drawing.Point(19, 106);
            this.guna2HtmlLabel18.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel18.Name = "guna2HtmlLabel18";
            this.guna2HtmlLabel18.Size = new System.Drawing.Size(72, 24);
            this.guna2HtmlLabel18.TabIndex = 14;
            this.guna2HtmlLabel18.Text = "1/2 rooms";
            this.guna2HtmlLabel18.UseWaitCursor = true;
            // 
            // guna2HtmlLabel15
            // 
            this.guna2HtmlLabel15.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel15.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel15.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel15.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel15.Location = new System.Drawing.Point(19, 54);
            this.guna2HtmlLabel15.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel15.Name = "guna2HtmlLabel15";
            this.guna2HtmlLabel15.Size = new System.Drawing.Size(53, 30);
            this.guna2HtmlLabel15.TabIndex = 12;
            this.guna2HtmlLabel15.Text = "50 %";
            this.guna2HtmlLabel15.UseWaitCursor = true;
            // 
            // guna2HtmlLabel11
            // 
            this.guna2HtmlLabel11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel11.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel11.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel11.Location = new System.Drawing.Point(19, 18);
            this.guna2HtmlLabel11.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel11.Name = "guna2HtmlLabel11";
            this.guna2HtmlLabel11.Size = new System.Drawing.Size(102, 24);
            this.guna2HtmlLabel11.TabIndex = 11;
            this.guna2HtmlLabel11.Text = "OCCUPANCY";
            this.guna2HtmlLabel11.UseWaitCursor = true;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel1.BorderRadius = 10;
            this.guna2Panel1.BorderThickness = 1;
            this.guna2Panel1.Controls.Add(this.guna2HtmlLabel2);
            this.guna2Panel1.Controls.Add(this.TotalRevenueHtmlLabel12);
            this.guna2Panel1.Controls.Add(this.guna2HtmlLabel3);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel1.FillColor = System.Drawing.Color.White;
            this.guna2Panel1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.guna2Panel1.Location = new System.Drawing.Point(7, 6);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(297, 148);
            this.guna2Panel1.TabIndex = 3;
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(15, 106);
            this.guna2HtmlLabel2.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(162, 24);
            this.guna2HtmlLabel2.TabIndex = 14;
            this.guna2HtmlLabel2.Text = "+ 12.5 % from last mth";
            this.guna2HtmlLabel2.UseWaitCursor = true;
            // 
            // TotalRevenueHtmlLabel12
            // 
            this.TotalRevenueHtmlLabel12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TotalRevenueHtmlLabel12.BackColor = System.Drawing.Color.Transparent;
            this.TotalRevenueHtmlLabel12.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalRevenueHtmlLabel12.ForeColor = System.Drawing.Color.Black;
            this.TotalRevenueHtmlLabel12.Location = new System.Drawing.Point(15, 54);
            this.TotalRevenueHtmlLabel12.Margin = new System.Windows.Forms.Padding(4);
            this.TotalRevenueHtmlLabel12.Name = "TotalRevenueHtmlLabel12";
            this.TotalRevenueHtmlLabel12.Size = new System.Drawing.Size(115, 30);
            this.TotalRevenueHtmlLabel12.TabIndex = 11;
            this.TotalRevenueHtmlLabel12.Text = "Rp 0";
            this.TotalRevenueHtmlLabel12.UseWaitCursor = true;
            this.TotalRevenueHtmlLabel12.Click += new System.EventHandler(this.TotalRevenueHtmlLabel12_Click);
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(15, 18);
            this.guna2HtmlLabel3.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(128, 24);
            this.guna2HtmlLabel3.TabIndex = 10;
            this.guna2HtmlLabel3.Text = "TOTAL REVENUE";
            this.guna2HtmlLabel3.UseWaitCursor = true;
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel3.BorderRadius = 10;
            this.guna2Panel3.BorderThickness = 1;
            this.guna2Panel3.Controls.Add(this.guna2HtmlLabel17);
            this.guna2Panel3.Controls.Add(this.guna2HtmlLabel14);
            this.guna2Panel3.Controls.Add(this.guna2HtmlLabel10);
            this.guna2Panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel3.FillColor = System.Drawing.Color.White;
            this.guna2Panel3.Location = new System.Drawing.Point(629, 6);
            this.guna2Panel3.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Size = new System.Drawing.Size(297, 148);
            this.guna2Panel3.TabIndex = 5;
            // 
            // guna2HtmlLabel17
            // 
            this.guna2HtmlLabel17.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel17.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel17.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel17.Location = new System.Drawing.Point(24, 106);
            this.guna2HtmlLabel17.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel17.Name = "guna2HtmlLabel17";
            this.guna2HtmlLabel17.Size = new System.Drawing.Size(77, 24);
            this.guna2HtmlLabel17.TabIndex = 14;
            this.guna2HtmlLabel17.Text = "Per month";
            this.guna2HtmlLabel17.UseWaitCursor = true;
            // 
            // guna2HtmlLabel14
            // 
            this.guna2HtmlLabel14.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel14.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel14.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel14.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel14.Location = new System.Drawing.Point(24, 54);
            this.guna2HtmlLabel14.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel14.Name = "guna2HtmlLabel14";
            this.guna2HtmlLabel14.Size = new System.Drawing.Size(81, 30);
            this.guna2HtmlLabel14.TabIndex = 12;
            this.guna2HtmlLabel14.Text = "Rp 1,4 jt";
            this.guna2HtmlLabel14.UseWaitCursor = true;
            // 
            // guna2HtmlLabel10
            // 
            this.guna2HtmlLabel10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel10.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel10.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel10.Location = new System.Drawing.Point(24, 18);
            this.guna2HtmlLabel10.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel10.Name = "guna2HtmlLabel10";
            this.guna2HtmlLabel10.Size = new System.Drawing.Size(85, 24);
            this.guna2HtmlLabel10.TabIndex = 11;
            this.guna2HtmlLabel10.Text = "AVG. RATE";
            this.guna2HtmlLabel10.UseWaitCursor = true;
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel2.BorderRadius = 10;
            this.guna2Panel2.BorderThickness = 1;
            this.guna2Panel2.Controls.Add(this.guna2HtmlLabel16);
            this.guna2Panel2.Controls.Add(this.guna2HtmlLabel13);
            this.guna2Panel2.Controls.Add(this.guna2HtmlLabel9);
            this.guna2Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel2.FillColor = System.Drawing.Color.White;
            this.guna2Panel2.Location = new System.Drawing.Point(318, 6);
            this.guna2Panel2.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(297, 148);
            this.guna2Panel2.TabIndex = 4;
            // 
            // guna2HtmlLabel16
            // 
            this.guna2HtmlLabel16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel16.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel16.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel16.Location = new System.Drawing.Point(23, 106);
            this.guna2HtmlLabel16.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel16.Name = "guna2HtmlLabel16";
            this.guna2HtmlLabel16.Size = new System.Drawing.Size(95, 24);
            this.guna2HtmlLabel16.TabIndex = 13;
            this.guna2HtmlLabel16.Text = "Wait confirm";
            this.guna2HtmlLabel16.UseWaitCursor = true;
            // 
            // guna2HtmlLabel13
            // 
            this.guna2HtmlLabel13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel13.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel13.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel13.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel13.Location = new System.Drawing.Point(23, 54);
            this.guna2HtmlLabel13.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel13.Name = "guna2HtmlLabel13";
            this.guna2HtmlLabel13.Size = new System.Drawing.Size(47, 30);
            this.guna2HtmlLabel13.TabIndex = 12;
            this.guna2HtmlLabel13.Text = "Rp 0";
            this.guna2HtmlLabel13.UseWaitCursor = true;
            // 
            // guna2HtmlLabel9
            // 
            this.guna2HtmlLabel9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel9.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel9.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel9.Location = new System.Drawing.Point(23, 18);
            this.guna2HtmlLabel9.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel9.Name = "guna2HtmlLabel9";
            this.guna2HtmlLabel9.Size = new System.Drawing.Size(109, 24);
            this.guna2HtmlLabel9.TabIndex = 11;
            this.guna2HtmlLabel9.Text = "PENDING REV.";
            this.guna2HtmlLabel9.UseWaitCursor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 572F));
            this.tableLayoutPanel3.Controls.Add(this.guna2Panel7, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.guna2Panel6, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(27, 294);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1247, 410);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // guna2Panel7
            // 
            this.guna2Panel7.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel7.BorderRadius = 10;
            this.guna2Panel7.BorderThickness = 1;
            this.guna2Panel7.Controls.Add(this.chart2);
            this.guna2Panel7.Controls.Add(this.guna2HtmlLabel7);
            this.guna2Panel7.Controls.Add(this.guna2HtmlLabel8);
            this.guna2Panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel7.FillColor = System.Drawing.Color.White;
            this.guna2Panel7.Location = new System.Drawing.Point(679, 4);
            this.guna2Panel7.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel7.Name = "guna2Panel7";
            this.guna2Panel7.Size = new System.Drawing.Size(564, 402);
            this.guna2Panel7.TabIndex = 0;
            // 
            // chart2
            // 
            this.chart2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart2.Legends.Add(legend1);
            this.chart2.Location = new System.Drawing.Point(20, 85);
            this.chart2.Margin = new System.Windows.Forms.Padding(4);
            this.chart2.Name = "chart2";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart2.Series.Add(series1);
            this.chart2.Size = new System.Drawing.Size(516, 303);
            this.chart2.TabIndex = 2;
            this.chart2.Text = "chart2";
            // 
            // guna2HtmlLabel7
            // 
            this.guna2HtmlLabel7.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel7.Font = new System.Drawing.Font("Inter", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel7.Location = new System.Drawing.Point(20, 46);
            this.guna2HtmlLabel7.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel7.Name = "guna2HtmlLabel7";
            this.guna2HtmlLabel7.Size = new System.Drawing.Size(137, 28);
            this.guna2HtmlLabel7.TabIndex = 1;
            this.guna2HtmlLabel7.Text = "Age distribution";
            // 
            // guna2HtmlLabel8
            // 
            this.guna2HtmlLabel8.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel8.Font = new System.Drawing.Font("Inter", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel8.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel8.Location = new System.Drawing.Point(17, 12);
            this.guna2HtmlLabel8.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel8.Name = "guna2HtmlLabel8";
            this.guna2HtmlLabel8.Size = new System.Drawing.Size(250, 34);
            this.guna2HtmlLabel8.TabIndex = 0;
            this.guna2HtmlLabel8.Text = "Tenant Demographics";
            // 
            // guna2Panel6
            // 
            this.guna2Panel6.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel6.BorderRadius = 10;
            this.guna2Panel6.BorderThickness = 1;
            this.guna2Panel6.Controls.Add(this.chart1);
            this.guna2Panel6.Controls.Add(this.guna2HtmlLabel6);
            this.guna2Panel6.Controls.Add(this.guna2HtmlLabel5);
            this.guna2Panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel6.FillColor = System.Drawing.Color.White;
            this.guna2Panel6.Location = new System.Drawing.Point(4, 4);
            this.guna2Panel6.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel6.Name = "guna2Panel6";
            this.guna2Panel6.Size = new System.Drawing.Size(667, 402);
            this.guna2Panel6.TabIndex = 0;
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(20, 85);
            this.chart1.Margin = new System.Windows.Forms.Padding(4);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(619, 303);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            // 
            // guna2HtmlLabel6
            // 
            this.guna2HtmlLabel6.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel6.Font = new System.Drawing.Font("Inter", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel6.Location = new System.Drawing.Point(17, 48);
            this.guna2HtmlLabel6.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel6.Name = "guna2HtmlLabel6";
            this.guna2HtmlLabel6.Size = new System.Drawing.Size(247, 28);
            this.guna2HtmlLabel6.TabIndex = 1;
            this.guna2HtmlLabel6.Text = "Monthly revenue breakdown";
            // 
            // guna2HtmlLabel5
            // 
            this.guna2HtmlLabel5.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel5.Font = new System.Drawing.Font("Inter", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel5.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel5.Location = new System.Drawing.Point(17, 15);
            this.guna2HtmlLabel5.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel5.Name = "guna2HtmlLabel5";
            this.guna2HtmlLabel5.Size = new System.Drawing.Size(266, 34);
            this.guna2HtmlLabel5.TabIndex = 0;
            this.guna2HtmlLabel5.Text = "Revenue by Room Type";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.guna2Panel8, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(23, 724);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1247, 446);
            this.tableLayoutPanel2.TabIndex = 11;
            // 
            // guna2Panel8
            // 
            this.guna2Panel8.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel8.BorderRadius = 10;
            this.guna2Panel8.BorderThickness = 1;
            this.guna2Panel8.Controls.Add(this.chart4);
            this.guna2Panel8.Controls.Add(this.guna2HtmlLabel20);
            this.guna2Panel8.Controls.Add(this.guna2HtmlLabel21);
            this.guna2Panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel8.FillColor = System.Drawing.Color.White;
            this.guna2Panel8.Location = new System.Drawing.Point(4, 4);
            this.guna2Panel8.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel8.Name = "guna2Panel8";
            this.guna2Panel8.Size = new System.Drawing.Size(1239, 438);
            this.guna2Panel8.TabIndex = 0;
            // 
            // chart4
            // 
            this.chart4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea3.Name = "ChartArea1";
            this.chart4.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart4.Legends.Add(legend3);
            this.chart4.Location = new System.Drawing.Point(20, 85);
            this.chart4.Margin = new System.Windows.Forms.Padding(4);
            this.chart4.Name = "chart4";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart4.Series.Add(series3);
            this.chart4.Size = new System.Drawing.Size(1191, 303);
            this.chart4.TabIndex = 2;
            this.chart4.Text = "chart4";
            // 
            // guna2HtmlLabel20
            // 
            this.guna2HtmlLabel20.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel20.Font = new System.Drawing.Font("Inter", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel20.Location = new System.Drawing.Point(17, 48);
            this.guna2HtmlLabel20.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel20.Name = "guna2HtmlLabel20";
            this.guna2HtmlLabel20.Size = new System.Drawing.Size(240, 28);
            this.guna2HtmlLabel20.TabIndex = 1;
            this.guna2HtmlLabel20.Text = "Last 6 months performance";
            // 
            // guna2HtmlLabel21
            // 
            this.guna2HtmlLabel21.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel21.Font = new System.Drawing.Font("Inter", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel21.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel21.Location = new System.Drawing.Point(17, 15);
            this.guna2HtmlLabel21.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel21.Name = "guna2HtmlLabel21";
            this.guna2HtmlLabel21.Size = new System.Drawing.Size(269, 34);
            this.guna2HtmlLabel21.TabIndex = 0;
            this.guna2HtmlLabel21.Text = "Monthly Revenue Trend";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 572F));
            this.tableLayoutPanel4.Controls.Add(this.guna2Panel5, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.guna2Panel9, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(23, 1190);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1247, 337);
            this.tableLayoutPanel4.TabIndex = 12;
            // 
            // guna2Panel5
            // 
            this.guna2Panel5.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel5.BorderRadius = 10;
            this.guna2Panel5.BorderThickness = 1;
            this.guna2Panel5.Controls.Add(this.guna2Panel13);
            this.guna2Panel5.Controls.Add(this.guna2Panel12);
            this.guna2Panel5.Controls.Add(this.guna2Panel11);
            this.guna2Panel5.Controls.Add(this.guna2HtmlLabel19);
            this.guna2Panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel5.FillColor = System.Drawing.Color.White;
            this.guna2Panel5.Location = new System.Drawing.Point(679, 4);
            this.guna2Panel5.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel5.Name = "guna2Panel5";
            this.guna2Panel5.Size = new System.Drawing.Size(564, 329);
            this.guna2Panel5.TabIndex = 0;
            // 
            // guna2Panel13
            // 
            this.guna2Panel13.BackColor = System.Drawing.Color.White;
            this.guna2Panel13.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.guna2Panel13.BorderRadius = 4;
            this.guna2Panel13.BorderThickness = 1;
            this.guna2Panel13.Controls.Add(this.guna2HtmlLabel31);
            this.guna2Panel13.Controls.Add(this.guna2HtmlLabel32);
            this.guna2Panel13.Controls.Add(this.guna2HtmlLabel33);
            this.guna2Panel13.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this.guna2Panel13.Location = new System.Drawing.Point(17, 198);
            this.guna2Panel13.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel13.Name = "guna2Panel13";
            this.guna2Panel13.Size = new System.Drawing.Size(523, 65);
            this.guna2Panel13.TabIndex = 4;
            // 
            // guna2HtmlLabel31
            // 
            this.guna2HtmlLabel31.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel31.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel31.ForeColor = System.Drawing.Color.Blue;
            this.guna2HtmlLabel31.Location = new System.Drawing.Point(329, 20);
            this.guna2HtmlLabel31.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel31.Name = "guna2HtmlLabel31";
            this.guna2HtmlLabel31.Size = new System.Drawing.Size(71, 26);
            this.guna2HtmlLabel31.TabIndex = 15;
            this.guna2HtmlLabel31.Text = "Rp 2,7 jt";
            // 
            // guna2HtmlLabel32
            // 
            this.guna2HtmlLabel32.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel32.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel32.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel32.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel32.Location = new System.Drawing.Point(20, 38);
            this.guna2HtmlLabel32.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel32.Name = "guna2HtmlLabel32";
            this.guna2HtmlLabel32.Size = new System.Drawing.Size(159, 24);
            this.guna2HtmlLabel32.TabIndex = 14;
            this.guna2HtmlLabel32.Text = "IF FULLY OCCOUPIED";
            this.guna2HtmlLabel32.UseWaitCursor = true;
            // 
            // guna2HtmlLabel33
            // 
            this.guna2HtmlLabel33.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel33.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel33.ForeColor = System.Drawing.Color.Blue;
            this.guna2HtmlLabel33.Location = new System.Drawing.Point(20, 7);
            this.guna2HtmlLabel33.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel33.Name = "guna2HtmlLabel33";
            this.guna2HtmlLabel33.Size = new System.Drawing.Size(123, 26);
            this.guna2HtmlLabel33.TabIndex = 2;
            this.guna2HtmlLabel33.Text = "Total Potential";
            // 
            // guna2Panel12
            // 
            this.guna2Panel12.BackColor = System.Drawing.Color.White;
            this.guna2Panel12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.guna2Panel12.BorderRadius = 4;
            this.guna2Panel12.BorderThickness = 1;
            this.guna2Panel12.Controls.Add(this.guna2HtmlLabel28);
            this.guna2Panel12.Controls.Add(this.guna2HtmlLabel29);
            this.guna2Panel12.Controls.Add(this.guna2HtmlLabel30);
            this.guna2Panel12.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(243)))), ((int)(((byte)(224)))));
            this.guna2Panel12.Location = new System.Drawing.Point(17, 123);
            this.guna2Panel12.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel12.Name = "guna2Panel12";
            this.guna2Panel12.Size = new System.Drawing.Size(523, 65);
            this.guna2Panel12.TabIndex = 3;
            // 
            // guna2HtmlLabel28
            // 
            this.guna2HtmlLabel28.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel28.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel28.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.guna2HtmlLabel28.Location = new System.Drawing.Point(329, 20);
            this.guna2HtmlLabel28.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel28.Name = "guna2HtmlLabel28";
            this.guna2HtmlLabel28.Size = new System.Drawing.Size(40, 26);
            this.guna2HtmlLabel28.TabIndex = 15;
            this.guna2HtmlLabel28.Text = "Rp 0";
            // 
            // guna2HtmlLabel29
            // 
            this.guna2HtmlLabel29.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel29.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel29.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel29.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel29.Location = new System.Drawing.Point(20, 38);
            this.guna2HtmlLabel29.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel29.Name = "guna2HtmlLabel29";
            this.guna2HtmlLabel29.Size = new System.Drawing.Size(134, 24);
            this.guna2HtmlLabel29.TabIndex = 14;
            this.guna2HtmlLabel29.Text = "0 TRANSACTIONS";
            this.guna2HtmlLabel29.UseWaitCursor = true;
            // 
            // guna2HtmlLabel30
            // 
            this.guna2HtmlLabel30.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel30.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel30.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.guna2HtmlLabel30.Location = new System.Drawing.Point(20, 7);
            this.guna2HtmlLabel30.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel30.Name = "guna2HtmlLabel30";
            this.guna2HtmlLabel30.Size = new System.Drawing.Size(73, 26);
            this.guna2HtmlLabel30.TabIndex = 2;
            this.guna2HtmlLabel30.Text = "Pending";
            // 
            // guna2Panel11
            // 
            this.guna2Panel11.BackColor = System.Drawing.Color.White;
            this.guna2Panel11.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.guna2Panel11.BorderRadius = 4;
            this.guna2Panel11.BorderThickness = 1;
            this.guna2Panel11.Controls.Add(this.guna2HtmlLabel24);
            this.guna2Panel11.Controls.Add(this.guna2HtmlLabel25);
            this.guna2Panel11.Controls.Add(this.guna2HtmlLabel27);
            this.guna2Panel11.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(252)))), ((int)(((byte)(231)))));
            this.guna2Panel11.Location = new System.Drawing.Point(17, 48);
            this.guna2Panel11.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel11.Name = "guna2Panel11";
            this.guna2Panel11.Size = new System.Drawing.Size(523, 65);
            this.guna2Panel11.TabIndex = 2;
            // 
            // guna2HtmlLabel24
            // 
            this.guna2HtmlLabel24.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel24.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.guna2HtmlLabel24.Location = new System.Drawing.Point(329, 20);
            this.guna2HtmlLabel24.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel24.Name = "guna2HtmlLabel24";
            this.guna2HtmlLabel24.Size = new System.Drawing.Size(101, 26);
            this.guna2HtmlLabel24.TabIndex = 15;
            this.guna2HtmlLabel24.Text = "Rp 600.000";
            // 
            // guna2HtmlLabel25
            // 
            this.guna2HtmlLabel25.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel25.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel25.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel25.Location = new System.Drawing.Point(20, 38);
            this.guna2HtmlLabel25.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel25.Name = "guna2HtmlLabel25";
            this.guna2HtmlLabel25.Size = new System.Drawing.Size(131, 24);
            this.guna2HtmlLabel25.TabIndex = 14;
            this.guna2HtmlLabel25.Text = "1 TRANSACTIONS";
            this.guna2HtmlLabel25.UseWaitCursor = true;
            // 
            // guna2HtmlLabel27
            // 
            this.guna2HtmlLabel27.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel27.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel27.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.guna2HtmlLabel27.Location = new System.Drawing.Point(20, 7);
            this.guna2HtmlLabel27.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel27.Name = "guna2HtmlLabel27";
            this.guna2HtmlLabel27.Size = new System.Drawing.Size(93, 26);
            this.guna2HtmlLabel27.TabIndex = 2;
            this.guna2HtmlLabel27.Text = "Confirmed";
            // 
            // guna2HtmlLabel19
            // 
            this.guna2HtmlLabel19.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel19.Font = new System.Drawing.Font("Inter", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel19.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel19.Location = new System.Drawing.Point(17, 12);
            this.guna2HtmlLabel19.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel19.Name = "guna2HtmlLabel19";
            this.guna2HtmlLabel19.Size = new System.Drawing.Size(178, 34);
            this.guna2HtmlLabel19.TabIndex = 0;
            this.guna2HtmlLabel19.Text = "Payment Status";
            // 
            // guna2Panel9
            // 
            this.guna2Panel9.BorderColor = System.Drawing.Color.Black;
            this.guna2Panel9.BorderRadius = 10;
            this.guna2Panel9.BorderThickness = 1;
            this.guna2Panel9.Controls.Add(this.guna2Panel10);
            this.guna2Panel9.Controls.Add(this.guna2HtmlLabel23);
            this.guna2Panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel9.FillColor = System.Drawing.Color.White;
            this.guna2Panel9.Location = new System.Drawing.Point(4, 4);
            this.guna2Panel9.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel9.Name = "guna2Panel9";
            this.guna2Panel9.Size = new System.Drawing.Size(667, 329);
            this.guna2Panel9.TabIndex = 0;
            // 
            // guna2Panel10
            // 
            this.guna2Panel10.AutoScroll = true;
            this.guna2Panel10.BackColor = System.Drawing.Color.White;
            this.guna2Panel10.BorderColor = System.Drawing.Color.Gray;
            this.guna2Panel10.BorderRadius = 10;
            this.guna2Panel10.BorderThickness = 1;
            this.guna2Panel10.Controls.Add(this.guna2HtmlLabel26);
            this.guna2Panel10.Controls.Add(this.guna2HtmlLabel22);
            this.guna2Panel10.Controls.Add(this.guna2HtmlLabel4);
            this.guna2Panel10.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.guna2Panel10.Location = new System.Drawing.Point(20, 71);
            this.guna2Panel10.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel10.Name = "guna2Panel10";
            this.guna2Panel10.Size = new System.Drawing.Size(623, 180);
            this.guna2Panel10.TabIndex = 1;
            // 
            // guna2HtmlLabel26
            // 
            this.guna2HtmlLabel26.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel26.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.guna2HtmlLabel26.Location = new System.Drawing.Point(557, 17);
            this.guna2HtmlLabel26.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel26.Name = "guna2HtmlLabel26";
            this.guna2HtmlLabel26.Size = new System.Drawing.Size(40, 26);
            this.guna2HtmlLabel26.TabIndex = 15;
            this.guna2HtmlLabel26.Text = "Rp 0";
            // 
            // guna2HtmlLabel22
            // 
            this.guna2HtmlLabel22.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel22.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel22.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.guna2HtmlLabel22.Location = new System.Drawing.Point(20, 49);
            this.guna2HtmlLabel22.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel22.Name = "guna2HtmlLabel22";
            this.guna2HtmlLabel22.Size = new System.Drawing.Size(94, 24);
            this.guna2HtmlLabel22.TabIndex = 14;
            this.guna2HtmlLabel22.Text = "1/2 occupied";
            this.guna2HtmlLabel22.UseWaitCursor = true;
            // 
            // guna2HtmlLabel4
            // 
            this.guna2HtmlLabel4.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel4.Font = new System.Drawing.Font("Inter", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel4.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel4.Location = new System.Drawing.Point(20, 17);
            this.guna2HtmlLabel4.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel4.Name = "guna2HtmlLabel4";
            this.guna2HtmlLabel4.Size = new System.Drawing.Size(117, 26);
            this.guna2HtmlLabel4.TabIndex = 2;
            this.guna2HtmlLabel4.Text = "Single Rooms";
            // 
            // guna2HtmlLabel23
            // 
            this.guna2HtmlLabel23.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel23.Font = new System.Drawing.Font("Inter", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel23.ForeColor = System.Drawing.Color.Black;
            this.guna2HtmlLabel23.Location = new System.Drawing.Point(17, 15);
            this.guna2HtmlLabel23.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel23.Name = "guna2HtmlLabel23";
            this.guna2HtmlLabel23.Size = new System.Drawing.Size(233, 34);
            this.guna2HtmlLabel23.TabIndex = 0;
            this.guna2HtmlLabel23.Text = "Revenue Breakdown";
            // 
            // timeReportButton2
            // 
            this.timeReportButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeReportButton2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.timeReportButton2.BorderRadius = 8;
            this.timeReportButton2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.timeReportButton2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.timeReportButton2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.timeReportButton2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.timeReportButton2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))), ((int)(((byte)(128)))));
            this.timeReportButton2.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeReportButton2.ForeColor = System.Drawing.Color.White;
            this.timeReportButton2.Image = global::Kost_SiguraGura.Properties.Resources.calendar;
            this.timeReportButton2.Location = new System.Drawing.Point(932, 54);
            this.timeReportButton2.Margin = new System.Windows.Forms.Padding(4);
            this.timeReportButton2.Name = "timeReportButton2";
            this.timeReportButton2.Size = new System.Drawing.Size(167, 39);
            this.timeReportButton2.TabIndex = 14;
            this.timeReportButton2.Text = "Last 6 Months";
            this.timeReportButton2.Click += new System.EventHandler(this.timeReportButton2_Click);
            // 
            // exportButton1
            // 
            this.exportButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton1.BorderRadius = 8;
            this.exportButton1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.exportButton1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.exportButton1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.exportButton1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.exportButton1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(191)))), ((int)(((byte)(36)))));
            this.exportButton1.Font = new System.Drawing.Font("Inter", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportButton1.ForeColor = System.Drawing.Color.Black;
            this.exportButton1.Image = global::Kost_SiguraGura.Properties.Resources.export_black;
            this.exportButton1.Location = new System.Drawing.Point(1107, 54);
            this.exportButton1.Margin = new System.Windows.Forms.Padding(4);
            this.exportButton1.Name = "exportButton1";
            this.exportButton1.Size = new System.Drawing.Size(159, 39);
            this.exportButton1.TabIndex = 13;
            this.exportButton1.Text = "Export Report";
            this.exportButton1.Click += new System.EventHandler(this.exportButton1_Click);
            // 
            // guna2HtmlLabel34
            // 
            this.guna2HtmlLabel34.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel34.Font = new System.Drawing.Font("Playfair Display", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel34.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.guna2HtmlLabel34.Location = new System.Drawing.Point(31, 25);
            this.guna2HtmlLabel34.Margin = new System.Windows.Forms.Padding(4);
            this.guna2HtmlLabel34.Name = "guna2HtmlLabel34";
            this.guna2HtmlLabel34.Size = new System.Drawing.Size(90, 40);
            this.guna2HtmlLabel34.TabIndex = 15;
            this.guna2HtmlLabel34.Text = "Report";
            this.guna2HtmlLabel34.UseWaitCursor = true;
            // 
            // Report
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.Controls.Add(this.guna2HtmlLabel34);
            this.Controls.Add(this.timeReportButton2);
            this.Controls.Add(this.exportButton1);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblWelcome);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Report";
            this.Size = new System.Drawing.Size(1296, 1542);
            this.Load += new System.EventHandler(this.Report_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.guna2Panel4.ResumeLayout(false);
            this.guna2Panel4.PerformLayout();
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.guna2Panel3.ResumeLayout(false);
            this.guna2Panel3.PerformLayout();
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.guna2Panel7.ResumeLayout(false);
            this.guna2Panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.guna2Panel6.ResumeLayout(false);
            this.guna2Panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.guna2Panel8.ResumeLayout(false);
            this.guna2Panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart4)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.guna2Panel5.ResumeLayout(false);
            this.guna2Panel5.PerformLayout();
            this.guna2Panel13.ResumeLayout(false);
            this.guna2Panel13.PerformLayout();
            this.guna2Panel12.ResumeLayout(false);
            this.guna2Panel12.PerformLayout();
            this.guna2Panel11.ResumeLayout(false);
            this.guna2Panel11.PerformLayout();
            this.guna2Panel9.ResumeLayout(false);
            this.guna2Panel9.PerformLayout();
            this.guna2Panel10.ResumeLayout(false);
            this.guna2Panel10.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblWelcome;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel4;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel18;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel15;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel11;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel TotalRevenueHtmlLabel12;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel17;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel14;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel10;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel16;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel13;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel9;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel7;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel8;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel6;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart4;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel20;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel21;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel19;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel9;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel23;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel10;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel4;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel22;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel26;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel13;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel31;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel32;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel33;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel12;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel28;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel29;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel30;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel11;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel24;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel25;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel27;
        private Guna.UI2.WinForms.Guna2Button exportButton1;
        private Guna.UI2.WinForms.Guna2Button timeReportButton2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel34;
    }
}
