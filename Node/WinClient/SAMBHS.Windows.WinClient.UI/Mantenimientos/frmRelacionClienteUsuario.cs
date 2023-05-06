using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmRelacionClienteUsuario : Form
    {
        private readonly UltraCombo _ucRolDocumento = new UltraCombo();
        UltraCombo ucbUnidadMedida = new UltraCombo();
        private List<int> IdsUpdates = new List<int>();
        private bool IsLoadRecetaSalida = false;
        public frmRelacionClienteUsuario(string p)
        {
            OperationResult objOperationResult = new OperationResult();
            _ucRolDocumento.DataSource = new VendedorBL().ObtenerUsuariosParaCombo(ref objOperationResult, null);
            InitializeComponent();
        }

        private void frmRelacionClienteUsuario_Load(object sender, EventArgs e)
        {
            BindGrid();

        }
        private void BindGrid()
        {
            Cargar();
        }

        private void Cargar()
        {
            if (IsLoadRecetaSalida) return ;
            BindingList<relacionusuarioclienteDto> _objData = new BindingList<relacionusuarioclienteDto>();
            OperationResult objOperationResult = new OperationResult();
            _objData = new ClienteBL().ObtenerRelacionesClientesUsuario(ref objOperationResult);
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Hubo un error al cargar Relaciones cliente Usuario", "Sistema", Icono: MessageBoxIcon.Error);
            }
            grdData.DataSource = _objData;
            IdsUpdates.Clear();
            IsLoadRecetaSalida = true;
        }
       

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
            grdData.Rows.Move(row, grdData.Rows.Count() - 1);
            this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
            row.Cells["i_SystemUser"].Value = -1;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            DocumentoRolBl.ActualizarDocumentosRol(ref objOperationResult,
                new List<DocumentoRolBl.RelacionDocumentoRol>(
                    (BindingList<DocumentoRolBl.RelacionDocumentoRol>)grdData.DataSource));

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(@"Datos Actualizados", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_SystemUser"].EditorComponent = _ucRolDocumento;
            e.Layout.Bands[0].Columns["i_SystemUser"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_SystemUser"].CellActivation = Activation.AllowEdit;
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "CodigoCliente":
                    {
                        var CodigoClienteAnt = grdData.ActiveRow.Cells["CodigoCliente"].Value ==null ?"": grdData.ActiveRow.Cells["CodigoCliente"].Value.ToString ();
                        
                        Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", "");
                        frm.ShowDialog();
                        if (frm._IdCliente != null)
                        {
                            grdData.ActiveRow.Cells["CodigoCliente"].Value = frm._CodigoCliente.Trim ();
                            grdData.ActiveRow.Cells["Cliente"].Value = frm._RazonSocial;
                            grdData.ActiveRow.Cells["v_IdCliente"].Value = frm._IdCliente;
                            if (CodigoClienteAnt != grdData.ActiveRow.Cells["CodigoCliente"].Value.ToString())
                            {
                                grdData.ActiveRow.Cells["DireccionCliente"].Value = "";
                                grdData.ActiveRow.Cells["i_IdDireccionCliente"].Value = 0;
                            }
                        }
                    }
                    break;

                case "DireccionCliente":
                        {

                            if (!string.IsNullOrEmpty(grdData.ActiveRow.Cells["v_IdCliente"].Value.ToString ()))
                            {
                                frmClienteDirecciones frm = new frmClienteDirecciones(grdData.ActiveRow.Cells["v_IdCliente"].Value.ToString (),true);
                                frm.ShowDialog();
                                if (frm.i_IdDireccionCliente != 0)
                                {
                                    grdData.ActiveRow.Cells["DireccionCliente"].Value = frm.v_DireccionCliente;

                                    grdData.ActiveRow.Cells["i_IdDireccionCliente"].Value = frm.i_IdDireccionCliente;
                                }
                            }
                        
                        }

                    break ;
            }


        }

        private bool ValidaCamposNulosVacios()
        {

            if (grdData.Rows.Where(p => p.Cells["i_SystemUser"].Value == null || p.Cells["i_SystemUser"].Value.ToString().Trim() == "-1").Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todos Usuarios", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["i_SystemUser"].Value == null || x.Cells["i_SystemUser"].Value.ToString().Trim() == "-1").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_SystemUser"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_SystemUser"];
                this.grdData.ActiveCell = aCell;
                return false;
            }
            if (grdData.Rows.Where(p => p.Cells["v_IdCliente"].Value == null || p.Cells["v_IdCliente"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todos Clientes", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_IdCliente"].Value == null || x.Cells["v_IdCliente"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["CodigoCliente"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["CodigoCliente"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Where(p => p.Cells["i_IdDireccionCliente"].Value == null || p.Cells["i_IdDireccionCliente"].Value.ToString().Trim() == "-1").Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente las direcciones de los Clientes", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["i_IdDireccionCliente"].Value == null || x.Cells["i_IdDireccionCliente"].Value.ToString().Trim() == "-1").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["DireccionCliente"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["DireccionCliente"];
                this.grdData.ActiveCell = aCell;
                return false;
            }
            

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objResult = new OperationResult();
            bool sucess = true;
            short news = 0, updates = 0;
            if (ValidaCamposNulosVacios())
            {
                foreach (var row in grdData.Rows)
                {
                    var itemRelacion = (row.ListObject as relacionusuarioclienteDto);
                    objResult.Success = 1;
                    if (itemRelacion.i_IdRelacionusuariocliente == 0)
                    {

                        new ClienteBL().InsertarRelacionUsuarioCliente(ref objResult, itemRelacion, Globals.ClientSession.GetAsList());
                        news++;
                    }
                    else if (IdsUpdates.Contains(itemRelacion.i_IdRelacionusuariocliente))
                    {
                        new ClienteBL().ActualizaRelacionUsuarioCliente(ref objResult, itemRelacion, Globals.ClientSession.GetAsList());
                        updates++;
                    }
                    if (objResult.Success == 0)
                    {
                        UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        sucess = false;
                        break;
                    }
                }
                if (sucess)
                {
                    UltraMessageBox.Show(string.Concat("Se Guardaron todos los cambios!",
                                            System.Environment.NewLine,
                                            news, " Nuevos",
                                            System.Environment.NewLine,
                                            updates, " Actualizados"), "Resumen");
                    IsLoadRecetaSalida = false;

                    Cargar();
                }
            }
        }

        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                var relacionClienteUsuarioDto = grdData.ActiveRow.ListObject as relacionusuarioclienteDto;
                if (relacionClienteUsuarioDto != null)
                {
                    var id = relacionClienteUsuarioDto.i_IdRelacionusuariocliente;
                    if (!IdsUpdates.Contains(id))
                        IdsUpdates.Add(id);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            if (grdData.ActiveRow != null)
            {
                var idVal = int.Parse(grdData.ActiveRow.Cells["i_IdRelacionusuariocliente"].Value.ToString());
                if (idVal == 0)
                {
                    grdData.ActiveRow.Delete();
                }
                else
                    if (MessageBox.Show("¿Esta seguro de eliminar este item?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        var result = new OperationResult();
                      new ClienteBL().EliminarRelacionUsuarioById(ref result, idVal, Globals.ClientSession.GetAsList());
                        if (result.Success == 0)
                            MessageBox.Show(result.ExceptionMessage, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            IsLoadRecetaSalida = false;
                            Cargar();
                           

                        }
                    }
            }
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            switch (this.grdData.ActiveCell.Column.Key)
            {

                case "CodigoCliente":
                    if (grdData.ActiveRow.Cells["CodigoCliente"].Value != null && string.IsNullOrEmpty(grdData.ActiveRow.Cells["CodigoCliente"].Value.ToString ()))
                        grdData.ActiveRow.Cells["v_IdCliente"].Value = null;
                      grdData.ActiveRow.Cells["DireccionCliente"].Value = null;
                      grdData.ActiveRow.Cells["i_IdDireccionCliente"].Value = -1;
                    break;

                case "i_SystemUser":
                    int Index = grdData != null && grdData.ActiveRow != null ? grdData.ActiveRow.Index : 1;
                    var Usuario = grdData.ActiveRow.Cells["i_SystemUser"].Value.ToString ();
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        if (Fila.Cells["i_SystemUser"].Value != null && Fila.Cells["i_SystemUser"].Value.ToString()!="-1")
                        {
                            if (Index != Fila.Index && Usuario == Fila.Cells["i_SystemUser"].Value.ToString())
                            {
                                UltraMessageBox.Show("Este usuario ya tiene asignado un Cliente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                grdData.ActiveRow.Cells["i_SystemUser"].Value = "-1";
                                return;
                            }
                        }
                    }
                    break;
            }
        }

       
    }
}
