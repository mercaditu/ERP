using System;
using System.Collections.Generic;
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
        public static decimal calculate_Vat(decimal unit_value, decimal rate)
        {
            decimal UnitValue_WithVAT = 0;
            if (rate > 0)
            {
                UnitValue_WithVAT = (rate * unit_value);
            }
            return UnitValue_WithVAT;
        }

        private static decimal calc_VATCoeficient(int id_vat_group)
        {
            decimal coefficient = 0;

            if (id_vat_group != 0)
            {
                using (db db = new db())
                {
                    List<app_vat_group_details> app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == id_vat_group).ToList();
                    foreach (app_vat_group_details app_vat_group in app_vat_group_details)
                    {
                        coefficient = coefficient + app_vat_group.app_vat.coefficient;
                    }
                }
            }

            return coefficient;
        }

        public static decimal return_ValueWithVAT(int id_vat_group, decimal ValueWithoutVAT)
        {
            decimal VAT_Value = 0;

            if (id_vat_group != 0)
            {
                using (db db = new db())
                {
                    List<app_vat_group_details> app_vat_group_details = new List<entity.app_vat_group_details>();
                    app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == id_vat_group).ToList();

                    foreach (app_vat_group_details app_vat_group in app_vat_group_details)
                    {
                        VAT_Value = VAT_Value + calculate_Vat(ValueWithoutVAT, app_vat_group.app_vat.coefficient);
                    }
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
                using (db db = new db())
                {
                    return db.app_vat_group.Where(i => i.is_default && i.id_company == Properties.Settings.Default.company_ID).FirstOrDefault().id_vat_group;
                }
            }
        }

        public static app_vat_group getItemVatgroup(item item)
        {
            if (item.app_vat_group != null)
            {
                return item.app_vat_group;
            }
            else
            {
                using (db db = new db())
                {
                    return db.app_vat_group.Where(i => i.is_default && i.id_company == Properties.Settings.Default.company_ID).FirstOrDefault();
                }
            }
        }
    }
}
