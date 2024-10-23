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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());

            }

        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            var spd = "";
            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            DataTable dt = db.getProcedureDataTable(spd, parameters);
            GenerateReport(dt);
        }

        private void GenerateReport(DataTable dt)
        {
            var reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("SystemDate", DataHelper.getSystemDateStr()));

            var ds = new ReportDataSource("", dt);
            DataHelper.generateOperationReport(ReportViewer1, "AllLoanActive", reportParameters, ds);
        }
    }
}