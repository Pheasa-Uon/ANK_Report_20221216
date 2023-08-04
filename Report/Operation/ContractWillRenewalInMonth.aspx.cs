using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using Report.Models;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Report.Operation
{
    public partial class ContractWillRenewalInMonth : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
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

            var ContractWillRenewalInMonthSQL = "PS_ContractWillRenewalInMonth";

            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });

            DataTable ContractWillRenewalInMonthDT = db.getProcedureDataTable(ContractWillRenewalInMonthSQL, parameters);
            GenerateReport(ContractWillRenewalInMonthDT);
        }
    }
}