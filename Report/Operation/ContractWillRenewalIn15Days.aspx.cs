using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using Report.Models;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Report.Operation
{
    public partial class ContractWillRenewalIn15Days : System.Web.UI.Page
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
        private void GenerateReport(DataTable ContractWillRenewalIn15DaysDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));

            var ContractWillRenewalIn15DaysDS = new ReportDataSource("ContractWillRenewalIn15DaysDS", ContractWillRenewalIn15DaysDT);

            DataHelper.generateOperationReport(ReportViewer1, "ContractWillRenewalIn15Days", reportParameters, ContractWillRenewalIn15DaysDS);

        }

        protected void btnView_Click(object sender, EventArgs e)
        {

            var ContractWillRenewalIn15DaysSQL = "PS_ContractWillRenewalIn15Days";

            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });

            DataTable ContractWillRenewalIn15DaysDT = db.getProcedureDataTable(ContractWillRenewalIn15DaysSQL, parameters);
            GenerateReport(ContractWillRenewalIn15DaysDT);
        }
    }
}