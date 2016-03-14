using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace entity.Brillo.Logic
{
    public partial class CloneObject : BaseDB
    {
       public  object Clone(object obj,object clone)
       {
           //Get entity to be cloned
           var source = obj;

           //Create and add clone object to context before setting its values
         

          // Copy values from source to clone
           var sourceValues = base.Entry(source).CurrentValues;
           base.Entry(clone).CurrentValues.SetValues(sourceValues);
           return clone;
       }
    }
}
