
using System;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq;
using System.Threading.Tasks;

namespace SAMBHS.CommonWIN.BL
{
    /// <summary>
    /// EQC. 04-08-2016
    /// </summary>
    public class BuscarReemplazarCuentas
    {
        public delegate void Progreso(string progreso);
        public delegate void Error(OperationResult pobjOperationResult);

        public event Progreso OnProgresoEvent;
        public event Error OnErrorEvent;

        protected virtual void OnOnProgresoEvent(string progreso)
        {
            var handler = OnProgresoEvent;
            if (handler != null) handler(progreso);
        }

        protected virtual void OnOnErrorEvent(OperationResult pobjoperationresult)
        {
            var handler = OnErrorEvent;
            if (handler != null) handler(pobjoperationresult);
        }

        /// <summary>
        /// Comienza de forma asincrona el proceso de busqueda y reemplazo de cuentas contables.
        /// </summary>
        /// <param name="pstrCuentaAntigua">Cuenta que se quiere reemplazar</param>
        /// <param name="pstrCuentaNueva">Cuenta que busca reemplazar a la anterior</param>
        public void ComenzarAsync(string pstrCuentaAntigua, string pstrCuentaNueva)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                Task.Factory.StartNew(() =>
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Realiza la búsqueda y el reemplazo.
                        var periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
                        var strPeriodo = periodo.ToString();
                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "clientes"));

