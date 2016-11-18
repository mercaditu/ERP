using System.Collections.Generic;

namespace cntrl.Class
{
    public class Report
    {
        public entity.App.Names Application { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }

        List<Report_Parameter> Parameters { get; set; }
    }

    public class Report_Parameter
    {
        public enum Types { StartDate, EndDate }
        public Types Type { get; set; }
    }
}
