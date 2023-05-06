using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReportePlanilllas
    {
    }

    public class Remuneraciones
    {

        public string Mes { get;set;}
        public string Cargo { get; set; }
        public int IdPlanilla { get; set; }
        public string NumeroPlanilla { get; set; }
        public int IDiasLaboradas { get; set; }
        public decimal DHorasLaboradas { get; set; }
        public int SDiasLaboradas { get; set; }
        public decimal SHorasLaboradas { get; set; }
    }
    public class ReportePlanillaConceptos
    {

        public string v_Codigo { get; set; }
        public int i_IdTipoConcepto { get; set; }
        public int i_IdTipoPlanilla { get; set; }
        public string v_Nombre { get; set; }
        public string TipoPlanilla { get; set; }

    }
    public class ReporteTrabajadores
    {

        public string v_Codigo { get; set; }
        public string NombreTrabajador { get; set; }
        public string v_NroDocIdentificacion { get; set; }
        public DateTime t_FechaAlta { get; set; }
        public DateTime? t_FechaCese { get; set; }
        public DateTime t_fechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public int i_Activo { get; set; }
        public string v_Direccion { get; set; }
        public string Activo { get; set; }


    }

    public class ReporteTrabjadoresAdicionales
    {

        public string v_Codigo { get; set; }
        public string NombreTrabajador { get; set; }
        public string v_NroDocIdentificacion { get; set; }
        public string ruc { get; set; }
        public string afp { get; set; }
        public string Ccosto { get; set; }
        public string NroContrato { get; set; }
        public decimal Ingresos { get; set; }
        public int? i_IdTipoContrato { get; set; }
        public int? i_idTipoPlanilla { get; set; }

    }

    public class ReporteTrabjadoresAdicionalesAFP
    {

        public string Trabajador { get; set; }
        public DateTime? FechaAfiliacion { get; set; }
        public string NroCuenta { get; set; }
        public string NroCussp { get; set; }
        public int? i_IdTipoContrato { get; set; }
        public int? i_idTipoPlanilla { get; set; }
        public string afp { get; set; }
        public string regimenactual { get; set; }



    }
    public class ReporteTrabjadoresAdicionalesCcosto
    {

        public string Trabajador { get; set; }
        public string Cargo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Ccosto { get; set; }
        public int? i_IdTipoContrato { get; set; }
        public int? i_idTipoPlanilla { get; set; }
        public string centrocostoActual { get; set; }

    }

    public class ReportePlanillaOficial
    {
        public int NumeroBoleta { get; set; }
        public decimal RemuneracionBasica { get; set; }
        public string Trabajador { get; set; }
        public string Cargo { get; set; }
        public string NroEssalud { get; set; }
        public string NroCussp { get; set; }
        public decimal HorasTrabajadas { get; set; }
        public int DiasTrabajados { get; set; }
        public string v_IdTrabajador { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaCese { get; set; }
        public string DiasTrabajadosS { get; set; }
        public string HorasTrabajadasS { get; set; }
        public int i_NumeroPlanilla { get; set; }
        public decimal AsignacionFamiliar { get; set; }
        public decimal Gratificacion { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal PagoHorasExtras { get; set; }
        public string Ccosto { get; set; }
        public string CodigoTrabajador { get; set; }
        public string TipoDocumentoTrabajador { get; set; }
        public string NroDocumentoTrabajador { get; set; }
        public string Banco { get; set; }
        public string CuentaBancaria { get; set; }
        public string CodBanco { get; set; }
        public string RegimenPensario { get; set; }
        public decimal HorasExtras { get; set; }
        public string CodigoConcepto { get; set; }
        public decimal ImporteConcepto { get; set; }
        public string RegimenLaboral { get; set; }
        public string TipoContrato { get; set; }
       
        //public decimal TotalIngresos { get; set; }
        //public decimal TotalDescuentos { get; set; }
        //public decimal TotalAportaciones { get; set; }

    }
    public class ReportePlanillaAFP
    {
        public string Trabajador { get; set; }
        public string Afp { get; set; }
        public string NroCussp { get; set; }
        public string v_IdTrabajador { get; set; }
        public string Dni { get; set; }
        public decimal Remuneracion { get; set; }
        public int i_NumeroPlanilla { get; set; }
        public decimal AporteObligatorio { get; set; }
        public decimal PrimaSeguro { get; set; }
        public decimal ComisionFija { get; set; }
        public decimal ComisionRa { get; set; }
        public int TipoRegimen { get; set; }
        public decimal Ipss { get; set; }
        public decimal SubTotal { get; set; }
        public string TotalS { get; set; }
        public int i_IdCentroCosto { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
        public string Ccosto { get; set; }
        public int TipoPlanilla { get; set; }
    }
    public class ReportePlanillaGenAFP
    {
        public string v_IdTrabajador { get; set; }
        public string NroCussp { get; set; }
        public int TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaCese { get; set; }
    }

    public class ReportePlanillaEssalud
    {
        public string Trabajador { get; set; }
        public string NroEssalud { get; set; }
        public string v_IdTrabajador { get; set; }
        public decimal RemuneracionAsegurable { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
        public decimal EsSaludVida { get; set; }
        public decimal EsSalud { get; set; }
        public int TipoPlanilla { get; set; }
    }
    public class ReporteResumenPlanillas
    {
        public decimal Importe { get; set; }
        public string  TipoConcepto{get;set;}
        public string Concepto { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
        public string Total { get; set; }
        public int IdTipoConcepto { get; set; }
        public decimal DTotal { get; set; }
        public int TipoPlanilla { get; set; }
        public string CodigoConcepto { get; set; }
        public string CodifoTrabajador { get; set; }
    
    
    }
    public class ReporteResumenVsAnterior
    {

        public decimal Importe { get; set; }
        public string TipoConcepto { get; set; }
        public string Concepto { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
        public string Total { get; set; }
        public int IdTipoConcepto { get; set; }
        public decimal DTotal { get; set; }
        public int TipoPlanilla { get; set; }
        public string PeriodoAnterior { get; set; }
        public string MesAnterior { get; set; }
        public decimal? ImporteMesAnterior { get; set; }
        public decimal DTotalMesAnterior { get; set; }
    
    }
    public class ReporteResumenAnual
    {

        public string TipoConcepto { get; set; }
        public string Concepto { get; set; }
        public string Periodo { get; set; }
        public decimal Importe { get; set; }
        public int IdTipoConcepto { get; set; }
        public string Mes { get; set; }
        public decimal Mes1 { get; set; }
        public decimal Mes2 { get; set; }
        public decimal Mes3 { get; set; }
        public decimal Mes4 { get; set; }
        public decimal Mes5 { get; set; }
        public decimal Mes6 { get; set; }
        public decimal Mes7 { get; set; }
        public decimal Mes8 { get; set; }
        public decimal Mes9 { get; set; }
        public decimal Mes10 { get; set; }
        public decimal Mes11 { get; set; }
        public decimal Mes12 { get; set; }
        public string NombreMes1 { get; set; }
        public string NombreMes2 { get; set; }
        public string NombreMes3 { get; set; }
        public string NombreMes4 { get; set; }
        public string NombreMes5 { get; set; }
        public string NombreMes6 { get; set; }

        public string NombreMes7 { get; set; }
        public string NombreMes8 { get; set; }
        public string NombreMes9 { get; set; }
        public string NombreMes10 { get; set; }
        public string NombreMes11{ get; set; }
        public string NombreMes12{ get; set; }
        public string Total { get; set; }
        public string v_IdConcepto { get; set; }
        public string v_IdConceptoPC { get; set; } 
    
    }
    public class ConceptosResultantesPlanilla
    {

        public string CodigoConcepto { get; set; }
        public string NombreConcepto { get; set; }
        public decimal  TipoConcepto { get; set; }
    
    }
    public class ReportePlanillaNetoPagar
    {
        public string Trabajador { get; set; }
        public string CodigoTrabajador { get; set; }
        public string v_IdTrabajador { get; set; }

        public decimal NetoPagar { get; set; }
    }
    public class ReportePlanillaDetalleDescuentos {

        public int i_IdPlanillaNumeracion { get; set; }
        public string Trabajador { get; set; }
        public string v_IdTrabajador { get; set; }
        public string Descuento1 { get; set; }
        public string Descuento2 { get; set; }
        public string Descuento3 { get; set; }
        public string Descuento4 { get; set; }
        public string Descuento5 { get; set; }
        public string Descuento6 { get; set; }
        public string Descuento7 { get; set; }
        public string Descuento8 { get; set; }
        public string Descuento9 { get; set; }
        public string Descuento10 { get; set; }
        public string Descuento11{ get; set; }
        public string Descuento12 { get; set; }
        public decimal  DDescuento1 { get; set; }
        public decimal DDescuento2 { get; set; }
        public decimal DDescuento3 { get; set; }
        public decimal DDescuento4 { get; set; }
        public decimal DDescuento5 { get; set; }
        public decimal DDescuento6 { get; set; }
        public decimal DDescuento7 { get; set; }
        public decimal DDescuento8 { get; set; }
        public decimal DDescuento9 { get; set; }
        public decimal DDescuento10 { get; set; }
        public decimal DDescuento11 { get; set; }
        public decimal DDescuento12 { get; set; }
        public decimal Remuneracion { get; set; }
    
    }

    public class PlameJornadaLaboral
    {

        public string TipoDocTrabajador { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal  DHorasTrabajadas { get; set; }
        public string MinutosTrabajados { get; set; }
        public string HorasExtras { get; set; }
        public string MinutosExtras { get; set; }
        public string  SHorasTrabajadas { get; set; }
        public string v_IdPlanillaVariablesTrabajador { get; set; }
        public string TipoPlame { get; set; }
        public string v_IdTrabajador { get; set; }
        public string Trabajador { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
        public int NroTipoPlame { get; set; }
        
    }
    public class PlameIngDesAport :ICloneable 
    {
        public string Trabajador { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoDocTrabajador { get; set; }
        public string CodigoConcepto { get; set; }
        public int  IdConceptoSunat { get; set; }
        public decimal MontoDevengado { get; set; }
        public decimal MontoPagado { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
        public string TipoPlame { get; set; }
        public string v_IdTrabajador { get; set; }
        public int NroTipoPlame { get; set; }
        public string CodigoConceptoSistema { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
        
    
    }
    public class PlamePrestadorCuartaDetalles
    {

        public string TipoDocTrabajador { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoComprobanteEmitido { get; set; }
        public int iTipoComprobanteEmitido { get; set; }
        public string SerieComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public decimal MontoTotalServicio { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaPago { get; set; }
        public string SFechaPago { get; set; }
        public string IndicarRentaCuarta { get; set; }
        public string TipoPlame { get; set; }
        public int NroTipoPlame { get; set; }
        public string v_IdTrabajador { get; set; }
        public string Trabajador { get; set; }
        public string v_Periodo { get; set; }
        public string v_Mes { get; set; }
    }
    public class PlameDiasSubsidiados {

        public string TipoDocTrabajador { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoSuspensionLaboral { get; set; }
        public int DiasSuspension { get; set; }
        public string v_IdTrabajador { get; set; }
        public string TipoPlame { get; set; }
        public int NroTipoPlame { get; set; }
        public string Trabajador { get; set; }
        public int i_IdPlanillaNumeracion { get; set; }
    
    
    
    }
    public class PlamePrestadorCuarta
    {
        public string NumeroDocumento { get; set; }
        public string TipoDocTrabajador { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public string Domiciliado { get; set; }
        public string DobleTributacion { get; set; }
        public string v_IdTrabajador { get; set; }
        public string TipoPlame { get; set; }
        public int NroTipoPlame { get; set; }
        public string Trabajador { get; set; }
        public string v_Periodo { get; set; }
        public string v_Mes { get; set; }
    }
    public class ListaPlame
    {
        public string Trabajador { get; set; }
        public string TipoPlame { get; set; }
        public int NroTipoPlame { get; set; }
        
    
    }
}
