using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public static class Discount
    {
        public static decimal Calculate_Discount(int id, decimal oldDiscount, decimal value, decimal unit_cost)
        {


            if (id > 0)
            {
                return value;

            }
            else
            {
                decimal new_discount = oldDiscount - value;
                return unit_cost + new_discount;

            }




        }
    }
}
