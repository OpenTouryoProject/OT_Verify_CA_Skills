<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/businessScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Suppliers.SuppliersB" Codebehind="SuppliersB.aspx.cs" EnableEventValidation="false" %>

<%-- グリッド中の動的コマンド ボタン（削除）を使うので EnableEventValidation="false"。 --%>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <h4>Suppliers 画面Ｂ</h4>
    <p>
        ［一覧取得］で一覧を表示し、セルを直接編集できます。<br />
        グリッド内の［削除］で行削除、グリッド外の［行追加］で行追加、［更新］でまとめてDBへ反映します。
    </p>

    <%-- グリッド外の［追加］ボタン（フッタ部ではない）。接頭辞 btn で自動結線される --%>
    <div style="margin-bottom: 8px;">
        <asp:Button ID="btnAddRow" runat="server" CssClass="btn btn-success btn-sm" Text="行追加" />
    </div>

    <%-- 一覧は GridView（DataSource にバインド）。
         ★ グリッド内のコントロールには自動結線の接頭辞（txt / btn 等）を付けない
            （付けると行ごとに TextChanged / Click が不要に結線される）。 --%>
    <asp:GridView ID="gvwSuppliers" runat="server" AutoGenerateColumns="False"
                  CssClass="table table-sm table-bordered" DataKeyNames="SupplierID">
        <Columns>
            <asp:TemplateField HeaderText="SupplierID">
                <ItemTemplate>
                    <asp:Label ID="lblSupplierId" runat="server" Text='<%# Eval("SupplierID") %>' />
                    <asp:HiddenField ID="hfSupplierId" runat="server" Value='<%# Eval("SupplierID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CompanyName">
                <ItemTemplate>
                    <asp:TextBox ID="tbCompanyName" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("CompanyName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ContactName">
                <ItemTemplate>
                    <asp:TextBox ID="tbContactName" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("ContactName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ContactTitle">
                <ItemTemplate>
                    <asp:TextBox ID="tbContactTitle" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("ContactTitle") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="City">
                <ItemTemplate>
                    <asp:TextBox ID="tbCity" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("City") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Country">
                <ItemTemplate>
                    <asp:TextBox ID="tbCountry" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Country") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Phone">
                <ItemTemplate>
                    <asp:TextBox ID="tbPhone" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Phone") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="操作">
                <ItemTemplate>
                    <%-- グリッド中の削除ボタン（RowDeleting で受ける） --%>
                    <asp:LinkButton ID="lbDelete" runat="server" CssClass="btn btn-danger btn-sm"
                                    CommandName="Delete" Text="削除" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
