using System;
using System.Collections.Generic;
using System.Data;

namespace entity.BrilloQuery
{
    public class Return
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Comment { get; set; }
        public decimal Balance { get; set; }
    }

    public class SalesRetrunQuery : IDisposable
    {
        public ICollection<Return> List { get; set; }

        public SalesRetrunQuery(int ContactID)
        {
            List = new List<Return>();

            string query = @" SELECT
                sr.id_sales_return AS ID,
                sr.code AS Code,
                sr.comment AS Comment,
                sr.number AS Number
                ,(select sum(debit) - sum(Credit) from payment_schedual where id_sales_return=srd.id_sales_return) as Balance,
                contacts.name as Name
                FROM
                sales_return_detail as srd

                LEFT OUTER JOIN
                (SELECT app_vat_group.id_vat_group, sum(app_vat.coefficient) as vat, sum(app_vat.coefficient) + 1 AS coef
                FROM app_vat_group
                LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
                LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
                GROUP BY app_vat_group.id_vat_group)
                vatco ON vatco.id_vat_group = srd.id_vat_group

                inner join sales_return as sr on srd.id_sales_return = sr.id_sales_return
                left outer join
                payment_schedual ON sr.id_sales_return = payment_schedual.id_sales_return
                inner join contacts on sr.id_contact=contacts.id_contact
                where (sr.id_company = {0} and sr.status=2 and sr.id_contact={1} ) and 
                (select sum(debit) - sum(Credit) from payment_schedual where id_sales_return=srd.id_sales_return) > 0
                group by sr.id_sales_return";

            query = String.Format(query, CurrentSession.Id_Company, ContactID);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    Return Return = new Return();

                    Return.ID = Convert.ToInt16(DataRow["ID"]);
                    Return.Code = Convert.ToString(DataRow["Code"]);
                    Return.Name = Convert.ToString(DataRow["Name"]);
                    Return.Comment = Convert.ToString(DataRow["Comment"]);
                    Return.Number = Convert.ToString(DataRow["Number"]);
                    Return.Balance = Convert.ToDecimal(DataRow["Balance"]);
                    if (Return.Balance > 0)
                    {
                        List.Add(Return);
                    }
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this != null)
            {
                if (disposing)
                {
                    this.Dispose();
                    // Dispose other managed resources.
                }
            }
        }
    }

    public class PurchaseReturnQuery : IDisposable
    {
        public ICollection<Return> List { get; set; }

        public PurchaseReturnQuery(int ContactID)
        {
            List = new List<Return>();

            string query = @" SELECT
                pr.id_purchase_return AS ID,
                pr.code AS Code,
                pr.comment AS Comment,
                pr.number AS Number,
                (select sum(credit) - sum(debit) from payment_schedual where id_purchase_return = prd.id_purchase_return) as Balance,
                contacts.name as Name
                FROM
                purchase_return_detail as prd

                LEFT OUTER JOIN
                (SELECT app_vat_group.id_vat_group, sum(app_vat.coefficient) as vat, sum(app_vat.coefficient) + 1 AS coef
                FROM app_vat_group
                LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
                LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
                GROUP BY app_vat_group.id_vat_group)
                vatco ON vatco.id_vat_group = prd.id_vat_group

                inner join purchase_return as pr on prd.id_purchase_return = pr.id_purchase_return
                left outer join
                payment_schedual ON pr.id_purchase_return = payment_schedual.id_purchase_return
                inner join contacts on pr.id_contact=contacts.id_contact

                where (pr.id_company = {0} and pr.status=2 and pr.id_contact={1} ) and
                (select sum(credit) - sum(debit) from payment_schedual where id_purchase_return = prd.id_purchase_return) > 0
                group by pr.id_purchase_return";

            query = String.Format(query, CurrentSession.Id_Company, ContactID);
            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    Return Return = new Return();
                    Return.ID = Convert.ToInt16(DataRow["ID"]);
                    Return.Code = Convert.ToString(DataRow["Code"]);
                    Return.Name = Convert.ToString(DataRow["Name"]);
                    Return.Comment = Convert.ToString(DataRow["Comment"]);
                    Return.Number = Convert.ToString(DataRow["Number"]);
                    Return.Balance = Convert.ToDecimal(DataRow["Balance"]);

                    //This blocks 0 Balanced Returns from showing.
                    if (Return.Balance > 0)
                    {
                        List.Add(Return);
                    }
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this != null)
            {
                if (disposing)
                {
                    this.Dispose();
                }
            }
        }
    }
}