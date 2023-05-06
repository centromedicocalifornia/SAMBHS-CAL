using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmClienteDirecciones : Form
    {
        private readonly SystemParameterBL _objSystemParameterBl = new SystemParameterBL();
        string _Mode;
        clientedireccionesDto _objclienteAnexoDto = new clientedireccionesDto();
        string IdCliente;
        bool UsadoPedido=false ;
        public int i_IdDireccionCliente =-1;
        public string v_DireccionCliente = "";
        public string Zona = "";
        public frmClienteDirecciones(string pIdCliente,bool bUsadoPedido=false)
        {
            InitializeComponent();
            IdCliente =pIdCliente ;
            UsadoPedido =bUsadoPedido ;
        }
        public int _IdAnexo = -1;
        public string _Anexo = null;
        private void frmClienteDirecciones_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            //Combo Departamento
            Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(ddlZonaDireccion, "Value1", "Id", new DatahierarchyBL().GetDataHierarchiesForCombo(ref objOperationResult, 161, null), DropDownListAction.Select);
            ddlZonaDireccion.Value = -1;
            ddlDepartamento.Value =
            ddlProvincia.Value =
            ddlDistrito.Value = "-1";
            BindGrid();
            VisualizacionBotones(UsadoPedido);
            if (UsadoPedido)
            {
                lblDatosDireccion.Visible = !UsadoPedido;
                grdData.Size = new Size(527, 410);
                grdData.Location = new Point(5, 3);
            }

        }

        private void VisualizacionBotones(bool visible)
        {
            btnEditar.Visible = !visible;
            btnGuardar.Visible = !visible;
            btnCancelar.Visible = !visible;
            btnNuevo.Visible = !visible;
            btnEliminar.Visible = !visible;
        
        }
        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            //if (grdData.Rows.Count == 0) return; 

            //if (grdData.ActiveRow != null)
            //{
            //    EdicionDesactivado();
            //    if (grdData.ActiveRow.Cells["i_IdVentaDetalleAnexo"].Value != null)
            //    {
            //        _IdAnexo = int.Parse(grdData.ActiveRow.Cells["i_IdVentaDetalleAnexo"].Value.ToString());
            //        _Anexo = grdData.ActiveRow.Cells["v_Anexo"].Value.ToString().Trim();
            //    }
            //    txtAnexo.Enabled = false;
            //    ActivarBotones(true, true, false, false, true);
            //}
            if (UsadoPedido)
            {

                i_IdDireccionCliente = grdData.ActiveRow.Cells["i_IdDireccionCliente"].Value == null ? 0 : int.Parse(grdData.ActiveRow.Cells["i_IdDireccionCliente"].Value.ToString());
                v_DireccionCliente = grdData.ActiveRow.Cells["v_Direccion"].Value == null ? "" : grdData.ActiveRow.Cells["v_Direccion"].Value.ToString();
                Zona = grdData.ActiveRow.Cells["Zona"].Value == null ? "" : grdData.ActiveRow.Cells["Zona"].Value.ToString();

                Close();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            _Mode = "Edit";
            EdicionActivado();
            ActivarBotones(false, false, true, true, false);
        }
        public void EdicionDesactivado()
        {

            grdData.Enabled = true;
        }

        public void EdicionActivado()
        {
            grdData.Enabled = false;

        }
        public void ActivarBotones(bool editar, bool eliminar, bool guardar, bool cancelar, bool nuevo)
        {
            btnEditar.Enabled = editar;
            btnEliminar.Enabled = eliminar;
            btnGuardar.Enabled = guardar;
            btnCancelar.Enabled = cancelar;
            btnNuevo.Enabled = nuevo;

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (Validar.Validate(true, false).IsValid)
            {

                
                
                if (_Mode == "New")
                {

                    _objclienteAnexoDto.v_Direccion = txtDireccion.Text.Trim();
                    _objclienteAnexoDto.i_IdZona = int.Parse(ddlZonaDireccion.Value.ToString());
                    _objclienteAnexoDto.i_IdDepartamento = int.Parse(ddlDepartamento.Value.ToString());
                    _objclienteAnexoDto.i_IdProvincia = int.Parse(ddlProvincia.Value.ToString());
                    _objclienteAnexoDto.i_IdDistrito = int.Parse(ddlDistrito.Value.ToString());
                    _objclienteAnexoDto.i_EsDireccionPredeterminada = uckPredeterminada.Checked ? 1 : 0;
                    bool YaTieneDireccionPredeterminada = new ClienteBL().YaTieneDireccionPredeterminada(_objclienteAnexoDto);
                    if (!YaTieneDireccionPredeterminada)
                    {
                        _objclienteAnexoDto = new ClienteBL().InsertarDireccionesCliente(ref objOperationResult, _objclienteAnexoDto, Globals.ClientSession.GetAsList(), IdCliente);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ya existe un Direccion predeterminada para este cliente.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                else if (_Mode == "Edit")
                {
                    _objclienteAnexoDto.v_Direccion = txtDireccion.Text.Trim();
                    _objclienteAnexoDto.i_IdZona = int.Parse(ddlZonaDireccion.Value.ToString());
                    _objclienteAnexoDto.i_IdDepartamento = int.Parse(ddlDepartamento.Value.ToString());
                    _objclienteAnexoDto.i_IdProvincia = int.Parse(ddlProvincia.Value.ToString());
                    _objclienteAnexoDto.i_IdDistrito = int.Parse(ddlDistrito.Value.ToString());
                    _objclienteAnexoDto.i_EsDireccionPredeterminada = uckPredeterminada.Checked ? 1 : 0;
                    bool YaTieneDireccionPredeterminada = new ClienteBL().YaTieneDireccionPredeterminada(_objclienteAnexoDto);
                    if (YaTieneDireccionPredeterminada)
                    {
                        _objclienteAnexoDto = new ClienteBL().ActualizarDireccionesCliente(ref objOperationResult, _objclienteAnexoDto, Globals.ClientSession.GetAsList());
                      
                    }
                    else
                    {
                        UltraMessageBox.Show("Ya existe un Direccion predeterminada para este cliente.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    ActivarBotones(true, true, false, false, true);
                    MantenerSeleccion(_objclienteAnexoDto.i_IdDireccionCliente);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                BindGrid();
                MantenerSeleccion(_objclienteAnexoDto.i_IdDireccionCliente);
            }
        }

        private void BindGrid()
        {
            var objData = GetData();

            grdData.DataSource = objData;
        }

        private List<clientedireccionesDto> GetData()
        {
            List<clientedireccionesDto> _objData = new List<clientedireccionesDto>();
            OperationResult objOperationResult = new OperationResult();


            _objData = new ClienteBL().ObtenerDireccionClientes(ref objOperationResult, IdCliente);
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Hubo un error al cargar Anexos", "Sistema", Icono: MessageBoxIcon.Error);
            }


            return _objData;
        }

        private void MantenerSeleccion(int ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (int.Parse(row.Cells["i_IdDireccionCliente"].Value.ToString()).Equals(ValorSeleccionado))
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CLR();
            ActivarBotones(false, false, false, false, true);
        }

        public void CLR()
        {
            grdData.Enabled = true;
            txtDireccion.Text = "";
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
            _Mode = "New";
            ActivarBotones(false, false, true, true, false);
            txtDireccion.Focus();
        }

        public void Nuevo()
        {
            lblDatosDireccion.Enabled = true;
            txtDireccion.Clear();
            uckPredeterminada.Checked = false;
            ddlZonaDireccion.Value = -1;
            ddlDepartamento.Value =
            ddlProvincia.Value =
            ddlDistrito.Value = "-1";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int intDocCode = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdDireccionCliente"].Value.ToString());
            if (intDocCode != null && intDocCode != 0)
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsDireccionPredeterminada"].Value.ToString() == "1")
                {
                    MessageBox.Show(
                               "No se puede eliminar una dirección predeterminada", "Sistema", MessageBoxButtons.OK,
                               MessageBoxIcon.Stop);
                    return;
                }
                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    new VentaBL().EliminarDireccionesCliente(ref objOperationResult, intDocCode, Globals.ClientSession.GetAsList());

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                            objOperationResult.AdditionalInformation, @"Error en el Proceso", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    BindGrid();
                    Nuevo();
                    EdicionDesactivado();
                    ActivarBotones(false, false, false, false, true);
                }
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                EdicionDesactivado();
                var obj = (clientedireccionesDto)grdData.ActiveRow.ListObject;
                int intDocCode = obj.i_IdDireccionCliente;
               
                lblDatosDireccion.Enabled = true;
                txtDireccion.Text = obj.v_Direccion;
                ddlZonaDireccion.Value = obj.i_IdZona;
                ddlDepartamento.Value = (obj.i_IdDepartamento ?? -1).ToString();
                ddlProvincia.Value = (obj.i_IdProvincia ?? -1).ToString();
                ddlDistrito.Value = (obj.i_IdDistrito ?? -1).ToString();
                uckPredeterminada.Checked = obj.i_EsDireccionPredeterminada == 1;
                txtDireccion.Enabled = true;
                _objclienteAnexoDto = new clientedireccionesDto();
                var objOperationResult = new OperationResult();
                _objclienteAnexoDto = new ClienteBL().ObtenerDireccionesPorId(ref objOperationResult, intDocCode);
                ActivarBotones(true, true, false, false, true);
            }


        }

        private void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlDepartamento.Value == null) return;

            if ((string)ddlDepartamento.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
            ddlProvincia.Value = "-1";
        }

        private void ddlProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlProvincia.Value == null) return;

            if ((string)ddlProvincia.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlProvincia.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
            ddlDistrito.Value = "-1";
        }

        private void frmTablaDescripciones_Fill_Panel_PaintClient(object sender, PaintEventArgs e)
        {

        }
    }
}
