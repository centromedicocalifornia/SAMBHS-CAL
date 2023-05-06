using System;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SAMBHS.Security.BL
{
    /// <summary>
    /// Consulta en Reniec.
    /// </summary>
    public class ConsultaPersona
    {
        #region Propiedades

        /// <summary>
        /// Devuelve la imagen para el reto capcha
        /// </summary>
        public Image GetCapcha { get { return ReadCapcha(); } }

        /// <summary>
        /// Si no Hubo error en la lectura de datos devuelve los nombres 
        /// <para>de la persona caso contrario devuelve <c>string</c>.Empty</para>
        /// </summary>
        public string Nombres { get { return _nombres; } }

        /// <summary>
        /// Si no Hubo error en la lectura de datos devuelve el Apellido Paterno
        /// <para>de la persona caso contrario devuelve <c>string</c>.Empty</para>
        /// </summary>
        public string ApePaterno { get { return _apePaterno; } }

        /// <summary>
        /// Si no Hubo error en la lectura de datos devuelve el Apellido Materno
        /// <para>de la persona caso contrario devuelve <c>string</c>.Empty</para>
        /// </summary>
        public string ApeMaterno { get { return _apeMaterno; } }

        /// <summary>
        /// Devuelve el resultado de la busqueda de DNI
        /// </summary>
        public Resul GetResul { get { return _state; } }

        #endregion

        #region Fields
        private Resul _state;
        private string _nombres;
        private string _apePaterno;
        private string _apeMaterno;
        public DateTime _fecha;
        private readonly CookieContainer _myCookie;
        #endregion

        #region Private Method
        private Image ReadCapcha()
        {
            try
            {
                var myWebRequest = (HttpWebRequest)WebRequest.Create("http://app1.susalud.gob.pe/registro/Home/GeneraCaptcha?id=brJ8wrEXd77");
                myWebRequest.CookieContainer = _myCookie;
                myWebRequest.Proxy = null;
                myWebRequest.Credentials = CredentialCache.DefaultCredentials;

                using (var myWebResponse = (HttpWebResponse)myWebRequest.GetResponse())
                {
                    using (var myImgStream = myWebResponse.GetResponseStream())
                    {
                        return myImgStream != null ? Image.FromStream(myImgStream) : null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

        #region Enums
        public enum Resul
        {
            /// <summary>
            /// Se encontro la persona
            /// </summary>
            Ok = 0,
            /// <summary>
            /// No se encontro la persona
            /// </summary>
            NoResul = 1,
            /// <summary>
            /// la imagen capcha no es valida
            /// </summary>
            ErrorCapcha = 2,
            /// <summary>
            /// Error no especificado
            /// </summary>
            Error = 3,
        }
        #endregion

        #region Public Method
        public ConsultaPersona()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            _myCookie = new CookieContainer();
        }

        public void GetInfo(string numDni, string imgCapcha)
        {
            //string URI = "http://app1.susalud.gob.pe/registro/Home/ConsultaAfiliadoPersona";
            string URI = "https://app1.susalud.gob.pe/registro";
            string myParameters = string.Format("cboPais=PER&cboTDoc=1&txtNroDoc={0}&txtCaptcha={1}", numDni, imgCapcha);

            var request = (HttpWebRequest)WebRequest.Create(URI);

            var data = Encoding.ASCII.GetBytes(myParameters);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.CookieContainer = _myCookie;
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var r = response.GetResponseStream();

            if (r == null)
            {
                _state = Resul.Error;
                return;
            }

            var responseString = new StreamReader(r).ReadToEnd();
            if (responseString.Contains("El texto de la imagen es incorrecto, intente nuevamente"))
            {
                _state = Resul.ErrorCapcha;
                return;
            }

            var document = new HtmlDocument();
            document.LoadHtml(responseString);
            var tablaFull = document.GetElementbyId("capabotons");
            if (tablaFull == null)
            {
                var tablaRes = document.DocumentNode.SelectSingleNode("//*[contains(@class,'nombres1')]");
                if (tablaRes == null)
                {
                    _state = Resul.NoResul;
                    return;
                }


                var d = tablaRes.InnerText.Replace("Apellidos y nombres: ", "");
                var rr = d.Split(',');
                _apePaterno = rr[0].Split(' ')[0];
                _apeMaterno = rr[0].Split(' ')[1];
                _nombres = rr[1];
                _fecha = DateTime.Now;
            }
            else
            {
                var scr = tablaFull.InnerHtml.Replace("<input type=\"button\" value=\"Imprimir Consulta\" onclick=\"javascript:CallPrintConstancia('imprime_constancia',", "");
                var i = scr.IndexOf(",'<div style", StringComparison.Ordinal);
                scr = scr.Substring(0, i).Replace("'", "");
                var rr = scr.Split(',');
                _apePaterno = rr[0].Split(' ')[0];
                _apeMaterno = rr[0].Split(' ')[1];
                _nombres = rr[1];
                _fecha = DateTime.ParseExact(rr[3], "dd/MM/yyyy", new CultureInfo("es-ES"));
            }

            _state = Resul.Ok;
        }
        #endregion
    }

    /// <summary>
    /// Consulta en SUNAT
    /// </summary>
    public class ConsultaContribuyente
    {
        #region Propiedades

        public Image GetCapcha { get { return ReadCapcha(); } }

        public string RazonSocial { get { return _razonSocial; } }

        public string NombreComercial { get { return _nombreComercial; } }

        public string FechaInscripcion { get { return _fechaInscripcion; } }

        public string FechaInicioActividades { get { return _fechaInicioActividades; } }

        public string EstadoContribuyente { get { return _estadoContribuyente; } }

        public string Condicion { get { return _condicion; } }

        public string DireccionFiscal { get { return _direccionFiscal; } }

        public string Telefonos { get { return _telefonos; } }

        public string ActividadComercioExterior { get { return _actividadComercioExterior; } }

        public string SistemaContable { get { return _sistemaContable; } }

        public string TipoContribuyente { get { return _tipoContribuyente; } }

        public Resul GetResul { get { return _state; } }

        #endregion

        #region Fields
        private Resul _state;
        private string _razonSocial;
        private string _nombreComercial;
        private string _fechaInscripcion;
        private string _fechaInicioActividades;
        private string _estadoContribuyente;
        private string _condicion;
        private string _direccionFiscal;
        private string _telefonos;
        private string _actividadComercioExterior;
        private string _sistemaContable;
        private string _tipoContribuyente;
        private string _actividadEconomica;
        private readonly CookieContainer _myCookie;
        #endregion

        #region Private Method
        private Image ReadCapcha()
        {
            var myWebRequest = (HttpWebRequest)WebRequest.Create("http://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/captcha?accion=image");
            myWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0";//esto creo que lo puse por gusto :/
            myWebRequest.CookieContainer = _myCookie;
            myWebRequest.Credentials = CredentialCache.DefaultCredentials;
            myWebRequest.Proxy = null;
            myWebRequest.ContentType = "text/xml;charset=\"utf-8\"";
            myWebRequest.Accept = "text/xml";
            myWebRequest.Method = "POST";

            using (var myWebResponse = myWebRequest.GetResponse())
            {
                var myImgStream = myWebResponse.GetResponseStream();
                return myImgStream != null ? Image.FromStream(myImgStream) : null;
            }
        }
        #endregion

        #region Enum
        public enum Resul
        {
            Ok = 0,
            NoResul = 1,
            ErrorCapcha = 2,
            Error = 3,
        }
        #endregion

        #region Public Methods
        public ConsultaContribuyente()
        {
            _myCookie = null;
            _myCookie = new CookieContainer();

            //Permitir SSL
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
        }

        public void GetInfo(string numDni, string imgCapcha)
        {
            var tipoPersona = numDni.Substring(0, 1) == "2" ? "1" : "0";
            var myUrl = string.Format("http://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/jcrS03Alias?accion=consPorRuc&nroRuc={0}&codigo={1}&tipdoc={2}", numDni, imgCapcha, tipoPersona);

            var myWebRequest = (HttpWebRequest)WebRequest.Create(myUrl);
            myWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0";//esto creo que lo puse por gusto :/
            myWebRequest.CookieContainer = _myCookie;
            myWebRequest.Credentials = CredentialCache.DefaultCredentials;
            myWebRequest.Proxy = null;
            myWebRequest.ContentType = "text/xml;charset=\"utf-8\"";
            var myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();

            var myStream = myHttpWebResponse.GetResponseStream();
            if (myStream == null)
            {
                _state = Resul.Error;
                return;
            }

            var myStreamReader = new StreamReader(myStream, Encoding.GetEncoding("ISO-8859-1"));
            var s = myStreamReader.ReadToEnd();

            HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(s);
            var tabla = document.DocumentNode.SelectSingleNode("//*[contains(@class,'form-table')]");

            if (tabla == null)
            {
                _state = Resul.ErrorCapcha;
                return;
            }

            var tablaSunat = tabla.InnerText;
            var resul = tablaSunat.Split(new[] { '\r', '\n', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var indexRs = resul.FindIndex(p => p.Contains("N&uacute;mero de RUC:")) + 1;
            var indexTipoC = resul.FindIndex(p => p.Contains("Tipo Contribuyente:")) + 1;
            var indexNombreC = resul.FindIndex(p => p.Contains("Nombre Comercial:")) + 1;
            var indexFechaInsc = resul.FindIndex(p => p.Contains("Fecha de Inscripci&oacute;n:")) + 1;
            var indexFechaInicioAc = resul.FindIndex(p => p.Contains("Fecha de Inicio de Actividades:")) + 1;
            var indexEstadoC = resul.FindIndex(p => p.Contains("Estado del Contribuyente:")) + 1;
            var indexCondicionC = resul.FindIndex(p => p.Contains("Condici&oacute;n del Contribuyente:")) + 3;
            var indexDireccionC = resul.FindIndex(p => p.Contains("Direcci&oacute;n del Domicilio Fiscal:")) + 1;

            var razonSocial = resul[indexRs];
            _razonSocial = razonSocial.Remove(0, razonSocial.IndexOf('-') + 1);
            _tipoContribuyente = resul[indexTipoC].Trim();
            _nombreComercial = resul[indexNombreC].Trim();
            _fechaInscripcion = resul[indexFechaInsc].Trim();
            _fechaInicioActividades = resul[indexFechaInicioAc].Trim();
            _estadoContribuyente = resul[indexEstadoC].Trim();
            _condicion = resul[indexCondicionC].Trim();
            _direccionFiscal = resul[indexDireccionC].Trim();
            _state = Resul.Ok;
        }
        #endregion
    }

    public class ConsultaValidezComprobante
    {
        private readonly CookieContainer _myCookie;

        public Image ReadCapcha()
        {
            var myWebRequest = (HttpWebRequest)WebRequest.Create("http://www.sunat.gob.pe/ol-ti-itconsvalicpe/captcha?accion=image&nmagic=0");
            myWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0";//esto creo que lo puse por gusto :/
            myWebRequest.CookieContainer = _myCookie;
            myWebRequest.Credentials = CredentialCache.DefaultCredentials;
            myWebRequest.Proxy = null;
            myWebRequest.ContentType = "text/xml;charset=\"utf-8\"";
            myWebRequest.Accept = "text/xml";
            myWebRequest.Method = "POST";

            using (var myWebResponse = myWebRequest.GetResponse())
            {
                var myImgStream = myWebResponse.GetResponseStream();
                return myImgStream != null ? Image.FromStream(myImgStream) : null;
            }
        }

        public string ConsultarValidez(string num_ruc, string tipocomprobante, string cod_docide,
            string num_docide, string num_serie, string num_comprob, DateTime fec_emision, decimal cantidad, string codigo)
        {
            try
            {
                const string urlBase = "http://www.sunat.gob.pe/ol-ti-itconsvalicpe/ConsValiCpe.htm";
                var parameters = string.Empty;

                switch (tipocomprobante)
                {
                    case "01":
                        parameters =
                            string.Format(
                                "accion=CapturaCriterioValidez&num_ruc={0}&tipocomprobante={1}&cod_docide={2}&num_docide={3}&num_serie={4}&num_comprob={5}&fec_emision={6}&cantidad={7}&codigo={8}",
                                num_ruc, "03", cod_docide, num_docide, num_serie.ToLower(), num_comprob.ToLower(), fec_emision.ToShortDateString(), cantidad.ToString("##.00"), codigo);
                        break;

                    case "03":
                        parameters =
                            string.Format(
                                "accion=CapturaCriterioValidez&num_ruc={0}&tipocomprobante={1}&num_serie={2}&num_comprob={3}&fec_emision={4}&codigo={5}",
                                num_ruc, "06", num_serie.ToLower(), num_comprob.ToLower(), fec_emision.ToShortDateString(), codigo);
                        break;

                    case "07":
                        break;

                    case "08":
                        break;

                    default:
                        break;
                }

                if (string.IsNullOrWhiteSpace(parameters)) return string.Empty;

                var webReq = string.Format("{0}?{1}", urlBase, parameters);
                var myWebRequest = (HttpWebRequest)WebRequest.Create(webReq);
                myWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0";//esto creo que lo puse por gusto :/
                myWebRequest.CookieContainer = _myCookie;
                myWebRequest.Credentials = CredentialCache.DefaultCredentials;
                myWebRequest.Proxy = null;

                var myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                var myStream = myHttpWebResponse.GetResponseStream();
                if (myStream == null) return null;
                var myStreamReader = new StreamReader(myStream);
                var s = myStreamReader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(s);
                var result = doc.DocumentNode.SelectNodes("//*[contains(@class,'beta')]");
                if (result != null && result.Count == 2)
                {
                    var text = result[1].InnerText.Replace("\t", "").Trim();
                    return Regex.Replace(text, @"[ ]+", " ");
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Format("* {0}", ex.Message));
                if (ex.InnerException != null) sb.AppendLine(string.Format("* {0}", ex.InnerException.Message));
                return sb.ToString();
            }
        }

        public ConsultaValidezComprobante()
        {
            _myCookie = null;
            _myCookie = new CookieContainer();

            //Permitir SSL
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
        }
    }
}

