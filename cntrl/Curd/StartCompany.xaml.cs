using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for StartCompany.xaml
    /// </summary>
    public partial class StartCompany : UserControl
    {
        public StartCompany()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            app_company app_company = new app_company();
            using (db db = new db())
            {
                try
                {
                    app_company.name = txtname.Text;
                    app_company.alias = txtalias.Text;
                    app_company.gov_code = txtGovID.Text;
                    app_company.address = txtAddress.Text;
                    app_company.app_company_interest = new app_company_interest();
                    db.app_company.Add(app_company);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            CurrentSession.Id_Company = app_company.id_company;
            
            Window parentGrid = (Window)this.Parent;
           

            parentGrid.Content=new cntrl.user();
        }
    }
}
