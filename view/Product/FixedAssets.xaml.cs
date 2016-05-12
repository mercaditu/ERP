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
using entity;

namespace Cognitivo.Product
{
    public partial class FixedAssets : Page
    {
        entity.ItemDB ItemDB = new entity.ItemDB();

        public FixedAssets()
        {
            InitializeComponent();
        }

        private void toolBar_Mini_btnSave_Click(object sender)
        {

        }

        private void toolBar_Mini_btnEdit_Click(object sender)
        {

        }

        private void toolBar_Mini_btnNew_Click(object sender)
        {

        }

        private void toolBar_btnEdit_Click(object sender)
        {

        }

        private void toolBar_btnSave_Click(object sender)
        {

        }

        private void toolBar_btnNew_Click(object sender)
        {

        }

        private void toolBar_btnCancel_Click(object sender)
        {

        }

        private void toolBar_btnDelete_Click(object sender)
        {

        }

        private void toolBar_btnApprove_Click(object sender)
        {

        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ItemDB.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.FixedAssets).ToList();
        }
    }
}
