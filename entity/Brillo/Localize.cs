using WPFLocalizeExtension.Extensions;

namespace entity.Brillo
{
    public static class Localize
    {
        public static T Text<T>(string key)
        {
            return LocExtension.GetLocalizedValue<T>("Cognitivo:local:" + key);
        }

        public static string StringText(string key)
        {
            string s = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + key);
            //In case "s" or localized text is not found, then return same key. Or else 
            return s != null ? s : key;
        }

        public static string Question_Delete { get { return StringText("Question_Delete"); } }
        public static string Question_Cancel { get { return StringText("Question_Cancel"); } }
        public static string PleaseSelect { get { return StringText("PleaseSelect"); } }
    }
}