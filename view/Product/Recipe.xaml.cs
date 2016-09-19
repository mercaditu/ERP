using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for ItemRecipe.xaml
    /// </summary>
    public partial class Recipe : Page
    {
        ProductRecipeDB ProductRecipeDB = new ProductRecipeDB();
        CollectionViewSource item_recepieViewSource, item_recepieitem_recepie_detailViewSource;
        int company_ID;
        public Recipe()
        {
            InitializeComponent();
        }

        private void item_Select(object sender, EventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = ProductRecipeDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                item.is_autorecepie = true;

                if (ProductRecipeDB.item_recepie.Where(x=>x.id_item==item.id_item).Count()==0)
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

                if (item_recepie!=null)
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProductRecipeDB.item_recepie.Where(a => a.id_company == company_ID
                                                       && (a.is_head == true)).ToList();

            item_recepieViewSource = ((CollectionViewSource)(FindResource("item_recepieViewSource")));
            item_recepieViewSource.Source = ProductRecipeDB.item_recepie.Local;

            item_recepieitem_recepie_detailViewSource = ((CollectionViewSource)(FindResource("item_recepieitem_recepie_detailViewSource")));

        }

        private void btnNew_Click(object sender)
        {
            item_recepie item_recepie = ProductRecipeDB.New();
            ProductRecipeDB.Entry(item_recepie).State = EntityState.Added;
            item_recepieViewSource.View.MoveCurrentToLast();
        }

        private void btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    item_recepie item_recepie = (item_recepie)item_receipeDataGrid.SelectedItem;
                    item_recepie.is_head = false;
                    item_recepie.State = EntityState.Deleted;
                    item_recepie.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }

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
                item_recepie_old.IsSelected = true;
                item_recepie_old.State = EntityState.Modified;
                ProductRecipeDB.Entry(item_recepie_old).State = EntityState.Modified;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnCalculateCost_Click(object sender, RoutedEventArgs e)
      {
          item_recepie item_recepie = item_recepieViewSource.View.CurrentItem as item_recepie;
          decimal Cost = 0;
          int id_currency = CurrentSession.CurrencyFX_Default.id_currency;

          foreach (item_recepie_detail item_recepie_detail in item_recepie.item_recepie_detail)
          {
              if (item_recepie_detail.item != null)
              {
                  Cost += (decimal)item_recepie_detail.item.unit_cost * item_recepie_detail.quantity;
              }
          }

          tbxCalculateCost.Text = entity.Brillo.Localize.StringText("Cost") + " : " + Math.Round(Cost) + " " + CurrentSession.Currency_Default.name;
      }
    }
}
