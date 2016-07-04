
namespace entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class purchase_tender_item : Audit
    {
        public purchase_tender_item()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            purchase_tender_dimension = new List<purchase_tender_dimension>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_purchase_tender_item { get; set; }
        public int id_purchase_tender { get; set; }
        public int? id_item { get; set; }
        public int? id_project_task { get; set; }
        public string item_description { get; set; }
        public decimal quantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public decimal Quantity_Factored
        {
            get { return _Quantity_Factored; }
            set
            {
                if (_Quantity_Factored != value)
                {
                    _Quantity_Factored = value;
                    RaisePropertyChanged("Quantity_Factored");

                    quantity = Brillo.ConversionFactor.Factor_Quantity_Back(item, Quantity_Factored);
                    RaisePropertyChanged("quantity");
                }
            }
        }
        private decimal _Quantity_Factored;

        public virtual item item { get; set; }
        public virtual purchase_tender purchase_tender { get; set; }
        public virtual IEnumerable<purchase_tender_detail> purchase_tender_detail { get; set; }
        public virtual ICollection<purchase_tender_dimension> purchase_tender_dimension { get; set; }
        public virtual project_task project_task { get; set; }
    }
}
