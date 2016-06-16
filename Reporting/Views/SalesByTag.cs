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
    public partial class SalesByTag : Form
    {
        public SalesByTag()
        {
            InitializeComponent();
        }

        private void SalesByTag_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'salesDB.SalesByDate' table. You can move, or remove it, as needed.
            this.salesByTagTableAdapter.Fill(this.salesDB.SalesByTag, dateTimePicker1.Value, dateTimePicker2.Value);
            this.reportViewer1.RefreshReport();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'salesDB.SalesByDate' table. You can move, or remove it, as needed.
            this.salesByTagTableAdapter.Fill(this.salesDB.SalesByTag, dateTimePicker1.Value, dateTimePicker2.Value);
            this.reportViewer1.RefreshReport();
        }
    }
}
