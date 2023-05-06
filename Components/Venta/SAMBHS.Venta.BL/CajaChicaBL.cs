using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.DataModel;
using System.ComponentModel;
using System.Transactions;
using System.Data;
using SAMBHS.Tesoreria.BL;


namespace SAMBHS.Venta.BL
{
    public class CajaChicaBL
    {

        TesoreriaBL _objTesoreriaBL = new TesoreriaBL();

        public cajachicaDto ObtenerCabeceraCajaChica(ref OperationResult objOperationResult, string IdCajaChica)
        {
            try
            {



                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var cajachica = (from a in dbContext.cajachica
                                     join b in dbContext.systemuser on new { usuario = a.i_InsertaIdUsuario, eliminado = 0 } equals new { usuario = b.i_InsertUserId, eliminado = b.i_IsDeleted.Value } into b_join
                                     from  b in b_join.DefaultIfEmpty ()                                     
                                     where a.i_Eliminado == 0 && a.v_IdCajaChica == IdCajaChica
                                     select new cajachicaDto
                                     {
                                         v_IdCajaChica = a.v_IdCajaChica,
                                         v_Periodo = a.v_Periodo.Trim(),
                                         v_Mes = a.v_Mes.Trim(),
                                         v_Correlativo = a.v_Correlativo.Trim(),
                                         t_FechaRegistro = a.t_FechaRegistro,
                                         i_IdEstado = a.i_IdEstado,
                                         i_IdTipoDocumento = a.i_IdTipoDocumento ?? -1,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario ?? 0,
                                         d_CajaSaldo = a.d_CajaSaldo ?? 0,
                                         d_TotalGastos = a.d_TotalGastos ?? 0,
                                         d_TotalIngresos = a.d_TotalIngresos ?? 0,
                                         i_Eliminado = a.i_Eliminado.Value,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         i_IdMoneda = a.i_IdMoneda ?? -1,
                                         d_TipoCambio = a.d_TipoCambio ?? 0,
                                     }).FirstOrDefault();


                    return cajachica;
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "CajaChicaBL.ObtenerCabeceraCajaChica()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }



        }

        public string BuscarResponsable(int Usuario)
        {

            using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
            {
                var objSystemUserDto = (from a in dbContext.systemuser

                                        join p in dbContext.person on a.i_PersonId equals p.i_PersonId into p_join
                                        from p in p_join.DefaultIfEmpty()

                                        where a.i_SystemUserId == Usuario && a.i_IsDeleted == 0 && p.i_IsDeleted == 0

                                        select new systemuserDto
                                        {
                                            i_SystemUserId = a.i_SystemUserId,
                                            v_UserName = a.v_UserName,
                                            i_PersonId = a.i_PersonId,
                                            i_RoleId = a.i_RoleId,
                                            v_PersonName = p.v_FirstLastName.Trim() + " " + p.v_SecondLastName.Trim() + " " + p.v_FirstName.Trim(),
                                            i_UsuarioContable = a.i_UsuarioContable
                                        }
                   ).FirstOrDefault();

                if (objSystemUserDto != null)
                {
                    return objSystemUserDto.v_PersonName;
                }
                else
                {
                    return "";
                }
            }
        }

       
        public BindingList<cajachicadetalleDto> ObtenerDetallesCajaChica(ref OperationResult objOperationResult, string IdCajaChica)
        {

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var detalles = (from a in dbContext.cajachicadetalle
                                    join b in dbContext.conceptoscajachica on new { concep = a.i_IdConceptosCajaChica ?? 0, eliminado = 0 } equals new { concep = b.i_IdConceptosCajaChica, eliminado = b.i_Eliminado.Value } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.cliente on new { client = a.v_IdCliente, eliminado = 0 } equals new { client = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    where a.v_IdCajaChica == IdCajaChica && a.i_Eliminado == 0
                                    select new cajachicadetalleDto
                                    {
                                        v_IdCajaChica = a.v_IdCajaChica,
                                        v_IdCajaChicaDetalle = a.v_IdCajaChicaDetalle,
                                        i_Motivo = a.i_Motivo ?? 0,
                                        v_Usuario = a.v_Usuario,
                                        v_NombreConceptoCajaChica = a.v_NombreConceptoCajaChica,
                                        v_Observacion = a.v_Observacion,
                                        i_IdConceptosCajaChica = a.i_IdConceptosCajaChica,
                                        i_Eliminado = a.i_Eliminado,
                                        i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                        t_InsertaFecha = a.t_InsertaFecha,
                                        i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                        t_ActualizaFecha = a.t_ActualizaFecha,
                                        i_IdTipoDocumento = a.i_IdTipoDocumento,
                                        v_NroDocumento = string.IsNullOrEmpty(a.v_NroDocumento) ? "" : a.v_NroDocumento,
                                        d_Importe = a.d_Importe ?? 0,
                                        v_IdCliente = a.v_IdCliente,
                                        CodigoAnexo = c.v_CodCliente,
                                        i_RequiereAnexo = b.i_RequiereAnexo ?? 0,
                                        i_RequiereTipoDocumento = b.i_RequiereTipoDocumento ?? 0,
                                        i_RequiereNumeroDocumento = b.i_RequiereNumeroDocumento ?? 0,


                                    }).ToList();



                    var ordentrabajodetalleFinal = new BindingList<cajachicadetalleDto>(detalles.ToList());

                    objOperationResult.Success = 1;
                    return ordentrabajodetalleFinal;
                }

            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "CajaChicaBL.ObtenerDetallesCajaChica()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;



            }
        }


