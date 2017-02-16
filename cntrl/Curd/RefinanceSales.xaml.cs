using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cntrl.Curd
{
    public partial class RefinanceSales : UserControl
    {
        private PaymentDB _entity = new PaymentDB();

        public sales_invoice sales_invoice { get; set; }

        public RefinanceSales()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    List<payment_schedual> payment_schedualList = _entity.payment_schedual
                   .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company && x.id_contact == sales_invoice.id_contact
                       && (x.id_sales_invoice > 0 || x.id_sales_order > 0) && x.id_note == null
                       && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                       .OrderBy(x => x.expire_date).ToList();
                    stackMain.DataContext = payment_schedualList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            _entity.SaveChanges();
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _entity.CancelAllChanges();

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}