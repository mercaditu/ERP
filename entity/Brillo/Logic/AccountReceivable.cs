using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo.Logic
{
    public class AccountReceivable
    {
        public void ReceivePayment(ref dbContext _entity, payment_schedual payment_schedual, int id_range, int id_currencyfx,int id_payment_type,
                                  int id_purchase_return, int id_sales_return, decimal value, string comment, int id_account, DateTime trans_date)
        {

            payment payment = new payment();
            if (id_sales_return > 0)
            {

                payment.id_contact = payment_schedual.contact.id_contact;

                if (id_range != null)
                {
                    payment.id_range = id_range;
                    if (_entity.db.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault() != null)
                    {
                        payment.app_document_range = _entity.db.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault();
                    }

                }


                payment_detail payment_detailreturn = new payment_detail();
                if (_entity.db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
                {
                    payment_detailreturn.app_currencyfx = _entity.db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault();
                }

                payment_detailreturn.id_currencyfx = id_currencyfx;
                payment_detailreturn.id_payment_type = id_payment_type;

                payment_detailreturn.id_purchase_return = id_purchase_return;
                payment_detailreturn.id_sales_return = id_sales_return;

                payment_detailreturn.value = value;
                payment_detailreturn.comment = comment;
                payment_schedual _payment_schedualreturn = new payment_schedual();

                _payment_schedualreturn.credit = Convert.ToDecimal(value);
                _payment_schedualreturn.parent = payment_schedual;
                _payment_schedualreturn.expire_date = payment_schedual.expire_date;
                _payment_schedualreturn.status = payment_schedual.status;
                _payment_schedualreturn.id_contact = payment_schedual.id_contact;
                _payment_schedualreturn.id_currencyfx = payment_schedual.id_currencyfx;
                _payment_schedualreturn.id_purchase_invoice = payment_schedual.id_purchase_invoice;
                _payment_schedualreturn.id_purchase_order = payment_schedual.id_purchase_order;
                _payment_schedualreturn.id_purchase_return = payment_schedual.id_purchase_return;
                _payment_schedualreturn.id_sales_invoice = payment_schedual.id_sales_invoice;
                _payment_schedualreturn.id_sales_order = payment_schedual.id_sales_order;
                _payment_schedualreturn.id_sales_return = id_sales_return;
                _payment_schedualreturn.trans_date = trans_date;
                payment_detailreturn.payment_schedual.Add(_payment_schedualreturn);
                payment.payment_detail.Add(payment_detailreturn);



            }
            else
            {
                payment.id_contact = payment_schedual.contact.id_contact;

                if (id_range != null)
                {
                    payment.id_range = id_range;
                    if (_entity.db.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault() != null)
                    {
                        payment.app_document_range = _entity.db.app_document_range.Where(x => x.id_range == payment.id_range).FirstOrDefault();
                    }

                }




                payment_detail payment_detail = new payment_detail();
                payment_detail.id_account = id_account;
                if (_entity.db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
                {
                    payment_detail.app_currencyfx = _entity.db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault();
                }
                payment_detail.id_currencyfx = id_currencyfx;
                payment_detail.id_payment_type = id_payment_type;

                payment_detail.id_purchase_return = id_purchase_return;
                payment_detail.id_sales_return = id_sales_return;

                payment_detail.value = value;
                payment_detail.comment = comment;
                payment_schedual _payment_schedual = new payment_schedual();

                _payment_schedual.credit = Convert.ToDecimal(value);
                _payment_schedual.parent = payment_schedual;
                _payment_schedual.expire_date = payment_schedual.expire_date;
                _payment_schedual.status = payment_schedual.status;
                _payment_schedual.id_contact = payment_schedual.id_contact;
                _payment_schedual.id_currencyfx = payment_schedual.id_currencyfx;
                _payment_schedual.id_purchase_invoice = payment_schedual.id_purchase_invoice;
                _payment_schedual.id_purchase_order = payment_schedual.id_purchase_order;
                _payment_schedual.id_purchase_return = payment_schedual.id_purchase_return;
                _payment_schedual.id_sales_invoice = payment_schedual.id_sales_invoice;
                _payment_schedual.id_sales_order = payment_schedual.id_sales_order;
                _payment_schedual.id_sales_return = payment_schedual.id_sales_return;
                _payment_schedual.trans_date = trans_date;
               

                payment_detail.payment_schedual.Add(_payment_schedual);
                payment.payment_detail.Add(payment_detail);

                //Add Account Logic. With IF FUnction if payment type is Basic Behaviour. If not ignore.
                if (_entity.db.payment_type.Where(x => x.id_payment_type == id_payment_type).FirstOrDefault().payment_behavior == payment_type.payment_behaviours.Normal)
                {
                    app_account_detail app_account_detail = new app_account_detail();
                    if (_entity.db.app_account_session.Where(x => x.id_account == id_account && x.is_active).FirstOrDefault() != null)
                    {
                        app_account_detail.id_session = _entity.db.app_account_session.Where(x => x.id_account == id_account && x.is_active).FirstOrDefault().id_session;
                    }
                    app_account_detail.id_account = id_account;
                    app_account_detail.id_currencyfx = payment_detail.id_currencyfx;
                    app_account_detail.id_payment_type = id_payment_type;
                    app_account_detail.trans_date = trans_date;
                    app_account_detail.debit = 0;
                    app_account_detail.credit = Convert.ToDecimal(value);
                    _entity.db.app_account_detail.Add(app_account_detail);
                }

            }


            _entity.db.payments.Add(payment);







            IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                _entity.SaveChanges();

                entity.Brillo.Document.Start.Automatic(payment, payment.app_document_range);
            }


        }
    }
}
