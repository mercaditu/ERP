using entity;
using System;
using System.Collections.Generic;
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
using System.Linq;

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
        ItemDB ItemDB = new ItemDB();
     

        /// <summary>
        /// CollectionViewSource
        /// </summary>
        CollectionViewSource item_requestViewSource, item_requestitem_request_detailViewSource;
 
        public SuppliesRequest()
        {
            InitializeComponent();
        }

        #region ActionButtons

     

     

        private void btnSave_Click(object sender, EventArgs e)
        {
            ItemDB.SaveChanges();
            item_request item_request = new entity.item_request();
            item_request.name = "Supplier Request";
            ItemDB.item_request.Add(item_request);
            
        }

      
        #endregion

        #region SmartBox Selection

      

        private void sbxItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID>0)
            {
                item item = ItemDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                item_request item_request = item_requestViewSource.View.CurrentItem as item_request;
                item_request_detail item_request_detail = new item_request_detail();
                item_request_detail.id_item = sbxItem.ItemID;
                item_request_detail.item = item;
                item_request_detail.quantity = 1;
                item_request_detail.urgency = entity.item_request_detail.Urgencies.Medium;
                item_request.item_request_detail.Add(item_request_detail);
                item_requestitem_request_detailViewSource.View.Refresh();
            }
         
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_requestitem_request_detailViewSource = (CollectionViewSource)this.FindResource("item_requestitem_request_detailViewSource");
            item_request item_request = new entity.item_request();
            item_request.name = "Supplier Request";
            ItemDB.item_request.Add(item_request);
            item_requestViewSource = (CollectionViewSource)this.FindResource("item_requestViewSource");
            item_requestViewSource.Source = ItemDB.item_request.Local;
            sbxItem.item_types = entity.item.item_type.Supplies;
        }

        

      

       
    }
}
