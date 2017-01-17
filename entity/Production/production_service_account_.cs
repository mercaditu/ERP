namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class production_service_account : Audit
    {
        public production_service_account()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
            child = new List<production_service_account>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_production_service_account { get; set; }
        public int? id_contact { get; set; }
        public int id_item { get; set; }
        public int? id_order_detail { get; set; }
    

        public int? id_purchase_order_detail { get; set; }
        public int? id_purchase_invoice_detail { get; set; }
        public Status.Production? status { get; set; }

        public decimal unit_cost { get; set; }

        [Required]
        public decimal debit { get; set; }
        [Required]
        public decimal credit { get; set; }

        [NotMapped]
        public decimal Balance { get { return credit- child.Sum(x => x.debit); } set { _Balance = value;  } }
        decimal _Balance;

        [Required]
        public DateTime trans_date { get; set; }

        public DateTime? exp_date { get; set; }

        //Heirarchy
        public virtual production_service_account parent { get; set; }
        public virtual ICollection<production_service_account> child { get; set; }

        public virtual item item { get; set; }
        public virtual contact contact { get; set; }
        public virtual List<production_execution_detail> production_execution_detail { get; set; }
        public virtual production_order_detail production_order_detail { get; set; }
        public virtual purchase_order_detail purchase_order_detail { get; set; }
        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
    }
}
