namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using System.Linq;
    public partial class item_movement : Audit
    {
        public item_movement()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            item_movement_value = new List<item_movement_value>();
            item_movement_dimension = new List<item_movement_dimension>();
            timestamp = DateTime.Now;
            trans_date = DateTime.Now;
            debit = 0;
            credit = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id_movement { get; set; }
        public int id_item_product { get; set; }
        public int? id_transfer_detail { get; set; }
        public int? id_execution_detail { get; set; }
        public int? id_purchase_invoice_detail { get; set; }
        public int? id_purchase_return_detail { get; set; }
        public long? id_sales_invoice_detail { get; set; }
        public int? id_sales_return_detail { get; set; }
        public int? id_inventory_detail { get; set; }
        public int? id_sales_packing_detail { get; set; }
        public int id_location { get; set; }
        public Status.Stock status { get; set; }
        public decimal debit { get; set; }
        public decimal credit { get; set; }
        public string comment { get; set; }
        public string code { get; set; }
        public DateTime? expire_date { get; set; }
        public DateTime trans_date { get; set; }




        //Heirarchy For Movement
        public virtual ICollection<item_movement> _child { get; set; }
        public virtual item_movement _parent { get; set; }

        public virtual app_location app_location { get; set; }
        public virtual item_product item_product { get; set; }
        public virtual sales_packing_detail sales_packing_detail { get; set; }
        public virtual item_transfer_detail item_transfer_detail { get; set; }
        public virtual production_execution_detail production_execution_detail { get; set; }
        public virtual purchase_invoice_detail purchase_invoice_detail { get; set; }
        public virtual purchase_return_detail purchase_return_detail { get; set; }
        public virtual sales_invoice_detail sales_invoice_detail { get; set; }
        public virtual sales_return_detail sales_return_detail { get; set; }
        public virtual ICollection<item_movement_value> item_movement_value { get; set; }
        public virtual ICollection<item_movement_dimension> item_movement_dimension { get; set; }

        public decimal GetValue_ByCurrency(app_currency app_currency)
        {
            decimal Value = 0M;

            foreach (item_movement_value item_movement_valueLIST in item_movement_value)
            {
                if (item_movement_valueLIST.app_currencyfx.app_currency == app_currency)
                {
                    Value = Value + item_movement_valueLIST.unit_value;
                }
                else
                {
                    //Take value in that currency fx. do not convert into new fx rate.
                    app_currencyfx app_currencyfx = item_movement_valueLIST.app_currencyfx;

                    //convert into current currency.
                    if (app_currency.app_currencyfx.Where(x => x.is_active).FirstOrDefault() != null)
                    {
                        Value = Value + Brillo.Currency.convert_Values(item_movement_valueLIST.unit_value, app_currency.app_currencyfx.Where(x => x.is_active).FirstOrDefault().id_currencyfx, app_currencyfx.id_currencyfx, App.Modules.Purchase);
                       // Value = Value + Brillo.Currency.convert_Value(item_movement_valueLIST.unit_value, app_currencyfx.id_currencyfx, App.Modules.Purchase);
                    }

                }
            }

            return Value;
        }
    }
}
