using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteLibroActivoDto : ICloneable
    {

       public string NombreMes { get; set; }
       public decimal?  ValorAdquisicionHistorico { get; set; }
       public decimal? ValorAdquisicionHistoricoFinal { get; set; }
       public int? MesesDepreciados { get; set; }
       public decimal?  d_ImporteMensualDepreciacion{get;set ;}
       public decimal? d_Importe1SoloMes { get; set; }
       public decimal? d_ImportePeriodoDepreciacion { get; set; }
       public  decimal? d_AcumuladoHistorico{get ;set;}
       public decimal? d_AcumuladoTotal { get; set; }
       public decimal? d_ValorNetoActual{get;set ;}
       public decimal? d_Comparacion{get;set ;}
       public decimal? AcumuladoComparacion { get; set; }
       public decimal? d_AjusteDepreciacion { get; set; }
       public decimal? d_AjusteInicial { get; set; }
       public string DescripcionActivo { get; set; }
       public string SoloDescripionActivo { get; set; }
       public string v_IdActivoFijo { get; set; }
       public string Cuenta33 { get; set; }
       public string Cuenta39 { get; set; }
       public string CuentaGasto { get; set; }
       public string CentroCosto { get; set; }
       public string AnioCompra { get; set; }
       public string MesCompra { get; set; }
       public string AnioUso { get; set; }
       public string MesesDepreciar { get; set; }
       public int i_Baja { get; set; }
       public string  AnioBaja { get; set; }
       public DateTime FechaUso { get; set; }
       public string  sFechaUso { get; set; }
       public string CodigoActivoFijo { get; set; }
       public DateTime FechaCompra { get; set; }
       public DateTime? FechaBaja { get; set; }
       public int i_IdCentroCosto { get; set; }
       public string TipoActivo { get; set; }
       public string v_Marca { get; set; }
       public string v_Modelo { get; set; }
       public string v_Serie { get; set; }
       public string v_Placa { get; set; }
       public string v_NumeroFactura { get; set; }
       public string TipoAdquisicion { get; set; }
       public decimal DepreciacionEjercicio { get; set; }
       public int i_Depreciara { get; set; }
       public decimal? AcumuladoAnterior { get; set; }
       public decimal? HistoricoAnterior { get; set; }
       public decimal? ValorNetoAnterior { get; set; }
       public decimal? AjusteAnterior { get; set; }
       public string PorcentajeDepreciacion { get; set; }
       public byte[] Foto { get; set; }
       public string Responsable { get; set; }
       public string  Ubicacion { get; set; }
       public string MesesPorcentajeDepreciacion { get; set; }
       public object Clone()
       {
           return MemberwiseClone();
       }
    }


   public class ReporteValorActualActivosDto
   {

       public string CodigoDescripcionActivo { get; set; }
       public DateTime? FechaCompra { get; set; }
       public decimal ValorHistorico { get; set; }
       public decimal ValorActualizado { get; set; }
       public int MesesDepreciar { get; set; }
       public decimal? ValorCompra { get; set; }
       public decimal? AcumuladoAnterior { get; set; }
       public int? MesesDepreciados { get; set; }
       public decimal?  MensualHistorica { get; set; }
       public decimal? AcumuladoHistoricoAnio { get; set; }
       public decimal? AcumuladoTotal { get; set; }
       public decimal? AjusteDepreciacion { get; set; }
       public decimal? ValorNetoActual { get; set; }
       public string Cuenta { get; set; }
       public string CentroCosto { get; set; }
       public DateTime? FechaUso { get;set; }
       public string sFechaUso { get; set; }
       public DateTime? FechaBaja { get; set; }
       public decimal? Comparacion { get; set; }
       public string CodigoActivoFijo { get; set; }
       public string DescripcionActivoFijo { get; set; }

   }
   public class ReporteResumenActivoFijoDto
   {
       public string CuentaDescripcion { get; set; }
       public decimal SaldoAl { get; set; }
       public decimal Adiciones { get; set; }
       public decimal Retiros { get; set; }
       public decimal Reclasificaciones { get; set; }
       public decimal SaldoAlMesAnio { get; set; }
       public decimal Depreciacion { get; set; }
       public string KeyCuenta { get; set; }
       public string CodigoActivoFijo { get; set; }
   
   }

   public class ActivosDepreaciadosDto
   {
       public string cuenta { get; set; }
       public DateTime  fechaCompra { get; set; }
       public string fechaBaja { get; set; }
       public decimal  CantidadDepreciada { get; set; }
    
   }
   public class ReporteResumenDepreciacionDto
   {
       public string CuentaDescripcion { get; set; }
       public decimal? SaldoAl { get; set; }
       public decimal? DepreciacionActual { get; set; }
       public decimal? Adiciones { get; set; }
       public decimal? Retiros { get; set; }
       public decimal? Total { get; set; }
       public decimal? SaldoTotal { get; set; }
       public string CodigoActivoFijo { get; set; }
       public string KeyCuenta { get; set; }
       public string v_IdActivoFijo { get; set; }
       public int i_IdCentroCosto { get; set; }
       public decimal? DepreciacionMensual { get; set; }
       public decimal? Ajuste { get; set; }
   
   }
   public class DiarioDepreciacionHistorica
   {
      
       public decimal? CantidadMensual { get; set; }
       public string CuentaGasto { get; set; }
       public string SoloCuentaGasto { get; set; }
       public string SoloCuenta39 { get; set; }
       public string Cuenta39 { get; set; }
       public int i_IdCentroCosto { get; set; }
       public string CentroCosto { get; set; }
       public string CodigoActivoFijo { get; set; }
       public string KeyCuenta3 { get; set; }
   }

   public class DiarioAjusteDepreciacion
   {
     
       public decimal? CantidadAjuste { get; set; }
       public string CuentaGasto { get; set; }
       public string SoloCuentaGasto { get; set; }
       public string SoloCuenta39 { get; set; }
       public string Cuenta39 { get; set; }
       public int i_IdCentroCosto { get; set; }
       public string CentroCosto { get; set; }
       public string CodigoActivoFijo { get; set; }
       public string KeyCuenta3 { get; set; }

   }

   public class IdDiarioAsientosInventarios
   {
       public int Tipo { get; set; }
       public string Id { get; set; }
   }
   public class NroDiario
   {

       public string NumeroDiario { get; set; }
       public int TipoAsiento { get; set; }
   }
 
}
