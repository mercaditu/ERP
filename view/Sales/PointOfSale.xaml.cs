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
using System.Collections.ObjectModel;
using System.Data.Entity;
using entity;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Data.Entity.Validation;
using entity.Brillo;

namespace Cognitivo.Sales
{
    /// <summary>
    /// Interaction logic for PointOfSale.xaml
    /// </summary>

    public partial class PointOfSale : Page
    {
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        entity.dbContext entity = new entity.dbContext();
        ObservableCollection<cls_Item> obj_item = new ObservableCollection<cls_Item>();
        CollectionViewSource itemViewSource;
        CollectionViewSource contactViewSource;
       // item_location _item_location = new item_location();
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        CollectionViewSource contractViewSource, conditionViewSource, currencyfxViewSource;
        public PointOfSale()
        {
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //  itemComboBox.SelectedIndex = -1;

            try
            {
                CollectionViewSource currencyViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_currencyViewSource");
                entity.db.app_currency.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                currencyViewSource.Source = entity.db.app_currency.Local;

                CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
                entity.db.payment_type.Where(a=>a.is_active == true).Load();
                payment_typeViewSource.Source = entity.db.payment_type.Local;

                currencyfxViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_currencyfxViewSource");
                entity.db.app_currencyfx.Include("app_currency").Where(a => a.is_active == true).Load();
                currencyfxViewSource.Source = entity.db.app_currencyfx.Local;

                contractViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("contractViewSource");
                contractViewSource.Source = entity.db.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();

                conditionViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("conditionViewSource");
                conditionViewSource.Source = entity.db.app_condition.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();

                CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
                entity.db.app_account.Where(a=>a.is_active == true && a.id_company == _entity.company_ID).Load();
                app_accountViewSource.Source = entity.db.app_account.Local;

                contactViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("contactViewSource")));
                entity.db.contacts.Where(a => a.is_active == true && a.is_customer == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                contactViewSource.Source = entity.db.contacts.Local;

                //Fetch Data
                itemViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("itemViewSource");
                entity.db.items.Include(x => x.item_tag_detail).Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
                itemViewSource.Source = entity.db.items.Local;

                //itemViewSource.Source = entity.db.items
                //    .Join(entity.db.item_tag_detail, ad => ad.id_item, cfx => cfx.id_item
                //    , (ad, cfx) => new { id_item = ad.id_item, item_description = ad.name, code = ad.code, tag = cfx.item_tag.name, unit_cost = ad.unit_cost })
                //    .Select(g => new
                //    {
                //        id_item = g.id_item,
                //        name = g.item_description,
                //        code = g.code,
                //        tag = g.tag,
                //        unit_cost = g.unit_cost
                //    }).ToList();
              
                txtBlockTotal.Text = "0";
            }
            catch { }


        }

