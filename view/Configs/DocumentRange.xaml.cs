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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    public partial class DocumentRange : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource document_rangeViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public DocumentRange()
        { 
            InitializeComponent(); 
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                document_rangeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_document_rangeViewSource")));
                entity.db.app_document_range.Include("app_document").Where(a=>a.id_company == _entity.company_ID).OrderByDescending(a => a.is_active).Load();
                document_rangeViewSource.Source = entity.db.app_document_range.Local;
            }
            catch { }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.range objRange = new cntrl.range();
            app_document_range app_document_range = new app_document_range();
            entity.db.app_document_range.Add(app_document_range);
            document_rangeViewSource.View.MoveCurrentToLast();
            objRange.document_rangeViewSource = document_rangeViewSource;
            objRange.entity = entity;
            crud_modal.Children.Add(objRange);
        }

        private void pnl_DocumentRange_Click(object sender, int idRange)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.range objRange = new cntrl.range();
            document_rangeViewSource.View.MoveCurrentTo(entity.db.app_document_range.Where(x => x.id_range == idRange).FirstOrDefault());
            objRange.document_rangeViewSource = document_rangeViewSource;
            objRange.entity = entity;
            crud_modal.Children.Add(objRange);
        }
    }
}
