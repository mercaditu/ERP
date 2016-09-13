namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class production_execution_detail : Audit
    {
        public production_execution_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
            is_head = true;
            trans_date = DateTime.Now;
            is_input = false;
            child = new List<production_execution_detail>();
            production_execution_dimension = new List<production_execution_dimension>();
            start_date = DateTime.Now;
            end_date = DateTime.Now;
            item_movement = new List<item_movement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_execution_detail { get; set; }
        public int? id_order_detail { get; set; }
        public int? id_project_task { get; set; }

        public int? id_time_coefficient { get; set; }
        public int? id_contact { get; set; }
        public int? id_item { get; set; }
        public int? movement_id { get; set; }
        public string name { get; set; }
        public Status.Production? status { get; set; }

        [Required]
        public decimal quantity { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public decimal unit_cost { get; set; }

        [NotMapped]
        public decimal hours
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
            }
        }
        decimal _hour;

        [Required]
        public bool is_input { get; set; }

        [Required]
        public DateTime trans_date { get; set; }

        //Heirarchy
        public virtual production_execution_detail parent { get; set; }
        public virtual ICollection<production_execution_detail> child { get; set; }
        public virtual ICollection<production_execution_dimension> production_execution_dimension { get; set; }
        public virtual ICollection<production_account> production_account { get; set; }
        public virtual ICollection<item_movement> item_movement { get; set; }
        public virtual hr_time_coefficient hr_time_coefficient { get; set; }
        public virtual production_order_detail production_order_detail { get; set; }
        public virtual project_task project_task { get; set; }
        public virtual item item { get; set; }
        public virtual contact contact { get; set; }
    }
}
