<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM004R.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM004R" %>
<%@ Register assembly="FineUI" namespace="FineUI" tagprefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server">
    <x:PageManager ID="PageManager1" runat="server" />
    <x:SimpleForm ID="SimpleForm1" BodyPadding="5px" runat="server" EnableBackgroundColor="true"
        ShowBorder="False" Title="Desde archivo..." Width="450px" ShowHeader="True" Height="120" AjaxLoadingType="Mask">
        <Items>
            <x:FileUpload runat="server" ID="FileUpload1" EmptyText="Porfavor elija un archivo" Label="Archivo" Required="true"
                ShowRedStar="true" AjaxLoadingType="Mask">
            </x:FileUpload>
            <x:Button ID="btnSubir" runat="server" OnClick="btnSaveRefresh_Click" ValidateForms="SimpleForm1"
                Text="Restaurar" Icon="DatabaseGo" ConfirmText="¿Seguro de restaurar?" AjaxLoadingType="Mask"> 
            </x:Button>
        </Items>
    </x:SimpleForm>
    </form>
</body>
</html>