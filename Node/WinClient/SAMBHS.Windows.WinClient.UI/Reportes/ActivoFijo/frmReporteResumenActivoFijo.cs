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
using SAMBHS.Common.BL;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Common.BE;
using CrystalDecisions.CrystalReports.Engine;

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteResumenActivoFijo : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteResumenActivoFijo(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmReporteResumenActivoFijo_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarPeriodo();

        }
        private void ValidarPeriodo()
        {
            nupAnio.Minimum = 1900;
            nupAnio.Maximum = 2050;
            nupMes.Minimum = 1;
            nupMes.Maximum = 12;
            nupAnio.Value =decimal.Parse ( Globals.ClientSession.i_Periodo.ToString ());
            nupMes.Value = decimal.Parse(DateTime.Now.Month.ToString());  
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
               
                CargarReporte( int.Parse (nupAnio.Value.ToString ()),int.Parse ( nupMes.Value.ToString ()));
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            
            }
        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Resumen de Activo Fijo"
                : @"Resumen de Activo Fijo";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }


        private void CargarReporte(int Periodo, int mes)
        {
            ReportDocument rp = new ReportDocument();

            if (uckMostrarDetalles.Checked)
            {
                rp = new Reportes.ActivoFijo.crReporteResumenActivoFijoDetallado();
            }
            else
            {
                rp = new Reportes.ActivoFijo.crReporteResumenActivoFijo();
            }
            OperationResult objOperationResult = new OperationResult();
              OcultarMostrarBuscar(true);
              List<ReporteResumenActivoFijoDto> aptitudeCertificate = new List<ReporteResumenActivoFijoDto>();
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

            aptitudeCertificate = new ActivoFijoBL().ReporteResumenActivoFijo(ref objOperationResult ,  Periodo,mes,uckMostrarDetalles.Checked);
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Resumen de Activo Fijo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Resumen de Activo Fijo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


            var Empresa = new NodeBL().ReporteEmpresa();
            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            ds1.Tables.Add(dt);
            ds1.Tables[0].TableName = "dsReporteResumenActivoFijo";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("RucEmpresa","R.U.C. : "+ Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
            rp.SetParameterValue("Periodo", "AL MES DE  : " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMes.Value.ToString())).ToUpper ()+"  DEL "+ nupAnio.Value.ToString ());
            rp.SetParameterValue ("Saldo","SALDO AL " +"31/12/"+  (Periodo -1 ).ToString () );
            rp.SetParameterValue("Adiciones", "ADICIONES \n (COMPRAS) " + ( Periodo ).ToString());
            rp.SetParameterValue("Retiros", "RETIROS(BAJAS) " + (Periodo).ToString());
            rp.SetParameterValue("SaldoAlMesAnio", "SALDO AL \n" + mes.ToString ("00")+"/"+ Periodo.ToString ());
            rp.SetParameterValue("Depreciaciones", "DEPRECIACIONES ACUM. AL " + mes.ToString("00") + "/" + Periodo.ToString()); 
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        
        }

        private void frmReporteResumenActivoFijo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
    }
}
