<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frmLogin.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.frmLogin" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>login</title>
   <%-- <link href="CSS/main.css" rel="stylesheet" type="text/css" />--%>
</head>
<body>  
  <form id="form1" runat="server">
    <x:PageManager ID="PageManager1" runat="server" />
    <x:Window ID="Window1" runat="server" Title="Acceso al sistema" IsModal="false" EnableClose="false"
        WindowPosition="GoldenSection" Width="350px">
        <Items>
            <x:SimpleForm ID="SimpleForm1" runat="server" ShowBorder="false" BodyPadding="10px"
                LabelWidth="80px" EnableBackgroundColor="true" ShowHeader="false">
                <Items>
                    <x:TextBox ID="txtUserName" Label="Usuario" Required="true" runat="server" Text="sa">
                    </x:TextBox>
                    <x:TextBox ID="txtPassword" Label="Contraseña" TextMode="Password" Required="true" runat="server" Text="12345678">
                    </x:TextBox>
                    <x:Button ID="btnLogin" Text="Login" Type="Submit" ValidateForms="SimpleForm1" ValidateTarget="Top"
                        runat="server" OnClick="btnLogin_Click">
                    </x:Button>
                </Items>
            </x:SimpleForm>
        </Items>
    </x:Window>
    </form>
</body>
</html>
