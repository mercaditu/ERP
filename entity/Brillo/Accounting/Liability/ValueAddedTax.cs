using System.Linq;

namespace entity.Brillo.Accounting.Liability
{
    class ValueAddedTax
    {
        public accounting_chart find_Chart(AccountingJournalDB context, app_vat vat)
        {
            if(vat != null)
            {
                if (context.accounting_chart.Where(i => i.id_vat == vat.id_vat && i.chart_type == accounting_chart.ChartType.Liability).FirstOrDefault() != null)
                {
                    return context.accounting_chart.Where(i => i.id_vat == vat.id_vat).FirstOrDefault();
                }
                else if (context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.VAT && i.chart_type == accounting_chart.ChartType.Liability && i.is_generic == true).FirstOrDefault() != null)
                {
                    return context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.VAT && i.chart_type == accounting_chart.ChartType.Liability && i.is_generic == true).FirstOrDefault();
                }
            }

            return null;
        }
    }
}
