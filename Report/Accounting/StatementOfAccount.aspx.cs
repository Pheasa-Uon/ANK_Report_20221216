using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Report.Accounting
{
    public partial class StatementOfAccount : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string format = "dd/MM/yyyy";
        public string dateError = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                txtContract.Text = "";
                dtpSystemDate.Text = DataHelper.getSystemDate().ToString(format);
            }
        }
        private void GenerateReport(DataTable dt)
        {
            var reportParameters = new ReportParameterCollection();
            // reportParameters.Add(new ReportParameter("ContractNo", txtContract.Text));
            reportParameters.Add(new ReportParameter("SystemDate", DateTime.ParseExact(dtpSystemDate.Text.Trim(), format, null).ToString("dd-MMM-yyyy")));

            var ds = new ReportDataSource("SOA", dt);
            DataHelper.generateAccountingReport(ReportViewer1, "StatementOfAccount", reportParameters, ds);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            var dateSearch = "";
            try
            {
                dateSearch = DateTime.ParseExact(dtpSystemDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateError = "* Date wrong format";
                return;
            }


            var spd = "StatementOfAccount";
            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pCNO", sql_db_type = MySqlDbType.VarChar, value_name = txtContract.Text });
            parameters.Add(item: new Procedure() { field_name = "@SystemDate", sql_db_type = MySqlDbType.VarChar, value_name = dateSearch });

            DataTable dt = db.getProcedureDataTable(spd, parameters);
            GenerateReport(dt);
        }
    }
}