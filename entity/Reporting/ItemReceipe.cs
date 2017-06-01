
namespace entity.Reporting
{
	public class ItemReceipe
	{
		public int id_recepie { get; set; }
		public string Product { get; set; }
        public string Code { get; set; }
        public string RawMaterial { get; set; }
        public string RawMaterialCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
	}
}
