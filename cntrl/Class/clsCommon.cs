using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using entity;

namespace cntrl.Class
{
    public static class clsCommon
    {
        public enum Mode
        {
            Add, Edit
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj, string name) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T &&
                          (child as FrameworkElement).Name.Equals(name))
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child, name))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static security_crud getUserSecurityValues(int UserId, App.Names Application)
        {
            using(db db = new db())
            {
                security_user security_user = db.security_user.Where(x => x.id_user == UserId).FirstOrDefault();
                if(security_user != null)
                {
                    int id_role = security_user.id_role;
                    return db.security_curd.Where(x => x.id_role == id_role && x.id_application == Application).FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