                        var clientes =
                            dbContext.cliente.Where(p => p.v_NroCuentaDetraccion.Equals(pstrCuentaAntigua)).ToList();
                        clientes.ForEach(c => c.v_NroCuentaDetraccion = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "compradetalles"));
                        var compradetalles = (from n in dbContext.compradetalle
                                              join J1 in dbContext.compra on n.v_IdCompra equals J1.v_IdCompra into J1_join
                                              from J1 in J1_join.DefaultIfEmpty()
                                              where J1.t_FechaEmision.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                                              select n).ToList();
                        compradetalles.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "ventadetalles"));
                        var ventadetalles = (from n in dbContext.ventadetalle
                                             join J1 in dbContext.venta on n.v_IdVenta equals J1.v_IdVenta into J1_join
                                             from J1 in J1_join.DefaultIfEmpty()
                                             where J1.t_FechaRegistro.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                                             select n).ToList();
                        ventadetalles.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "diariodetalles"));
                        var diariodetalles = (from n in dbContext.diariodetalle
                                              join J1 in dbContext.diario on n.v_IdDiario equals J1.v_IdDiario into J1_join
                                              from J1 in J1_join.DefaultIfEmpty()
                                              where J1.t_Fecha.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                                              select n).ToList();

                        diariodetalles.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "guiaremisioncompradetalles"));
                        var guiaremisioncompradetalles = (from n in dbContext.guiaremisioncompradetalle
                                                          join J1 in dbContext.guiaremisioncompra on n.v_IdGuiaCompra equals J1.v_IdGuiaCompra into
                                                              J1_join
                                                          from J1 in J1_join.DefaultIfEmpty()
                                                          where J1.t_Fecha.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                                                          select n).ToList();

                        guiaremisioncompradetalles.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "liquidacioncompradetalles"));
                        var liquidacioncompradetalles = (from n in dbContext.liquidacioncompradetalle
                            join J1 in dbContext.liquidacioncompra on n.v_IdLiquidacionCompra equals
                                J1.v_IdLiquidacionCompra into J1_join
                            from J1 in J1_join.DefaultIfEmpty()
                            where J1.t_FechaEmision.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                            select n).ToList();    
                        liquidacioncompradetalles.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "movimientoestadobancarios"));
                        var movimientoestadobancarios =
                            dbContext.movimientoestadobancario.Where(p => p.v_Anio.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua)).ToList();
                        movimientoestadobancarios.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "pendientesconciliacions"));
                        
                        var pendientesconciliacions = dbContext.pendientesconciliacion.Where(p => p.v_Anio.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua)).ToList();
                        pendientesconciliacions.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "planillarelaciondescuentos"));
                        var planillarelaciondescuentos = dbContext.planillarelaciondescuentos.Where(p => p.v_Periodo.Trim().Equals(strPeriodo) &&
                            p.v_NroCuenta.Equals(pstrCuentaAntigua)).ToList();
                        planillarelaciondescuentos.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "planillarelacionesaportaciones"));
                        var planillarelacionesaportaciones =
                            dbContext.planillarelacionesaportaciones.Where(
                                p => p.v_Periodo.Trim().Equals(strPeriodo) && p.v_NroCuenta_A.Equals(pstrCuentaAntigua)).ToList();
                        planillarelacionesaportaciones.ForEach(c => c.v_NroCuenta_A = pstrCuentaNueva);

                        var planillarelacionesaportacioneb =
                            dbContext.planillarelacionesaportaciones.Where(
                                p => p.v_Periodo.Trim().Equals(strPeriodo) && p.v_NroCuenta_B.Equals(pstrCuentaAntigua)).ToList();
                        planillarelacionesaportacioneb.ForEach(c => c.v_NroCuenta_B = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "planillarelacionesdescuentosafp"));
                        var planillarelacionesdescuentosafp =
                            dbContext.planillarelacionesdescuentosafp.Where(p => p.v_Periodo.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua))
                                .ToList();
                        planillarelacionesdescuentosafp.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "planillarelacionesnetopagar"));
                        var planillarelacionesnetopagar =
                            dbContext.planillarelacionesnetopagar.Where(p => p.v_Periodo.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua))
                                .ToList();
                        planillarelacionesnetopagar.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "planillarelacioningresos"));
                        var planillarelacioningresos =
                            dbContext.planillarelacioningresos.Where(p => p.v_Periodo.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua))
                                .ToList();
                        planillarelacioningresos.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "recibohonorariodetalle"));
                        var recibohonorariodetalle = (from n in dbContext.recibohonorariodetalle
                            join J1 in dbContext.recibohonorario on n.v_IdReciboHonorario equals J1.v_IdReciboHonorario
                                into J1_join
                            from J1 in J1_join.DefaultIfEmpty()
                            where J1.t_FechaEmision.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                            select n).ToList();

                        recibohonorariodetalle.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "saldoestadobancario"));
                        var saldoestadobancario =
                            dbContext.saldoestadobancario.Where(p => p.v_Anio.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua)).ToList();
                        saldoestadobancario.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "saldomensualbancos"));
                        var saldomensualbancos =
                            dbContext.saldomensualbancos.Where(p => p.v_Anio.Trim().Equals(strPeriodo) && p.v_NroCuenta.Equals(pstrCuentaAntigua)).ToList();
                        saldomensualbancos.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "tesoreria"));
                        var tesoreria = dbContext.tesoreria.Where(p => p.t_FechaRegistro.Value.Year == periodo &&
                            p.v_NroCuentaCajaBanco.Equals(pstrCuentaAntigua)).ToList();
                        tesoreria.ForEach(c => c.v_NroCuentaCajaBanco = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent(string.Format("Procesando: {0}", "tesoreriadetalle"));
                        var tesoreriadetalle = (from n in dbContext.tesoreriadetalle
                            join J1 in dbContext.tesoreria on n.v_IdTesoreria equals J1.v_IdTesoreria into J1_join
                            from J1 in J1_join.DefaultIfEmpty()
                            where J1.t_FechaRegistro.Value.Year == periodo && n.v_NroCuenta.Equals(pstrCuentaAntigua)
                            select n).ToList();
                        tesoreriadetalle.ForEach(c => c.v_NroCuenta = pstrCuentaNueva);

                        if (OnProgresoEvent != null)
                            OnProgresoEvent("Aplicando los cambios...");
                        dbContext.SaveChanges();
                        if (OnProgresoEvent != null)
                            OnProgresoEvent("Proceso Completado!");

                        #endregion
                    }
                }
                , TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "BuscarReemplazarCuentas.ComenzarAsync()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (OnErrorEvent != null)
                    OnErrorEvent(pobjOperationResult);
            }
        }
    }
}
