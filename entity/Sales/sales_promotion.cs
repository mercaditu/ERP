namespace entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
        public int reference { get; set; }

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
        public string InputName { get; set; }
        [NotMapped]
        public string OutputName { get; set; }
    }
}
