<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/testBlankScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Start.menu" Codebehind="menu.aspx.cs" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    -------------------------<br />
    メニュー<br />
    -------------------------<br />
    <ul>
        <%-- 業務画面を追加したら、ここにリンクを足す。
             例）<li><a href="<%= this.ResolveUrl("~/Aspx/<業務>/<画面>.aspx") %>">画面名</a></li>
             ※ サンプル／単体テスト画面へのリンクは最小化（minimize）で除去済み。
                リンク切れはビルドでも aspnet_compiler でも検出されない（実行時 404）ので、
                画面を消したら必ずここも掃除する。 --%>
    </ul>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
