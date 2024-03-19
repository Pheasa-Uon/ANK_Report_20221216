<%@ Page Title="Contract Will Continue Mandate in month Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ContractWillRenewalInMonth.aspx.cs" Inherits="Report.Operation.ContractWillRenewalInMonth" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="../Scripts/datetimepicker.js" type="text/javascript"></script>
    <div class="panel panel-warning no-margin">
        <div class="panel-body">
            <div class="row">
            <div class="row">
                <div class="col-sm-3 form-group">
                    <label>Branch:</label> 
                    <asp:DropDownList ID="ddBranchName" runat="server" AutoPostBack="true" CssClass="form-control input-sm">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddBranchName"
                        ErrorMessage="* Please select branch" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div class="col-sm-2 form-group">
                    <label>From Date:</label>
                    <asp:TextBox ID="dtpFromDate" runat="server" class="form-control input-sm datepick"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="dtpFromDate"
                        ErrorMessage="* Please select date" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="reqValidate" runat="server" ControlToValidate="dtpFromDate"
                        ErrorMessage="* Wrong date formate" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic"
                        ValidationExpression="^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"></asp:RegularExpressionValidator>
                    <span style="color: red"><%=dateFromError %></span>
                </div>
                <div class="col-sm-2 form-group">
                    <label>To Date:</label>
                    <asp:TextBox ID="dtpToDate" runat="server" class="form-control input-sm datepick"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="dtpToDate"
                        ErrorMessage="* Please select date" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="dtpToDate"
                        ErrorMessage="* Wrong date formate" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic"
                        ValidationExpression="^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"></asp:RegularExpressionValidator>
                    <span style="color: red"><%=dateToError %></span>
                </div>
                <div class="col-sm-2 form-group">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddBranchName" />
                        </Triggers>
                        <ContentTemplate>
                            <label>Pawn Officer:</label>
                            <asp:DropDownList ID="ddOfficer" runat="server" CssClass="form-control input-sm" Enabled="false">
                            </asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                
                <div class="form-group ml16">
                    <div>
                        <label>&nbsp;</label>
                    </div>
                    <asp:Button ID="btnView" runat="server" Text="View Report" CssClass="btn btn-sm btn-primary" OnClick="btnView_Click"/>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <center>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" nt-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana"  
                WaitMessageFont-Size="14pt" ShowPrintButton="true" ShowBackButton="true" BackColor="#999999" CssClass="printer"  
                PageCountMode="Actual" ShowZoomControl="False" SizeToReportContent="True">
                </rsweb:ReportViewer>
            </center>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSContent" runat="server">
</asp:Content>