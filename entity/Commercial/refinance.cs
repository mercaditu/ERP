using System;

namespace entity
{
    public partial class refinance
    {
        public payment_schedual payment_schedual { get; set; }
        public DateTime Date { get; set; }

        public decimal Value { get; set; }
    }
}