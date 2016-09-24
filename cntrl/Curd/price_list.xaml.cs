using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;
using System.Data.Entity;
namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for price_list.xaml
    /// </summary>
    public partial class price_list : UserControl
    {
        entity.dbContext mydb = new entity.dbContext();
        CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        public object curMainObject { get; set; }

        public Class.clsCommon.Mode operationMode { get; set; }

        private entity.item_price_list _price_listobject = null;
        public entity.item_price_list price_listobject { get { return _price_listobject; } set { _price_listobject = value; } }

        CollectionViewSource _item_price_listViewSource = null;
        public CollectionViewSource item_price_listViewSource { get { return _item_price_listViewSource; } set { _item_price_listViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public price_list()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (!isExternalCall)
                {
                    stackMain.DataContext = item_price_listViewSource;
                }
                else
                {
                    MainViewSource.View.MoveCurrentTo(curMainObject);
                    if (operationMode == Class.clsCommon.Mode.Add)
                    {
                        entity.item_price_list newitem_price_list = new entity.item_price_list();
                        mydb.db.item_price_list.Add(newitem_price_list);
                        myViewSource.Source = mydb.db.item_price_list.Local;
                        myViewSource.View.Refresh();
                        myViewSource.View.MoveCurrentTo(newitem_price_list);
                        stackMain.DataContext = myViewSource;
                    }
                    else if (operationMode == Class.clsCommon.Mode.Edit)
                    {
                        item_price_listViewSource.View.MoveCurrentTo(_price_listobject);
                        stackMain.DataContext = item_price_listViewSource;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            _entity.SaveChanges();
                            btnCancel_Click(sender, e);
                        }
                    }
                    else
                    {
                        IEnumerable<DbEntityValidationResult> validationresult = mydb.db.GetValidationErrors();
                        if (validationresult.Count() == 0)
                        {
                            if (operationMode == Class.clsCommon.Mode.Add)
                            {
                                mydb.SaveChanges();
                                entity.item_price_list item_price_list = myViewSource.View.CurrentItem as entity.item_price_list;
                                mydb.db.Entry(item_price_list).State = EntityState.Detached;
                                _entity.db.item_price_list.Attach(item_price_list);
                                item_price_listViewSource.View.Refresh();
                                item_price_listViewSource.View.MoveCurrentTo(item_price_list);
                                MainViewSource.View.Refresh();
                                MainViewSource.View.MoveCurrentTo(curMainObject);
                                btnCancel_Click(sender, e);
                            }
                            else if (operationMode == Class.clsCommon.Mode.Edit)
                            {
                                btnCancel_Click(sender, e);
                            }
                        }
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
                if (!isExternalCall)
                {
                    MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        item_price_list item_price_list = item_price_listViewSource.View.CurrentItem as entity.item_price_list;
                        item_price_list.is_active = false;
                        btnSave_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (!isExternalCall)
            {
                _entity.CancelChanges();
                item_price_listViewSource.View.Refresh();
            }
            else
            {
                if (operationMode == Class.clsCommon.Mode.Add)
                    mydb.CancelChanges();
            }
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
