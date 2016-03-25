using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Project
{
    /// <summary>
    /// Interaction logic for ProjectTag.xaml
    /// </summary>
    public partial class ProjectTag : Page
    {
       dbContext entity = new dbContext();
       CollectionViewSource project_tagViewSource = null;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public ProjectTag()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_tagViewSource = (CollectionViewSource)Resources["project_tagViewSource"];
            entity.db.project_tag.Where(a => a.id_company == _entity.company_ID && a.is_active == true).OrderBy(a => a.name).Load();
            project_tagViewSource.Source = entity.db.project_tag.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ProjectTag _ProjectTag = new cntrl.Curd.ProjectTag();
            project_tag project_tag = new project_tag();
            entity.db.project_tag.Add(project_tag);
            project_tagViewSource.View.MoveCurrentToLast();
            _ProjectTag.project_tagViewSource = project_tagViewSource;
            _ProjectTag.entity = entity;
            crud_modal.Children.Add(_ProjectTag);
        }

        private void pnl_item_tag_linkEdit_Click(object sender, int idContactTag)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ProjectTag _ProjectTag = new cntrl.Curd.ProjectTag();
            project_tagViewSource.View.MoveCurrentTo(entity.db.project_tag.Where(x => x.id_tag == idContactTag).FirstOrDefault());
            _ProjectTag.project_tagViewSource = project_tagViewSource;
            _ProjectTag.entity = entity;
            crud_modal.Children.Add(_ProjectTag);
        }

    }
}
