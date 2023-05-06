<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM008B.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM008B" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
 <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" AutoSizePanelID="Panel5" runat="server" />
        <x:Panel ID="Panel5" runat="server" ShowBorder="True" BodyPadding="5px" ShowHeader="False"
            EnableBackgroundColor="True" AutoWidth="true">
            <Toolbars>
                <x:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <x:Button ID="btnClose" EnablePostBack="false" Text="Cerrar" runat="server" Icon="SystemClose" TabIndex="8">
                        </x:Button>
                    </Items>
                </x:Toolbar>
            </Toolbars>
            <Items>
                <x:Panel ID="Panel2" EnableBackgroundColor="true"
                    runat="server" BodyPadding="5px" ShowBorder="False" ShowHeader="False">
                    <Items>
                        <x:GroupPanel runat="server" Title="Escoger Almacén" ID="GroupPanel3" EnableBackgroundColor="True">
                            <Items>
                                <x:Form ID="Form2" runat="server" EnableBackgroundColor="true" ShowBorder="False" ShowHeader="False"
                                    LabelWidth="80px">
                                    <Rows>
                                        <x:FormRow ID="FormRow1" runat="server">
                                            <Items>
                                               <x:DropDownList ID="ddlWarehouseId" runat="server" Width="350" Label="Almacenes" Resizable="True"></x:DropDownList>
                                            </Items>
                                        </x:FormRow>
                                        <x:FormRow ID="FormRow3" runat="server">
                                            <Items>
                                                <x:Button ID="btnAdd" Text="Agregar" runat="server" Icon="Add"
                                                    ValidateForms="Form2" TabIndex="7" OnClick="btnAdd_Click">
                                                </x:Button>
                                            </Items>
                                        </x:FormRow>
                                    </Rows>
                                </x:Form>
                            </Items>
                        </x:GroupPanel>
                        <x:GroupPanel runat="server" Title="Almacenes asignados" ID="GroupPanel1" EnableBackgroundColor="True" AutoWidth="true">
                            <Items>
                               <x:Grid ID="grdData" ShowBorder="false" ShowHeader="false" Title="Almacenes" runat="server"
                                    EnableRowNumber="True" AllowPaging="false" OnPageIndexChange="grdData_PageIndexChange"
                                    IsDatabasePaging="true" EnableRowNumberPaging="true" AutoHeight="true" RowNumberWidth="40" AjaxLoadingType="Default"
                                    EnableMouseOverColor="true" ShowGridHeader="true" OnRowCommand="grdData_RowCommand" DataKeyNames="v_OrganizationWarehouseId"
                                    EnableTextSelection="true" EnableAlternateRowColor="true" Height="260px">

                                    <Columns>
                                        <x:LinkButtonField TextAlign="Center" ConfirmText="Está seguro de eliminar el item seleccionado?" Icon="Delete" ConfirmTarget="Top"
                                            ColumnID="lbfAction2" Width="30px" ToolTip="Eliminar Almacén" CommandName="DeleteAction" />
                                        <x:BoundField Width="400px" DataField="v_WarehouseName" DataFormatString="{0}" HeaderText="Almacén" />
                                    </Columns>
                                </x:Grid>
                            </Items>
                        </x:GroupPanel>
                    </Items>
                </x:Panel>

            </Items>
        </x:Panel>

    </form>
</body>
</html>
