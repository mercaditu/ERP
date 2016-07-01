namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_FixedAsset : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_asset", "id_department", c => c.Int());
            AddColumn("item_asset", "id_contact", c => c.Int());
            //Sql(InstallScript);
        }
        
        public override void Down()
        {
            DropColumn("item_asset", "id_contact");
            DropColumn("item_asset", "id_department");
        }

        private const string InstallScript = @"
        CREATE  PROCEDURE `inventory`(in enddate datetime,
															in idcompany int(11))
BEGIN
	DECLARE done INT DEFAULT 0;
    declare donecost int default 0;
    DECLARE loc INT;
    DECLARE prod INT;
    DECLARE m INT DEFAULT 0;
    DECLARE j INT DEFAULT 0;
    DECLARE crd DECIMAL DEFAULT 0;
    DECLARE uvalue DECIMAL(20,4) DEFAULT 0;
    DECLARE quantity DECIMAL DEFAULT 0;
    DECLARE cost DECIMAL(20,4) DEFAULT 0;
    DECLARE idmov int(11);
    DECLARE QuantityCost CURSOR  FOR SELECT 
		id_movement,
		id_item_product,
        id_location,
        (sum(credit) - sum(debit)) as total 
	FROM item_movement as im 
    where im.trans_date <= enddate
    and im.id_company = idcompany
	GROUP BY id_location,id_item_product;
    
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    
    
    DROP TABLE IF EXISTS QC;
    CREATE TEMPORARY TABLE QC(
			id_movement int(11) primary key,
			id_item_product int(11),
            id_location int(11),
            quantity decimal(20,4),
            totcost decimal(20,4) default null);
            
	INSERT INTO QC(id_movement,id_item_product,id_location,quantity)
    SELECT 
		id_movement,
		id_item_product,
        id_location,
        sum(credit) - sum(debit) as total 
	FROM item_movement as im 
    where im.trans_date <= enddate
    and im.id_company = idcompany
	GROUP BY id_location,id_item_product;
    
    OPEN QuantityCost;
    
    QuantityCostLoop : LOOP
		FETCH QuantityCost into idmov,prod,loc,quantity;
        IF done = 1 THEN
			set done = 0;
            LEAVE QuantityCostLoop;
        END IF;
        
        costblock: begin
			declare itemmovementvalue cursor for 
				SELECT credit,unit_value from
				item_movement as im 
				inner join item_movement_value as imv
				on imv.id_movement = im.id_movement
				where id_item_product = prod
                and im.trans_date <=enddate
                and im.id_company = idcompany
				order by im.id_movement desc;
			DECLARE CONTINUE HANDLER FOR NOT FOUND SET donecost = 1;
            
            open itemmovementvalue;
            costloop: loop
				fetch itemmovementvalue into crd,uvalue;
                IF donecost = 1 THEN
					set donecost = 0;
					LEAVE costloop;
				END IF;
                IF quantity <= crd THEN
					SET cost = cost + (quantity * uvalue);
					LEAVE costloop;
				ELSEIF quantity > crd THEN
					SET quantity = quantity - crd;
					SET cost = cost + (crd*uvalue);
				END IF;
            end loop costloop;
		close itemmovementvalue;
        end costblock;

        UPDATE QC
        SET totcost = cost
        WHERE id_movement = idmov;
        set cost = 0;
    END LOOP;
    CLOSE QuantityCost;
    select dpto.name AS depositio, 
 		   produc.code AS codigo, 
 		QC.quantity AS quantity, 
        produc.name AS producto,QC.totcost as cost,
         it.name as category
	from qc
    left join item_product as ip
    on qc.id_item_product = ip.id_item_product
    left join items as produc
    on produc.id_item = ip.id_item
    left join (select id_item,id_tag
				from 
				item_Tag_detail
				group by id_item) as itd
    on itd.id_item = ip.id_item
    left join item_tag as it
    on it.id_tag = itd.id_tag
    left join app_location as ubi
    on ubi.id_location = qc.id_location
    left join app_branch as dpto
    on dpto.id_branch = ubi.id_branch
    order by dpto.name, producto;
END
    ";

    }
}
