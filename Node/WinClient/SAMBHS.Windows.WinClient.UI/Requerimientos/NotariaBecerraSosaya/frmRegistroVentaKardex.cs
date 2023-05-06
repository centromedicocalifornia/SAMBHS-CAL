using Infragistics.Win.UltraWinGrid;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;
using Infragistics.Win;
using SAMBHS.CommonWIN.BL;
namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmRegistroVentaKardex : Form
    {

        public string strModo = "";
        public string pstrIdVenta = String.Empty;
        VentaBL _objVentaBL = new VentaBL();
        public BindingList<nbs_ventakardexDto> ListaGrilla = new BindingList<nbs_ventakardexDto>();
        BindingList<nbs_ventakardexDto> ListaTemporal = new BindingList<nbs_ventakardexDto>();
        nbs_ventakardexDto objKardex = new nbs_ventakardexDto();
        public List<nbs_ventakardexDto> _TempDetalle_EliminarDto = new List<nbs_ventakardexDto>();
        public decimal Total;
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        object sender = new object();
        EventArgs e = new EventArgs();
        List<string> ListaTipoKardex = new List<string>();
        UltraCombo ucTiposKardex = new UltraCombo();
        public frmRegistroVentaKardex(string IdVenta, BindingList<nbs_ventakardexDto> lListaTemporal, string pstrModo, List<nbs_ventakardexDto> _TempDetalle_EliminarDtos)
        {
            InitializeComponent();
            pstrIdVenta = IdVenta;
            strModo = pstrModo;
            ListaTemporal = lListaTemporal;
            _TempDetalle_EliminarDto = _TempDetalle_EliminarDtos;
        }

        private void CargarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();


            #region Configura Combo Tipo Kardex
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Value2");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 267;
            ultraGridColumnaDescripcion.Hidden = false;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID });
            ucTiposKardex.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucTiposKardex.DropDownWidth = 270;
            ucTiposKardex.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion
            ucTiposKardex.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucTiposKardex, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 153, null), DropDownListAction.Select);

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var primeraFila = grdDataKardex.Rows.FirstOrDefault();
            var tipoKardex = primeraFila != null ? primeraFila.Cells["v_TipoKardex"].Value.ToString() : string.Empty;
            var ultimaFila = grdDataKardex.Rows.LastOrDefault();
            if (ultimaFila == null || (ultimaFila.Cells["v_TipoKardex"].Value != null))
            {
                var row = grdDataKardex.DisplayLayout.Bands[0].AddNew();
                grdDataKardex.Rows.Move(row, grdDataKardex.Rows.Count() - 1);
                this.grdDataKardex.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["d_Monto"].Value = "0";
                row.Cells["v_TipoKardex"].Value = tipoKardex;
                var celDefecto = grdDataKardex.Rows.Count == 1 ? "v_TipoKardex" : "v_NroKardex";
                var aCell = grdDataKardex.ActiveRow.Cells[celDefecto];
                grdDataKardex.Focus();
                aCell.Activate();
                grdDataKardex.PerformAction(UltraGridAction.EnterEditMode);
            }
        }

        private void frmRegistroVentaKardex_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ObtenerListaDetalleKardex();
            ListaTipoKardex.Add("K");
            ListaTipoKardex.Add("C");
            ListaTipoKardex.Add("P");
            ListaTipoKardex.Add("D");
            ListaTipoKardex.Add("L");
            ListaTipoKardex.Add("E");
            ListaTipoKardex.Add("N");
            ListaTipoKardex.Add("H");
            ListaTipoKardex.Add("M");
            ListaTipoKardex.Add("T");

            //var primeraFila = grdDataKardex.Rows.FirstOrDefault();
            //var tipoKardex = primeraFila != null ? primeraFila.Cells["v_TipoKardex"].Value.ToString() : string.Empty;
            //var ultimaFila = grdDataKardex.Rows.LastOrDefault();
            //if (ultimaFila == null || (ultimaFila.Cells["v_TipoKardex"].Value != null))
            //{
            //    var row = grdDataKardex.DisplayLayout.Bands[0].AddNew();
            //    grdDataKardex.Rows.Move(row, grdDataKardex.Rows.Count() - 1);
            //    this.grdDataKardex.ActiveRowScrollRegion.ScrollRowIntoView(row);
            //    row.Cells["i_RegistroEstado"].Value = "Agregado";
            //    row.Cells["i_RegistroTipo"].Value = "Temporal";
            //    row.Cells["d_Monto"].Value = "0";
            //    row.Cells["v_TipoKardex"].Value = tipoKardex;
            //    var celDefecto = grdDataKardex.Rows.Count == 1 ? "v_TipoKardex" : "v_NroKardex";
            //    var aCell = grdDataKardex.ActiveRow.Cells[celDefecto];
            //    aCell.Activate();
            //    grdDataKardex.ActiveRow = aCell.Row;
            //    grdDataKardex.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            //    aCell.Activate();
            //    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
            //}

            // grdDataKardex.Focus();
        }

        private void ObtenerListaDetalleKardex()
        {
            switch (strModo)
            {
                case "Edicion":
                    CargarDetalleKardex(pstrIdVenta);

                    break;
                case "Nuevo":
                    CargarDetalleKardex("");

                    break;
                case "Guardado":
                    CargarDetalleKardex(pstrIdVenta);

                    break;

            }


        }

        private void CargarDetalleKardex(string IdVenta)
        {

            OperationResult objOperationResult = new OperationResult();
            if (ListaTemporal.Any())
            {
                grdDataKardex.DataSource = ListaTemporal;

            }
            else
            {
                grdDataKardex.DataSource = _objVentaBL.ObtenerDetalleKardex(ref objOperationResult, IdVenta);
                if (grdDataKardex.Rows.Any())
                {
                    for (int i = 0; i < grdDataKardex.Rows.Count(); i++)
                    {
                        grdDataKardex.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                        grdDataKardex.Rows[i].Cells["i_RegistroEstado"].Value = "NoModificado";
                    }

                }
            }
            CalcularTotales();

            if (grdDataKardex.Rows.Any()) return;

            //btnAgregar_Click(sender, e);

        }

        private void grdDataKardex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdDataKardex.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdDataKardex.ActiveCell.Column.Key)
                {
                    case "v_TipoKardex":
                        //if (e.KeyChar == 'H'  || e.KeyChar == 'h'  || e.KeyChar == 8 || e.KeyChar == 'E' || e.KeyChar == 'e' || e.KeyChar == 'M' || e.KeyChar == 'm' || e.KeyChar == 'K' || e.KeyChar == 'k' || e.KeyChar == 'R' || e.KeyChar == 'r')
                        //// char.IsLetterOrDigit(e.KeyChar)
                        //{
                        //    e.Handled = false;
                        //}
                        //else
                        //{
                        //    e.Handled = true;
                        //}

                        foreach (var item in ListaTipoKardex)
                        {
                            if (e.KeyChar.ToString().ToUpper() == item || e.KeyChar.ToString().ToUpper() == "V" || e.KeyChar == 8)
                            {
                                e.Handled = false;
                                return;
                            }
                            else
                            {
                                e.Handled = true;
                            }

                        }

                        break;

                    case "d_Monto":
                        //Celda = grdDataKardex.ActiveCell;
                        //Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;
                }
            }
        }

        private void frmRegistroVentaKardex_FormClosing(object sender, FormClosingEventArgs e)
        {
            int  Ingreso = 1;
            foreach (var fila1 in grdDataKardex.Rows)
            {
                foreach (var fila2 in grdDataKardex.Rows)
                {
                    if (fila2.Cells["v_TipoKardex"].Value != null && fila2.Cells["v_NroKardex"].Value!=null && fila1.Cells["v_TipoKardex"].Value != null && fila1.Cells["v_NroKardex"].Value!=null )


                        if (  Ingreso ==1 && fila1.Index != fila2.Index && fila1.Cells["v_TipoKardex"].Value.ToString () == fila2.Cells["v_TipoKardex"].Value.ToString() &&  fila1.Cells["v_NroKardex"].Value.ToString() == fila2.Cells["v_NroKardex"].Value.ToString())
                        {
                            UltraMessageBox.Show("El Kardex no se puede repetir. : Items : " + (fila1.Index + 1).ToString ("00")+ "-"+ (fila2.Index +1).ToString ("00"), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Ingreso = 0;
                        }

                }

            }
            
            CalcularTotales();
            var filasVacias = grdDataKardex.Rows.Where(fila => fila.Cells["v_TipoKardex"].Value == null ||
                fila.Cells["v_NroKardex"].Value == null ||
                string.IsNullOrWhiteSpace(fila.Cells["v_TipoKardex"].Value.ToString()) ||
                string.IsNullOrWhiteSpace(fila.Cells["v_NroKardex"].Value.ToString())).ToList();
            filasVacias.ForEach(fila => fila.Delete(false));
            ListaGrilla = new BindingList<nbs_ventakardexDto>();
            foreach (var fila in grdDataKardex.Rows)
            {
                var objFila = (nbs_ventakardexDto)fila.ListObject;
                ListaGrilla.Add(objFila);
            }

            Total = decimal.Parse(txtTotal.Text);
        }


        private bool ValidaCamposNulosVacios()
        {
            if (grdDataKardex.Rows.Count(p => p.Cells["v_TipoKardex"].Value == null || p.Cells["v_TipoKardex"].Value.ToString().Trim() == string.Empty) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente tipo de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataKardex.Rows.FirstOrDefault(x => x.Cells["v_TipoKardex"].Value == null || x.Cells["v_TipoKardex"].Value.ToString().Trim() == string.Empty);
                grdDataKardex.Selected.Cells.Add(Row.Cells["v_TipoKardex"]);
                grdDataKardex.Focus();
                Row.Activate();
                grdDataKardex.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataKardex.ActiveRow.Cells["v_TipoKardex"];
                this.grdDataKardex.ActiveCell = aCell;
                return false;
            }

            if (grdDataKardex.Rows.Count(p => p.Cells["v_NroKardex"].Value == null || p.Cells["v_NroKardex"].Value.ToString().Trim() == string.Empty) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Número de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataKardex.Rows.FirstOrDefault(x => x.Cells["v_NroKardex"].Value == null || x.Cells["v_NroKardex"].Value.ToString().Trim() == string.Empty);
                grdDataKardex.Selected.Cells.Add(Row.Cells["v_NroKardex"]);
                grdDataKardex.Focus();
                Row.Activate();
                grdDataKardex.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataKardex.ActiveRow.Cells["v_NroKardex"];
                this.grdDataKardex.ActiveCell = aCell;
                return false;
            }

            return true;
        }

        private void grdDataKardex_CellChange(object sender, CellEventArgs e)
        {
            EmbeddableEditorBase editor = e.Cell.EditorResolved;
            if (editor.Value != null && string.IsNullOrEmpty(editor.Value.ToString())) return;
            e.Cell.Value = editor.Value;
            grdDataKardex.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grdDataKardex_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdDataKardex.ActiveCell == null) return;


            //if (grdDataKardex.ActiveCell.Column.Key != "v_TipoKardex")
            //{

            switch (e.KeyCode)
            {
                case Keys.Up:
                    grdDataKardex.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grdDataKardex.PerformAction(UltraGridAction.AboveCell, false, false);
                    e.Handled = true;
                    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Down:
                    grdDataKardex.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grdDataKardex.PerformAction(UltraGridAction.BelowCell, false, false);
                    e.Handled = true;
                    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Right:
                    grdDataKardex.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grdDataKardex.PerformAction(UltraGridAction.NextCellByTab, false, false);
                    e.Handled = true;
                    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Left:
                    grdDataKardex.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grdDataKardex.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                    e.Handled = true;
                    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;


                case Keys.Enter:
                    DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdDataKardex.ActiveCell);
                    grdDataKardex_DoubleClickCell(sender, eventos);
                    e.Handled = true;
                    break;
            }
            //}
            //else
            //{

            //    switch (e.KeyCode)
            //    {
            //        case Keys.Right:
            //            grdDataKardex.PerformAction(UltraGridAction.ExitEditMode, false, false);
            //            grdDataKardex.PerformAction(UltraGridAction.NextCellByTab, false, false);
            //            e.Handled = true;
            //            grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
            //            break;
            //        case Keys.Left:
            //            grdDataKardex.PerformAction(UltraGridAction.ExitEditMode, false, false);
            //            grdDataKardex.PerformAction(UltraGridAction.PrevCellByTab, false, false);
            //            e.Handled = true;
            //            grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
            //            break;
            //    }
            //}
        }

        private void grdDataKardex_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));
            if (row == null)
            {

                btnEliminar.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;

            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdDataKardex.ActiveRow == null) return;
            if (grdDataKardex.Rows[grdDataKardex.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    objKardex = new nbs_ventakardexDto();
                    objKardex.v_IdVentaKardex = grdDataKardex.Rows[grdDataKardex.ActiveRow.Index].Cells["v_IdVentaKardex"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(objKardex);
                    grdDataKardex.Rows[grdDataKardex.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                grdDataKardex.Rows[grdDataKardex.ActiveRow.Index].Delete(false);
            }

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
           
        }

        private void frmRegistroVentaKardex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                string x = "";
            }
        }

        private void grdDataKardex_AfterExitEditMode(object sender, EventArgs e)
        {

            CalcularTotales();
        }
        private void CalcularTotales()
        {
            decimal sumaMonto = 0;
            foreach (UltraGridRow Fila in grdDataKardex.Rows)
            {
                if (Fila.Cells["d_Monto"].Value == null) { Fila.Cells["d_Monto"].Value = "0"; };

                sumaMonto = sumaMonto + (Fila.Cells["d_Monto"].Value == null || string.IsNullOrEmpty(Fila.Cells["d_Monto"].Value.ToString()) ? 0 : decimal.Parse(Fila.Cells["d_Monto"].Value.ToString()));
                txtTotal.Text = Utils.Windows.DevuelveValorRedondeado(sumaMonto, 2).ToString();
                
            }
        }
        private void grdDataKardex_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (this.grdDataKardex.ActiveCell == null || this.grdDataKardex.ActiveCell.Column.Key == null) return;
            switch (this.grdDataKardex.ActiveCell.Column.Key)
            {
                case "v_TipoKardex":

                    if (grdDataKardex.Rows[grdDataKardex.ActiveRow.Index].Cells["v_TipoKardex"].Value != null && grdDataKardex.Rows[grdDataKardex.ActiveRow.Index].Cells["v_TipoKardex"].Value.ToString() != string.Empty)
                    {

                        //UltraGridCell aCell = this.grdDataKardex.ActiveRow.Cells["v_NroKardex"];
                        //this.grdDataKardex.ActiveCell = aCell;
                        //grdDataKardex.Focus();
                        //grdDataKardex.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        //grdDataKardex.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    }
                    break;

            }
        }

        private void grdDataKardex_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Item"].Value = (e.Row.Index + 1).ToString("00");
        }

        private void grdDataKardex_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            //e.Layout.Bands[0].Columns["v_TipoKardex"].EditorComponent = ucTiposKardex;
            //e.Layout.Bands[0].Columns["v_TipoKardex"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void grdDataKardex_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            int Index = grdDataKardex != null && grdDataKardex.ActiveRow != null ? grdDataKardex.ActiveRow.Index : 1;
            var TipoKardexGrilla = grdDataKardex != null && grdDataKardex.ActiveRow != null & grdDataKardex.ActiveRow.Cells["v_TipoKardex"].Value != null ? grdDataKardex.ActiveRow.Cells["v_TipoKardex"].Value.ToString() : "";
            var NroKardexGrilla = grdDataKardex != null && grdDataKardex.ActiveRow != null & grdDataKardex.ActiveRow.Cells["v_NroKardex"].Value != null ? grdDataKardex.ActiveRow.Cells["v_NroKardex"].Value.ToString() : "";
            switch (e.Cell.Column.Key)
            {
                case "v_TipoKardex":

                    foreach (UltraGridRow Fila in grdDataKardex.Rows)
                    {
                        if (Fila.Cells["v_TipoKardex"].Value != null)
                        {

                            if (Index != Fila.Index && TipoKardexGrilla == Fila.Cells["v_TipoKardex"].Value.ToString() && NroKardexGrilla == Fila.Cells["v_NroKardex"].Value.ToString())
                            {
                                UltraMessageBox.Show("El Kardex ya existe en el detalle. Ver Item : " +( Fila.Index +1).ToString ("00"), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    grdDataKardex.ActiveRow.Cells["v_NroKardex"].Activate();
                    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode);

                    break;
                case "v_NroKardex":


                    foreach (UltraGridRow Fila in grdDataKardex.Rows)
                    {
                        if (Fila.Cells["v_NroKardex"].Value != null)
                        {

                            if (Index != Fila.Index && TipoKardexGrilla == Fila.Cells["v_TipoKardex"].Value.ToString() && NroKardexGrilla == Fila.Cells["v_NroKardex"].Value.ToString())
                            {
                                UltraMessageBox.Show("El Kardex ya existe en el detalle .Ver Item : " +( Fila.Index +1).ToString ("00"), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }



                    grdDataKardex.ActiveRow.Cells["d_Monto"].Activate();
                    grdDataKardex.PerformAction(UltraGridAction.EnterEditMode);

                    break;

                case "d_Monto":
                    btnAgregar_Click(sender, e);
                    break;
            }
        }



    }
}
