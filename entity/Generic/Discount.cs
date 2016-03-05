using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public static class Discount
    {
        public static decimal Calculate_Discount(decimal oldDiscount, decimal value, decimal unit_cost)
        {
            decimal new_discount = oldDiscount - value;
            return unit_cost + new_discount;
        }
    }
}
