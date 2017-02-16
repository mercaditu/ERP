using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for VATGroup.xaml
    /// </summary>
    public partial class VATGroup : Page
    {
        private entity.dbContext _entity = new entity.dbContext();
        private CollectionViewSource app_vat_groupViewSource;
        private Cognitivo.Properties.Settings _pref_Cognitivo = new Cognitivo.Properties.Settings();
        // entity.Properties.Settings _settings = new entity.Properties.Settings();

        public VATGroup()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_vat_groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_vat_groupViewSource")));
            _entity.db.app_vat_group.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_vat_groupViewSource.Source = _entity.db.app_vat_group.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.vat_group vat_group = new cntrl.Curd.vat_group();
            app_vat_group app_vat_group = new app_vat_group();
            _entity.db.app_vat_group.Add(app_vat_group);
            app_vat_groupViewSource.View.MoveCurrentToLast();
            vat_group.app_vat_groupViewSource = app_vat_groupViewSource;
            vat_group._entity = _entity;
            crud_modal.Children.Add(vat_group);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.vat_group vat_group = new cntrl.Curd.vat_group();
            app_vat_groupViewSource.View.MoveCurrentTo(_entity.db.app_vat_group.Where(x => x.id_vat_group == intId).FirstOrDefault());
            vat_group.app_vat_groupViewSource = app_vat_groupViewSource;
            vat_group._entity = _entity;

            crud_modal.Children.Add(vat_group);
        }
    }
}