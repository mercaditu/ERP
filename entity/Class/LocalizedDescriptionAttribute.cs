using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

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
