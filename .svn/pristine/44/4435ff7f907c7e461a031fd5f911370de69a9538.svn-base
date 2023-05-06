using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Linq.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SAMBHS.Tesoreria.BL
{
    public class PendientesBL
    {
        public void InsertarPendientePorDiario(ref OperationResult pobjOperationResult, string pstrIdDiario, List<string> ClientSession, int FlagTipoMovimiento)
        {
            diariodetalle flag;
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();

                        int SecuentialId = 0;
                        string newIdtesoreria = string.Empty;
                        string newIdtesoreriaDetalle = string.Empty;
                        int intNodeId;
                        pendientecobrar objPendienteCobrarEntity = new pendientecobrar();

                        diario DiarioCabecera = (from d in dbContext.diario
                                                 where d.v_IdDiario == pstrIdDiario && d.i_Eliminado == 0
                                                 select d).FirstOrDefault();

                        if (DiarioCabecera == null) return;

                        var VentaRef = (from v in dbContext.venta
                                        where v.v_IdVenta == DiarioCabecera.v_IdDocumentoReferencia
                                        select new { v.i_IdTipoVenta.Value, v.v_IdCliente, v.i_IdTipoDocumento, v.v_SerieDocumento, v_CorrelativoDocumento = v.v_CorrelativoDocumento.Trim() }).FirstOrDefault();

                        #region Inserta Cabecera
                        var DiarioDetalleCuentaPendiente = DiarioCabecera.diariodetalle.Where(p => Utils.Windows.CuentaRequiereDetalle(p.v_NroCuenta) && p.i_Eliminado == 0)
                                                        .GroupBy(p => new { p.v_NroCuenta, p.i_IdTipoDocumento, v_NroDocumento = p.v_NroDocumento.Trim (), p.v_IdCliente, Naturaleza = p.v_Naturaleza });

                        foreach (var diarioDetalleCuenta12O42 in DiarioDetalleCuentaPendiente)
                        {
                            if (diarioDetalleCuenta12O42.Any())
                            {
                                var diaroDetalle = flag = diarioDetalleCuenta12O42.FirstOrDefault();
                                var cuentaDto = Utils.Windows.DevuelveCuentaDatos(diaroDetalle.v_NroCuenta);
                                FlagTipoMovimiento = (int)cuentaDto.NaturalezaCuenta;
                                int idDocumento;
                                string nroDocumeno;
                                string idCliente;

                                if (diaroDetalle.i_IdTipoDocumento != 7 && diaroDetalle.i_IdTipoDocumento != 8)
                                {
                                    idDocumento = diaroDetalle.i_IdTipoDocumento ?? 1;
                                    nroDocumeno = !string.IsNullOrWhiteSpace(diaroDetalle.v_NroDocumento) ? diaroDetalle.v_NroDocumento.Trim() : string.Empty; //Agrego L/C
                                    idCliente = diaroDetalle.v_IdCliente;
                                }
                                else
                                {
                                    idDocumento = diaroDetalle.i_IdTipoDocumentoRef ?? 1;
                                    nroDocumeno = !string.IsNullOrWhiteSpace(diaroDetalle.v_NroDocumentoRef) ? diaroDetalle.v_NroDocumentoRef.Trim() : string.Empty; //Agrego L/C
                                    idCliente = diaroDetalle.v_IdCliente;
                                }

                                objPendienteCobrarEntity = (from pc in dbContext.pendientecobrar
                                                            where pc.i_FlagTipoMovimiento == FlagTipoMovimiento && pc.i_IdTipoDocumento == idDocumento
                                                            && pc.v_NroDocumento == nroDocumeno
                                                            && pc.v_IdCliente == idCliente && pc.v_NroCuenta.Trim() == cuentaDto.v_NroCuenta.Trim()
                                                            select pc).FirstOrDefault();

                                if (objPendienteCobrarEntity != null && objPendienteCobrarEntity.pendientecobrardetalle != null)
                                {
                                    var debeSoles = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "D") != null
                                            ? objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "D").Sum(p => p.d_ImporteSaldo) : 0;
                                    var debeDolares = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "D") != null
                                        ? objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "D").Sum(p => p.d_ImporteSaldoDolares) : 0;
                                    var haberSoles = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "H") != null
                                        ? objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "H").Sum(p => p.d_ImporteSaldo) : 0;
                                    var haberDolares = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "H") != null
                                            ? objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "H").Sum(p => p.d_ImporteSaldoDolares) : 0;

                                    objPendienteCobrarEntity.d_ImporteSaldo = (decimal)((float)debeSoles.Value - (float)haberSoles.Value);
                                    objPendienteCobrarEntity.d_ImporteSaldoDolares = (decimal)((float)debeDolares.Value - (float)haberDolares.Value);
                                    dbContext.pendientecobrar.ApplyCurrentValues(objPendienteCobrarEntity);
                                }

                                if (objPendienteCobrarEntity == null)
                                {
                                    if (diarioDetalleCuenta12O42 != null)
                                    {
                                        objPendienteCobrarEntity = new pendientecobrar();
                                        objPendienteCobrarEntity.v_NroCuenta = diaroDetalle.v_NroCuenta;
                                        objPendienteCobrarEntity.t_InsertaFecha = DateTime.Now;
                                        objPendienteCobrarEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                        objPendienteCobrarEntity.t_FechaRegistro = DiarioCabecera.t_Fecha;
                                        objPendienteCobrarEntity.d_ImporteSaldo = DiarioCabecera.i_IdMoneda == 1 ? DevuelveValorRedondeado(diarioDetalleCuenta12O42.Sum(p => p.d_Importe ?? 0)) : diarioDetalleCuenta12O42.Sum(p => p.d_Cambio ?? 0);
                                        objPendienteCobrarEntity.d_ImporteSaldoDolares = DiarioCabecera.i_IdMoneda == 2 ? DevuelveValorRedondeado(diarioDetalleCuenta12O42.Sum(p => p.d_Importe ?? 0)) : diarioDetalleCuenta12O42.Sum(p => p.d_Cambio ?? 0);
                                        objPendienteCobrarEntity.i_IdMoneda = DiarioCabecera.i_IdMoneda;
                                        objPendienteCobrarEntity.t_FechaRegistro = DiarioCabecera.t_Fecha;
                                        objPendienteCobrarEntity.v_IdCliente = VentaRef != null ? VentaRef.v_IdCliente : diaroDetalle.v_IdCliente;
                                        objPendienteCobrarEntity.v_NroDocumento = VentaRef != null ? VentaRef.v_SerieDocumento.Trim() + "-" + VentaRef.v_CorrelativoDocumento.Trim() : diaroDetalle.v_NroDocumento.Trim();
                                        objPendienteCobrarEntity.i_IdTipoDocumento = VentaRef != null ? VentaRef.i_IdTipoDocumento : diaroDetalle.i_IdTipoDocumento ?? 0;
                                        intNodeId = int.Parse(ClientSession[0]);
                                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 61);
                                        newIdtesoreria = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XK");
                                        objPendienteCobrarEntity.v_IdPendienteCobrar = newIdtesoreria;
                                        objPendienteCobrarEntity.i_FlagTipoMovimiento = FlagTipoMovimiento;

                                        string RUC = "";
                                        if (FlagTipoMovimiento == (int)TipoMovimientoTesoreria.Egreso)
                                        {
                                            var _cliente = (from p in dbContext.cliente
                                                            where p.v_IdCliente == objPendienteCobrarEntity.v_IdCliente
                                                            select new { p.v_NroDocIdentificacion }).FirstOrDefault();

                                            RUC = _cliente != null ? _cliente.v_NroDocIdentificacion : string.Empty;
                                            objPendienteCobrarEntity.v_NroRucProveedor = RUC;
                                        }

                                        dbContext.AddTopendientecobrar(objPendienteCobrarEntity);

                                        #region Inserta Detalle
                                        pendientecobrardetalle objEntityPendienteDetalle = new pendientecobrardetalle();
                                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 62);
                                        newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                                        objEntityPendienteDetalle.v_IdPendienteCobrarDetalle = newIdtesoreriaDetalle;
                                        objEntityPendienteDetalle.v_IdDiario = DiarioCabecera.v_IdDiario;
                                        objEntityPendienteDetalle.v_IdPendienteCobrar = newIdtesoreria;
                                        objEntityPendienteDetalle.v_Naturaleza = diaroDetalle.v_Naturaleza;
                                        objEntityPendienteDetalle.v_NroCuenta = diaroDetalle.v_NroCuenta;
                                        objEntityPendienteDetalle.v_NroDocumento = DiarioCabecera.v_Mes + "-" + DiarioCabecera.v_Correlativo;
                                        objEntityPendienteDetalle.d_ImporteSaldo = DiarioCabecera.i_IdMoneda == 1 ? DevuelveValorRedondeado(diarioDetalleCuenta12O42.Sum(p => p.d_Importe ?? 0)) : diarioDetalleCuenta12O42.Sum(p => p.d_Cambio ?? 0);
                                        objEntityPendienteDetalle.d_ImporteSaldoDolares = DiarioCabecera.i_IdMoneda == 2 ? DevuelveValorRedondeado(diarioDetalleCuenta12O42.Sum(p => p.d_Importe ?? 0)) : diarioDetalleCuenta12O42.Sum(p => p.d_Cambio ?? 0);
                                        objEntityPendienteDetalle.t_InsertaFecha = DateTime.Now;
                                        objEntityPendienteDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                        objEntityPendienteDetalle.v_IdCliente = VentaRef != null ? VentaRef.v_IdCliente : diaroDetalle.v_IdCliente;
                                        objEntityPendienteDetalle.v_IdDiario = DiarioCabecera.v_IdDiario;
                                        objEntityPendienteDetalle.t_FechaRegistro = diaroDetalle.t_Fecha;
                                        objEntityPendienteDetalle.i_IdMoneda = DiarioCabecera.i_IdMoneda;
                                        objEntityPendienteDetalle.i_IdTipoDocumento = DiarioCabecera.i_IdTipoDocumento;
                                        objEntityPendienteDetalle.v_NroRucProveedor = RUC;
                                        dbContext.AddTopendientecobrardetalle(objEntityPendienteDetalle);
                                        #endregion

                                        dbContext.SaveChanges();
                                        pobjOperationResult.Success = 1;
                                    }
                                }
                                else
                                {
                                    string RUC = "";
                                    if (FlagTipoMovimiento == (int)TipoMovimientoTesoreria.Egreso)
                                    {
                                        var _cliente = (from p in dbContext.cliente
                                                        where p.v_IdCliente == objPendienteCobrarEntity.v_IdCliente
                                                        select new { p.v_NroDocIdentificacion }).FirstOrDefault();

                                        RUC = _cliente != null ? _cliente.v_NroDocIdentificacion : string.Empty;
                                        objPendienteCobrarEntity.v_NroRucProveedor = RUC;
                                    }

                                    #region Inserta Detalle
                                    intNodeId = int.Parse(ClientSession[0]);
                                    pendientecobrardetalle objEntityPendienteDetalle = new pendientecobrardetalle();
                                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 62);
                                    newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                                    objEntityPendienteDetalle.v_IdPendienteCobrarDetalle = newIdtesoreriaDetalle;
                                    objEntityPendienteDetalle.v_IdDiario = DiarioCabecera.v_IdDiario;
                                    objEntityPendienteDetalle.v_IdPendienteCobrar = objPendienteCobrarEntity.v_IdPendienteCobrar;
                                    objEntityPendienteDetalle.v_Naturaleza = diaroDetalle.v_Naturaleza;
                                    objEntityPendienteDetalle.v_NroCuenta = diaroDetalle.v_NroCuenta;
                                    objEntityPendienteDetalle.v_NroDocumento = DiarioCabecera.v_Mes + "-" + DiarioCabecera.v_Correlativo;
                                    objEntityPendienteDetalle.d_ImporteSaldo = DiarioCabecera.i_IdMoneda == 1 ? DevuelveValorRedondeado(diarioDetalleCuenta12O42.Sum(p => p.d_Importe ?? 0)) : diarioDetalleCuenta12O42.Sum(p => p.d_Cambio ?? 0);
                                    objEntityPendienteDetalle.d_ImporteSaldoDolares = DiarioCabecera.i_IdMoneda == 2 ? DevuelveValorRedondeado(diarioDetalleCuenta12O42.Sum(p => p.d_Importe ?? 0)) : diarioDetalleCuenta12O42.Sum(p => p.d_Cambio ?? 0);
                                    objEntityPendienteDetalle.t_InsertaFecha = DateTime.Now;
                                    objEntityPendienteDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                    objEntityPendienteDetalle.v_IdCliente = VentaRef != null ? VentaRef.v_IdCliente : diaroDetalle.v_IdCliente;
                                    objEntityPendienteDetalle.v_IdDiario = DiarioCabecera.v_IdDiario;
                                    objEntityPendienteDetalle.t_FechaRegistro = diaroDetalle.t_Fecha;
                                    objEntityPendienteDetalle.i_IdMoneda = DiarioCabecera.i_IdMoneda;
                                    objEntityPendienteDetalle.i_IdTipoDocumento = DiarioCabecera.i_IdTipoDocumento;
                                    objEntityPendienteDetalle.v_NroRucProveedor = RUC;
                                    dbContext.AddTopendientecobrardetalle(objEntityPendienteDetalle);
                                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pendientecobrardetalle", objEntityPendienteDetalle.v_IdPendienteCobrarDetalle);
                                    #endregion

                                    if (objEntityPendienteDetalle.v_Naturaleza == "H")
                                    {
                                        objPendienteCobrarEntity.d_ImporteSaldo = (decimal)((float)objPendienteCobrarEntity.d_ImporteSaldo.Value - (float)objEntityPendienteDetalle.d_ImporteSaldo.Value);
                                        objPendienteCobrarEntity.d_ImporteSaldoDolares = (decimal)((float)objPendienteCobrarEntity.d_ImporteSaldoDolares.Value - (float)objEntityPendienteDetalle.d_ImporteSaldoDolares.Value);
                                    }
                                    else if (objEntityPendienteDetalle.v_Naturaleza == "D")
                                    {
                                        objPendienteCobrarEntity.d_ImporteSaldo = (decimal)((float)objPendienteCobrarEntity.d_ImporteSaldo.Value + (float)objEntityPendienteDetalle.d_ImporteSaldo.Value);
                                        objPendienteCobrarEntity.d_ImporteSaldoDolares = (decimal)((float)objPendienteCobrarEntity.d_ImporteSaldoDolares.Value + (float)objEntityPendienteDetalle.d_ImporteSaldoDolares.Value);
                                    }

                                    objPendienteCobrarEntity.d_ImporteSaldo = objPendienteCobrarEntity.d_ImporteSaldo < 0 ? objPendienteCobrarEntity.d_ImporteSaldo.Value * -1 : objPendienteCobrarEntity.d_ImporteSaldo.Value;
                                    objPendienteCobrarEntity.d_ImporteSaldoDolares = objPendienteCobrarEntity.d_ImporteSaldoDolares < 0 ? objPendienteCobrarEntity.d_ImporteSaldoDolares.Value * -1 : objPendienteCobrarEntity.d_ImporteSaldoDolares.Value;

                                    dbContext.pendientecobrar.ApplyCurrentValues(objPendienteCobrarEntity);
                                    dbContext.SaveChanges();

                                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pendientecobrar", objPendienteCobrarEntity.v_IdPendienteCobrar);

                                    pobjOperationResult.Success = 1;
                                }
                            }
                        }


                        #endregion
                        pobjOperationResult.Success = 1;
                        ts.Complete(); 
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PendientesBL.InsertarPendientePorDiario()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void InsertarPendientePorTesoreria(ref OperationResult pobjOperationResult, string pstrIdTesoreria, tesoreriadetalle TesoreriaDetalleCuenta12o42, List<string> ClientSession, int FlagTipoMovimiento)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SecuentialBL objSecuentialBL = new SecuentialBL();

                    int SecuentialId = 0;
                    string newIdtesoreria = string.Empty;
                    string newIdtesoreriaDetalle = string.Empty;
                    int intNodeId;

                    tesoreria TesoreriaCabecera = (from d in dbContext.tesoreria
                                                   where d.v_IdTesoreria == pstrIdTesoreria && d.i_Eliminado == 0
                                                   select d).FirstOrDefault();

                    #region Inserta Cabecera

                        if (TesoreriaDetalleCuenta12o42 != null)
                        {
                            var objPendienteCobrarEntity = new pendientecobrar();
                            var diaroDetalle = TesoreriaDetalleCuenta12o42;
                            var cuentaDto = Utils.Windows.DevuelveCuentaDatos(diaroDetalle.v_NroCuenta);
                            FlagTipoMovimiento = (int)cuentaDto.NaturalezaCuenta;

                            var objPendienteCobrar = (from p in dbContext.pendientecobrar
                                                    where
                                                        p.i_IdTipoDocumento == TesoreriaDetalleCuenta12o42.i_IdTipoDocumento &&
                                                        p.v_NroCuenta == TesoreriaDetalleCuenta12o42.v_NroCuenta &&
                                                        p.v_NroDocumento == TesoreriaDetalleCuenta12o42.v_NroDocumento.Trim() &&
                                                        p.i_FlagTipoMovimiento == FlagTipoMovimiento && p.v_IdCliente.Equals(TesoreriaDetalleCuenta12o42.v_IdCliente)
                                                    select p).FirstOrDefault();


                            var PendienteCobrarDetalle = (from pcd in dbContext.pendientecobrardetalle
                                                          where pcd.v_IdTesoreria == TesoreriaCabecera.v_IdTesoreria && pcd.v_IdCliente == diaroDetalle.v_IdCliente
                                                          select pcd).FirstOrDefault();

                            if (objPendienteCobrar == null)
                            {
                                objPendienteCobrarEntity.v_NroCuenta = diaroDetalle.v_NroCuenta;
                                objPendienteCobrarEntity.t_InsertaFecha = DateTime.Now;
                                objPendienteCobrarEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objPendienteCobrarEntity.t_FechaRegistro = TesoreriaCabecera.t_FechaRegistro;
                                objPendienteCobrarEntity.d_ImporteSaldo = TesoreriaCabecera.i_IdMoneda == 1
                                    ? DevuelveValorRedondeado(TesoreriaCabecera.d_TotalHaber_Importe.Value) * -1
                                    : DevuelveValorRedondeado(TesoreriaCabecera.d_TotalHaber_Importe.Value *
                                                              TesoreriaCabecera.d_TipoCambio.Value) * -1;
                                objPendienteCobrarEntity.d_ImporteSaldoDolares = TesoreriaCabecera.i_IdMoneda == 2
                                    ? DevuelveValorRedondeado(TesoreriaCabecera.d_TotalHaber_Importe.Value) * -1
                                    : DevuelveValorRedondeado(TesoreriaCabecera.d_TotalHaber_Importe.Value /
                                                              TesoreriaCabecera.d_TipoCambio.Value) * -1;
                                objPendienteCobrarEntity.i_IdMoneda = TesoreriaCabecera.i_IdMoneda;
                                objPendienteCobrarEntity.t_FechaRegistro = TesoreriaCabecera.t_FechaRegistro;
                                objPendienteCobrarEntity.v_IdCliente = diaroDetalle.v_IdCliente;
                                objPendienteCobrarEntity.v_NroDocumento = diaroDetalle.v_NroDocumento;

                                objPendienteCobrarEntity.i_IdTipoDocumento = diaroDetalle.i_IdTipoDocumento.Value;
                                intNodeId = int.Parse(ClientSession[0]);
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 61);
                                newIdtesoreria = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XK");
                                objPendienteCobrarEntity.v_IdPendienteCobrar = newIdtesoreria;
                                objPendienteCobrarEntity.i_FlagTipoMovimiento = FlagTipoMovimiento;

                                string RUC = "";
                                if (FlagTipoMovimiento == (int)TipoMovimientoTesoreria.Egreso)
                                {
                                    RUC = (from p in dbContext.cliente
                                           where p.v_IdCliente == objPendienteCobrarEntity.v_IdCliente
                                           select new { p.v_NroDocIdentificacion }).First().v_NroDocIdentificacion;
                                    objPendienteCobrarEntity.v_NroRucProveedor = RUC;
                                }

                                dbContext.AddTopendientecobrar(objPendienteCobrarEntity);
                                dbContext.SaveChanges();
                                #region Inserta Detalle

                                pendientecobrardetalle objEntityPendienteDetalle = new pendientecobrardetalle();
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 62);
                                newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                                objEntityPendienteDetalle.v_IdPendienteCobrarDetalle = newIdtesoreriaDetalle;
                                objEntityPendienteDetalle.v_IdTesoreria = TesoreriaCabecera.v_IdTesoreria;
                                objEntityPendienteDetalle.v_IdPendienteCobrar = newIdtesoreria;
                                objEntityPendienteDetalle.v_Naturaleza = diaroDetalle.v_Naturaleza;
                                objEntityPendienteDetalle.v_NroCuenta = diaroDetalle.v_NroCuenta;
                                objEntityPendienteDetalle.v_NroDocumento = TesoreriaCabecera.v_Mes + "-" +
                                                                           TesoreriaCabecera.v_Correlativo;
                                objEntityPendienteDetalle.d_ImporteSaldo = TesoreriaCabecera.i_IdMoneda == 1
                                    ? DevuelveValorRedondeado(diaroDetalle.d_Importe.Value)
                                    : DevuelveValorRedondeado(diaroDetalle.d_Importe.Value *
                                                              TesoreriaCabecera.d_TipoCambio.Value);
                                objEntityPendienteDetalle.d_ImporteSaldoDolares = TesoreriaCabecera.i_IdMoneda == 2
                                    ? DevuelveValorRedondeado(diaroDetalle.d_Importe.Value)
                                    : DevuelveValorRedondeado(diaroDetalle.d_Importe.Value /
                                                              TesoreriaCabecera.d_TipoCambio.Value);
                                objEntityPendienteDetalle.t_InsertaFecha = DateTime.Now;
                                objEntityPendienteDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityPendienteDetalle.v_IdCliente = diaroDetalle.v_IdCliente;
                                objEntityPendienteDetalle.t_FechaRegistro = diaroDetalle.t_Fecha;
                                objEntityPendienteDetalle.i_IdMoneda = TesoreriaCabecera.i_IdMoneda;
                                objEntityPendienteDetalle.i_IdTipoDocumento = TesoreriaCabecera.i_IdTipoDocumento;
                                objEntityPendienteDetalle.v_NroRucProveedor = RUC;
                                dbContext.AddTopendientecobrardetalle(objEntityPendienteDetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pendientecobrardetalle", objEntityPendienteDetalle.v_IdPendienteCobrarDetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pendientecobrar", objPendienteCobrarEntity.v_IdPendienteCobrar);
                                dbContext.SaveChanges();
                                #endregion

                            }
                            else if (PendienteCobrarDetalle == null)
                            {
                                string RUC = "";
                                if (FlagTipoMovimiento == (int)TipoMovimientoTesoreria.Egreso)
                                {
                                    RUC = (from p in dbContext.cliente
                                           where p.v_IdCliente == objPendienteCobrar.v_IdCliente //cambie
                                           select new { p.v_NroDocIdentificacion }).First().v_NroDocIdentificacion;
                                }
                                #region Inserta Detalle
                                intNodeId = int.Parse(ClientSession[0]);
                                pendientecobrardetalle objEntityPendienteDetalle = new pendientecobrardetalle();
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 62);
                                newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                                objEntityPendienteDetalle.v_IdPendienteCobrarDetalle = newIdtesoreriaDetalle;
                                objEntityPendienteDetalle.v_IdTesoreria = TesoreriaCabecera.v_IdTesoreria;
                                objEntityPendienteDetalle.v_IdPendienteCobrar = objPendienteCobrar.v_IdPendienteCobrar; //cambie
                                objEntityPendienteDetalle.v_Naturaleza = diaroDetalle.v_Naturaleza;
                                objEntityPendienteDetalle.v_NroCuenta = diaroDetalle.v_NroCuenta;
                                objEntityPendienteDetalle.v_NroDocumento = TesoreriaCabecera.v_Mes + "-" +
                                                                           TesoreriaCabecera.v_Correlativo;
                                objEntityPendienteDetalle.d_ImporteSaldo = TesoreriaCabecera.i_IdMoneda == 1
                                    ? DevuelveValorRedondeado(diaroDetalle.d_Importe.Value)
                                    : DevuelveValorRedondeado(diaroDetalle.d_Importe.Value *
                                                              TesoreriaCabecera.d_TipoCambio.Value);
                                objEntityPendienteDetalle.d_ImporteSaldoDolares = TesoreriaCabecera.i_IdMoneda == 2
                                    ? DevuelveValorRedondeado(diaroDetalle.d_Importe.Value)
                                    : DevuelveValorRedondeado(diaroDetalle.d_Importe.Value /
                                                              TesoreriaCabecera.d_TipoCambio.Value);
                                objEntityPendienteDetalle.t_InsertaFecha = DateTime.Now;
                                objEntityPendienteDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityPendienteDetalle.v_IdCliente = diaroDetalle.v_IdCliente;
                                objEntityPendienteDetalle.t_FechaRegistro = diaroDetalle.t_Fecha;
                                objEntityPendienteDetalle.i_IdMoneda = TesoreriaCabecera.i_IdMoneda;
                                objEntityPendienteDetalle.i_IdTipoDocumento = TesoreriaCabecera.i_IdTipoDocumento;
                                objEntityPendienteDetalle.v_NroRucProveedor = RUC;
                                dbContext.AddTopendientecobrardetalle(objEntityPendienteDetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pendientecobrardetalle", objEntityPendienteDetalle.v_IdPendienteCobrarDetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pendientecobrar", objPendienteCobrarEntity.v_IdPendienteCobrar);
                                dbContext.SaveChanges();
                                #endregion

                            }
                    }

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PendientesBL.InsertarPendientePorTesoreria()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public List<pendientecobrarDto> ListarPendientesJerarquica(ref OperationResult pobjOperationResult, string pstrFilterExpression, DateTime ptimeFInicio, DateTime ptimeFFin)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodoActual = Globals.ClientSession.i_Periodo;
                    #region Query Base
                    var query = (from pendientecobrar n in dbContext.pendientecobrar
                                 where n.t_FechaRegistro >= ptimeFInicio && n.t_FechaRegistro <= ptimeFFin
                                 select new
                                 {
                                     v_IdPendienteCobrar = n.v_IdPendienteCobrar,
                                     v_NroCuenta = n.v_NroCuenta,
                                     v_IdCliente = n.v_IdCliente,
                                     v_NroDocumento = n.v_NroDocumento ?? string.Empty,
                                     t_FechaReferencia = n.t_FechaReferencia,
                                     i_IdTipoDocumento = n.i_IdTipoDocumento,
                                     i_IdMoneda = n.i_IdMoneda,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     d_ImporteSaldo = n.d_ImporteSaldo,
                                     d_ImporteSaldoDolares = n.d_ImporteSaldoDolares,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     t_FechaRegistro = n.t_FechaRegistro,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     i_FlagTipoMovimiento = n.i_FlagTipoMovimiento,
                                     pendientecobrardetalleDto = (from pendientecobrardetalle AA in dbContext.pendientecobrardetalle

                                                                  join J1 in dbContext.documento on new { p = AA.i_IdTipoDocumento.Value, eliminado = 0 } equals new { p = J1.i_CodigoDocumento, eliminado = J1.i_Eliminado.Value } into J1_join
                                                                  from J1 in J1_join.DefaultIfEmpty()

                                                                  join J2 in dbContext.diario on new { d = AA.v_IdDiario, eliminado = 0 } equals new { d = J2.v_IdDiario, eliminado = J2.i_Eliminado.Value } into J2_join
                                                                  from J2 in J2_join.DefaultIfEmpty()

                                                                  join J3 in dbContext.systemuser on new { i_InsertUserId = AA.i_InsertaIdUsuario.Value, eliminado = 0 }
                                                                                                equals new { i_InsertUserId = J3.i_SystemUserId, eliminado = J3.i_IsDeleted.Value } into J3_join
                                                                  from J3 in J3_join.DefaultIfEmpty()

                                                                  join J4 in dbContext.tesoreria on AA.v_IdTesoreria equals J4.v_IdTesoreria into J4_join
                                                                  from J4 in J4_join.DefaultIfEmpty()

                                                                  where AA.v_IdPendienteCobrar == n.v_IdPendienteCobrar

                                                                  select new pendientecobrardetalleDto
                                                                  {
                                                                      d_ImporteSaldo = AA.d_ImporteSaldo,
                                                                      d_ImporteSaldoDolares = AA.d_ImporteSaldoDolares,
                                                                      t_InsertaFecha = AA.t_InsertaFecha,
                                                                      t_FechaRegistro = AA.t_FechaRegistro,
                                                                      v_IdCliente = AA.v_IdCliente,
                                                                      v_IdDiario = AA.v_IdDiario,
                                                                      v_IdPendienteCobrar = AA.v_IdPendienteCobrar,
                                                                      v_IdPendienteCobrarDetalle = AA.v_IdPendienteCobrarDetalle,
                                                                      v_IdTesoreria = AA.v_IdTesoreria,
                                                                      v_Naturaleza = AA.v_Naturaleza,
                                                                      v_NroCuenta = AA.v_NroCuenta,
                                                                      v_NroDocumento = AA.v_NroDocumento,
                                                                      TipoDocumento = AA.v_IdDiario != null ? J1 == null ? J2.v_Mes.Trim() + "-" + J2.v_Correlativo : J1.v_Siglas + " " + J2.v_Mes.Trim() + "-" + J2.v_Correlativo : J1.v_Siglas + " " + J4.v_Mes.Trim() + "-" + J4.v_Correlativo,
                                                                      Moneda = AA.i_IdMoneda == 1 ? "S/." : "US$.",
                                                                      v_UsuarioCreacion = J3 != null ? J3.v_UserName : "",
                                                                  }).AsEnumerable()
                                 }
                                 ).AsEnumerable();
                    #endregion

                    #region Query Resultante
                    var v = (from n in query

                             join A in dbContext.cliente on new { n.v_IdCliente } equals new { A.v_IdCliente } into A_join
                             from A in A_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                            equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J2 in dbContext.documento on n.i_IdTipoDocumento.Value equals J2.i_CodigoDocumento into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             select new pendientecobrarDto
                             {
                                 v_IdPendienteCobrar = n.v_IdPendienteCobrar,
                                 v_NroCuenta = n.v_NroCuenta,
                                 t_FechaRegistro = n.t_FechaRegistro,
                                 v_IdCliente = n.v_IdCliente,
                                 NombreCliente = A != null ? n.v_IdCliente != "N002-CL000000000" ? (A.v_PrimerNombre + " " + A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_RazonSocial).Trim() : "Público General" : string.Empty,
                                 d_ImporteSaldo = n.d_ImporteSaldo,
                                 d_ImporteSaldoDolares = n.d_ImporteSaldoDolares,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 v_UsuarioCreacion = J3 == null ? "" : J3.v_UserName,
                                 RUC = A != null ? A.v_NroDocIdentificacion : string.Empty,
                                 NroDocumento = n.v_NroDocumento,
                                 CodAnexo = A != null ? A.v_CodCliente : string.Empty,
                                 i_FlagTipoMovimiento = n.i_FlagTipoMovimiento,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 pendientecobrardetalleDto = n.pendientecobrardetalleDto != null ? n.pendientecobrardetalleDto.ToList() : null,
                                 Moneda = n.i_IdMoneda == 1 ? "S/." : "US$.",
                                 TipoDocumento = J2 != null ? J2.v_Siglas + " " + n.v_NroDocumento : string.Empty
                             }
                             ).AsQueryable();
                    #endregion

                    if (v != null)
                    {
                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                            v = v.Where(pstrFilterExpression);

                        List<pendientecobrarDto> objData = v.ToList();
                        pobjOperationResult.Success = 1;

                        return objData;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PendientesBL.ListarPendientesJerarquica()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string DevuelveCuentaPVenta(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string NroCuentaPVenta = (from c in dbContext.administracionconceptos
                                              where c.v_Codigo == "03" && c.i_Eliminado == 0
                                              select new { c.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta.Trim();

                    pobjOperationResult.Success = 1;
                    return NroCuentaPVenta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "Al parecer no se encuentra el concepto 03." + '\n' + "PendientesBL.DevuelveCuentaPVenta()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }

        public string DevuelveCuentaPCompra(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string NroCuentaPVenta = (from c in dbContext.administracionconceptos
                                              where c.v_Codigo == "01" && c.i_Eliminado == 0
                                              select new { c.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta.Trim();
                    pobjOperationResult.Success = 1;

                    return NroCuentaPVenta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "Al parecer no se encuentra el concepto 01." + '\n' + "PendientesBL.DevuelveCuentaPCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }

        public decimal DevuelveValorRedondeado(decimal Valor)
        {
            return decimal.Parse(Math.Round(Valor, 2, MidpointRounding.AwayFromZero).ToString());
        }

        public void ReprocesarPendientes(ref OperationResult pobjOperationResult, int pintMesFinal, int pintPeriodo)
        {
            var flagIdDiario = string.Empty;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();

                    IEnumerable<pendientecobrar> Pendientes = dbContext.pendientecobrar.AsParallel().Where(o => o.t_FechaRegistro.Value.Year == pintPeriodo && o.t_FechaRegistro.Value.Month >= 1 && o.t_FechaRegistro.Value.Month <= pintMesFinal).ToList();
                    IEnumerable<tesoreriadetalle> _tesoreriasdetale = dbContext.tesoreriadetalle.AsParallel().Where(p => p.t_Fecha.Value.Year == pintPeriodo && p.t_Fecha.Value.Month >= 1 && p.t_Fecha.Value.Month <= pintMesFinal && p.i_Eliminado == 0).ToList();
                    IEnumerable<diario> _diarios = dbContext.diario.AsParallel().Where(p => p.t_Fecha.Value.Year == pintPeriodo && p.t_Fecha.Value.Month >= 1 && p.t_Fecha.Value.Month <= pintMesFinal && p.i_Eliminado == 0).ToList();
                    var ClientesGlobals = dbContext.cliente.AsParallel().Where(p => p.i_Eliminado == 0).ToList();

                    #region Elimina Anteriores Pendientes
                    foreach (pendientecobrar Pendiente in Pendientes.AsParallel())
                    {
                        Pendiente.pendientecobrardetalle.ToList().ForEach(p => dbContext.DeleteObject(p));
                        dbContext.DeleteObject(Pendiente);
                    }
                    dbContext.SaveChanges();
                    #endregion

                    var Tesorerias = _tesoreriasdetale.AsParallel().Where(p => Utils.Windows.CuentaRequiereDetalle(p.v_NroCuenta) && p.i_Eliminado == 0);
                    Globals.ProgressbarStatus.i_TotalProgress = _diarios.Count() + Tesorerias.Count();

                    foreach (var _diario in _diarios.AsParallel())
                    {
                        if (!string.IsNullOrEmpty(_diario.v_IdDiario))
                        {
                            flagIdDiario = _diario.v_IdDiario;
                            InsertarPendientePorDiario(ref pobjOperationResult, _diario.v_IdDiario, Globals.ClientSession.GetAsList(), 0);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        Globals.ProgressbarStatus.i_Progress++;
                    }
                    foreach (var tesoreriadetalle in Tesorerias.AsParallel())
                    {
                        TesoreriaBL _objTesoreriaBL = new TesoreriaBL();
                        tesoreriaDto _tesoreriaDto = tesoreriadetalle.tesoreria.ToDTO();
                        int TipoMov = _tesoreriaDto.i_TipoMovimiento.Value;

                        var _cliente = (from p in ClientesGlobals
                                        where p.v_IdCliente == tesoreriadetalle.v_IdCliente
                                        select new { p.v_NroDocIdentificacion }).FirstOrDefault();

                        string RUC = _cliente != null ? _cliente.v_NroDocIdentificacion : string.Empty;

                        if (!string.IsNullOrEmpty(RUC))
                        {
                            _objTesoreriaBL.ActualizaPendientePorCobrarDetalle(ref pobjOperationResult, _tesoreriaDto, tesoreriadetalle, Globals.ClientSession.GetAsList(), TipoMov, RUC);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        Globals.ProgressbarStatus.i_Progress++;
                    }

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.REPROCESO, Globals.ClientSession.v_UserName, "pendientecobrar", "");

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PendientesBL.ReprocesarPendientes()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + '\n' + flagIdDiario;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }
}
