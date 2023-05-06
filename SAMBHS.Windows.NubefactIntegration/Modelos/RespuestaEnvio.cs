// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using SAMBHS.Windows.NubefactIntegration.Modelos;
//
//    var respuestaEnvio = RespuestaEnvio.FromJson(jsonString);

namespace SAMBHS.Windows.NubefactIntegration.Modelos
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RespuestaEnvio : IRespuesta
    {
        [JsonProperty("tipo_de_comprobante")]
        public long TipoDeComprobante { get; set; }

        [JsonProperty("serie")]
        public string Serie { get; set; }

        [JsonProperty("numero")]
        public long Numero { get; set; }

        [JsonProperty("enlace")]
        public string Enlace { get; set; }

        [JsonProperty("aceptada_por_sunat")]
        public bool AceptadaPorSunat { get; set; }

        [JsonProperty("sunat_description")]
        public string SunatDescription { get; set; }

        [JsonProperty("sunat_note")]
        public string SunatNote { get; set; }

        [JsonProperty("sunat_responsecode")]
        public string SunatResponsecode { get; set; }

        [JsonProperty("sunat_soap_error")]
        public string SunatSoapError { get; set; }

        [JsonProperty("pdf_zip_base64")]
        public object PdfZipBase64 { get; set; }

        [JsonProperty("xml_zip_base64")]
        public object XmlZipBase64 { get; set; }

        [JsonProperty("cdr_zip_base64")]
        public object CdrZipBase64 { get; set; }

        [JsonProperty("cadena_para_codigo_qr")]
        public string CadenaParaCodigoQr { get; set; }

        [JsonProperty("codigo_hash")]
        public string CodigoHash { get; set; }

        [JsonProperty("codigo_de_barras")]
        public string CodigoDeBarras { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("digest_value")]
        public string DigestValue { get; set; }

        [JsonProperty("enlace_del_pdf")]
        public string EnlaceDelPdf { get; set; }

        [JsonProperty("enlace_del_xml")]
        public string EnlaceDelXml { get; set; }

        [JsonProperty("enlace_del_cdr")]
        public string EnlaceDelCdr { get; set; }

        [JsonProperty("invoice")]
        public Invoice Invoice { get; set; }
    }

    public partial class Invoice
    {
        [JsonProperty("tipo_de_comprobante")]
        public long TipoDeComprobante { get; set; }

        [JsonProperty("serie")]
        public string Serie { get; set; }

        [JsonProperty("numero")]
        public long Numero { get; set; }

        [JsonProperty("enlace")]
        public string Enlace { get; set; }

        [JsonProperty("aceptada_por_sunat")]
        public bool AceptadaPorSunat { get; set; }

        [JsonProperty("sunat_description")]
        public string SunatDescription { get; set; }

        [JsonProperty("sunat_note")]
        public object SunatNote { get; set; }

        [JsonProperty("sunat_responsecode")]
        public string SunatResponsecode { get; set; }

        [JsonProperty("sunat_soap_error")]
        public string SunatSoapError { get; set; }

        [JsonProperty("pdf_zip_base64")]
        public object PdfZipBase64 { get; set; }

        [JsonProperty("xml_zip_base64")]
        public object XmlZipBase64 { get; set; }

        [JsonProperty("cdr_zip_base64")]
        public object CdrZipBase64 { get; set; }

        [JsonProperty("cadena_para_codigo_qr")]
        public string CadenaParaCodigoQr { get; set; }

        [JsonProperty("codigo_hash")]
        public string CodigoHash { get; set; }

        [JsonProperty("digest_value")]
        public string DigestValue { get; set; }

        [JsonProperty("codigo_de_barras")]
        public string CodigoDeBarras { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

    public partial class RespuestaEnvio
    {
        public static RespuestaEnvio FromJson(string json)
        {
            return JsonConvert.DeserializeObject<RespuestaEnvio>(json, RespuestaEnvioConverter.Settings);
        }
    }

    public static class RespuestaEnvioSerialize
    {
        public static string ToJson(this RespuestaEnvio self)
        {
            return JsonConvert.SerializeObject(self, RespuestaEnvioConverter.Settings);
        }
    }

    internal static class RespuestaEnvioConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
