using System.Linq;

namespace entity.Brillo
{
    public static class GetDefault
    {

        public static app_document_range Range(db db, App.Names AppName)
        {            

            app_document_range app_document_range = entity.Brillo.Logic.Range.List_Range(AppName, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();
            
            if (app_document_range != null)
            {
                return app_document_range;
            }

            return null;
        }

        public static int? CurrencyFX(System.Data.Entity.EntityState State)
        {
            return null;
        }
    }
}
