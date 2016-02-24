using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for Acocunting_Template_Entry.xaml
    /// </summary>
    public partial class Acocunting_Template_Entry : UserControl
    {
        dbContext entity = new dbContext();
        CollectionViewSource _accounting_journalViewSource = null; 
        public CollectionViewSource accounting_journalViewSource { get { return _accounting_journalViewSource; } set { _accounting_journalViewSource = value; } }
        accounting_cycle _accounting_cycle = null;
        public accounting_cycle accounting_cycle { get { return _accounting_cycle; } set { _accounting_cycle = value; } }
        entity.Properties.Settings _setting = new entity.Properties.Settings();
      public  accounting_journal accounting_journal { get; set; }
        public Acocunting_Template_Entry()
        {
            InitializeComponent();
        }



        public event btnSave_ClickedEventHandler Save_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            List<accounting_template_detail> accounting_template_detail;
            int id_template = 0;
            if (cbxTemplate.SelectedValue != null)
            {
                id_template = (int)cbxTemplate.SelectedValue;
            }

            accounting_template_detail = entity.db.accounting_template.Where(x => x.id_template == id_template).FirstOrDefault().accounting_template_detail.ToList();
            accounting_journal _accounting_journal = new accounting_journal();
            _accounting_journal.id_cycle = (int)cbxFiscal.SelectedValue;
            _accounting_journal.comment = commentTextBox.Text;
            foreach (accounting_template_detail _accounting_template_detail in accounting_template_detail)
            {
                accounting_journal_detail accounting_journal_detail = new accounting_journal_detail();
                accounting_journal_detail.id_application = App.Names.Item;
                accounting_journal_detail.id_chart = _accounting_template_detail.id_chart;
                accounting_journal_detail.id_currencyfx = entity.db.app_currency.Where(x => x.is_priority == true).FirstOrDefault().app_currencyfx.FirstOrDefault().id_currencyfx;
                if (_accounting_template_detail.is_debit == true)
                {

                    accounting_journal_detail.debit = Convert.ToDecimal(txtAmount.Text) * _accounting_template_detail.coefficeint;
                    accounting_journal_detail.credit = 0;
                }
                else
                {
                    accounting_journal_detail.debit = 0;
                    accounting_journal_detail.credit = Convert.ToDecimal(txtAmount.Text) * _accounting_template_detail.coefficeint;
                }
                accounting_journal_detail.trans_date = (DateTime)dtpTrans_Date.SelectedDate;
                accounting_journal_detail.is_head = false;
                _accounting_journal.accounting_journal_detail.Add(accounting_journal_detail);
                accounting_journal = _accounting_journal;
                //  entity.db.accounting_journal.Add(accounting_journal);
            }
            Save_Click(sender);
            

        }
      
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
          
            
            CollectionViewSource accounting_templateViewSource = ((CollectionViewSource)(FindResource("accounting_templateViewSource")));

            accounting_templateViewSource.Source = entity.db.accounting_template.Where(x=>x.id_company==_setting.company_ID).OrderBy(b => b.name).ToList();

            CollectionViewSource accounting_cycleViewSource = ((CollectionViewSource)(FindResource("accounting_cycleViewSource")));

            accounting_cycleViewSource.Source = entity.db.accounting_cycle.Where(x => x.id_company == _setting.company_ID).OrderBy(b => b.name).ToList();

            cbxFiscal.SelectedItem = entity.db.accounting_cycle.Where(x =>x.id_company == _setting.company_ID && x.is_active == true).FirstOrDefault();
        }
    }
}
