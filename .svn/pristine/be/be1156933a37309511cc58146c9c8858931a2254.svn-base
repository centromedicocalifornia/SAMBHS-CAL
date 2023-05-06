using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoListaPrecios : Form
    {
        int IdAlmacen = -1;
        int IdListaPrecios = -1;
        ListaPreciosBL _objListaPrecioBL = new ListaPreciosBL();
        string TipoImpresion = "";
        public frmDocumentoListaPrecios(int pintAlmacen, int pIdListaPrecios,string sTipoImpresion)
        {
            InitializeComponent();
            IdAlmacen = pintAlmacen;
            IdListaPrecios = pIdListaPrecios;
            TipoImpresion = sTipoImpresion;
        }

        private void frmDocumentoListaPrecios_Load(object sender, EventArgs e)
        {
            ImpresionListaPrecios();
        }
        private void ImpresionListaPrecios()
        { 
            OperationResult objOperationResult= new OperationResult ();
          var aptitudeCertificate =  _objListaPrecioBL.ImpresionListaPreciosyDetalles(ref objOperationResult, IdAlmacen, IdListaPrecios);

            ReportDocument rp = new ReportDocument();
            if (TipoImpresion == "D")
            {
                var _crystalDescuentos = new Reportes.Ventas.crDocumentoListaPreciosDescuentos();
                rp = _crystalDescuentos;


            }
            else
            { 
              var _crystalUtilidades = new Reportes.Ventas.crDocumentoListaPreciosUtilidades() ;
                rp = _crystalUtilidades;
            
            }

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            else
            {
                DataSet ds = new DataSet();

                var Empresa = new NodeBL().ReporteEmpresa();
                dt.TableName = "dsImpresionListaPrecios";
                ds.Tables.Add(dt);
                rp.SetDataSource(ds);

                //rp.SetParameterValue("CantidadDecimalC", (int)Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa","R.U.C. "+" "+ Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                rp.SetParameterValue("DecimalesPrecio", Globals.ClientSession.i_PrecioDecimales.Value);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
        
        
        
        }
    }
}
