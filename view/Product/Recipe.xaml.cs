using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public partial class Recipe : Page
    {
        private ProductRecipeDB ProductRecipeDB = new ProductRecipeDB();
        private CollectionViewSource item_recepieViewSource, item_recepieitem_recepie_detailViewSource;

        public Recipe()
        {
            InitializeComponent();
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = ProductRecipeDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

                if (ProductRecipeDB.item_recepie.Where(x => x.id_item == item.id_item).Count() == 0)
                {
                    item_recepie item_recepie = item_recepieViewSource.View.CurrentItem as item_recepie;
                    item_recepie.id_item = item.id_item;
                    item_recepie.item = item;
                }
                else
                {
                    toolBar.msgWarning("Item Already Used....");
                }
            }
        }

        private void item_Select_detail(object sender, EventArgs e)
        {
            if (sbxItemDetail.ItemID > 0)
            {
                item_recepie item_recepie = item_recepieViewSource.View.CurrentItem as item_recepie;
                item item = ProductRecipeDB.items.Where(x => x.id_item == sbxItemDetail.ItemID).FirstOrDefault();

                if (item_recepie != null)
                {
                    item_recepie_detail item_recepie_detail = new item_recepie_detail();
                    item_recepie_detail.id_item = item.id_item;
                    item_recepie_detail.item = item;
                    item_recepie_detail.quantity = 1;
                    item_recepie_detail.item_recepie = item_recepie;
                    item_recepie.item_recepie_detail.Add(item_recepie_detail);
                }
                item_recepieitem_recepie_detailViewSource.View.Refresh();
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ProductRecipeDB.item_recepie.Where(a => a.id_company == CurrentSession.Id_Company
                                                       && (a.is_head == true)).Include(b => b.item).OrderBy(x => x.item.name).ToListAsync();

            item_recepieViewSource = FindResource("item_recepieViewSource") as CollectionViewSource;
            item_recepieViewSource.Source = ProductRecipeDB.item_recepie.Local;

            item_recepieitem_recepie_detailViewSource = FindResource("item_recepieitem_recepie_detailViewSource") as CollectionViewSource;
        }

        private void btnNew_Click(object sender)
        {
            item_recepie item_recepie = ProductRecipeDB.New();
            ProductRecipeDB.Entry(item_recepie).State = EntityState.Added;
            item_recepieViewSource.View.MoveCurrentToLast();
        }

        private void btnSave_Click(object sender)
        {
            if (ProductRecipeDB.SaveChanges() > 0)
            {
                item_recepieViewSource.View.Refresh();
                toolBar.msgSaved(ProductRecipeDB.NumberOfRecords);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (item_receipeDataGrid.SelectedItem != null)
            {
                item_recepie item_recepie_old = (item_recepie)item_receipeDataGrid.SelectedItem;

                if (item_recepie_old != null)
                {
                    item_recepie_old.IsSelected = true;
                    item_recepie_old.State = EntityState.Modified;
                    ProductRecipeDB.Entry(item_recepie_old).State = EntityState.Modified;
                }
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ProductRecipeDB.CancelAllChanges();
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_recepie_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    item_recepie_detail item_recepie_detail = item_recepieitem_recepie_detailViewSource.View.CurrentItem as item_recepie_detail;
                    //DeleteDetailGridRow
                    dgvReceipeDetail.CancelEdit();
                    ProductRecipeDB.item_recepie_detail.Remove(e.Parameter as item_recepie_detail);
                    item_recepieitem_recepie_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                item_recepieViewSource.View.Filter = i =>
                {
                    item_recepie item = i as item_recepie;
                    item it = item.item;

                    if (item.Error == null && it != null)
                    {
                        if (it.name.ToLower().Contains(query.ToLower()) || it.code.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                };
            }
            else
            {
                item_recepieViewSource.View.Filter = null;
            }
        }

        private void btnCalculateCost_Click(object sender, RoutedEventArgs e)
        {
            item_recepie item_recepie = item_recepieViewSource.View.CurrentItem as item_recepie;
            if (item_recepie != null)
            {
                List<item_recepie_detail> item_recepie_detailList = item_recepie.item_recepie_detail.ToList();

                if (item_recepie_detailList.Count > 0)
                {
                    cntrl.PanelAdv.pnlCostCalculationReceipe pnlCostCalculationReceipe = new cntrl.PanelAdv.pnlCostCalculationReceipe();
                    pnlCostCalculationReceipe.Outputitem_recepie_detailList = item_recepie_detailList;
                    List<item_recepie> item_recepieList = new List<item_recepie>();
                    item_recepieList.Add(item_recepie);
                    pnlCostCalculationReceipe.Inputitem_recepieList = item_recepieList;
                    crud_modal_cost.Visibility = Visibility.Visible;
                    crud_modal_cost.Children.Add(pnlCostCalculationReceipe);
                }
            }
        }
    }
}