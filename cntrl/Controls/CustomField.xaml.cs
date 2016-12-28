using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for CustomField.xaml
    /// </summary>
    public partial class CustomField : UserControl
    {



        public static DependencyProperty FieldIDProperty = DependencyProperty.Register("FieldID", typeof(Int32), typeof(CustomField));
        public Int32 FieldID
        {
            get { return Convert.ToInt32(GetValue(FieldIDProperty)); }
            set { SetValue(FieldIDProperty, value); }
        }
        
        public entity.app_field.field_types appFieldTypes { get; set; }
      

        public CustomField()
        {
            InitializeComponent();
        }

        private void CustomField_Loaded(object sender, RoutedEventArgs e)
        {
            cbxFieldType.ItemsSource = entity.CurrentSession.AppField.Where(x=>x.field_type== appFieldTypes);
        }
    }
}
