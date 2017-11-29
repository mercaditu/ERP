using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd.Promotion
{
    /// <summary>
    /// Interaction logic for BuyTag_GetThat.xaml
    /// </summary>
    public partial class BuyTag_GetThat : Page
    {
        private CollectionViewSource item_tagViewSource;
        private CollectionViewSource _sales_promotionViewSource = null;
        public CollectionViewSource sales_promotionViewSource { get { return _sales_promotionViewSource; } set { _sales_promotionViewSource = value; } }

        private dbContext objentity = null;

        public dbContext entity { get { return objentity; } set { objentity = value; } }

        public BuyTag_GetThat()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = sales_promotionViewSource;

                item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;

                entity.db.item_tag.Where(x => x.id_company == CurrentSession.Id_Company).Load();
                item_tagViewSource.Source = entity.db.item_tag.Local;
            }
        }

        private void sbxBonusItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxBonusItem.ItemID > 0)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    sales_promotion.reference_bonus = sbxBonusItem.ItemID;

                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
            {
                sales_promotion.type = sales_promotion.salesPromotion.BuyTag_GetThat;
            }

            if (entity.db.GetValidationErrors().Count() == 0)
            {
                entity.SaveChanges();

                cntrl.promotion mainWindow = CurrentSession.FindParentOfType<cntrl.promotion>(this);
                mainWindow.btnCancel_Click(null, null);
            }
        }

        //private object checkParent(DependencyObject ob)
        //{
        //    cntrl.promotion mainWindow = VisualTreeHelper.GetParent(ob) as cntrl.promotion;
        //    return mainWindow != null ? mainWindow : null;
        //}
    }
}
