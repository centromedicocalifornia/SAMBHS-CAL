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
using System.Threading;
using System.Threading.Tasks;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteActaEntrega : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteActaEntrega(string pstr)
        {
            InitializeComponent();
        }

        private void frmReporteActaEntrega_Load(object sender, EventArgs e)
        {
            cboOrdenar.Text = "CÓDIGO ACTIVO FIJO";
            rdbAltas.Checked = true;
            rdbBajas.Checked = false;
            dtpFechaBaja.Enabled = false;
            dtpFechaCompra.Enabled = true;
            this.BackColor = new GlobalFormColors().FormColor;

        }

        private void rdbAltas_CheckedChanged(object sender, EventArgs e)
        {
            dtpFechaCompra.Enabled = rdbAltas.Checked;
        }

        private void rdbBajas_CheckedChanged(object sender, EventArgs e)
        {
            dtpFechaBaja.Enabled = rdbBajas.Checked;
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

        private void txtUbicacion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(103, "Buscar Ubicación");
            frm2.ShowDialog();
            if (frm2._itemId != null)
            {
                txtUbicacion.Text = frm2._value2.Trim();
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

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvValidar.Validate(true, false).IsValid)
                {
                    string _strFilterExpression = string.Empty, GrupoLlave = String.Empty, strOrderExpression = string.Empty;
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

                    _strFilterExpression = string.Empty;
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }

                    if (chkAgrupadoResponsable.Checked == true)
                    {
                        GrupoLlave = "Responsable";
                        Agrupado = 1;
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
                   
                        CargarReporte(_strFilterExpression, GrupoLlave, Agrupado, strOrderExpression);
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
                ? @"Generando... " + "Acta  de Entrega"
                : @"Acta  de Entrega";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;

        }
        private void CargarReporte(string _strFilterExpression, string GrupoLlave, int Agrupado, string Orden)
        {
            OperationResult objOperationResult = new OperationResult();
            var rp = new Reportes.ActivoFijo.crReporteActaEntrega();
             OcultarMostrarBuscar(true);
             List<actaentregaDto> aptitudeCertificate = new List<actaentregaDto>();
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
             aptitudeCertificate = new ActivoFijoBL().ReporteActaEntrega(  ref objOperationResult , _strFilterExpression, GrupoLlave, Orden);
          
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Acta de Entrega", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Acta de Entrega", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
            
            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);

            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsActaEntrega";
            ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);
            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("Agrupado", Agrupado);
            rp.SetParameterValue("NombreEmpresa", aptitudeCertificate1.FirstOrDefault ().NombreEmpresaPropietaria.Trim ());
            rp.SetParameterValue("RucEmpresa", "R.U.C. : "+aptitudeCertificate1.FirstOrDefault ().RucEmpresaPropietaria.Trim  ());


            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());





        }

        private void chkAgrupadoResponsable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmReporteActaEntrega_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }



    }
}
