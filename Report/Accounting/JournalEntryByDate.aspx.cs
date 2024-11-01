﻿using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using MySql.Data.MySqlClient;
using Report.Models;
using System.Collections.Generic;

namespace Report.Accounting
{
    public partial class JournalEntryByDate : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string fromDate, toDate;
        public string format = "dd/MM/yyyy";
        public string dateFromError = "", dateToError = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                DataHelper.populateTransactionTypeDDL(ddTransactionType);
                this.populateUser();
                var date = DataHelper.getSystemDateTextbox();
                dtpFromDate.Text = date;
                dtpToDate.Text = date;
            }
        }
        private void populateUser()
        {
            if (ddBranchName.SelectedItem.Value != "")
            {
                if (ddBranchName.SelectedItem.Value == "ALL")
                {
                    ddUser.Enabled = true;
                    DataHelper.GetUserNamesAll(ddUser);
                }
                else
                {
                    ddUser.Enabled = true;
                    DataHelper.GetUserNames(ddUser, Convert.ToInt32(ddBranchName.SelectedItem.Value));
                }

            }
            else
            {
                ddUser.Enabled = false;
                ddUser.Items.Clear();
            }
        }

        protected void ddBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateUser();
        }

        private void GenerateReport(DataTable trialBalanceDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("TransactionType", ddTransactionType.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("User", ddUser.SelectedItem.Text));

            var _journalEntry = new ReportDataSource("JournalEntryByDateDS", trialBalanceDT);
            DataHelper.generateAccountingReport(ReportViewer1, "JournalEntryByDate", reportParameters, _journalEntry);
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

            var sql = "JournalEntryByDate";

            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@FromDate", sql_db_type = MySqlDbType.Date, value_name = fromDate });
            procedureList.Add(item: new Procedure() { field_name = "@ToDate", sql_db_type = MySqlDbType.Date, value_name = toDate });
            procedureList.Add(item: new Procedure() { field_name = "@TransactionType", sql_db_type = MySqlDbType.VarChar, value_name = ddTransactionType.SelectedItem.Value });
            procedureList.Add(item: new Procedure() { field_name = "@pUSER", sql_db_type = MySqlDbType.VarChar, value_name = ddUser.SelectedItem.Value });

            DataTable journalEntryDT = db.getProcedureDataTable(sql, procedureList);
            GenerateReport(journalEntryDT);
        }
    }
}