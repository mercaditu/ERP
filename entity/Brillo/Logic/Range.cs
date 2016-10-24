using System;
using System.Collections.Generic;
using System.Linq;

namespace entity.Brillo.Logic
{
    public static class Range
    {
        public static string branch_Code { get; set; }
        public static string terminal_Code { get; set; }
        public static string user_Code { get; set; }
        public static string project_Code { get; set; }

        public static string calc_Range(app_document_range original_app_document_range, bool is_generated)
        {
            string prefix = string.Empty;

            if (original_app_document_range != null)
            {
                int current_value = original_app_document_range.range_current;
                int end_value = original_app_document_range.range_end;

                //Range Calculator
                prefix = original_app_document_range.range_template;
                prefix = return_Prefix(prefix);

                if (prefix != null & current_value <= end_value)
                {
                    ///Get latest Current Value to be sure we don't use the same value somebody else has already used.
                    using (db db = new db())
                    {
                        app_document_range _app_document_range = db.app_document_range.Find(original_app_document_range.id_range);

                        //Range
                        if (prefix.Contains("#Range"))
                        {
                            //Add Padding filler
                            _app_document_range.range_current += 1;
                            string str = _app_document_range.range_current.ToString(original_app_document_range.range_padding);
                            prefix = prefix.Replace("#Range", str);
                        }
                        else
                        {
                            _app_document_range.range_current += 1;
                            prefix = prefix + _app_document_range.range_current.ToString(original_app_document_range.range_padding);
                        }

                        if (is_generated)
                        {
                            original_app_document_range.range_current = _app_document_range.range_current;
                            if (original_app_document_range.range_current == end_value)
                            {
                                original_app_document_range.is_active = false;
                            }

                            //Save new number into database, as quick as possible.
                            db.SaveChangesAsync();
                        }
                    }
                }
            }
            return prefix;
        }

        private static string return_Prefix(string prefix)
        {
            //project
            if (prefix.Contains("#Project"))
            { prefix = prefix.Replace("#Project", project_Code); }
            //user
            if (prefix.Contains("#User"))
            { prefix = prefix.Replace("#User", user_Code); }
            //Branch
            if (prefix.Contains("#Branch"))
            { prefix = prefix.Replace("#Branch", branch_Code); }

            //Terminal
            if (prefix.Contains("#Terminal"))
            { prefix = prefix.Replace("#Terminal", terminal_Code); }

            //Year
            if (prefix.Contains("#Year"))
            { prefix = prefix.Replace("#Year", DateTime.Now.Year.ToString()); }

            //Month
            if (prefix.Contains("#Month"))
            { prefix = prefix.Replace("#Month", DateTime.Now.Month.ToString()); }

            //Now
            if (prefix.Contains("#Now"))
            { prefix = prefix.Replace("#Now", DateTime.Now.Date.ToString()); }

            //Range will be calculated later on, as there is extra business logic to be handled
            return prefix;
        }

        public static List<app_document_range> List_Range(db db, App.Names AppName, int BranchID, int TerminalID)
        {
            return db.app_document_range.Where(x =>
                                    (
                                    x.app_document.filterby_branch == false && 
                                    x.app_document.filterby_tearminal == false
                                    )
                                      ||
                                    (
                                    x.app_document.filterby_branch == true && 
                                    x.id_branch == BranchID && 
                                    x.app_document.filterby_tearminal == true && 
                                    x.id_terminal == TerminalID
                                    ) 
                                    &&
                                    x.is_active && x.id_company == CurrentSession.Id_Company && 
                                    x.app_document.id_application == AppName
                                    ).OrderBy(x => x.expire_date)
                             .ToList();
        }
    }
}
