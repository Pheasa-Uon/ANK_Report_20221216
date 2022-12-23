using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;


namespace Report.Accounting
{
    public partial class IncomeStatementYTD : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        private static string asOfDate;
        public string format = "dd/MM/yyyy";
        public string dateFromError = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                dtpAsOfDate.Text = DataHelper.getSystemDateTextbox();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                asOfDate = DateTime.ParseExact(dtpAsOfDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateFromError = "* Date wrong format";
                return;
            }
           
            var PS_INC = "PS_INC";
            var PS_GOS = "PS_GOS";
            var PS_TTGOS = "PS_TTGOS";
            var PS_EXP = "PS_EXP";
            var PS_TTNDI = "PS_TTNDI";
            var PS_OTHRE = "PS_OTHRE";
            var PS_NETINCOME = "PS_NETINCOME";

            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@TO_Date", sql_db_type = MySqlDbType.Date, value_name = asOfDate});

            DataTable PS_INC_DT = db.getProcedureDataTable(PS_INC, procedureList);
            DataTable PS_GOS_DT = db.getProcedureDataTable(PS_GOS, procedureList);
            DataTable PS_TTGOS_DT = db.getProcedureDataTable(PS_TTGOS, procedureList);
            DataTable PS_EXP_DT = db.getProcedureDataTable(PS_EXP, procedureList);
            DataTable PS_TTNDI_DT = db.getProcedureDataTable(PS_TTNDI, procedureList);
            DataTable PS_OTHRE_DT = db.getProcedureDataTable(PS_OTHRE, procedureList);
            DataTable PS_NETINCOME_DT = db.getProcedureDataTable(PS_NETINCOME, procedureList);

            GenerateReport(PS_INC_DT, PS_GOS_DT, PS_TTGOS_DT, PS_EXP_DT, PS_TTNDI_DT, PS_OTHRE_DT, PS_NETINCOME_DT);
        }

        private void GenerateReport(DataTable PS_INC_DT, DataTable PS_GOS_DT, DataTable PS_TTGOS_DT, DataTable PS_EXP_DT, DataTable PS_TTNDI_DT, DataTable PS_OTHRE_DT, DataTable PS_NETINCOME_DT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("AsOfDate", DateTime.ParseExact(dtpAsOfDate.Text, format, null).ToString("dd-MMM-yyyy")));
        
            var PS_INC_DS = new ReportDataSource("PS_INC", PS_INC_DT);
            var PS_GOS_DS = new ReportDataSource("PS_GOS", PS_GOS_DT);
            var PS_TTGOS_DS = new ReportDataSource("PS_TTGOS", PS_TTGOS_DT);
            var PS_EXP_DT_DS = new ReportDataSource("PS_EXP", PS_EXP_DT);
            var PS_TTNDI_DS = new ReportDataSource("PS_TTNDI", PS_TTNDI_DT);
            var PS_OTHRE_DS = new ReportDataSource("PS_OTHRE", PS_OTHRE_DT);
            var PS_NETINCOME_DS = new ReportDataSource("PS_NETINCOME", PS_NETINCOME_DT);

            DataHelper.generateAccountingReport(ReportViewer1, "IncomeStatementYTD", reportParameters,
                PS_INC_DS,
                PS_GOS_DS,
                PS_TTGOS_DS,
                PS_EXP_DT_DS,
                PS_TTNDI_DS,
                PS_OTHRE_DS,
                PS_NETINCOME_DS
                );
        }
    }
}