        private void itemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    itemViewSource.View.Filter = item =>
                    {
                        dynamic objItem = item;
                        int id = Convert.ToInt32(((item)itemComboBox.Data).id_item);
                        if (objItem.id_item == id)
                        {

                            if (obj_item.Where(x => x.ItemId == objItem.id_item).Count() > 0)
                            {
                                cls_Item PropItems = obj_item.Where(x => x.ItemId == objItem.id_item).FirstOrDefault();
                                PropItems.ItemId = objItem.id_item;
                                PropItems.Quantity = PropItems.Quantity + 1;
                                PropItems.Code = objItem.code;
                                PropItems.Product = objItem.name;
                                PropItems.Price = Convert.ToDecimal(objItem.unit_cost);
                                PropItems.SubTotal = PropItems.Quantity * PropItems.Price;
                            }
                            else
                            {
                                cls_Item PropItems = new cls_Item();
                                PropItems.ItemId = objItem.id_item;
                                PropItems.Quantity = 1;
                                PropItems.Code = objItem.code;
                                PropItems.Product = objItem.name;
                                PropItems.Price = Convert.ToDecimal(objItem.unit_cost);
                                PropItems.SubTotal = PropItems.Quantity * PropItems.Price;
                                obj_item.Add(PropItems);
                                dgItemDesc.ItemsSource = obj_item;
                            }

                            txtBlockTotal.Text = Convert.ToString(obj_item.Sum(x => x.SubTotal));
                            return false;
                        }
                        else
                            return false;
                    };
                    itemViewSource.View.Filter = null;
                }

            }
            catch { }
        }

        private void dgItemDesc_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            cls_Item obj_item = e.Row.Item as cls_Item;
            obj_item.SubTotal = obj_item.Quantity * obj_item.Price;
            ObservableCollection<cls_Item> obj_cls_item = (ObservableCollection<cls_Item>)dgItemDesc.ItemsSource;
            txtBlockTotal.Text = Convert.ToString(obj_cls_item.Sum(x => x.SubTotal));
        }

        //private void itemComboBoxTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    itemComboBox.IsDropDownOpen = true;
        //    if (itemComboBox.Text != "")
        //    {
        //        itemViewSource.View.Filter = item =>
        //        {
        //            dynamic objItem = item;

        //            if (Convert.ToString(objItem.code).ToLower().Contains(itemComboBox.Text.ToLower()) || Convert.ToString(objItem.name).ToLower().Contains(itemComboBox.Text.ToLower()) || Convert.ToString(objItem.tag).ToLower().Contains(itemComboBox.Text.ToLower()))
        //            {

        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        };
        //    }
        //    else
        //        itemViewSource.View.Filter = null;
        //}


        //private void contactComboBoxTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    contactComboBox.IsDropDownOpen = true;
        //    if (contactComboBox.Text != "")
        //    {
        //        contactViewSource.View.Filter = contacts =>
        //        {
        //            contact objItem = (contact)contacts;

        //            if (Convert.ToString(objItem.code).ToLower().Contains(contactComboBox.Text.ToLower()) || Convert.ToString(objItem.name).ToLower().Contains(contactComboBox.Text.ToLower()) || Convert.ToString(objItem.gov_code).ToLower().Contains(contactComboBox.Text.ToLower()))
        //            {

        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        };
        //    }
        //    else
        //        contactViewSource.View.Filter = null;
        //}
        #region curd extra
        private void btnNewContact(object sender, MouseButtonEventArgs e)
        {
            //AddContact            
            //crud_modal.Visibility = System.Windows.Visibility.Visible;
            //cntrl.Curd.contact contact = new cntrl.Curd.contact();
            //contact.contactViewSource = contactViewSource;
            //contact._entity = entity;
            //contact.module = app_module.Applications.SalesInvoice;
            //crud_modal.Children.Add(contact);
        }
        #endregion

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            int id_module = 1;
            contact contacts =(contact)contactComboBox.Data;
            sales_invoice _sales_invoice = new sales_invoice();
            _sales_invoice.id_contact = contacts.id_contact;
            _sales_invoice.id_condition = Cognitivo.Sales.POS.Default.id_condition;
            _sales_invoice.id_contract = Cognitivo.Sales.POS.Default.id_contract;
            _sales_invoice.id_currencyfx = Cognitivo.Sales.POS.Default.id_currencyfx;

            for (int i = 0; i < dgItemDesc.Items.Count - 1; i++)
            {
                cls_Item item = (cls_Item)dgItemDesc.Items[i];
                sales_invoice_detail _sales_invoice_detail = new sales_invoice_detail();
                //_sales_invoice_detail.id_contact = contacts.id_contact;
                _sales_invoice_detail.id_item = item.ItemId;
                _sales_invoice_detail.quantity = item.Quantity;
                _sales_invoice_detail.SubTotal = item.Price;
                //_sales_invoice_detail.sub_total_wo_vat = item.SubTotal;
                //_sales_invoice_detail.id_select_branch = _setting.branch_ID;
                //if (_sales_invoice_detail.id_select_branch > 0)
                //{
                //    _sales_invoice_detail.id_location = _item_location.getlocationsalesinvoice(item.ItemId, _sales_invoice_detail.id_select_branch, 0);
                //}
                //else
                //{
                //    _sales_invoice_detail.id_location = _item_location.getlocationsalesinvoice(item.ItemId, _sales_invoice_detail.id_select_branch, 0);
                //}
                _sales_invoice.sales_invoice_detail.Add(_sales_invoice_detail);
            }
            entity.db.sales_invoice.Add(_sales_invoice);
            entity.db.SaveChanges();
            try
            {
                //entity.Brillo.Start obj_business = new entity.Brillo.Start();
                int id_invoice = Convert.ToInt32(_sales_invoice.id_sales_invoice);

                //obj_business.Update((short)id_module, id_invoice);

            }
            catch { }

            entity.Properties.Settings enittysetting = new entity.Properties.Settings();

            payment _payment = new payment();
            _payment.id_contact = contacts.id_contact;

            payment_detail payment_detail = new payment_detail();
            app_account_detail appaccountdetail = new app_account_detail();
            payment_detail.id_account = Cognitivo.Sales.POS.Default.id_account;
            payment_detail.id_currencyfx = Cognitivo.Sales.POS.Default.id_currencyfx;
           // payment_detail.id_payment_schedual = entity.db.payment_schedual.Where(x => x.id_sales_invoice == _sales_invoice.id_sales_invoice).FirstOrDefault().id_payment_schedual;
            payment_detail.id_payment_type = (int)cbxPamentType.SelectedValue;
            payment_detail.id_purchase_return = 0;
            payment_detail.id_sales_return = 0;
            payment_detail.id_user = CurrentSession.Id_User;
            payment_detail.value = Convert.ToDecimal(txtBlockTotal.Text); ;
            appaccountdetail.id_account = Cognitivo.Sales.POS.Default.id_account; ;
            appaccountdetail.id_currencyfx = Cognitivo.Sales.POS.Default.id_currencyfx;
            appaccountdetail.credit = Convert.ToDecimal(txtBlockTotal.Text);
            appaccountdetail.debit = 0;
            appaccountdetail.id_payment_type = (int)cbxPamentType.SelectedValue;
            _payment.payment_detail.Add(payment_detail);
            entity.db.app_account_detail.Add(appaccountdetail);
            entity.db.payments.Add(_payment);
            entity.db.SaveChanges();

            //Configs.PrintDocument PrintDocument = new Configs.PrintDocument();
            //string name = entity.db.app_document.Where(x => (int)x.id_module == id_module).FirstOrDefault().designer_name;
            //PrintDocument.filename = "Documents\\" + name;
            //PrintDocument.id = _sales_invoice.id_sales_invoice;
            //PrintDocument.Show();

        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                contractViewSource.View.Filter = i =>
                {
                    app_contract objContract = (app_contract)i;
                    if (objContract.id_condition == app_condition.id_condition)
                    { return true; }
                    else
                    { return false; }
                };
                cbxContract.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Cognitivo.Sales.POS.Default.Save();
            setting.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            setting.Visibility = System.Windows.Visibility.Visible;
        }

    }

    public class cls_Item : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private int ItemIdValue = 0;
        private int QuantityValue = 0;
        private string CodeValue = String.Empty;
        private string ProductValue = String.Empty;
        private decimal PriceValue = 0;
        private decimal SubTotalValue = 0;
        public int ItemId
        {
            get
            {
                return this.ItemIdValue;
            }

            set
            {
                if (value != this.ItemIdValue)
                {
                    this.ItemIdValue = value;
                    OnPropertyChanged("ItemId");
                }
            }
        }
        public int Quantity
        {
            get
            {
                return this.QuantityValue;
            }

            set
            {
                if (value != this.QuantityValue)
                {
                    this.QuantityValue = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }
        public string Code
        {
            get
            {
                return this.CodeValue;
            }

            set
            {
                if (value != this.CodeValue)
                {
                    this.CodeValue = value;
                    OnPropertyChanged("Code");
                }
            }
        }
        public string Product
        {
            get
            {
                return this.ProductValue;
            }

            set
            {
                if (value != this.ProductValue)
                {
                    this.ProductValue = value;
                    OnPropertyChanged("Product");
                }
            }
        }
        public decimal Price
        {
            get
            {
                return this.PriceValue;
            }

            set
            {
                if (value != this.PriceValue)
                {
                    this.PriceValue = value;
                    OnPropertyChanged("Price");
                }
            }
        }
        public decimal SubTotal
        {
            get
            {
                return this.SubTotalValue;
            }

            set
            {
                if (value != this.SubTotalValue)
                {
                    this.SubTotalValue = value;
                    OnPropertyChanged("SubTotal");
                }
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
