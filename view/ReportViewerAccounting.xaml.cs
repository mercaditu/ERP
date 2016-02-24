using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using entity;
using Microsoft.Reporting.WinForms;

namespace View
{
    /// <summary>
    /// Interaction logic for ReportViewerAccounting.xaml
    /// </summary>
    public partial class ReportViewerAccounting : Window
    {
        entity.db db = new db();
        List<Cognitivo.Project.PrintingPress.calc_Cost> final_cost = new List<Cognitivo.Project.PrintingPress.calc_Cost>();
        public ReportViewerAccounting()
        {
            InitializeComponent();
        }

        public void loadBalanceGeneral()
        {
            try
            {
                //List<sales_invoice_detail> sales_invoice_detail = new List<entity.sales_invoice_detail>();
                //sales_invoice_detail = db.sales_invoice_detail.ToList();
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                //List<accounting_journal_detail> accounting_journal_detail = db.accounting_journal_detail.ToList();

                //reportDataSource.Value = accounting_journal_detail
                //              .Select(g => new
                //              {
                //                  code = g.accounting_journal.code,
                //                  name = g.accounting_chart.name,
                //                  debit = g.debit,
                //                  credit = g.credit,
                //                  company_name = g.accounting_journal.app_company.name,
                //                  gov_id = g.accounting_journal.app_company.gov_code,
                //                  legal_rep = g.accounting_journal.app_company.representative_name,
                //                  Accountant_name = g.accounting_journal.app_company.accountant_name,
                //                  Accountant_code = g.accounting_journal.app_company.accountant_gov_code,
                //                  desde = g.accounting_journal.accounting_cycle.start_date,
                //                  hasta = g.accounting_journal.accounting_cycle.end_date,
                //                  parent_id = g.accounting_chart.parent != null ? g.accounting_chart.parent.id_chart : 0
                //              }).ToList();



                List<accounting_chart> accounting_chart = db.accounting_chart.ToList();

                var chart = accounting_chart
                                .Select(g => new
                                {
                                    code = g.code,
                                    name = g.name,
                                    //parent1=g.parent.id_chart,
                                    //parent2 = g.parent.parent.id_chart,
                                    //parent3 = g.parent.parent.parent.id_chart,
                                    //parent4 = g.parent.parent.parent.parent.id_chart,
                                    //parent5 = g.parent.parent.parent.parent.parent.id_chart,
                                    child_total = g.child_total,
                                    company_name = g.app_company.name,
                                    gov_id = g.app_company.gov_code,
                                    legal_rep = g.app_company.representative_name,
                                    Accountant_name = g.app_company.accountant_name,
                                    Accountant_code = g.app_company.accountant_gov_code,
                                    desde = db.accounting_cycle.FirstOrDefault().start_date,
                                    hasta = db.accounting_cycle.FirstOrDefault().end_date

                                }).ToList();
                reportDataSource.Value = chart;
                reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\BALANCEGENERAL.rdlc"; // Path of the rdlc file

                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();

            }
            catch { }
        }
        public void loaddiario(DateTime start_date, DateTime end_date,Boolean global)
        {
            try
            {

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                List<accounting_journal_detail> accounting_journal_detail = db.accounting_journal_detail.ToList();
                //
                var journal_detail = accounting_journal_detail.Where(x => x.accounting_journal.trans_date >= start_date && x.accounting_journal.trans_date <= end_date)
                               .Select(g => new
                               {
                                   trans_date = g.accounting_journal.trans_date,
                                   code = g.accounting_journal.code,
                                   comment = g.accounting_journal.comment,
                                   id_journal = g.accounting_journal.id_journal,
                                   chart_code = g.accounting_chart.code,
                                   chart_name = g.accounting_chart.name,
                                   debit = g.debit,
                                   credit = g.credit,
                                   id_branch = g.accounting_journal.id_branch,
                                   branch_code = g.accounting_journal.app_branch.code,
                                   branch_name = g.accounting_journal.app_branch.name,
                               }).ToList();


                reportDataSource.Value = journal_detail;
                if (global == true)
                {
                    reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Accounting\\DiarioGlobal.rdlc"; // Path of the rdlc file
                }
                else
                {
                    reportViewer.LocalReport.ReportPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Accounting\\Diario.rdlc"; // Path of the rdlc file
                }
                ReportParameter[] param = new ReportParameter[2];
                param[0] = new ReportParameter("startdate", start_date.ToString());
                param[1] = new ReportParameter("enddate", end_date.ToString());
                reportViewer.LocalReport.SetParameters(param);
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();

            }
            catch { }
        }
    }
}
