using System.Collections;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ImportacionRegistroVentaDetalleDto
    {
        private readonly int[] _documentosSoportados = { 1, 3, 7, 8, 12};
        /// <summary>
        /// Id del tipo de documento de venta.
        /// </summary>
        public int IdTipoDoc { get; set; }
        /// <summary>
        /// Serie del documento de venta.
        /// </summary>
        public string Serie { get; set; }
        /// <summary>
        /// Correlativo del documento de venta.
        /// </summary>
        public string Correlativo { get; set; }
        /// <summary>
        /// Codigo del artículo, si no existe, se creará uno con éste codigo.
        /// </summary>
        public string CodArticulo { get; set; }
        /// <summary>
        /// Id real del producto en la bd, se asignará el valor en el proceso de importación.
        /// </summary>
        public string IdProducto { get; set; }
        /// <summary>
        /// Descripción del producto, si el artículo no existe se creará con ésta descripción.
        /// </summary>
        public string DescripionArticulo { get; set; }
        /// <summary>
        /// Cantidad que se esta vendiendo de éste artículo.
        /// </summary>
        public decimal Cantidad { get; set; }
        /// <summary>
        /// Monto total por artículo en moneda.
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// Cuenta contable para éste artículo.
        /// </summary>
        public string Cuenta { get; set; }

        public decimal Precio {
            get
            {
                var cant = Cantidad > 0 ? Cantidad : 1;
                return Total / cant;
            }
        }

        public decimal Valor {
            get { return Total / 1.18M; }
        }

        public decimal Igv {
            get { return Total - Valor; }
        }

       

        public bool Valido {
            get { return string.IsNullOrWhiteSpace(Errores); }
        }

        public string Errores {
            get
            {
                int i;
                var errores = new StringBuilder();
               
                if (!((IList)_documentosSoportados).Contains(IdTipoDoc))
                    errores.AppendLine(string.Format("T. Doc. -> Documento no sportado ({0})", IdTipoDoc));

                if (!int.TryParse(Correlativo, out i) || i <= 0)
                    errores.AppendLine(string.Format("Correlativo -> Número de Correlativo incorrecto ({0})", Correlativo));

                if (Total <= 0)
                    errores.AppendLine("Total -> El total no contiene un valor válido.");

                if (string.IsNullOrWhiteSpace(CodArticulo.Trim()))
                    errores.AppendLine("Cod. Artículo -> El código del artículo no puede estar en blanco.");

                if (string.IsNullOrWhiteSpace(DescripionArticulo))
                    errores.AppendLine("Descripción. -> El código del artículo no puede estar en blanco.");

                if (string.IsNullOrWhiteSpace(Cuenta))
                    errores.AppendLine("Cuenta. -> La cuenta no puede estar en blanco.");

                if(Cantidad <= 0)
                    errores.AppendLine("Cantidad -> Cantidad no puede ser 0");

                if(!int.TryParse(Cuenta, out i))
                    errores.AppendLine("Cuenta -> Cuenta contable no válida.");

                return errores.ToString();
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

        public string EstadoRegistro { get { return string.IsNullOrWhiteSpace(Errores) ? "CORRECTO" : "CON ERRORES"; } }
    }
}
