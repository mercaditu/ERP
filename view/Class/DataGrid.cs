using WPFLocalizeExtension.Extensions;

namespace Cognitivo.Class
{
    public partial class DataGrid : System.Windows.Controls.DataGrid
    {
        public DataGrid()
        {
           //DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGrid)), new FrameworkPropertyMetadata(typeof(DataGrid)));
        }
        
        public override void EndInit()
        {
            base.EndInit();
            Localize();
        }

        private void Localize()
        {
            foreach (var Column in Columns)
            {
                string str = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + Column.Header.ToString());
                if (!string.IsNullOrEmpty(str))
                {
                    Column.Header = str;
                }
            }
        }
    }
}
