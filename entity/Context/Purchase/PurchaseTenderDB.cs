using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseTenderDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Order();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Order();
            return base.SaveChangesAsync();
        }

        private void validate_Order()
        {

        }
    }
}