using SAMBHS.Common.BE;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Transactions;

namespace SAMBHS.Common.BL
{
    public class SaldoContableBL
    {
        private static string periodo = Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString();
        public void ReprocesarSaldosContables(ref OperationResult pobjOperationResult, int Anio, int Mes = 0)
        {
            try
            {
                //using (var ts = TransactionUtils.CreateTransactionScope())
                //{
                //    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                //    {

                //        List<diariodetalle> _diarios = new List<diariodetalle>();
                //        List<tesoreriadetalle> _tesorerias = new List<tesoreriadetalle>();

                //        #region Elimina la tabla saldoscontables
                //        var SaldosContable = (from n in dbContext.saldoscontables
                //                              select n).ToList();
                //        SaldosContable = Mes != 0 ? SaldosContable.Where(p => p.v_Anio == Anio.ToString() && p.v_Mes.Trim() == Mes.ToString()).ToList() : SaldosContable.Where(p => p.v_Anio == Anio.ToString()).ToList();

                //        foreach (var Saldo in SaldosContable)
                //        {
                //            dbContext.saldoscontables.DeleteObject(Saldo);
                //        }

                //        dbContext.SaveChanges();
                //        #endregion

                //        #region Reprocesar Diarios y Tesorerías
                //        var Consulta = (from n in dbContext.diariodetalle
                //                        where n.t_Fecha.Value.Year == Anio && n.i_Eliminado == 0
                //                        select n);
                //        if (Mes != 0)
                //        {
                //            Consulta = Consulta.Where(p => p.t_Fecha.Value.Month == Mes);
                //        }

                //        var Consulta2 = (from n in dbContext.tesoreriadetalle
                //                         where n.t_Fecha.Value.Year == Anio && n.i_Eliminado == 0
                //                         select n);
                //        if (Mes != 0)
                //        {
                //            Consulta2 = Consulta2.Where(p => p.t_Fecha.Value.Month == Mes);
                //        }

                //        _tesorerias = Consulta2.ToList();

                //        _diarios = Consulta.ToList();

                //        int pbs_Total = _diarios.Count() + _tesorerias.Count();

                //        Globals.ProgressbarStatus.i_TotalProgress = pbs_Total;

                //        if (pbs_Total != 0)
                //        {
                //            Globals.ProgressbarStatus.MensajeOutput = new List<string>
                //            {
                //                string.Format(
                //                    "-------------------- Proceso de Recálculo de Saldos Contables iniciado a las {0} por el usuario {1} --------------------",
                //                    DateTime.Now.ToLongTimeString(), Globals.ClientSession.v_UserName)
                //            };
                //            foreach (diariodetalle _diariodetalle in _diarios)
                //            {
                //                var replicationId = _diariodetalle.v_IdDiarioDetalle.Substring(0, 1);
                //                ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, _diariodetalle.diario.t_Fecha.Value, _diariodetalle.diario.i_IdMoneda.Value, _diariodetalle.ToDTO(), RecordStatus.Agregado, replicationId);
                //                Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("Correcto: {0} en la Tabla: diariodetalle", _diariodetalle.v_IdDiarioDetalle));
                //                Globals.ProgressbarStatus.i_Progress++;
                //            }

                //            foreach (tesoreriadetalle _tesoreria in _tesorerias)
                //            {
                //                var replicationId = _tesoreria.v_IdTesoreriaDetalle.Substring(0, 1);
                //                ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Tesoreria, _tesoreria.tesoreria.t_FechaRegistro.Value, _tesoreria.tesoreria.i_IdMoneda.Value, tesoreriadetalleAssembler.ToDTO(_tesoreria), RecordStatus.Agregado, replicationId);
                //                Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("Correcto: {0} en la Tabla: tesoreriadetalle", _tesoreria.v_IdTesoreriaDetalle));
                //                Globals.ProgressbarStatus.i_Progress++;
                //            }

                //            Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("-------------------------------- Proceso terminado a las {0} con {1} tareas realizadas --------------------------------", DateTime.Now.ToLongTimeString(), Globals.ProgressbarStatus.i_Progress.ToString()));
                //        }
                //        else
                //        {
                //            Globals.ProgressbarStatus.MensajeOutput = new List<string>();
                //            pobjOperationResult.Success = 0;
                //            pobjOperationResult.ErrorMessage = "No se encontró información en la fecha indicada";
                //            Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("Error: {0}, se canceló el proceso.", pobjOperationResult.ErrorMessage));
                //            Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("--------------------------- Proceso terminado con fallas a las {0} con {1} tareas realizadas ----------------------------", DateTime.Now.ToLongTimeString(), Globals.ProgressbarStatus.i_Progress.ToString()));
                //            pobjOperationResult.AdditionalInformation = "SaldoContableBL.ReprocesarSaldosContables()";
                //            return;
                //        }
                //        #endregion
                //    }
                //    ts.Complete();
                //}
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoContableBL.ReprocesarSaldosContables()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("Error: {0}, se canceló el proceso.", pobjOperationResult.ExceptionMessage != string.Empty ? pobjOperationResult.ExceptionMessage : pobjOperationResult.ErrorMessage));
                Globals.ProgressbarStatus.MensajeOutput.Add(string.Format("--------------------------- Proceso terminado con fallas a las {0} con {1} tareas realizadas ----------------------------", DateTime.Now.ToLongTimeString(), Globals.ProgressbarStatus.i_Progress.ToString()));
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        /// <summary>
        /// Actualiza el saldo contable de los procesos de Tesorería y Diario
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="Proceso"> Indicar si es Diario o Tesorería.</param>
        /// <param name="IdMoneda"></param>
        /// <param name="oDto_detalleProceso"></param>
        /// <param name="_RecordStatus"></param>
        /// <param name="MontoSolesAnterior"></param>
        /// <param name="MontoDolaresAnterior"></param>
        public void ActualizarSaldoContable(ref OperationResult pobjOperationResult, ListaProcesos Proceso, DateTime Fecha, int IdMoneda, object oDto_detalleProceso, RecordStatus _RecordStatus, string ReplicationId, decimal MontoSolesAnterior = 0, decimal MontoDolaresAnterior = 0)
        {
            try
            {
                //using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                //{
                //    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                //    {
                //        dynamic _detalleProceso;

                //        switch (Proceso)
                //        {
                //            case ListaProcesos.Tesoreria:
                //                _detalleProceso = (tesoreriadetalleDto)oDto_detalleProceso;
                //                break;

                //            default:
                //                _detalleProceso = (diariodetalleDto)oDto_detalleProceso;
                //                break;
                //        }

                //        string Anio = Fecha.Year.ToString();
                //        string Mes = Fecha.Month.ToString("00");
                //        string NroCuenta = (string)_detalleProceso.v_NroCuenta;
                //        if (!dbContext.asientocontable.Any(p => p.v_NroCuenta == NroCuenta && p.i_Eliminado ==0 && p.v_Periodo ==periodo ))
                //            throw new NullReferenceException(string.Format("Cuenta no existe en el plan contable: {0}",
                //                NroCuenta));

                //        var SaldoDelMesAnioCuenta = (from n in dbContext.saldoscontables
                //                                     where n.v_NroCuenta == NroCuenta &&
                //                                           n.v_Anio == Anio &&
                //                                           n.v_Mes == Mes && n.v_ReplicationId == ReplicationId
                //                                     select n).FirstOrDefault();

                //        decimal ImporteSoles = IdMoneda == 1 ? _detalleProceso.d_Importe : _detalleProceso.d_Cambio;
                //        decimal ImporteDolares = IdMoneda == 2 ? _detalleProceso.d_Importe : _detalleProceso.d_Cambio;

                //        #region Procesar Saldo
                //        switch (_RecordStatus)
                //        {
                //            case RecordStatus.Agregado:

                //                if (SaldoDelMesAnioCuenta != null)
                //                {
                //                    #region Actualiza Saldo
                //                    switch ((string)_detalleProceso.v_Naturaleza)
                //                    {
                //                        case "D":
                //                            SaldoDelMesAnioCuenta.d_ImporteDolaresD = SaldoDelMesAnioCuenta.d_ImporteDolaresD + ImporteDolares ?? ImporteDolares;
                //                            SaldoDelMesAnioCuenta.d_ImporteSolesD = SaldoDelMesAnioCuenta.d_ImporteSolesD + ImporteSoles ?? ImporteSoles;

                //                            break;

                //                        case "H":
                //                            SaldoDelMesAnioCuenta.d_ImporteDolaresH = SaldoDelMesAnioCuenta.d_ImporteDolaresH + ImporteDolares ?? ImporteDolares;
                //                            SaldoDelMesAnioCuenta.d_ImporteSolesH = SaldoDelMesAnioCuenta.d_ImporteSolesH + ImporteSoles ?? ImporteSoles;
                //                            break;
                //                    }
                //                    dbContext.saldoscontables.ApplyCurrentValues(SaldoDelMesAnioCuenta);
                //                    #endregion
                //                }
                //                else
                //                {
                //                    #region Crea Saldo

                //                    saldoscontables _NuevoSaldoAnioMesCuenta = new saldoscontables
                //                    {
                //                        i_IdMoneda = IdMoneda,
                //                        v_Anio = Anio,
                //                        v_Mes = Mes,
                //                        v_NroCuenta = _detalleProceso.v_NroCuenta,
                //                        v_ReplicationId = ReplicationId
                //                    };

                //                    switch ((string)_detalleProceso.v_Naturaleza)
                //                    {
                //                        case "D":
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteDolaresD = ImporteDolares;
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteSolesD = ImporteSoles;
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteDolaresH = 0;
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteSolesH = 0;
                //                            break;

                //                        case "H":
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteDolaresD = 0;
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteSolesD = 0;
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteDolaresH = ImporteDolares;
                //                            _NuevoSaldoAnioMesCuenta.d_ImporteSolesH = ImporteSoles;
                //                            break;
                //                    }
                //                    dbContext.AddTosaldoscontables(_NuevoSaldoAnioMesCuenta);
                //                    #endregion
                //                }
                //                break;

                //            case RecordStatus.Modificado:

                //                if (SaldoDelMesAnioCuenta != null)
                //                {
                //                    #region Actualza Saldo
                //                    switch ((string)_detalleProceso.v_Naturaleza)
                //                    {
                //                        case "D":
                //                            if (SaldoDelMesAnioCuenta.d_ImporteDolaresD != null)
                //                                SaldoDelMesAnioCuenta.d_ImporteDolaresD = (SaldoDelMesAnioCuenta.d_ImporteDolaresD.Value - MontoDolaresAnterior) + ImporteDolares;
                //                            if (SaldoDelMesAnioCuenta.d_ImporteSolesD != null)
                //                                SaldoDelMesAnioCuenta.d_ImporteSolesD = (SaldoDelMesAnioCuenta.d_ImporteSolesD.Value - MontoSolesAnterior) + ImporteSoles;
                //                            break;

                //                        case "H":
                //                            if (SaldoDelMesAnioCuenta.d_ImporteDolaresH != null)
                //                                SaldoDelMesAnioCuenta.d_ImporteDolaresH = (SaldoDelMesAnioCuenta.d_ImporteDolaresH.Value - MontoDolaresAnterior) + ImporteDolares;
                //                            if (SaldoDelMesAnioCuenta.d_ImporteSolesH != null)
                //                                SaldoDelMesAnioCuenta.d_ImporteSolesH = (SaldoDelMesAnioCuenta.d_ImporteSolesH.Value - MontoSolesAnterior) + ImporteSoles;
                //                            break;
                //                    }
                //                    dbContext.saldoscontables.ApplyCurrentValues(SaldoDelMesAnioCuenta);
                //                    #endregion
                //                }
                //                break;

                //            case RecordStatus.EliminadoLogico:

                //                if (SaldoDelMesAnioCuenta != null)
                //                {
                //                    #region Restaura Saldo
                //                    switch ((string)_detalleProceso.v_Naturaleza)
                //                    {
                //                        case "D":
                //                            if (SaldoDelMesAnioCuenta.d_ImporteDolaresD != null)
                //                            {
                //                                SaldoDelMesAnioCuenta.d_ImporteDolaresD = SaldoDelMesAnioCuenta.d_ImporteDolaresD.Value - ImporteDolares;
                //                                if (SaldoDelMesAnioCuenta.d_ImporteSolesD != null)
                //                                    SaldoDelMesAnioCuenta.d_ImporteSolesD = SaldoDelMesAnioCuenta.d_ImporteSolesD.Value - ImporteSoles;

                //                                SaldoDelMesAnioCuenta.d_ImporteDolaresD = SaldoDelMesAnioCuenta.d_ImporteDolaresD < 0 ? 0 : SaldoDelMesAnioCuenta.d_ImporteDolaresD;
                //                            }
                //                            SaldoDelMesAnioCuenta.d_ImporteSolesD = SaldoDelMesAnioCuenta.d_ImporteSolesD < 0 ? 0 : SaldoDelMesAnioCuenta.d_ImporteSolesD;
                //                            break;

                //                        case "H":
                //                            if (SaldoDelMesAnioCuenta.d_ImporteDolaresH != null)
                //                            {
                //                                SaldoDelMesAnioCuenta.d_ImporteDolaresH = SaldoDelMesAnioCuenta.d_ImporteDolaresH.Value - ImporteDolares;
                //                                if (SaldoDelMesAnioCuenta.d_ImporteSolesH != null)
                //                                    SaldoDelMesAnioCuenta.d_ImporteSolesH = SaldoDelMesAnioCuenta.d_ImporteSolesH.Value - ImporteSoles;

                //                                SaldoDelMesAnioCuenta.d_ImporteDolaresH = SaldoDelMesAnioCuenta.d_ImporteDolaresH < 0 ? 0 : SaldoDelMesAnioCuenta.d_ImporteDolaresH;
                //                            }
                //                            SaldoDelMesAnioCuenta.d_ImporteSolesH = SaldoDelMesAnioCuenta.d_ImporteSolesH < 0 ? 0 : SaldoDelMesAnioCuenta.d_ImporteSolesH;
                //                            break;
                //                    }
                //                    dbContext.saldoscontables.ApplyCurrentValues(SaldoDelMesAnioCuenta);
                //                    #endregion
                //                }
                //                break;
                //        }
                //        #endregion

                //        dbContext.SaveChanges();
                //        pobjOperationResult.Success = 1;
                //        ts.Complete();
                //    }
                //}
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoContableBL.ActualizarSaldoContable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        /// <summary>
        /// Obtiene la consulta comparativa de los saldos contables y los saldos administrativos.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrPeriodo"></param>
        /// <returns></returns>
        public List<ComparacionSaldoContableAdministrativo> ObtenerComparacionEntreSaldosContablesYAdministrativos(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Obtiene la consulta Administrativa
                    var queryAdministrativo = (from n in dbContext.cobranzapendiente
                                               join J1 in dbContext.venta on new { id = n.v_IdVenta, estado = 1, eliminado = 0 }
                                                   equals new { id = J1.v_IdVenta, estado = J1.i_IdEstado ?? 0, eliminado = J1.i_Eliminado ?? 0 } into J1_join
                                               from J1 in J1_join.DefaultIfEmpty()

                                               join J2 in dbContext.cliente on J1.v_IdCliente equals J2.v_IdCliente into J2_join
                                               from J2 in J2_join.DefaultIfEmpty()

                                               where J1.v_Periodo.Equals(pstrPeriodo) && n.i_Eliminado == 0 && J1.i_IdTipoDocumento != 7
                                               select new
                                               {
                                                   IdVenta = J1.v_IdVenta,
                                                   TipoDocumento = J1.i_IdTipoDocumento ?? -1,
                                                   NroDocumento = J1.v_SerieDocumento.Trim() + "-" + J1.v_CorrelativoDocumento.Trim(),
                                                   saldoAdministrativo = n.d_Saldo ?? 0,
                                                   Anexo = (J2.v_ApePaterno + " " + J2.v_ApeMaterno + " " + J2.v_PrimerNombre + " " + J2.v_RazonSocial).Trim(),
                                                   IdMoneda = J1.i_IdMoneda ?? 1,
                                                   IdCobranzaPendiente = n.v_IdCobranzaPendiente
                                               }).ToList();
                    #endregion

                    #region Obtiene la consulta contable.
                    var periodo = !string.IsNullOrWhiteSpace(pstrPeriodo) ? int.Parse(pstrPeriodo) : DateTime.Now.Year;
                    var queryContable = dbContext.pendientecobrar.Where(p => p.i_IdTipoDocumento != 7 && p.t_FechaRegistro.HasValue && p.t_FechaRegistro.Value.Year == periodo).ToList();
                    #endregion

                    //mas adelante debera n ser saldo contable en lugar de admninistrativo.
                    #region Obtiene la consulta final
                    var queryComparacion = (from n in queryAdministrativo.AsParallel()
                                            join J1 in queryContable.AsParallel() on new { tipoDoc = n.TipoDocumento, serieCorrelativo = n.NroDocumento }
                                                equals new { tipoDoc = J1.i_IdTipoDocumento ?? 0, serieCorrelativo = J1.v_NroDocumento } into
                                                J1_join
                                            from J1 in J1_join.DefaultIfEmpty()
                                            select new ComparacionSaldoContableAdministrativo
                                            {
                                                Anexo = n.Anexo,
                                                SerieCorrelativo = n.NroDocumento,
                                                IdVenta = n.IdVenta,
                                                SaldoAdministrativo = n.saldoAdministrativo,
                                                SaldoContable = J1 != null ? n.IdMoneda == 1 ? J1.d_ImporteSaldo ?? 0 : J1.d_ImporteSaldoDolares ?? 0 : -1,
                                                IdCobranzaPendiente = n.IdCobranzaPendiente,
                                                IdTipoDocumento = n.TipoDocumento
                                            }).ToList();
                    #endregion

                    if (queryComparacion.Any())
                    {
                        //Se filtran los saldos contables mayores a 0 porque| si el saldo contable es negativo tiene notas de credito asociadas
                        //y eso no se refleja como negativo en el modulo administrativo.
                        queryComparacion = queryComparacion.Where(p => p.SaldoContable >= 0 && p.SaldoContable != p.SaldoAdministrativo && p.SaldoAdministrativo > p.SaldoContable).ToList();
                    }

                    pobjOperationResult.Success = 1;

                    return queryComparacion.Any() ? queryComparacion : null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoContableBL.ObtenerComparacionEntreSaldosContablesYAdministrativos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Iguala los saldos administrativos con los contables, depende de la data que se envie en el método 'ObtenerComparacionEntreSaldosContablesYAdministrativos'
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="plistComparativa"></param>
        /// <param name="pstrPeriodo"></param>
        public void IgualarSaldosContablesAdministrativos(ref OperationResult pobjOperationResult,
            List<ComparacionSaldoContableAdministrativo> plistComparativa, string pstrPeriodo)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var listaCobranzaPendiente = dbContext.cobranzapendiente.Where(p => p.venta != null && p.venta.v_Periodo.Equals(pstrPeriodo) && p.i_Eliminado.HasValue && p.i_Eliminado.Value.Equals(0)).ToList();
                        if (!listaCobranzaPendiente.Any()) throw new Exception("No existe cobranzas pendientes");
                        foreach (var pendienteCobrar in plistComparativa.AsParallel())
                        {
                            if (!listaCobranzaPendiente.Any(p => p.v_IdCobranzaPendiente.Equals(pendienteCobrar.IdCobranzaPendiente))) continue;
                            var cp = listaCobranzaPendiente.FirstOrDefault(p => p.v_IdCobranzaPendiente.Equals(pendienteCobrar.IdCobranzaPendiente));
                            if (cp == null) continue;
                            var valorTotalAdministrativo = cp.d_Acuenta ?? 0 + cp.d_Saldo ?? 0;
                            cp.d_Acuenta = valorTotalAdministrativo - pendienteCobrar.SaldoContable;
                            cp.d_Saldo = pendienteCobrar.SaldoContable;
                            dbContext.cobranzapendiente.ApplyCurrentValues(cp);
                        }

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoContableBL.IgualarSaldosContablesAdministrativos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static object CargarSaldosParaComparacion(string nroCuentaMayor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.pendientecobrar
                        join J1 in dbContext.documento on n.i_IdTipoDocumento ?? 0 equals J1.i_CodigoDocumento into
                            J1_join
                        from J1 in J1_join.DefaultIfEmpty()
                        join J2 in dbContext.cliente on n.v_IdCliente equals J2.v_IdCliente into J2_join
                        from J2 in J2_join.DefaultIfEmpty()
                        where n.v_NroCuenta.Trim().StartsWith(nroCuentaMayor) && n.d_ImporteSaldo > 0
                        select new
                        {
                            Cuenta = n.v_NroCuenta,
                            RucAnexo = J2.v_NroDocIdentificacion,
                            Anexo =
                                (J2.v_ApePaterno + " " + J2.v_ApeMaterno + " " + J2.v_PrimerNombre + " " +
                                 J2.v_SegundoNombre + " " + J2.v_RazonSocial).Trim(),
                            Documento = J1.v_Siglas.Trim() + " " + n.v_NroDocumento.Trim(),
                            SaldoSoles = n.d_ImporteSaldo ?? 0,
                            SaldoDolares = n.d_ImporteSaldoDolares ?? 0
                        }).ToList();

                    return query;
                }
            }
            catch 
            {
                return null;
            }
        }
    }
}
