using System;
using System.Web;
using Report.Utils;

namespace Report
{
    public partial class LogInForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.Session["userID"] != null && HttpContext.Current.Session["isSuperAdmin"] != null)
                {
                    Response.Redirect("~/Dashboard");
                }

                txtUsername.Text = "super-admin";
                txtPassword.Text = "Admin@123";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //var cls = new ClsCrypto();
            //var pEncrypt = cls.Encrypt(txtPassword.Text);
            //var ts = new AES();
            var pEncrypt = AES.EncryptAndEncode(txtPassword.Text);

            var user = DataHelper.login(txtUsername.Text, pEncrypt);  //"AQUpvTCF66ztPrYRtLm9ew=="

            if (user.id != 0)
            {
                HttpContext.Current.Session["userID"] = user.id;
                HttpContext.Current.Session["isSuperAdmin"] = user.isSuperAdmin;
                HttpContext.Current.Session["name"] = user.name;
                HttpContext.Current.Session["username"] = user.username;
                Response.Redirect("~/Dashboard");
            }
            else
            {
                lblLogin.Text = "notMatch";
            }
        }
    }
}