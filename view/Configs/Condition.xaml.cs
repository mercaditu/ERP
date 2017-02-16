using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Condition : Page
    {
        private entity.dbContext entity = new entity.dbContext();
        private CollectionViewSource conditionViewSource;

        public Condition()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conditionViewSource = ((CollectionViewSource)(this.FindResource("app_conditionViewSource")));
            entity.db.app_condition.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            conditionViewSource.Source = entity.db.app_condition.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.condition objCon = new cntrl.condition();
            app_condition obj_app_condition = new app_condition();
            entity.db.app_condition.Add(obj_app_condition);
            conditionViewSource.View.MoveCurrentToLast();
            objCon.conditionViewSource = conditionViewSource;
            objCon.entity = entity;
            crud_modal.Children.Add(objCon);
        }

        private void pnl_Condition_linkEdit_click(object sender, int intConditionId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.condition objCon = new cntrl.condition();
            conditionViewSource.View.MoveCurrentTo(entity.db.app_condition.Where(x => x.id_condition == intConditionId).FirstOrDefault());
            objCon.conditionViewSource = conditionViewSource;
            objCon.entity = entity;
            crud_modal.Children.Add(objCon);
        }
    }
}