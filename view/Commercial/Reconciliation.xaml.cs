using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Commercial
{
    public partial class Reconciliation : Page, INotifyPropertyChanged
    {
        private CollectionViewSource app_accountViewSource;
        private CollectionViewSource app_accountapp_account_detailViewSource;
        private CollectionViewSource app_accountapp_account_detailApproveViewSource;
        private CollectionViewSource app_accountapp_account_detailAnulledViewSource;
        private db db = new db();
        public decimal Balance { get; set; }
        public Reconciliation()
        {
            InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_accountViewSource = ((CollectionViewSource)(FindResource("app_accountViewSource")));
            db.app_account.Where(x => x.id_company == CurrentSession.Id_Company && x.id_account_type == app_account.app_account_type.Bank).Load();
            app_accountViewSource.Source = db.app_account.Local;
            app_accountapp_account_detailViewSource = ((CollectionViewSource)(FindResource("app_accountapp_account_detailViewSource")));
            app_accountapp_account_detailApproveViewSource = ((CollectionViewSource)(FindResource("app_accountapp_account_detailApproveViewSource")));
            app_accountapp_account_detailAnulledViewSource = ((CollectionViewSource)(FindResource("app_accountapp_account_detailAnulledViewSource")));
            filter_approve();
            filter_pending();
            filter_annulled();
        }

        public void filter_pending()
        {
            try
            {
                if (app_accountapp_account_detailViewSource != null)
                {
                    if (app_accountapp_account_detailViewSource.View != null)
                    {
                        app_accountapp_account_detailViewSource.View.Filter = i =>
                        {
                            app_account_detail _app_account_detail = (app_account_detail)i;
                            if (_app_account_detail.status==Status.Documents_General.Pending)
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
            catch { }
        }


        public void filter_approve()
        {
            try
            {
                if (app_accountapp_account_detailApproveViewSource != null)
                {
                    if (app_accountapp_account_detailApproveViewSource.View != null)
                    {
                       
                        app_accountapp_account_detailApproveViewSource.View.Filter = i =>
                        {
                            app_account_detail _app_account_detail = (app_account_detail)i;
                            if (_app_account_detail.status == Status.Documents_General.Approved)
                               
                                return true;
                            else
                                return false;
                        };
                        List<app_account_detail> app_account_detailList = app_accountapp_account_detailApproveViewSource.View.OfType<app_account_detail>().ToList();
                        Balance = Math.Round(app_account_detailList.Sum(x => x.credit - x.debit),2);
                        RaisePropertyChanged("Balance");
                    }
                }
            }
            catch (Exception ex)
            {
              
            }
        }
        public void filter_annulled()
        {
            try
            {
                if (app_accountapp_account_detailAnulledViewSource != null)
                {
                    if (app_accountapp_account_detailAnulledViewSource.View != null)
                    {

                        app_accountapp_account_detailAnulledViewSource.View.Filter = i =>
                        {
                            app_account_detail _app_account_detail = (app_account_detail)i;
                            if (_app_account_detail.status == Status.Documents_General.Annulled)

                                return true;
                            else
                                return false;
                        };
                      
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void PendingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as app_account_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void PendingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            app_account_detail app_account_detail = app_accountapp_account_detailViewSource.View.CurrentItem as app_account_detail;
            app_account_detail.status = Status.Documents_General.Pending;

            app_accountapp_account_detailViewSource.View.Refresh();
            app_accountapp_account_detailApproveViewSource.View.Refresh();
            filter_approve();
            filter_pending();
            filter_annulled();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
            db.SaveChanges();
            app_account.State = EntityState.Unchanged;
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
            if (app_account != null)
            {
                app_account.State = EntityState.Modified;
            }
        }

        private void dgvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
            if (app_account != null)
            {
                app_account.State = EntityState.Modified;
            }
            filter_approve();
            filter_pending();
            filter_annulled();
        }

        private void toolBar_btnApprove_Click(object sender, RoutedEventArgs e)
        {
			dgvAccountDetail.CommitEdit();
			app_account app_account = app_accountViewSource.View.CurrentItem as app_account;

            if (app_account != null)
            {
                foreach (app_account_detail app_account_detail in app_account.app_account_detail.Where(x => x.IsSelected))
                {
                    app_account_detail.status = Status.Documents_General.Approved;
                    app_account_detail.RaisePropertyChanged("status");
                }

               
                db.SaveChanges();
             
            }
            dgvAccountDetail.CancelEdit();
         

              app_accountapp_account_detailViewSource.View.Refresh();
             app_accountapp_account_detailApproveViewSource.View.Refresh();
            app_accountapp_account_detailAnulledViewSource.View.Refresh();
            filter_approve();
            filter_pending();
            filter_annulled();
            //   app_accountapp_account_detailViewSource.View.Refresh();
        }

        private void toolBar_btnAnull_Click(object sender, RoutedEventArgs e)
        {
            app_account app_account = app_accountViewSource.View.CurrentItem as app_account;

            if (app_account != null)
            {
                foreach (app_account_detail app_account_detail in app_account.app_account_detail.Where(x => x.IsSelected))
                {
                    app_account_detail.status = Status.Documents_General.Annulled;
                    app_account_detail.RaisePropertyChanged("status");
                }

              
                db.SaveChanges();
                app_accountapp_account_detailViewSource.View.Refresh();
                app_accountapp_account_detailApproveViewSource.View.Refresh();
                filter_approve();
                filter_pending();
                filter_annulled();
                //  app_accountapp_account_detailViewSource.View.Refresh();
            }
        }
    }
}