using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmMenus : Form
    {
        readonly ApplicationHierarchyBL _objApplicationHierarchyBl = new ApplicationHierarchyBL();
        private List<UltraGridRow> _resultadoBusqueda = new List<UltraGridRow>();
        private int _Index;
        public frmMenus(string n)
        {
            InitializeComponent();
        }

        private void frmMenus_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarMenus();
        }

        void CargarMenus(bool expandido = false)
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();

                var menusDs = _objApplicationHierarchyBl.ObtenerMenus(ref objOperationResult);

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (menusDs == null || menusDs.Count <= 0) return;
                    var nivel = 0;
                    grdData.AllowDrop = true;
                    grdData.DataSource = menusDs;
                    grdData.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
                    grdData.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnDisplay;
                    grdData.Rows.ExpandAll(expandido);

                    foreach (UltraGridBand banda in grdData.DisplayLayout.Bands)
                    {
                        if (nivel == 0 || nivel == 1 || nivel == 2)
                        {
                            if (nivel == 0)
                            {
                                banda.Columns["Descripcion"].CellAppearance.FontData.SizeInPoints = 15;
                            }

                            if (nivel == 1)
                            {
                                banda.Columns["Descripcion"].CellAppearance.FontData.SizeInPoints = 12;
                            }

                            banda.Columns["Descripcion"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                        }

                        banda.Columns["Descripcion"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                        banda.Columns["Descripcion"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                        banda.Columns["Id"].Hidden = true;
                        banda.Columns["Form"].Hidden = true;
                        if (nivel > 0) banda.ColHeadersVisible = false;
                        nivel++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right || grdData.ActiveRow == null) return;
            var applicationHierarchyId = int.Parse(grdData.ActiveRow.Cells["Id"].Value.ToString());
            var nombreMenu = grdData.ActiveRow.Cells["Descripcion"].Value.ToString();
            ucMenuPopup popup = new ucMenuPopup(applicationHierarchyId, nombreMenu);
            upccPopupMenu.PopupControl = popup;
            upccPopupMenu.Show();
        }

        public void RealizarAccionesMenu(AccionesMenu oAccionesMenu, int IdApplicationHierarchy = 0)
        {
            upccPopupMenu.Close();
            switch (oAccionesMenu)
            {
                case AccionesMenu.Modificar:
                    var value = grdData.ActiveRow.Cells["Id"].Value;
                    if (value != null)
                    {
                        var id = int.Parse(value.ToString());
                        frmMantenimientoMenu f = new frmMantenimientoMenu(id, frmMantenimientoMenu.TipoMantenimiento.Editar);
                        f.ShowDialog();
                    }
                    break;

                case AccionesMenu.Clonar:
                    break;

                case AccionesMenu.Eliminar:
                    if (grdData.ActiveRow != null)
                    {
                        OperationResult objOperationResult = new OperationResult();
                        var menu = grdData.ActiveRow.Cells["Descripcion"].Value.ToString();
                        var mensaje = string.Format("¿Está seguro de eliminar el menú {0}?", menu);
                        if (MessageBox.Show(mensaje, @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            _objApplicationHierarchyBl.DeleteApplicationHierarchy(ref objOperationResult, IdApplicationHierarchy, Globals.ClientSession.GetAsList());

                            if (objOperationResult.Success == 0)
                            {
                                MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                var scrollPos = grdData.ActiveRowScrollRegion.ScrollPosition;
                                CargarMenus(true);
                                grdData.ActiveRowScrollRegion.ScrollPosition = scrollPos;
                            }
                        }
                    }
                    break;

                case AccionesMenu.AgregarMenuHijo:
                    ultraButton1.PerformClick();
                    break;

                case AccionesMenu.ColapsarMenusHijos:
                    if (grdData.ActiveRow != null) grdData.ActiveRow.CollapseAll();
                    break;

                case AccionesMenu.ExpanderMenusHijos:
                    if (grdData.ActiveRow != null) grdData.ActiveRow.ExpandAll();
                    break;

                case AccionesMenu.Refrescar:
                    var _scrollPos = grdData.ActiveRowScrollRegion.ScrollPosition;
                    CargarMenus();
                    grdData.ActiveRowScrollRegion.ScrollPosition = _scrollPos;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("oAccionesMenu", oAccionesMenu, null);
            }
        }

        public enum AccionesMenu
        {
            Modificar = 1,
            Clonar = 2,
            Eliminar = 3,
            AgregarMenuHijo = 4,
            ExpanderMenusHijos = 5,
            ColapsarMenusHijos = 6,
            Refrescar = 7
        }

        private void btnContraer_Click(object sender, EventArgs e)
        {
            grdData.Rows.ExpandAll(true);
        }

        private void btnColapsar_Click(object sender, EventArgs e)
        {
            grdData.Rows.CollapseAll(true);
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            int Id = int.Parse(grdData.ActiveRow.Cells["Id"].Value.ToString());
            frmMantenimientoMenu f = new frmMantenimientoMenu(Id);
            f.ShowDialog();
        }

        private void grdData_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void grdData_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            UltraGrid grid = sender as UltraGrid;
            Point pointInGridCoords = grid.PointToClient(new Point(e.X, e.Y));

            if (pointInGridCoords.Y < 20)
                // Scroll up
                this.grdData.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
            else if (pointInGridCoords.Y > grid.Height - 20)
                // Scroll down
                this.grdData.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);
        }

        private void grdData_DragDrop(object sender, DragEventArgs e)
        {
            UIElement uieOver = grdData.DisplayLayout.UIElement.ElementFromPoint(grdData.PointToClient(new Point(e.X, e.Y)));
            UltraGridRow ugrOver = uieOver.GetContext(typeof (UltraGridRow), true) as UltraGridRow;

            if (ugrOver == null || e.Data.GetData(typeof (SelectedRowsCollection)) == null) return;
            var dropIndexRow = ugrOver.Index;
            var SelRows = (SelectedRowsCollection) e.Data.GetData(typeof (SelectedRowsCollection));
            foreach (UltraGridRow aRow in SelRows)
            {
                grdData.Rows.Move(aRow, dropIndexRow);
            }
        }

        private void grdData_SelectionDrag(object sender, CancelEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            grdData.ActiveRow.Selected = true;
            grdData.DoDragDrop(grdData.ActiveRow, DragDropEffects.Move);
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var criterio = txtBuscar.Text.Trim().ToLower();
                var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
                _resultadoBusqueda = filas.Where(p => p.Cells["Descripcion"].Value.ToString().ToLower().Contains(criterio)).ToList();

                if (_resultadoBusqueda.Any())
                {
                    if (_resultadoBusqueda.Count == 1)
                    {
                        txtBuscar.ButtonsRight[0].Enabled = false;
                        txtBuscar.ButtonsRight[1].Enabled = false;
                    }
                    else
                    {
                        txtBuscar.ButtonsRight[1].Enabled = true;
                    }

                    _Index = 0;
                    _resultadoBusqueda[_Index].Activate();

                    grdData.Selected.Rows.Clear();
                    _resultadoBusqueda[_Index].Selected = true;
                }
                else
                {
                    txtBuscar.ButtonsRight[0].Enabled = false;
                    txtBuscar.ButtonsRight[1].Enabled = false;
                }
            }
        }

        private void txtBuscar_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            switch (e.Button.Key)
            {
                case "btnSiguiente":
                    if (_Index < _resultadoBusqueda.Count - 1)
                    {
                        e.Button.Enabled = true;
                        txtBuscar.ButtonsRight["btnAnterior"].Enabled = true;
                        _Index++;
                        _resultadoBusqueda[_Index].Activate();
                        grdData.Selected.Rows.Clear();
                        _resultadoBusqueda[_Index].Selected = true;
                    }
                    else
                        e.Button.Enabled = false;
                    break;
                    
                case "btnAnterior":
                    if (_Index > 0)
                    {
                        e.Button.Enabled = true;
                        txtBuscar.ButtonsRight["btnSiguiente"].Enabled = true;
                        _Index--;
                        _resultadoBusqueda[_Index].Activate();
                        grdData.Selected.Rows.Clear();
                        _resultadoBusqueda[_Index].Selected = true;
                    }
                    else
                        e.Button.Enabled = false;
                    break;
            }
        }
    }
}
