using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public partial class refinance
    {

        public payment_schedual payment_schedual { get; set; }
            public DateTime Date { get; set; }
       
            public decimal Value { get; set; }
        
    }
}
