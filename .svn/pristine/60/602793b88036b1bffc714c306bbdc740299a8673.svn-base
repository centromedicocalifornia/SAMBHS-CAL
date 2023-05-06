using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls
{
    public partial class ucMantenimientoHierarchy : UserControl
    {
        #region VARIABLES
        DatahierarchyBL _objDataHierarchyBL = new DatahierarchyBL();
        datahierarchyDto _datahierarchyDto;
        private string _modeDetalle;
        private int _pIntIdGroup;
        private int Value;
        private int GrupoRegimenLaboral = 115, GrupoOcupacion = 118, GrupoUnidadMedida = 17, GrupoUbicacion = 103, GrupoTipoActivo = 104, GrupoCentroCosto = 31, GrupoZona = 5,
        GrupoNotaEstadoSituacionFinanciera=172;

        #endregion

        public ucMantenimientoHierarchy(int pintGroupId, int Valor = 2)
        {
            InitializeComponent();
            this._pIntIdGroup = pintGroupId;
            this.Value = Valor;
            this.Load += delegate
            {
                grbDescripcion.Enabled = false;
                CargarGrillaDetalle();
                grdData.DoubleClickRow += btnEditar_Click;
                grdData.KeyDown += (sender, e) =>
                {
                    if (e.KeyCode == Keys.Enter && grdData.ActiveRow != null)
                        txtTitulo.Focus();
                };
            };
            txtCodigo.KeyPress += (sender, e) =>
            {
                //Utils.Windows.NumeroEnteroUltraTextBox(txtCodigo, e);             //comentado temporalmente, UM es alfanumerico
            };
            grdData.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnEditar_Click(null, null);
                else if (e.KeyCode == Keys.Delete) btnEliminar_Click(null, null);
            };
            lblValor2.Text = Value == 4 ? "Código Sunat : " : "Código:";
        }

        #region Evento Click Buttons
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (txtTitulo.Text.Trim() == "")
            {
                UltraMessageBox.Show("Por favor ingrese un Titulo.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }
            //if (_pIntIdGroup == 115 || _pIntIdGroup == 118)
            if (_pIntIdGroup == GrupoRegimenLaboral || _pIntIdGroup == GrupoOcupacion || _pIntIdGroup ==GrupoZona || _pIntIdGroup ==GrupoNotaEstadoSituacionFinanciera)
            {

            }
            else
            {
                if (txtCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Codigo", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigo.Focus();
                    return;
                }

                if (_pIntIdGroup == GrupoUnidadMedida)
                {
                    if (txtValor2.Text.Trim().Equals(""))
                    {
                        UltraMessageBox.Show("Por favor ingrese la equivalencia de la unidad Medida", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtValor2.Focus();
                        return;
                    }
                    else if (decimal.Parse (txtValor2.Text)<=0)
                    {
                        UltraMessageBox.Show("Por favor ingrese la equivalencia de la unidad Medida correcta (>0)", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtValor2.Focus();
                        return;
                    }


                }
                else
                {


                    if (ValidarCodigo(txtCodigo.Text) && (_modeDetalle.Equals("New") || !txtCodigo.Text.Trim().Equals(_datahierarchyDto.v_Value2)))
                    {
                        UltraMessageBox.Show("Por favor ingrese un Nuevo Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigo.Text = string.Empty;
                        txtCodigo.Focus();
                        return;
                    }
                }
                
            }

            if (_modeDetalle == "New")
            {
                _datahierarchyDto = new datahierarchyDto();

                var header = txtCodigo.Text.EndsWith("00") ? 1 : 0;
                int codigo = 0;
                int sort = 0;
                if (_pIntIdGroup == GrupoOcupacion || _pIntIdGroup == GrupoRegimenLaboral  || _pIntIdGroup == GrupoUnidadMedida || _pIntIdGroup ==GrupoZona|| _pIntIdGroup==GrupoNotaEstadoSituacionFinanciera)
                {
                }
                else
                {
                    try
                    {
                        codigo = int.Parse(txtCodigo.Text);
                        sort = (header == 1) ? (codigo / 100) : (codigo % 100);
                    }
                    catch (Exception ex)
                    {
                        sort = header == 1 ? 1 : 0;
                    }
                }


                _datahierarchyDto.i_GroupId = this._pIntIdGroup;
                _datahierarchyDto.i_ItemId = _objDataHierarchyBL.ObtenerMaxino(ref objOperationResult, _datahierarchyDto.i_GroupId);
                _datahierarchyDto.i_Sort = sort;
                _datahierarchyDto.v_Value1 = txtTitulo.Text;
                if (Value == 4)
                {
                    _datahierarchyDto.v_Value4 = txtCodigo.Text;
                }
                else
                {
                    _datahierarchyDto.v_Value2 = txtCodigo.Text;
                }
                if (_pIntIdGroup == GrupoUnidadMedida)
                {
                    _datahierarchyDto.v_Value2 = txtValor2.Text;
                }
                if (_pIntIdGroup == GrupoNotaEstadoSituacionFinanciera)
                {
                    _datahierarchyDto.v_Value2 = txtDescripcion.Text;
                    _datahierarchyDto.v_Field = txtCodigo.Text;
                }
                _datahierarchyDto.i_Header = header;

                // Save the data
                _objDataHierarchyBL.AddDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

            }
            else if (_modeDetalle == "Edit")
            {
                _datahierarchyDto.v_Value1 = txtTitulo.Text;
               // _datahierarchyDto.v_Value2 = txtCodigo.Text;

                if (Value == 4)
                {
                    _datahierarchyDto.v_Value4 = txtCodigo.Text;
                }
                else
                {
                    _datahierarchyDto.v_Value2 = txtCodigo.Text;
                }
                if (_pIntIdGroup == GrupoUnidadMedida)
                {
                    _datahierarchyDto.v_Value2 = txtValor2.Text;
                }
                if (_pIntIdGroup == GrupoNotaEstadoSituacionFinanciera)
                {
                    _datahierarchyDto.v_Value2 = txtDescripcion.Text;
                    _datahierarchyDto.v_Field = txtCodigo.Text;
                }

                // Save the data
                _objDataHierarchyBL.UpdateDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());
            }
            //// Analizar el resultado de la operación
            if (objOperationResult.Success == 1)  // Operación sin error
            {
                CargarGrillaDetalle();
                btnEliminar.Enabled = btnEditar.Enabled = false;
                grbDescripcion.Enabled = false;
                btnNuevo.Focus();
            }
            else  // Operación con error
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtTitulo.Text = string.Empty;
            txtCodigo.Text = string.Empty;
            grbDescripcion.Enabled = true;
            txtTitulo.Focus();
            this._modeDetalle = "New";
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.Selected.Rows.Count > 0)
            {
                int intIdItem = int.Parse(grdData.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());
                CargarDetalle(intIdItem);
                txtTitulo.Focus();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.Selected.Rows.Count > 0)
            {
                OperationResult objOperationResult = new OperationResult();
                bool isHeader = false;
                int intCodigo = 0;
                if (_pIntIdGroup == GrupoRegimenLaboral || _pIntIdGroup == GrupoOcupacion)
                {
                }
                else
                {
                    intCodigo = int.Parse(grdData.Selected.Rows[0].Cells["v_Value2"].Value.ToString());

                    string mensajeDeAlerta;
                    isHeader = intCodigo % 100 == 0;

                    mensajeDeAlerta = (isHeader) ? ("Esta accion eliminara los hijos de " + grdData.Selected.Rows[0].Cells["v_Value1"].Value
                        + ", si existen.\n¿Desea Continuar?") : ("¿Esta seguro de eliminar el elemento con codigo " + intCodigo + " ?");

                    if (UltraMessageBox.Show(mensajeDeAlerta, "Eliminar Elemento(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        == DialogResult.No)
                        return;

                    this._datahierarchyDto = new datahierarchyDto();
                }
                if (isHeader)

                    
                {

                    //var intIdItems = (from n in grdData.Rows
                    //                  let cod = int.Parse(n.Cells["v_Value2"].Value.ToString())
                    //                  where (cod - cod % 100) == intCodigo
                    //                  select int.Parse(n.Cells["i_ItemId"].Value.ToString()));
                    string cod = grdData.Selected.Rows[0].Cells["v_Value2"].Value.ToString();
                    var intIdItems = (from n in grdData.Rows
                                      //let cod = int.Parse(n.Cells["v_Value2"].Value.ToString())
                                      where n.Cells["v_Value2"].Value.ToString ().StartsWith (cod)
                                      select int.Parse(n.Cells["i_ItemId"].Value.ToString()));
                    intIdItems.ToList().ForEach(id =>
                    {
                        this._datahierarchyDto = _objDataHierarchyBL.GetDataHierarchy(ref objOperationResult, this._pIntIdGroup, id);
                        _objDataHierarchyBL.DeleteDataHierarchy(ref objOperationResult, this._pIntIdGroup, id, Globals.ClientSession.GetAsList());
                    });
                }
                else
                {
                    var intIdItem = int.Parse(grdData.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());
                    this._datahierarchyDto = _objDataHierarchyBL.GetDataHierarchy(ref objOperationResult, this._pIntIdGroup, intIdItem);
                    _objDataHierarchyBL.DeleteDataHierarchy(ref objOperationResult, this._pIntIdGroup, intIdItem, Globals.ClientSession.GetAsList());
                }

                if (objOperationResult.Success == 1)
                {
                    CargarGrillaDetalle();
                    btnEliminar.Enabled = false;
                    btnEditar.Enabled = false;
                }
                else
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos UltraGrid
        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row != null)
                {
                    grdData.Rows[row.Index].Selected = true;
                    btnEliminar.Enabled = true;
                    btnEditar.Enabled = true;
                    
                }
                else
                {
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                   
                }
            }
        }
        private void grdData_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_Header"].Value != null && e.Row.Cells["i_Header"].Value.ToString() == "1")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                e.Row.Appearance.BackColor = Color.Bisque;
                e.Row.Appearance.BackColor2 = Color.Bisque;
            }
            else if (e.Row.Cells["i_Header"].Value == null || e.Row.Cells["i_Header"].Value.ToString() == "0")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                e.Row.Appearance.BackColor = Color.White;
            }
        }
        #endregion

        /// <summary>
        /// Obtiene el Dto segun su id, para editarlo
        /// </summary>
        /// <param name="pintIdItem">Id de la Entidad</param>
        void CargarDetalle(int pintIdItem)
        {

            OperationResult objOperationResult = new OperationResult();
            this._datahierarchyDto = new datahierarchyDto();
            this._datahierarchyDto = _objDataHierarchyBL.GetDataHierarchy(ref objOperationResult, this._pIntIdGroup, pintIdItem);
            grbDescripcion.Enabled = true;
            if (_pIntIdGroup != GrupoNotaEstadoSituacionFinanciera)
            {
                txtTitulo.Text = _datahierarchyDto.v_Value1;
                txtCodigo.Text = Value == 4 ? _datahierarchyDto.v_Value4 : _datahierarchyDto.v_Value2;
                
            }

            if (_pIntIdGroup == GrupoUnidadMedida)
            {
                txtValor2.Text = _datahierarchyDto.v_Value2;
            }
            
            if (_pIntIdGroup == GrupoNotaEstadoSituacionFinanciera)
            {
                txtCodigo.Text = _datahierarchyDto.v_Field;
                txtDescripcion.Text = _datahierarchyDto.v_Value2;
                txtTitulo.Text = _datahierarchyDto.v_Value1;
            }

            if (_pIntIdGroup != GrupoNotaEstadoSituacionFinanciera)
            {
                ValidatorDescription.Validate(true, false);
            }
            _modeDetalle = "Edit";

          
        }

        /// <summary>
        /// Se cargar el contenido al Grid
        /// </summary>
        void CargarGrillaDetalle()
        {
            grdData.DataSource = GetData("v_Value2 asc", "");
        }

        private List<datahierarchyDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objDataHierarchyBL.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, pstrSortExpression, pstrFilterExpression, this._pIntIdGroup);

            if (objOperationResult.Success != 1)
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage,
                                     "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return _objData;
        }

        /// <summary>
        /// Valida si el codigo nuevo ya existe en el Grid
        /// </summary>
        /// <param name="pcodigo">Codigo a verificar</param>
        /// <returns>True si existe, de otro modo False</returns>
        private bool ValidarCodigo(string pcodigo)
        {
            if (_pIntIdGroup == GrupoRegimenLaboral || _pIntIdGroup == GrupoOcupacion)
            {
                return true;
            }
            else
            {

                var existCode = grdData.Rows.Any(r => r.GetCellValue("v_Value2").ToString().Equals(pcodigo));
                return existCode;
            }
        }

        private void ucMantenimientoHierarchy_Load(object sender, EventArgs e)
        {


            if (_pIntIdGroup == GrupoRegimenLaboral)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Código Sunat";
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Régimen Laboral";
            }
            if (_pIntIdGroup == GrupoOcupacion)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Código";
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Nombre Ubicación";
            }

            if (_pIntIdGroup == GrupoTipoActivo)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Código";
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Nombre Tipo Activo";
            }

            if (_pIntIdGroup == GrupoCentroCosto )
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Código";
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Nombre Centro Costo";
            }
            if (_pIntIdGroup == GrupoOcupacion)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Código";
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Ocupación";
            }
            if (_pIntIdGroup == GrupoUnidadMedida)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Unidad de Medida";
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Eq. Unidad Medida";
                txtValor2.Visible = true;
                lblValor2UnidadMedida.Visible = true;
            }
            if (_pIntIdGroup == GrupoZona )
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Nombre Zona";
                lblTitulo.Text = "Zona : ";
            }

            if (_pIntIdGroup == GrupoNotaEstadoSituacionFinanciera)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Header.Caption = "Nota";
                grdData.DisplayLayout.Bands[0].Columns["v_Field"].Header.Caption = "N. Alternativo";
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Descripción";
                txtValor2.Visible = true;
                lblValor2UnidadMedida.Visible = true;
                txtValor2.Size = new Size(281,21);
            }
            configurarGrilla();
        }

        private void configurarGrilla()
        {
            UltraGridColumn v_Value2 = grdData.DisplayLayout.Bands[0].Columns["v_Value2"];
            UltraGridColumn v_Value4 = grdData.DisplayLayout.Bands[0].Columns["v_Value4"];
            UltraGridColumn v_Field = grdData.DisplayLayout.Bands[0].Columns["v_Field"];


            v_Value2.Hidden = Value == 4;
            v_Value4.Hidden = Value != 4;
            v_Field.Hidden = true;

            if (_pIntIdGroup ==GrupoOcupacion )
            {
                v_Value2.Hidden = true;
                lblValor2.Visible = false;
                txtCodigo.Visible = false;
            }
            if (_pIntIdGroup == GrupoUnidadMedida)
            {
                txtValor2.Visible = true;
                lblValor2UnidadMedida.Visible = true;
                v_Value2.Hidden = false;
            
            }else  if (_pIntIdGroup ==GrupoZona)
            {
                v_Value2.Hidden = true;
                txtValor2.Visible = false;
                lblValor2UnidadMedida.Visible = false;
                txtCodigo.Visible = false;
                lblValor2.Visible = false;
            }
            else if (_pIntIdGroup == GrupoNotaEstadoSituacionFinanciera)
            {
                txtCodigo.Size = new Size(250, 21);
                lblValor2.Visible = true;
                lblValor2.Text = "N. Alternativo :";
                lblTitulo.Text = "Nombre : ";
                txtCodigo.Visible = true;
                txtValor2.Visible = false;
                lblValor2UnidadMedida.Visible = false;
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Hidden = false;
                grdData.DisplayLayout.Bands[0].Columns["v_Field"].Hidden = false;
                lblDesc.Visible = true;
                txtDescripcion.Visible = true;
                lblDesc.Visible = true;

            }



            
        }

        private void txtValor2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }
    }
}
