using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo.Promotion
{
    public class Start
    {
        public List<sales_promotion> SalesPromotionLIST = new List<sales_promotion>();

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
                Detail.Item = _Detail.item;
                Detail.Quantity = _Detail.quantity;
                Detail.Price = _Detail.unit_price;
                Detail.PriceVAT = _Detail.UnitPrice_Vat;
                Detail.SubTotal = _Detail.SubTotal;
                Detail.SubTotalVAT = _Detail.SubTotal_Vat;

                Invoice.Details.Add(Detail);
            }

            foreach (var Promo in SalesPromotionLIST)
            {
                //if (Promo.types == sales_promotion.Type.Discount_onGrandTotal)
                //{
                if (Promo.types == sales_promotion.Type.BuyThis_GetThat)
                {
                    if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step).Count() > 0)
                    {
                        foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                        {
                            if (Promo.quantity_step > 0)
                            {
                                Promo _Promo = new Promo();
                                _Promo.Type = sales_promotion.Type.BuyThis_GetThat;
                                _Promo.Shared = true;

                                _Detail.Promos.Add(_Promo);

                                List<sales_invoice_detail> sid = SalesInvoice.sales_invoice_detail.Where(x => x.id_item == Promo.reference_bonus).ToList();
                                //Prevent double clicking button and adding extra bonus to sale. find better way to implement. Short term code.
                                foreach (sales_invoice_detail _Detail_ in sid)
                                {
                                    SalesInvoice.sales_invoice_detail.Remove(_Detail_);
                                }

                                sales_invoice_detail sales_invoice_detail = new sales_invoice_detail();

                                //Needed to calculate the discounts and unit prices further on.
                                sales_invoice_detail.State = System.Data.Entity.EntityState.Added;

                                using (db db = new db())
                                {
                                    item item = db.items.Where(x => x.id_item == Promo.reference_bonus).FirstOrDefault();
                                    if (item != null)
                                    {
                                        sales_invoice_detail.id_vat_group = item.id_vat_group;
                                        sales_invoice_detail.item = item;   
                                    }

                                    item_price item_price = item.item_price.Where(x => x.item_price_list.is_default == true).FirstOrDefault();
                                    if (item_price != null)
                                    {
                                        sales_invoice_detail.unit_price = item_price.value;
                                        sales_invoice_detail.discount = sales_invoice_detail.unit_price;
                                    }
                                }

                                sales_invoice_detail.IsPromo = true;
                                sales_invoice_detail.id_item = Promo.reference_bonus;
                                sales_invoice_detail.item_description = sales_invoice_detail.item.name;
                                sales_invoice_detail.quantity = Math.Floor(_Detail.Quantity / Promo.quantity_step);
                                SalesInvoice.sales_invoice_detail.Add(sales_invoice_detail);
                            }
                        }
                    }
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

                if (Promo.types == sales_promotion.Type.BuyThis_GetThat)
                {
                    if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step).Count() > 0)
                    {
                        foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                        {
                            Promo _Promo = new Promo();
                            _Promo.Type = sales_promotion.Type.BuyThis_GetThat;
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

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
        public decimal PriceVAT { get; set; }

        public decimal SubTotal { get; set; }
        public decimal SubTotalVAT { get; set; }

        public virtual ICollection<Promo> Promos { get; set; }
    }

    public class Promo
    {
        public sales_promotion.Type Type { get; set; }
        public decimal DiscountValue { get; set; }
        public bool Shared { get; set; }
    }
}
