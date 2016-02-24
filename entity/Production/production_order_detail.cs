namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class production_order_detail : Audit
    {
        public production_order_detail()
        {
            id_company = Properties.Settings.Default.company_ID;
            id_user = Properties.Settings.Default.user_ID;
            is_head = true;
            child = new List<production_order_detail>();
            trans_date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_order_detail { get; set; }
        public int id_production_order { get; set; }
        public int? id_project_task { get; set; }

        public int? id_item { get; set; }
        public string name { get; set; }
        public decimal quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    RaisePropertyChanged("quantity");

                    foreach (production_order_detail production_order_detail in child)
                    {
                        using (db db = new db())
                        {
                            if (db.item_recepie.Where(x => x.id_item == id_item).FirstOrDefault() != null)
                            {
                                production_order_detail.quantity = db.item_recepie.Where(x => x.id_item == id_item)
                                    .FirstOrDefault()
                                    .item_recepie_detail
                                    .Where(x => x.id_item == production_order_detail.id_item)
                                    .FirstOrDefault()
                                    .quantity * quantity;

                                production_order_detail.RaisePropertyChanged("quantity");
                            }
                        }
                    }
                }
            }
        }

        private decimal _quantity;
        public bool is_input { get; set; }

        [Required]
        public DateTime trans_date { get; set; }

        public DateTime? start_date_est { get; set; }
        public DateTime? end_date_est { get; set; }

        //Self Referencing
        public virtual production_order_detail parent { get; set; }
        public virtual ICollection<production_order_detail> child { get; set; }
        public virtual IEnumerable<item_request_detail> item_request_detail { get; set; }
        public virtual production_order production_order { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual item item { get; set; }
    }
}
