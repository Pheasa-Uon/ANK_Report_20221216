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
    public partial class LoanOutstandingByDate : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string fromDate, toDate;
        public string format = "dd/MM/yyyy";
        public string dateFromError = "", dateToError = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Utils.DataHelper.checkLoginSession();
                //DataHelper.populateBranchDDLAllowAll(ddBranchName, DataHelper.getUserId());
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                var date = DataHelper.getSystemDateTextbox();
                dtpFromDate.Text = date;
                dtpToDate.Text = date;
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                fromDate = DateTime.ParseExact(dtpFromDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateFromError = "* Date wrong format";
                return;
            }
            try
            {
                toDate = DateTime.ParseExact(dtpToDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateToError = "* Date wrong format";
                return;
            }

            var ContractLoanOS = "ContractLoanOS";

            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@branch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters.Add(item: new Procedure() { field_name = "@frDate", sql_db_type = MySqlDbType.VarChar, value_name = fromDate });
            parameters.Add(item: new Procedure() { field_name = "@toDate", sql_db_type = MySqlDbType.VarChar, value_name = toDate });

            DataTable dt = db.getProcedureDataTable("ContractLoanOS", parameters);
            GenerateReport(dt);

        }

        private void GenerateReport(DataTable dt)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));

            var ds = new ReportDataSource("ContractOutstandingByDateDataset", dt);
            DataHelper.generateOperationReport(ReportViewer1, "ContractOutstandingByDate", reportParameters, ds);
        }
    }
}