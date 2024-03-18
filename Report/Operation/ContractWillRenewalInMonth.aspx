<%@ Page Title="Contract Will Continue Mandate Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ContractWillRenewalInMonth.aspx.cs" Inherits="Report.Operation.ContractWillRenewalInMonth" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="panel panel-default no-margin">
        <div class="panel-body">
            <div class="row">
                <div class="col-sm-3 form-group">
                    <label>Branch: </label>
                    <asp:DropDownList ID="ddBranchName" runat="server" CssClass="form-control input-sm">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddBranchName"
                        ErrorMessage="* Please select branch" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div class="form-group ml16">
                    <div>
                        <label>&nbsp;</label>
                    </div>
                    <asp:Button ID="btnView" runat="server" Text="View Report" CssClass="btn btn-sm btn-primary" OnClick="btnView_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <center>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" nt-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana"  
                WaitMessageFont-Size="14pt" ShowPrintButton="true" ShowBackButton="true" BackColor="#999999" CssClass="printer"  
                PageCountMode="Actual" ShowZoomControl="False" BorderStyle="None"></rsweb:ReportViewer>
            </center>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSContent" runat="server">
</asp:Content>
