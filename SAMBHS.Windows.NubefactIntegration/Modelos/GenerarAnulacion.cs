namespace SAMBHS.Windows.NubefactIntegration.Modelos
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GeneraAnulacion
    {
        [JsonProperty("operacion")]
        public string Operacion { get; set; }

        [JsonProperty("tipo_de_comprobante")]
        public string TipoDeComprobante { get; set; }

        [JsonProperty("serie")]
        public string Serie { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("motivo")]
        public string Motivo { get; set; }

        [JsonProperty("codigo_unico")]
        public string CodigoUnico { get; set; }
    }

    public partial class GeneraAnulacion
    {
        public static GeneraAnulacion FromJson(string json)
        {
            return JsonConvert.DeserializeObject<GeneraAnulacion>(json, GeneraAnulacionConverter.Settings);
        }
    }

    public static class GeneraAnulacionSerialize
    {
        public static string ToJson(this GeneraAnulacion self)
        {
            return JsonConvert.SerializeObject(self, GeneraAnulacionConverter.Settings);
        }
    }

    internal static class GeneraAnulacionConverter
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
