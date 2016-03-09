﻿using System.Collections.Generic;
using System.Linq;

namespace entity.Brillo.Accounting.Asset
{
    class Inventory
    {
        public accounting_chart find_Chart(AccountingJournalDB context, List<item_tag> item_tagLIST)
        {
            if (context.accounting_chart.Where(i => item_tagLIST.Contains(i.item_tag)).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => item_tagLIST.Contains(i.item_tag)).FirstOrDefault();
            }
            else if (context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.Inventory && i.is_generic == true).FirstOrDefault() != null)
            {
                return context.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.Inventory && i.is_generic == true).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

    }
}
