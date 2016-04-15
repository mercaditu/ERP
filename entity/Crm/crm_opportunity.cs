namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class crm_opportunity : Audit
    {
        public crm_opportunity()
        {
            sales_invoice = new List<sales_invoice>();
            sales_order = new List<sales_order>();
            sales_budget = new List<sales_budget>();
            sales_packing = new List<sales_packing>();
            sales_return = new List<sales_return>();
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_opportunity { get; set; }
        public int id_contact { get; set; }
        public int id_currency { get; set; }
        public decimal value { get; set; }

        public virtual contact contact { get; set; }
        public virtual ICollection<sales_budget> sales_budget { get; set; }
        public virtual ICollection<sales_order> sales_order { get; set; }
        public virtual ICollection<sales_packing> sales_packing { get; set; }
        public virtual ICollection<sales_invoice> sales_invoice { get; set; }
        public virtual ICollection<sales_return> sales_return { get; set; }

        //public void Add(crm_opportunity crm_opportunity)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
