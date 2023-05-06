using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmVentaDetalleAnexos : Form
    {
        string _Mode;
        ventadetalleanexoDto _objventadetalleanexo = new ventadetalleanexoDto();
        public frmVentaDetalleAnexos(string p)
        {
            InitializeComponent();
        }
        public int _IdAnexo = -1;
        public string _Anexo = null;
        private void frmVentaDetalleAnexos_Load(object sender, EventArgs e)
        {

            BindGrid();

        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            //if (grdData.Rows.Count == 0) return; //se cambio

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

            if (_Mode == "New")
            {
                _objventadetalleanexo = new ventadetalleanexoDto
                {
                    v_Anexo = txtAnexo.Text.Trim(),
                };

                _objventadetalleanexo = new VentaBL().InsertarVentaDetalleAnexo(ref objOperationResult, _objventadetalleanexo, Globals.ClientSession.GetAsList());

            }
            else if (_Mode == "Edit")
            {
                _objventadetalleanexo.v_Anexo = txtAnexo.Text.Trim();
                _objventadetalleanexo = new VentaBL().ActualizarVentaDetalleAnexo(ref objOperationResult, _objventadetalleanexo, Globals.ClientSession.GetAsList());
            }
            if (objOperationResult.Success == 1)  // Operación sin error
            {
                ActivarBotones(true, true, false, false, true);
                MantenerSeleccion(_objventadetalleanexo.i_IdVentaDetalleAnexo);
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else  // Operación con error
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            BindGrid();
            MantenerSeleccion(_objventadetalleanexo.i_IdVentaDetalleAnexo);
        }

        private void BindGrid()
        {
            var objData = GetData();

            grdData.DataSource = objData;
        }

        private BindingList<ventadetalleanexoDto> GetData()
        {
            BindingList<ventadetalleanexoDto> _objData = new BindingList<ventadetalleanexoDto>();
            OperationResult objOperationResult = new OperationResult();
            _objData = new VentaBL().ObtenerVentaDetallesAnexo(ref  objOperationResult);

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
                if (int.Parse(row.Cells["i_IdVentaDetalleAnexo"].Value.ToString()).Equals(ValorSeleccionado))
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
            txtAnexo.Text = "";
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
            _Mode = "New";
            ActivarBotones(false, false, true, true, false);
            txtAnexo.Focus();
        }

        public void Nuevo()
        {
            txtAnexo.Text = "";
            txtAnexo.Enabled = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int intDocCode = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdVentaDetalleAnexo"].Value.ToString());
            if (intDocCode != null && intDocCode != 0)
            {

                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    new VentaBL().EliminarVentaDetalleAnexo(ref objOperationResult, intDocCode, Globals.ClientSession.GetAsList());

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                            objOperationResult.AdditionalInformation, "Error en el Proceso", MessageBoxButtons.OK,
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
            if (grdData.ActiveRow.IsActiveRow == true)
            {
                EdicionDesactivado();
                int intDocCode = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdVentaDetalleAnexo"].Value.ToString());
                OperationResult objOperationResult = new OperationResult();
                txtAnexo.Text = grdData.ActiveRow.Cells["v_Anexo"].Value.ToString();
                txtAnexo.Enabled = true;
                _objventadetalleanexo = new ventadetalleanexoDto();
                _objventadetalleanexo = new VentaBL().ObtenerVentaDetallesAnexoPorId(ref objOperationResult, intDocCode);
                ActivarBotones(true, true, false, false, true);
            }


        }
    }
}