        public string InsertarCajaChica(ref OperationResult pobjOperationResult, cajachicaDto pobjDtoEntity,
                List<string> ClientSession, List<cajachicadetalleDto> pTemp_Insertar)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();

                        cajachica objEntitycajachica = pobjDtoEntity.ToEntity();

                        int SecuentialId = 0;
                        string newIdCajaChica = string.Empty;
                        string newIdcajachicadetalle = string.Empty;

                        #region Inserta Cabecera

                        objEntitycajachica.t_InsertaFecha = DateTime.Now;
                        objEntitycajachica.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitycajachica.i_Eliminado = 0;

                        var intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 108);
                        newIdCajaChica = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LY");
                        pobjDtoEntity.v_IdCajaChica = newIdCajaChica;
                        objEntitycajachica.v_IdCajaChica = newIdCajaChica;
                        dbContext.AddTocajachica(objEntitycajachica);
                        dbContext.SaveChanges();

                        #endregion

                        foreach (var cajachicadetalleDto in pTemp_Insertar)
                        {
                            var objEntityCajaChicaDetalle = cajachicadetalleDto.ToEntity();
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 109);
                            newIdcajachicadetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LP");
                            objEntityCajaChicaDetalle.v_IdCajaChicaDetalle = newIdcajachicadetalle;

                            objEntityCajaChicaDetalle.v_IdCajaChica = newIdCajaChica;
                            objEntityCajaChicaDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityCajaChicaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityCajaChicaDetalle.i_Eliminado = 0;

