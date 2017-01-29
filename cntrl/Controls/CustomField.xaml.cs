using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;
using System.Data.Entity;

namespace cntrl.Controls
{
    public partial class CustomField : UserControl
    {
        db db = new db();

        public static DependencyProperty FieldIDProperty = DependencyProperty.Register("FieldID", typeof(short), typeof(CustomField));
        public short FieldID
        {
            get { return (short)GetValue(FieldIDProperty); }
            set { SetValue(FieldIDProperty, value); }
        }

        public static DependencyProperty FieldNameProperty = DependencyProperty.Register("FieldName", typeof(string), typeof(CustomField));
        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        public static DependencyProperty FieldValueProperty = DependencyProperty.Register("FieldValue", typeof(string), typeof(CustomField));
        public string FieldValue
        {
            get { return (string)GetValue(FieldValueProperty); }
            set { SetValue(FieldValueProperty, value); }
        }

        public app_field.field_types appFieldTypes { get; set; }
      

        public CustomField()
        {
            InitializeComponent();
        }

        private void CustomField_Loaded(object sender, RoutedEventArgs e)
        {
            db.app_field.Where(x => x.field_type == appFieldTypes).Load();
            cbxFieldType.ItemsSource = db.app_field.Local;
        }

        private void cbxFieldType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxFieldType.SelectedItem!=null)
            {
                FieldName = cbxFieldType.Text;
            }
        }

        private void Add_field(object sender, MouseButtonEventArgs e)
        {
            app_field app_field = new app_field();
            app_field.field_type = appFieldTypes;
            app_field.name = "New";
            db.app_field.Add(app_field);
            db.SaveChanges();

            cbxFieldType.SelectedItem = app_field;
        }
    }
}
