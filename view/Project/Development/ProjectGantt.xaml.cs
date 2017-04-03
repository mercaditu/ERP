using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Project.Development
{
    /// <summary>
    /// Interaction logic for ProjectGantt.xaml
    /// </summary>
    public partial class ProjectGantt : Page
    {
        private entity.db db = new entity.db();

        public ProjectGantt()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            CollectionViewSource projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));

            await db.projects.Where(a => a.is_active && a.id_company == entity.CurrentSession.Id_Company).LoadAsync();
            projectViewSource.Source = db.projects.Local;
        }
    }
}