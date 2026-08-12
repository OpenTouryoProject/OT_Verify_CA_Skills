<%@ Page Language="C#" MasterPageFile="~/Aspx/Common/Master/businessScreen.master" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Suppliers.SuppliersA" Codebehind="SuppliersA.aspx.cs" %>

<asp:Content ID="cphHeaderScripts" ContentPlaceHolderID="cphHeaderScripts" Runat="Server">
    <!-- Head 部の ContentPlaceHolder -->
</asp:Content>

<asp:Content ID="ContentPlaceHolder_A" ContentPlaceHolderID="ContentPlaceHolder_A" Runat="Server">
    <h4>Suppliers 画面Ａ</h4>
    <p>
        ［件数確認］で Suppliers のデータ件数を OK メッセージ ダイアログに表示します。<br />
        ［画面遷移］で画面Ｂへ遷移します。
    </p>
    <%-- メイン ボタンはマスタ ページのフッタ部（btnMain1〜5）に配置している --%>
</asp:Content>

<asp:Content ID="cphFooterScripts" ContentPlaceHolderID="cphFooterScripts" Runat="Server">
    <!-- Footer 部の ContentPlaceHolder -->
</asp:Content>
