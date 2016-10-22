using System.Linq;

namespace entity.Brillo
{
    class General
    {
        public int Get_Currency(int id_company)
        {
            using (db db = new db())
            {
                if (db.app_currency.Where(x => x.id_company == id_company).FirstOrDefault() != null && db.app_currency.Any(x => x.is_priority == true))
                {
                    return db.app_currency.Where(x => x.id_company == id_company).FirstOrDefault().id_currency;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int get_price_list(int id_company)
        {
            using (db db = new db())
            {
                if (db.item_price_list.Where(a => a.id_company == id_company && a.is_default == true).FirstOrDefault() != null)
                    return db.item_price_list.Where(a => a.id_company == id_company && a.is_default == true).FirstOrDefault().id_price_list;
                else
                    return 0;
            }
        }
    }
}
