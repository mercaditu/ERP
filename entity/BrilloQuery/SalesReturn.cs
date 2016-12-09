using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{

  
    public class Return
    {
        public int ID { get; set; }
        public string code { get; set; }
        public string number { get; set; }
        public string comment { get; set; }
        public string Balance { get; set; }
    }
    public class SalesRetrunQuery : IDisposable
    {
        public ICollection<Return> List { get; set; }
        public SalesRetrunQuery()
        {
            List = new List<Return>();

            string query = @" SELECT 
    sr.id_sales_return AS ID,
    code AS Code,
    sr.comment AS Comment,
    sr.number AS Number
   ,sum((srd.quantity * srd.unit_price * (vatco.vat + 1)))-sum(payment_schedual.credit) as Balance
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
   where (sr.id_company = {0} ) 
    group by sr.id_sales_return";     
                               

            query = String.Format(query, entity.CurrentSession.Id_Company);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    Return Return = new Return();

                    Return.ID = Convert.ToInt16(DataRow["ID"]);
                    Return.code = Convert.ToString(DataRow["Code"]);
                    Return.comment = Convert.ToString(DataRow["Comment"]);
                    Return.number = Convert.ToString(DataRow["Number"]);
                    Return.Balance = Convert.ToString(DataRow["Balance"]);
                    List.Add(Return);
                }
            }
        }

        public void Dispose()
        {
            // Dispose(true);
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
                //release unmanaged resources.
            }
        }
    }
    public class PurchaseReturnQuery : IDisposable
    {
        public ICollection<Return> List { get; set; }
        public PurchaseReturnQuery()
        {
            List = new List<Return>();

            string query = @" SELECT 
    pr.id_purchase_return AS ID,
    code AS Code,
    pr.comment AS Comment,
    pr.number AS Number
   ,sum((prd.quantity * prd.unit_cost * (vatco.vat + 1)))-sum(payment_schedual.debit) as Balance
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
   where (pr.id_company = {0} ) 
    group by pr.id_purchase_return";


            query = String.Format(query, entity.CurrentSession.Id_Company);

            using (DataTable dt = QueryExecutor.DT(query))
            {
                foreach (DataRow DataRow in dt.Rows)
                {
                    Return Return = new Return();

                    Return.ID = Convert.ToInt16(DataRow["ID"]);
                    Return.code = Convert.ToString(DataRow["Code"]);
                    Return.comment = Convert.ToString(DataRow["Comment"]);
                    Return.number = Convert.ToString(DataRow["Number"]);
                    Return.Balance = Convert.ToString(DataRow["Balance"]);
                    List.Add(Return);
                }
            }
        }

        public void Dispose()
        {
            // Dispose(true);
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
                //release unmanaged resources.
            }
        }
    }
}



