using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognitivo.Data;

namespace Reporting.Views
{
    public partial class SalesByDate : Form
    {
        public SalesByDate()
        {
            InitializeComponent();

            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void SalesByDate_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'salesDB.SalesByDate' table. You can move, or remove it, as needed.
            Cognitivo.Data.SalesDBTableAdapters.Fill(, dateTimePicker1.Value, dateTimePicker2.Value);
            this.reportViewer1.RefreshReport();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Cognitivo.Data.salesByDateTableAdapter.Fill(this.salesDB.SalesByDate, dateTimePicker1.Value, dateTimePicker2.Value);
            Cognitivo.Data.reportViewer1.RefreshReport();
        }
    }
}
