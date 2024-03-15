<%@ Page Title="Loan Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AllContractList.aspx.cs" Inherits="Report.Operation.AllContractList1" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="panel panel-default no-margin">
        <div class="panel-body">
            <div class="row">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="col-sm-3 form-group">
                            <label>Branch: </label>
                            <asp:DropDownList ID="ddBranchName" runat="server" CssClass="form-control input-sm" AutoPostBack="true" OnSelectedIndexChanged="ddBranchName_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddBranchName"
                                ErrorMessage="* Please select branch" ForeColor="Red" Font-Names="Tahoma" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-sm-3 form-group">
                            <label>Pawn Officer:</label>
                            <asp:DropDownList ID="ddOfficer" runat="server" CssClass="form-control input-sm">
                            </asp:DropDownList>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="col-sm-2 form-group">
                    <label>Contract Status:</label>
                    <asp:DropDownList ID="ddContractStatus" runat="server" CssClass="form-control input-sm">
                        <asp:ListItem Value="0" Text="--- All ---" />
                        <asp:ListItem Selected="True" Value="4" Text="Active" />
                        <asp:ListItem Value="6" Text="Redeem" />
                    </asp:DropDownList>
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
