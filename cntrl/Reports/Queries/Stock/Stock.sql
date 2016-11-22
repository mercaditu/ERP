 select(sum(mov.credit) - sum(mov.debit)) as Quantity from item_movement as mov
inner join app_location as   loc on mov.id_location = loc.id_location
inner join app_branch as branch on  loc.id_branch = branch.id_branch
  inner join item_product as prod on mov.id_item_product =   prod.id_item_product
inner join items as item on     prod.id_item = item.id_item

where mov.id_company = @CompanyID and  branch.id_branch = @BranchID and mov.trans_date <= @EndDate and item.id_item = @ItemID
