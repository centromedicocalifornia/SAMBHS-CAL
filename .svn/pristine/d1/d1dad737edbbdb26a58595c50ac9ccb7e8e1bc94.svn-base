using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmPermisos : Form
    {
        readonly SecurityBL _objSecurityBl = new SecurityBL();
        readonly ApplicationHierarchyBL _objApplicationHierarchyBl = new ApplicationHierarchyBL();
        readonly int _rolId;

        public frmPermisos(int pintIdRol)
        {
            InitializeComponent();
            _rolId = pintIdRol;
        }

        private void frmPermisos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarMenus();
            ObtenerData();
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
                    grdData.DataSource = menusDs;
                    grdData.DisplayLayout.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
                    grdData.Rows.ExpandAll(expandido);

                    foreach (UltraGridBand banda in grdData.DisplayLayout.Bands)
                    {
                        if (nivel == 0 || nivel == 1 || nivel == 2)
                        {
                            if (nivel == 0)
                            {
                                banda.Columns["Descripcion"].CellAppearance.FontData.SizeInPoints = 10;
                                banda.Columns["Descripcion"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                            }

                            if (nivel == 1)
                            {
                                banda.Columns["Descripcion"].CellAppearance.FontData.SizeInPoints = 10;
                            }
                        }
                        banda.Columns.Add("_Check", "");
                        banda.Columns.Add("_Modificado", "");
                        banda.Columns["_Modificado"].DataType = typeof(bool);
                        banda.Columns["_Modificado"].Hidden = true;
                        banda.Columns["_Check"].Style = ColumnStyle.CheckBox;
                        banda.Columns["_Check"].DataType = typeof(bool);
                        banda.Columns["_Check"].Header.VisiblePosition = 1;
                        banda.Columns["_Check"].MinWidth = 7;
                        banda.Columns["_Check"].Width = 7;
                        banda.Columns["Descripcion"].Header.VisiblePosition = 2;
                        banda.Columns["Descripcion"].CellActivation = Activation.ActivateOnly;
                        banda.Columns["Descripcion"].CellClickAction = CellClickAction.RowSelect;
                        banda.Columns["Id"].Hidden = true;
                        banda.Columns["Form"].Hidden = true;
                        if (nivel > 0) banda.ColHeadersVisible = false;
                        nivel++;
                    }

                    if (Globals.ClientSession.i_RoleId != 1)
                    {
                        BloquearModuloSeguridad();
                    }
                }

                foreach (UltraGridRow oFila in grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null))
                {
                    oFila.Cells["_Check"].Value = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell != null)
            {
                var editor = e.Cell.EditorResolved;
                e.Cell.Value = editor.Value;
                e.Cell.Row.Cells["_Modificado"].Value = true;
                ActualizarPadres(e.Cell.Row);
                ActualizaHijos(e.Cell.Row);
            }
        }

        private void ActualizarPadres(UltraGridRow fila)
        {
            var filaPadre = fila.ParentRow;
            if (filaPadre == null) return;

            var filasHijas = fila.Band.GetRowEnumerator(GridRowType.DataRow).Cast<UltraGridRow>().ToList().Where(o => o.ParentRow == filaPadre);
            filaPadre.Cells["_Check"].Value = filasHijas.Any(p => Convert.ToBoolean(p.Cells["_Check"].Value.ToString()));
            filaPadre.Cells["_Modificado"].Value = true;
            ActualizarPadres(filaPadre);
        }

        private void ActualizaHijos(UltraGridRow fila)
        {
            var filaHija = fila.GetChild(ChildRow.First);
            if (filaHija == null) return;

            var filasHijas = filaHija.Band.GetRowEnumerator(GridRowType.DataRow).Cast<UltraGridRow>().ToList().Where(o => o.ParentRow == fila).ToList();
            var estadoPadre = Convert.ToBoolean(fila.Cells["_Check"].Value.ToString());

            filasHijas
                .ForEach(f =>
                {
                    f.Cells["_Check"].Value = estadoPadre;
                    f.Cells["_Modificado"].Value = true;
                    ActualizaHijos(f);
                });
        }

        private void ObtenerData()
        {
            OperationResult objCommonOperationResultGlobal = new OperationResult();
            var objGlobalAuthorization = _objSecurityBl.GetRoleProfiles(ref objCommonOperationResultGlobal, _rolId);
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var ids = objGlobalAuthorization.Select(p => p.I_ApplicationHierarchyId).ToList();

            filas.ForEach(fila =>
            {
                var id = int.Parse(fila.Cells["Id"].Value.ToString());
                if (ids.Contains(id))
                {
                    fila.Cells["_Check"].Value = true;
                }
            });
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            #region Recopilación de data 
            OperationResult objOperationResult = new OperationResult();
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            filas = filas.Where(p => Convert.ToBoolean(p.Cells["_Modificado"].Value.ToString())).ToList();
            var filasActualizar = filas.Where(p => Convert.ToBoolean(p.Cells["_Check"].Value.ToString())).ToList();
            var filasEliminar = filas.Where(p => !Convert.ToBoolean(p.Cells["_Check"].Value.ToString())).ToList();

            var listaActualizar = filasActualizar.Select(p => new roleprofileDto
            {
                i_ApplicationHierarchyId = int.Parse(p.Cells["Id"].Value.ToString()),
                i_RoleId = _rolId
            }).ToList();

            var listaEliminar = filasEliminar.Select(p => new roleprofileDto
            {
                i_ApplicationHierarchyId = int.Parse(p.Cells["Id"].Value.ToString()),
                i_RoleId = _rolId
            }).ToList(); 
            #endregion

            _objSecurityBl.UpdateRoleProfiles(ref objOperationResult, listaActualizar, listaEliminar, Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Close();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BloquearModuloSeguridad()
        {
            var filaSeguridadPadre = grdData.Rows.FirstOrDefault(p => p.Cells["Id"].Value.ToString() == "9");
            if (filaSeguridadPadre != null)
            {
                filaSeguridadPadre.Activation = Activation.Disabled;
                filaSeguridadPadre.CollapseAll();
                filaSeguridadPadre.ExpansionIndicator = ShowExpansionIndicator.Never;
            }
        }
    }
}
