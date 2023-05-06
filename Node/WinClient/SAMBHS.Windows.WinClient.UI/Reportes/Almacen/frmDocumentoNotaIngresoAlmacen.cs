using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmDocumentoNotaIngresoAlmacen : Form
    {
        #region Declaraciones / Referencias
        string IdMovimiento;
        #endregion
        #region Declaraciones / Referencias
        public frmDocumentoNotaIngresoAlmacen(string _IdMovimiento)
        {
            InitializeComponent();
            IdMovimiento = _IdMovimiento;
            ImpresionDirecto();
        }
        #endregion
        #region Prodecimientos/Funciones
        private void ImpresionDirecto()
        {
            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;

            if (IdMovimiento != null)
            {
                
                var rp = new Reportes.Almacen.crNotaIngresoAlmacen();
                OperationResult objOperationResult = new OperationResult();
                NodeBL _objVentasBL = new NodeBL();
                var Empresa = new NodeBL().ReporteEmpresa();
                var aptitudeCertificate = new AlmacenBL().ReporteNotaIngresoAlmacen(IdMovimiento, (int)TipoDeMovimiento.NotadeIngreso);
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                dt.TableName = "dsNotaIngresoAlmacen";

                ds1.Tables.Add(dt);
                rp.SetDataSource(ds1);


                crParameterValues = new ParameterValues();
                crParameterDiscreteValue = new ParameterDiscreteValue();
                crParameterDiscreteValue.Value = (int)Globals.ClientSession.i_CantidadDecimales;  // TextBox con el valor del Parametro
                crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                crParameterFieldDefinition = crParameterFieldDefinitions["DecimalesCantidad"];
                crParameterValues = crParameterFieldDefinition.CurrentValues;
                crParameterValues.Clear();
                crParameterValues.Add(crParameterDiscreteValue);
                crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);



                crParameterValues = new ParameterValues();
                crParameterDiscreteValue = new ParameterDiscreteValue();
                crParameterDiscreteValue.Value = (int)Globals.ClientSession.i_PrecioDecimales;  // TextBox con el valor del Parametro
                crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                crParameterFieldDefinition = crParameterFieldDefinitions["DecimalesPrecio"];
                crParameterValues = crParameterFieldDefinition.CurrentValues;
                crParameterValues.Clear();
                crParameterValues.Add(crParameterDiscreteValue);
                crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());


                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                
            }
            else
            {
                UltraMessageBox.Show("El documento no se puede imprimir ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.Close();
            }
        }
        #endregion
    }
}
