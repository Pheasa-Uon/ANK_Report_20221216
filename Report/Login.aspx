<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Report.LogInForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Log in - Pawn Shop Reporting</title>
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/override.css" />
    <link href="~/Content/login.css" rel="stylesheet" />
</head>
<body>
    <div class="login-wrapper">
        <form id="form1" runat="server">

            <asp:Label ID="lblLogin" runat="server" Text="" Visible="False"></asp:Label>
            <div class="login-form">
                <div class="login-logo">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/midas.png" />
                </div>
                <p>Please enter your credential</p>
                <div class="form-group">
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Username"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="txtUsername" CssClass="text-danger" Text="Please enter username" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Password" TextMode="Password"></asp:TextBox>
                    <%--<asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Password"></asp:TextBox>--%>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="txtPassword" CssClass="text-danger" Text="Please enter password" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <%if (Page.IsPostBack)
                        {
                            if (lblLogin.Text == "notMatch")
                            {%>
                    <span class="text-danger">Username or password not match!</span>
                    <%}
                        }
                    %>
                </div>
                <div class="form-group">
                    <asp:Button ID="Button1" runat="server" Text="Log in" CssClass="btn btn-primary btn-block" OnClick="Button1_Click" />
                </div>
            </div>
        </form>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".alert").delay(5000).hide(300);
        })
    </script>
</body>
</html>
