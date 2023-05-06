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

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmProducto : Form
    {
        #region Declaraciones / Referencias
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        LineaBL _objLineaBL = new LineaBL();
        string _strFilterExpression;
        string _whereAlmacenesConcatenados;
        string _AlmacenesConcatenados;
        string strOrderExpression;
        string strGrupollave, strGrupollave2;
        string strNombreGrupollave, strNombreGrupollave2;
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        #endregion     
        #region Carga de inicializacion
        public frmProducto(string _IdProducto)
        {
            InitializeComponent();
        }
        #endregion
        #region Cargar Load
        private void frmProducto_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
 
            OperationResult objOperationResult = new OperationResult();

            #region Cargar Combos
            Utils.Windows.LoadDropDownList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 61, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboAgrupar, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 62, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);

            #endregion
        }
        #endregion
        #region Cargar Reporte
        private void CargarReporte(string _IdProducto, string _IdLinea)
        {

            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;

            OperationResult objOperationResult = new OperationResult();



            datahierarchyDto __datahierarchyDto = new datahierarchyDto();
            List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
            List<datahierarchyDto> _datahierarchyDto = new List<datahierarchyDto>();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();

            var rp = new Reportes.Compras.crProducto();
            strOrderExpression = "";
            _ListadoGrupos = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 62, null);


            //string Seccion;

            //Seccion = cboAgrupar.SelectedValue.ToString().ToLower();
            //Seccion = Seccion.Replace("g", "G");
            //Seccion = Seccion.Replace("s", "S");
            //Seccion = Seccion.Replace("h", "H");

            //strGrupollave = "";
            //strNombreGrupollave = "";
            //strGrupollave2 = null;
            //strNombreGrupollave2 = null;
            //Grupollave = new List<string>();
            //NombreGrupollave = new List<string>();

            //for (int i = 0; i <= _ListadoGrupos.Count - 1; i++)
            //{

            //    if (cboAgrupar.SelectedValue.ToString().Trim() == _ListadoGrupos[i].Value2.ToString().Trim() && _ListadoGrupos[i].Value3.ToString().Trim() != "")
            //    {

            //        if (cboAgrupar.Text.Trim() == _ListadoGrupos[i].Value1.ToString())
            //        {

            //            strNombreGrupollave = _ListadoGrupos[i].Value1.ToString();


            //            string[] splitNombreGrupollave = strNombreGrupollave.Split(new Char[] { '/' });
            //            foreach (string s in splitNombreGrupollave)
            //            {
            //                if (s.Trim() != "")
            //                    NombreGrupollave.Add(s);

            //            }

            //            if (NombreGrupollave.Count == 2)
            //            {
            //                strNombreGrupollave = NombreGrupollave[0];
            //                strNombreGrupollave2 = NombreGrupollave[1];

            //            }
            //            else
            //            {
            //                strNombreGrupollave = NombreGrupollave[0];
            //            }

            //            strOrderExpression = _ListadoGrupos[i].Value3.ToString();
            //            strGrupollave = _ListadoGrupos[i].Value3.ToString();
            //        }
            //    }
            //    string[] split = _ListadoGrupos[i].Value2.Split(new Char[] { ',' });
            //    foreach (string s in split)
            //    {
            //        if (s.Trim() != "")
            //            Retonar.Add(s);

            //    }
            //}
            //Retonar = Retonar.Distinct().ToList();
            //for (int i = 0; i <= Retonar.Count() - 1; i++)
            //{

            //    Retonar2.Add(Retonar[0]);
            //    Seccion = Retonar[i].ToLower();
            //    Seccion = Seccion.Replace("g", "G");
            //    Seccion = Seccion.Replace("s", "S");
            //    Seccion = Seccion.Replace("h", "H");
            //    Seccion = Seccion.Replace("f", "F");
            //}

            //if (cboAgrupar.SelectedValue.ToString().Trim() != "")
            //{

            //    string[] split_ = cboAgrupar.SelectedValue.ToString().Split(new Char[] { ',' });
            //    foreach (string s in split_)
            //    {
            //        if (s.Trim() != "")
            //            Seccion = s.ToLower();
            //        Seccion = Seccion.Replace("g", "G");
            //        Seccion = Seccion.Replace("s", "S");
            //        Seccion = Seccion.Replace("h", "H");
            //        Seccion = Seccion.Replace("f", "F");
            //        if (cboAgrupar.Text.ToString() == "NINGUNO")
            //        {

            //            rp.ReportDefinition.Sections[Seccion].SectionFormat.EnableSuppress = true;
            //        }
            //        else
            //        {
            //            rp.ReportDefinition.Sections[Seccion].SectionFormat.EnableSuppress = false;
            //        }
            //    }
            //}




            //NodeBL _objVentasBL = new NodeBL();


           

            //string[] splitGrupollave = strGrupollave.Split(new Char[] { ',' });
            //foreach (string s in splitGrupollave)
            //{
            //    if (s.Trim() != "")
            //        Grupollave.Add(s);

            //}
            //if (Grupollave.Count > 0)
            //{
            //    if (Grupollave.Count == 2)
            //    {
            //        strGrupollave = Grupollave[0];
            //        strGrupollave2 = Grupollave[1];

            //    }
            //    else
            //    {
            //        strGrupollave = Grupollave[0];
            //    }
            //}

            strOrderExpression += strOrderExpression != "" ? strOrderExpression != cboOrden.SelectedValue.ToString().Trim() ? "," + cboOrden.SelectedValue.ToString().Trim() : "" : cboOrden.SelectedValue.ToString().Trim();
            strGrupollave = cboAgrupar.Text.Trim();
            var aptitudeCertificate = new ProductoBL().ReporteProducto(_IdProducto, _IdLinea, "" + strOrderExpression + " ASC", strGrupollave);
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsProducto";
            ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = chkHoraimpresion.Checked == true ? "1" : "0";  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["FechaHoraImpresion"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            crystalReportViewer1.Zoom(110);
        }
        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (uvDatos.Validate(true, false).IsValid)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
                    CargarReporte(TxtProducto.Text.Trim(), cboLinea.SelectedValue.ToString());

                }
                else
                {

                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }



            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(1, "PedidoVenta", "", "");
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                TxtProducto.Text = frm._CodigoInternoProducto;

            }
            else
            {

            }
        }
         #endregion
    }
}
