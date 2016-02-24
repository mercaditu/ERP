using System.Linq;

namespace entity.Brillo.Accounting.Liability
{
    class ValueAddedTax
    {
        public accounting_chart find_Chart(AccountingJournalDB context, app_vat vat)
        {
            if(vat != null)
            {
                if (context.accounting_chart.Where(i => i.id_vat == vat.id_vat).FirstOrDefault() != null)
                {
                    return context.accounting_chart.Where(i => i.id_vat == vat.id_vat).FirstOrDefault();
                }
                else if (context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.VAT && i.is_generic == true).FirstOrDefault() != null)
                {
                    return context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.VAT && i.is_generic == true).FirstOrDefault();
                }
            }

            return null;
            //accounting_journal __accounting_journal = new accounting_journal();
            //__accounting_journal.id_chart = accounting_chart.id_chart;
            //__accounting_journal.debit = sales_invoice.invoice_Total;
        }
    }
}
