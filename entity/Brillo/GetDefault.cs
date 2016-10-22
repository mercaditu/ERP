using System.Linq;

namespace entity.Brillo
{
    public static class GetDefault
    {

        public static int? Return_RangeID(App.Names AppName)
        {
            using (db db = new db())
            {
                //Returns Range from a Using Context. Do not Use Range as Is. Must convert into ID and then pass it on.
                app_document_range app_document_range = Logic.Range.List_Range(db, AppName, CurrentSession.Id_Branch, CurrentSession.Id_Terminal).FirstOrDefault();

                if (app_document_range != null)
                {
                    return app_document_range.id_range;
                }

            }
            return null;
        }
    }
}
