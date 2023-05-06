using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public sealed class ImportacionRegistroVentaDto
    {
        private readonly int[] _documentosSoportados = { 1, 3, 7, 8, 12 };
        public int IdTipoDoc { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public string FechaEmision { get; set; }
        public decimal TipoCambio { get; set; }
        public string Moneda { get; set; }
        public string Anulado { get; set; }
        public int IdTipoDocRef { get; set; }
        public string SerieRef { get; set; }
        public string CorrelativoRef { get; set; }
        public string NroDocumentoIdentidad { get; set; }
        public string RazonSocialNombre { get; set; }
        public string Direccion { get; set; }
        public decimal ImporteVenta { get; set; }
        public string CuentaMercaderia { get; set; }
        public string Glosa { get; set; }
        public bool CampoDuplicado { get; set; }
        public bool CuentaExiste { get; set; }
        public string Errores
        {
            get
            {
                int i, ii;
                DateTime t;
                var errores = new StringBuilder();
                if (!Anulado.Trim().Equals("S") && !CuentaExiste)
                    errores.AppendLine("Cuenta de mercadería asignada no existe.!");

                if (CampoDuplicado)
                    errores.AppendLine("Este registro está duplicado o ya fue ingresado anteriormente.!");

                if (!((IList)_documentosSoportados).Contains(IdTipoDoc))
                    errores.AppendLine(string.Format("T. Doc. -> Documento no sportado ({0})", IdTipoDoc));
                
                if (!int.TryParse(Correlativo, out i) || i <= 0)
                    errores.AppendLine(string.Format("Correlativo -> Número de Correlativo incorrecto ({0})", Correlativo));

                if (IdTipoDoc == 7 || IdTipoDoc == 8)
                    if (!int.TryParse(SerieRef, out i) || i <= 0 || !int.TryParse(CorrelativoRef, out ii) || ii <= 0 || !((IList)_documentosSoportados).Contains(IdTipoDocRef))
                        errores.AppendLine("Documento referencia inválido");

                if (!DateTime.TryParse(FechaEmision, out t))
                    errores.AppendLine(string.Format("FechaEmision -> Valor incorrecto ({0})", FechaEmision));

                if (TipoCambio <= 0)
                    errores.AppendLine(string.Format("TipoCambio -> Valor incorrecto ({0})", TipoCambio));

                if (!Moneda.Trim().Equals("S") && !Moneda.Trim().Equals("D"))
                    errores.AppendLine(string.Format("Moneda -> Valor debe ser 'S' (Soles) ó 'D' (Dólares) ({0})", Moneda));

                if (!Anulado.Trim().Equals("S") && !Anulado.Trim().Equals("N"))
                    errores.AppendLine(string.Format("Anulado -> Valor debe ser 'S' (Sí) ó 'N' (No) ({0})", Anulado));

                if (Anulado.Trim().Equals("N"))
                {
                    if (NroDocumentoIdentidad.Trim().Length == 11 && !ValidarRuc(NroDocumentoIdentidad))
                        errores.AppendLine(string.Format("NroDocumentoIdentidad -> RUC Inválido ({0})", NroDocumentoIdentidad));

                    if (IdTipoDoc == 1 && !ValidarRuc(NroDocumentoIdentidad))
                        errores.AppendLine(string.Format("NroDocumentoIdentidad -> RUC Inválido ({0})", NroDocumentoIdentidad));

                    if (IdTipoDoc == 1 && string.IsNullOrWhiteSpace(RazonSocialNombre))
                        errores.AppendLine("RazonSocialNombre -> Ingrese la razón social");

                    if (IdTipoDoc == 1 && NroDocumentoIdentidad.StartsWith("1") && RazonSocialNombre.Split(' ').Count(o => !string.IsNullOrWhiteSpace(o.Trim())) < 3)
                        errores.AppendLine("RazonSocialNombre -> Debe almenos existir 3 palabras conformando el nombre.");

                    if (IdTipoDoc == 1 && string.IsNullOrWhiteSpace(Direccion))
                        errores.AppendLine("Direccion -> Ingrese la Direccion");

                    if (ImporteVenta <= 0)
                        errores.AppendLine("ImporteVenta -> Importe de la venta inválido");

                    if (string.IsNullOrWhiteSpace(CuentaMercaderia))
                        errores.AppendLine("CuentaMercaderia -> Se necesita una cuenta contable para la mercadería.");
                }

                //if (DetalleVenta.Any())
                //{
                //    if(DetalleVenta.Sum(p => p.Total) != ImporteVenta)
                //        errores.AppendLine("DetalleVenta -> La suma del total de los detalles no coincide con el Importe de la cabecera.");

                //    if (DetalleVenta.Any(p => !p.Valido))
                //        errores.AppendLine("DetalleVenta -> Uno o más de los detalles relacionados a este documento están marcando error, por favor revise el log de errores del detalle.");
                //}

                return errores.ToString();
            }
        }
        
        public bool Valido
        {
            get { return string.IsNullOrWhiteSpace(Errores); }
        }

        public string EstadoRegistro { get { return string.IsNullOrWhiteSpace(Errores) ? "CORRECTO" : "CON ERRORES"; } }

        public string KeyVenta {
            get
            {
                int i;
                var serie = int.TryParse(Serie.Trim(), out i) ? i.ToString("0000") : Serie;
                var correlativo = int.TryParse(Correlativo.Trim(), out i) ? i.ToString("00000000") : "??";
                return IdTipoDoc + serie.Trim() + correlativo.Trim();
            }
        }

        public string NroDocumento
        {
            get
            {
                int i;
                var doc = IdTipoDoc == 1
                    ? "FAC"
                    : IdTipoDoc == 3 ? "BOL" : IdTipoDoc == 7 ? "NCR" : IdTipoDoc == 8 ? "NDB" : IdTipoDoc == 12 ? "TCK" : "??";
                var serie = int.TryParse(Serie, out i) ? i.ToString("0000") : Serie;
                var correlativo = int.TryParse(Correlativo, out i) ? i.ToString("00000000") : "??";
                return string.Format("{0} {1}-{2}", doc, serie, correlativo);
            }
        }

        public string NroDocumentoRef
        {
            get
            {
                int i;
                var doc = IdTipoDocRef == 1
                    ? "FAC"
                   : IdTipoDocRef == 3 ? "BOL" : IdTipoDocRef == 7 ? "NCR" : IdTipoDocRef == 8 ? "NDB" : IdTipoDocRef == 12 ? "TCK" : "??";
                var serie = int.TryParse(SerieRef, out i) ? i.ToString("0000") : "??";
                var correlativo = int.TryParse(CorrelativoRef, out i) ? i.ToString("00000000") : "??";
                return string.Format("{0} {1}-{2}", doc, serie, correlativo);
            }
        }

        private static bool ValidarRuc(string nroDocumento)
        {
            nroDocumento = nroDocumento.Trim();

            if (string.IsNullOrEmpty(nroDocumento) || nroDocumento.Length != 11) return false;
            int[] factores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var productos = new int[10];
            var longitudDocumento = nroDocumento.Length;
            var nroIdentificador = int.Parse(nroDocumento.Substring(longitudDocumento - 1, 1));

            for (var i = 0; i < 10; i++)
            {
                var valor = int.Parse(nroDocumento.Substring(i, 1));
                productos[i] = valor * factores[i];
            }

            var sumaProductos = productos.Sum();
            var resultado = 11 - (sumaProductos % 11);

            switch (resultado)
            {
                case 10:
                    resultado = 0;
                    break;

                case 11:
                    resultado = 1;
                    break;
            }

            if (resultado <= 11) return resultado == nroIdentificador;
            var result = resultado.ToString();
            resultado = int.Parse(result.Substring(result.Length - 1, 1));

            return resultado == nroIdentificador;
        }
        
        public string IdVenta { get; set; }
        public string IdCliente { get; set; }
        public string NroRegistro { get; set; }
        public string Cuenta12 { get; set; }
    }
}
