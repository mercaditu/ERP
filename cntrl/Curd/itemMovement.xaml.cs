using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for itemMovement.xaml
    /// </summary>
    public partial class itemMovement : UserControl
    {
        public bool isValid { get; set; }
        CollectionViewSource _item_transferViewSource = null;
        public CollectionViewSource item_transferViewSource { get { return _item_transferViewSource; } set { _item_transferViewSource = value; } }

        //entity.Properties.Settings _settings = new entity.Properties.Settings();

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }


        private entity.project _item_transferobject = null;
        public entity.project item_transferobject { get { return _item_transferobject; } set { _item_transferobject = value; } }

        private dbContext entity = null;
        public dbContext dbContext { get { return entity; } set { entity = value; } }

        public enum Mode
        {
            Add,
            Edit
        }

        public bool canedit { get; set; }
        public bool candelete { get; set; }
        public Mode EnterMode { get; set; }
        public itemMovement()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                for (int i = 0; i < item_transfer_detailDataGrid.Items.Count - 1; i++)
                {

                    item_transfer_detail item = (item_transfer_detail)item_transfer_detailDataGrid.Items[i];

                    item_movement item_movement_origin = new item_movement();
                    item_movement_origin.debit = 0;
                    item_movement_origin.credit = item.quantity_origin;
                    item_movement_origin.id_location = item.item_transfer.app_location_origin.id_location;
                  //  item_movement_origin.transaction_id = 0;
                    item_movement_origin.status = Status.Stock.InStock;
                    item_movement_origin.trans_date = item.item_transfer.trans_date;
                    if (item.item_product.id_item_product != 0)
                    {
                        if (entity.db.item_product.Where(x => x.id_item_product == item.id_item_product).FirstOrDefault() != null)
                        {
                            item_movement_origin.id_item_product = entity.db.item_product.Where(x => x.id_item_product == item.id_item_product).FirstOrDefault().id_item_product;
                        }
                    }
                    entity.db.item_movement.Add(item_movement_origin);
                    item_movement item_movement_dest = new item_movement();
                    item_movement_dest.debit = item.quantity_destination;
                    item_movement_dest.credit = 0;
                    item_movement_dest.id_location = item.item_transfer.app_location_destination.id_location;
                    //item_movement_dest.transaction_id = 0;
                    item_movement_dest.status = Status.Stock.InStock;
                    item_movement_dest.trans_date = item.item_transfer.trans_date;
                    if (item.item_product.id_item_product != 0)
                    {
                        if (entity.db.item_product.Where(x => x.id_item_product == item.id_item_product).FirstOrDefault() != null)
                        {
                            item_movement_dest.id_item_product = entity.db.item_product.Where(x => x.id_item_product == item.id_item_product).FirstOrDefault().id_item_product;
                        }
                    }
                    entity.db.item_movement.Add(item_movement_dest);
                }
                entity.SaveChanges();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (EnterMode == Mode.Edit)
            {
                if (canedit == false)
                {
                    stpDisplay.IsEnabled = false;
                    btnSave.IsEnabled = false;
                }
            }
            if (EnterMode == Mode.Add)
            {
                stpDisplay.IsEnabled = true;
                btnSave.IsEnabled = true;
            }
            stackMain.DataContext = _item_transferViewSource;
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            entity.CancelChanges();
            _item_transferViewSource.View.Refresh();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

    }
}
