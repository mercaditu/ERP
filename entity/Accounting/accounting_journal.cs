namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;

    public class accounting_journal : Audit
    {
        public enum Types
        {
            Expense,
            Income,
            Journal
        }

        public accounting_journal()
        {
            id_branch = CurrentSession.Branch.id_branch;
            id_company = CurrentSession.Company.id_company;
            id_user = CurrentSession.User.id_user;
            is_head = true;
            trans_date = DateTime.Now;
            accounting_journal_detail = new List<accounting_journal_detail>();
            sales_invoice = new List<sales_invoice>();
            purchase_invoice = new List<purchase_invoice>();
            status = Status.Documents_General.Pending;
            using (db context = new db())
            {
                accounting_cycle accounting_cycle = context.accounting_cycle.Where(i => i.is_active == true).FirstOrDefault();

                if (accounting_cycle != null)
                {
                    int cycle_id = accounting_cycle.id_cycle;
                    if (context.accounting_journal.Where(c => c.id_cycle == cycle_id).Count() > 0)
                    {
                        code = context.accounting_journal.Where(c => c.id_cycle == cycle_id).Max(i => i.code) + 1;
                    }
                }
            }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_journal { get; set; }
        public int id_branch { get; set; }
        public int id_cycle { get; set; }
        public Types type { get; set; }

        //Abhi> Add logic to increase Group Code +1 for every group of transaction within 
        //Period. If Period is starting, then start from 0.
        public int code { get; set; }
        public string comment { get; set; }
        public DateTime trans_date { get; set; }
        public Status.Documents_General status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("status"); }
        }
        Status.Documents_General _status;

        [NotMapped]
        public bool is_accounted { get; set; }

        public virtual app_branch app_branch { get; set; }
        public virtual accounting_cycle accounting_cycle { get; set; }
        public virtual ICollection<accounting_journal_detail> accounting_journal_detail { get; set; }

        public virtual ICollection<purchase_invoice> purchase_invoice { get; set; }
        public virtual IEnumerable<purchase_return> purchase_return { get; set; }

        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
        public virtual IEnumerable<sales_return> sales_return { get; set; }

        public virtual IEnumerable<payment> payment { get; set; }
        public virtual IEnumerable<payment_withholding_tax> payment_withholding_tax { get; set; }
        #region "Validations"
        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }
                return error.Length == 0 ? null : error.ToString();
            }
        }
        public string this[string columnName]
        {
            get
            {
                // apply property level validation rules
                if (columnName == "id_cycle")
                {
                    if (id_cycle == 0)
                        return "Cycle needs to be selected";
                }
                if (columnName == "id_branch" && app_branch == null)
                {
                    if (id_branch == 0)
                        return "Branch needs to be selected";
                }
             
                return "";
            }
        }
        #endregion
    }
}
