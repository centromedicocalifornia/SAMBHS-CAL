using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class compraDto
    {
        public string NombreProveedor { get; set; }
        public string CodigoProveedor { get; set; }
        public string RUCProveedor { get; set; }
        public string NombreDetraccion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string NroRegistro { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string RegistroKey { get; set; }
        public bool FlagPendientePorCobrar { get; set; }
        public string RegistroOrigen { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string Moneda { get; set; }
        public string CtasVistaRapida { get; set; }
        public IEnumerable<string> NroRegistroOrigen { get; set; }
        public IEnumerable<string> ListaCuentasUsadas { get; set; }
        public string Origen { get; set; }
        public decimal Saldo { get; set; }
        public string v_Rectificacion { get; set; }
        
    }

    public class ConsultaComprasRetencion
    {
        public string TipoDocumento { get; set; }
        public string v_SerieDocumento { get; set; }
        public string  v_CorrelativoDocumento { get; set; }
        public int i_IdTipoDocumento { get; set; }
        public decimal d_MontoPago { get; set; }
        public decimal d_MontoRetenido { get; set; }
        public string v_IdCompra { get; set; }
    }

}
