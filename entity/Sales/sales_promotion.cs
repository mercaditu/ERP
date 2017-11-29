namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class sales_promotion : Audit
    {
        public enum salesPromotion
        {
            Discount_onGrandTotal = 1,

            //Discount_onQuantityTotal = 2,
            //Discount_onQuantityRow = 3,
            Discount_onTag = 4,

            //Discount_onBrand = 5,
            Discount_onItem = 6,

            BuyThis_GetThat = 7,
            BuyTag_GetThat = 8,
            Discount_onCustomerType = 10
            //BuyThis_Discount_OnSecond = 9
        }

        public sales_promotion()
        {
            is_head = true;
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            date_start = DateTime.Now;
            date_end = DateTime.Now.AddDays(10);
            quantity_step = 1;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_sales_promotion { get; set; }

        public salesPromotion type { get; set; }
        public string name { get; set; }

        public int reference
        {
            get
            {
                return _refrence;
            }
            set
            {
                _refrence = value;
            }
        }

        private int _refrence;
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public decimal quantity_min { get; set; }
        public decimal quantity_max { get; set; }
        public decimal quantity_step { get; set; }
        public bool is_percentage { get; set; }
        public decimal result_value { get; set; }
        public decimal result_step { get; set; }
        public int reference_bonus { get; set; }

        [NotMapped]
        public string InputName
        {
            get
            {
                using (db db = new db())
                {
                    if (type==salesPromotion.BuyThis_GetThat || type == salesPromotion.Discount_onItem)
                    {
                        entity.item item = db.items.Where(x => x.id_item == reference).FirstOrDefault();
                        if (item != null)
                        {
                            _InputName = item.name;

                        }
                    }
                  


                }
                return _InputName;
            }
            set
            {
                _InputName = value;
            }
        }
        string _InputName;

        [NotMapped]
        public string OutputName
        {
            get
            {
                using (db db = new db())
                {
                    if (type == salesPromotion.BuyThis_GetThat || type==salesPromotion.BuyTag_GetThat)
                    {
                        entity.item item = db.items.Where(x => x.id_item == reference_bonus).FirstOrDefault();
                        if (item != null)
                        {
                            _OutputName = item.name;

                        }
                    }



                }
                return _OutputName;
            }
            set
            {
                _OutputName = value;
            }
        }
        string _OutputName;
    }
}