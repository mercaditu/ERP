namespace Reporting.Views
{
    partial class SalesByDate
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.salesDB = new Reporting.Data.SalesDB();
            this.salesDBBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.salesByDateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.salesByDateTableAdapter = new Reporting.Data.SalesDBTableAdapters.SalesByDateTableAdapter();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.salesDB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDBBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesByDateBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Bottom;
            reportDataSource3.Name = "SalesByDate";
            reportDataSource3.Value = this.salesByDateBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Reporting.Reports.SalesByDate.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 83);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(1234, 495);
            this.reportViewer1.TabIndex = 0;
            // 
            // salesDB
            // 
            this.salesDB.DataSetName = "SalesDB";
            this.salesDB.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // salesDBBindingSource
            // 
            this.salesDBBindingSource.DataSource = this.salesDB;
            this.salesDBBindingSource.Position = 0;
            // 
            // salesByDateBindingSource
            // 
            this.salesByDateBindingSource.DataMember = "SalesByDate";
            this.salesByDateBindingSource.DataSource = this.salesDBBindingSource;
            // 
            // salesByDateTableAdapter
            // 
            this.salesByDateTableAdapter.ClearBeforeFill = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(531, 22);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(191, 47);
            this.button1.TabIndex = 6;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(277, 22);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(244, 47);
            this.dateTimePicker2.TabIndex = 5;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(11, 22);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(256, 47);
            this.dateTimePicker1.TabIndex = 4;
            // 
            // SalesByDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 578);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.reportViewer1);
            this.Name = "SalesByDate";
            this.Text = "SalesByDate";
            this.Load += new System.EventHandler(this.SalesByDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.salesDB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDBBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesByDateBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource salesByDateBindingSource;
        private System.Windows.Forms.BindingSource salesDBBindingSource;
        private Data.SalesDB salesDB;
        private Data.SalesDBTableAdapters.SalesByDateTableAdapter salesByDateTableAdapter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
    }
}