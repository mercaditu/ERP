namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public partial class item_movement : Audit
    {
        public item_movement()
        {
            id_company = CurrentSession.Id_Company;
            id_user =  CurrentSession.Id_User;
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
        public App.Names id_application { get; set; }
        public int? id_transfer { get; set; }
        public int? id_production_execution { get; set; }
        public int? id_purchase_invoice { get; set; }
        public int? id_purchase_return { get; set; }
        public int? id_sales_invoice { get; set; }
        public int? id_sales_return { get; set; }
        public int? id_inventory { get; set; }
        public int? id_sales_packing { get; set; }
        public int transaction_id { get; set; }
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
        public virtual sales_packing sales_packing { get; set; }
        public virtual item_transfer item_transfer { get; set; }
        public virtual production_execution production_execution { get; set; }
        public virtual purchase_invoice purchase_invoice { get; set; }
        public virtual purchase_return purchase_return { get; set; }
        public virtual sales_invoice sales_invoice { get; set; }
        public virtual sales_return sales_return { get; set; }
        public virtual ICollection<item_movement_value> item_movement_value { get; set; }
        public virtual ICollection<item_movement_dimension> item_movement_dimension { get; set; }

        public decimal GetValue_ByCurrency(app_currency app_currency)
        {
            decimal Value = 0M;

            foreach (item_movement_value item_movement_valueLIST in item_movement_value)
            {
                if(item_movement_valueLIST.app_currencyfx.app_currency == app_currency)
                {
                    Value = Value + item_movement_valueLIST.unit_value;
                }
                else
                {
                    //Take value in that currency fx. do not convert into new fx rate.
                    app_currencyfx app_currencyfx = item_movement_valueLIST.app_currencyfx;

                    //convert into current currency.
                    Value = Value + Brillo.Currency.convert_Value(item_movement_valueLIST.unit_value, app_currencyfx.id_currencyfx, App.Modules.Purchase);
                }
            }

            return Value;
        }
    }
}
