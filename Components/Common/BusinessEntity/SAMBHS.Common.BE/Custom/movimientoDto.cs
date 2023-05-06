using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class movimientoDto
    {
        #region Movimiento
        public string v_NombreCliente { get; set; }
        public string KeyRegistro { get; set; }
        public int i_IdZona { get; set; }
        #endregion

        #region Bandeja
        public string v_MesCorrelativo { get; set; }
        public string v_AlmacenOrigen { get; set; }
        public string v_NombreProveedor { get; set; }
        public string v_DescripcionMotivo { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string RegistroOrigen { get; set; }
        public string v_AlmacenDestino { get; set; }
        public string v_MesGuardado { get; set; }
        public string v_CorrelativoGuardado { get; set; }
        public string v_AnioGuardado { get; set; }
        public string Moneda { get; set; }
        public decimal Total { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string v_NroPedido { get; set; }
        public string NroDocumentoCabecera { get; set; }


        public string Producto { get; set; }
        public decimal? PrecioBaseU { get; set; }
        public decimal? PrecioEgresoU { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? TotalMov { get; set; }
        public string Glosa { get; set; }
        public string Unidad { get; set; }
        public string Marca { get; set; }

        #endregion

        //#region Kardex


        //#endregion
    }
    //public class movimientoshortD
}
