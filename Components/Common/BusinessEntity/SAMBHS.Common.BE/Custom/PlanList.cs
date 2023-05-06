using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class PlanList
    {
        public int? i_PlanId { get; set; }
        public string v_OrganizationSeguroId { get; set; }
        public string v_ProtocoloId { get; set; }
        public string v_IdUnidadProductiva { get; set; }
        public int? i_EsDeducible { get; set; }
        public int? i_EsCoaseguro { get; set; }
        public decimal? d_Importe { get; set; }
        public decimal? d_ImporteCo { get; set; }
        public string v_ComentaryUpdate { get; set; }
    }

    public class Receta
    {
        public string NombreMedicamento { get; set; }
        public string i_IdReceta { get; set; }     
        public string v_DiagnosticRepositoryId { get; set; }
        public decimal? d_Cantidad { get; set; }
        public string v_Posologia { get; set; }
        public string v_Duracion { get; set; }    
        public DateTime? t_FechaFin { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string v_Lote { get; set; }
        public int? i_IdAlmacen { get; set; }
        public int? i_Lleva { get; set; }
        public string llevo { get; set; }
        public int? i_NoLleva { get; set; }
        public string v_IdVentaPaciente { get; set; }
        public string v_IdVentaAseguradora { get; set; }
        public string v_IdUnidadProductiva { get; set; }       
        public decimal? d_SaldoPaciente { get; set; }
        public decimal? d_SaldoAseguradora { get; set; }
        public string v_ServiceId { get; set; }
        public string v_ComentaryUpdate { get; set; }
        public string v_ReceiptId { get; set; }
        public int i_MedicoId { get; set; }
        public string v_MedicoName { get; set; }
        public decimal d_total { get; set; }

    }

    public class Diagnostico
    {
        public string v_DiagnosticRepositoryId { get; set; }
        public string v_ServiceId { get; set; }
        public decimal v_DiseasesId { get; set; }

    }
    
}
