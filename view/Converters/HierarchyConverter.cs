using System;
using System.Linq;
using System.Windows.Data;
using entity;
using System.Globalization;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;

namespace Cognitivo.Converters
{
    class HierarchyConverter : IValueConverter
    {
        bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!designTime)
            {
                using (db db = new db())
                {
                    List<project_task> project_task;
                    project_task = db.project_task.Where(i => i.parent.id_project_task == i.id_project_task).ToList();
                    return project_task;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
