﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data;
using System.Threading.Tasks;

namespace Cognitivo.Sales
{
	public partial class Budget : Page
	{
		SalesBudgetDB SalesBudgetDB = new SalesBudgetDB();

		CollectionViewSource sales_budgetViewSource,
			sales_budgetsales_budget_detailViewSource;

		public Budget()
		{
			InitializeComponent();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				Settings SalesSettings = new Settings();

				if (SalesSettings.FilterByBranch)
				{
					await SalesBudgetDB.sales_budget.Where(a => a.id_company == CurrentSession.Id_Company && a.id_branch == CurrentSession.Id_Branch).Include(x => x.contact).OrderByDescending(x => x.trans_date).LoadAsync();
				}
				else
				{
					await SalesBudgetDB.sales_budget.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.contact).OrderByDescending(x => x.trans_date).LoadAsync();
				}

				sales_budgetViewSource = ((CollectionViewSource)(FindResource("sales_budgetViewSource")));
				sales_budgetViewSource.Source = SalesBudgetDB.sales_budget.Local;
				sales_budgetsales_budget_detailViewSource = FindResource("sales_budgetsales_budget_detailViewSource") as CollectionViewSource;

				CollectionViewSource branchViewSource = ((CollectionViewSource)(FindResource("branchViewSource")));
				branchViewSource.Source = CurrentSession.Get_Branch();

				CollectionViewSource contractViewSource = ((CollectionViewSource)(FindResource("contractViewSource")));
				contractViewSource.Source = CurrentSession.Get_Contract();

				CollectionViewSource conditionViewSource = ((CollectionViewSource)(FindResource("conditionViewSource")));
				conditionViewSource.Source = CurrentSession.Get_Condition();

				await Dispatcher.InvokeAsync(new Action(() =>
				{
					cbxSalesRep.ItemsSource = CurrentSession.Get_SalesRep();
				}));

				CollectionViewSource app_document_rangeViewSource = FindResource("app_document_rangeViewSource") as CollectionViewSource;
				app_document_rangeViewSource.Source = entity.Brillo.Logic.Range.List_Range(SalesBudgetDB, entity.App.Names.SalesBudget, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

				CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
				app_vat_groupViewSource.Source = CurrentSession.Get_VAT_Group();
				
			}
			catch (Exception ex)
			{
				toolBar.msgError(ex);
			}
		}

		private void toolBar_btnPrint_Click(object sender, MouseButtonEventArgs e)
		{
			sales_budget sales_budget = sales_budgetDataGrid.SelectedItem as sales_budget;
			if (sales_budget != null)
			{
				entity.Brillo.Document.Start.Manual(sales_budget, sales_budget.app_document_range);
			}
			else
			{
				toolBar.msgWarning("Please select");
			}
		}

		#region toolbar Events
		private void New_Click(object sender)
		{
			sales_budget sales_budget = SalesBudgetDB.New();
			cbxCurrency.get_DefaultCurrencyActiveRate();

			SalesBudgetDB.sales_budget.Add(sales_budget);
			sales_budgetViewSource.View.MoveCurrentTo(sales_budget);
		}

		private void toolBar_btnEdit_Click(object sender)
		{
			if (sales_budgetDataGrid.SelectedItem != null)
			{
				sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
				sales_budget.IsSelected = true;
				sales_budget.State = EntityState.Modified;
				SalesBudgetDB.Entry(sales_budget).State = EntityState.Modified;
			}

			else
			{
				toolBar.msgWarning("Please Select A Record");
			}
		}

		private void Save_Click(object sender)
		{
			if (SalesBudgetDB.SaveChanges() > 0)
			{
				sales_budgetViewSource.View.Refresh();
				toolBar.msgSaved(SalesBudgetDB.NumberOfRecords);
				sbxContact.Text = "";   
			}
		}

		private void toolBar_btnDelete_Click(object sender)
		{
			MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (res == MessageBoxResult.Yes)
			{
				SalesBudgetDB.sales_budget.Remove((sales_budget)sales_budgetDataGrid.SelectedItem);
				sales_budgetViewSource.View.MoveCurrentToFirst();
				Save_Click(sender);
			}
		}

