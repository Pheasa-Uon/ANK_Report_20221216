using Microsoft.Reporting.WebForms;
using Report.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Report.Utils
{
    public class DataHelper
    {
        public static void checkLoginSession()
        {
            if (HttpContext.Current.Session["userID"] != null && HttpContext.Current.Session["isSuperAdmin"] != null)
            {
                checkPage(Convert.ToInt32(HttpContext.Current.Session["userID"]), Convert.ToBoolean(HttpContext.Current.Session["isSuperAdmin"]));
            }
            else
            {
                var token = HttpContext.Current.Request.QueryString["token"];
                if (token != null)
                {
                    checkLogin(login(token));
                    checkLoginSession();
                }
                else
                {
                    HttpContext.Current.Response.Redirect("~/Login");
                }
            }
        }

        public static int getUserId()
        {
            return Convert.ToInt32(HttpContext.Current.Session["userID"]);
        }
        

        public static void generateReport(ReportViewer reportViewer, string reportName, params ReportDataSource[] reportDataSources)
        {
            reportViewer.SizeToReportContent = true;
            reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(String.Format("~/Reports/{0}.rdlc", reportName));
            reportViewer.LocalReport.DataSources.Clear();

            foreach (var item in reportDataSources)
            {
                reportViewer.LocalReport.DataSources.Add(item);
            }
            reportViewer.LocalReport.Refresh();
        }

        public static void generateReport(ReportViewer reportViewer, string reportName, ReportParameter[] parameters, params ReportDataSource[] reportDataSources)
        {
            reportViewer.SizeToReportContent = true;
            reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(String.Format("~/Reports/{0}.rdlc", reportName));
            reportViewer.LocalReport.DataSources.Clear();

            foreach (var item in reportDataSources)
            {
                reportViewer.LocalReport.DataSources.Add(item);
            }
            reportViewer.LocalReport.SetParameters(parameters);
         
            reportViewer.LocalReport.Refresh();
        }
        

        public static User login(string username, string password) 
        {
            var user = new User();
            var db = new DBConnect();
            var sql = "SELECT U.id, ST.name, U.username, U.email, U.image_path, is_super_admin FROM USER U LEFT JOIN staff_info ST ON U.`staff_id`=ST.id WHERE U.b_status=True AND U.username='" + username + "' AND U.password='" + password + "';";
            var dt = db.getDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["username"].ToString() == username)
                {
                    user.id = Convert.ToInt32(dt.Rows[0]["id"]);
                    user.name = dt.Rows[0]["name"].ToString();
                    user.username = dt.Rows[0]["username"].ToString();
                    user.email = dt.Rows[0]["email"].ToString();
                    user.imagePath = dt.Rows[0]["image_path"].ToString();
                    user.isSuperAdmin = Convert.ToBoolean(dt.Rows[0]["is_super_admin"]);
                }
            }
            return user;
        }

        public static User login(string token)
        {
            var user = new User();
            var db = new DBConnect();
            var sql = "SELECT U.id, ST.name, U.username, U.email, U.image_path, is_super_admin FROM USER U LEFT JOIN staff_info ST ON U.`staff_id`=ST.id WHERE U.b_status=True AND U.login_token='" + token + "';";
            var dt = db.getDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                user.id = Convert.ToInt32(dt.Rows[0]["id"]);
                user.name = dt.Rows[0]["name"].ToString();
                user.username = dt.Rows[0]["username"].ToString();
                user.email = dt.Rows[0]["email"].ToString();
                user.imagePath = dt.Rows[0]["image_path"].ToString();
                user.isSuperAdmin = Convert.ToBoolean(dt.Rows[0]["is_super_admin"]);
            }
            return user;
        }

        public static void checkLogin(User user)
        {
            if (user.id != 0)
            {
                HttpContext.Current.Session["userID"] = user.id;
                HttpContext.Current.Session["isSuperAdmin"] = user.isSuperAdmin;
                HttpContext.Current.Session["name"] = user.name;
                HttpContext.Current.Session["username"] = user.username;
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/Login");
            }
        }

        public static List<ReportItem> getMenuItem()
        {
            var lsRp = new List<ReportItem>();
            if (HttpContext.Current.Session["menuItem"] != null)
            {
                lsRp = (List<ReportItem>)HttpContext.Current.Session["menuItem"];
            }
            else
            {
                var sql = "SELECT DISTINCT ri.id, ri.routing, ri.menu_order, ri.title, ri.parent_id " +
                    "FROM report_item ri WHERE ri.disabled = False order by menu_order;";

                var isSuperAdmin = Convert.ToBoolean(HttpContext.Current.Session["isSuperAdmin"]);
                if(isSuperAdmin == false)
                {
                     sql = "SELECT DISTINCT ri.id, ri.routing, ri.menu_order, ri.title, ri.parent_id " +
                    "FROM user_role u INNER JOIN role_report rr ON u.role_id = rr.role_id " +
                    "INNER JOIN report_item ri ON rr.report_item_id = ri.id WHERE u.user_id = " + 
                    HttpContext.Current.Session["userID"] + " AND ri.disabled = False order by menu_order;";
                }
                
                var db = new DBConnect();
                var dt = db.getDataTable(sql);
                var subRp = new List<SubItem>();
                foreach (DataRow item in dt.Rows)
                {
                    if (item["parent_id"].ToString() == "")
                    {
                        lsRp.Add(new ReportItem
                        {
                            id = Convert.ToInt32(item["id"]),
                            routing = item["routing"].ToString(),
                            title = item["title"].ToString()
                        });
                    }
                    else
                    {
                        var rp = new SubItem
                        {
                            id = Convert.ToInt32(item["id"]),
                            routing = item["routing"].ToString(),
                            title = item["title"].ToString(),
                            parent_id = Convert.ToInt32(item["parent_id"])
                        };
                        lsRp.Find(x => x.id == rp.parent_id).items.Add(rp);
                        subRp.Add(rp);
                    }
                }
                HttpContext.Current.Session["menuItem"] = lsRp;
                HttpContext.Current.Session["subMenuItem"] = subRp;
            }
            return lsRp;
        }
        
        //Check Page Statement
        public static void checkPage(int userId, bool isSuperAdmin)
        {
            string urlPath = HttpContext.Current.Request.Url.AbsolutePath.Replace(".aspx", "");

            if (isSuperAdmin == true)
            {
                return;
            }
            else
            {
                if (urlPath.ToLower() == "/dashboard" || urlPath == "")
                {
                    return;
                }
                else
                {
                    var list = (List<SubItem>)HttpContext.Current.Session["subMenuItem"];
                    if (list == null)
                    {
                        getMenuItem();
                        var list1 = (List<SubItem>)HttpContext.Current.Session["subMenuItem"];
                        if (list1.Count <= 0)
                        {
                            HttpContext.Current.Response.Redirect("~/Login");
                        }
                        else
                        {
                            if (list1.Any(x => x.routing.ToLower() == urlPath.ToLower()))
                            {
                                return;
                            }
                            else
                            {
                                HttpContext.Current.Response.Redirect("~/Login");
                            }
                        }
                    }
                    else
                    {
                        if (list.Any(x => x.routing.ToLower() == urlPath.ToLower().Replace(".aspx", "")))
                        {
                            return;
                        }
                        else
                        {
                            HttpContext.Current.Session["userID"] = null;
                            HttpContext.Current.Response.Redirect("~/Login");
                        }
                    }
                }
            }
        }

        //Get Branch List Statement
        public static List<Branch> getBranch(Int32 userId)
        {
            if (HttpContext.Current.Session["branch_" + userId] != null)
            {
                var branch = HttpContext.Current.Session["branch_" + userId];
                var list = ((IEnumerable)branch).Cast<Branch>().ToList();
                return list;
            }
            else
            {
                DBConnect db = new DBConnect();
                return db.GetBranchNames(Convert.ToInt32(HttpContext.Current.Session["userID"]));
            }
        }

        //Get Routing List Statement
        public static List<string> getRouting()
        {
            List<string> result = new List<string>();
            List<MenuField> operationList = getOperationMenu();
            List<MenuField> accountingList = getAccountingMenu();
            List<MenuField> mergeList = new List<MenuField>();

            var resultList = mergeList.Concat(operationList).Concat(accountingList).ToList();
            foreach (var item in resultList)
            {
                result.Add(item.routing);
            }
            return result;
        }

        //Check URL Statement
        public static void getURL(string urlPath, List<string> path)
        {
            string subStringPath = urlPath.Substring(1);
            if (subStringPath == "Dashboard")
            {
                return;
            }
            path = DataHelper.getRouting();
            var result = path.Any(s => s.ToLower() == subStringPath.ToLower());

            if (!result)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Response.Redirect("/Login", true);
            }
        }

        //Get Operation Menu Type List Statement
        public static List<MenuField> getOperationMenu()
        {
            if (HttpContext.Current.Session["opertion_menu"] != null)
            {
                var operation = HttpContext.Current.Session["operation_menu"];
                var list = ((IEnumerable)operation).Cast<MenuField>().ToList();
                return list;
            }
            else
            {
                DBConnect db = new DBConnect();
                return db.GetMenuItems(Convert.ToInt32(HttpContext.Current.Session["userID"]), 3);
            }
        }

        //Get Accounting Menu Type List Statement
        public static List<MenuField> getAccountingMenu()
        {
            if (HttpContext.Current.Session["accounting_menu"] != null)
            {
                var accounting = HttpContext.Current.Session["accounting_menu"];
                var list = ((IEnumerable)accounting).Cast<MenuField>().ToList();
                return list;
            }
            else
            {
                DBConnect db = new DBConnect();
                return db.GetMenuItems(Convert.ToInt32(HttpContext.Current.Session["user_id"]), 15);
            }
        }

        //Populate Operation Menu Type List To Dropdown List Statement
        public static void populateOperationMenuDDL(DropDownList ddl)
        {
            DBConnect db = new DBConnect();
            var operationList = getOperationMenu();
            ddl.DataTextField = "title";
            ddl.DataValueField = "routing";
            ddl.DataSource = operationList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- Select a Value ---", "0"));
            ddl.SelectedValue = "0";
        }

        //Populate Accounting Menu Type List To Dropdown List Statement
        public static void populateAccountingMenuDDL(DropDownList ddl)
        {
            DBConnect db = new DBConnect();
            var operationList = getAccountingMenu();
            ddl.DataTextField = "title";
            ddl.DataValueField = "routing";
            ddl.DataSource = operationList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- Select a Value ---", "0"));
            ddl.SelectedValue = "0";
        }

        public static void GetUserNamesAll(DropDownList ddl)
        {
            DBConnect db = new DBConnect();
            var operationList = db.GetUserNamesAll();
            ddl.DataTextField = "name";
            ddl.DataValueField = "id";
            ddl.DataSource = operationList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- ALL ---", "ALL"));
            ddl.SelectedValue = "ALL";
        }

        public static void GetUserNames(DropDownList ddl, int branchId)
        {
            DBConnect db = new DBConnect();
            var operationList = db.GetUserNames(branchId);
            ddl.DataTextField = "name";
            ddl.DataValueField = "id";
            ddl.DataSource = operationList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- ALL ---", "ALL"));
            ddl.SelectedValue = "ALL";
        }

        //Check And Get System Date Statement
        public static String getSystemDateStr(string format = "dd-MMM-yyyy")
        {
            if (HttpContext.Current.Session["system_date"] != null)
            {
                DateTime sysDate = Convert.ToDateTime(HttpContext.Current.Session["system_date"].ToString());
                return sysDate.ToString(format);
            }
            else
            {
                DBConnect db = new DBConnect();
                var res = db.GetSystemDate();
                DateTime sysDate = Convert.ToDateTime(res);
                return sysDate.ToString(format);
            }
        }

        public static DateTime getSystemDate()
        {
            if (HttpContext.Current.Session["system_date"] != null)
            {
                return Convert.ToDateTime(HttpContext.Current.Session["system_date"].ToString());
            }
            else
            {
                DBConnect db = new DBConnect();
                var res = db.GetSystemDate();
                return Convert.ToDateTime(res);
            }
        }

        public static string getSystemDateTextbox()
        {
            if (HttpContext.Current.Session["system_date"] != null)
            {
                DateTime sysDate = Convert.ToDateTime(HttpContext.Current.Session["system_date"].ToString());
                return sysDate.ToString("dd/MM/yyyy");
            }
            else
            {
                DBConnect db = new DBConnect();
                var res = db.GetSystemDate();
                DateTime sysDate = Convert.ToDateTime(res);
                return sysDate.ToString("dd/MM/yyyy");
            }
        }

        //Populate Officer List To Dropdown List Statement
        public static void populateOfficerDDL(DropDownList ddl, int branchId)
        {
            DBConnect db = new DBConnect();
            var officerList = db.GetOfficerNames(branchId);
            ddl.DataTextField = "name";
            ddl.DataValueField = "id";
            ddl.DataSource = officerList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- All ---", "0"));
            ddl.SelectedIndex = 0;
        }

        public static void populateOfficerDDLAll (DropDownList ddl)
        {
            DBConnect db = new DBConnect();
            var officerList = db.GetOfficerNamesAll();
            ddl.DataTextField = "name";
            ddl.DataValueField = "id";
            ddl.DataSource = officerList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- All ---", "0"));
            ddl.SelectedIndex = 0;
        }

        //Populate CO Productivity Officer List To Dropdown List Statement
        public static void populateCOProductivityOfficerDDL(DropDownList ddl, int branchId)
        {
            DBConnect db = new DBConnect();
            var officerList = db.GetOfficerNames(branchId);
            ddl.DataTextField = "name";
            ddl.DataValueField = "id";
            ddl.DataSource = officerList;
            ddl.DataBind();
            ddl.SelectedIndex = 0;
        }

        //Populate GL List To Dropdown List Statement
        public static void populateGLDDL(DropDownList ddl, int branchId)
        {
            DBConnect db = new DBConnect();
            var glList = db.GetGL(branchId);
            ddl.DataTextField = "gl_name";
            ddl.DataValueField = "id";
            ddl.DataSource = glList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- Select a Value ---", ""));
            ddl.SelectedIndex = 0;
        }

        //Populate Branch List To Dropdown List Statement
        public static void populateBranchDDL(DropDownList ddl, Int32 token)
        {
            var branchList = getBranch(token);
            ddl.DataTextField = "branch_name";
            ddl.DataValueField = "id";
            ddl.DataSource = branchList;
            ddl.DataBind();
            if (branchList.Count >= 1)
            {
                // ddl.Items.Insert(0, new ListItem("--- Select a Value ---", ""));
                ddl.Items.Insert(0, new ListItem("-- ALL --", "ALL"));
            }
        }

        //Populate Balance Sheet Branch List To Dropdown List Statement
        public static void populateBranchDDLAllowAll(DropDownList ddl, Int32 token)
        {
            var branchList = getBranch(token);
            ddl.DataTextField = "branch_name";
            ddl.DataValueField = "id";
            ddl.DataSource = branchList;
            ddl.DataBind();
            if (branchList.Count > 1)
            {
                ddl.Items.Insert(0, new ListItem("-- ALL --", "ALL"));
            }
           
        }

        //Populate Transaction Type List To Dropdown List Statement
        public static void populateTransactionTypeDDL(DropDownList ddl)
        {
            DBConnect db = new DBConnect();
            var transactionTypeList = db.GetTransactionType();
            ddl.DataTextField = "transaction_type";
            ddl.DataValueField = "id";
            ddl.DataSource = transactionTypeList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--- All ---", "0"));
            ddl.SelectedIndex = 0;
        }

        //Populate Currency List To Dropdown List Statement
        public static List<Currency> populateCurrencyDDL(DropDownList ddl)
        {
            DBConnect db = new DBConnect();
            var currencyList = db.GetCurrency();
            ddl.DataTextField = "currency";
            ddl.DataValueField = "id";
            ddl.DataSource = currencyList;
            ddl.DataBind();
            ddl.SelectedIndex = 1;
            return currencyList;
        }

        //Generate Operation Report Type Statement
        public static void generateOperationReport(ReportViewer reportViewer, string reportName, ReportParameterCollection reportParameterCollection, params ReportDataSource[] reportDataSources)
        {
            reportViewer.SizeToReportContent = true;
            reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(String.Format("~/Operation/{0}.rdlc", reportName));
            reportViewer.LocalReport.DataSources.Clear();
            //Add Default Parameter CompanyName
            reportParameterCollection.Add(new ReportParameter("CompanyName", DataHelper.getCompanyName()));
            reportViewer.LocalReport.SetParameters(reportParameterCollection);

            foreach (var item in reportDataSources)
            {
                reportViewer.LocalReport.DataSources.Add(item);
            }
            reportViewer.LocalReport.Refresh();
        }

        public static void generateOperationReport(ReportViewer reportViewer, string reportName, params ReportDataSource[] reportDataSources)
        {
            reportViewer.SizeToReportContent = true;
            reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(String.Format("~/Operation/{0}.rdlc", reportName));
            reportViewer.LocalReport.DataSources.Clear();
            
            foreach (var item in reportDataSources)
            {
                reportViewer.LocalReport.DataSources.Add(item);
            }
            reportViewer.LocalReport.Refresh();
        }

        //Generate Accounting Report Type Statement
        public static void generateAccountingReport(ReportViewer reportViewer, string reportName, Microsoft.Reporting.WebForms.ReportParameterCollection reportParameterCollection, params ReportDataSource[] reportDataSources)
        {
            reportViewer.SizeToReportContent = true;
            reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(String.Format("~/Accounting/{0}.rdlc", reportName));
            reportViewer.LocalReport.DataSources.Clear();
            //Add Default Parameter CompanyName
            reportParameterCollection.Add(new ReportParameter("CompanyName", DataHelper.getCompanyName()));
            reportViewer.LocalReport.SetParameters(reportParameterCollection);

            foreach (var item in reportDataSources)
            {
                reportViewer.LocalReport.DataSources.Add(item);
            }
            reportViewer.LocalReport.Refresh();
        }

        public static String getCompanyName()
        {
            if (HttpContext.Current.Session["company_name"] != null)
            {
                return HttpContext.Current.Session["company_name"].ToString();
            }
            else
            {
                DBConnect db = new DBConnect();
                var res = db.GetCompanyName();
                HttpContext.Current.Session["company_name"] = res;
                return res;
            }
        }
    }
}   