using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cntrl.Curd.Promotion
{
    /// <summary>
    /// Interaction logic for BuyThis_GetThat.xaml
    /// </summary>
    public partial class BuyThis_GetThat : Page
    {
        public BuyThis_GetThat()
        {
            InitializeComponent();
        }

        private CollectionViewSource _sales_promotionViewSource = null;
        public CollectionViewSource sales_promotionViewSource { get { return _sales_promotionViewSource; } set { _sales_promotionViewSource = value; } }

        private entity.dbContext objentity = null;


        public entity.dbContext entity { get { return objentity; } set { objentity = value; } }

       

      
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = sales_promotionViewSource;

            }
        }

        private void sbxRefItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxRefItem.ItemID > 0)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    sales_promotion.reference = sbxRefItem.ItemID;
                    sales_promotion.RaisePropertyChanged("InputName");

                }
            }
        }

        private void sbxBonusItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxBonusItem.ItemID > 0)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    sales_promotion.reference_bonus = sbxBonusItem.ItemID;
                    sales_promotion.RaisePropertyChanged("OutputName");

                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
            {
                sales_promotion.type = sales_promotion.salesPromotion.BuyThis_GetThat;
                sales_promotion.timestamp = DateTime.Now;
            }

            if (entity.db.GetValidationErrors().Count() == 0)
            {
                entity.SaveChanges();

                cntrl.promotion mainWindow = CurrentSession.FindParentOfType<cntrl.promotion>(this);
                mainWindow.btnCancel_Click(null, null);
            }
        }

    }
}
