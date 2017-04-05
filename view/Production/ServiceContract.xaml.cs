using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Production
{
    public partial class ServiceContract : Page, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion INotifyPropertyChanged

        private CollectionViewSource contactViewSource, production_service_accountViewSource;

        public ServiceContract()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contact contact = contactViewSource.View.CurrentItem as contact;
            if (contact != null && production_service_accountViewSource != null)
            {
                production_service_accountViewSource.View.Filter = i =>
                {
                    payment_schedual payment_schedual = i as payment_schedual;
                    if (payment_schedual.id_contact == contact.id_contact)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
            else
            {
                contactViewSource.View.Filter = null;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load_Schedual();
        }

        private async void load_Schedual()
        {
            production_service_accountViewSource = (CollectionViewSource)FindResource("production_service_accountViewSource");
            if (production_service_accountViewSource != null)
            {
                contactViewSource = FindResource("contactViewSource") as CollectionViewSource;
                List<contact> contactLIST = new List<contact>();

                using (db db = new db())
                {
                    production_service_accountViewSource.Source = await db.production_service_account
                                        .Where(x => x.id_company == CurrentSession.Id_Company
                                           && (x.id_purchase_order_detail > 0)
                                           && (x.credit - (x.child.Count() > 0 ? x.child.Sum(y => y.debit) : 0)) > 0)
                                           .Include(y => y.purchase_order_detail.purchase_order)
                                           .Include(z => z.contact)
                                           .OrderByDescending(x => x.trans_date)
                                        .ToListAsync();

                    foreach (production_service_account service_account in db.production_service_account.Local.OrderBy(x => x.contact.name).ToList())
                    {
                        if (contactLIST.Contains(service_account.contact) == false)
                        {
                            contact contact = new contact();
                            contact = service_account.contact;
                            contactLIST.Add(contact);
                        }
                    }
                }

                contactViewSource.Source = contactLIST;
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    contactViewSource.View.Filter = i =>
                    {
                        contact contact = i as contact;
                        string name = "";
                        string code = "";
                        string gov_code = "";

                        if (contact.name != null)
                        {
                            name = contact.name.ToLower();
                        }

                        if (contact.code != null)
                        {
                            code = contact.code.ToLower();
                        }

                        if (contact.gov_code != null)
                        {
                            gov_code = contact.gov_code.ToLower();
                        }

                        if (name.Contains(query.ToLower())
                            || code.Contains(query.ToLower())
                            || gov_code.Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    contactViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolbar.msgError(ex);
            }
        }
    }
}