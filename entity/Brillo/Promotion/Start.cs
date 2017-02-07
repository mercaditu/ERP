using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace entity.Brillo.Promotion
{
    public class Start
    {
        public List<sales_promotion> SalesPromotionLIST = new List<sales_promotion>();
        public List<Detail> DetailLIST = new List<Detail>();

        public Start(bool Run)
        {
            if (Run)
            {
                using (db db = new db())
                {
                    SalesPromotionLIST = db.sales_promotion.Where(x => x.date_start <= DateTime.Now && x.date_end >= DateTime.Now).ToList();
                }
            }
        }

        public void Calculate_SalesInvoice(ref sales_invoice SalesInvoice)
        {
            Invoice Invoice = new Invoice();
            Invoice.Contact = SalesInvoice.contact;
            Invoice.Date = SalesInvoice.trans_date;
            Invoice.GrandTotal = SalesInvoice.GrandTotal;

            foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
            {
                Detail Detail = new Detail();
                Detail.DetailID = _Detail.id_sales_invoice_detail;
                Detail.Item = _Detail.item;
                Detail.Quantity = _Detail.quantity;
                Detail.Price = _Detail.unit_price;
                Detail.PriceVAT = _Detail.UnitPrice_Vat;
                Detail.SubTotal = _Detail.SubTotal;
                Detail.SubTotalVAT = _Detail.SubTotal_Vat;
                Detail.is_promo = _Detail.IsPromo;
                Invoice.Details.Add(Detail);
            }

            sales_promotion promo = SalesPromotionLIST.Where(x => x.type == sales_promotion.salesPromotion.Discount_onCustomerType).FirstOrDefault();
            if (promo != null)
            {
                Discount_onCustomerType(promo, Invoice, SalesInvoice);
            }
            else
            {
                foreach (var Promo in SalesPromotionLIST)
                {
                    BuyThis_GetThat(Promo, Invoice);
                    Discount_onItem(Promo, Invoice);
                    Discount_onTag(Promo, Invoice);
                    Discount_onGrandTotal(Promo, Invoice);
                }

                //Logic to see which promotion is best.
                foreach (Detail detail in Invoice.Details)
                {
                    //for each row or item, see which has best discount.
                    Detail best = DetailLIST.Where(x => x.DetailID == detail.DetailID && x.Discount == DetailLIST.Max(y => y.Discount)).FirstOrDefault();
                    detail.is_promo = true;
                    detail.Discount = best.Discount;
                    detail.DiscountVAT = best.DiscountVAT;
                    detail.PromotionID = best.PromotionID;

                    //Assign to Sales Detail...

                }
            }
        }

        private void BuyThis_GetThat(sales_promotion Promo, Invoice Invoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.BuyThis_GetThat)
            {
                if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step && x.is_promo == false).Count() > 0)
                {

                    Invoice PromotionInvoice = new Invoice();
                    PromotionInvoice.Contact = Invoice.Contact;
                    PromotionInvoice.Date = Invoice.Date;
                    PromotionInvoice.GrandTotal = Invoice.GrandTotal;
                    foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                    {
                        if (Promo.quantity_step > 0)
                        {
                            Promo _Promo = new Promo();
                            _Promo.Type = sales_promotion.salesPromotion.BuyThis_GetThat;
                            _Promo.Shared = true;

                            _Detail.Promos.Add(_Promo);

                            List<sales_invoice_detail> sid = SalesInvoice.sales_invoice_detail.Where(x => x.id_item == Promo.reference_bonus && x.IsPromo).ToList();
                            //Prevent double clicking button and adding extra bonus to sale. find better way to implement. Short term code.

                            if (sid.Count == 0)
                            {
                                sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();

                                //Needed to calculate the discounts and unit prices further on.
                                sales_invoice_detail.State = System.Data.Entity.EntityState.Added;

                                using (db db = new db())
                                {
                                    item item = db.items.Where(x => x.id_item == Promo.reference_bonus).FirstOrDefault();
                                    if (item != null)
                                    {
                                        sales_invoice_detail.id_vat_group = item.id_vat_group;
                                        sales_invoice_detail.id_item = item.id_item;
                                        sales_invoice_detail.item_description = item.name;
                                        //sales_invoice_detail.item = item;
                                    }

                                    item_price item_price = item.item_price.Where(x => x.item_price_list.is_default == true).FirstOrDefault();
                                    if (item_price != null)
                                    {
                                        sales_invoice_detail.unit_price = item_price.value;
                                        sales_invoice_detail.discount = item_price.value;
                                    }
                                }

                                sales_invoice_detail.IsPromo = true;

                                sales_invoice_detail.quantity = Math.Floor(_Detail.Quantity / Promo.quantity_step);
                                SalesInvoice.sales_invoice_detail.Add(sales_invoice_detail);
                                Detail Detail = new Detail();
                                Detail.DetailID = sales_invoice_detail.id_sales_invoice_detail;
                                Detail.Item = sales_invoice_detail.item;
                                Detail.Quantity = sales_invoice_detail.quantity;
                                Detail.Price = sales_invoice_detail.unit_price;
                                Detail.PriceVAT = sales_invoice_detail.UnitPrice_Vat;
                                Detail.Discount = sales_invoice_detail.discount;
                                Detail.DiscountVAT = sales_invoice_detail.DiscountVat;
                                Detail.SubTotal = sales_invoice_detail.SubTotal;
                                Detail.SubTotalVAT = sales_invoice_detail.SubTotal_Vat;
                                Detail.is_promo = sales_invoice_detail.IsPromo;
                                PromotionInvoice.Details.Add(Detail);
                            }

                        }
                    }

                    if (PromotionInvoice.Details.Count() > 0)
                    {
                        SalesLIST.Add(PromotionInvoice);
                    }
                }
            }
        }

        private void BuyTag_GetThat(sales_promotion Promo, Invoice Invoice, sales_invoice SalesInvoice)
        {


            if (Promo.type == sales_promotion.salesPromotion.BuyTag_GetThat)
            {




                decimal TotalQuantity = 0;

                List<Detail> DetailList = new List<Detail>();
                List<DetailTag> DetailTagList = new List<DetailTag>();

                using (db db = new db())
                {
                    DetailList = Invoice.Details.Where(x => x.Item.item_tag_detail.Any(y => y.id_tag == Promo.reference) && x.is_promo == false).ToList();
                    if (DetailList.Count() > 0)
                    {
                        DetailTag DetailTag = new DetailTag();
                        DetailTag.Tag = Promo.reference;
                        DetailTag.Quantity = DetailList.Sum(x => x.Quantity);
                        DetailTagList.Add(DetailTag);
                    }
                    TotalQuantity = DetailList.Sum(x => x.Quantity);

                }


                if (DetailTagList.Count() > 0 && TotalQuantity >= Promo.quantity_step)
                {
                    foreach (DetailTag _DetailTag in DetailTagList)
                    {
                        Promo _Promo = new Promo();
                        _Promo.Type = sales_promotion.salesPromotion.BuyTag_GetThat;
                        _Promo.Shared = true;



                        List<sales_invoice_detail> sid = SalesInvoice.sales_invoice_detail.Where(x => x.item.item_tag_detail.Any(y => y.id_tag == Promo.reference_bonus) && x.IsPromo).ToList();
                        //Prevent double clicking button and adding extra bonus to sale. find better way to implement. Short term code.
                        if (sid.Count == 0)
                        {



                            PromotionProduct window = new PromotionProduct()
                            {
                                Title = "Modal Dialog",
                                ShowInTaskbar = false,               // don't show the dialog on the taskbar
                                Topmost = true,                      // ensure we're Always On Top
                                ResizeMode = ResizeMode.NoResize,    // remove excess caption bar buttons
                                TagID = Promo.reference_bonus,
                                TotalQuantity = Math.Floor(_DetailTag.Quantity / Promo.quantity_step),
                            };

                            window.ShowDialog();

                            List<DetailProduct> DetailProduct = window.ProductList;
                            if (DetailProduct != null)
                            {
                                Invoice PromotionInvoice = new Invoice();
                                PromotionInvoice.Contact = SalesInvoice.contact;
                                PromotionInvoice.Date = SalesInvoice.trans_date;
                                PromotionInvoice.GrandTotal = SalesInvoice.GrandTotal;
                                foreach (DetailProduct _DetailProduct in DetailProduct)
                                {
                                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();

                                    //Needed to calculate the discounts and unit prices further on.
                                    sales_invoice_detail.State = System.Data.Entity.EntityState.Added;

                                    using (db db = new db())
                                    {
                                        item item = db.items.Where(x => x.id_item == _DetailProduct.ProductId).FirstOrDefault();
                                        if (item != null)
                                        {
                                            sales_invoice_detail.id_vat_group = item.id_vat_group;
                                            sales_invoice_detail.id_item = item.id_item;
                                            sales_invoice_detail.item_description = item.name;
                                        }

                                        item_price item_price = item.item_price.Where(x => x.item_price_list.is_default == true).FirstOrDefault();
                                        if (item_price != null)
                                        {
                                            sales_invoice_detail.unit_price = item_price.value;
                                            sales_invoice_detail.discount = item_price.value;
                                        }
                                    }

                                    sales_invoice_detail.IsPromo = true;

                                    sales_invoice_detail.quantity = _DetailProduct.Quantity;
                                    SalesInvoice.sales_invoice_detail.Add(sales_invoice_detail);
                                    Detail Detail = new Detail();
                                    Detail.DetailID = sales_invoice_detail.id_sales_invoice_detail;
                                    Detail.Item = sales_invoice_detail.item;
                                    Detail.Quantity = sales_invoice_detail.quantity;
                                    Detail.Price = sales_invoice_detail.unit_price;
                                    Detail.PriceVAT = sales_invoice_detail.UnitPrice_Vat;
                                    Detail.Discount = sales_invoice_detail.discount;
                                    Detail.DiscountVAT = sales_invoice_detail.DiscountVat;
                                    Detail.SubTotal = sales_invoice_detail.SubTotal;
                                    Detail.SubTotalVAT = sales_invoice_detail.SubTotal_Vat;
                                    Detail.is_promo = sales_invoice_detail.IsPromo;
                                    PromotionInvoice.Details.Add(Detail);
                                }
                                SalesLIST.Add(PromotionInvoice);

                            }
                        }





                    }
                }
            }
        }

        private void Discount_onItem(sales_promotion Promo, Invoice Invoice, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onItem)
            {
                if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step && x.is_promo == false).Count() > 0)
                {
                    Invoice PromotionInvoice = new Invoice();
                    PromotionInvoice.Contact = SalesInvoice.contact;
                    PromotionInvoice.Date = SalesInvoice.trans_date;
                    PromotionInvoice.GrandTotal = SalesInvoice.GrandTotal;
                    foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                    {
                        if (Promo.quantity_step > 0)
                        {
                            Promo _Promo = new Promo();
                            _Promo.Type = sales_promotion.salesPromotion.Discount_onItem;
                            _Promo.Shared = true;

                            _Detail.Promos.Add(_Promo);
                            foreach (sales_invoice_detail _Detail_ in SalesInvoice.sales_invoice_detail.Where(x => x.id_item == Promo.reference_bonus && x.IsPromo))
                            {
                                _Detail_.DiscountVat = 0;
                                _Detail_.IsPromo = false;
                            }

                            sales_invoice_detail sales_invoice_detail = SalesInvoice.sales_invoice_detail.Where(x => x.id_item == Promo.reference && x.IsPromo == false).FirstOrDefault();

                            if (sales_invoice_detail != null)
                            {
                                sales_invoice_detail.IsPromo = true;
                                sales_invoice_detail.DiscountVat = sales_invoice_detail.UnitPrice_Vat * Promo.result_value;
                                Detail Detail = new Detail();
                                Detail.DetailID = sales_invoice_detail.id_sales_invoice_detail;
                                Detail.Item = sales_invoice_detail.item;
                                Detail.Quantity = sales_invoice_detail.quantity;
                                Detail.Price = sales_invoice_detail.unit_price;
                                Detail.PriceVAT = sales_invoice_detail.UnitPrice_Vat;
                                Detail.Discount = sales_invoice_detail.discount;
                                Detail.DiscountVAT = sales_invoice_detail.DiscountVat;
                                Detail.SubTotal = sales_invoice_detail.SubTotal;
                                Detail.SubTotalVAT = sales_invoice_detail.SubTotal_Vat;
                                Detail.is_promo = sales_invoice_detail.IsPromo;
                                PromotionInvoice.Details.Add(Detail);
                            }
                        }
                    }
                    SalesLIST.Add(PromotionInvoice);
                }
            }
        }

        private void Discount_onTag(sales_promotion Promo, Invoice Invoice, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onTag)
            {
                decimal TotalQuantity = 0;

                List<Detail> DetailList = new List<Detail>();
                List<DetailTag> DetailTagList = new List<DetailTag>();

                using (db db = new db())
                {
                    DetailList = Invoice.Details.Where(x => x.Item.item_tag_detail.Any(y => y.id_tag == Promo.reference) && x.is_promo == false).ToList();
                    if (DetailList.Count() > 0)
                    {
                        DetailTag DetailTag = new DetailTag();
                        DetailTag.Tag = Promo.reference;
                        DetailTag.Quantity = DetailList.Sum(x => x.Quantity);
                        DetailTagList.Add(DetailTag);
                    }
                    TotalQuantity = DetailList.Sum(x => x.Quantity);
                }
                foreach (sales_invoice_detail _Detail_ in SalesInvoice.sales_invoice_detail.Where(x => x.item.item_tag_detail.Any(y => y.id_tag == Promo.reference_bonus) && x.IsPromo))
                {
                    _Detail_.DiscountVat = 0;
                    _Detail_.IsPromo = false;
                }
            

             

                if (DetailTagList.Count() > 0 && TotalQuantity >= Promo.quantity_step)
                {
                    Invoice PromotionInvoice = new Invoice();
                    PromotionInvoice.Contact = SalesInvoice.contact;
                    PromotionInvoice.Date = SalesInvoice.trans_date;
                    PromotionInvoice.GrandTotal = SalesInvoice.GrandTotal;
                    foreach (DetailTag _DetailTag in DetailTagList)
                    {
                        Promo _Promo = new Promo();
                        _Promo.Type = sales_promotion.salesPromotion.BuyTag_GetThat;
                        _Promo.Shared = true;

                        List<sales_invoice_detail> sidpromo = SalesInvoice.sales_invoice_detail.Where(x => x.item.item_tag_detail.Any(y => y.id_tag == Promo.reference) && x.IsPromo == false).ToList();
                        //Prevent double clicking button and adding extra bonus to sale. find better way to implement. Short term code.
                        foreach (sales_invoice_detail _Detail in sidpromo)
                        {
                            _Detail.IsPromo = true;
                            _Detail.DiscountVat = _Detail.UnitPrice_Vat * Promo.result_value;

                            Detail Detail = new Detail();
                            Detail.DetailID = _Detail.id_sales_invoice_detail;
                            Detail.Item = _Detail.item;
                            Detail.Quantity = _Detail.quantity;
                            Detail.Price = _Detail.unit_price;
                            Detail.PriceVAT = _Detail.UnitPrice_Vat;
                            Detail.Discount = _Detail.discount;
                            Detail.DiscountVAT = _Detail.DiscountVat;
                            Detail.SubTotal = _Detail.SubTotal;
                            Detail.SubTotalVAT = _Detail.SubTotal_Vat;
                            Detail.is_promo = _Detail.IsPromo;
                            PromotionInvoice.Details.Add(Detail);
                        }

                    }
                    SalesLIST.Add(PromotionInvoice);
                }
              
            }
        }
        private void Discount_onGrandTotal(sales_promotion Promo, Invoice Invoice, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onGrandTotal)
            {
                if (Promo.reference == SalesInvoice.app_currencyfx.id_currency)
                {


                    if (Promo.quantity_step <= Invoice.GrandTotal)
                    {
                        SalesInvoice.DiscountPercentage = Promo.result_value;

                        Invoice PromotionInvoice = new Invoice();
                        PromotionInvoice.Contact = SalesInvoice.contact;
                        PromotionInvoice.Date = SalesInvoice.trans_date;
                        PromotionInvoice.GrandTotal = SalesInvoice.GrandTotal;

                        foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                        {
                            Detail Detail = new Detail();
                            Detail.DetailID = _Detail.id_sales_invoice_detail;
                            Detail.Item = _Detail.item;
                            Detail.Quantity = _Detail.quantity;
                            Detail.Price = _Detail.unit_price;
                            Detail.PriceVAT = _Detail.UnitPrice_Vat;
                            Detail.Discount = _Detail.discount;
                            Detail.DiscountVAT = _Detail.DiscountVat;
                            Detail.SubTotal = _Detail.SubTotal;
                            Detail.SubTotalVAT = _Detail.SubTotal_Vat;
                            Detail.is_promo = _Detail.IsPromo;
                            PromotionInvoice.Details.Add(Detail);
                        }
                        SalesLIST.Add(PromotionInvoice);



                    }
                }





            }
        }
        private void Discount_onCustomerType(sales_promotion Promo, Invoice Invoice, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onCustomerType)
            {
                if (SalesInvoice.sales_invoice_detail.Where(x => x.IsPromo).Count() > 0)
                {
                    SalesInvoice.DiscountPercentage = 0;

                    foreach (sales_invoice_detail _Detail_ in SalesInvoice.sales_invoice_detail)
                    {
                        _Detail_.IsPromo = false;

                    }
                }

                if (SalesInvoice.contact.contact_tag_detail.Where(x => x.id_tag == Promo.reference).Count() > 0)
                {

                    SalesInvoice.DiscountPercentage = Promo.result_value;

                    Invoice PromotionInvoice = new Invoice();
                    PromotionInvoice.Contact = SalesInvoice.contact;
                    PromotionInvoice.Date = SalesInvoice.trans_date;
                    PromotionInvoice.GrandTotal = SalesInvoice.GrandTotal;

                    foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                    {
                        Detail Detail = new Detail();
                        Detail.DetailID = _Detail.id_sales_invoice_detail;
                        Detail.Item = _Detail.item;
                        Detail.Quantity = _Detail.quantity;
                        Detail.Price = _Detail.unit_price;
                        Detail.PriceVAT = _Detail.UnitPrice_Vat;
                        Detail.Discount = _Detail.discount;
                        Detail.DiscountVAT = _Detail.DiscountVat;
                        Detail.SubTotal = _Detail.SubTotal;
                        Detail.SubTotalVAT = _Detail.SubTotal_Vat;
                        Detail.is_promo = _Detail.IsPromo;
                        PromotionInvoice.Details.Add(Detail);
                    }
                    SalesLIST.Add(PromotionInvoice);


                }
                foreach (sales_invoice_detail _Detail_ in SalesInvoice.sales_invoice_detail)
                {
                    _Detail_.IsPromo = true;

                }




            }
        }


        private void Calculate(ref Invoice Invoice)
        {
            foreach (var Promo in SalesPromotionLIST)
            {
                //if (Promo.types == sales_promotion.Type.Discount_onGrandTotal)
                //{
                //    if (Promo.quantity_max >= Invoice.GrandTotal && Promo.quantity_min <= Invoice.GrandTotal)
                //    {
                //        Promo _Promo = new Promo();
                //        _Promo.Type = sales_promotion.Type.Discount_onGrandTotal;
                //        _Promo.Shared = true;
                //        _Promo.DiscountValue = Invoice.GrandTotal - (Promo.is_percentage == false ? Promo.result_value : (Invoice.GrandTotal * (Promo.result_value)));
                //        Invoice.Promos.Add(_Promo);
                //    }
                //    else if (Math.Floor(Invoice.GrandTotal / Promo.quantity_step) >= 1)
                //    {
                //        int Step = (int)Math.Floor(Invoice.GrandTotal / Promo.quantity_step);

                //        Promo _Promo = new Promo();
                //        _Promo.Type = sales_promotion.Type.Discount_onGrandTotal;
                //        _Promo.Shared = true;
                //        _Promo.DiscountValue = Invoice.GrandTotal - (Promo.is_percentage == false ? (Promo.result_value * Step) : (Invoice.GrandTotal * (Promo.result_value * Step)));
                //        Invoice.Promos.Add(_Promo);
                //    }
                //}

                //if (Promo.types == sales_promotion.Type.Discount_onBrand)
                //{
                //    if (Invoice.Details.Where(x => x.Item.item_brand.id_brand == Promo.reference).Count() > 0)
                //    {
                //        foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.item_brand.id_brand == Promo.reference))
                //        {
                //            Promo _Promo = new Promo();
                //            _Promo.Type = sales_promotion.Type.Discount_onBrand;
                //            _Promo.Shared = true;
                //            _Promo.DiscountValue = _Detail.PriceVAT - (Promo.is_percentage == false ? Promo.result_value : (_Detail.PriceVAT * (Promo.result_value)));
                //            _Detail.Promos.Add(_Promo);
                //        }
                //    }
                //}

                if (Promo.type == sales_promotion.salesPromotion.BuyThis_GetThat)
                {
                    if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step).Count() > 0)
                    {
                        foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                        {
                            Promo _Promo = new Promo();
                            _Promo.Type = sales_promotion.salesPromotion.BuyThis_GetThat;
                            _Promo.Shared = true;

                            using (db db = new db())
                            {
                                _Promo.DiscountValue = db.item_price.Where(x => x.id_item == _Detail.Item.id_item).FirstOrDefault().value;
                            }

                            _Detail.Promos.Add(_Promo);
                        }
                    }
                }
            }
        }
    }

    public class Invoice
    {
        public Invoice()
        {
            Details = new List<Detail>();
            Promos = new List<Promo>();
        }

        public contact Contact { get; set; }
        public DateTime Date { get; set; }
        public decimal GrandTotal { get; set; }

        public virtual ICollection<Detail> Details { get; set; }
        public virtual ICollection<Promo> Promos { get; set; }
    }

    public class Detail
    {
        public Detail()
        {
            Promos = new List<Promo>();
        }
        public item Item { get; set; }
        public int DetailID { get; set; }
        public decimal Quantity { get; set; }
        public bool is_promo { get; set; }
        public int PromotionID { get; set; }
        public decimal Price { get; set; }
        public decimal PriceVAT { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountVAT { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubTotalVAT { get; set; }

        public virtual ICollection<Promo> Promos { get; set; }
    }
    public class DetailTag
    {
        public int Tag { get; set; }
        public decimal Quantity { get; set; }
    }
    public class DetailProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
    }

    public class Promo
    {
        public sales_promotion.salesPromotion Type { get; set; }
        public decimal DiscountValue { get; set; }
        public bool Shared { get; set; }
    }
}
