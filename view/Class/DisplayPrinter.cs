using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognitivo.Class
{
    class DisplayPrinter
    {

        private decimal Id;
        public decimal IdProperty
        {
            get { return Id; }
            set { Id = value; }
        }

        private decimal Cost;
        public decimal CostProperty
        {
            get { return Cost; }
            set { Cost = value; }
        }

        private String Name;
        public String NameProperty
        {
            get { return Name; }
            set { Name = value; }
        }
        
    }
}
