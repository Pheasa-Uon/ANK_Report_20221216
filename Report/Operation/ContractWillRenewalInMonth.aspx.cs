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
    public partial class ContractWillRenewalInMonth : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string fromDate, toDate;
        public string format = "dd/MM/yyyy";
        public string dateFromError = "", dateToError = "";
        public string officer;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateOfficer();
                var date = DataHelper.getSystemDateTextbox();
                dtpFromDate.Text = date;
                dtpToDate.Text = date;
            }
        }
        private void GenerateReport(DataTable ContractWillRenewalInMonthDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));

            var ContractWillRenewalInMonthDS = new ReportDataSource("ContractWillRenewalInMonthDS", ContractWillRenewalInMonthDT);

            DataHelper.generateOperationReport(ReportViewer1, "ContractWillRenewalInMonth", reportParameters, ContractWillRenewalInMonthDS);

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
            string officer = null;
            if (ddOfficer.SelectedItem.Value != "0")
            {
                officer = ddOfficer.SelectedItem.Value;
            }

            var ContractWillRenewalInMonthSQL = "PS_ContractWillRenewalInMonth";

            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters.Add(item: new Procedure() { field_name = "@FromDate", sql_db_type = MySqlDbType.VarChar, value_name = fromDate });
            parameters.Add(item: new Procedure() { field_name = "@ToDate", sql_db_type = MySqlDbType.VarChar, value_name = toDate });
            parameters.Add(item: new Procedure() { field_name = "@PawnOfficer", sql_db_type = MySqlDbType.VarChar, value_name = officer });


            DataTable ContractWillRenewalInMonthDT = db.getProcedureDataTable(ContractWillRenewalInMonthSQL, parameters);
            GenerateReport(ContractWillRenewalInMonthDT);
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
    }
}