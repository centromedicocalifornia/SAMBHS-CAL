#region Name Space
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;
#endregion


namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaAlmacenResumen : Form
    {
        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();

        LineaBL _objLineaBL = new LineaBL();

        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        #endregion
        #region Carga de inicializacion
        public frmRegistroVentaAlmacenResumen( string parametro)
        {
            InitializeComponent();
        }
        #endregion
        #region Cargar Load       
        private void frmRegistroVentaAlmacenResumen_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;

            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList (cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();  
        }
     #endregion
        #region Cargar Reporte
        private void CargarReporte( DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdAlmacen, string FechaHoraImpresion, string Orden)
        {
            int NroRegistros=0 ;
            var rp = new Reportes.Ventas.crRegistroVentaAlmacenResumen();
 
            var aptitudeCertificate = new VentaBL().ReporteVentaAlmacenResumen( 0, FechaRegistroIni, FechaRegistroFin, IdAlmacen, Orden);
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
            var aptitudeCertificate3 = new VentaBL().ReporteVentaAlmacenResumenDocumentos( 0, FechaRegistroIni, FechaRegistroFin, IdAlmacen, Orden);
             NroRegistros = aptitudeCertificate.Count ()==0 && aptitudeCertificate3.Count ()==0 ? 0 : aptitudeCertificate.Count (); 
  
            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);
            DataTable dt3 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate3);
            for (int i = 0; i <= dt3.Rows.Count - 1; i++)
            {
                if (i == 0) {
                    DataRow workRow1;
                    workRow1 = dt.NewRow();


                    workRow1[0] = "_________________________________________________________________";
                    workRow1[1] = "** RESUMEN DOCUMENTOS **";
                    workRow1[2] = 0;
                    workRow1[3] = 0;
                    workRow1[4] = 0;
                    workRow1[5] = dt3.Rows[i][5].ToString();
                    workRow1[6] = dt3.Rows[i][6].ToString();
                    dt.Rows.Add(workRow1);
                }

                DataRow workRow;
                workRow = dt.NewRow();
                workRow[0] = "";
                workRow[1] = dt3.Rows[i][0].ToString();
                workRow[2] = decimal.Parse(dt3.Rows[i][2].ToString());
                workRow[4] = dt3.Rows[i][4].ToString();
                workRow[5] = dt3.Rows[i][5].ToString();
                workRow[6] = dt3.Rows[i][6].ToString();
                dt.Rows.Add(workRow);
            }

            ds1.Tables.Add(dt2);
            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt3);

            ds1.Tables[0].TableName = "dsEmpresa";
            ds1.Tables[1].TableName = "dsRegistroVentaAlmacenResumen";
            ds1.Tables[2].TableName = "dsRegistroVentaAlmacenResumenDocumentos";
            rp.SetDataSource(ds1);

            
            rp.SetParameterValue("NombreEmpresa",aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("RucEmpresa", aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
            rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
            rp.SetParameterValue("IdMoneda", 1);
            rp.SetParameterValue("Fecha","DEL " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " AL " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") +"/"+ FechaRegistroFin.Date.Year.ToString());
            rp.SetParameterValue("NroRegistros",  NroRegistros);
            TxtCodigoProducto.ReportSource = rp;
            TxtCodigoProducto.Show();
        }
        #endregion

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {

                    string strOrderExpression;
                    var rp = new Reportes.Ventas.crRegistroVentaProductoResumen();
                    List<string> Filters = new List<string>();
                    strOrderExpression = "";
                    using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                    CargarReporte( DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), int.Parse(cboAlmacen.Value.ToString()), chkHoraimpresion.Checked == true ? "1" : "0", strOrderExpression);

                }
                else
                {
                   // UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
