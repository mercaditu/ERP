namespace entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class purchase_invoice_dimension : Audit
    {
        public purchase_invoice_dimension()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_invoice_property { get; set; }
        public long id_purchase_invoice_detail { get; set; }
        public int id_dimension { get; set; }
        public decimal value { get; set; }
        public int id_measurement { get; set; }

        public virtual purchase_invoice_detail purchase_invoice_detail 
        { 
            get
            { 
             
                if (_purchase_invoice_detail != null)
                {
                    _purchase_invoice_detail.Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(_purchase_invoice_detail.item, _purchase_invoice_detail.quantity, _purchase_invoice_detail.GetDimensionValue(_purchase_invoice_detail.purchase_invoice_dimension));
                }
                return _purchase_invoice_detail;
            } 
            set
            {
                _purchase_invoice_detail = value;
                if (_purchase_invoice_detail!=null)
                {
                    _purchase_invoice_detail.Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(_purchase_invoice_detail.item, _purchase_invoice_detail.quantity, _purchase_invoice_detail.GetDimensionValue(_purchase_invoice_detail.purchase_invoice_dimension));        
                }
             
            } 
        }
        purchase_invoice_detail _purchase_invoice_detail;
        public virtual app_dimension app_dimension { get; set; }
        public virtual app_measurement app_measurement { get; set; }
    }

}
