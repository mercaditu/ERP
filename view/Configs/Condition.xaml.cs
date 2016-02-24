using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for ConditionRecords.xaml
    /// </summary>
    public partial class Condition : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource conditionViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        public Condition()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            conditionViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_conditionViewSource")));
            entity.db.app_condition.Where(a=>a.id_company == _entity.company_ID).OrderByDescending(a => a.is_active).Load();
            conditionViewSource.Source = entity.db.app_condition.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
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
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.condition objCon = new cntrl.condition();
            conditionViewSource.View.MoveCurrentTo(entity.db.app_condition.Where(x => x.id_condition == intConditionId).FirstOrDefault());
            objCon.conditionViewSource = conditionViewSource;
            objCon.entity = entity;
            crud_modal.Children.Add(objCon);
        }
    }
}
