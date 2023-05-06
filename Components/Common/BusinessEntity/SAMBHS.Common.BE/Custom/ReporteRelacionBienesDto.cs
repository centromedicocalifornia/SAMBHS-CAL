using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public  class ReporteRelacionBienesDto
    {
        public int AnioCompra { get; set; }
        public int AnioBaja { get; set; }
        public int i_Baja { get; set; }
        public string CodigoActivoFijo { get; set; }
        public string DescripcionActivoFijo { get; set; }
        public string CodTipoActivo { get; set; }
        public string TipoActivo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string Placa { get; set; }
        public string Color { get; set; }
        public string Estado { get; set; }
        public string CodUbicacion { get; set; }
        public string Ubicacion { get; set; }
        public string CodResponsable { get; set; }
        public string Responsable { get; set; }
        public string GrupoLlave1 { get; set; }
        public string GrupoLlave2 { get; set; }
        public string CodCentroCosto { get; set; }
        public string CentroCosto { get; set; }
        public string RucProveedor { get; set; }
        public DateTime?  FechaCompra { get; set; }
        public DateTime? FechaUso { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string Factura { get; set; }
        public string Adquisicion { get; set; }
        public int TotalMeses { get; set; }
        public int TotalMesesPeriodoAnterior { get; set; }
        public decimal ValorCompra { get; set; }
        public decimal ValorHistorico { get; set; }
        public string Cuenta33 { get; set; }
        public string PorcentajeDepreciacion { get; set; }
        public string MesesPorcentajeDepreciacion { get; set; }
        public decimal Ajuste { get; set; }
        public string MesesDepreciar { get; set; }
        public decimal DepreciacionCierreAnterior { get; set; }
        public decimal DepreciacionEjercicio { get; set; }
        public string sFechaAdquisicion { get; set; }
        public DateTime dFechaAdquisicion { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
        public int i_SeDepreciara { get; set; }
        public decimal AdquisionesAdicionales { get; set; }
        public decimal Mejoras { get; set; }
        public decimal Bajas { get; set; }
        public byte[] Foto { get; set; }
        public string v_IdActivoFijo { get; set; }
    }
}
