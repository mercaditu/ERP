using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reporting.Views
{
    public partial class SalesByItem : Form
    {
        public SalesByItem()
        {
            InitializeComponent();

            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void SalesByItem_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'salesDB.SalesByDate' table. You can move, or remove it, as needed.
            this.salesByItemTableAdapter.Fill(this.salesDB.SalesByItem, dateTimePicker1.Value, dateTimePicker2.Value);
            this.reportViewer1.RefreshReport();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'salesDB.SalesByDate' table. You can move, or remove it, as needed.
            this.salesByItemTableAdapter.Fill(this.salesDB.SalesByItem, dateTimePicker1.Value, dateTimePicker2.Value);
            this.reportViewer1.RefreshReport();
        }
    }
}
