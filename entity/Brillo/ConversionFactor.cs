using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo
{
    public static class ConversionFactor
    {
        public static decimal Factor_Quantity_Back(item item, decimal Quantity_Factored, decimal BaseDimension)
        {
            if (item != null)
            {
                if (item.item_product.FirstOrDefault() != null)
                {
                    if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault() != null)
                    {
                        if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value > 0)
                        {
                            if (item.item_dimension.Count() > 0)
                            {
                                //decimal i = 1M;
                                //foreach (item_dimension item_dimension in item.item_dimension)
                                //{
                                //    if (item_dimension.value > 0)
                                //    {
                                //        i = i * item_dimension.value;
                                //    }
                                //}
                                return Quantity_Factored / (BaseDimension * item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value);
                            }
                            else
                            {
                                return Quantity_Factored / item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value;
                            }
                        }
                    }
                }
            }

            return Quantity_Factored;
        }

        public static decimal Factor_Quantity(item item, decimal Quantity,Decimal BaseDimension)
        {
            if (item != null)
            {
                if (item.item_product.FirstOrDefault() != null)
                {
                    if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault() != null)
                    {
                        if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value > 0)
                        {
                            if (item.item_dimension.Count()>0)
                            {
                                //decimal i = 1M;
                                //foreach (Dimension item_dimension in BaseDimension)
                                //{
                                //    if (item_dimension.value > 0)
                                //    {
                                //        i = i * item_dimension.value;
                                //    }
                                //}

                                return Quantity * (BaseDimension * item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value);
                            }
                            else
                            {
                                return Quantity * item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value;
                            }
                        }
                    }
                }
            }

            return Quantity;
        }
    }
}
