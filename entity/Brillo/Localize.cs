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
            if (s==null)
            {
                s = key; 
            }
            return s;
        }
    }
}
