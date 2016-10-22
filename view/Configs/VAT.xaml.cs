using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    public partial class VAT : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_vatViewSource;
        Cognitivo.Properties.Settings _pref_Cognitivo = new Cognitivo.Properties.Settings();
       // entity.Properties.Settings _entity = new entity.Properties.Settings();

        public VAT()
        { InitializeComponent(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app_vatViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_vatViewSource")));
            entity.db.app_vat.Where(a=>a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_vatViewSource.Source = entity.db.app_vat.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.vat objVat = new cntrl.vat();
            app_vat app_vat = new app_vat();
            entity.db.app_vat.Add(app_vat);
            app_vatViewSource.View.MoveCurrentToLast();
            objVat.app_vatViewSource = app_vatViewSource;
            objVat.entity = entity;
            crud_modal.Children.Add(objVat);
        }

        private void pnl_VAT_linkEdit_Click(object sender, int intVatId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.vat objVat = new cntrl.vat();
            app_vatViewSource.View.MoveCurrentTo(entity.db.app_vat.Where(x => x.id_vat == intVatId).FirstOrDefault());
            objVat.app_vatViewSource = app_vatViewSource;
            objVat.entity = entity;
            crud_modal.Children.Add(objVat);
        }
    }
}
