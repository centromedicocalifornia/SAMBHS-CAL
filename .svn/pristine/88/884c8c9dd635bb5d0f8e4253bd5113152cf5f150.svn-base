using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Compra.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmPagoPendienteConsulta : Form
    {
        public string v_IdProveedor = String.Empty;
        ClienteBL _objClienteBL= new ClienteBL ();
        string _strFilterExpression = null;
        string SiglasDocumento;
        public bool IncluirLetras = true, SoloLetras = false;
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        readonly DocumentoBL _objDocumentoBL = new DocumentoBL();
        PagoBL _objPagoBL = new PagoBL();
        private readonly bool _mostrarDocInversos;

       public frmPagoPendienteConsulta(string IdProveedor = null, bool _IncluirLetras = true, bool _SoloLetras = false, string siglasDocumento = null, bool mostrarInversos = false)
        {
            InitializeComponent();

            if (IdProveedor != null)
            {
                OperationResult objOpertationResult = new OperationResult();
                var Proveedor = new ClienteBL().ObtenerClientePorID(ref objOpertationResult, IdProveedor);
                txtProveedor.Text = Proveedor.v_CodCliente;
                txtRazonSocial.Text =(Proveedor.v_ApePaterno +" "+ Proveedor.v_ApeMaterno +" " + Proveedor.v_PrimerNombre +" " + Proveedor.v_RazonSocial ).Trim ();
                txtProveedor.Enabled = false;
                v_IdProveedor = Proveedor.v_IdCliente;
                SoloLetras = _SoloLetras;
                _mostrarDocInversos = mostrarInversos;
            }

            SiglasDocumento = siglasDocumento;
        }

        private void frmPagoPendienteConsulta_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.All);
            cboTipoDocumento.Value = "-1";
            dtpFechaInicio.Value = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString ());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            btnBuscar_Click(sender, e);
            btnSeleccionarTodos.Enabled = !SoloLetras;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            _strFilterExpression = string.Empty;

            if (uvDatos.Validate(true, false).IsValid)
            {
                List<string> Filters = new List<string>();
                if (cboTipoDocumento.Value.ToString() != "-1")
                {
                    Filters.Add("i_IdTipoDocumento==" + cboTipoDocumento.Value.ToString());

                    if (!string.IsNullOrEmpty(txtCorrelativoDoc.Text) && !string.IsNullOrEmpty(txtSerieDoc.Text))
                    {
                        if (int.Parse(txtCorrelativoDoc.Text) > 0 && int.Parse(txtSerieDoc.Text) > 0)
                        {
                            Filters.Add("NroDocumento==\"" + txtSerieDoc.Text + "-" + txtCorrelativoDoc.Text + "\"");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(v_IdProveedor)) Filters.Add("v_IdProveedor==\"" + v_IdProveedor + "\"");
               
                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }

                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    this.BindGrid();
                    grdData.Focus();
                }

                txtTotal.Text = grdData.Rows.Where(p => p.Cells["d_Saldo"].Value != null).Sum(o => decimal.Parse(o.Cells["d_Saldo"].Value.ToString())).ToString("0.00");
            }
        }


        private void BindGrid()
        {
            var objData = GetData("FechaRegistro  ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);

            if (grdData.Rows.Count > 0)
            {
                for (int i = 0; i < grdData.Rows.Count(); i++)
                {
                    grdData.Rows[i].Cells["Seleccion"].Value = "0";
                    grdData.Rows[i].Cells["Ubicacion"].ToolTipText = grdData.Rows[i].Cells["UbicacionNombreCompleto"].Value != null ? grdData.Rows[i].Cells["UbicacionNombreCompleto"].Value.ToString() : string.Empty;

                    if (!string.IsNullOrEmpty(SiglasDocumento) && grdData.Rows[i].Cells["Ubicacion"].Value != null)
                    {
                        string Ubicacion = grdData.Rows[i].Cells["Ubicacion"].Value.ToString();

                        if (!string.IsNullOrEmpty(Ubicacion))
                        {
                            if (Ubicacion != SiglasDocumento)
                            {
                                grdData.Rows[i].Cells["Seleccion"].Activation = Activation.Disabled;
                            }
                            else
                            {
                                grdData.Rows[i].Cells["Seleccion"].Activation = Activation.AllowEdit;
                            }
                        }
                        else
                        {
                            grdData.Rows[i].Cells["Seleccion"].Activation = Activation.AllowEdit;
                        }
                    }
                    if (!_mostrarDocInversos)
                        grdData.Rows[i].Hidden = grdData.Rows[i].Cells["EsDocInverso"].Value != null && grdData.Rows[i].Cells["EsDocInverso"].Value.ToString() == "1";
                }
            }

        }

        

        private List<pagopendienteDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objPagoBL.ListarPagosPendientes(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value, IncluirLetras, SoloLetras);
           

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtProveedor.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejas(ref objOperationResult, txtProveedor.Text.Trim(), "V");
                if (Cliente != null)
                {
                    txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim();
                    v_IdProveedor = Cliente.v_IdCliente;
                }
                else
                {
                    txtRazonSocial.Clear();
                    v_IdProveedor = "-1";
                }
            }
            else
            {
                txtRazonSocial.Clear();
                v_IdProveedor = string.Empty;
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["Seleccion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        }

        private void btnTerminar_Click(object sender, EventArgs e)
        {
            List<UltraGridRow> Filas = new List<UltraGridRow>();

            grdData.Rows.Where(p => p.Cells["Seleccion"].Value != null && p.Cells["Seleccion"].Value.ToString() == "1").ToList().ForEach(o => Filas.Add(o));

            if (System.Windows.Forms.Application.OpenForms["frmPago"] != null)
            {
                (System.Windows.Forms.Application.OpenForms["frmPago"] as Procesos.frmPago).RecibirItems(Filas);
            }


            this.Close();
        }

        private void btnSeleccionarTodos_Click(object sender, EventArgs e)
        {
            grdData.Rows.Where(p => p.Cells["Seleccion"].Activation == Activation.AllowEdit).ToList().ForEach(o => o.Cells["Seleccion"].Value = "1");

        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("P", txtProveedor.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                txtProveedor.Text = frm._NroDocumento;
                txtRazonSocial.Text = frm._RazonSocial;
                v_IdProveedor = frm._IdCliente;
            }
        }
    }
}
