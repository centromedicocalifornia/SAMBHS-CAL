using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class importacionDto
    {

        public string RucProveedor1 { get; set; }
        public string NombreProveedor1 { get; set; }
        public string RucProveedor2 { get; set; }
        public string NombreProveedor2 { get; set; }
        public string RucProveedor3 { get; set; }
        public string NombreProveedor3 { get; set; }
        public string RucProveedor4 { get; set; }
        public string NombreProveedor4 { get; set; }
        public string NroRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string Documento { get; set; }
        public string TipoVia { get; set; }
        public string Almacen { get; set; }
        public string NroDocumentoReferencia { get; set; }
        public string Moneda { get; set; }
        public string KeyRegistro { get; set; }
        public IEnumerable<string> ListaProveedoresFob { get; set; }
        public string ProveedoresVistaRapida { get; set; }

        public partial class CompiladoResultImportacion
        {
            public string NombreProveedor { get; set; }
            public string CodigoProveedor { get; set; }
            public string CodigoProducto { get; set; }
            public string NombreProducto { get; set; }
            public int? EsServicio { get; set; }
            public decimal? d_Empaque { get; set; }
            public string UmEmpaque { get; set; }
            public int? i_IdUnidadMedidaProducto { get; set; }
        }




    }
    public partial class importaciondetallefobDto
    {

        public string NombreProveedorFOB { get; set; }
        public string v_CodCliente { get; set; }
        public string v_RazonSocial { get; set; }
        public int i_IdTipoDocumentoReferencia { get; set; }
        public string v_SerieDocumentoReferencia { get; set; }
        public string v_CorrelativoDocumentoReferencia { get; set; }

    }
}
