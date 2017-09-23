using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public partial class item_movement_value_detail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement_value_detail { get; set; }
        public long id_movement_value_rel { get; set; }

        public decimal unit_value { get; set; }
        public string comment { get; set; }

      public item_movement_value_rel item_movement_value_rel { get; set; }
    }
}
