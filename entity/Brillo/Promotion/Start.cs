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
            DetailLIST.Clear();
            foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
            {
                _Detail.discount = 0;
            }
            foreach (var Promo in SalesPromotionLIST)
            {
                Discount_onCustomerType(Promo, SalesInvoice);
                BuyThis_GetThat(Promo, SalesInvoice);
                Discount_onItem(Promo, SalesInvoice);
                Discount_onTag(Promo, SalesInvoice);
                Discount_onGrandTotal(Promo, SalesInvoice);
            }
            var Discountlist = DetailLIST.GroupBy(x => x.sales_invoice_detail).Select(x => new { sales_invoice_detail = x.Key, discountvat = x.Max(y => y.DiscountVAT) });
            var list = DetailLIST.Where(x => x.is_promo).GroupBy(x => x.PromotionID).Select(x => new { Detail = x.Max(z => z), discountvat = x.Max(y => y.DiscountVAT) });
            foreach (var data in Discountlist)
            {
                sales_invoice_detail _sales_invoice_detail = data.sales_invoice_detail;
                Detail Best_Promotion = DetailLIST.Where(x => x.sales_invoice_detail == _sales_invoice_detail).FirstOrDefault();

                if (Best_Promotion != null)
                {
                    if (Best_Promotion.is_promo == false && Best_Promotion.sales_invoice_detail != null)
                    {
                        _sales_invoice_detail.DiscountVat = data.discountvat;
                        _sales_invoice_detail.id_sales_promotion = Best_Promotion.Promotion.id_sales_promotion;
                    }
                }
            }
            foreach (var data in list)
            {
                Detail Best_Promotion = data.Detail;

                if (Best_Promotion != null)
                {
                    ///Logic to add new Items from GetThat Promotions...
                    sales_invoice_detail sales_invoice_detail = new sales_invoice_detail()
                    {
                        State = System.Data.Entity.EntityState.Added,
                        id_item = Best_Promotion.Item.id_item,
                        item_description = Best_Promotion.Item.name,
                        quantity = Best_Promotion.Quantity,
                        unit_price = Best_Promotion.Price,
                        discount = Best_Promotion.Discount
                    };
                    SalesInvoice.sales_invoice_detail.Add(sales_invoice_detail);
                }
            }
        }

        private void BuyThis_GetThat(sales_promotion Promo, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.BuyThis_GetThat)
            {
                Invoice Invoice = new Invoice()
                {
                    Contact = SalesInvoice.contact,
                    Date = SalesInvoice.trans_date,
                    GrandTotal = SalesInvoice.GrandTotal
                };
                foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                {
                    Detail Detail = new Detail()
                    {
                        DetailID = _Detail.id_sales_invoice_detail,
                        Item = _Detail.item,
                        Quantity = _Detail.quantity,
                        Price = _Detail.unit_price,
                        PriceVAT = _Detail.UnitPrice_Vat,
                        SubTotal = _Detail.SubTotal,
                        SubTotalVAT = _Detail.SubTotal_Vat,
                        is_promo = _Detail.IsPromo
                    };
                    Invoice.Details.Add(Detail);
                }

                if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step && x.is_promo == false).Count() > 0)
                {
                    foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                    {
                        if (Promo.quantity_step > 0)
                        {
                            Promo _Promo = new Promo()
                            {
                                Type = sales_promotion.salesPromotion.BuyThis_GetThat,
                                Shared = true
                            };

                            _Detail.Promos.Add(_Promo);

                            Detail Detail = new Detail();

                            using (db db = new db())
                            {
                                item item = db.items.Where(x => x.id_item == Promo.reference_bonus && x.id_company == CurrentSession.Id_Company).FirstOrDefault();

                                if (item != null)
                                {
                                    Detail.Item = item;
                                }

                                item_price item_price = item.item_price.Where(x => x.item_price_list.is_default == true).FirstOrDefault();

                                if (item_price != null)
                                {
                                    Detail.Price = item_price.value;
                                    Detail.Discount = item_price.value;
                                }
                            }

                            Detail.is_promo = true;
                            Detail.Promotion = Promo;
                            Detail.PromotionID = Promo.id_sales_promotion;
                            Detail.Quantity = Math.Floor(_Detail.Quantity / Promo.quantity_step);

                            DetailLIST.Add(Detail);
                        }
                    }
                }
            }
        }

        private void BuyTag_GetThat(sales_promotion Promo, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.BuyTag_GetThat)
            {
                Invoice Invoice = new Invoice()
                {
                    Contact = SalesInvoice.contact,
                    Date = SalesInvoice.trans_date,
                    GrandTotal = SalesInvoice.GrandTotal
                };
                foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                {
                    Detail Detail = new Detail()
                    {
                        DetailID = _Detail.id_sales_invoice_detail,
                        Item = _Detail.item,
                        Quantity = _Detail.quantity,
                        Price = _Detail.unit_price,
                        PriceVAT = _Detail.UnitPrice_Vat,
                        SubTotal = _Detail.SubTotal,
                        SubTotalVAT = _Detail.SubTotal_Vat,
                        is_promo = _Detail.IsPromo
                    };
                    Invoice.Details.Add(Detail);
                }

                decimal TotalQuantity = 0;

                List<Detail> DetailList = new List<Detail>();
                List<DetailTag> DetailTagList = new List<DetailTag>();

                DetailList = Invoice.Details.Where(x => x.Item.item_tag_detail.Any(y => y.id_tag == Promo.reference) && x.is_promo == false).ToList();

                if (DetailList.Count() > 0)
                {
                    DetailTag DetailTag = new DetailTag()
                    {
                        Tag = Promo.reference,
                        Quantity = DetailList.Sum(x => x.Quantity)
                    };

                    DetailTagList.Add(DetailTag);
                }

                TotalQuantity = DetailList.Sum(x => x.Quantity);

                if (DetailTagList.Count() > 0 && TotalQuantity >= Promo.quantity_step)
                {
                    foreach (DetailTag _DetailTag in DetailTagList)
                    {
                        Promo _Promo = new Promo()
                        {
                            Type = sales_promotion.salesPromotion.BuyTag_GetThat,
                            Shared = true
                        };

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
                            foreach (DetailProduct _DetailProduct in DetailProduct)
                            {
                                //Needed to calculate the discounts and unit prices further on.
                                Detail Detail = new Detail();

                                using (db db = new db())
                                {
                                    item item = db.items.Where(x => x.id_item == _DetailProduct.ProductId).FirstOrDefault();
                                    if (item != null)
                                    {
                                        Detail.Item = item;
                                    }

                                    item_price item_price = item.item_price.Where(x => x.item_price_list.is_default == true).FirstOrDefault();

                                    if (item_price != null)
                                    {
                                        Detail.Price = item_price.value;
                                        Detail.Discount = item_price.value;
                                    }
                                }

                                Detail.is_promo = true;
                                Detail.Promotion = Promo;
                                Detail.PromotionID = Promo.id_sales_promotion;
                                Detail.Quantity = _DetailProduct.Quantity;

                                DetailLIST.Add(Detail);
                            }
                        }
                    }
                }
            }
        }

        private void Discount_onItem(sales_promotion Promo, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onItem)
            {
                Invoice Invoice = new Invoice()
                {
                    Contact = SalesInvoice.contact,
                    Date = SalesInvoice.trans_date,
                    GrandTotal = SalesInvoice.GrandTotal
                };
                foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                {
                    Detail Detail = new Detail()
                    {
                        sales_invoice_detail = _Detail,
                        DetailID = _Detail.id_sales_invoice_detail,
                        Item = _Detail.item,
                        Quantity = _Detail.quantity,
                        Price = _Detail.unit_price,
                        PriceVAT = _Detail.UnitPrice_Vat,
                        SubTotal = _Detail.SubTotal,
                        SubTotalVAT = _Detail.SubTotal_Vat,
                        is_promo = _Detail.IsPromo
                    };
                    Invoice.Details.Add(Detail);
                }

                if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step && x.is_promo == false).Count() > 0)
                {
                    foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                    {
                        if (Promo.quantity_step > 0)
                        {
                            Promo _Promo = new Promo()
                            {
                                Type = sales_promotion.salesPromotion.Discount_onItem,
                                Shared = true
                            };

                            _Detail.Promos.Add(_Promo);

                            Detail Detail = Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.is_promo == false).FirstOrDefault();

                            if (Detail != null)
                            {
                                Detail.is_promo = false;
                                Detail.DiscountVAT = Detail.PriceVAT * Promo.result_value;
                                Detail.Promotion = Promo;
                                Detail.PromotionID = Promo.id_sales_promotion;
                                Detail.sales_invoice_detail = _Detail.sales_invoice_detail;
                                DetailLIST.Add(Detail);
                            }
                        }
                    }
                }
            }
        }

        private void Discount_onTag(sales_promotion Promo, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onTag)
            {
                Invoice Invoice = new Invoice()
                {
                    Contact = SalesInvoice.contact,
                    Date = SalesInvoice.trans_date,
                    GrandTotal = SalesInvoice.GrandTotal
                };
                foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                {
                    Detail Detail = new Detail()
                    {
                        sales_invoice_detail = _Detail,
                        DetailID = _Detail.id_sales_invoice_detail,
                        Item = _Detail.item,
                        Quantity = _Detail.quantity,
                        Price = _Detail.unit_price,
                        PriceVAT = _Detail.UnitPrice_Vat,
                        SubTotal = _Detail.SubTotal,
                        SubTotalVAT = _Detail.SubTotal_Vat,
                        is_promo = _Detail.IsPromo
                    };
                    Invoice.Details.Add(Detail);
                }

                decimal TotalQuantity = 0;

                List<Detail> DetailList = new List<Detail>();
                List<DetailTag> DetailTagList = new List<DetailTag>();

                using (db db = new db())
                {
                    DetailList = Invoice.Details.Where(x => x.Item.item_tag_detail.Any(y => y.id_tag == Promo.reference) && x.is_promo == false).ToList();
                    if (DetailList.Count() > 0)
                    {
                        DetailTag DetailTag = new DetailTag()
                        {
                            Tag = Promo.reference,
                            Quantity = DetailList.Sum(x => x.Quantity)
                        };
                        DetailTagList.Add(DetailTag);
                    }
                    TotalQuantity = DetailList.Sum(x => x.Quantity);
                }

                if (DetailTagList.Count() > 0 && TotalQuantity >= Promo.quantity_step)
                {
                    foreach (DetailTag _DetailTag in DetailTagList)
                    {
                        Promo _Promo = new Promo()
                        {
                            Type = sales_promotion.salesPromotion.BuyTag_GetThat,
                            Shared = true
                        };

                        List<Detail> sidpromo = Invoice.Details.Where(x => x.Item.item_tag_detail.Any(y => y.id_tag == Promo.reference) && x.is_promo == false).ToList();
                        //Prevent double clicking button and adding extra bonus to sale. find better way to implement. Short term code.

                        foreach (Detail _Detail in sidpromo)
                        {
                            _Detail.is_promo = false;
                            _Detail.DiscountVAT = _Detail.PriceVAT * Promo.result_value;
                            _Detail.Promotion = Promo;
                            _Detail.PromotionID = Promo.id_sales_promotion;
                            _Detail.sales_invoice_detail = _Detail.sales_invoice_detail;
                            DetailLIST.Add(_Detail);
                        }
                    }
                }
            }
        }

        private void Discount_onGrandTotal(sales_promotion Promo, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onGrandTotal)
            {
                Invoice Invoice = new Invoice()
                {
                    Contact = SalesInvoice.contact,
                    Date = SalesInvoice.trans_date,
                    GrandTotal = SalesInvoice.GrandTotal
                };

                foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                {
                    Detail Detail = new Detail()
                    {
                        sales_invoice_detail = _Detail,
                        DetailID = _Detail.id_sales_invoice_detail,
                        Item = _Detail.item,
                        Quantity = _Detail.quantity,
                        Price = _Detail.unit_price,
                        PriceVAT = _Detail.UnitPrice_Vat,
                        SubTotal = _Detail.SubTotal,
                        SubTotalVAT = _Detail.SubTotal_Vat,
                        is_promo = _Detail.IsPromo
                    };
                    Invoice.Details.Add(Detail);
                }

                if (Promo.reference == SalesInvoice.app_currencyfx.id_currency)
                {
                    if (Promo.quantity_step <= Invoice.GrandTotal)
                    {
                        SalesInvoice.DiscountPercentage = Promo.result_value;

                        foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                        {
                            Detail Detail = new Detail()
                            {
                                DetailID = _Detail.id_sales_invoice_detail,
                                Item = _Detail.item,
                                Quantity = _Detail.quantity,
                                Price = _Detail.unit_price,
                                PriceVAT = _Detail.UnitPrice_Vat,
                                Discount = _Detail.discount,
                                DiscountVAT = _Detail.DiscountVat,
                                SubTotal = _Detail.SubTotal,
                                SubTotalVAT = _Detail.SubTotal_Vat,
                                is_promo = false,
                                Promotion = Promo,
                                PromotionID = Promo.id_sales_promotion,
                                sales_invoice_detail = _Detail
                            };
                            DetailLIST.Add(Detail);
                        }
                    }
                }
            }
        }

        private void Discount_onCustomerType(sales_promotion Promo, sales_invoice SalesInvoice)
        {
            if (Promo.type == sales_promotion.salesPromotion.Discount_onCustomerType)
            {
                if (SalesInvoice.contact != null)
                {
                    Invoice Invoice = new Invoice()
                    {
                        Contact = SalesInvoice.contact,
                        Date = SalesInvoice.trans_date,
                        GrandTotal = SalesInvoice.GrandTotal
                    };

                    foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                    {
                        Detail Detail = new Detail()
                        {
                            sales_invoice_detail = _Detail,
                            DetailID = _Detail.id_sales_invoice_detail,
                            Item = _Detail.item,
                            Quantity = _Detail.quantity,
                            Price = _Detail.unit_price,
                            PriceVAT = _Detail.UnitPrice_Vat,
                            SubTotal = _Detail.SubTotal,
                            SubTotalVAT = _Detail.SubTotal_Vat,
                            is_promo = _Detail.IsPromo
                        };
                        Invoice.Details.Add(Detail);
                    }
                    
                    if (SalesInvoice.contact.contact_tag_detail.Where(x => x.id_tag == Promo.reference).Count() > 0)
                    {
                        if (SalesInvoice.GrandTotal > Promo.quantity_step)
                        {
                            SalesInvoice.DiscountPercentage = Promo.result_value;

                            foreach (sales_invoice_detail _Detail in SalesInvoice.sales_invoice_detail)
                            {
                                Detail Detail = new Detail()
                                {
                                    DetailID = _Detail.id_sales_invoice_detail,
                                    Item = _Detail.item,
                                    Quantity = _Detail.quantity,
                                    Price = _Detail.unit_price,
                                    PriceVAT = _Detail.UnitPrice_Vat,
                                    Discount = _Detail.discount,
                                    DiscountVAT = _Detail.DiscountVat,
                                    SubTotal = _Detail.SubTotal,
                                    SubTotalVAT = _Detail.SubTotal_Vat,
                                    is_promo = false,
                                    Promotion = Promo,
                                    PromotionID = Promo.id_sales_promotion,
                                    sales_invoice_detail = _Detail
                                };

                                DetailLIST.Add(Detail);
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
                if (Promo.type == sales_promotion.salesPromotion.BuyThis_GetThat)
                {
                    if (Invoice.Details.Where(x => x.Item.id_item == Promo.reference && x.Quantity >= Promo.quantity_step).Count() > 0)
                    {
                        foreach (Detail _Detail in Invoice.Details.Where(x => x.Item.id_item == Promo.reference))
                        {
                            Promo _Promo = new Promo()
                            {
                                Type = sales_promotion.salesPromotion.BuyThis_GetThat,
                                Shared = true
                            };

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

    #region Sales-Object

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
        public sales_invoice_detail sales_invoice_detail { get; set; }
        public sales_promotion Promotion { get; set; }

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

    #endregion Sales-Object
}