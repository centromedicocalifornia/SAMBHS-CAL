using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SAMBHS.Common.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmEmpresa : Form
    {
        private string _nroDoc;

        public string orgnizationEmployerId { get; set; }
        public string locationnEmployerId { get; set; }
        public string OrganizationemployerName { get; set; }

        public frmEmpresa(string ruc)
        {
            _nroDoc = ruc;
            InitializeComponent();
        }

        private void btnConsultaInternet_Click(object sender, EventArgs e)
        {
            string[] _Contribuyente = new string[10];
            frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(_nroDoc);

            frm.ShowDialog();
            if (frm.ConectadoRecibido == true)
            {
                _Contribuyente = frm.DatosContribuyente;
                txtRazonSocial.Text = _Contribuyente[0].ToUpper().Trim();

                txtDireccion.Text = Regex.Replace(_Contribuyente[5], @"[ ]+", " ");
                var resultUbigueo = Utils.Ubigeo.GetUbigueo(txtDireccion.Text);
                if (resultUbigueo != null)
                {
                    ddlDepartamento.Value = resultUbigueo[0].Key;
                    ddlProvincia.Value = resultUbigueo[1].Key;
                    ddlDistrito.Value = resultUbigueo[2].Key;
                }
              
            }
        }

        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        private void frmEmpresa_Load(object sender, EventArgs e)
        {
            txtNroDocumento.Text = _nroDoc;
            OperationResult objOperationResult = new OperationResult();
            
            //Combo País
            var ListaPaises = (_objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null)).FindAll(p => p.Value3 == "-1");
            Utils.Windows.LoadUltraComboEditorList(ddlPais, "Value1", "Id", ListaPaises, DropDownListAction.Select);
            //Combo Departamento
            Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);

            ddlPais.Value = "1";
            ddlDepartamento.Value = "1391";
            ddlProvincia.Value = "1392";
            ddlDistrito.Value = "1393";

            txtNroDocumento.Focus();
        }

        private void ddlPais_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlPais.Value == null) return;

            //si el combo esta en seleccione tengo que reiniciar el combo departamento
            if (ddlPais.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlPais.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlDepartamento_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlDepartamento.Value == null) return;

            if (ddlDepartamento.Value.ToString() == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlProvincia_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlProvincia.Value == null) return;

            if (ddlProvincia.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlProvincia.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EmpresaDto oEmpresaDto = new EmpresaDto();

            OrganizationemployerName = txtRazonSocial.Text;
            oEmpresaDto.v_Name = txtRazonSocial.Text;
            oEmpresaDto.v_Address = txtDireccion.Text;
            oEmpresaDto.v_IdentificationNumber = txtNroDocumento.Text;
            oEmpresaDto.v_Mail = txtEMail.Text;
            oEmpresaDto.v_PhoneNumber = txtCelular.Text;
            oEmpresaDto.i_SectorTypeId = -1;
            oEmpresaDto.i_OrganizationTypeId = 1;
            EmpresaBl oEmpresaBl = new  EmpresaBl();

            var _organizationId = oEmpresaBl.AddOrganization(oEmpresaDto);
           
            var lista = oEmpresaBl.GetOrdenReportes("N009-OO000000052");

            List<OrdenReportes> ListaOrdem = new List<OrdenReportes>();
            OrdenReportes oOrdenReportes;
            foreach (var item in lista)
            {
                oOrdenReportes = new OrdenReportes();
                oOrdenReportes.i_Orden = item.i_Orden;
                oOrdenReportes.v_OrganizationId = _organizationId;
                oOrdenReportes.v_NombreReporte = item.v_NombreReporte;
                oOrdenReportes.v_ComponenteId = item.v_ComponenteId;
                oOrdenReportes.v_NombreCrystal = item.v_NombreCrystal;
                oOrdenReportes.i_NombreCrystalId = item.i_NombreCrystalId;
                ListaOrdem.Add(oOrdenReportes);
            }

            oEmpresaBl.AddOrdenReportes(ListaOrdem);

            //Agregar Sede
            LocationDto objLocationDto = new LocationDto();
            // Populate the entity
            objLocationDto.v_OrganizationId = _organizationId;
            objLocationDto.v_Name = ddlDistrito.Text;

            var locationId = oEmpresaBl.AddLocation(objLocationDto);
            locationnEmployerId = locationId;
            orgnizationEmployerId = _organizationId + "|" + locationId;

            NodeOrganizationLoactionWarehouseList objNodeOrganizationLoactionWarehouseList = new NodeOrganizationLoactionWarehouseList();

            //Llenar Entidad Empresa/sede
            objNodeOrganizationLoactionWarehouseList.i_NodeId = 9;
            objNodeOrganizationLoactionWarehouseList.v_OrganizationId = _organizationId;
            objNodeOrganizationLoactionWarehouseList.v_LocationId = locationId;

            oEmpresaBl.AddNodeOrganizationLoactionWarehouse(objNodeOrganizationLoactionWarehouseList, null);

            //Crear GESO
            groupoccupationDto objgroupoccupationDto = new groupoccupationDto();
            // Populate the entity
            objgroupoccupationDto.v_Name = "ADMINISTRATIVO";
            objgroupoccupationDto.v_LocationId = locationId;
            // Save the data
            oEmpresaBl.AddGroupOccupation(objgroupoccupationDto);

            objgroupoccupationDto = new groupoccupationDto();
            // Populate the entity
            objgroupoccupationDto.v_Name = "OPERARIO";
            objgroupoccupationDto.v_LocationId = locationId;
            // Save the data
            oEmpresaBl.AddGroupOccupation(objgroupoccupationDto);

            DialogResult = DialogResult.OK;
        }
    }
}
