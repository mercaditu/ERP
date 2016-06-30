
namespace entity
{
    using entity.Brillo;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class impex_expense : Audit
    {
        public impex_expense()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_impex_expense { get; set; }
        public int id_impex { get; set; }
        public int? id_purchase_invoice { get; set; }
        public int id_incoterm_condition { get; set; }
        public decimal value { get; set; }
        public int id_currencyfx { 
            get {
               return _id_currencyfx ;
            }
            set
            {
                if (_id_currencyfx != value)
                {
                    if (State != System.Data.Entity.EntityState.Unchanged && State > 0)
                    {
                        this.value = Currency.convert_Values(this.value, _id_currencyfx, value, App.Modules.Purchase);
                        RaisePropertyChanged("value");


                    }
                    _id_currencyfx = value;
                }
            }
        }
        int _id_currencyfx;
        [NotMapped]
        public int id_item { get; set; }
        public virtual impex impex { get; set; }
        public virtual impex_incoterm_condition impex_incoterm_condition { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
    }
}
