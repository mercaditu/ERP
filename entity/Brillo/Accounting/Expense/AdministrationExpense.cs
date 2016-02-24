using System.Linq;

namespace entity.Brillo.Accounting.Expense
{
    class AdministrationExpense
    {
        public accounting_chart find_Chart(AccountingJournalDB context, app_cost_center app_cost_center)
        {
            if (context.accounting_chart.Where(i => i.id_cost_center == app_cost_center.id_cost_center).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.id_cost_center == app_cost_center.id_cost_center).FirstOrDefault();
            }
            else if (context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.AdministrationExpense && i.is_generic == true).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.AdministrationExpense && i.is_generic == true).FirstOrDefault();
            }
            else
            {
                return null;
            }

            //accounting_journal __accounting_journal = new accounting_journal();
            //__accounting_journal.id_chart = accounting_chart.id_chart;
            //__accounting_journal.debit = sales_invoice.invoice_Total;
        }
    }
}
