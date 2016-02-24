using System.Linq;

namespace entity.Brillo.Accounting.Asset
{
    class Cash
    {
        public accounting_chart find_Chart(AccountingJournalDB context, app_account app_account)
        {
            if (context.accounting_chart.Where(i => i.id_account == app_account.id_account).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.id_account == app_account.id_account).FirstOrDefault();
            }
            else if (context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.Cash && i.is_generic == true).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.Cash && i.is_generic == true).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
