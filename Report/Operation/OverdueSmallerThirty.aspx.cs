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
    public partial class OverdueSmallerThirty : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
       
        protected void Page_Load(object sender, EventArgs e)
        {
            DataHelper.checkLoginSession();
            DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            var sql = "Ps_Overdue_Smaller_Thirty";

            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });


            DataTable overduelargerDT = db.getProcedureDataTable(sql, procedureList);
            GenerateReport(overduelargerDT);

        }

        private void GenerateReport(DataTable dt)
        {
            var reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));

            var ds = new ReportDataSource("OverdueSmallerThirtyDS", dt);
            DataHelper.generateOperationReport(ReportViewer1, "OverdueSmallerThirty", reportParameters, ds);
        }
    }
}