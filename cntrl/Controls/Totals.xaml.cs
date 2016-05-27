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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for Totals.xaml
    /// </summary>
    public partial class Totals : UserControl
    {
        public static readonly DependencyProperty GrandTotalProperty = DependencyProperty.Register("GrandTotal", typeof(decimal), typeof(Totals));
        public decimal GrandTotal
        {
            get { return (decimal)GetValue(GrandTotalProperty); }
            set { SetValue(GrandTotalProperty, value); }
        }

        public static readonly DependencyProperty DiscountPercentageProperty = DependencyProperty.Register("DiscountPercentage", typeof(decimal), typeof(Totals));
        public decimal DiscountPercentage
        {
            get { return (decimal)GetValue(DiscountPercentageProperty); }
            set { SetValue(DiscountPercentageProperty, value); }
        }

        public static readonly DependencyProperty DiscountValueProperty = DependencyProperty.Register("DiscountValue", typeof(decimal), typeof(Totals));
        public decimal DiscountValue
        {
            get { return (decimal)GetValue(DiscountValueProperty); }
            set { SetValue(DiscountValueProperty, value); }
        }

        public static readonly DependencyProperty CurrencyProperty = DependencyProperty.Register("Currency", typeof(string), typeof(Totals));
        public string Currency
        {
            get { return (string)GetValue(CurrencyProperty); }
            set { SetValue(CurrencyProperty, value); }
        }

        public Totals()
        {
            InitializeComponent();
        }

        private void lblInformation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void lblTotal_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            Storyboard Animate = (Storyboard)FindResource("TextChanged");
            Animate.Begin(this); 
        }
    }
}