                            dbContext.AddTocajachicadetalle(objEntityCajaChicaDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                                "cajachicadetalle", newIdcajachicadetalle);


                        }
                        dbContext.SaveChanges();
                        if (pobjDtoEntity.i_IdEstado == 0) // Cierre caja
                        {

                            ProcesoInsertarTesoreria(ref pobjOperationResult, pobjDtoEntity);
                        }

                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "cajachica",
                            newIdCajaChica);
                        ts.Complete();
                        return newIdCajaChica;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CajaChicaBL.InsertarCajaChica()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }


        public void ProcesoInsertarTesoreria(ref OperationResult objOperationResult, cajachicaDto _objCajaChica, string[] IdRegistroEliminado = null)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    objOperationResult.Success = 1;
                    var _ListadoTesorerias = new List<KeyValueDTO>();
                    var ListaDetalles = dbContext.cajachicadetalle.Where(o => o.i_Eliminado == 0 && o.v_IdCajaChica == _objCajaChica.v_IdCajaChica).ToList();
                    tesoreriaDto _objTesoreria = new tesoreriaDto();
                    var Cuenta = _objTesoreriaBL.DevuelveCuentaCajaBanco(ref objOperationResult, _objCajaChica.i_IdTipoDocumento.Value);
                    if (objOperationResult.Success == 0) return;
                    //_objTesoreria.v_Periodo =_objCajaChica.v_Periodo ;
                    //_objTesoreria.v_Mes = _objCajaChica.v_Mes;
                    //var Correlativo = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult , ListaProcesos.Tesoreria, _objTesoreria.t_FechaRegistro.Value,_objCajaChica.t_FechaRegistro.Value, _objTesoreria.v_Correlativo, int.Parse(_objCajaChica.i_IdTipoDocumento.Value.ToString()));
                    _ListadoTesorerias = _objTesoreriaBL.ObtenerListadoTesorerias(ref objOperationResult, _objCajaChica.t_FechaRegistro.Value.Year.ToString(), _objCajaChica.t_FechaRegistro.Value.Month.ToString("00"), _objCajaChica.i_IdTipoDocumento.Value);
                    var _MaxMovimiento = _ListadoTesorerias.Any() ? int.Parse(_ListadoTesorerias[_ListadoTesorerias.Count() - 1].Value1) : 0;
                    _MaxMovimiento++;
                    if (IdRegistroEliminado != null && IdRegistroEliminado[0] != null && IdRegistroEliminado[1] != null &&
                              IdRegistroEliminado[2] != null)
                    {
                        _objTesoreria.v_Mes = _objCajaChica.t_FechaRegistro.Value.Year.ToString() !=
                                               IdRegistroEliminado[0] ||
                                               _objCajaChica.t_FechaRegistro.Value.Month.ToString("00") !=
                                               IdRegistroEliminado[1].Trim()
                            ? _objCajaChica.t_FechaRegistro.Value.Month.ToString("00")
                            : IdRegistroEliminado[1].Trim();
                        _objTesoreria.v_Periodo = _objCajaChica.t_FechaRegistro.Value.Year.ToString() !=
                                                   IdRegistroEliminado[0]
                            ? _objCajaChica.t_FechaRegistro.Value.Year.ToString()
                            : IdRegistroEliminado[0];
                        _objTesoreria.v_Correlativo = _objCajaChica.t_FechaRegistro.Value.Year.ToString() !=
                                                       IdRegistroEliminado[0] ||
                                                       _objCajaChica.t_FechaRegistro.Value.Month.ToString("00") !=
                                                       IdRegistroEliminado[1].Trim()
                            ? _MaxMovimiento.ToString("00000000")
                            : IdRegistroEliminado[2];
                    }
                    else
                    {
                        _objTesoreria.v_Mes = _objCajaChica.t_FechaRegistro.Value.Month.ToString("00");
                        _objTesoreria.v_Periodo = _objCajaChica.t_FechaRegistro.Value.Year.ToString();
                        _objTesoreria.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    }

                    if (objOperationResult.Success == 0) return;
                    // _objTesoreria.v_Correlativo = Correlativo;
                    _objTesoreria.i_IdTipoDocumento = _objCajaChica.i_IdTipoDocumento.Value;
                    while (!_objTesoreriaBL.ExisteNroRegistro(_objTesoreria.i_IdTipoDocumento.Value, _objTesoreria.v_Periodo, _objTesoreria.v_Mes, _objTesoreria.v_Correlativo))
                    {
                        _objTesoreria.v_Correlativo = (int.Parse(_objTesoreria.v_Correlativo) + 1).ToString("00000000");
                    }
                    _objTesoreria.i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Egreso;
                    _objTesoreria.i_IdMoneda = _objCajaChica.i_IdMoneda.Value;
                    _objTesoreria.t_FechaRegistro = _objCajaChica.t_FechaRegistro;
                    _objTesoreria.i_IdMedioPago = 1;
                    _objTesoreria.v_Nombre = "CAJA CHICA DEL DIA " + _objCajaChica.t_FechaRegistro.Value.Date.ToShortDateString();
                    _objTesoreria.v_Glosa = _objTesoreria.v_Nombre;
                    _objTesoreria.i_IdMedioPago = 1;
                    _objTesoreria.i_IdEstado = 1;

                    _objTesoreria.v_IdCobranza = _objCajaChica.v_IdCajaChica;
                    _objTesoreria.d_TipoCambio = _objCajaChica.d_TipoCambio;
                    _objTesoreria.v_NroCuentaCajaBanco = Cuenta.NroCuenta;
                    var Gastos = ListaDetalles.Where(o => o.i_Motivo == 0).ToList();// Recopilamos todas las salidas de la caja chica
                    decimal TipoCambio = _objTesoreria.d_TipoCambio.Value;
                    int Moneda = _objTesoreria.i_IdMoneda.Value;
                    var TesoreriaDetalles = (from a in Gastos

                                             join b in dbContext.conceptoscajachica on new { concepto = a.i_IdConceptosCajaChica.Value } equals new { concepto = b.i_IdConceptosCajaChica } into b_join
                                             from b in b_join.DefaultIfEmpty()

                                             select new tesoreriadetalleDto
                                             {
                                                 v_NroCuenta = b.v_NroCuenta,
                                                 d_Importe = Moneda == 1 ? a.d_Importe : a.d_Importe * TipoCambio,
                                                 d_Cambio = Moneda == 1 ? a.d_Importe / TipoCambio : a.d_Importe,
                                                 i_IdTipoDocumento = a.i_IdTipoDocumento ?? -1,
                                                 v_NroDocumento = string.IsNullOrEmpty(a.v_NroDocumento) ? "" : a.v_NroDocumento,
                                                 v_Naturaleza = "D",
                                                 v_IdCliente = string.IsNullOrEmpty(a.v_IdCliente) ? null : a.v_IdCliente,

                                             }).ToList().GroupBy(o => new { o.v_NroCuenta, o.i_IdTipoDocumento, o.v_NroDocumento, o.v_IdCliente }).Select(y =>
                        {
                            var primero = y.FirstOrDefault() != null ? y.FirstOrDefault() : new tesoreriadetalleDto();
                            return new tesoreriadetalleDto
                            {
                                d_Importe = y.Sum(o => o.d_Importe),
                                d_Cambio = y.Sum(o => o.d_Cambio),
                                v_Naturaleza = primero.v_Naturaleza,
                                i_IdTipoDocumento = primero.i_IdTipoDocumento,
                                v_NroDocumento = primero.v_NroDocumento,
                                v_NroCuenta = primero.v_NroCuenta,
                                v_IdCliente = primero.v_IdCliente,
                                t_Fecha = _objTesoreria.t_FechaRegistro,
                            };
                        }).ToList();

                    tesoreriadetalleDto _objTesoreriadetalle = new tesoreriadetalleDto();
                    var sumaImporte = TesoreriaDetalles.Sum(o => o.d_Importe);
                    var sumaCambio = TesoreriaDetalles.Sum(o => o.d_Cambio);
                    _objTesoreriadetalle.v_Naturaleza = "H";
                    _objTesoreriadetalle.d_Importe = Utils.Windows.DevuelveValorRedondeado(sumaImporte.Value, 2);
                    _objTesoreriadetalle.d_Cambio = Utils.Windows.DevuelveValorRedondeado(sumaCambio.Value, 2);
                    _objTesoreriadetalle.v_NroCuenta = Cuenta.NroCuenta;
                    _objTesoreriadetalle.i_IdTipoDocumento = -1;
                    _objTesoreriadetalle.v_NroDocumento = "";
                    _objTesoreriadetalle.t_Fecha = _objTesoreria.t_FechaRegistro;


                    TesoreriaDetalles.Add(_objTesoreriadetalle);
                    _objTesoreria.d_TotalDebe_Cambio = TesoreriaDetalles.Where(o => o.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    _objTesoreria.d_TotalDebe_Importe = TesoreriaDetalles.Where(o => o.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    _objTesoreria.d_TotalHaber_Cambio = TesoreriaDetalles.Where(o => o.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                    _objTesoreria.d_TotalHaber_Importe = TesoreriaDetalles.Where(o => o.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    _objTesoreria.d_Diferencia_Importe = (_objTesoreria.d_TotalDebe_Importe - _objTesoreria.d_TotalHaber_Importe);
                    _objTesoreria.d_Diferencia_Cambio = (_objTesoreria.d_TotalDebe_Cambio - _objTesoreria.d_TotalHaber_Cambio);
                    if (TesoreriaDetalles.Count() >= 2)
                    {
                        _objTesoreriaBL.Insertartesoreria(ref objOperationResult, _objTesoreria, Globals.ClientSession.GetAsList(), TesoreriaDetalles);
                    }

                    if (objOperationResult.Success == 0) return;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "CajaChicaBL.ProcesoInsertarTesoreria()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = objOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return;
            }

        }


        public void ActualizarCajaChica(ref OperationResult pobjOperationResult, cajachicaDto pobjDtoEntity,
             List<string> ClientSession, List<cajachicadetalleDto> pTemp_Insertar, List<cajachicadetalleDto> pTemp_Editar,
             List<cajachicadetalleDto> pTemp_Eliminar)
        {
            try
            {
                pobjOperationResult.Success = 1;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    cajachica objEntitycajachica = cajachicaAssembler.ToEntity(pobjDtoEntity);
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    string newIdCajaChicaDetalle = string.Empty;

                    #region Actualiza Cabecera

                    var intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (from a in dbContext.cajachica
                                           where a.v_IdCajaChica == pobjDtoEntity.v_IdCajaChica
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    cajachica objEntity = pobjDtoEntity.ToEntity();
                    dbContext.cajachica.ApplyCurrentValues(objEntity);

                    #endregion



                    #region Actualiza Detalle

                    foreach (cajachicadetalleDto cajachicadetalleDto in pTemp_Insertar)
                    {
                        cajachicadetalle objEntityCajaChicaDetalle =
                            cajachicadetalleAssembler.ToEntity(cajachicadetalleDto);
                        var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 109);
                        newIdCajaChicaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LP");
                        objEntityCajaChicaDetalle.v_IdCajaChicaDetalle = newIdCajaChicaDetalle;
                        objEntityCajaChicaDetalle.v_IdCajaChica = objEntitySource.v_IdCajaChica;
                        objEntityCajaChicaDetalle.t_InsertaFecha = DateTime.Now;

                        objEntityCajaChicaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityCajaChicaDetalle.i_Eliminado = 0;
                        dbContext.AddTocajachicadetalle(objEntityCajaChicaDetalle);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                              "cajachicadetalle", objEntityCajaChicaDetalle.v_IdCajaChicaDetalle);



                    }

                    foreach (cajachicadetalleDto cajachicadetalleDto in pTemp_Editar)
                    {

                        cajachicadetalle _objEntity = cajachicadetalleDto.ToEntity();


                        var query = (dbContext.cajachicadetalle.Where(
                            n => n.v_IdCajaChicaDetalle == cajachicadetalleDto.v_IdCajaChicaDetalle)).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.cajachicadetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                            "cajachicadetalle", query.v_IdCajaChicaDetalle);






                    }

                    foreach (cajachicadetalleDto cajachicadetalleDto in pTemp_Eliminar)
                    {

                        cajachicadetalle _objEntity = cajachicadetalleDto.ToEntity();
                        var query = (from n in dbContext.cajachicadetalle
                                     where n.v_IdCajaChicaDetalle == cajachicadetalleDto.v_IdCajaChicaDetalle
                                     select n).FirstOrDefault();

                        if (query != null && query.EntityState != EntityState.Deleted)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;




                            dbContext.cajachicadetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                "cajachicadetalle", query.v_IdCajaChicaDetalle);


                        }

                    }

                    #endregion

                    dbContext.SaveChanges();

                    if (pobjDtoEntity.i_IdEstado == 1) // AbrenCaja , hay que eliminar la tesoreria
                    {
                        EliminarTesoreria(ref pobjOperationResult, pobjDtoEntity.v_IdCajaChica);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    else
                    {

                        RegenerarTesoreria(ref  pobjOperationResult, pobjDtoEntity);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                        "cajachica", pobjDtoEntity.v_IdCajaChica);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CajaChicaBL.ActualizarCajaChica()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }


        public void RegenerarTesoreria(ref OperationResult pobjOperationResult, cajachicaDto _objCajaChica)
        {
            try
            {
                var reg = EliminarTesoreria(ref pobjOperationResult, _objCajaChica.v_IdCajaChica);
                if (pobjOperationResult.Success == 0) return;
                ProcesoInsertarTesoreria(ref pobjOperationResult, _objCajaChica, reg);
                if (pobjOperationResult.Success == 0) return;
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CajaChicaBL.RegenerarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }



        public List<ReporteDocumentoCajaChica> RepoorteDocumentoCajaChica(ref OperationResult objOperationResult, string IdCajaChica, DateTime FechaInicio, DateTime FechaFin ,bool Varias=false)
        {

            try
            {
                objOperationResult.Success = 1;

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    if (!Varias)
                    {
                        var CajaChica = (from a in dbContext.cajachica
                                         join b in dbContext.cajachicadetalle on new { d = a.v_IdCajaChica, eliminado = 0 } equals new { d = b.v_IdCajaChica, eliminado = b.i_Eliminado.Value } into b_join
                                         from b in b_join.DefaultIfEmpty()
                                         join c in dbContext.conceptoscajachica on new { c = b.i_IdConceptosCajaChica ?? 0, eliminado = 0 } equals new { c = c.i_IdConceptosCajaChica, eliminado = c.i_Eliminado.Value } into c_join
                                         from c in c_join.DefaultIfEmpty()
                                         join d in dbContext.documento on new { doc = a.i_IdTipoDocumento ?? -1, eliminado = 0 } equals new { doc = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                         from d in d_join.DefaultIfEmpty()

                                         join e in dbContext.documento on new { docdetalle = b.i_IdTipoDocumento ?? -1, eliminado = 0 } equals new { docdetalle = e.i_CodigoDocumento, eliminado = e.i_Eliminado.Value } into e_join
                                         from e in e_join.DefaultIfEmpty()

                                         join f in dbContext.cliente on new { cliente = b.v_IdCliente, eliminado = 0 } equals new { cliente = f.v_IdCliente, eliminado = f.i_Eliminado.Value } into f_join
                                         from f in f_join.DefaultIfEmpty()
                                         where a.v_IdCajaChica == IdCajaChica

                                         select new
                                         {
                                             TipoDocumento = d == null ? "" : d.v_Siglas,
                                             NumeroDocumento = a == null ? "" : a.v_Mes + " " + a.v_Correlativo + " - " + a.v_Periodo,
                                             UsuarioCreacion = a.i_InsertaIdUsuario ?? 0,
                                             Fecha = a.t_FechaRegistro.Value,
                                             iMotivo = b.i_Motivo ?? -1,
                                             DescripcionMotivo = c.v_NombreConceptoCajaChica,
                                             Usuario = b.v_Usuario,
                                             observacion = b.v_Observacion,
                                             TipoDocDetalle = e == null ? "" : e.v_Siglas,
                                             NumeroDocDetalle = b.v_NroDocumento,
                                             importe = b.d_Importe ?? 0,
                                             Anexo = f == null ? "" : (f.v_ApePaterno + " " + f.v_ApeMaterno + " " + f.v_PrimerNombre + " " + f.v_RazonSocial).Trim(),
                                             Estado = a.i_IdEstado == 1 ? "CAJA ABIERTA" : "CAJA CERRADA",
                                             TotalIngresos = a.d_TotalIngresos ?? 0,
                                             TotalSalidas = a.d_TotalGastos ?? 0,
                                             Saldo = a.d_CajaSaldo ?? 0,
                                             Llave =a.v_IdCajaChica ,

                                         }).ToList().Select(o => new ReporteDocumentoCajaChica
                                         {
                                             TipoDocmentoCajaChica = "CONTROL DE CAJA N° " + o.TipoDocumento + " " + o.NumeroDocumento,
                                             Resposnsable = "RESPONSABLE : " + BuscarResponsable(o.UsuarioCreacion),
                                             Fecha = "FECHA : " + o.Fecha.ToShortDateString(),
                                             Motivo = o.DescripcionMotivo,
                                             iMotivo = o.iMotivo,
                                             Usuario = o.Usuario,
                                             Observacion = o.observacion,
                                             TipoDocumentoDetalle = o.TipoDocDetalle + " " + o.NumeroDocDetalle,
                                             Importe = o.importe,
                                             Anexo = o.Anexo,
                                             Estado = o.Estado,
                                             TotalIngresos = o.TotalIngresos,
                                             TotalSalidas = o.TotalSalidas,
                                             Saldo = o.Saldo,
                                             Llave =o.Llave ,

                                         }).ToList();
                        return CajaChica.OrderByDescending(o => o.iMotivo).ToList();
                    }
                    else
                    {


                        var CajaChica = (from a in dbContext.cajachica
                                         join b in dbContext.cajachicadetalle on new { d = a.v_IdCajaChica, eliminado = 0 } equals new { d = b.v_IdCajaChica, eliminado = b.i_Eliminado.Value } into b_join
                                         from b in b_join.DefaultIfEmpty()
                                         join c in dbContext.conceptoscajachica on new { c = b.i_IdConceptosCajaChica ?? 0, eliminado = 0 } equals new { c = c.i_IdConceptosCajaChica, eliminado = c.i_Eliminado.Value } into c_join
                                         from c in c_join.DefaultIfEmpty()
                                         join d in dbContext.documento on new { doc = a.i_IdTipoDocumento ?? -1, eliminado = 0 } equals new { doc = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                         from d in d_join.DefaultIfEmpty()

                                         join e in dbContext.documento on new { docdetalle = b.i_IdTipoDocumento ?? -1, eliminado = 0 } equals new { docdetalle = e.i_CodigoDocumento, eliminado = e.i_Eliminado.Value } into e_join
                                         from e in e_join.DefaultIfEmpty()

                                         join f in dbContext.cliente on new { cliente = b.v_IdCliente, eliminado = 0 } equals new { cliente = f.v_IdCliente, eliminado = f.i_Eliminado.Value } into f_join
                                         from f in f_join.DefaultIfEmpty()
                                         where a.t_FechaRegistro >=FechaInicio && a.t_FechaRegistro <=FechaFin  && a.i_Eliminado ==0

                                         select new
                                         {
                                             TipoDocumento = d == null ? "" : d.v_Siglas,
                                             NumeroDocumento = a == null ? "" : a.v_Mes + " " + a.v_Correlativo + " - " + a.v_Periodo,
                                             UsuarioCreacion = a.i_InsertaIdUsuario ?? 0,
                                             Fecha = a.t_FechaRegistro.Value,
                                             iMotivo = b.i_Motivo ?? -1,
                                             DescripcionMotivo = c.v_NombreConceptoCajaChica,
                                             Usuario = b.v_Usuario,
                                             observacion = b.v_Observacion,
                                             TipoDocDetalle = e == null ? "" : e.v_Siglas,
                                             NumeroDocDetalle = b.v_NroDocumento,
                                             importe = b.d_Importe ?? 0,
                                             Anexo = f == null ? "" : (f.v_ApePaterno + " " + f.v_ApeMaterno + " " + f.v_PrimerNombre + " " + f.v_RazonSocial).Trim(),
                                             Estado = a.i_IdEstado == 1 ? "CAJA ABIERTA" : "CAJA CERRADA",
                                             TotalIngresos = a.d_TotalIngresos ?? 0,
                                             TotalSalidas = a.d_TotalGastos ?? 0,
                                             Saldo = a.d_CajaSaldo ?? 0,
                                             LLave =a.v_IdCajaChica ,

                                         }).ToList().Select(o => new ReporteDocumentoCajaChica
                                         {
                                             TipoDocmentoCajaChica = "CONTROL DE CAJA N° " + o.TipoDocumento + " " + o.NumeroDocumento,
                                             Resposnsable = "RESPONSABLE : " + BuscarResponsable(o.UsuarioCreacion),
                                             Fecha = "FECHA : " + o.Fecha.ToShortDateString(),
                                             Motivo = o.DescripcionMotivo,
                                             iMotivo = o.iMotivo,
                                             Usuario = o.Usuario,
                                             Observacion = o.observacion,
                                             TipoDocumentoDetalle = o.TipoDocDetalle + " " + o.NumeroDocDetalle,
                                             Importe = o.importe,
                                             Anexo = o.Anexo,
                                             Estado = o.Estado,
                                             TotalIngresos = o.TotalIngresos,
                                             TotalSalidas = o.TotalSalidas,
                                             Saldo = o.Saldo,
                                             Llave =o.LLave ,

                                         }).ToList();


                        return CajaChica.OrderByDescending(o => o.iMotivo).ToList();
                    
                    
                    
                    
                    
                    }
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "CajaChicaBL.ReporteDocumentoCajaChica()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = objOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;


            }



        }

        private static string[] EliminarTesoreria(ref OperationResult pobjOperationResult, string strIdCajaChica)
        {
            try
            {
                var IdRegistroEliminado = new TesoreriaBL().EliminarTesoreriaXDocRef(ref pobjOperationResult, strIdCajaChica, Globals.ClientSession.GetAsList());
                if (pobjOperationResult.Success == 0) return null;
                pobjOperationResult.Success = 1;
                return IdRegistroEliminado;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CajaChicaBL.EliminarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }




        public void EliminarCajaChica(ref OperationResult pobjOperationResult, string pstrIdCajaChica,
            List<string> clientSession)
        {
            var objOperationResult = new OperationResult();

            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();

                    #region Elimina Cabecera

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.cajachica
                                           where a.v_IdCajaChica == pstrIdCajaChica && a.i_Eliminado == 0
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    #endregion

                    #region Elimina Detalles

                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesCajaChica = (from a in dbContext.cajachicadetalle
                                                            where a.v_IdCajaChica == pstrIdCajaChica
                                                            select a).ToList();

                    foreach (var CajaChicaDetalle in objEntitySourceDetallesCajaChica)
                    {
                        CajaChicaDetalle.t_ActualizaFecha = DateTime.Now;
                        CajaChicaDetalle.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                        CajaChicaDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                            "CajaChicaDetalle", CajaChicaDetalle.v_IdCajaChicaDetalle);

                    }

                    #endregion

                    #region EliminarTesoreria
                    EliminarTesoreria(ref pobjOperationResult, objEntitySource.v_IdCajaChica);
                    if (pobjOperationResult.Success == 0) return;
                    #endregion
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "CajaChica",
                        pstrIdCajaChica);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        public List<cajachicaDto> ListarBusquedaCajaChica(ref OperationResult objOperationResut, DateTime FechaInicio, DateTime FechaFin,int SystemUserId)
        {

            try
            {
                objOperationResut.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Listacajachica = (from a in dbContext.cajachica
                                          join b in dbContext.documento on new { eliminado = 0, doc = a.i_IdTipoDocumento.Value } equals new { eliminado = b.i_Eliminado.Value, doc = b.i_CodigoDocumento } into b_join
                                          from b in b_join.DefaultIfEmpty()


                                          join J2 in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value }
                                          equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                          from J2 in J2_join.DefaultIfEmpty()

                                          join J3 in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value }
                                              equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                          from J3 in J3_join.DefaultIfEmpty()

                                          join c in dbContext.vendedor on a.i_InsertaIdUsuario equals c.i_SystemUser

                                          where a.i_Eliminado == 0 && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin && a.i_InsertaIdUsuario == SystemUserId

                                          select new

                                          {
                                              TipoDocumento = b.v_Siglas,
                                              NroRegistro = a.v_Mes + " - " + a.v_Correlativo,
                                              v_UsuarioCreacion = J3.v_UserName,
                                              v_UsuarioModificacion = J2.v_UserName,
                                              t_InsertaFecha = a.t_InsertaFecha,
                                              t_ActualizaFecha = a.t_ActualizaFecha,
                                              Estado = a.i_IdEstado == 1 ? "ABIERTA" : "CERRADA",
                                              t_FechaRegistro = a.t_FechaRegistro,
                                              Responsable = J3.v_UserName,
                                              i_IdEstado = a.i_IdEstado,
                                              v_IdCajaChica = a.v_IdCajaChica,
                                              d_TotalIngresos = a.d_TotalIngresos ?? 0,
                                              d_TotalGastos = a.d_TotalGastos ?? 0,
                                              d_CajaSaldo = a.d_CajaSaldo ?? 0,
                                              i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                              v_Perido =a.v_Periodo ,
                                              v_IdVendedor = c.v_IdVendedor

                                          }).ToList().Select(o => new cajachicaDto
                                          {
                                              TipoDocumento = o.TipoDocumento,
                                              NroRegistro = o.NroRegistro,
                                              v_UsuarioCreacion = o.v_UsuarioCreacion,
                                              v_UsuarioModificacion = o.v_UsuarioModificacion,
                                              t_InsertaFecha = o.t_InsertaFecha,
                                              t_ActualizaFecha = o.t_ActualizaFecha,
                                              Estado = o.Estado,
                                              t_FechaRegistro = o.t_FechaRegistro,
                                              Responsable = BuscarResponsable(o.i_InsertaIdUsuario ?? 0),
                                              i_IdEstado = o.i_IdEstado,
                                              v_IdCajaChica = o.v_IdCajaChica,
                                              d_TotalIngresos = o.d_TotalIngresos,
                                              d_TotalGastos = o.d_TotalGastos,
                                              d_CajaSaldo = o.d_CajaSaldo,
                                              v_Periodo =o.v_Perido ,
                                              v_IdVendedor = o.v_IdVendedor
                                          }).ToList(); ;


                    return Listacajachica;
                }
            }
            catch (Exception ex)
            {
                objOperationResut.Success = 0;
                objOperationResut.AdditionalInformation = "CajaChicaBL.ListarBusquedaCajaChica()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResut.ErrorMessage = ex.Message;
                objOperationResut.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResut);
                return null;
            }

        }


        public decimal ObtenerSaldoUltimaCajaChica(List<string> ClientSession, int TipoDocumento)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                // Solo se traslada el saldo de las cajas cerradas  que hayan sido creadas por ese usuario o responsable
                int UsuarioCreacion = Int32.Parse(ClientSession[2]);
                var CajasExistentes = dbContext.cajachica.Where(o => o.i_Eliminado == 0 && o.i_IdEstado == 0 && o.i_InsertaIdUsuario == UsuarioCreacion && o.i_IdTipoDocumento == TipoDocumento).ToList();
                var MaxRegistro = CajasExistentes.Count() > 0 ? dbContext.cajachica.Where(o => o.i_Eliminado == 0 && o.i_IdEstado == 0 && o.i_InsertaIdUsuario == UsuarioCreacion && o.i_IdTipoDocumento == TipoDocumento).ToList().OrderBy(o => o.t_FechaRegistro).ThenBy(o => o.i_IdTipoDocumento).ThenBy(o => o.v_Mes + " " + o.v_Correlativo).LastOrDefault().v_IdCajaChica : null; //.Select(o => o.t_FechaRegistro).Max(); // Saco la maxima fecha de las cajas cerrdadas

                var cajachica = (from a in dbContext.cajachica

                                 where a.i_Eliminado == 0 && a.v_IdCajaChica == MaxRegistro
                                 select a).FirstOrDefault();

                return cajachica != null ? cajachica.d_CajaSaldo.Value : 0;
            }

        }

        public bool VerificarSiUsuarioTieneCajaAbiertaFecha(List<string> ClientSession, DateTime FechaIngresada)
        {
            int UsuarioCreacion = Int32.Parse(ClientSession[2]);
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var CajaAbiertaFecha = dbContext.cajachica.Where(o => o.i_Eliminado == 0 && o.t_FechaRegistro <= FechaIngresada && o.i_IdEstado == 1 && o.i_InsertaIdUsuario == UsuarioCreacion).ToList();
                return CajaAbiertaFecha.Any() ? true : false;

            }
        }


        public bool ExisteNroRegistro(int TipoDoc, string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var replicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.cajachica
                            where
                                n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo &&
                                n.i_IdTipoDocumento == TipoDoc
                                && n.v_IdCajaChica.Substring(0, 1) == replicationID
                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            return false;
        }
        public bool VerificarSiUsuarioTieneCajaAbiertaFechaEditado(List<string> ClientSession, DateTime FechaIngresada, string IdCajaChica)
        {
            int UsuarioCreacion = Int32.Parse(ClientSession[2]);
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var CajaAbiertaFecha = dbContext.cajachica.Where(o => o.i_Eliminado == 0 && o.t_FechaRegistro <= FechaIngresada && o.i_IdEstado == 1 && o.i_InsertaIdUsuario == UsuarioCreacion && o.v_IdCajaChica != IdCajaChica).ToList();
                return CajaAbiertaFecha.Any() ? true : false;

            }
        }



        public bool VerificarSiUsuarioTieneCajaAbiertaFechaPosterior(List<string> ClientSession, DateTime FechaIngresada, string IdCajaChica)
        {
            int UsuarioCreacion = Int32.Parse(ClientSession[2]);
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var CajaAbiertaFecha = dbContext.cajachica.Where(o => o.i_Eliminado == 0 && o.t_FechaRegistro >= FechaIngresada && o.i_IdEstado == 1 && o.i_InsertaIdUsuario == UsuarioCreacion && o.v_IdCajaChica != IdCajaChica).ToList();
                return CajaAbiertaFecha.Any() ? true : false;

            }
        }

    }
}
