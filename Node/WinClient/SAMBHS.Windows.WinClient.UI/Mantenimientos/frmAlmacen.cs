using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmAlmacen : Form
    {
        #region Properties
        AlmacenBL _objAlmacenBL = new AlmacenBL();
        ProductoBL _objProductoBL = new ProductoBL();
        almacenDto _almacenDto;
        string _mode = default(string);
        #endregion

        public frmAlmacen(string N)
        {
            InitializeComponent();
            Load += delegate
            {
                BackColor = new GlobalFormColors().FormColor;
                DetalleAlmacen(false);
                BindGrid();
                grdData.DoubleClickRow += btnEditar_Click; // doble click para Editar
                grdData.KeyDown += (sender, e) =>
                {
                    if(e.KeyCode == Keys.Enter && grdData.ActiveRow != null)
                        txtNombre.Focus();
                };
                btnAgregar.Focus();
            };
        }

        #region Grilla
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["i_IdAlmacen"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }
        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (grdData.ActiveRow == null)
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
                btnMatricularProductos.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
                btnMatricularProductos.Enabled = true;
            }
        }
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (pnDatos.Visible)
            {
                CargaDetalleAlmacen((grdData.ActiveRow.ListObject as AlmacenConsulta).i_IdAlmacen);
                DetalleAlmacen(true);
            }
        }
        private void BindGrid()
        {
            var objData = GetData("i_IdAlmacen ASC");
            grdData.DataSource = objData;
            grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand,
                                                                                    Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All);
        }
        private List<AlmacenConsulta> GetData(string pstrSortExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objAlmacenBL.ObtenerListadoAlmacen(ref objOperationResult, pstrSortExpression, "");
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _objData;
        }
        #endregion

        #region Events Click Buttons
        private void btnGrabar_Click(object sender, EventArgs e)
        {    
            if (txtNombre.Text.Trim().Equals(""))
            {
                UltraMessageBox.Show("Por favor Ingrese un Nombre Correcto.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            if (ExisteNombre(txtNombre.Text.Trim()) && (_mode.Equals("New") || !txtNombre.Text.Trim().Equals(_almacenDto.v_Nombre.ToUpper())))
            {
                UltraMessageBox.Show("Este Nombre ya esta siendo utilizado, Ingrese otro nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            if (txtDireccion.Text.Trim().Equals(""))
            {
                UltraMessageBox.Show("Por favor Ingrese una Direccion Correcta.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDireccion.Focus();
                return;
            }

            OperationResult objOperationResult = new OperationResult();
            if (_mode.Equals("New"))
                _almacenDto = new almacenDto();

            _almacenDto.v_Nombre = txtNombre.Text.Trim();
            _almacenDto.v_Direccion = txtDireccion.Text.Trim();
            _almacenDto.v_Telefono = txtTelefono.Text.Trim();
            _almacenDto.v_NombreComercial = txtNomComercial.Text.Trim();
            _almacenDto.v_NumeroSerieTicket = txtNomSerieTicket.Text.Trim();
            _almacenDto.v_Observacion = txtObservacion.Text.Trim();
            _almacenDto.i_ValidarStockAlmacen = uckValidarStock.Checked ? 1 : 0;
            _almacenDto.v_Ubigueo = txtUbigueo.Text;
            var idAlmacen = _mode.Equals("New")
                ? _objAlmacenBL.InsertarAlmacen(ref objOperationResult, _almacenDto, Globals.ClientSession.GetAsList())
                : _objAlmacenBL.ActualizarAlmacen(ref objOperationResult, _almacenDto, Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 1)
            {
                BindGrid();
                MantenerSeleccion(idAlmacen.ToString());
                DetalleAlmacen(false);
                UltraMessageBox.Show("El Registro se ha " + (_mode.Equals("New")? "guardado" : "actualizado") + " correctamente",  "Sistemas");
            }
            else
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DetalleAlmacen(false);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            _mode = "New";
            DetalleAlmacen(true);
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                CargaDetalleAlmacen((grdData.ActiveRow.ListObject as AlmacenConsulta).i_IdAlmacen);
                DetalleAlmacen(true);
                txtNombre.Focus();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            int IdAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());
            if (_objAlmacenBL.ExistenciaAlmacenDiversosProcesos(IdAlmacen))
                UltraMessageBox.Show("No se puede eliminar,este almacén esta siendo utilizado en Otros procesos", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                DialogResult Resultado = UltraMessageBox.Show("¿Está seguro de eliminar el Registro?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Resultado == DialogResult.No) return;
                
                OperationResult objOperationResult = new OperationResult();
                _objAlmacenBL.EliminarAlmacen(ref objOperationResult, IdAlmacen, Globals.ClientSession.GetAsList());
                if (objOperationResult.Success == 1)
                {
                    DetalleAlmacen(false);
                    BindGrid();
                }
                else
                    UltraMessageBox.Show("No se pudo Elimina el Registro " + System.Environment.NewLine + objOperationResult.ErrorMessage,
                                          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Methods
        private void DetalleAlmacen(bool state)
        {
            //gboxDetalle.Enabled = state;
            pnDatos.Visible = state;
        }

        /// <summary>
        /// Limpias los TextBox del GroupBox Datos de Almacen
        /// </summary>
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtDireccion.Clear();
            txtNomComercial.Clear();
            txtNomSerieTicket.Clear();
            txtTelefono.Clear();
            txtObservacion.Clear();
        }

        /// <summary>
        /// Carga los datos de un registro para edicion
        /// </summary>
        /// <param name="pintIdAlmacen">Id del registro en almacen</param>
        private void CargaDetalleAlmacen(int pintIdAlmacen)
        {
            OperationResult objOperationResult = new OperationResult();
            _almacenDto = _objAlmacenBL.ObtenerAlmacen(ref objOperationResult, pintIdAlmacen);
            if (_almacenDto == null)
                return;

            _mode = "Edit";
            txtNombre.Text = _almacenDto.v_Nombre;
            txtDireccion.Text = _almacenDto.v_Direccion;
            txtTelefono.Text = _almacenDto.v_Telefono;
            txtNomComercial.Text = _almacenDto.v_NombreComercial;
            txtNomSerieTicket.Text = _almacenDto.v_NumeroSerieTicket;
            txtObservacion.Text = _almacenDto.v_Observacion;
            uckValidarStock.Checked = _almacenDto.i_ValidarStockAlmacen == 1;
            txtUbigueo.Text = _almacenDto.v_Ubigueo;
            ValidatorCampos.Validate(true, false);
        }

        /// <summary>
        /// Comprueba si el Nombre de un Registro se encuentra en almacen.
        /// </summary>
        /// <param name="nombre">Propieda Nombre del Registro a validar</param>
        /// <returns>true si el nombre ya ha sido registrado, false de otro modo</returns>
        private bool ExisteNombre(string nombre)
        {
            var exist = grdData.Rows.Any(row => row.Cells["v_Nombre"].Text.ToUpper() == nombre);
            return exist;
        }
        #endregion

        private void btnMatricularProductos_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult ();
           
            if (grdData.ActiveRow != null)
            {
                if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
                {
                    if (UltraMessageBox.Show("Está seguro de insertar los productos en : \nAlmacén : " + grdData.ActiveRow.Cells["v_Nombre"].Value.ToString() + "\nPeriodo : " + Globals.ClientSession.i_Periodo.ToString(), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Globals.ProgressbarStatus.i_Progress = 1;
                        Globals.ProgressbarStatus.i_TotalProgress = 1;
                        Globals.ProgressbarStatus.b_Cancelado = false;
                        bwkProcesoBL.RunWorkerAsync();
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();

                    }
                }
            }

            
        }


        private void bwkProcesoBL_DoWork(object sender, DoWorkEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            MovimientoBL _objMovimientoBL = new MovimientoBL();

            _objProductoBL.InscribirProductosEnAlmacen(ref objOperationResult, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), Globals.ClientSession.i_Periodo.ToString(), Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Globals.ProgressbarStatus.b_Cancelado = true;
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al matricular productos en almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void txtUbigueo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var ubigueo = txtUbigueo.Text;
            var frm = new frmBusquedaUbigueo(ubigueo);
            if (frm.ShowDialog() != DialogResult.OK)
                return;

            txtUbigueo.Text = frm.Ubigueo;
        }

    }
}
