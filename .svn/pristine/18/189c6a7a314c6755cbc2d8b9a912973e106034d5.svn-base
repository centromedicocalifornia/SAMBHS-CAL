using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Letras.BL;
using SAMBHS.Common.Resource ;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Letras.LetrasCobrar
{
    public partial class frmDocumentoLetrasPorCobrar : Form
    {
        public string IdCanje= string.Empty;
        LetrasBL _objLetrasBL= new LetrasBL ();
        
        public frmDocumentoLetrasPorCobrar(string sIdCanje)
        {

            IdCanje = sIdCanje;
            ImpresionLetra(IdCanje);
            InitializeComponent();
        }

        private void ImpresionLetra(string IdCanj)
        { 
        
                    OperationResult objOperationResult= new OperationResult ();
                    var aptitudeCertificate = _objLetrasBL.ReporteDocumentoLetra(ref objOperationResult, IdCanj,false);
                    var rp = new Reportes.Letras.LetrasCobrar.crImprimirLetrasCobrar();
                    
                    DataSet ds1 = new DataSet();
                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                    dt.TableName = "dsLetrasCobrar";
                    ds1.Tables.Add(dt);
                    
                    rp.SetDataSource(ds1);


                 

                    //rp.SetParameterValue("Fecha", "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString());
                    //rp.SetParameterValue("Establecimiento", cboEstablecimiento.Text.Trim());
                    //rp.SetParameterValue("DecimalesCantidad", Globals.ClientSession.i_CantidadDecimales);
                    //rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    //rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    //rp.SetParameterValue("RucEmpresa", Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                    crystalReportViewer1.Zoom(110);




                 


        
        }
    }
}
