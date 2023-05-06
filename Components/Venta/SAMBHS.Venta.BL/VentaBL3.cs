using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.CommonWIN;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Venta.BL
{
    public static class VentaHelper
    {
        public static string GetPdf(string pstrIdVenta, TipoConstancia tipoConstancia, 
            TipoRepresentacion tipoRepresentacion)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var vta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta == pstrIdVenta);
                    if (vta == null) return string.Empty;
                    switch (tipoConstancia)
                    {
                        case TipoConstancia.ENVIO:
                            if (string.IsNullOrWhiteSpace(vta.v_EnlaceEnvio)) return string.Empty;
                            switch (tipoRepresentacion)
                            {
                                case TipoRepresentacion.PDF: return vta.v_EnlaceEnvio + ".pdf";
                                case TipoRepresentacion.XML: return vta.v_EnlaceEnvio + ".xml";
                                case TipoRepresentacion.CDR: return vta.v_EnlaceEnvio + ".cdr";
                                default: return string.Empty;
                            }

                        case TipoConstancia.BAJA:
                            if (string.IsNullOrWhiteSpace(vta.v_EnlaceBaja)) return string.Empty;
                            switch (tipoRepresentacion)
                            {
                                case TipoRepresentacion.PDF: return vta.v_EnlaceBaja + ".pdf";
                                case TipoRepresentacion.XML: return vta.v_EnlaceBaja + ".xml";
                                case TipoRepresentacion.CDR: return vta.v_EnlaceBaja + ".cdr";
                                default: return string.Empty;
                            }
                    }
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public enum TipoRepresentacion
        {
            PDF,
            XML,
            CDR,
        }

        public enum TipoConstancia
        {
            ENVIO,
            BAJA
        }
    }
}
