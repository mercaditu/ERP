using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cognitivo.Sales
{
    public partial class PointOfSale : Page
    {
        entity.SalesInvoiceDB SalesInvoiceDB = new entity.SalesInvoiceDB();

        public PointOfSale()
        {
            InitializeComponent();
        }

        #region Buttons

        private void btnClient_Click(object sender, EventArgs e)
        {
            tabContact.IsSelected = true;
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            tabSales.IsSelected = true;
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            tabPayment.IsSelected = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Run approve code here.
        }

        #endregion

        #region SmartBox Selection

        private void sbxContact_Select(object sender, RoutedEventArgs e)
        {

        }

        private void sbxItem_Select(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
