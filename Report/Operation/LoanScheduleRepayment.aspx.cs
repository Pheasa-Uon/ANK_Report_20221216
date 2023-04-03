using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Report.Operation
{
    public partial class LoanScheduleRepayment : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                txtContract.Text = "";
            }
        }
        private void GenerateReport(DataTable dt)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("@pACNO", txtContract.Text.Trim()));

            var ds = new ReportDataSource("LoanScheduleRepaymentDS", dt);

            DataHelper.generateOperationReport(ReportViewer1, "LoanScheduleRepayment", reportParameters, ds);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            
            var sql = "PS_SCHEDULE_LOAN";

            List<Procedure> procedureList = new List<Procedure>();

            DataTable dt = db.getProcedureDataTable(sql, procedureList);

            GenerateReport(dt);
        }
    }
}