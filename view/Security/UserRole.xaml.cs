using Cognitivo.Menu;
using entity;
using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Security
{
    public partial class UserRole : Page
    {
        private UserRoleDB UserRoleDB = new UserRoleDB();

        private CollectionViewSource
            security_rolesecurity_curdViewSource,
            security_roleViewSource,
            security_rolesecurity_role_privilageViewSource;

        private CurrentSession.Versions CurrentVersion;
       // private Licence Licence = new Licence();

        public UserRole()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            security_roleViewSource = (CollectionViewSource)this.FindResource("security_roleViewSource");

            await UserRoleDB.security_role.Where(a => a.id_company == CurrentSession.Id_Company)
                                            .OrderBy(a => a.name)
                                            .Include(y => y.app_department)
                                            //.Include(y => y.security_role_privilage)
                                            .LoadAsync();
            security_roleViewSource.Source = UserRoleDB.security_role.Local;

            security_rolesecurity_curdViewSource = FindResource("security_rolesecurity_curdViewSource") as CollectionViewSource;
            security_rolesecurity_role_privilageViewSource = FindResource("security_rolesecurity_role_privilageViewSource") as CollectionViewSource;

            CollectionViewSource app_departmentViewSource = FindResource("app_departmentViewSource") as CollectionViewSource;
            app_departmentViewSource.Source = await UserRoleDB.app_department.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();

            Add_Privallge();
            cbxVersion.ItemsSource = Enum.GetValues(typeof(CurrentSession.Versions));

            //app_company app_company = UserRoleDB.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault();
            //if (app_company != null)
            //{
            //    Licence.VerifyCompanyLicence(app_company.version);
            //    if (Licence.CompanyLicence != null)
            //    {
            //        VersionGrid.ItemsSource = Licence.CompanyLicence.versions;
            //    }

            //}
        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                security_roleViewSource.View.Filter = i =>
                {
                    security_role security_role = i as security_role;
                    if (security_role.name.ToLower().Contains(query.ToLower()))
                    {
                        return true;
                    }

                    return false;
                };
            }
            else
            {
                security_roleViewSource.View.Filter = null;
            }
        }

        private void Save_Click(object sender)
        {
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
            if (security_role != null)
            {
                CurrentSession.Versions Version = (CurrentSession.Versions)Enum.Parse(typeof(CurrentSession.Versions), Convert.ToString(cbxVersion.Text));
                security_role.Version = Version;

                List<security_role> security_roleList = UserRoleDB.security_role.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                //int UserCount = 0;
                //UserCount = security_roleList.Where(x => x.Version == Version).Sum(x => x.security_user.Count);

                //int UserLimit = 0;

                ////Reload the Version Information.
                //Licence.VerifyCompanyLicence(
                //    UserRoleDB
                //    .app_company
                //    .Where(x => x.id_company == CurrentSession.Id_Company)
                //    .Select(y => y.version)
                //    .FirstOrDefault(),(int)security_role.Version,UserCount);

                //if (Licence.CompanyLicence != null)
                //{
                //    versions versions = Licence.CompanyLicence.versions.Where(x => x.version == Version).FirstOrDefault();

                //    if (versions != null)
                //    { //Exists = Yes.
                //        UserLimit = versions.web_user_count;

                //        if (UserLimit < UserCount)
                //        { //No space Avaiable
                //            MessageBox.Show("You have surpassed your User Limit of " + UserLimit + " for " + security_role.Version.ToString() + " Plan. /n" +
                //                "If you feel this is a mistake, please contact Cognitivo at hello@cognitivo.in. For now, we will revert you to the Free Plan."
                //                , "Cognitivo ERP");

                //            security_role.Version = CurrentSession.Versions.Lite;
                //        }
                //    }
                //    else
                //    {
                //        string key = Licence.CreateLicenceVersion(Licence.CompanyLicence.license_key, (int)security_role.Version);
                //        //write code for trial 15 days for this plan.
                //        if (key == Licence.CompanyLicence.license_key && security_role.Version != CurrentSession.Versions.Lite)
                //        {
                //            MessageBox.Show("Done. Since you do not have this plan set up, we have gone ahead and registered the " + security_role.Version.ToString() + " Plan on your behalf. /n" +
                //                       "You will have 15 days trial period, once finished, you will be diverted to the free account."
                //                       , "Cognitivo ERP");
                //        }
                //    }
                //}

                //UserRoleDB.SaveChanges();

                CurrentSession.Load_Security();
                security_roleViewSource.View.Refresh();
            }
        }

        private void Cancel_Click(object sender)
        {
            foreach (security_user security_role in UserRoleDB.security_user.Local.Where(x => x.IsSelected))
            {
                security_role.IsSelected = false;
                security_role.State = EntityState.Unchanged;
                UserRoleDB.Entry(security_role).State = EntityState.Unchanged;
            }

            UserRoleDB.SaveChanges();
        }

        private void New_Click(object sender)
        {
            security_role security_role = new security_role();
            if (security_role != null)
            {
                Add_Privallge();
                Add_MissingRecords(security_role);
                security_role.State = EntityState.Added;
                security_role.Version = CurrentSession.Versions.Lite;

                security_role.IsSelected = true;
                UserRoleDB.security_role.Add(security_role);

                security_roleViewSource.View.Refresh();
                security_roleViewSource.View.MoveCurrentToLast();
            }
        }

        private void Edit_Click(object sender)
        {
            security_role security_role = (security_role)security_roleDataGrid.SelectedItem;
            if (security_role != null)
            {
                Add_Privallge();
                Add_MissingRecords(security_role);
                security_role.IsSelected = true;
                security_role.State = EntityState.Modified;
                UserRoleDB.Entry(security_role).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning(Localize.PleaseSelect);
            }
        }

        private void Add_Privallge()
        {
            List<entity.App.Names> Application = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
            List<Privilage.Privilages> Privilages = Enum.GetValues(typeof(Privilage.Privilages)).Cast<Privilage.Privilages>().ToList();
            security_role security_role = security_roleDataGrid.SelectedItem as security_role;

            foreach (entity.App.Names Names in Application)
            {
                if (Names == entity.App.Names.SalesInvoice)
                {
                    foreach (Privilage.Privilages Privilage in Privilages)
                    {
                        if (UserRoleDB.security_privilage.Where(x => x.name == Privilage).Count() == 0)
                        {
                            if ((int)Privilage >= 3)
                            {
                                security_privilage security_privilage = new security_privilage()
                                {
                                    id_application = entity.App.Names.ProductionExecution,
                                    name = Privilage
                                };

                                UserRoleDB.security_privilage.Add(security_privilage);
                            }
                            else
                            {
                                security_privilage security_privilage = new security_privilage()
                                {
                                    id_application = Names,
                                    name = Privilage
                                };
                                UserRoleDB.security_privilage.Add(security_privilage);
                            }
                        }
                    }
                }
            }

            List<entity.App.Names> PreferenceList = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
            List<security_privilage> security_privilageList = UserRoleDB.security_privilage.ToList();

            foreach (security_privilage security_privilage in security_privilageList)
            {
                if (security_privilage.id_application == entity.App.Names.SalesInvoice ||
                    security_privilage.id_application == entity.App.Names.ProductionExecution)
                {
                    if (UserRoleDB.security_role_privilage.Where(x => x.id_privilage == security_privilage.id_privilage && x.id_role == security_role.id_role).Count() == 0)
                    {
                        security_role_privilage _security_role_privilage = new security_role_privilage()
                        {
                            id_privilage = security_privilage.id_privilage,
                            security_privilage = security_privilage,
                            id_role = security_role.id_role,
                            security_role = security_role
                        };
                        UserRoleDB.security_role_privilage.Add(_security_role_privilage);
                    }
                }
            }
        }

        private void ChangePlan(object sender, RoutedEventArgs e)
        {
            if ((security_role)security_roleDataGrid.SelectedItem != null)
            {
                Add_MissingRecords((security_role)security_roleDataGrid.SelectedItem);
            }
        }

        private void Add_MissingRecords(security_role security_role)
        {
            AppList appList = new AppList();
            List<security_crud> security_curd = security_rolesecurity_curdViewSource.View.OfType<security_crud>().ToList().Where(x => x.id_role == security_role.id_role).ToList();

            if (security_curd.Count() == 0)
            {
                List<entity.App.Names> Application = Enum.GetValues(typeof(entity.App.Names)).Cast<entity.App.Names>().ToList();
                foreach (entity.App.Names AppName in Application)
                {
                    security_crud _security_curd = new security_crud()
                    {
                        id_application = AppName,
                        can_update = false,
                        can_read = false,
                        can_delete = false,
                        can_create = false,
                        can_approve = false,
                        can_annul = false
                    };
                    security_role.security_curd.Add(_security_curd);
                }
            }
            else
            {

				if (Convert.ToString(cbxVersion.Text) != null)
				{


					CurrentSession.Versions NewVersion =
						(CurrentSession.Versions)Enum.Parse(typeof(CurrentSession.Versions),
						Convert.ToString(cbxVersion.Text));

					if (CurrentVersion < NewVersion)
					{
						List<entity.App.Names> dtApplication = new List<entity.App.Names>();
						string condition = "";

						if (NewVersion == CurrentSession.Versions.Full)
						{
							condition = "1=1";
						}
						else if (NewVersion == CurrentSession.Versions.Medium)
						{
							condition = "Version='" + NewVersion + "' and Version ='Lite' and Version='Basic'";
						}
						else if (NewVersion == CurrentSession.Versions.Basic)
						{
							condition = "Version='" + NewVersion + "' and Version ='Lite' ";
						}
						else if (NewVersion == CurrentSession.Versions.Lite)
						{
							condition = "Version='" + NewVersion + "'";
						}
						foreach (DataRow item in appList.dtApp.Select(condition))
						{
							if (Enum.IsDefined(typeof(entity.App.Names), Convert.ToString(item["name"])) == true)
							{
								dtApplication.Add((entity.App.Names)Enum.Parse(typeof(entity.App.Names), Convert.ToString(item["name"])));
							}
						}

						List<entity.App.Names> _security_curdApplication = security_curd.Select(x => x.id_application).ToList();
						List<entity.App.Names> AddList = Enumerable.Except(dtApplication, _security_curdApplication).ToList();
						foreach (entity.App.Names AppName in AddList)
						{
							security_crud _rolesecurity_curd = security_role.security_curd.Where(x => x.id_application == AppName).FirstOrDefault();
							if (_rolesecurity_curd == null)
							{
								security_crud _security_curd = new security_crud()
								{
									id_application = AppName,
									can_update = false,
									can_read = false,
									can_delete = false,
									can_create = false,
									can_approve = false,
									can_annul = false
								};
								security_role.security_curd.Add(_security_curd);
							}
						}
					}
					else if (CurrentVersion > NewVersion)
					{
						List<entity.App.Names> dtApplication = new List<entity.App.Names>();
						foreach (DataRow item in appList.dtApp.Select("Version = '" + NewVersion + "'"))
						{
							if (Enum.IsDefined(typeof(entity.App.Names), Convert.ToString(item["name"])) == true)
							{
								dtApplication.Add((entity.App.Names)Enum.Parse(typeof(entity.App.Names), Convert.ToString(item["name"])));
							}
						}

						List<entity.App.Names> _security_curdApplication = security_curd.Select(x => x.id_application).ToList();
						List<entity.App.Names> AddList = Enumerable.Except(_security_curdApplication, dtApplication).ToList();

						foreach (entity.App.Names AppName in AddList)
						{
							security_crud _security_curd = security_role.security_curd.Where(x => x.id_application == AppName).FirstOrDefault();
							if (_security_curd != null)
							{
								//  UserRoleDB.Entry(_security_curd).State = EntityState.Deleted;
								security_role.security_curd.Remove(_security_curd);
							}
						}
					}
					CurrentVersion = NewVersion;
				}
            }
            security_roleViewSource.View.Refresh();
            security_rolesecurity_curdViewSource.View.Refresh();
            security_rolesecurity_role_privilageViewSource.View.Refresh();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (security_roleViewSource.View.CurrentItem is security_role security_role)
            {
                if (security_rolesecurity_curdViewSource != null)
                {
                    if (security_rolesecurity_curdViewSource.View != null)
                    {
                        security_rolesecurity_curdViewSource.View.Filter = i =>
                        {
                            security_crud security_curd = (security_crud)i;
                            string TranslatedName = Localize.StringText(security_curd.id_application.ToString());

                            if (TranslatedName.ToUpper().Contains(txtsearch.Text.ToUpper()))
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

		private void Security_roleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (security_roleViewSource.View.CurrentItem is security_role security_role)
			{
				cbxVersion.SelectedItem = security_role.Version;
				
				//CurrentVersion = security_role.Version;
				//lblVersionlocal.Content = UserRoleDB.security_role.Local.Where(x => x.Version == security_role.Version).Sum(x => x.security_user.Count);

				//if (Licence.CompanyLicence != null)
				//{
				//	versions versions = Licence.CompanyLicence.versions.Where(x => x.version == security_role.Version).FirstOrDefault();

				//	if (versions != null)
				//	{
				//		//Exists = Yes.
				//		lblVersionInternet.Content = versions.web_user_count;
				//	}
				//}
			}
		}
	}
}