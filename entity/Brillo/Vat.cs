using System.Linq;

namespace entity.Brillo
{
    public static class Vat
    {
        /// <summary>
        /// Calculates the Vat of an Item based on conditions such as Unit Value and Rate.
        /// This is strictly for manual calculations. For automatic calculations, please check getVat Methods.
        /// </summary>
        /// <param name="unit_value">Unit Value of Item</param>
        /// <param name="rate">Rate at which to Calculate</param>
        /// <returns>Returns Unit Value (decimal) for the VAT of that Item.</returns>
        public static decimal calculate_Vat(decimal unit_value, decimal participation, decimal rate)
        {
            decimal UnitValue_WithVAT = 0;
            if (rate > 0)
            {
                UnitValue_WithVAT = (rate * (unit_value * participation));
            }
            return UnitValue_WithVAT;
        }

        private static decimal calc_VATCoeficient(int id_vat_group)
        {
            decimal coefficient = 0;

            if (id_vat_group != 0)
            {
                foreach (app_vat_group_details app_vat_group_details in CurrentSession.VAT_GroupDetails.Where(x => x.id_vat_group == id_vat_group))
                {
                    coefficient = coefficient + (CurrentSession.VATs.Where(x => x.id_vat == app_vat_group_details.id_vat).FirstOrDefault().coefficient * app_vat_group_details.percentage);
                }
            }

            return coefficient;
        }

        public static decimal return_ValueWithVAT(int id_vat_group, decimal ValueWithoutVAT)
        {
            decimal VAT_Value = 0;

            if (id_vat_group != 0)
            {
                foreach (app_vat_group_details app_vat_group_detail in CurrentSession.VAT_GroupDetails.Where(x => x.id_vat_group == id_vat_group).ToList())
                {
                    //Run If based on VAT Type
                    //if (app_vat_group_detail.app_vat != null)
                    //{
                    //    if (app_vat_group_detail.app_vat.on_product)
                    //    {
                    //        VAT_Value = VAT_Value + calculate_Vat(ValueWithoutVAT, app_vat_group_detail.percentage, app_vat_group_detail.app_vat.coefficient);
                    //    }
                    //    else if (app_vat_group_detail.app_vat.on_branch)
                    //    {
                    //        if (CurrentSession.Id_Branch > 0)
                    //        {
                    //            int? VatID = CurrentSession.Branches.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault().id_vat;
                    //        }
                    //    }
                    //    else
                    //    {
                    //      
                    //    }
                    //}

                    VAT_Value = VAT_Value + calculate_Vat(ValueWithoutVAT, app_vat_group_detail.percentage, app_vat_group_detail.app_vat.coefficient);
                }
            }

            return ValueWithoutVAT + VAT_Value;
        }

        public static decimal return_ValueWithoutVAT(int id_vat_group, decimal ValueWithVAT)
        {
            decimal VAT_Value = 0;

            if (id_vat_group != 0)
            {
                return ValueWithVAT / (1 + calc_VATCoeficient(id_vat_group));
            }

            return VAT_Value;
        }

        public static int getItemVat(item item)
        {
            if (item.app_vat_group != null)
            {
                return item.id_vat_group;
            }
            else
            {
                return CurrentSession.VAT_Groups.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group;
            }
        }
    }
}