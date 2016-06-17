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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SalesByDate));
            this.salesByDateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.salesDBBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.salesDB = new Cognitivo.Data.SalesDB();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.salesByDateTableAdapter = new Cognitivo.Data.SalesDBTableAdapters.SalesByDateTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.salesByDateBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDBBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDB)).BeginInit();
            this.SuspendLayout();
            // 
            // salesByDateBindingSource
            // 
            this.salesByDateBindingSource.DataMember = "SalesByDate";
            this.salesByDateBindingSource.DataSource = this.salesDBBindingSource;
            // 
            // salesDBBindingSource
            // 
            this.salesDBBindingSource.DataSource = this.salesDB;
            this.salesDBBindingSource.Position = 0;
            // 
            // salesDB
            // 
            this.salesDB.DataSetName = "SalesDB";
            this.salesDB.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Reporting.Reports.SalesByDate.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 70);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(4);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(773, 461);
            this.reportViewer1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(541, 14);
            this.button1.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(271, 47);
            this.button1.TabIndex = 6;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dateTimePicker2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(283, 14);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(244, 47);
            this.dateTimePicker2.TabIndex = 5;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(16, 14);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(253, 47);
            this.dateTimePicker1.TabIndex = 4;
            // 
            // salesByDateTableAdapter
            // 
            this.salesByDateTableAdapter.ClearBeforeFill = true;
            // 
            // SalesByDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 529);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.reportViewer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SalesByDate";
            this.Text = "Sales By Date";
            this.Load += new System.EventHandler(this.SalesByDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.salesByDateBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDBBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesDB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource salesByDateBindingSource;
        private System.Windows.Forms.BindingSource salesDBBindingSource;
        private Cognitivo.Data.SalesDB salesDB;
        private Cognitivo.Data.SalesDBTableAdapters.SalesByDateTableAdapter salesByDateTableAdapter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
    }
}