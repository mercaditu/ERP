namespace cntrl.Reports.Purchase
{
    public static class Import
    {
        public static string query = @"
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                                set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
											  CASE
      WHEN impexes.status = 1 THEN '" + entity.Brillo.Localize.StringText("Pending") + @"'
      WHEN impexes.status=2 THEN '" + entity.Brillo.Localize.StringText("Approved") + @"'
      WHEN impexes.status=3 THEN '" + entity.Brillo.Localize.StringText("Anulled") + @"'
      WHEN impexes.status=4 THEN '" + entity.Brillo.Localize.StringText("Rejected") + @"'
        END
      as Status,
											impexes.number as Number,
											Date(impexes.etd) as ETD,
											Date(impexes.eta) as ETA,
											purchase_invoice.number as PurchaseInvoice,
											value as Value,
                                            impex_incoterm_condition.name as Expense
											

											from impex_expense
											inner join impexes
											on impex_expense.id_impex=impexes.id_impex
											inner join impex_incoterm_condition on impex_expense.id_incoterm_condition = impex_incoterm_condition.id_incoterm_condition
                                            left join purchase_invoice on impex_expense.id_purchase_invoice = purchase_invoice.id_purchase_invoice
											where impexes.id_company = @CompanyID";
											
    }
}