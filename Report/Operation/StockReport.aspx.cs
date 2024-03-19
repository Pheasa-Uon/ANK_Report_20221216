using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using Report.Models;
using System.Collections.Generic;

namespace Report.Operation
{
    public partial class StockReport : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public static string systemDateStr;
        public string format = "dd/MM/yyyy";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateOfficer();
                systemDateStr = DataHelper.getSystemDateStr();
            }
        }

        private void GenerateReport(DataTable stockreportDT)
        {
            var reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("PawnOfficer", ddOfficer.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("SystemDate", DataHelper.getSystemDate().ToString("dd-MMM-yyyy")));
            var _stockreport = new ReportDataSource("StockDS", stockreportDT);
            DataHelper.generateOperationReport(ReportViewer1, "StockReport", reportParameters, _stockreport);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            var dateFormat = DataHelper.getSystemDate().ToString("yyyy-MM-dd");

            string officer = null;
            if (ddOfficer.SelectedItem.Value != "0")
            {
                officer = ddOfficer.SelectedItem.Value;
            }

            var sql = "PS_StockPawnReport";

            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@pOfficer", sql_db_type = MySqlDbType.VarChar, value_name = officer });

            DataTable stockreportDT = db.getProcedureDataTable(sql, procedureList);
            GenerateReport(stockreportDT);
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
    }
}
