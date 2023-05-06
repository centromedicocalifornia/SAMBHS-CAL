using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial  class recibohonorarioDto
    {
        public string NombreProveedor { get; set; }
        public string CodigoProveedor { get; set; }
        public string RUCProveedor { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
         public string KeyRegistro { get; set; }
       

        #region Bandeja

        public string v_IdentificacionProveedor { get; set; }
        public string  v_NumeroRegistro { get;set; }
        public string v_NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoDocumentoRef { get; set; }
        public string Moneda { get; set; }
        #endregion
    }

    public partial class BindgListReciboHonorarioDetalleDto
    {
        public string v_IdReciboHonorarioDetalle { get; set; }
        public string v_IdReciboHonorario { get; set; }
        public string v_NroCuenta { get; set; }
        public string  i_CCosto { get; set; }
        public decimal d_ImporteSoles { get; set; }
        public decimal d_ImporteDolares { get; set; }
        public int? i_Eliminado { get; set; }
        public int? i_InsertaIdUsuario { get; set; }
        public DateTime? t_InsertaFecha { get; set; }
        public int? i_ActualizaIdUsuario { get; set; }
        public DateTime? t_ActualizaFecha { get; set; }
        public int i_ValidarCentroCosto { get; set; }

    }
    public partial class recibohonorariodetalleDto
    {

        public string KeyRegistro { get; set; }
        public int i_ValidarCentroCosto { get; set; }
    }
}
