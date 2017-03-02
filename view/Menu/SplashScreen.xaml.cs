using System;
using System.Deployment.Application;
using System.Reflection;
using System.Windows;

namespace Cognitivo.Menu
{
    public partial class SplashScreen : Window
    {
        public Version AssemblyVersion
        {
            get
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }

        public Version LocalVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
        }
    }
}
