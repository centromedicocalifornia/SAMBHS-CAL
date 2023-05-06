namespace SAMBHS.Windows.NubefactIntegration.Modelos
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ConsultaEstado
    {
        [JsonProperty("operacion")]
        public string Operacion { get; set; }

        [JsonProperty("tipo_de_comprobante")]
        public string TipoDeComprobante { get; set; }

        [JsonProperty("serie")]
        public string Serie { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }
    }

    public partial class ConsultaEstado
    {
        public static ConsultaEstado FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ConsultaEstado>(json, ConsultaEstadoConverter.Settings);
        }
    }

    public static class ConsultaEstadoSerialize
    {
        public static string ToJson(this ConsultaEstado self)
        {
            return JsonConvert.SerializeObject(self, ConsultaEstadoConverter.Settings);
        }
    }

    internal static class ConsultaEstadoConverter
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
