using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class range : UserControl
    {
        CollectionViewSource _document_rangeViewSource = null;
        public CollectionViewSource document_rangeViewSource { get { return _document_rangeViewSource; } set { _document_rangeViewSource = value; } }

        private dbContext _entity = null;
        public dbContext entity { get { return _entity; } set { _entity = value; } }

        public range()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = document_rangeViewSource;

                CollectionViewSource app_documentViewSource = (CollectionViewSource)FindResource("app_documentViewSource");
                app_documentViewSource.Source = entity.db.app_document.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

                CollectionViewSource app_branchViewSource = (CollectionViewSource)FindResource("app_branchViewSource");
                app_branchViewSource.Source = entity.db.app_branch.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                CollectionViewSource app_terminalViewSource = (CollectionViewSource)FindResource("app_terminalViewSource");
                app_terminalViewSource.Source = entity.db.app_terminal.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

                List<string> listOfSysPrinters = new List<string>();
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    listOfSysPrinters.Add(printer);
                }
                cbxPrinters.ItemsSource = listOfSysPrinters;
            }
        }

        private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            app_document app_document = (app_document)cbxDocument.SelectedItem;
            if (app_document != null)
            {
                if (app_document.filterby_branch)
                {
                    stackBranch.Visibility = Visibility.Visible;
                }
                else
                {
                    stackBranch.Visibility = Visibility.Collapsed;
                }

                if (app_document.filterby_tearminal)
                {
                    stackTerminal.Visibility = Visibility.Visible;
                }
                else
                {
                    stackTerminal.Visibility = Visibility.Collapsed;
                }
            }
        }

        #region Template
        private void Tag_MouseDown(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            tbxRangeTemplate.AppendText(lbl.Tag.ToString());
        }

        private void tbxTemplate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxRangeTemplate.Text.Contains("#Range") != true)
            {
                //tbxRangeTemplate.AppendText("#Range");
                tbxRangeTemplate.BorderBrush = Brushes.Purple;
            }
            else
            {
                tbxRangeTemplate.BorderBrush = Brushes.Black;
            }
        }

        #endregion

        #region CURD
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.db.SaveChanges();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    app_document_range app_document_range = document_rangeViewSource.View.CurrentItem as app_document_range;
                    app_document_range.is_active = false;
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _entity.CancelChanges();
                document_rangeViewSource.View.Refresh();
                Grid parentGrid = (Grid)Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
