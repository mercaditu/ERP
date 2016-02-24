using System.Linq;

namespace entity.Brillo.Accounting.Asset
{
    class FixedAsset
    {
        public accounting_chart find_Chart(ref dbContext context, item_asset_group item_asset_group)
        {
            if (context.db.accounting_chart.Where(i => i.id_item_asset_group == item_asset_group.id_item_asset_group).FirstOrDefault() != null)
            {
                return context.db.accounting_chart.Where(i => i.id_item_asset_group == item_asset_group.id_item_asset_group).FirstOrDefault();
            }
            else if (context.db.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.FixedAsset && i.is_generic == true).FirstOrDefault() != null)
            {
                return context.db.accounting_chart.Where(i => i.chartsub_type == accounting_chart.ChartSubType.FixedAsset && i.is_generic == true).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
