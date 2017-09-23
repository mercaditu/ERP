using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for MovementPrint.xaml
    /// </summary>
    public partial class MovementPrint : UserControl
    {
        public long? ParentID { get; set; }




        private CollectionViewSource item_movementViewSource;
        public App.Names ApplicationName { get; set; }
        public MovementPrint()
        {
            InitializeComponent();
            ApplicationName = App.Names.Movement;
        }
        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            using (db db = new db())
            {
                var MovementList = (from item in db.item_movement
                                    join attach in db.app_attachment on item.id_movement equals attach.reference_id
                                    join loc in db.app_location on item.id_location equals loc.id_location
                                    join b in db.app_branch on loc.id_branch equals b.id_branch
                                    join ip in db.item_product on item.id_item_product equals ip.id_item_product
                                    join i in db.items on ip.id_item equals i.id_item
                                    where
                                    item.id_movement == ParentID
                                    && attach.application==App.Names.Movement
                                    select new ItemMovement
                                    {
                                        MovementID = item.id_movement,
                                        Branch = b.name,
                                        Location = loc.name,
                                        ProductCode = i.code,
                                        ProductName = i.name,
                                        Date = item.trans_date,
                                        ExpiryDate = item.expire_date,
                                        BatchCode = item.code,
                                        Quantity = item.debit,
                                        Cost = item.item_movement_value.Sum(x => x.unit_value),
                                        Comment = item.comment,
                                        DisplayImage = item.debit > 0 ? true : false,
                                        IsTransfer = item.id_transfer_detail != null ? true : false,
                                        file=attach.file
                                    })
                                    .OrderByDescending(x => x.Date)
                                    .ToList();

                item_movementViewSource = FindResource("item_movementViewSource") as CollectionViewSource;
                item_movementViewSource.Source = MovementList;
                item_movementViewSource.View.Refresh();



            }








        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemMovement ItemMovement = item_movementViewSource.View.CurrentItem as ItemMovement;
            if (ItemMovement != null)
            {
             
            }
        }

    }
}
