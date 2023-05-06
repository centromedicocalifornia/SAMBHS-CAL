using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.ActivoFijo.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteRelacionBienes : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteRelacionBienes(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmReporteRelacionBienes_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            cboOrdenar.Text = "CÓDIGO ACTIVO FIJO";
            cboAgrupar.Text = "--Seleccionar--";
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvValidar.Validate(true, false).IsValid)
                {
                    string _strFilterExpression = string.Empty, GrupoLlave1 = String.Empty, strOrderExpression = string.Empty, GrupoLlave2 = String.Empty, TotalGrupoLlave1 = String.Empty, TotalGrupoLLave2 = string.Empty;
                    int Agrupado = 0;
                    List<string> Filters = new List<string>();
                    int anioCompra = 0;
                    int anioBaja = 0;

                    if (rdbAltas.Checked == true)
                    {
                        if (dtpFechaCompra.Checked == true)
                        {
                            anioCompra = dtpFechaCompra.Value.Date.Year;
                            Filters.Add("AnioCompra==" + anioCompra);
                        }
                    }
                    else if (rdbBajas.Checked == true)
                    {
                        Filters.Add("i_Baja==" + 1);
                        if (dtpFechaBaja.Checked == true)
                        {
                            anioBaja = dtpFechaBaja.Value.Date.Year;
                            Filters.Add("AnioBaja==" + anioBaja);
                        }
                    }

                    if (txtCodigoResponsable.Text != string.Empty) Filters.Add("CodigoProveedor==\"" + txtCodigoResponsable.Text.Trim() + "\"");
                    if (txtTipoActivoFijo.Text != string.Empty) Filters.Add("CodTipoActivo==\"" + txtTipoActivoFijo.Text.Trim() + "\"");
                    if (txtCodigoResponsable.Text != string.Empty) Filters.Add("CodResponsable==\"" + txtCodigoResponsable.Text.Trim() + "\"");
                    if (txtUbicacion.Text != string.Empty) Filters.Add("CodUbicacion==\"" + txtUbicacion.Text.Trim() + "\"");
                    if (txtCentroCosto.Text != string.Empty) Filters.Add("CodCentroCosto==\"" + txtCentroCosto.Text.Trim() + "\"");

                    _strFilterExpression = string.Empty;
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }

                    if (cboAgrupar.Text != "--Seleccionar--")
                    {
                        if (cboAgrupar.Text == "RESPONSABLE")
                        {
                            GrupoLlave1 = "Responsable";
                            TotalGrupoLlave1 = "SUB TOTAL POR RESPONSABLE : ";
                            TotalGrupoLLave2 = "";

                        }
                        else if (cboAgrupar.Text == "CENTRO COSTO  Y UBICACIÓN")
                        {
                            GrupoLlave1 = "Centro";
                            GrupoLlave2 = "Ubicacion";
                            TotalGrupoLlave1 = "SUB TOTAL POR CENTRO DE COSTO : ";
                            TotalGrupoLLave2 = "SUB TOTAL POR UBICACIÓN : ";
                        }
                    }


                    if (cboOrdenar.Text == "CÓDIGO ACTIVO FIJO")
                    {
                        strOrderExpression = "CodigoActivoFijo";
                    }
                    else if (cboOrdenar.Text == "DESCRIPCIÓN ACTIVO FIJO")
                    {
                        strOrderExpression = "DescripcionActivoFijo";
                    }
                    else if (cboOrdenar.Text == "UBICACIÓN")
                    {
                        strOrderExpression = "Ubicacion";
                    }

                    CargarReporte(_strFilterExpression, GrupoLlave1, GrupoLlave2, Agrupado, strOrderExpression, TotalGrupoLlave1, TotalGrupoLLave2, Globals.ClientSession.i_Periodo.Value, DateTime.Now.Month);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrrió un Error al Generar el Reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Relación de Bienes"
                : @"Reporte de Relación de Bienes";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }
        private void CargarReporte(string Filtro, string GrupoLLave1, string GrupoLLave2, int Agrupado, string Orden, string TotalGrupoLlave1, string TotalGrupoLlave2, int periodo, int mes)
        {


            var rp = new Reportes.ActivoFijo.crReporteRelacionBienes();
            OperationResult objOperationResult = new OperationResult();
            List<ReporteRelacionBienesDto> aptitudeCertificate = new List<ReporteRelacionBienesDto>();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new ActivoFijoBL().ReporteRelacionBienes(ref objOperationResult, Filtro, GrupoLLave1, GrupoLLave2, Orden, periodo, mes, 0, 0);

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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Relación de Bienes", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Relación de Bienes", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);

                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteRelacionBienes";
                ds1.Tables[1].TableName = "dsEmpresa";

                rp.SetDataSource(ds1);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("TotalGrupoLlave1", TotalGrupoLlave1);
                rp.SetParameterValue("TotalGrupoLlave2", TotalGrupoLlave2);
                //rp.SetParameterValue("Agrupado", Agrupado);
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate1.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate1.FirstOrDefault().RucEmpresaPropietaria.Trim());
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void txtCentroCosto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
            frm2.ShowDialog();
            if (frm2._itemId != null)
            {
                txtCentroCosto.Text = frm2._value2.Trim();
            }
        }

        private void txtUbicacion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(103, "Buscar Ubicación");
            frm2.ShowDialog();
            if (frm2._itemId != null)
            {
                txtUbicacion.Text = frm2._value2.Trim();
            }
        }

        private void txtTipoActivoFijo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarTipoActivoFijo")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.ActivoFijo.frmBuscarTipoActivo frm = new Mantenimientos.ActivoFijo.frmBuscarTipoActivo("MOVIMIENTO", 104);
                frm.ShowDialog();
                txtTipoActivoFijo.Text = frm.CodigoTipoActivoFijo.Trim();

            }
        }

        private void txtCodigoResponsable_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProveedor")
            {
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("REPORTE", "RUC");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {

                    txtCodigoResponsable.Text = frm._CodigoProveedor.Trim();

                }
            }
        }

        private void rdbAltas_CheckedChanged(object sender, EventArgs e)
        {
            dtpFechaBaja.Checked = false;
        }

        private void rdbBajas_CheckedChanged(object sender, EventArgs e)
        {
            dtpFechaCompra.Checked = false;
        }

        private void frmReporteRelacionBienes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

    }
}
