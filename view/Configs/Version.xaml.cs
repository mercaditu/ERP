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

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Version.xaml
    /// </summary>
    public partial class Version : Page
    {
        public int UserNumber { get; set; }

        public Version()
        {
            using (entity.db db = new entity.db())
            {
                UserNumber = db.security_user.Where(x => x.id_company == entity.CurrentSession.Id_Company).Count();
            }

            InitializeComponent();
        }
    }
}
