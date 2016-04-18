﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Controls
{
    public partial class SmartBox_Item : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public bool can_New
        {
            get { return _can_new; }
            set
            {
                 entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Item);
                 if (Sec.create)
                 {
                     _can_new = value;
                     RaisePropertyChanged("can_New");
                 }
                 else
                 {
                     _can_new = false;
                     RaisePropertyChanged("can_New");
                 }
            }
        }
        bool _can_new;
        public bool can_Edit
        {
            get { return _can_new; }
            set
            {
                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Item);
                if (Sec.edit)
                {
                    _can_edit = value;
                    RaisePropertyChanged("can_Edit");
                }
                else
                {
                    _can_edit = false;
                    RaisePropertyChanged("can_Edit");
                }
            }
        }
bool _can_edit;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Item));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public event RoutedEventHandler Select;
        private void ItemGrid_MouseDoubleClick(object sender, EventArgs e)
        {
            if (itemViewSource != null)
            {
                if (itemViewSource.View != null)
                {
                    Item = itemViewSource.View.CurrentItem as entity.item;

                    if (Item != null)
                    {
                        ItemID = Item.id_item;
                        ContactPopUp.IsOpen = false;
                        Text = Item.name;
                    }
                    else
                    {
                        ItemID = 0;
                        Text = tbxSearch.Text;
                    }

                    tbxSearch.SelectAll();
                }
            }

            if (Select != null)
            { Select(this, new RoutedEventArgs()); }
        }

        public int ItemID { get; set; }
        public entity.item Item { get; set; }
        public entity.item.item_type? item_types { get; set; }
        entity.dbContext db = new entity.dbContext();
        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource itemViewSource;

        public SmartBox_Item()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
            }
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ItemGrid_MouseDoubleClick(sender, e);
            }

            else if (e.Key == Key.Up)
            {
                itemViewSource.View.MoveCurrentToPrevious();
                itemViewSource.View.Refresh();
            }
            else if (e.Key == Key.Down)
            {
                itemViewSource.View.MoveCurrentToNext();
                itemViewSource.View.Refresh();
            }
            else
            {

                string SearchText = tbxSearch.Text;

                if (SearchText.Count() >= 3)
                {
                    if (taskSearch != null)
                    {
                        if (taskSearch.Status == TaskStatus.Running)
                        {
                            tokenSource.Cancel();
                        }
                    }

                    progBar.IsActive = true;

                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    taskSearch = Task.Factory.StartNew(() => Search_OnThread(SearchText), token);
                }
                else
                {
                    //contactViewSource.Source = null;
                }
            }
        }

        private void Search_OnThread(string SearchText)
        {
            using (entity.db db = new entity.db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                List<entity.item> results;
                if (item_types == null)
                {
                    results = db.items
                               .Where(x =>
                                         x.id_company == entity.CurrentSession.Id_Company &&
                                         (
                                             x.code.Contains(SearchText) ||
                                             x.name.Contains(SearchText) ||
                                             x.item_brand.name.Contains(SearchText)
                                         ) &&
                                        x.is_active == true
                                       )
                               .OrderBy(x => x.name)
                               .ToList();
                }
                else
                {
                    results = db.items
                                  .Where(x =>
                                            x.id_company == entity.CurrentSession.Id_Company && x.id_item_type == item_types &&
                                            (
                                                x.code.Contains(SearchText) ||
                                                x.name.Contains(SearchText) ||
                                                x.item_brand.name.Contains(SearchText)
                                            ) &&
                                           x.is_active == true
                                          )
                                  .OrderBy(x => x.name)
                                  .ToList();

                }

                Dispatcher.InvokeAsync(new Action(() =>
                {
                    //itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
                    itemViewSource.Source = results;
                    Item = itemViewSource.View.CurrentItem as entity.item;

                    ContactPopUp.IsOpen = true;
                    progBar.IsActive = false;
                }));
            }
        }

        private void Add_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Item);
            if (Sec.create)
            {
                crudItem.itemobject = new entity.item();
                crudItem.entity = db;
                popCrud.IsOpen = true;
                popCrud.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void crudItem_btnSave_Click(object sender)
        {
            //using (entity.db db = new entity.db())
            //{
            //    if (crudItem.itemList.Count() > 0)
            //    {
            //        foreach (entity.item item in crudItem.itemList)
            //        {
            //            if (item.id_item == 0)
            //            {
            //                db.items.Add(item);
            //            }
            //        }
            //        db.SaveChanges();
            //    }
            //}
            db.db.items.Add(crudItem.itemobject);
            db.SaveChanges();
            popCrud.IsOpen = false;
            popCrud.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Edit_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Item);
            if (Sec.edit)
            {
                crudItem.itemobject = new entity.item();
                popCrud.IsOpen = true;
                popCrud.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void crudItem_btnCancel_Click(object sender)
        {
            popCrud.IsOpen = false;
        }

     
    }
}
