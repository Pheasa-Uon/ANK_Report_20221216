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
    public partial class LoanActiveByClientName : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        string urlPath = HttpContext.Current.Request.Url.AbsoluteUri;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateCustomer();
            }
        }

        protected void ddBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateCustomer();
        }

        private void populateCustomer()
        {
            if (ddBranchName.SelectedItem.Value != "")
            {
                if (ddBranchName.SelectedItem.Value == "ALL")
                {
                    ddCustomer.Enabled = true;
                    DataHelper.customerGetName(ddCustomer);
                }
                else
                {
                    ddCustomer.Enabled = true;
                    DataHelper.customerGetName(ddCustomer);
                }

            }
            else
            {
                ddCustomer.Enabled = false;
                ddCustomer.Items.Clear();
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            string customer = null;
            if (ddCustomer.SelectedItem.Value != "0")
            {
                customer = ddCustomer.SelectedItem.Value;
            }

            var spd = "";
            List<Procedure> procedureList = new List<Procedure>();

            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@pCustomer", sql_db_type = MySqlDbType.VarChar, value_name = customer });
            DataTable dt = db.getProcedureDataTable(spd, procedureList);
            GenerateReport(dt);
        }

        private void GenerateReport(DataTable dt)
        {

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("Customername", ddCustomer.SelectedItem.Text));

            var ds = new ReportDataSource("", dt);
            DataHelper.generateOperationReport(ReportViewer1, "LoanActiveByClientName", reportParameters, ds);
        }
    }
}