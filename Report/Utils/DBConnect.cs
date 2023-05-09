using MySql.Data.MySqlClient;
using Report.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Report.Utils
{
    public class DBConnect
    {
        private MySqlConnection connection;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize Values
        private void Initialize()
        {
            var constr = System.Configuration.ConfigurationManager.ConnectionStrings["p_dbConnectionString"].ConnectionString;
            connection = new MySqlConnection(constr);
        }

        //Open Connection To Database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close Connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }

        public User getToken(string login_token)
        {
            string query = "SELECT id,is_super_admin FROM user where login_token='" + login_token + "' AND b_status=true limit 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                var user = new User();
                user.id = Convert.ToInt32(dt.Rows[0]["id"]);
                user.isSuperAdmin = Convert.ToBoolean(dt.Rows[0]["is_super_admin"]);
                HttpContext.Current.Session["id_" + login_token] = user.id;
                HttpContext.Current.Session["isSuperAdmin_" + login_token] = user.isSuperAdmin;
                HttpContext.Current.Session["branch"] = GetBranchNames(user.id);
                return user;
            }
            else
            {
                return null;
            }
        }

        //Get Currency Statement
        public DataTable getProcedureDataTable(string procedureName, List<Procedure> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(procedureName, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
                foreach (Procedure procedure in parameters)
                {
                    cmd.Parameters.Add(procedure.field_name, procedure.sql_db_type).Value = procedure.value_name; 
                }
                cmd.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand = cmd;
                adapter.Fill(dt);
            return dt;
        }

        //Get Currency Statement
        public List<Currency> GetCurrency()
        {
            string query = "SELECT id,currency,currency_label FROM currency WHERE currency_status=True;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new Currency()
            {
                id = Convert.ToInt32(r["id"]),
                currency = (string)r["currency"],
                currency_label = (string)r["currency_label"]
            }).ToList();

            return list;
        }

        //Get System Date Statement
        public string GetSystemDate()
        {
            string system_date = "";
            string query = "SELECT system_date FROM system_date where is_active = 1";
            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                system_date = cmd.ExecuteScalar().ToString();
                CloseConnection();
            }
            return system_date;
        }

        //Get Company name
        public string GetCompanyName()
        {
            string companyName = "";
            string query = "SELECT company_name FROM general_setting ORDER BY id LIMIT 1;";
            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                companyName = cmd.ExecuteScalar().ToString();
                CloseConnection();
            }
            return companyName;
        }

        //Get Max Date Statement
        public String GetMaxDate(string system_date, string branch_id)
        {
            DateTime max_date = new DateTime();
            string format = "dd/MM/yyyy";
            DateTime current_date = DateTime.Now;
            string systemDateFormat = DateTime.ParseExact(system_date, format, null).ToString("yyyy-MM-dd");
            string query = "SELECT due_date as max_due_date FROM acc_transaction ACT "+
                           "LEFT JOIN schedule SC ON ACT.schedule_id = SC.id " +
                           "LEFT JOIN contract_gl_product CGP ON ACT.contract_id = CGP.contract_id AND RIGHT(ACT.gl_code,8) = CGP.gl " +
                           "WHERE '" + systemDateFormat + "' BETWEEN SC.created_date AND SC.due_date AND ACT.branch_id = " + branch_id +
                           " ORDER BY due_date DESC LIMIT 1";
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    max_date = (DateTime)obj;
                }
                else
                {
                    max_date = current_date;
                }
                this.CloseConnection();
            }
            return max_date.ToString("dd/MM/yyyy");
        }

        //Get Branch Name Statement
        public List<Branch> GetBranchNames(int user_id)
        {
            string query = "SELECT id, branch_name FROM branch WHERE id IN (SELECT branch_id FROM user_branch WHERE user_id=" + user_id + ")";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new Branch()
            {
                id = Convert.ToInt32(r["id"]),
                branch_name = (string) r["branch_name"]
            }).ToList();

            return list;
        }
        
        //Get Branch Name Statement
        public List<MenuField> GetMenuItems(int user_id, int parent_id)
        {
            string query = "SELECT DISTINCT u.user_id, ri.routing, ri.menu_order, ri.title from user_role u inner join role_report rr on u.role_id = rr.role_id inner join report_item ri on rr.report_item_id = ri.id where u.user_id = " + user_id + " and u.b_status = true and ri.parent_id = " + parent_id ;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new MenuField()
            {
                menu_order = Convert.ToInt32(r["menu_order"]),
                routing = (string) r["routing"],
                title = (string) r["title"]
            }).ToList();

            return list;
        }

        //Get Page URL Statement
        public String getPages(int user_id, string routing)
        {
            string page = "";
            string query = "SELECT DISTINCT ri.routing from user_role ur inner join role_report rr on ur.role_id = rr.role_id inner join report_item ri on ri.id = rr.report_item_id where ur.user_id = " + user_id + " and ri.routing =  '" + routing + "'";
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                page = cmd.ExecuteScalar().ToString();
                this.CloseConnection();
                return page;
            }
            else
            {
                return null;
            }
        }

        //Get Officer Statement
        public List<Officer> GetOfficerNames(int branch_id)
        {
            string query = "SELECT id, name FROM staff_info WHERE b_status=1 and branch_id =" + branch_id;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new Officer()
            {
                id = Convert.ToInt32(r["id"]),
                name = (string)r["name"]
            }).ToList();

            return list;
        }

        //Get Officer Statement
        public List<Officer> GetOfficerNamesAll()
        {
            string query = "SELECT id, name FROM staff_info WHERE b_status=1;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new Officer()
            {
                id = Convert.ToInt32(r["id"]),
                name = (string)r["name"]
            }).ToList();

            return list;
        }
        //Get User Statement
        public List<User> GetUserNames(int branch_id)
        {
            string query = "select u.id, u.username from `user` u inner join staff_info st on u.staff_id = st.id where u.b_status = '1' and st.branch_id = " + branch_id;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new User()
            {
                id = Convert.ToInt32(r["id"]),
                name = (string)r["username"]
            }).ToList();

            return list;
        }

        //Get User Statement
        public List<User> GetUserNamesAll()
        {
            string query = "select u.id, u.username from `user` u inner join staff_info st on u.staff_id = st.id where u.b_status = '1' ;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new User()
            {
                id = Convert.ToInt32(r["id"]),
                name = (string)r["username"]
            }).ToList();

            return list;
        }


        //Get GL Statement
        public List<GL> GetGL(int branch_id)
        {
            string query = "SELECT id, gl, gl_name from acc_chat_of_account where branch_id =" + branch_id;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new GL()
            {
                id = Convert.ToInt32(r["id"]),
                gl_name = (string)r["gl"] +" - "+ (string)r["gl_name"] 
            }).ToList();

            return list;
        }
        
        //Get Transaction Type Statement
        public List<TransactionType> GetTransactionType()
        {
            string query = "select id, transaction_type FROM acc_transaction_type where b_status = 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            var list = dt.AsEnumerable().Select(r => new TransactionType()
            {
                id = Convert.ToInt32(r["id"]),
                transaction_type = (string)r["transaction_type"]
            }).ToList();

            return list;
        }

        //Get Datatable Statement
        public DataTable getDataTable(string sql)
        {
            DataTable dt = new DataTable();
            MySqlCommand command = new MySqlCommand(sql, connection);
            var adapter = new MySqlDataAdapter(command);
            adapter.Fill(dt);
            return dt;
        }
    }
}