namespace Reporting.Views
{
    partial class SalesByItem
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource3 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.salesDB = new Reporting.Data.SalesDB();
            this.salesByItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.salesByItemTableAdapter = new Reporting.Data.SalesDBTableAdapters.SalesByItemTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.salesDB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesByItemBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(539, 5);
            this.button1.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(220, 47);
            this.button1.TabIndex = 14;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dateTimePicker2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(277, 5);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(248, 47);
            this.dateTimePicker2.TabIndex = 13;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(17, 5);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(246, 47);
            this.dateTimePicker1.TabIndex = 12;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            reportDataSource3.Name = "SalesByItem";
            reportDataSource3.Value = this.salesByItemBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Reporting.Reports.SalesByItem.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(-2, 69);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(779, 454);
            this.reportViewer1.TabIndex = 11;
            // 
            // salesDB
            // 
            this.salesDB.DataSetName = "SalesDB";
            this.salesDB.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // salesByItemBindingSource
            // 
            this.salesByItemBindingSource.DataMember = "SalesByItem";
            this.salesByItemBindingSource.DataSource = this.salesDB;
            // 
            // salesByItemTableAdapter
            // 
            this.salesByItemTableAdapter.ClearBeforeFill = true;
            // 
            // SalesByItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 529);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.reportViewer1);
            this.Name = "SalesByItem";
            this.Text = "SalesByItem";
            this.Load += new System.EventHandler(this.SalesByItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.salesDB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesByItemBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource salesByItemBindingSource;
        private Data.SalesDB salesDB;
        private Data.SalesDBTableAdapters.SalesByItemTableAdapter salesByItemTableAdapter;
    }
}