﻿using System;
using System.Linq;

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
                                if ((BaseDimension * item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value) > 0)
                                {
                                    return Quantity_Factored / (BaseDimension * item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value);
                                }
                                else
                                {
                                    if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value > 0)
                                    {
                                        return Quantity_Factored / item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value;
                                    }
                                }
                            }
                            else
                            {
                                if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value > 0)
                                {
                                    return Quantity_Factored / item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value;
                                }
                            }
                        }
                    }
                }
            }

            return Quantity_Factored;
        }

        public static decimal Factor_Quantity(item item, decimal Quantity, Decimal BaseDimension)
        {
            if (item != null)
            {
                if (item.item_product.FirstOrDefault() != null)
                {
                    if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault() != null)
                    {
                        if (item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value > 0)
                        {
                            if (BaseDimension > 0)
                            {
                                //decimal i = 1M;
                                //foreach (Dimension item_dimension in BaseDimension)
                                //{
                                //    if (item_dimension.value > 0)
                                //    {
                                //        i = i * item_dimension.value;
                                //    }
                                //}

                                decimal i = Quantity * (BaseDimension * item.item_product.FirstOrDefault().item_conversion_factor.FirstOrDefault().value);
                                return i;
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