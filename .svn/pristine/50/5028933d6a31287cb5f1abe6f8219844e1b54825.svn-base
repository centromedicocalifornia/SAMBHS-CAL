namespace SAMBHS.Common.BE.Custom
{
    public class DocumentoRetencionPendiente
    {
        public string v_IdDocumentoRetencionDetalle { get; set; }
        public string v_IdDocumentoRetencion { get; set; }
        public int i_IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string v_SerieDocumento { get; set; }
        public string v_CorrelativoDocumento { get; set; }
        public string NroDocumento {
            get { return v_SerieDocumento + "-" + v_CorrelativoDocumento; }
        }

        public decimal d_MontoRetenido { get; set; }
        public string IdAnexo { get; set; }
        public string RucAnexo { get; set; }
        public string RazonSocial { get; set; }
        public string CodAnexo { get; set; }
        public decimal Importe { get; set; }
        public decimal Cambio { get; set; }
        public int IdMoneda { get; set; }
    }
}
