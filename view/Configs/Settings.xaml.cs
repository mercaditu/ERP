using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;

namespace Cognitivo.Configs
{
	public partial class Settings : Page
	{
		private CollectionViewSource app_companyViewSource;
		private CollectionViewSource app_companyapp_branchViewSource;
		private CollectionViewSource app_companyapp_branchapp_terminalViewSource;
		private CollectionViewSource app_companyapp_branchapp_terminalapp_accountViewSource;
		private Menu.MainWindow mainWindow;

		private entity.db db = new entity.db();

		public Settings()
		{
			InitializeComponent();

			app_companyViewSource = ((CollectionViewSource)(this.FindResource("app_companyViewSource")));
			db.app_company.Include(v => v.app_branch).LoadAsync();

			app_companyViewSource.Source = db.app_company.Local;
			app_companyapp_branchViewSource = ((CollectionViewSource)(this.FindResource("app_companyapp_branchViewSource")));
			app_companyapp_branchapp_terminalViewSource = ((CollectionViewSource)(this.FindResource("app_companyapp_branchapp_terminalViewSource")));
			app_companyapp_branchapp_terminalapp_accountViewSource = ((CollectionViewSource)(this.FindResource("app_companyapp_branchapp_terminalapp_accountViewSource")));

			Filter_Data();
		}
		private void Filter_Data()
		{
			if (app_companyapp_branchViewSource != null)
			{
				if (app_companyapp_branchViewSource.View != null)
				{
					app_companyapp_branchViewSource.View.Filter = i =>
					  {
						  app_branch app_branch = i as app_branch;
						  if (app_branch.is_active)
						  {
							  return true;
						  }
						  else
						  {
							  return false;
						  }
					  };
				}
			}
			if (app_companyapp_branchapp_terminalViewSource != null)
			{
				if (app_companyapp_branchapp_terminalViewSource.View != null)
				{
					app_companyapp_branchapp_terminalViewSource.View.Filter = i =>
					  {
						  app_terminal app_terminal = i as app_terminal;
						  if (app_terminal.is_active)
						  {
							  return true;
						  }
						  else
						  {
							  return false;
						  }
					  };
				}
			}
			if (app_companyapp_branchapp_terminalapp_accountViewSource != null)
			{
				if (app_companyapp_branchapp_terminalapp_accountViewSource.View != null)
				{
					app_companyapp_branchapp_terminalapp_accountViewSource.View.Filter = i =>
					  {
						  app_account app_account = i as app_account;
						  if (app_account.is_active)
						  {
							  return true;
						  }
						  else
						  {
							  return false;
						  }
					  };
				}
			}

		}
		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			if (app_companyViewSource.View != null)
			{
				int new_CompanyID = ((entity.app_company)app_companyViewSource.View.CurrentItem).id_company;

				entity.Properties.Settings.Default.Save();

				if (new_CompanyID != entity.CurrentSession.Id_Company &&
					MessageBox.Show(entity.Brillo.Localize.StringText("Applicationrestartwillberequired"), "Cognitivo ERP",
					MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
				{
					//Restart Application because Company has changed.
					System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
					Application.Current.Shutdown();
				}
				else
				{
					entity.CurrentSession.Id_Company = entity.Properties.Settings.Default.company_ID;
					entity.CurrentSession.Id_Branch = entity.Properties.Settings.Default.branch_ID;
					entity.CurrentSession.Id_Terminal = entity.Properties.Settings.Default.terminal_ID;
					entity.CurrentSession.Id_Account = entity.Properties.Settings.Default.account_ID;
					entity.Properties.Settings.Default.Save();

					//GoBack with changed data.
					Frame frm = entity.Brillo.Parent.GetParent<Frame>(this);
					if (frm != null)
					{
						frm.NavigationService.GoBack();
					}
				}
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			mainWindow = Window.GetWindow(this) as Menu.MainWindow;
		}
	}
}