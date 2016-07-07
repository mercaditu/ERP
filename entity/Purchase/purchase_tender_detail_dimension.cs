namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class purchase_tender_detail_dimension : Audit
    {
        public purchase_tender_detail_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_tender_detail_property { get; set; }
        public int id_purchase_tender_detail { get; set; }
        public int id_dimension { get; set; }
        public int id_measurement { get; set; }
        public decimal value { get; set; }

      
        public virtual purchase_tender_detail purchase_tender_detail
        {
            get
            {

                if (_purchase_tender_detail != null)
                {
                    _purchase_tender_detail.Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(_purchase_tender_detail.purchase_tender_item.item, _purchase_tender_detail.quantity, _purchase_tender_detail.GetDimensionValue());
                }
                return _purchase_tender_detail;
            }
            set
            {
                _purchase_tender_detail = value;
                if (_purchase_tender_detail != null)
                {
                    _purchase_tender_detail.Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(_purchase_tender_detail.purchase_tender_item.item, _purchase_tender_detail.quantity, _purchase_tender_detail.GetDimensionValue());
                }

            }
        }
        purchase_tender_detail _purchase_tender_detail;
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }
}
