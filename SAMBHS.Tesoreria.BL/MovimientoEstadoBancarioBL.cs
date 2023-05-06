using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using System.ComponentModel;



namespace SAMBHS.Tesoreria.BL
{
    public partial class MovimientoEstadoBancarioBL
    {
        #region Formulario
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public List<movimientoestadobancarioDto> ExtraerAsientosTesoreria(ref OperationResult objOperationResult,  string pstrAño, string pstrMes, string pstrNumeroCuenta)
        {

            try
            {
                DateTime fechaInicio = DateTime.Parse("01/" + pstrMes + "/" + pstrAño);
                DateTime fechaFin = DateTime.Parse(DateTime.DaysInMonth(int.Parse(pstrAño), int.Parse(pstrMes)).ToString() + "/" + pstrMes + "/" + pstrAño+" 23:59");
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                string Orden = "NumeroCuenta,Fecha";



                var MonedaCuenta = (from n in dbContext.asientocontable
                                    where n.v_NroCuenta == pstrNumeroCuenta && n.i_Eliminado ==0 && n.v_Periodo == periodo  
                                    select new { n.i_IdMoneda }).FirstOrDefault();



                var TesoreriaDetalle = (from n in dbContext.tesoreria

                                        join a in dbContext.tesoreriadetalle on new { IdTesoreria = n.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = a.v_IdTesoreria, eliminado = a.i_Eliminado.Value } into a_join

                                        from a in a_join.DefaultIfEmpty()

                                        join b in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { TipoDoc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                        from b in b_join.DefaultIfEmpty()


                                        where n.i_Eliminado == 0 && n.t_FechaRegistro  >= fechaInicio && n.t_FechaRegistro  <= fechaFin && a.v_NroCuenta == pstrNumeroCuenta && n.i_IdEstado == 1

                                        select new movimientoestadobancarioDto
                                        {
                                            t_Fecha = n.t_FechaRegistro ,
                                            d_Cargo = a.v_Naturaleza == "H" && MonedaCuenta.i_IdMoneda.Value == 1 ? a.d_Importe : a.v_Naturaleza == "H" && MonedaCuenta.i_IdMoneda.Value == 2 ? a.d_Importe : 0,
                                            d_Abono = a.v_Naturaleza == "D" && MonedaCuenta.i_IdMoneda.Value == 1 ? a.d_Importe : a.v_Naturaleza == "D" && MonedaCuenta.i_IdMoneda.Value == 2 ? a.d_Importe : 0,
                                            //d_Cargo = a.v_Naturaleza == "H" ?  MonedaCuenta.i_IdMoneda.Value == n.i_IdMoneda ? a.d_Importe : a.d_Cambio :0 , //a.d_Importe : a.v_Naturaleza == "H" && MonedaCuenta.i_IdMoneda.Value == 2 ? a.d_Importe : 0,
                                            //d_Abono = a.v_Naturaleza == "D" ?  MonedaCuenta.i_IdMoneda.Value == n.i_IdMoneda ? a.d_Importe :a.d_Cambio   : 0,
                                            i_IdTipoDocumento = a.i_IdTipoDocumento,
                                            v_NumeroDocumento = a.v_NroDocumento,
                                            v_Concepto = a.v_Analisis == null || a.v_Analisis == string.Empty ? n.v_Glosa : a.v_Analisis,
                                            v_CodAsiento = b.v_Siglas,
                                            v_NumeroAsiento = n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim(),
                                            i_Mes = 0,
                                            v_Anio = "",
                                            v_Mes = "",
                                            v_NroCuenta = a.v_NroCuenta,
                                            v_IdReferencia = a.v_IdTesoreriaDetalle,
                                            i_IdTipoDocRef = a.i_IdTipoDocumentoRef.Value == null ? 0 : a.i_IdTipoDocumentoRef.Value,
                                            v_NroDocumentoRef = a.v_NroDocumentoRef,
                                        }).ToList().Select (a=> new movimientoestadobancarioDto 
                                            {
                                            
                                            t_Fecha =a.t_Fecha ,
                                            d_Cargo =a.d_Cargo ,
                                            d_Abono =a.d_Abono ,
                                            i_IdTipoDocumento = a.i_IdTipoDocumento,
                                            v_NumeroDocumento = a.v_NumeroDocumento ,
                                            v_Concepto = a.v_Concepto ,
                                            v_CodAsiento = a.v_CodAsiento ,
                                            v_NumeroAsiento = a.v_NumeroAsiento ,
                                            i_Mes = a.i_Mes ,
                                            v_Anio = pstrAño,
                                            v_Mes = pstrMes,
                                            v_NroCuenta = a.v_NroCuenta,
                                            v_IdReferencia = a.v_IdReferencia,
                                            i_IdTipoDocRef = a.i_IdTipoDocRef,
                                            v_NroDocumentoRef = a.v_NroDocumentoRef,

                                            
                                            }).ToList ();

                var MovimientosEB = (from n in dbContext.movimientoestadobancario  //Verifico si de Enero a MesAnteriordel Extracto hay Movimientos Estado Bancarios
                                     where n.i_Eliminado.Value == 0 && n.v_Anio == pstrAño
                                     select n).ToList();

                var MovimientosEBFinal = (from n in MovimientosEB
                                          where int.Parse(n.v_Mes.ToString()) <= int.Parse(pstrMes.ToString())
                                          select n).ToList();
                // En caso no haya Extraigo el del periodo Anterior , los que i_mes=0

                if (MovimientosEBFinal.Count() == 0)
                {
                    string MesPendienteAnioAnterior = ((int)Mes.Diciembre).ToString("00");
                    string AnioAnterior = (int.Parse(pstrAño) - 1).ToString();

                    var PendientesAñoAnterior = (from n in dbContext.pendientesconciliacion
                                                 where n.i_Eliminado ==0 &&  n.v_Anio == AnioAnterior && n.v_Mes == MesPendienteAnioAnterior && n.v_NroCuenta == pstrNumeroCuenta

                                                 select new movimientoestadobancarioDto
                                                 {

                                                     t_Fecha = n.t_Fecha.Value,
                                                     d_Cargo = n.v_Naturaleza == "H" ? n.d_Importe : 0,
                                                     d_Abono = n.v_Naturaleza == "D" ? n.d_Importe : 0,
                                                     i_IdTipoDocumento = n.i_IdTipoDoc,
                                                     v_NumeroDocumento = n.v_NumeroDoc,
                                                     v_Concepto = n.v_Glosa,
                                                     v_CodAsiento ="",
                                                     v_NumeroAsiento = "",
                                                     i_Mes = 0,
                                                     v_NroCuenta = n.v_NroCuenta,
                                                     v_IdReferencia = n.v_IdPendientesConciliacion,
                                                     i_IdTipoDocRef = 0,
                                                     v_NroDocumentoRef = "",

                                                 }).ToList();

                    List<string> ListaIdPendientesAnterior = PendientesAñoAnterior.Select(x => x.v_IdReferencia).ToList();
                    // Verifico si los pendientes del Anio Anterior ya han sido tomandos en los Mov. Estado Bancario siguientes

                    string MesPosterior = (int.Parse(pstrMes) - 1).ToString("00");
                    var MovimientosEBMesPosterior = (from n in dbContext.movimientoestadobancario

                                                     where n.i_Eliminado == 0 && n.v_Anio == pstrAño && n.v_NroCuenta == pstrNumeroCuenta
                                                     select n).ToList();


                    var MovimientosEBMesPosteriorFinal = (from n in MovimientosEBMesPosterior

                                                          where int.Parse(n.v_Mes) > int.Parse(MesPosterior.ToString())

                                                          select n).ToList();

                    List<string> ListaIdMovEB = MovimientosEBMesPosteriorFinal.Select(x => x.v_IdReferencia).ToList();
                    bool Generado = false;

                    foreach (var item in ListaIdPendientesAnterior)
                    {
                        if (ListaIdMovEB.Contains(item))
                        {
                            Generado = true;
                        }
                    }
                    if (!Generado)
                    {
                        var queryFinal = PendientesAñoAnterior.Concat(TesoreriaDetalle).ToList();
                        objOperationResult.Success = 1;
                        return queryFinal.OrderBy(l => l.v_NroCuenta).ThenBy(l => l.t_Fecha).ToList ();
                    }

                    else
                    {
                        var queryFinal = TesoreriaDetalle.ToList();
                        objOperationResult.Success = 1;
                        return queryFinal.OrderBy(l => l.v_NroCuenta).ThenBy(l => l.t_Fecha).ToList();
                    }
                }

                else
                {

                    string MesAnterior =  int.Parse (pstrMes)==(int)Mes.Enero ?  ((int)Mes.Diciembre).ToString ("00") :  (int.Parse(pstrMes) - 1).ToString("00");
                    pstrAño =int.Parse (pstrMes)==(int)Mes.Enero ?   (int.Parse (pstrAño)-1).ToString () : pstrAño ;

                    List<movimientoestadobancarioDto> PendientesMesAnterior = new List<movimientoestadobancarioDto>();
                    if (int.Parse (pstrMes) == (int)Mes.Enero)
                    {
                        PendientesMesAnterior = (from n in dbContext.pendientesconciliacion
                                                     where n.i_Eliminado == 0 && n.v_Anio == pstrAño && n.v_Mes == MesAnterior && n.v_NroCuenta == pstrNumeroCuenta 

                                                     select new movimientoestadobancarioDto
                                                     {

                                                         t_Fecha = n.t_Fecha.Value,
                                                         d_Cargo = n.v_Naturaleza == "H" ? n.d_Importe : 0,
                                                         d_Abono = n.v_Naturaleza == "D" ? n.d_Importe : 0,
                                                         i_IdTipoDocumento = n.i_IdTipoDoc,
                                                         v_NumeroDocumento = n.v_NumeroDoc,
                                                         v_Concepto = n.v_Glosa,
                                                         v_CodAsiento = "",
                                                         v_NumeroAsiento = "",
                                                         i_Mes = 0,
                                                         v_NroCuenta = n.v_NroCuenta,
                                                         v_IdReferencia = n.v_IdPendientesConciliacion,
                                                         i_IdTipoDocRef = 0,
                                                         v_NroDocumentoRef = "",

                                                     }).ToList();

                    }

                    else
                    {
                        PendientesMesAnterior = (from n in dbContext.movimientoestadobancario

                                                     where n.i_Eliminado == 0 && n.v_Mes == MesAnterior && n.v_Anio == pstrAño && n.i_Mes == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.i_Eliminado == 0

                                                     select new movimientoestadobancarioDto
                                                     {

                                                         t_Fecha = n.t_Fecha,
                                                         d_Cargo = n.d_Cargo,
                                                         d_Abono = n.d_Abono,
                                                         i_IdTipoDocumento = n.i_IdTipoDocumento,
                                                         v_NumeroDocumento = n.v_NumeroDocumento,
                                                         v_Concepto = n.v_Concepto,
                                                         v_CodAsiento = n.v_CodAsiento,
                                                         v_NumeroAsiento = n.v_NumeroAsiento,
                                                         i_Mes = n.i_Mes,
                                                         v_Mes = n.v_Mes,
                                                         v_Anio = n.v_Anio,
                                                         v_NroCuenta = n.v_NroCuenta,
                                                         v_IdReferencia = n.v_IdReferencia,
                                                         i_IdTipoDocRef = n.i_IdTipoDocRef == null ? 0 : n.i_IdTipoDocRef.Value,
                                                         v_NroDocumentoRef = n.v_NroDocumentoRef == null ? string.Empty : n.v_NroDocumentoRef,
                                                     }).ToList();

                    }


                    
                    var queryFinal = PendientesMesAnterior.Concat(TesoreriaDetalle).ToList();
                    objOperationResult.Success = 1;
                    return queryFinal.OrderBy(l => l.v_NroCuenta).ThenBy(l => l.t_Fecha).ToList();

                }
            }
            catch (Exception e)
            {
                objOperationResult.Success = 0;
                return null;
                

            }


        }
     
        public BindingList<movimientoestadobancarioDto> ObtenerListaEstadosBancarios(string pstrAño, string pstrMes, string pstrNumeroCuenta)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var MovimientoEstadoBancario = (from n in dbContext.movimientoestadobancario
                                                where n.i_Eliminado == 0 && n.v_Anio == pstrAño && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumeroCuenta

                                                select new movimientoestadobancarioDto
                                                {
                                                    t_Fecha = n.t_Fecha,
                                                    d_Cargo = n.d_Cargo,
                                                    d_Abono = n.d_Abono,
                                                    i_IdTipoDocumento = n.i_IdTipoDocumento,
                                                    v_NumeroDocumento = n.v_NumeroDocumento,
                                                    v_Concepto = n.v_Concepto,
                                                    v_CodAsiento = n.v_CodAsiento,
                                                    v_NumeroAsiento = n.v_NumeroAsiento,
                                                    i_Mes = n.i_Mes,
                                                    v_Mes = n.v_Mes,
                                                    v_Anio = n.v_Anio,
                                                    v_NroCuenta = n.v_NroCuenta,
                                                    v_IdMovimientoEstadoBancario = n.v_IdMovimientoEstadoBancario,
                                                    i_IdTipoDocRef = n.i_IdTipoDocRef,
                                                    v_NroDocumentoRef = n.v_NroDocumentoRef,

                                                }).ToList().OrderBy (l=>l.t_Fecha).ToList ();



                return new BindingList<movimientoestadobancarioDto>(MovimientoEstadoBancario);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void InsertarMovimientoEstadoBancario(ref OperationResult pobjOperationResult, List<string> ClientSession, List<movimientoestadobancarioDto> pTemp_Insertar)
        {
            string NroDoc = "";
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();

                    string newIdmovimientoestadoBancario = string.Empty;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);

                    foreach (movimientoestadobancarioDto movimientoestadobancarioDto in pTemp_Insertar)
                    {

                        movimientoestadobancario objEntityMovimientoEstadoBancario = movimientoestadobancarioAssembler.ToEntity(movimientoestadobancarioDto);
                        var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 66);
                        newIdmovimientoestadoBancario = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "YD");
                        objEntityMovimientoEstadoBancario.v_IdMovimientoEstadoBancario = newIdmovimientoestadoBancario;
                        objEntityMovimientoEstadoBancario.t_InsertaFecha = DateTime.Now;
                        NroDoc = objEntityMovimientoEstadoBancario.v_NumeroDocumento;
                        objEntityMovimientoEstadoBancario.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityMovimientoEstadoBancario.i_Eliminado = 0;
                        dbContext.AddTomovimientoestadobancario(objEntityMovimientoEstadoBancario);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "movimientoestadobancario", newIdmovimientoestadoBancario);
                        dbContext.SaveChanges();
                    }

                    pobjOperationResult.Success = 1; 
                }

            }
            catch (Exception ex)
            {
                
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoEstadoBancarioBL.InsertarMovimientoEstadoBancario()\nLinea:" +NroDoc+
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;

               
            }
        }

        public void EliminarMovimientoEstadoBancario(ref OperationResult objOperationResult, string pstrAnio, string pstrMes, string pstrNumeroCuenta)
        {
            try
            {
                objOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var EstadoBancarioEliminar = (from n in dbContext.movimientoestadobancario

                                                  where n.i_Eliminado == 0 && n.v_Anio == pstrAnio && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumeroCuenta
                                                  select n).ToList();

                    if (EstadoBancarioEliminar.Count() != 0)
                    {

                        foreach (var estadobancario in EstadoBancarioEliminar)
                        {
                            dbContext.movimientoestadobancario.DeleteObject(estadobancario);
                        }
                        dbContext.SaveChanges();
                    } 
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "MovimientoEstadoBancarioBL.EliminarMovimientoEstadoBancario()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = objOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;
            }
        }

        public void EliminarMovimientoEstadoBancarioXReferencia(ref OperationResult objOperationResult, tesoreriadetalleDto objTesoreriaD,List<string> ClientSession)
        {

            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var EstadoBancarioEliminar = (from n in dbContext.movimientoestadobancario

                                                  where n.v_IdReferencia == objTesoreriaD.v_IdTesoreriaDetalle && n.i_Eliminado ==0
                                                  select n).ToList();

                    if (EstadoBancarioEliminar.Count() != 0)
                    {

                        foreach (var estadobancario in EstadoBancarioEliminar)
                        {

                            estadobancario.i_Eliminado = 1;
                            estadobancario.i_ActualizaIdUsuario =Int32.Parse(ClientSession[2]);
                            estadobancario.t_ActualizaFecha =DateTime.Now ;
                            dbContext.movimientoestadobancario.ApplyCurrentValues(estadobancario);
                           // dbContext.movimientoestadobancario.DeleteObject(estadobancario);

                           
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "MovimientoEstadoBancarioBL.EliminarMovimientoEstadoBancarioXReferencia()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = objOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;

            }

        }

        public bool EsImputable(string pstrNumeroCuenta)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var AsientoContable = (from n in dbContext.asientocontable
                                   where n.i_Eliminado == 0 && n.i_Imputable == 1 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Periodo ==periodo 
                                   select n).FirstOrDefault();
            if (AsientoContable != null)
            {

                return true;
            }
            else
            {
                return false;
            }


        }

        public void ActualizaMovimientoEstadoBancario(ref OperationResult pobjOperationResult, List<string> ClientSession, List<movimientoestadobancarioDto> pTemp_Editar)
        {
            try
            {
                SecuentialBL objSecuentialBL = new SecuentialBL();
                OperationResult objOperationResult = new OperationResult();
                pedidodetalleDto pobjDtoPedidoDetalle = new pedidodetalleDto();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                string newIdPedidoDetalle = string.Empty, newIdSeparacionProducto = string.Empty;
                var queryMovimientoEstadoBancario = (from n in dbContext.movimientoestadobancario
                                                     select n).ToList();
                foreach (movimientoestadobancarioDto movimientoestadobancarioDto in pTemp_Editar)
                {

                    movimientoestadobancario _objEntity = queryMovimientoEstadoBancario.Where(p => p.v_IdMovimientoEstadoBancario == movimientoestadobancarioDto.v_IdMovimientoEstadoBancario).FirstOrDefault();
                    _objEntity.i_Mes = movimientoestadobancarioDto.i_Mes.Value;
                    _objEntity.v_Mes = movimientoestadobancarioDto.v_Mes;
                    _objEntity.t_ActualizaFecha = DateTime.Now;
                    _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    dbContext.movimientoestadobancario.ApplyCurrentValues(_objEntity);

                }
                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoEstadoBancarioBL.ActualizaMovimientoEstadoBancario()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;

            }
        }
        public bool ExistenciaEstadoMovimientoEstadoBancario(string pstrAnio, string pstrMes, string pstrNumCuenta)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var MovEstadoBancario = (from n in dbContext.movimientoestadobancario

                                     where n.i_Eliminado == 0 && n.v_Anio == pstrAnio && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumCuenta

                                     select n).ToList();

            if (MovEstadoBancario.Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool PendientesMesAnterior(string pstrMes, string pstrAnio, string pstrNumeroCuenta)
        {

            try
            {
                //Si es Enero y hay tesoreriaDetalles en el Año Anterior y si no hay pendientes en el año anterior   debe retornar falso 


                List<tesoreriadetalleDto> TesoreriasAnteriores = new List<tesoreriadetalleDto>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    string MesAnterior = string.Empty, Anio;
                    if (int.Parse(pstrMes) != (int)Mes.Enero)
                    {

                        MesAnterior = (int.Parse(pstrMes) - 1).ToString("00");
                        Anio = pstrAnio;
                    }
                    else
                    {

                        MesAnterior = "12";
                        Anio = (int.Parse(pstrAnio) - 1).ToString();
                    }

                    var Tesorerias = (from a in dbContext.tesoreria


                                      join b in dbContext.tesoreriadetalle on new { IdTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                      from b in b_join.DefaultIfEmpty()

                                      where a.i_Eliminado == 0 && b.v_NroCuenta == pstrNumeroCuenta && a.i_IdEstado == 1
                                      orderby b.t_Fecha
                                      select b).ToList();



                    if (int.Parse(pstrMes) != (int)Mes.Enero)
                    {

                        TesoreriasAnteriores = (from n in Tesorerias

                                                where n.t_Fecha.Value.Date.Year.ToString() == Anio && int.Parse(n.t_Fecha.Value.Date.Month.ToString()) <= int.Parse(MesAnterior)

                                                select new tesoreriadetalleDto()).ToList();
                    }
                    else
                    {

                        TesoreriasAnteriores = (from n in Tesorerias

                                                where n.t_Fecha.Value.Date.Year.ToString() == Anio && n.t_Fecha.Value.Date.Month == int.Parse(MesAnterior)

                                                select new tesoreriadetalleDto()).ToList();
                    }

                    if (TesoreriasAnteriores.Count() > 0)
                    {
                        if (dbContext.movimientoestadobancario.Count(n => n.i_Eliminado == 0 && n.v_Mes == MesAnterior && n.v_Anio == pstrAnio) > 0)
                        {

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else { return true; }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string ObtenerNombreCuenta(string pstrNumeroCuenta)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            try
            {
                var AsientoContable = (from n in dbContext.asientocontable

                                       where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.i_Imputable == 1 && n.v_Periodo ==periodo 
                                       select new { n.v_NombreCuenta }).FirstOrDefault();

                if (AsientoContable != null)
                {
                    return AsientoContable.v_NombreCuenta;

                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #endregion
        #region Reportes
        public List<ReporteConciliacionBancaria> ReporteConciliacion(ref OperationResult objOperationResult,  string pstrNumeroCuenta, string pstranio, string pstrMes)
        {

            try
            {
                objOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<ReporteConciliacionBancaria> MasReporte = (from n in dbContext.movimientoestadobancario
                                                                    join a in dbContext.documento on new { td = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = a.i_CodigoDocumento, eliminado = a.i_Eliminado.Value } into a_join
                                                                    from a in a_join

                                                                    where n.i_Eliminado == 0 && n.d_Abono != 0 && n.i_Mes == 0 && n.v_Anio == pstranio && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumeroCuenta && pstrNumeroCuenta.StartsWith("104")
                                                                    select new ReporteConciliacionBancaria
                                                                    {

                                                                        Comprobante = n.v_CodAsiento.Trim() + " " + n.v_NumeroAsiento.Trim(),
                                                                        Documento = a == null ? n.v_NumeroDocumento : a.v_Siglas.Trim() + " " + n.v_NumeroDocumento.Trim(),
                                                                        Fecha = n.t_Fecha,
                                                                        Detalle = n.v_Concepto,
                                                                        Importe = n.d_Abono.Value,
                                                                        Auxiliar = "MAS",
                                                                        ValorAuxiliar = n.d_Abono.Value,


                                                                    }).ToList();

                    List<ReporteConciliacionBancaria> MenosReporte = (from n in dbContext.movimientoestadobancario
                                                                      join a in dbContext.documento on new { td = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = a.i_CodigoDocumento, eliminado = a.i_Eliminado.Value } into a_join
                                                                      from a in a_join
                                                                      where n.i_Eliminado == 0 && n.d_Cargo != 0 && n.i_Mes == 0 && n.v_Anio == pstranio && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumeroCuenta && pstrNumeroCuenta.StartsWith("104")
                                                                      select new ReporteConciliacionBancaria
                                                                      {
                                                                          Comprobante = n.v_CodAsiento.Trim() + " " + n.v_NumeroAsiento.Trim(),
                                                                          Documento = a == null ? n.v_NumeroDocumento.Trim() : a.v_Siglas.Trim() + " " + n.v_NumeroDocumento.Trim(),
                                                                          Fecha = n.t_Fecha,
                                                                          Detalle = n.v_Concepto,
                                                                          Importe = n.d_Cargo.Value,
                                                                          Auxiliar = "MENOS",
                                                                          ValorAuxiliar = n.d_Cargo.Value * -1,

                                                                      }).ToList();

                    List<ReporteConciliacionBancaria> queryFinal = MasReporte.Concat(MenosReporte).ToList(); 
               

                    return queryFinal; 
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            
            }

        }

        public decimal ObtenerSaldoLibros( ref OperationResult objOperationResult,  string pstrNumeroCuenta, string pstrAnio, string pstrMes)
        {
            try
            {
                objOperationResult.Success = 1;
                DateTime FechaInicio = DateTime.Parse("01/01/" + pstrAnio);
                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth (int.Parse ( pstrAnio) , int.Parse (pstrMes))+"/"+ pstrMes + "/"+pstrAnio+" 23:59" );
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var MonedaCuenta = (from n in dbContext.asientocontable
                                        where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Periodo == periodo

                                        select new { n.i_IdMoneda}).FirstOrDefault();
                    if (MonedaCuenta != null)
                    {


                        var Saldo = new TesoreriaBL().ObtenerSaldoMensualBanco(ref objOperationResult, FechaInicio, FechaFin, pstrNumeroCuenta, MonedaCuenta.i_IdMoneda??1,"Fecha",-1,"",false);

                        //var query = (from n in dbContext.saldomensualbancos
                        //             where n.i_Eliminado == 0 && n.v_Anio == pstrAnio && n.v_Mes == pstrMes && n.v_NroCuenta == pstrNumeroCuenta

                        //             select new saldomensualbancosDto
                        //             {
                        //                 SaldoLibroBanco = MonedaCuenta.i_IdMoneda == (int)Moneda.Soles ? n.d_SaldoSoles : n.d_SaldoDolares,

                        //             }).FirstOrDefault();
                        //if (query != null)
                        //{
                        return Saldo;
                        //}
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = " MovimientoEstadoBancarioBL.ObtenerSaldoLibros()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = objOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return 0;
            
            }

        }

        public decimal ObtenerSaldoSegunBanco(string pstrNumeroCuenta, string pstrAnio, string pstrMes)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbContext.saldoestadobancario

                             where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Anio == pstrAnio && n.v_Mes == pstrMes

                             select new { n.d_SaldoBanco }).FirstOrDefault();
                return query != null ? query.d_SaldoBanco.Value : 0;
            }

        }

        public List<ReporteExtractoBancario> ReporteExtractoBancario( ref OperationResult objOperationResult, string pstrNumeroCuenta, string pstrAnio, string pstrMes, int TipoBusqueda)
        {
            try
            {

                objOperationResult.Success = 1;
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


                if (TipoBusqueda == -1)
                {
                    var ReporteExtractoBancario = (from n in dbContext.movimientoestadobancario
                                                   join a in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                equals new { TipoDoc = a.i_CodigoDocumento, eliminado = a.i_Eliminado.Value } into a_join
                                                   from a in a_join.DefaultIfEmpty()

                                                   where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Mes == pstrMes && n.v_Anio == pstrAnio && pstrNumeroCuenta.StartsWith("104")

                                                   select new ReporteExtractoBancario
                                                   {
                                                       Fecha = n.t_Fecha.Value,
                                                       TipoDocumento = a == null ? "" : a.v_Siglas,
                                                       NumeroDocumento = n.v_NumeroDocumento,
                                                       Descripcion = n.v_Concepto,
                                                       Cargo = n.d_Cargo,
                                                       Abono = n.d_Abono,
                                                       Marcado = n.i_Mes == 1 ? "✔" : string.Empty,
                                                   }).ToList();

                    return ReporteExtractoBancario;
                }
                else
                {

                    var ReporteExtractoBancario = (from n in dbContext.movimientoestadobancario
                                                   join a in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                equals new { TipoDoc = a.i_CodigoDocumento, eliminado = a.i_Eliminado.Value } into a_join
                                                   from a in a_join.DefaultIfEmpty()

                                                   where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Mes == pstrMes && n.v_Anio == pstrAnio && n.i_Mes == TipoBusqueda && pstrNumeroCuenta.StartsWith("104")

                                                   select new ReporteExtractoBancario
                                                   {
                                                       Fecha = n.t_Fecha.Value,
                                                       TipoDocumento = a == null ? "" : a.v_Siglas,
                                                       NumeroDocumento = n.v_NumeroDocumento,
                                                       Descripcion = n.v_Concepto,
                                                       Cargo = n.d_Cargo,
                                                       Abono = n.d_Abono,
                                                       Marcado = n.i_Mes == 1 ? "✔" : string.Empty
                                                   }).ToList();

                    return ReporteExtractoBancario;

                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            
            }
        }



        #endregion

    }
}
