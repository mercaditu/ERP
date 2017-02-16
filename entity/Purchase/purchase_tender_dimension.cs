namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class purchase_tender_dimension : Audit
    {
        public purchase_tender_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_tender_property { get; set; }

        public int id_purchase_tender_item { get; set; }
        public int id_dimension { get; set; }
        public int id_measurement { get; set; }
        public decimal value { get; set; }

        public virtual purchase_tender_item purchase_tender_item
        {
            get
            {
                return _purchase_tender_item;
            }
            set
            {
                _purchase_tender_item = value;
            }
        }

        private purchase_tender_item _purchase_tender_item;

        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }
}