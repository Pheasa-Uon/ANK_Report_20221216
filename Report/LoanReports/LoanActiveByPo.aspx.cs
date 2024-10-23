using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Services.Discovery;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Report.LoanReports
{
    public partial class LoanActiveByPo : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateOfficer();
                populateCustomer(); 
            }

        }

        protected void ddBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateOfficer();
        }
        private void populateOfficer()
        {
            if (ddBranchName.SelectedItem.Value != "")
            {
                if (ddBranchName.SelectedItem.Value == "ALL")
                {
                    ddOfficer.Enabled = true;
                    DataHelper.populateOfficerDDLAll(ddOfficer);
                }
                else
                {
                    ddOfficer.Enabled = true;
                    DataHelper.populateOfficerDDL(ddOfficer, Convert.ToInt32(ddBranchName.SelectedItem.Value));
                }

            }
            else
            {
                ddOfficer.Enabled = false;
                ddOfficer.Items.Clear();
            }
        }
        protected void ddOffcer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateCustomer();
        }

        private void populateCustomer()
        {
            if(ddOfficer.SelectedItem.Value != "")
            {
                if (ddOfficer.SelectedItem.Value == "0")
                {
                    ddCustomer.Enabled = true;
                    DataHelper.populateCustomerDDLAll(ddCustomer);
                }
                else
                {
                    ddCustomer.Enabled = true;
                    DataHelper.populateCustomerDDL(ddCustomer, Convert.ToInt32(ddOfficer.SelectedItem.Value));
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
            string customer =null;
            string office =null;

            if (ddCustomer.SelectedItem.Value != "0" && ddOfficer.SelectedItem.Value != "0")
            {
                customer = ddCustomer.SelectedItem.Value;
                office = ddOfficer.SelectedItem.Value;
            }
            var spd = "";
            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@pOfficer", sql_db_type = MySqlDbType.VarChar, value_name = office });
            procedureList.Add(item: new Procedure() { field_name = "@pCustomer", sql_db_type = MySqlDbType.VarChar, value_name = customer });


            DataTable dt = db.getProcedureDataTable(spd, procedureList);
            GenerateReport(dt);
        }

        private void GenerateReport(DataTable dt)
        {

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("Officer", ddCustomer.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("Customername", ddCustomer.SelectedItem.Text));

            var ds = new ReportDataSource("", dt);
            DataHelper.generateOperationReport(ReportViewer1, "LoanActiveByPo", reportParameters, ds);
        }
    }
}