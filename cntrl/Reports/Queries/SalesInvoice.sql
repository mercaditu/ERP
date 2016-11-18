SELECT 
	                            branch.name as Branch,
                                s.status as Status,
	                            DATE_FORMAT(s.trans_date,'%d %b %y') as InvoiceDate, 
                                contact.name as Customer, 
                                s.number as Invoice, 
	                            sum(sd.quantity) AS Quantity, 
    
                                round(sum(sd.quantity * sd.unit_price * vatco.coef),4) as SubTotalVAT,
	                            round(sum(sd.quantity * sd.discount * vatco.coef),4) as SubTotalDiscountVAT,
                                round(sum(sd.quantity * sd.unit_cost * vatco.coef),4) as SubTotalCostVAT,
	                            round(sum(sd.quantity * sd.unit_price * vatco.vat),4) as VAT_SubTotal,
    
                                round(sum(sd.quantity * sd.unit_cost),4) as SubTotalCost,
	                            round(sum(sd.quantity * sd.unit_price),4) as SubTotal,

	    
                                ((sum(sd.quantity * sd.unit_price * vatco.coef) - sum(sd.quantity * sd.unit_cost * vatco.coef)) / sum(sd.quantity * sd.unit_price * vatco.coef)) as Margin,
                                ((sum(sd.quantity * sd.unit_price * vatco.coef) - sum(sd.quantity * sd.unit_cost * vatco.coef)) / sum(sd.quantity * sd.unit_cost * vatco.coef)) as MarkUp,
                                 (sum(sd.quantity * sd.unit_price * vatco.coef) - sum(sd.quantity * sd.unit_cost * vatco.coef)) as Profit
   

                            from  sales_invoice s inner join
                                     contacts as contact ON s.id_contact = contact.id_contact 
                                     inner join
                                     app_branch as branch on s.id_branch = branch.id_branch
                                     inner join 
                                     sales_invoice_detail sd ON s.id_sales_invoice = sd.id_sales_invoice 
                                     LEFT OUTER JOIN 
                                     items i ON i.id_item = sd.id_item 
                                         LEFT OUTER JOIN 
                                         (SELECT app_vat_group.id_vat_group, sum(app_vat.coefficient) as vat, sum(app_vat.coefficient) + 1 AS coef
                                        FROM  app_vat_group LEFT OUTER JOIN 
                                                 app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group LEFT OUTER JOIN 
                                                 app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
                                        GROUP BY app_vat_group.id_vat_group) vatco ON vatco.id_vat_group = sd.id_vat_group
                          
                            group by s.id_sales_invoice
                            order by s.trans_date