		private void toolBar_btnCancel_Click(object sender)
		{
			sales_budget_detailDataGrid.CancelEdit();
			sales_budgetViewSource.View.MoveCurrentToFirst();
			SalesBudgetDB.CancelAllChanges();
			
			if (sales_budgetsales_budget_detailViewSource.View != null)
				sales_budgetsales_budget_detailViewSource.View.Refresh();
		}

	 
		#endregion

		#region Filter Data
	
		private void id_conditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			sales_budget sales_budget = sales_budgetDataGrid.SelectedItem as sales_budget;
			if (cbxCondition.SelectedItem != null)
			{
				app_condition app_condition = cbxCondition.SelectedItem as app_condition;

				CollectionViewSource contractViewSource = ((CollectionViewSource)(FindResource("contractViewSource")));
				contractViewSource.View.Filter = i =>
				{
					app_contract objContract = (app_contract)i;
					if (objContract.id_condition == app_condition.id_condition)
					{ return true; }
					else
					{ return false; }
				};
				if (sales_budget!=null)
				{
					if (sales_budget.id_contract==0)
					{
						cbxContract.SelectedIndex = 0;
					}
				}
			   
			}

		}
		#endregion

		private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
		{
			popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
			popupCustomize.StaysOpen = false;
			popupCustomize.IsOpen = true;
		}
		private void popupCustomize_Closed(object sender, EventArgs e)
		{
			Settings SalesSettings = new Settings();

			popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
			Settings.Default.Save();
			SalesSettings = Settings.Default;
			popupCustomize.IsOpen = false;
		}

		private void calculate_vat(object sender, EventArgs e)
		{
			sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
			sales_budget.RaisePropertyChanged("GrandTotal");
			List<sales_budget_detail> sales_budget_detail = sales_budget.sales_budget_detail.ToList();
			dgvvat.ItemsSource = sales_budget_detail
				 .Join(SalesBudgetDB.app_vat_group_details, ad => ad.id_vat_group, cfx => cfx.id_vat_group
					  , (ad, cfx) => new { name = cfx.app_vat.name, value = ad.unit_price * cfx.app_vat.coefficient, id_vat = cfx.app_vat.id_vat, ad })
					  .GroupBy(a => new { a.name, a.id_vat, a.ad })
			   .Select(g => new
			   {
				   id_vat = g.Key.id_vat,
				   name = g.Key.name,
				   value = g.Sum(a => a.value * a.ad.quantity)
			   }).ToList();
		}

		private void sales_budgetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;

			if (sales_budget != null)
			{
				calculate_vat(sender, e);
			}
		}

		private void sales_budget_detailDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			calculate_vat(sender, e);
		}

		private void toolBar_btnApprove_Click(object sender)
		{
			if (SalesBudgetDB.Approve())
			{
				cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesBudgetDB, entity.App.Names.SalesBudget, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
				cbxDocument.SelectedIndex = 0;
				toolBar.msgApproved(SalesBudgetDB.NumberOfRecords);
			}
		}

		private void toolBar_btnAnull_Click(object sender)
		{
			if (SalesBudgetDB.Anull())
			{
				toolBar.msgAnnulled(SalesBudgetDB.NumberOfRecords);
			}
		}

		private void item_Select(object sender, EventArgs e)
		{
		   
			if (sbxItem.ItemID > 0)
			{
				sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
				item item = SalesBudgetDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();

				if (item != null && item.id_item > 0 && sales_budget!= null)
				{
					Settings SalesSettings = new Settings();
					Task Thread = Task.Factory.StartNew(() => select_Item(sales_budget, item, SalesSettings.AllowDuplicateItem));
				}

				sales_budget.RaisePropertyChanged("GrandTotal");
			}
		}

		private void select_Item(sales_budget sales_budget, item item, bool AllowDuplicateItem)
		{
			if (sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null || AllowDuplicateItem)
			{
				sales_budget_detail _sales_budget_detail = new sales_budget_detail();
				_sales_budget_detail.State = EntityState.Added;
				_sales_budget_detail.sales_budget = sales_budget;
				_sales_budget_detail.Contact = sales_budget.contact;
				_sales_budget_detail.item_description = item.description;
				_sales_budget_detail.item = item;
			
				_sales_budget_detail.id_item = item.id_item;
		   
				sales_budget.sales_budget_detail.Add(_sales_budget_detail);
			}
			else
			{
				sales_budget_detail sales_budget_detail = sales_budget.sales_budget_detail.Where(a => a.id_item == item.id_item).FirstOrDefault();
				sales_budget_detail.quantity += 1;
			}

			Dispatcher.BeginInvoke((Action)(() =>
			{
				sales_budgetsales_budget_detailViewSource.View.Refresh();
				calculate_vat(null, null);
			}));
		}

		private void toolBar_btnSearch_Click(object sender, string query)
		{
			try
			{
				if (!string.IsNullOrEmpty(query))
				{
					sales_budgetViewSource.View.Filter = i =>
					{
						sales_budget sales_budget = i as sales_budget;
						if (sales_budget.contact.name.ToLower().Contains(query.ToLower()))
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
					sales_budgetViewSource.View.Filter = null;
				}
			}
			catch (Exception)
			{ }
		}

		private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (e.Parameter as sales_budget_detail != null)
			{
				e.CanExecute = true;
			}
		}

		private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{

				MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					//sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
					sales_budget_detailDataGrid.CancelEdit();
					SalesBudgetDB.sales_budget_detail.Remove(e.Parameter as sales_budget_detail);
					sales_budgetsales_budget_detailViewSource.View.Refresh();
				}
			}
			catch (Exception ex)
			{
				toolBar.msgError(ex);
			}
		}

		private void set_ContactPref(object sender, EventArgs e)
		{
			if (sbxContact.ContactID > 0)
			{
				contact contact = SalesBudgetDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
				sales_budget sales_budget = (sales_budget)sales_budgetDataGrid.SelectedItem;
				sales_budget.id_contact = contact.id_contact;
				sales_budget.contact = contact;
				Task thread_SecondaryData = Task.Factory.StartNew(() => set_ContactPref_Thread(contact));
			}
		}

		private async void set_ContactPref_Thread(contact objContact)
		{
			if (objContact != null)
			{
				await Dispatcher.InvokeAsync(new Action(() =>
				{
					cbxContactRelation.ItemsSource = SalesBudgetDB.contacts.Where(x => x.parent.id_contact == objContact.id_contact).ToList();

					//Condition
					if (objContact.app_contract != null)
						cbxCondition.SelectedValue = objContact.app_contract.id_condition;

					//Contract
					if (objContact.id_contract != null)
						cbxContract.SelectedValue = Convert.ToInt32(objContact.id_contract);

					//Currency
					cbxCurrency.get_ActiveRateXContact(ref objContact);

					//SalesMan
					if (objContact.sales_rep != null)
						cbxCondition.SelectedValue = objContact.sales_rep.id_sales_rep;
				}));
			}
		}

		private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
		{
			sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
			if (sales_budget != null)
			{
				if (sales_budget.id_currencyfx > 0)
				{
					if (SalesBudgetDB.app_currencyfx.Where(x => x.id_currencyfx == sales_budget.id_currencyfx).FirstOrDefault() != null)
					{


						sales_budget.app_currencyfx = SalesBudgetDB.app_currencyfx.Where(x => x.id_currencyfx == sales_budget.id_currencyfx).FirstOrDefault();
					}
				}
			}
			calculate_vat(sender, e);
		}

		private void btnDuplicateInvoice_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;
			if (sales_budget != null)
			{
				if (sales_budget.id_sales_budget != 0)
				{
					var originalEntity = SalesBudgetDB.sales_budget.AsNoTracking().FirstOrDefault(x => x.id_sales_budget == sales_budget.id_sales_budget);
					SalesBudgetDB.sales_budget.Add(originalEntity);
					sales_budgetViewSource.View.Refresh();
					sales_budgetViewSource.View.MoveCurrentToLast();
				}
				else
				{
					toolBar.msgWarning("Please save before duplicating");
				}
			}
		}

		private void sales_budget_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			//calculate_total(sender, e);
			calculate_vat(sender, e);
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//calculate_total(sender, e);
			calculate_vat(sender, e);
		}

		private void lblEditProduct_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void Totals_btnClean_Click(object sender)
		{
			sales_budget sales_budget = sales_budgetViewSource.View.CurrentItem as sales_budget;

			if (sales_budget != null)
			{
				decimal TrailingDecimals = sales_budget.GrandTotal - Math.Floor(sales_budget.GrandTotal);
				sales_budget.DiscountWithoutPercentage += TrailingDecimals;
			}
		}
	}
}
