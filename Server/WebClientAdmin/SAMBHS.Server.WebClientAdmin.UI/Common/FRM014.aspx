<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM014.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM014" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" runat="server" />
        <x:Panel ID="Panel1" runat="server" Height="200px" Width="600px"  ShowBorder="false"
            Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigPadding="5"
            BoxConfigChildMargin="0 5 0 0" ShowHeader="false" Title="Tablas" >

            <Items>
                <x:Panel ID="Panel3" Title="Tablas" EnableBackgroundColor="true" Width="150" runat="server"
                    BodyPadding="5px" ShowBorder="true" ShowHeader="true">
                    <Items>
                        <x:CheckBox ID="chkConfigEmpresa" ShowLabel="false" Readonly="true" runat="server" Text="Configuración Empresa" Checked="True" AutoPostBack="True">
                        </x:CheckBox>
                        <x:CheckBox ID="chkPlanCuentas" ShowLabel="false" Readonly="true" runat="server" Text="Plan de Cuentas" Checked="True" AutoPostBack="True">
                        </x:CheckBox>
                        <x:CheckBox ID="chkClientesProveedores" ShowLabel="false" Readonly="true" runat="server" Text="Clientes y Proveedores" Checked="True" AutoPostBack="True">
                        </x:CheckBox>
                        <x:CheckBox ID="chkDestinos" ShowLabel="false" Readonly="true" runat="server" Text="Destinos" Checked="True" AutoPostBack="True">
                        </x:CheckBox>
                        <x:CheckBox ID="chkConceptos" ShowLabel="false" Readonly="true" runat="server" Text="Conceptos" Checked="True" AutoPostBack="True">
                        </x:CheckBox>
                    </Items>
                </x:Panel>

                <x:Panel ID="Panel2" Title="Empresas" EnableBackgroundColor="true" BoxFlex="2" Width="425" runat="server"
                    BodyPadding="5px" ShowBorder="true" ShowHeader="false" Layout="VBox">
                    <Items>
                        <x:Panel ID="Panel4" Title="Empresa Origen" EnableBackgroundColor="true" Width="415" runat="server"
                            BodyPadding="5px" ShowBorder="true" ShowHeader="true" BoxMargin="0 0 5 0">
                            <Items>
                                <x:DropDownList runat="server" ID="ddlEmpresaOrigen"  Width ="330" ShowRedStar="true" CompareType="String"
                                    CompareValue="-1" CompareOperator="NotEqual" CompareMessage="Please select province!"
                                    AutoPostBack="true"> </x:DropDownList>
                            </Items>
                        </x:Panel>
                        
                        <x:Panel ID="Panel5" Title="Empresa Destino" Width="415" EnableBackgroundColor="true" runat="server"
                            BodyPadding="5px" ShowBorder="true" ShowHeader="true" BoxMargin="0 0 5 0">
                            <Items>
                                <x:DropDownList runat="server" ID="ddlEmpresaDestino" Width ="330" ShowRedStar="true" CompareType="String"
                                    CompareValue="-1" CompareOperator="NotEqual" CompareMessage="Please select province!"
                                    AutoPostBack="true" ></x:DropDownList>
                            </Items>
                        </x:Panel>

                        <x:Button ID="btnMigrar" runat="server" Icon="ApplicationGo" Text ="Migrar Tablas" OnClick="btnMigrar_Click"  EnablePostBack="true"></x:Button>
                    </Items>
                </x:Panel>
            </Items>
        </x:Panel>
    </form>
</body>
</html>
