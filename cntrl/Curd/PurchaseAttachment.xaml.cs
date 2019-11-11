using entity;
using entity.Brillo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DataObject = System.Windows.DataObject;

namespace cntrl.Curd
{
    public partial class PurchaseAttachment : UserControl
    {
        public db PurchaseInvoiceDB { get; set; }

        public List<purchase_invoice_detail> purchase_Invoice_Details { get; set; }

        public PurchaseAttachment()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource purchase_detailViewSource = ((CollectionViewSource)(FindResource("purchase_detailViewSource")));
                purchase_detailViewSource.Source = purchase_Invoice_Details;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DataObject data = new DataObject();
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                foreach (purchase_invoice_detail item in purchase_Invoice_Details.Where(x => x.IsSelected))
                {
                    foreach (item_movement item_movement in item.item_movement)
                    {

                        app_attachment app_attachment = new app_attachment();

                        app_attachment.mime = "image/" + Path.GetExtension(op.FileName); ;

                        Byte2FileConverter ByteConverter = new Byte2FileConverter();
                        app_attachment.file = ByteConverter.ResizeImage(op.FileName.ToString());


                        app_attachment.reference_id = Convert.ToInt32(item_movement.id_movement);
                        app_attachment.application = App.Names.Movement;


                        PurchaseInvoiceDB.app_attachment.Add(app_attachment);
                        PurchaseInvoiceDB.SaveChangesAsync();


                    }

                }
            }
        }
    }
}