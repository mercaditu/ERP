using System.Linq;

namespace entity.Brillo.Accounting.Asset
{
    class AccountsReceivable
    {
        public accounting_chart find_Chart(AccountingJournalDB context, contact contact)
        {
          
            if (context.accounting_chart.Where(i => i.id_contact == contact.id_contact).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.id_contact == contact.id_contact).FirstOrDefault();
            }
            else if (context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.AccountsReceivable && i.is_generic == true).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.AccountsReceivable && i.is_generic == true).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }


}
