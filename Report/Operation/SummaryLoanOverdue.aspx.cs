using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Report.Operation
{
    public partial class SummaryLoanOverdue : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public static string systemDateStr;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                systemDateStr = DataHelper.getSystemDate().ToString("dd/MM/yyyy");

            }

        }
        private void GenerateReport(DataTable AccountReceivableAgingReportDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("SystemDate", DataHelper.getSystemDateStr()));

            var lateDS = new ReportDataSource("SummaryLoanOverdueDS", AccountReceivableAgingReportDT);

            DataHelper.generateOperationReport(ReportViewer1, "SummaryLoanOverdue", reportParameters, lateDS);

        }

        protected void btnView_Click(object sender, EventArgs e)
        {

            var ar = "PS_Summary_loan_overdue";

            List<Procedure> parameters = new List<Procedure>();

            DataTable SummaryLoanOverdueDT = db.getProcedureDataTable(ar, parameters);

            GenerateReport(SummaryLoanOverdueDT);
        }
    }
}