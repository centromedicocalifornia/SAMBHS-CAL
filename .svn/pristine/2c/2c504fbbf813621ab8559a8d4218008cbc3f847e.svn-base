// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using SAMBHS.Windows.NubefactIntegration.Modelos;
//
//    var consultaAnulacion = ConsultaAnulacion.FromJson(jsonString);

namespace SAMBHS.Windows.NubefactIntegration.Modelos
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RespuestaError : IRespuesta
    {
        [JsonProperty("errors")]
        public string Errors { get; set; }

        [JsonProperty("codigo")]
        public long Codigo { get; set; }
    }

    public partial class RespuestaError
    {
        public static RespuestaError FromJson(string json)
        {
            return JsonConvert.DeserializeObject<RespuestaError>(json, RespuestaErrorConverter.Settings);
        }
    }

    public static class RespuestaErrorSerialize
    {
        public static string ToJson(this RespuestaError self)
        {
            return JsonConvert.SerializeObject(self, RespuestaErrorConverter.Settings);
        }
    }

    internal static class RespuestaErrorConverter
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
