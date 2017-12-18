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
using entity;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for PrintMovemen.xaml
    /// </summary>
    public partial class PrintMovement : Page
    {
        db db = new db();
        public int SalesID { get; set; }
        public PrintMovement()
        {
            InitializeComponent();
        }
      

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            stackFlow.Children.Clear();

            sales_invoice sales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == SalesID).FirstOrDefault();
            if (sales_invoice != null)
            {
                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail.ToList())
                {
                    foreach (item_movement item_movement in sales_invoice_detail.item_movement.ToList())
                    {

                        cntrl.Controls.MovementPrint _MovementPrint = new cntrl.Controls.MovementPrint();
                        if (item_movement.parent!=null)
                        {
                            _MovementPrint.ParentID = item_movement.parent.id_movement;
                        }
                  

                        stackFlow.Children.Add(_MovementPrint);
                    }
                }

            }
        }
    }

}
