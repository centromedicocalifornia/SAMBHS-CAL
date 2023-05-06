using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Cobranza.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRelacionesFormaPagoDocumentos : Form
    {
        FormaPagoDocumentoBL _objFormaPagoDocumentoBL = new FormaPagoDocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        UltraCombo ucFormaPago = new UltraCombo();
        UltraCombo ucDocumento = new UltraCombo();
        relacionformapagodocumentoDto _relacionformapagodocumentoDto = new relacionformapagodocumentoDto();

        #region Temporales DetalleDiario
        List<relacionformapagodocumentoDto> _TempDetalle_AgregarDto = new List<relacionformapagodocumentoDto>();
        List<relacionformapagodocumentoDto> _TempDetalle_ModificarDto = new List<relacionformapagodocumentoDto>();
        List<relacionformapagodocumentoDto> _TempDetalle_EliminarDto = new List<relacionformapagodocumentoDto>();
        #endregion

        public frmRelacionesFormaPagoDocumentos(string P)
        {
            InitializeComponent();
        }

        private void frmRelacionesFormaPagoDocumentos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            
            CargarCombosDetalle();
            CargarGrilla();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() > 0 )
            {
                if (grdData.ActiveRow.Cells["i_IdFormaPago"].Value.ToString() != "-1" && grdData.ActiveRow.Cells["i_CodigoDocumento"].Value.ToString() != "-1")
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["Relacion"].Value = Resource.arrow_right;
                    row.Cells["i_IdFormaPago"].Value = "-1";
                    row.Cells["i_CodigoDocumento"].Value = "-1";
                }
            }
            else
            {
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["Relacion"].Value = Resource.arrow_right;
                row.Cells["i_IdFormaPago"].Value = "-1";
                row.Cells["i_CodigoDocumento"].Value = "-1";
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdFormaPago"].EditorComponent = ucFormaPago;
            e.Layout.Bands[0].Columns["i_IdFormaPago"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_CodigoDocumento"].EditorComponent = ucDocumento;
            e.Layout.Bands[0].Columns["i_CodigoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _relacionformapagodocumentoDto = new relacionformapagodocumentoDto();
                    _relacionformapagodocumentoDto.i_IdRelacion = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdRelacion"].Value.ToString());
                    _TempDetalle_EliminarDto.Add(_relacionformapagodocumentoDto);
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                    btnEliminarDetalle.Enabled = false;
                }
            }
            else
            {
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                btnEliminarDetalle.Enabled = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarNulosVacios() == true)
            {
                LlenarTemporalesDiario();
                OperationResult objOperationResult = new OperationResult();
                _objFormaPagoDocumentoBL.ActualizarRelacion(ref objOperationResult, _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, Globals.ClientSession.GetAsList());
                _TempDetalle_AgregarDto = new List<relacionformapagodocumentoDto>();
                _TempDetalle_ModificarDto = new List<relacionformapagodocumentoDto>();
                _TempDetalle_EliminarDto = new List<relacionformapagodocumentoDto>();

                if (objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrilla();
                }
                else
                {
                    if (objOperationResult.ErrorMessage == null)
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            e.Cell.Row.Cells["i_RegistroEstado"].Value = "Modificado";


        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        #region Clases/Validaciones
        void CargarGrilla()
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objFormaPagoDocumentoBL.ObtenerRelaciones(ref objOperationResult);
            grdData.Rows.ToList().ForEach(p => p.Cells["i_RegistroTipo"].Value = "NoTemporal");
            grdData.Rows.ToList().ForEach(p => p.Cells["Relacion"].Value = Resource.arrow_right);
        }

        void CargarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();

            #region Configura Combo Forma Pago
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 267;
            ultraGridColumnaID.Hidden = true;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID });
            ucFormaPago.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucFormaPago.DropDownWidth = 270;
            ucFormaPago.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            #region Configura Combo Documento
            UltraGridBand _ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            _ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            _ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            _ultraGridColumnaDescripcion.Width = 327;
            _ultraGridColumnaID.Hidden = true;
            _ultraGridBanda.Columns.AddRange(new object[] { _ultraGridColumnaDescripcion, _ultraGridColumnaID });
            ucDocumento.DisplayLayout.BandsSerializer.Add(_ultraGridBanda);
            ucDocumento.DropDownWidth = 330;
            #endregion

            ucFormaPago.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucFormaPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 46, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null), DropDownListAction.Select);
        }

        void LlenarTemporalesDiario()
        {
            if (grdData.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _relacionformapagodocumentoDto = new relacionformapagodocumentoDto();
                                _relacionformapagodocumentoDto.i_IdRelacion = Fila.Cells["i_IdRelacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdRelacion"].Value.ToString());
                                _relacionformapagodocumentoDto.i_IdFormaPago = Fila.Cells["i_IdFormaPago"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString());
                                _relacionformapagodocumentoDto.i_CodigoDocumento = Fila.Cells["i_CodigoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_CodigoDocumento"].Value.ToString());

                                _TempDetalle_AgregarDto.Add(_relacionformapagodocumentoDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _relacionformapagodocumentoDto = new relacionformapagodocumentoDto();
                                _relacionformapagodocumentoDto.i_IdRelacion = Fila.Cells["i_IdRelacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdRelacion"].Value.ToString());
                                _relacionformapagodocumentoDto.i_IdFormaPago = Fila.Cells["i_IdFormaPago"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString());
                                _relacionformapagodocumentoDto.i_CodigoDocumento = Fila.Cells["i_CodigoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_CodigoDocumento"].Value.ToString());
                                _relacionformapagodocumentoDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _relacionformapagodocumentoDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _TempDetalle_ModificarDto.Add(_relacionformapagodocumentoDto);
                            }
                            break;
                    }
                }
            }

        }

        bool ValidarNulosVacios()
        {
            if (grdData.Rows.Where(p => p.Cells["i_IdFormaPago"].Value.ToString() == "-1").Count() > 0)
            {
                UltraGridRow Fila = grdData.Rows.Where(p => p.Cells["i_IdFormaPago"].Value.ToString() == "-1").FirstOrDefault();
                Fila.Activate();
                grdData.ActiveRow.Selected = true;
                UltraMessageBox.Show("Hay formas de pago sin seleccionar.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            List<string> FormasPago = grdData.Rows.Where(o => o.Cells["i_IdFormaPago"].Value != null).Select(p => p.Cells["i_IdFormaPago"].Value.ToString()).Distinct().ToList();
            
            foreach (string FormaPago in FormasPago)
            {
                if (grdData.Rows.Where(p => p.Cells["i_IdFormaPago"].Value.ToString() == FormaPago).Count() > 1)
                {
                    UltraMessageBox.Show("Las formas de pago no se pueden repetir.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }    
            }

            return true;
        }
        #endregion

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell.Column.Key == "i_CodigoDocumento" && grdData.ActiveCell.Value.ToString() != "-1")
            {
                if (!_objDocumentoBL.TieneCuentaValida(int.Parse(grdData.ActiveCell.Value.ToString())))
                {
                    UltraMessageBox.Show("El documento con el que intenta pagar no tiene una cuenta contable válida relacionada. \nModifique la información del documento e ingrésele una cuenta válida.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    grdData.ActiveCell.Value = "-1";
                    return;
                }
            }
        }

    }
}
