<%@ Page Title="Continue Mandate Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RenewReport.aspx.cs" Inherits="Report.Operation.Renew" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="../Scripts/datetimepicker.js" type="text/javascript"></script>
    <div class="panel panel-warning no-margin">
        <div class="panel-body">
            <div class="row">
                <div class="col-sm-3 form-group">
                    <label>Branch:</label>
                    <asp:DropDownList ID="ddBranchName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddBranchName_SelectedIndexChanged" CssClass="form-control input-sm">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddBranchName"
                        ErrorMessage="* Please select branch" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div class="col-sm-2 form-group">
                    <label>From Date: </label> &nbsp;&nbsp;&nbsp;
                    
                     <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkFromDate" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:TextBox ID="dtpFromDate" runat="server" class="form-control input-sm input-inline datepick"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkFromDate" runat="server" Text=" Null" AutoPostBack="true" TextAlign="Right" OnCheckedChanged="chkFromDate_CheckedChanged"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    
                </div>
                <div class="col-sm-2 form-group">
                    <label>System Date:</label>
                    <asp:TextBox ID="dtpSystemDate" runat="server" class="form-control input-sm datepick"></asp:TextBox>
                </div>
                <div class="col-sm-2 form-group">
                    <label>Currency:</label>
                    <asp:DropDownList ID="ddCurrency" runat="server" CssClass="form-control input-sm">
                    </asp:DropDownList>
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
                    <asp:Button ID="btnView" runat="server" Text="View Report" OnClick="btnView_Click" CssClass="btn btn-sm btn-primary" />
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <center>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" nt-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana"  
                WaitMessageFont-Size="14pt" ShowPrintButton="true" ShowBackButton="true" BackColor="#999999" CssClass="printer"  
                PageCountMode="Actual" ShowZoomControl="False"></rsweb:ReportViewer>
            </center>
        </div>
    </div>
</asp:Content>
