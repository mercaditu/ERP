using entity;

namespace Cognitivo.Class
{
    public class CreditLimit
    {
        public void Check_CreditAvailability(object Document)
        {
            string BaseName = Document.GetType().BaseType.ToString();
            string AppName = Document.GetType().ToString();
            entity.BrilloQuery.Finance Finance = new entity.BrilloQuery.Finance();

            if (AppName == typeof(sales_invoice).ToString() || BaseName == typeof(sales_invoice).ToString())
            {
                sales_invoice sales_invoice = (sales_invoice)Document;
                if (sales_invoice != null && sales_invoice.contact != null && sales_invoice.contact.credit_limit != null)
                {
                    decimal Balance = (decimal)Finance.SpecialFXBalance_ByCustomer(sales_invoice.app_currencyfx.buy_value, sales_invoice.id_contact);
                    sales_invoice.contact.credit_availability = Balance;
                    sales_invoice.contact.RaisePropertyChanged("credit_availability");
                }
            }
            else if (AppName == typeof(sales_budget).ToString() || BaseName == typeof(sales_budget).ToString())
            {
                sales_budget sales_budget = (sales_budget)Document;
                if (sales_budget != null && sales_budget.contact != null && sales_budget.contact.credit_limit != null)
                {
                    decimal Balance = (decimal)Finance.SpecialFXBalance_ByCustomer(sales_budget.app_currencyfx.buy_value, sales_budget.id_contact);
                    sales_budget.contact.credit_availability = Balance;
                    sales_budget.contact.RaisePropertyChanged("credit_availability");
                }
            }
            else if (AppName == typeof(sales_order).ToString() || BaseName == typeof(sales_order).ToString())
            {
                sales_order sales_order = (sales_order)Document;
                if (sales_order != null && sales_order.contact != null && sales_order.contact.credit_limit != null)
                {
                    decimal Balance = (decimal)Finance.SpecialFXBalance_ByCustomer(sales_order.app_currencyfx.buy_value, sales_order.id_contact);
                    sales_order.contact.credit_availability = Balance;
                    sales_order.contact.RaisePropertyChanged("credit_availability");
                }
            }
        }
    }
}