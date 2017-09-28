using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public partial class item_movement_value_rel : Audit
    {
        public item_movement_value_rel()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            is_read = false;
            timestamp = DateTime.Now;
            item_movement = new List<item_movement>();
            item_movement_value_detail = new List<item_movement_value_detail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement_value_rel { get; set; }
        public bool is_estimate { get; set; }
        public string batch_code { get; set; }
        public DateTime? entry_date { get; set; }
        public decimal total_value
        {
            get
            {


                _total_value= item_movement_value_detail.Sum(x => x.unit_value);
                return _total_value;

            }
            set
            {
                _total_value = value; ;
            }
        }
        decimal _total_value;


        public virtual ICollection<item_movement_value_detail> item_movement_value_detail { get; set; }
        public virtual ICollection<item_movement> item_movement { get; set; }
    }
}
