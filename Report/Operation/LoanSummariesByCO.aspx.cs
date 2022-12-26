using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Report.Operation
{
    public partial class LoanSummariesByCO : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        private static string asOfDate;
        public string format = "dd/MM/yyyy";
        public string dateFromError = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                dtpAsOfDate.Text = DataHelper.getSystemDateTextbox();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                asOfDate = DateTime.ParseExact(dtpAsOfDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateFromError = "* Date wrong format";
                return;
            }

            var sql = "LoanSummariesByCO";

            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@pSystem_Date", sql_db_type = MySqlDbType.Date, value_name = asOfDate });

            DataTable dt = db.getProcedureDataTable(sql, procedureList);

            GenerateReport(dt);

        }

        private void GenerateReport(DataTable dt)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("AsOfDate", DateTime.ParseExact(dtpAsOfDate.Text, format, null).ToString("dd-MMM-yyyy")));


            var ds = new ReportDataSource("LoanSummariesByCODS", dt);

            DataHelper.generateOperationReport(ReportViewer1, "LoanSummariesByCO", reportParameters, ds);
        }
    }
}