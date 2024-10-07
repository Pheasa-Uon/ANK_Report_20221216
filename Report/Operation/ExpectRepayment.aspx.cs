using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;

namespace Report.Operation
{
    public partial class CollectionSheet : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        DateTime currentDate = DateTime.Today;
        private static string fromDate, toDate;
        public string format = "dd/MM/yyyy";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                fromDate = dtpFromDate.Text;
                toDate = dtpToDate.Text;
                var sysDate = DataHelper.getSystemDate();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateOfficer();
                dtpFromDate.Text = sysDate.ToString(format);
                dtpToDate.Text = sysDate.ToString(format);
            }
        }

        private void GenerateReport(DataTable collectionSheetDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("PawnOfficer", ddOfficer.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));

            var _collectionSheetlist = new ReportDataSource("CollectionSheetDS", collectionSheetDT);
            DataHelper.generateOperationReport(ReportViewer1, "ExpectRepayment", reportParameters, _collectionSheetlist);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {   
            var fromDateSql = DateTime.ParseExact(dtpFromDate.Text, format, null);
            var fromDay = fromDateSql.ToString("yyyy-MM-dd");

            var toDateSql = DateTime.ParseExact(dtpToDate.Text, format, null);
            var toDay = toDateSql.ToString("yyyy-MM-dd");

            var sql = "";
            if (ddBranchName.SelectedItem.Value == "ALL")
            {
                sql = "	SELECT ST.ticket_no,CUS.customer_name,CUS.personal_phone, 		" +
                        "   CUR.currency,PL.principle_less princ_outstanding, ST.due_date, 	" +
                        "   C.disbursement_date,C.pawn_price,C.expired_date, " +
                        "   ST.interest_less,ST.principle_less,ST.penalty_less, 	" +
                        "   P.lob_name,SI.name CO_Name, 							" +
                        "   case when ticket_status ='P' then 'Paid' 				" +
                        "	when ticket_status ='A' then 'Active' 				" +
                        "   when ticket_status ='I' then 'Inactive' 			" +
                        "   when ticket_status ='PPP' then 'Partial Prepaid'  	" +
                        "   when ticket_status ='FPP' then 'Full Prepaid'  	" +
                        "   else 'Partial Paid' end ticket_status 				" +
                        " FROM schedule_ticket ST 								" +
                        " LEFT JOIN contract C ON ST.contract_id = C.id 			" +
                        " LEFT JOIN customer CUS ON C.customer_id = CUS.id 		" +
                        " LEFT JOIN currency CUR ON C.currency_id = CUR.id 		" +
                        " LEFT JOIN product P ON C.product_id = P.id 			" +
                        " LEFT JOIN staff_info SI ON C.pawn_officer_id = SI.id 	" +
                        " LEFT JOIN(SELECT contract_id, SUM(principle_less) principle_less  		" +
                        " FROM schedule_ticket 									" +
                        " GROUP BY contract_id) PL ON C.id = PL.contract_id 		" +
                        " WHERE  DATE(ST.due_date) BETWEEN DATE('" + fromDay + "') AND DATE(' " + toDay + " ') 		" +
                        " AND C.contract_status IN(4, 7) AND c.`b_status`= TRUE	" +
                        " order by due_date ";
            }
            else
            {
                sql = "	SELECT ST.ticket_no,CUS.customer_name,CUS.personal_phone, 		" +
                        "   CUR.currency,PL.principle_less princ_outstanding, ST.due_date, 	" +
                        "   C.disbursement_date,C.pawn_price,C.expired_date, " +
                        "   ST.interest_less,ST.principle_less,ST.penalty_less, 	" +
                        "   P.lob_name,SI.name CO_Name, 							" +
                        "   case when ticket_status ='P' then 'Paid' 				" +
                        "	when ticket_status ='A' then 'Active' 				" +
                        "   when ticket_status ='I' then 'Inactive' 			" +
                        "   when ticket_status ='PPP' then 'Partial Prepaid'  	" +
                        "   when ticket_status ='FPP' then 'Full Prepaid'  	" +
                        "   else 'Partial Paid' end ticket_status 				" +
                        " FROM schedule_ticket ST 								" +
                        " LEFT JOIN contract C ON ST.contract_id = C.id 			" +
                        " LEFT JOIN customer CUS ON C.customer_id = CUS.id 		" +
                        " LEFT JOIN currency CUR ON C.currency_id = CUR.id 		" +
                        " LEFT JOIN product P ON C.product_id = P.id 			" +
                        " LEFT JOIN staff_info SI ON C.pawn_officer_id = SI.id 	" +
                        " LEFT JOIN(SELECT contract_id, SUM(principle_less) principle_less  		" +
                        " FROM schedule_ticket 									" +
                        " WHERE branch_id = " + ddBranchName.SelectedItem.Value +
                        " GROUP BY contract_id) PL ON C.id = PL.contract_id 		" +
                        " WHERE  DATE(ST.due_date) BETWEEN DATE('" + fromDay + "') AND DATE(' " + toDay + " ') 		" +
                        " AND C.branch_id = " + ddBranchName.SelectedItem.Value +
                        " AND C.contract_status IN(4, 7) AND c.`b_status`= TRUE	" +
                        " order by due_date ";
            }
            if (ddOfficer.SelectedItem.Value != "0")
            {
                sql += " AND C.pawn_officer_id=" + ddOfficer.SelectedItem.Value + ";"; 
            }
            
            var collectionSheetDT = db.getDataTable(sql);
            GenerateReport(collectionSheetDT);
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