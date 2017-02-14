using System;
using System.Collections.Generic;
using System.Linq;

namespace entity.Brillo
{
    class Location
    {
        public int get_Location(item_product item_product, app_branch app_branch)
        {

            try
            {
                return get_ProductLocation(item_product, app_branch);
            }
            catch
            {
                app_location app_location = new app_location();
                app_location.id_branch = app_branch.id_branch;
                app_location.name = "Default of " + app_branch.name;
                app_location.is_default = true;
                using (db db = new db())
                {
                    db.app_location.Add(app_location);
                    db.SaveChangesAsync();
                    return app_location.id_location;
                }

            }


        }

        public int get_ProductLocation(item_product item_product, app_branch app_branch)
        {
            int id_location = 0;

            if (item_product != null)
            {
                //calculate location.
                using (db db = new db())
                {
                    //Pankeel. Check Stock Level within this Branch and Return correct Location.
                    List<item_movement> item_movementList = db.item_movement.Where(x => x.item_product.id_item_product == item_product.id_item_product && x.app_location.id_branch == app_branch.id_branch).ToList();
                    item_movement item_movement = item_movementList.Where(x => x.avlquantity > 0).FirstOrDefault();
                    if (item_movement != null)
                    {
                        id_location = Convert.ToInt16(item_movement.id_location);
                    }
                    else
                    {
                        id_location = get_DefaultLocation(app_branch);
                    }
                }
            }
            return id_location;
        }

        public int get_DefaultLocation(app_branch app_branch)
        {
            int id_location = 0;

            if (app_branch.app_location.Count != 0)
            {
                if (app_branch.app_location.Any(x => x.is_default))
                {
                    id_location = app_branch.app_location.Where(x => x.is_default == true).Select(y => y.id_location).FirstOrDefault();
                }
                else
                {
                    id_location = app_branch.app_location.FirstOrDefault().id_location;
                }
            }
            return id_location;
        }
    }
}
