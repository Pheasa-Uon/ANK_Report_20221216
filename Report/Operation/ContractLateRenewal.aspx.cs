using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using Report.Models;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Report.Operation
{
    public partial class ContractLateRenewal : System.Web.UI.Page
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
        private void GenerateReport(DataTable ContractLateRenewalDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));

            var ContractLateRenewalDS = new ReportDataSource("ContractLateRenewalDS", ContractLateRenewalDT);

            DataHelper.generateOperationReport(ReportViewer1, "ContractLateRenewal", reportParameters, ContractLateRenewalDS);

        }

        protected void btnView_Click(object sender, EventArgs e)
        {

            var ContractLateRenewalSQL = "PS_ContractLateRenewal";

            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });

            DataTable ContractLateRenewalDT = db.getProcedureDataTable(ContractLateRenewalSQL, parameters);
            GenerateReport(ContractLateRenewalDT);
        }
    }
}