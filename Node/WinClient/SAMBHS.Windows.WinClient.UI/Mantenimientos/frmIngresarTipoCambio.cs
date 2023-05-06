using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmIngresarTipoCambio : Form
    {
        private Task _buscarTipoCambio;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public FrmIngresarTipoCambio()
        {
            InitializeComponent();
        }

        private void frmIngresarTipoCambio_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            linkLabel1.Visible = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ObtenerTipoCambioSunat()
        {
            try
            {
                string sUrl = "http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias";

                Encoding objEncoding = Encoding.GetEncoding("ISO-8859-1");
                //WebProxy objWebProxy = new WebProxy("proxy", 80);
                CookieCollection objCookies = new CookieCollection();

                //USANDO GET
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(sUrl);
                getRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.UserAgent = ".NET Framework 4.0";
                getRequest.Method = "GET";

                getRequest.Headers.Add("Cookie", "YPF8827340282Jdskjhfiw_928937459182JAX666=127.0.0.102");
                getRequest.CookieContainer = new CookieContainer();
                getRequest.CookieContainer.Add(objCookies);

                string sGetResponse = string.Empty;

                using (HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse())
                {
                    using (StreamReader srGetResponse = new StreamReader(getResponse.GetResponseStream(), objEncoding))
                    {
                        sGetResponse = srGetResponse.ReadToEnd();
                    }
                }
                //Obtenemos Informacion
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sGetResponse);

                HtmlNodeCollection NodesTr = document.DocumentNode.SelectNodes("//table[@class='class=\"form-table\"']//tr");

                if (NodesTr != null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Día", typeof(String));
                    dt.Columns.Add("Compra", typeof(String));
                    dt.Columns.Add("Venta", typeof(String));

                    int iNumFila = 0;
                    foreach (HtmlNode Node in NodesTr)
                    {
                        if (iNumFila > 0)
                        {
                            int iNumColumna = 0;
                            DataRow dr = dt.NewRow();
                            foreach (HtmlNode subNode in Node.Elements("td"))
                            {

                                if (iNumColumna == 0) dr = dt.NewRow();

                                string sValue = subNode.InnerHtml.Trim();
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
                    string _MesAnio = "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                    Int32 _DiaActual = DateTime.Now.Day, _RegActual = 0, _Dia = 0;
                    decimal _UltimoRegCompra = 0, _UltimoRegVenta = 0;
                    var _tipodecambioDto = new tipodecambioDto();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (((string)row[0]).Trim() == DateTime.Now.Day.ToString())
                        {
                            var objOperationResult = new OperationResult();
                            _tipodecambioDto.d_FechaTipoC = Convert.ToDateTime(row[0] + _MesAnio);
                            _tipodecambioDto.d_ValorCompra = Convert.ToDecimal(row[1]);
                            _tipodecambioDto.d_ValorVenta = Convert.ToDecimal(row[2]);
                            _tipodecambioDto.d_ValorCompraContable = Convert.ToDecimal(row[1]);
                            _tipodecambioDto.d_ValorVentaContable = Convert.ToDecimal(row[2]);
                            _tipodecambioDto.i_Periodo = Globals.ClientSession.i_Periodo.Value;
                            new TipoCambioBL().InsertarTipoCambio(ref objOperationResult, _tipodecambioDto, Globals.ClientSession.GetAsList());
                            if (objOperationResult.Success == 0)
                            {
                                MessageBox.Show(@"Error al guardar el tipo de cambio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                btnBuscar.Enabled = true;
                                btnGuardar.Enabled = true;
                                btnSalir.Enabled = true;
                                return;
                            }

                            txtCompra.Text = (_tipodecambioDto.d_ValorCompra ?? 0).ToString();
                            txtVenta.Text = (_tipodecambioDto.d_ValorVenta ?? 0).ToString();
                            Close();
                        }
                    }

                    btnBuscar.Enabled = true;
                    btnGuardar.Enabled = true;
                    linkLabel1.Visible = false;
                    btnSalir.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message, "Error al Obtener datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnBuscar.Enabled = true;
                btnGuardar.Enabled = true;
                btnSalir.Enabled = true;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            btnBuscar.Enabled = false;
            btnGuardar.Enabled = false;
            btnSalir.Enabled = false;
            linkLabel1.Visible = true;
            var token = _cts.Token;
            _buscarTipoCambio = new Task(() =>
            {
                Invoke(new MethodInvoker(ObtenerTipoCambioSunat));
            }, token, TaskCreationOptions.AttachedToParent);
            _buscarTipoCambio.Start();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVenta.Text) || string.IsNullOrWhiteSpace(txtCompra.Text)) return;
            if (decimal.Parse(txtVenta.Text) == 0 || decimal.Parse(txtCompra.Text) == 0) return;
            var tipodecambioDto = new tipodecambioDto();
            var objOperationResult = new OperationResult();
            tipodecambioDto.d_FechaTipoC = DateTime.Now;
            tipodecambioDto.d_ValorCompra = decimal.Parse(txtCompra.Text);
            tipodecambioDto.d_ValorVenta = decimal.Parse(txtVenta.Text);
            tipodecambioDto.d_ValorCompraContable = decimal.Parse(txtCompra.Text);
            tipodecambioDto.d_ValorVentaContable = decimal.Parse(txtVenta.Text);
            tipodecambioDto.i_Periodo = Globals.ClientSession.i_Periodo ?? 2016;
            new TipoCambioBL().InsertarTipoCambio(ref objOperationResult, tipodecambioDto, Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(@"Error al guardar el tipo de cambio", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnBuscar.Enabled = true;
                btnGuardar.Enabled = true;
                btnSalir.Enabled = true;
                return;
            }
            Close();
        }

        private void txtCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtCompra, e);
        }

        private void txtVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtVenta, e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_cts != null)
                _cts.Cancel();

            Close();
        }
    }
}
