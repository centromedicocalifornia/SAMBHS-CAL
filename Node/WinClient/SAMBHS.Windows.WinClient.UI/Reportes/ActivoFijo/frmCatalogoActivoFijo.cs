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
    public partial class frmCatalogoActivoFijo : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmCatalogoActivoFijo(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmCatalogoActivoFijo_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            
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
                    if (rdbAltas.Checked == true)
                    {
                        if (dtpFechaCompra.Checked == true)
                        {
                            anioCompra = dtpFechaCompra.Value.Date.Year;
                            Filters.Add("AnioCompra==" + anioCompra);
                        }
                    }
                    _strFilterExpression = string.Empty;
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
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
                ? @"Generando... " + "Catálogo de Activos Fijos"
                : @"Catálogo de Activos Fijos";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }
        private void CargarReporte(string Filtro, string GrupoLLave1, string GrupoLLave2, int Agrupado, string Orden, string TotalGrupoLlave1, string TotalGrupoLlave2, int periodo, int mes)
        {


            var rp = new Reportes.ActivoFijo.crCatalogoActivosFijos();
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Catálogo de Activos Fijos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Catálogo de Activos Fijos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate1.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate1.FirstOrDefault().RucEmpresaPropietaria.Trim());
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
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
