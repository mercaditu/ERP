using System.Linq;
using System.Windows.Controls;

namespace cntrl.Controls
{
    public partial class InventoryFlowDataGrid : UserControl
    {
        public int? ParentID { get; set; }
        public int ProductID { get; set; }

        public InventoryFlowDataGrid(int? InvParentID, int InvProductID)
        {
            InitializeComponent();

            ParentID = InvParentID;
            ProductID = InvProductID;

            using (entity.db db = new entity.db())
            {
                db.item_movement.Where(x => x.parent.id_movement == ParentID && x.id_item_product == ProductID).ToList();
            }
        }
    }
}
