<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM009A.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Common.FRM009A" %>
<%@ Register assembly="FineUI" namespace="FineUI" tagprefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/main.css" rel="stylesheet" />
</head>
<body>
     <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" AutoSizePanelID="Panel5" runat="server" />
        <x:Panel ID="Panel5" runat="server" ShowBorder="True" BodyPadding="5px" ShowHeader="False" EnableBackgroundColor="True">
            <Toolbars>
                <x:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <x:Button ID="btnSaveRefresh" Text="Guardar y Cerrar" runat="server" Icon="SystemSave" OnClick="btnSaveRefresh_Click" ValidateForms="Form2,SimpleForm2" 
                            TabIndex="8">
                        </x:Button>
                        <x:Button ID="btnClose" EnablePostBack="false" Text="Cancelar y Cerrar" runat="server" Icon="SystemClose" TabIndex="9">
                        </x:Button>
                    </Items>
                </x:Toolbar>
            </Toolbars>
            <Items>
                <x:Panel ID="Panel6" AutoHeight="true" EnableBackgroundColor="True"
                    runat="server" BodyPadding="2px" ShowBorder="False" ShowHeader="False">
                    <Items>
                        <x:GroupPanel runat="server" Title="Datos Generales" ID="GroupPanel4">
                            <Items>
                                <x:Form ID="Form2" runat="server" EnableBackgroundColor="true" ShowBorder="False" ShowHeader="False"
                                    LabelWidth="160px">
                                    <Rows>
                                        <x:FormRow ID="FormRow1" runat="server">
                                            <Items>
                                                <x:TextBox ID="txtName" Label="Nombre" Required="true" ShowRedStar="true" runat="server" TabIndex="1" MaxLength="250" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 250" CssClass="mayus"/>                                              
                                            </Items>
                                        </x:FormRow>
                                        <x:FormRow ID="FormRow3" runat="server">
                                            <Items>
                                                <x:TextBox ID="txtAddress" Label="Dirección" Required="false" runat="server" TabIndex="2" MaxLength="250" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 250" CssClass="mayus" />                                            
                                            </Items>
                                        </x:FormRow>
                                         <x:FormRow ID="FormRow5" runat="server">
                                            <Items>
                                                <x:TextBox ID="txtPhoneNumber" Label="Teléfono" Required="false" runat="server" TabIndex="3" MaxLength="15" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 15" CssClass="mayus" />                                            
                                            </Items>
                                        </x:FormRow>
                                             <x:FormRow ID="FormRow6" runat="server">
                                            <Items>
                                                <x:TextBox ID="txtTicketSerialNumber" Label="Nro. Serie Ticket" Required="false" runat="server" TabIndex="4" MaxLength="50" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 50" CssClass="mayus" />                                            
                                            </Items>
                                        </x:FormRow>
                                        <x:FormRow ID="FormRow7" runat="server">
                                            <Items>
                                                <x:TextBox ID="txtAlm_Nro_Cod_Etb" Label="Nro. Serie Impresora" Required="false" runat="server" TabIndex="5" MaxLength="20" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 20" CssClass="mayus" />                                            
                                            </Items>
                                        </x:FormRow>
                                   
                                    </Rows>
                                </x:Form>
                            </Items>
                        </x:GroupPanel>
                    
                    </Items>
                </x:Panel>
            </Items>
        </x:Panel>
      
    </form>
</body>
</html>
