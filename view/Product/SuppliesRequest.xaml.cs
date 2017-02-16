using entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for SuppliesRequest.xaml
    /// </summary>
    public partial class SuppliesRequest : Page
    {
        /// <summary>
        /// Context
        /// </summary>
        private db ItemDB = new db();

        /// <summary>
        /// CollectionViewSource
        /// </summary>
        private CollectionViewSource item_requestViewSource, item_requestitem_request_detailViewSource;

        public SuppliesRequest()
        {
            InitializeComponent();
        }

        #region SmartBox Selection

        private void sbxItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                item item = ItemDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                if (item != null)
                {
                    item_request item_request = item_requestViewSource.View.CurrentItem as item_request;
                    if (item_request != null)
                    {
                        item_request_detail item_request_detail = new item_request_detail();
                        item_request_detail.id_item = sbxItem.ItemID;
                        item_request_detail.item = item;
                        item_request_detail.quantity = 1;
                        item_request_detail.urgency = entity.item_request_detail.Urgencies.Medium;
                        item_request.item_request_detail.Add(item_request_detail);
                        item_requestitem_request_detailViewSource.View.Refresh();
                    }
                }
            }
        }

        #endregion SmartBox Selection

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_requestitem_request_detailViewSource = (CollectionViewSource)this.FindResource("item_requestitem_request_detailViewSource");
            item_request item_request = new entity.item_request();
            item_request.name = "Supplier Request";
            ItemDB.item_request.Add(item_request);

            item_requestViewSource = (CollectionViewSource)this.FindResource("item_requestViewSource");
            item_requestViewSource.Source = ItemDB.item_request.Local;
            //sbxItem.item_types = entity.item.item_type.Supplies;
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            item_request item_request = item_requestViewSource.View.CurrentItem as item_request;

            if (item_request.item_request_detail.Count > 0)
            {
                //Search User.
                security_user security_user = new security_user();
                //Load User Name and Department.
                if (ItemDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault() != null)
                {
                    security_user = ItemDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault();
                    item_request.request_user = security_user;
                    item_request.security_user = security_user;
                    item_request.id_department = security_user.security_role.id_department;
                }

                //Get Branch.
                if (ItemDB.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault() != null)
                {
                    item_request.id_branch = ItemDB.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault().id_branch;
                }

                item_request.name = security_user.name + " Request";

                ItemDB.SaveChanges();

                //Creates a new Request
                item_request item_request_New = new item_request();
                ItemDB.item_request.Add(item_request_New);
                item_requestViewSource.View.MoveCurrentTo(item_request_New);
            }
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_request_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    item_request_detail item_request_detail = item_requestitem_request_detailViewSource.View.CurrentItem as item_request_detail;
                    //DeleteDetailGridRow
                    dgvSalesDetail.CancelEdit();
                    //item_request_detail.item_request_decision.Remove(e.Parameter as item_request_decision);
                    ItemDB.item_request_detail.Remove(e.Parameter as item_request_detail);
                    item_requestitem_request_detailViewSource.View.Refresh();

                    //calculate_total(sender, e);
                    item_request_detail.item_request.GetTotalDecision();
                }
            }
            catch
            {
                // toolBar.msgError(ex);
            }
        }
    }
}