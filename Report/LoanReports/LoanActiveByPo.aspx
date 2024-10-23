<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LoanActiveByPo.aspx.cs" Inherits="Report.LoanReports.LoanActiveByPo" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="panel panel-default no-margin">
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
                  <div class="col-sm-3 form-group">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddBranchName" />
                        </Triggers>
                        <ContentTemplate>
                            <label>Po Name:</label>
                            <asp:DropDownList ID="ddOfficer" runat="server" OnSelectedIndexChanged ="ddOffcer_SelectedIndexChanged" CssClass="form-control input-sm" Enabled="false">
                            </asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                 <div class="col-sm-2 form-group">
                     <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                         <Triggers>
                             <asp:AsyncPostBackTrigger ControlID="ddBranchName" />
                         </Triggers>
                         <ContentTemplate>
                             <label>Customer Name:</label>
                             <asp:DropDownList ID="ddCustomer" runat="server" CssClass="form-control input-sm" Enabled="false">
                             </asp:DropDownList>
                         </ContentTemplate>
                     </asp:UpdatePanel>
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
