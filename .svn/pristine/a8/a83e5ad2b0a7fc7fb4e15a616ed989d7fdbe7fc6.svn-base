<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FRM006A.aspx.cs" Inherits="SAMBHS.Server.WebClientAdmin.UI.Security.FRM006A" %>
<%@ Register Assembly="FineUI" Namespace="FineUI" TagPrefix="x" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
 <form id="form1" runat="server">
        <x:PageManager ID="PageManager1" AutoSizePanelID="Panel5" runat="server" />
        <x:Panel ID="Panel5" runat="server" ShowBorder="True" BodyPadding="5px" ShowHeader="False" EnableBackgroundColor="True">
            <Toolbars>
                <x:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <x:Button ID="btnSaveRefresh" Text="Guardar y Cerrar" runat="server" Icon="SystemSave" OnClick="btnSaveRefresh_Click" ValidateForms="Form2,SimpleForm2" 
                            TabIndex="20">
                        </x:Button>
                        <x:Button ID="btnClose" EnablePostBack="false" Text="Cancelar y Cerrar" runat="server" Icon="SystemClose" TabIndex="21">
                        </x:Button>
                    </Items>
                </x:Toolbar>
            </Toolbars>
            <Items>
                        <x:GroupPanel runat="server" Title="Datos Generales" ID="GroupPanel4">
                            <Items>
                                   <x:Form ID="Form2" runat="server" EnableBackgroundColor="true" ShowBorder="False" ShowHeader="False"
                                    LabelWidth="145px">
                                        <Rows>
                                            <x:FormRow ID="FormRow1" runat="server">
                                                <Items>
                                                    <x:TextBox ID="txtFirstName" Label="Nombres" Required="true" ShowRedStar="true" runat="server" TabIndex="1" MaxLength="50" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 50" CssClass="mayus"> </x:TextBox>
                                                    <x:DropDownList ID="ddlDocType" Label="Tipo Documento" ShowRedStar="true" CompareType="String" CompareValue="-1"
                                                    CompareOperator="NotEqual" CompareMessage="Campo requerido" runat="server" 
                                                    TabIndex="8" OnSelectedIndexChanged="ddlDocType_SelectedIndexChanged" AutoPostBack="true"> </x:DropDownList>

                                                </Items>
                                            </x:FormRow>
                                            <x:FormRow ID="FormRow2" runat="server">
                                                <Items>
                                                    <x:TextBox ID="txtFirstLastName" Label="Apellido Paterno" Required="true" ShowRedStar="true" runat="server" TabIndex="2" MaxLength="50"
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 50" CssClass="mayus"/>
                                                    <x:NumberBox ID="txtDocNumber" NoDecimal="true" NoNegative="true" Label="Número de Documento" 
                                                    Required="true" ShowRedStar="true" 
                                                    runat="server" TabIndex="9" />

                                                </Items>
                                            </x:FormRow>
                                            <x:FormRow ID="FormRow3" runat="server">
                                                <Items>
                                                    <x:TextBox ID="txtSecondLastName" Label="Apellido Materno" Required="true" runat="server" TabIndex="3" MaxLength="50" 
                                                    MaxLengthMessage="El número máximo de caracteres permitidos es 50" CssClass="mayus" />
                                                    <x:DropDownList ID="ddlSexType" Label="Género" ShowRedStar="true" CompareType="String" CompareValue="-1"
                                                    CompareOperator="NotEqual" CompareMessage="Campo requerido" runat="server" TabIndex="10"/>

                                                </Items>
                                            </x:FormRow>

                                              <x:FormRow ID="FormRow4" runat="server">
                                                <Items>
                                                    <x:DropDownList ID="ddlMaritalStatus" Label="Estado Civil" ShowRedStar="true" CompareType="String" CompareValue="-1"
                                                    CompareOperator="NotEqual" CompareMessage="Campo requerido" runat="server" TabIndex="4" />
                                                    <x:DropDownList ID="ddlLevelOfId" Label="Nivel de Estudios" Required="false" runat="server" TabIndex="11"/>                                              
                                                </Items>
                                            </x:FormRow>

                                             <x:FormRow ID="FormRow5" runat="server">
                                                <Items>
                                                    <x:TextBox ID="txtTelephoneNumber" Label="Teléfono" Required="false" runat="server" TabIndex="6" MaxLength="15"
                                                     MaxLengthMessage="El número máximo de caracteres permitidos es 15"/>

                                                    <x:TextBox ID="txtAdressLocation" Label="Dirección" Required="false" runat="server" TabIndex="7" 
                                                    MaxLength="250" MaxLengthMessage="el número máximo de caracteres permitidos es 250" CssClass="mayus"/>

                                                </Items>
                                            </x:FormRow>
                                        </Rows>
                                    </x:Form>     
                            </Items>
                        </x:GroupPanel>                       
                        <x:GroupPanel runat="server" Title="Datos de Usuario" ID="GroupPanel2" Height="150">
                            <Items>
                                <x:SimpleForm ID="SimpleForm2" ShowBorder="false" ShowHeader="false" EnableBackgroundColor="true"
                                    AutoScroll="true" BodyPadding="5px" runat="server" EnableCollapse="True" LabelWidth="145px">
                                    <Items>
                                        <x:TextBox ID="txtUserName" Label="Usuario" runat="server" Required="true" ShowRedStar="true" TabIndex="17"
                                            MaxLength="25" MaxLengthMessage="El número máximo de caracteres permitidos es 25" />
                                        <x:TextBox ID="txtPassword1" Label="Password" runat="server" Required="true" ShowRedStar="true" TabIndex="18" TextMode="Password" 
                                            MaxLength="25" MinLength="8" MaxLengthMessage="el número máximo de caracteres permitidos es 25" 
                                            MinLengthMessage="El número mínimo de caracteres permitidos es 8" />
                                        <x:TextBox ID="txtPassword2" Label="Repetir Password" runat="server"
                                            CompareType="String" CompareControl="txtPassword1"
                                            CompareOperator="Equal" CompareMessage="Las contraseñas introducidas no coinciden. Vuelve a intentarlo"
                                            Required="true" ShowRedStar="true" TabIndex="19" TextMode="Password"
                                            MaxLength="25" MinLength="8" MaxLengthMessage="El número máximo de caracteres permitidos es 25" 
                                            MinLengthMessage="el número mínimo de caracteres permitidos es 8" />
                                         <x:DropDownList ID="ddlRole" Label="Rol" ShowRedStar="true" CompareType="String" CompareValue="-1"
                                                    CompareOperator="NotEqual" CompareMessage="Campo requerido" runat="server" TabIndex="4" />
                                         
                                    </Items>
                                </x:SimpleForm>
                            </Items>
                        </x:GroupPanel>
                    </Items>
                </x:Panel>
<%--            </Items>
        </x:Panel>--%>
        <x:HiddenField ID="hfRefresh" runat="server" />
    </form>
</body>
</html>
