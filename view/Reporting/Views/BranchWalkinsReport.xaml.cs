using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Forms;
using entity;

namespace Cognitivo.Reporting.Views
{
    public partial class BranchWalkinsReport : Page
    {
        List<clsBranchWalkin> clsBranchWalkinList = new List<clsBranchWalkin>();
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        private DateTime _StartDate = DateTime.Now.AddMonths(-1);

        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        private DateTime _EndDate = DateTime.Now.AddDays(+1);

        public BranchWalkinsReport()
        {
            InitializeComponent();

            using (db db = new db())
            {
                db.app_branch.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).OrderBy(y => y.name).ToList();
                cbxBranch.ItemsSource = db.app_branch.Local;
            }

            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            app_branch app_branch = cbxBranch.SelectedItem as app_branch;
            decimal branchvalue = 0;
            decimal salesvalue = 0;
           
            if (app_branch != null)
            {
                int _id_branch = app_branch.id_branch;
                using (db db = new db())
                {
                    if (_id_branch>0)
                    {
                        branchvalue = db.app_branch_walkins.Where(x => (x.start_date >= _StartDate || x.start_date <= _EndDate) && x.id_branch == _id_branch).Count();
                        List<sales_invoice> sales_invoiceList=db.sales_invoice.Where(x => (x.trans_date >= _StartDate || x.trans_date <= _EndDate) && x.id_branch == _id_branch).ToList();
                        salesvalue = sales_invoiceList.Sum(x => x.GrandTotal);
                        if (branchvalue>0 && salesvalue>0)
                        {
                            clsBranchWalkin clsBranchWalkin = new clsBranchWalkin();
                            clsBranchWalkin.BranchName = app_branch.name;
                            clsBranchWalkin.Value = salesvalue/branchvalue;
                            clsBranchWalkinList.Add(clsBranchWalkin);
                        }
                    }

                    
                }
                dgvSalesDetail.ItemsSource = clsBranchWalkinList;
            }
        }
    }
    public class clsBranchWalkin
    {
        public string BranchName { get; set; }
        public decimal Value { get; set; }
    }

}
