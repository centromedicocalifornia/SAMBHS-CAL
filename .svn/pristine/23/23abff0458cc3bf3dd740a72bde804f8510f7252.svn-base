using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Compra.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmRegistroDetracciones : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        ComprasBL _objCompraBL = new ComprasBL();
        public frmRegistroDetracciones(string p)
        {
            InitializeComponent();
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + " Registro de Detracciones"
                : @"Registro de Detracciones";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            List<RegistroRetenciones> RegistroDetracciones = new List<RegistroRetenciones>();
            OperationResult objOperationResult = new OperationResult();
            if (Validar.Validate(true, false).IsValid)
            {

                OcultarMostrarBuscar(true);
                Cursor.Current = Cursors.WaitCursor;
                Task.Factory.StartNew(() =>
                {
                    RegistroDetracciones = _objCompraBL.ReporteRegistroDetracciones(ref  objOperationResult, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Text + " 23:59"), int.Parse(cboTipoCompra.Value.ToString()), txtNroCuenta.Text.Trim(), cboOrden.Value.ToString(), int.Parse (cboMoneda.Value.ToString ()));

                }, _cts.Token)
                .ContinueWith(t =>
                {
                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    Cursor.Current = Cursors.Default;
                    if (objOperationResult.Success == 0)
                    {
                        if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                        {
                            UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error " + " Reporte Kardex Físico / Valorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error" + " Reporte Kardex Físico / Valorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }



                    var Empresa = new NodeBL().ReporteEmpresa().FirstOrDefault();
                    var rp = new Reportes.Compras.crRegistroDetracciones();
                    DataSet ds1 = new DataSet();
                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(RegistroDetracciones);

                    dt.TableName = "dsRegistroDetraccion";
                    ds1.Tables.Add(dt);
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("NombreEmpresa", Empresa.NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa","RUC :"+ Empresa.RucEmpresaPropietaria.Trim());
                    rp.SetParameterValue ("FechaDel","DEL " + dtpFechaInicio.Text +" AL " + dtpFechaFin.Text );
                    rp.SetParameterValue ("IdMoneda" ,int.Parse  (cboMoneda.Value.ToString()));
                    rp.SetParameterValue("NroRegistros", RegistroDetracciones.Count());
                    crystalReportViewer1.ReportSource = rp;

                }
                    , TaskScheduler.FromCurrentSynchronizationContext());
            }

        }

        private void frmRegistroDetracciones_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipoCompra, "Value1", "Id", _objCompraBL.ObtenerConceptosParaCombo(ref objOperationResult, 1, null), DropDownListAction.Select);
          
            cboTipoCompra.Value = "-1";
            cboOrden.Value = "-1";
            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.Value.ToString();
            cboOrden.Value = "NroRegistro";
        }


    }
}
