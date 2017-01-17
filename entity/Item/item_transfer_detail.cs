
namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class item_transfer_detail : Audit
    {
        public item_transfer_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            item_movement = new List<item_movement>();
            item_transfer_dimension = new List<item_transfer_dimension>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_transfer_detail { get; set; }
        public Status.Documents_General status { get; set; }
        public Status.Documents_General status_dest { get; set; }
        public int id_transfer { get; set; }
        public int? id_project_task { get; set; }
        public int id_item_product { get; set; }
        public int? movement_id { get; set; }

        public decimal quantity_origin 
        {
            get { return _quantity_origin; }
            set
            {
                if (_quantity_origin != value)
                {
                    _quantity_origin = value;
                    RaisePropertyChanged("quantity_origin");

                    //Updates the Destination automatically. 
                    quantity_destination = _quantity_origin;
                }
            }
        }
        decimal _quantity_origin;

        [NotMapped]
        public decimal Quantity_InStock
        {
            get { return _Quantity_InStock; }
            set
            {
                if (_Quantity_InStock != value)
                {
                    _Quantity_InStock = value;
                    RaisePropertyChanged("Quantity_InStock");

                    InStock = quantity_origin <= _Quantity_InStock ? true : false;
                    RaisePropertyChanged("InStock");
                }
            }
        }
        private decimal _Quantity_InStock;

        [NotMapped]
        public bool InStock { get; set; }

        public decimal quantity_destination 
        {
            get { return _quantity_destination; }
            set
            {
                if (_quantity_destination != value)
                {
                    _quantity_destination = value;
                    RaisePropertyChanged("quantity_destination");
                }
            }
        }
        decimal _quantity_destination;


        public DateTime expire_date { get; set; }
        public string batch_code { get; set; }

        public virtual item_transfer item_transfer { get; set; }
        public virtual item_product item_product { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual ICollection<item_movement> item_movement { get; set; }
        public virtual ICollection<item_transfer_dimension> item_transfer_dimension { get; set; }

        public void Calulate_Stock()
        {
            //entity.Brillo.
        }
   
    }
}
