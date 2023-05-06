
namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class RecetaDto
    {
        public bool b_Seleccionar { get; set; }
        public string v_ServiceId { get; set; }
        public string IdReceta { get; set; }
        public string IdProductoDetalle { get; set; }
        public string DiagnosticRepositoryId { get; set; }
        public string Medicamento { get; set; }
        public decimal Cantidad { get; set; }
        public int IdUnidadMedida { get; set; }

        public decimal PrecioVenta { get; set; }
        public decimal Igv { get; set; }
        public decimal Pu { get; set; }
        public decimal ValorV { get; set; }
        public string CodigoInterno { get; set; }
        public float r_MedicineDiscount { get; set; }
        public string ProductName { get; set; }
        public int i_IdLista { get; set; }
        public string v_IdListaPrecios { get; set; }
        public string v_ProtocolId { get; set; }
        public string Result { get; set; }
        public string UnidadProductiva { get; set; }
        public decimal SaldoPaciente { get; set; }
        public decimal SaldoAseguradora { get; set; }
        public string TicketHosp { get; set; }
    }
}
