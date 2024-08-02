using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;

namespace Report.Accounting
{
    public partial class GLTransactionHistory : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string format = "dd/MM/yyyy";
        public static string fromDate, toDate;
        private string glSelect;
        String[] glCode;
        public string dateFromError = "", dateToError = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                DataHelper.populateCurrencyDDL(ddCurrency);
                dtpFromDate.Text = DataHelper.getSystemDateTextbox();
                dtpToDate.Text = DataHelper.getSystemDateTextbox();
            }
        }

        private void GenerateReport(DataTable glTransactionDT, DataTable openingBalanceDT, DataTable closingBalanceDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("Currency", ddCurrency.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("GlCode", txtGLCode.Text.Trim()));

            var _firstGLTransaction = new ReportDataSource("GLHistDataset", glTransactionDT);
            var _thirdGLTransaction = new ReportDataSource("OpenBalance", openingBalanceDT);
            var _fourdGLTransaction = new ReportDataSource("CloseBalance", closingBalanceDT);
            DataHelper.generateAccountingReport(ReportViewer1, "GLTransactionHistory", reportParameters, _firstGLTransaction, _thirdGLTransaction, _fourdGLTransaction);
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


            var glTransactionSql = "";
            if (ddBranchName.SelectedItem.Value == "ALL")
            {
                glTransactionSql = "SELECT CASE WHEN act.balance_side = 1 THEN act.amount ELSE NULL END AS 'DR', " +
                " CASE WHEN act.balance_side = 2 THEN act.amount ELSE NULL END AS 'CR',jou.is_manual, " +
                " CASE " +
                "     WHEN aac.id = 1 OR aac.id = 5 THEN " +
                "         CASE WHEN act.balance_side = 1 THEN - act.amount ELSE act.amount END " +
                "     ELSE CASE WHEN act.balance_side = 2 THEN act.amount ELSE -act.amount END " +
                " END AS amount, " +
                " jou.trnx_id,act.trx_memo,act.gl_code,coa.gl_name,cur.currency,jou.trnx_ref, " +
                " date(sd.system_date) as sys_date,jou.journal_desc,usr.username,aac.id AS class_type,avd.vendor_name,cus.customer_name,act.balance_side " +
                " FROM acc_transaction act " +
                " LEFT JOIN acc_chat_of_account coa ON act.gl_id = coa.id " +
                " LEFT JOIN acc_account_class aac ON coa.acc_class_id = aac.id " +
                " LEFT JOIN currency cur ON act.currency_id = cur.id " +
                " LEFT JOIN acc_journal jou ON act.jounal_id = jou.id " +
                " LEFT JOIN acc_bill acb ON jou.id = acb.journal_id " +
                " LEFT JOIN acc_vendor avd ON acb.vendor_id = avd.id " +
                " LEFT JOIN contract con ON act.contract_id = con.id " +
                " LEFT JOIN customer cus ON con.customer_id = cus.id " +
                " LEFT JOIN `user` usr ON jou.created_by_id = usr.id " +
                " LEFT JOIN system_date sd ON act.system_date_id = sd.id " +
                " WHERE DATE(act.sys_date) BETWEEN DATE('" + fromDate + "') AND DATE('" + toDate + "') AND jou.trnx_status = 1 AND act.trx_status IN(1, 2) " +
                " AND act.gl_code = '" + txtGLCode.Text.Trim() + "' ORDER BY date(sd.system_date), entry_no; ";
            }
            else
            {
                glTransactionSql = "SELECT CASE WHEN act.balance_side = 1 THEN act.amount ELSE NULL END AS 'DR', " +
                " CASE WHEN act.balance_side = 2 THEN act.amount ELSE NULL END AS 'CR',jou.is_manual, " +
                " CASE " +
                "     WHEN aac.id = 1 OR aac.id = 5 THEN " +
                "         CASE WHEN act.balance_side = 1 THEN - act.amount ELSE act.amount END " +
                "     ELSE CASE WHEN act.balance_side = 2 THEN act.amount ELSE -act.amount END " +
                " END AS amount, " +
                " jou.trnx_id,act.trx_memo,act.gl_code,coa.gl_name,cur.currency,jou.trnx_ref, " +
                " date(sd.system_date) as sys_date,jou.journal_desc,usr.username,aac.id AS class_type,avd.vendor_name,cus.customer_name,act.balance_side " +
                " FROM acc_transaction act " +
                " LEFT JOIN acc_chat_of_account coa ON act.gl_id = coa.id " +
                " LEFT JOIN acc_account_class aac ON coa.acc_class_id = aac.id " +
                " LEFT JOIN currency cur ON act.currency_id = cur.id " +
                " LEFT JOIN acc_journal jou ON act.jounal_id = jou.id " +
                " LEFT JOIN acc_bill acb ON jou.id = acb.journal_id " +
                " LEFT JOIN acc_vendor avd ON acb.vendor_id = avd.id " +
                " LEFT JOIN contract con ON act.contract_id = con.id " +
                " LEFT JOIN customer cus ON con.customer_id = cus.id " +
                " LEFT JOIN `user` usr ON jou.created_by_id = usr.id " +
                " LEFT JOIN system_date sd ON act.system_date_id = sd.id " +
                " WHERE DATE(act.sys_date) BETWEEN DATE('" + fromDate + "') AND DATE('" + toDate + "') AND jou.trnx_status = 1 AND act.branch_id = " + ddBranchName.SelectedItem.Value + " AND act.trx_status IN(1, 2) " +
                " AND act.gl_code = '" + txtGLCode.Text.Trim() + "' ORDER BY date(sd.system_date),entry_no; ";
            }


            var openingBalanceSql = "";
            if (ddBranchName.SelectedItem.Value == "ALL")
            {
                openingBalanceSql = "SELECT ACA.acc_class_id,IFNULL(ABH.balance,0) as open_balance FROM acc_gl_balance_hist ABH LEFT JOIN acc_chat_of_account ACA ON ABH.gl = ACA.gl " +
                " WHERE  DATE(ABH.sys_date) = DATE_ADD('" + fromDate + "', INTERVAL - 1 DAY) AND ABH.gl = '" + txtGLCode.Text.Trim() + "' LIMIT 1; ";
            }
            else
            {
                openingBalanceSql = "SELECT ACA.acc_class_id,IFNULL(ABH.balance,0) as open_balance FROM acc_gl_balance_hist ABH LEFT JOIN acc_chat_of_account ACA ON ABH.gl = ACA.gl " +
                " WHERE ABH.branch_id = " + ddBranchName.SelectedItem.Value + " AND DATE(ABH.sys_date) = DATE_ADD('" + fromDate + "', INTERVAL - 1 DAY) AND ABH.gl = '" + txtGLCode.Text.Trim() + "' LIMIT 1; ";
            }


            var closeBalance = "";
            if (ddBranchName.SelectedItem.Value == "ALL")
            {
                closeBalance = "SELECT IFNULL(balance,0) as close_balance FROM acc_gl_balance_hist WHERE  DATE(sys_date) = DATE('" + fromDate + "') AND gl = '" + txtGLCode.Text.Trim() + "' LIMIT 1;";
            }
            else
            {
                closeBalance = "SELECT IFNULL(balance,0) as close_balance FROM acc_gl_balance_hist WHERE  branch_id = " + ddBranchName.SelectedItem.Value + " AND DATE(sys_date) = DATE('" + fromDate + "') AND gl = '" + txtGLCode.Text.Trim() + "' LIMIT 1;";
            }

            DataTable glTransactionDT = db.getDataTable(glTransactionSql);
            DataTable openingBalanceDT = db.getDataTable(openingBalanceSql);
            DataTable closingBalanceDT = db.getDataTable(closeBalance);
            GenerateReport(glTransactionDT, openingBalanceDT, closingBalanceDT);
        }
    }
}