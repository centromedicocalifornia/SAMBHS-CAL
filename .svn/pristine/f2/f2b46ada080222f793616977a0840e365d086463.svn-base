using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using SAMBHS.Common.BL;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    /// <summary>
    /// Autor: Eduardo Quiroz Cosme
    /// Fecha: 27/10/2014
    /// </summary>
    public partial class frmTipoCambio : Form
    {
        private readonly TipoCambioBL _objTipoCambioBl = new TipoCambioBL();
        private tipodecambioDto _tipodecambioDto = new tipodecambioDto();
        private int Periodo {get { return Globals.ClientSession.i_Periodo ?? DateTime.Now.Year; }}
        private string TipoCambioUrl {
            get { return "http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias?mes=" + cboMes.Value + "&anho=" + Periodo; }
        }
        string _mode;

        public frmTipoCambio(string Parametro)
        {
            InitializeComponent();
        }

        private void frmTipoCambio_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            btnEliminar.Enabled = grdData.Rows.Any();
            tcDetalle.Enabled = false;
            _mode = "New";
            lblPeriodo.Text = string.Format("Periodo: {0}", Periodo);
            cboMes.Value = DateTime.Now.Month.ToString("00");
        }

        #region Cabecera
        private void ultraButton2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(TipoCambioUrl);
        }

        private void btnActualizaSUNAT_Click(object sender, EventArgs e)
        {
                ObtenerTipoCambioSunat();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            OperationResult _OperationResult = new OperationResult();
            _tipodecambioDto = _objTipoCambioBl.ObtenerTipoCambio(ref _OperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_CodTipoCambio"].Value.ToString()));
            if (_tipodecambioDto == null) return;
            txtFecha.Text = _tipodecambioDto.d_FechaTipoC.Value.Date.ToString();
            txtValorCompra.Text = _tipodecambioDto.d_ValorCompra.ToString();
            txtValorVenta.Text = _tipodecambioDto.d_ValorVenta.ToString();
            txtValorCompraC.Text = _tipodecambioDto.d_ValorCompraContable.ToString();
            txtValorVentaC.Text = _tipodecambioDto.d_ValorVentaContable.ToString();
            tcDetalle.Enabled = true;
            _mode = "Edit";
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            tcDetalle.Enabled = true;
            LimpiaDetalle();
            txtValorCompra.Focus();
            _mode = "New";
            txtFecha.Enabled = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                _objTipoCambioBl.Eliminartipodecambio(ref objOperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_CodTipoCambio"].Value.ToString()), Globals.ClientSession.GetAsList());
                Buscar();
                LimpiaDetalle();
                tcDetalle.Enabled = grdData.Rows.Count() != 0;
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;
            }
        }

        private void BindGrid()
        {
            int i;
            var mes = int.TryParse(cboMes.Value.ToString(), out i) ? i : DateTime.Now.Month;
            var objData = GetData(Periodo, mes);
            grdData.DataSource = objData;
        }

        private List<tipodecambioDto> GetData(int periodo, int mes)
        {
            var objOperationResult = new OperationResult();
            var _objData = _objTipoCambioBl.ObtenerListadoTipoCambio(ref objOperationResult, periodo, mes);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        #endregion

        #region Detalles
        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (txtFecha.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Fecha.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFecha.Focus();
                    return;
                }

                if (txtValorCompra.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Valor de Compra.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorCompra.Focus();
                    return;
                }

                if (txtValorVenta.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Valor de Venta.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValorVenta.Focus();
                    return;
                }
                txtValorCompraC.Text = txtValorCompra.Text;
                txtValorVentaC.Text = txtValorVenta.Text;
                if (_mode == "New")
                {
                    _tipodecambioDto = new tipodecambioDto
                    {
                        d_FechaTipoC = txtFecha.Value,
                        d_ValorCompra = decimal.Parse(txtValorCompra.Text),
                        d_ValorVenta = decimal.Parse(txtValorVenta.Text),
                        d_ValorCompraContable = decimal.Parse(txtValorCompraC.Text),
                        d_ValorVentaContable = decimal.Parse(txtValorVentaC.Text),
                        i_Periodo = 2015
                    };
                    if (ConsultarExistente(txtFecha.Value) == false)
                    {
                        _objTipoCambioBl.InsertarTipoCambio(ref objOperationResult, _tipodecambioDto, Globals.ClientSession.GetAsList());
                    }
                    else
                    {
                        UltraMessageBox.Show("Ya existe un Tipo de Cambio para esta fecha.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (_mode == "Edit")
                {
                    _tipodecambioDto.d_FechaTipoC = txtFecha.Value;
                    _tipodecambioDto.d_ValorCompra = decimal.Parse(txtValorCompra.Text);
                    _tipodecambioDto.d_ValorVenta = decimal.Parse(txtValorVenta.Text);
                    _tipodecambioDto.d_ValorCompraContable = decimal.Parse(txtValorCompraC.Text);
                    _tipodecambioDto.d_ValorVentaContable = decimal.Parse(txtValorVentaC.Text);
                    _tipodecambioDto.i_Periodo = 2015;
                    _objTipoCambioBl.Actualizartipodecambio(ref objOperationResult, _tipodecambioDto, Globals.ClientSession.GetAsList());
                }
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    txtFecha.Enabled = false;
                    Buscar();
                    if (_tipodecambioDto.d_FechaTipoC != null)
                        MantenerSeleccion(_tipodecambioDto.d_FechaTipoC.Value.Date);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Clases/Validaciones
        private void LimpiaDetalle()
        {
            txtFecha.Value = DateTime.Now.Date;
            txtValorCompra.Text = String.Empty;
            txtValorVenta.Text = String.Empty;
            txtValorCompraC.Text = string.Empty;
            txtValorVentaC.Text = string.Empty;
        }

        private bool ConsultarExistente(DateTime fecha)
        {
            var objOperationResult = new OperationResult();
            return _objTipoCambioBl.ExisteTipoCambioPorFecha(ref objOperationResult, fecha);
        }

        private void MantenerSeleccion(DateTime ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (Convert.ToDateTime(row.Cells["d_FechaTipoC"].Value.ToString()).Equals(ValorSeleccionado))
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void ObtenerTipoCambioSunat()
        {
            try
            {
                Encoding objEncoding = Encoding.GetEncoding("ISO-8859-1");
                //WebProxy objWebProxy = new WebProxy("proxy", 80);
                CookieCollection objCookies = new CookieCollection();

                //USANDO GET
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(TipoCambioUrl);
                getRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.UserAgent = ".NET Framework 4.0";
                getRequest.Method = "GET";

                getRequest.CookieContainer = new CookieContainer();
                getRequest.CookieContainer.Add(objCookies);

                string sGetResponse = string.Empty;

                using (HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse())
                {
                    objCookies = getResponse.Cookies;

                    using (StreamReader srGetResponse = new StreamReader(getResponse.GetResponseStream(), objEncoding))
                    {
                        sGetResponse = srGetResponse.ReadToEnd();
                    }
                }
                //Obtenemos Informacion
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sGetResponse);

                var nodesTr = document.DocumentNode.SelectNodes("//table[@class='class=\"form-table\"']//tr");

                if (nodesTr != null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Día", typeof(String));
                    dt.Columns.Add("Compra", typeof(String));
                    dt.Columns.Add("Venta", typeof(String));

                    int iNumFila = 0;
                    foreach (HtmlNode Node in nodesTr)
                    {
                        if (iNumFila > 0)
                        {
                            int iNumColumna = 0;
                            DataRow dr = dt.NewRow();
                            foreach (HtmlNode subNode in Node.Elements("td"))
                            {

                                if (iNumColumna == 0) dr = dt.NewRow();

                                string sValue = subNode.InnerHtml.ToString().Trim();
                                sValue = System.Text.RegularExpressions.Regex.Replace(sValue, "<.*?>", " ");
                                dr[iNumColumna] = sValue;

                                iNumColumna++;

                                if (iNumColumna == 3)
                                {
                                    dt.Rows.Add(dr);
                                    iNumColumna = 0;
                                }
                            }
                        }
                        iNumFila++;
                    }
                    dt.AcceptChanges();

                    //Aca se ingresa los registros recogidos de la pagina de la sunat a nuestra BD
                    string _MesAnio = "/" + cboMes.Value + "/" + Periodo;
                    Int32 _RegActual = 0, _Dia = 0;
                    decimal _UltimoRegCompra = 0, _UltimoRegVenta = 0;
                    //se recorre el datatable vaciando los datos a la entidad del tipo de cambio...
                    foreach (DataRow row in dt.Rows)
                    {
                        //valido que el dia seleccionado no exista antes de guardar
                        if (!ConsultarExistente(Convert.ToDateTime(row[0] + _MesAnio)))
                        {
                            OperationResult objOperationResult = new OperationResult();
                            _tipodecambioDto = new tipodecambioDto();
                            _tipodecambioDto.d_FechaTipoC = Convert.ToDateTime(row[0] + _MesAnio);
                            _Dia = Convert.ToInt32(row[0]);
                            _tipodecambioDto.d_ValorCompra = Convert.ToDecimal(row[1]);
                            _tipodecambioDto.d_ValorVenta = Convert.ToDecimal(row[2]);
                            _tipodecambioDto.d_ValorCompraContable = Convert.ToDecimal(row[1]);
                            _tipodecambioDto.d_ValorVentaContable = Convert.ToDecimal(row[2]);
                            //_UltimoRegCompra = Convert.ToDecimal(row[1]);
                            //_UltimoRegVenta = Convert.ToDecimal(row[2]);
                            _tipodecambioDto.i_Periodo = Periodo;
                            _objTipoCambioBl.InsertarTipoCambio(ref objOperationResult, _tipodecambioDto, Globals.ClientSession.GetAsList());
                        }
                        //aca me aseguro que se guarden los ultimos valores de compra y venta por si los ultimos valores no pasaron la validacion
                        
                        _RegActual++;
                        //-------si hay un salto de fecha esta parte rellena los dias sin datos y coloca los valores del ultimo dia.
                        if (_RegActual < _Dia)
                        {
                            for (int i = _RegActual; i < _Dia; i++)
                            {
                                if (!ConsultarExistente(Convert.ToDateTime(i + _MesAnio)))
                                {
                                    OperationResult objOperationResult = new OperationResult();
                                    _tipodecambioDto = new tipodecambioDto();
                                    _tipodecambioDto.d_FechaTipoC = Convert.ToDateTime(i + _MesAnio);
                                    _tipodecambioDto.d_ValorCompra = _UltimoRegCompra;
                                    _tipodecambioDto.d_ValorVenta = _UltimoRegVenta;
                                    _tipodecambioDto.d_ValorCompraContable = Convert.ToDecimal(row[1]);
                                    _tipodecambioDto.d_ValorVentaContable = Convert.ToDecimal(row[2]);
                                    _tipodecambioDto.i_Periodo = Periodo;
                                    _objTipoCambioBl.InsertarTipoCambio(ref objOperationResult, _tipodecambioDto, Globals.ClientSession.GetAsList());
                                }
                            }
                        }

                        _UltimoRegCompra = Convert.ToDecimal(row[1]);
                        _UltimoRegVenta = Convert.ToDecimal(row[2]);
                        //-----------------------------------------------------------------------------------------
                    }
                    //refresco la grilla.
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message, "Error al Obtener datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnActulizarTodoFebrero_Click(object sender, EventArgs e)
        {
          //  Utils.Windows.
        }

        private void cboMes_ValueChanged(object sender, EventArgs e)
        {
            Buscar();
        }

        void Buscar()
        {
            BindGrid();
            btnEliminar.Enabled = grdData.Rows.Any();
            if (!grdData.Rows.Any())
            {
                LimpiaDetalle();
            }
        }
    }
}
