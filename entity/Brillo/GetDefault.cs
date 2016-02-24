using System.Linq;

namespace entity.Brillo
{
    public static class GetDefault
    {

        public static int? Range(App.Names AppName)
        {
            entity.Properties.Settings _setting = new Properties.Settings();
            using (db db = new db())
            {
                app_document_range app_document_range = entity.Brillo.Logic.Range.List_Range(AppName, _setting.branch_ID, _setting.terminal_ID)
                                                                             .FirstOrDefault();
                if (app_document_range != null)
                {
                    return app_document_range.id_range;
                }
            }

            return null;
        }

        public static int? CurrencyFX(System.Data.Entity.EntityState State)
        {
            return null;
        }
    }
}
