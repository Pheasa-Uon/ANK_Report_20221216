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

namespace Report.LoanReports
{
    public partial class AllLoanActive : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        string urlPath = HttpContext.Current.Request.Url.AbsoluteUri;
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
            var spd = "PS_AllLoanActiveDetails";
            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters.Add(item: new Procedure() { field_name = "@pDate", sql_db_type = MySqlDbType.Date, value_name = asOfDate });
            DataTable dt = db.getProcedureDataTable(spd, parameters);
            GenerateReport(dt);
        }

        private void GenerateReport(DataTable dt)
        {
            var reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("AsOfDate", DateTime.ParseExact(dtpAsOfDate.Text, format, null).ToString("dd-MMM-yyyy")));

            var ds = new ReportDataSource("AllLoanActiveDT", dt);
            DataHelper.generateLoanReports(ReportViewer1, "AllLoanActive", reportParameters, ds);
        }
    }
}