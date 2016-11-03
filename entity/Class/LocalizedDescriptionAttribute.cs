using System.ComponentModel;

namespace entity.Class
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        string _resourceKey;

        public LocalizedDescriptionAttribute(string resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                return Brillo.Localize.StringText(_resourceKey);
            }
        }
    }
}
