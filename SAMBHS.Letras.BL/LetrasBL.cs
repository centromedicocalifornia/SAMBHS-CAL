using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.CommonWIN.BL;
using System.Transactions;
using SAMBHS.Tesoreria.BL;
using System.ComponentModel;
using SAMBHS.Cobranza.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BL;
namespace SAMBHS.Letras.BL
{
    /// <summary>
    /// Lógica del negocio para las Letras Por Cobrar y Pagar.
    /// EQC Set-2015
    /// </summary>
    public class LetrasBL
    {
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();

        #region Mantenimiento de Letras Por Cobrar y Pagar

        public List<letrasmantenimientoDto> ObtenerMantenimientosBandeja(ref OperationResult pobjOperationResult,
            int pintPeriodo, string pstrIdCliente, TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Cobranza)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Consulta = (from n in dbContext.letrasmantenimiento
                                    join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                        equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                        equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()
                                    where n.t_FechaRegistro.Value.Year == pintPeriodo && n.i_Eliminado == 0
                                    select new letrasmantenimientoDto
                                    {
                                        v_IdLetrasMantenimiento = n.v_IdLetrasMantenimiento,
                                        NroRegistro = n.v_Periodo + "-" + n.v_Correlativo,
                                        NombreCliente =
                                            (n.cliente.v_ApePaterno + " " + n.cliente.v_ApeMaterno + " " + n.cliente.v_PrimerNombre +
                                             " " + n.cliente.v_RazonSocial).Trim(),
                                        t_FechaRegistro = n.t_FechaRegistro,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        v_UsuarioCreacion = J3.v_UserName,
                                        v_UsuarioModificacion = J2.v_UserName,
                                        v_IdCliente = n.v_IdCliente,
                                        i_IdTipoMantenimiento = n.i_IdTipoMantenimiento
                                    }).ToList();

                    if (!string.IsNullOrEmpty(pstrIdCliente))
                    {
                        Consulta = Consulta.Where(p => p.v_IdCliente == pstrIdCliente).ToList();
                    }

                    Consulta = Consulta.Where(p => p.i_IdTipoMantenimiento == (int)oTipoMantenimiento).ToList();

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerMantenimientosBandeja()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public letrasmantenimientoDto ObtenerCabeceraMantenimiento(ref OperationResult pobjOperationResult,
            string pstrIdLetrasMantenimiento)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Consulta = (from n in dbContext.letrasmantenimiento
                                    where n.v_IdLetrasMantenimiento == pstrIdLetrasMantenimiento && n.i_Eliminado == 0
                                    select new letrasmantenimientoDto
                                    {
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdCliente,
                                        v_IdLetrasMantenimiento = n.v_IdLetrasMantenimiento,
                                        NombreCliente =
                                            (n.cliente.v_ApePaterno + " " + n.cliente.v_ApeMaterno + " " + n.cliente.v_PrimerNombre +
                                             " " + n.cliente.v_RazonSocial).Trim(),
                                        v_Periodo = n.v_Periodo,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaRegistro = n.t_FechaRegistro,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Eliminado = n.i_Eliminado,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_IdTipoMantenimiento = n.i_IdTipoMantenimiento
                                    }
                        ).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerCabeceraMantenimiento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<letrasmantenimientodetalleDto> ObtenerDetalleMantenimiento(
            ref OperationResult pobjOperationResult, string pstrIdLetrasMantenimiento,
            TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Cobranza)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    switch (oTipoMantenimiento)
                    {
                        case TipoMantenimiento.Cobranza:
                            var Consulta = (from n in dbContext.letrasmantenimientodetalle
                                            join J1 in dbContext.letrasdetalle on n.v_IdLetrasDetalle equals J1.v_IdLetrasDetalle
                                                into J1_join
                                            from J1 in J1_join.DefaultIfEmpty()
                                            where n.v_IdLetrasMantenimiento == pstrIdLetrasMantenimiento && n.i_Eliminado == 0
                                            select new letrasmantenimientodetalleDto
                                            {
                                                v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                                v_IdLetrasMantenimiento = n.v_IdLetrasMantenimiento,
                                                v_IdLetrasMantenimientoDetalle = n.v_IdLetrasMantenimientoDetalle,
                                                v_NroUnico = n.v_NroUnico,
                                                i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                                i_Eliminado = n.i_Eliminado,
                                                i_IdEstado = n.i_IdEstado,
                                                i_IdUbicacion = n.i_IdUbicacion,
                                                i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                                t_ActualizaFecha = n.t_ActualizaFecha,
                                                t_FechaCancelacion = n.t_FechaCancelacion,
                                                t_InsertaFecha = n.t_InsertaFecha,
                                                FechaEmision = J1.t_FechaEmision ?? DateTime.Now,
                                                FechaVencimiento = J1.t_FechaVencimiento ?? DateTime.Now,
                                                Importe = J1.d_Importe ?? 0,
                                                Moneda = (J1.i_IdMoneda ?? 0) == 1 ? "SOLES" : "DÓLARES",
                                                NroLetra = J1.v_Serie + "-" + J1.v_Correlativo,
                                                d_GastosAdministrativos = n.d_GastosAdministrativos ?? 0
                                            }
                                ).ToList();

                            pobjOperationResult.Success = 1;
                            return new BindingList<letrasmantenimientodetalleDto>(Consulta);

                        case TipoMantenimiento.Pago:
                            var _Consulta = (from n in dbContext.letrasmantenimientodetalle
                                             join J1 in dbContext.letraspagardetalle on n.v_IdLetrasPagarDetalle equals
                                                 J1.v_IdLetrasPagarDetalle into J1_join
                                             from J1 in J1_join.DefaultIfEmpty()
                                             where n.v_IdLetrasMantenimiento == pstrIdLetrasMantenimiento && n.i_Eliminado == 0
                                             select new letrasmantenimientodetalleDto
                                             {
                                                 v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                                 v_IdLetrasMantenimiento = n.v_IdLetrasMantenimiento,
                                                 v_IdLetrasMantenimientoDetalle = n.v_IdLetrasMantenimientoDetalle,
                                                 v_NroUnico = n.v_NroUnico,
                                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                                 i_Eliminado = n.i_Eliminado,
                                                 i_IdEstado = n.i_IdEstado,
                                                 i_IdUbicacion = n.i_IdUbicacion,
                                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                                 t_FechaCancelacion = n.t_FechaCancelacion,
                                                 t_InsertaFecha = n.t_InsertaFecha,
                                                 FechaEmision = J1.t_FechaEmision ?? DateTime.Now,
                                                 FechaVencimiento = J1.t_FechaVencimiento ?? DateTime.Now,
                                                 Importe = J1.d_Importe ?? 0,
                                                 Moneda = (J1.i_IdMoneda ?? 0) == 1 ? "SOLES" : "DÓLARES",
                                                 NroLetra = J1.v_Serie + "-" + J1.v_Correlativo
                                             }
                                ).ToList();

                            pobjOperationResult.Success = 1;
                            return new BindingList<letrasmantenimientodetalleDto>(_Consulta);

                        default:
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerDetalleMantenimiento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<letrasdetalleDto> ObtenerLetrasConsutaPorCliente(ref OperationResult pobjOperationResult,
            string pstrIdCliente)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                    var Consulta = (from n in dbContext.letrasdetalle
                                    where n.v_IdCliente == pstrIdCliente && n.i_Eliminado == 0 && n.i_Pagada != 1
                                    select new
                                    {
                                        NroDocLetra = n.v_Serie + "-" + n.v_Correlativo,
                                        d_Importe = n.d_Importe.Value,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdCliente,
                                        v_IdLetras = n.v_IdLetras,
                                        v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                        v_IdLetrasRenovacion = n.v_IdLetrasRenovacion,
                                        v_Serie = n.v_Serie,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Eliminado = n.i_Eliminado,
                                        i_IdTipoDocumento = n.i_IdTipoDocumento,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_Pagada = n.i_Pagada,
                                        i_TotalDias = n.i_TotalDias,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaEmision = n.t_FechaEmision,
                                        t_FechaVencimiento = n.t_FechaVencimiento,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        Moneda = n.i_IdMoneda.Value == 1 ? "SOLES" : "DÓLARES"
                                    }
                        ).ToList().Select(n =>
                        {
                            var ubicacion = ObtenerUltimaUbicacionLetra(n.v_IdLetrasDetalle, tempDs);
                            return new letrasdetalleDto
                            {
                                NroDocLetra = n.NroDocLetra,
                                d_Importe = n.d_Importe,
                                v_Correlativo = n.v_Correlativo,
                                v_IdCliente = n.v_IdCliente,
                                v_IdLetras = n.v_IdLetras,
                                v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                v_IdLetrasRenovacion = n.v_IdLetrasRenovacion,
                                v_Serie = n.v_Serie,
                                i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                i_Eliminado = n.i_Eliminado,
                                i_IdTipoDocumento = n.i_IdTipoDocumento,
                                i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                i_Pagada = n.i_Pagada,
                                i_TotalDias = n.i_TotalDias,
                                t_ActualizaFecha = n.t_ActualizaFecha,
                                t_FechaEmision = n.t_FechaEmision,
                                t_FechaVencimiento = n.t_FechaVencimiento,
                                t_InsertaFecha = n.t_InsertaFecha,
                                Moneda = n.Moneda,
                                Estado = ubicacion.Estado,
                                Ubicacion = ubicacion.Ubicacion,
                                NroUnico = ubicacion.v_NroUnico
                            };
                        }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasConsutaPorCliente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<letraspagardetalleDto> ObtenerLetrasConsutaPorProveedor(ref OperationResult pobjOperationResult,
            string pstrIdProveedor)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                    var Consulta = (from n in dbContext.letraspagardetalle
                                    where n.v_IdProveedor == pstrIdProveedor && n.i_Eliminado == 0 && n.i_Pagada != 1
                                    select new
                                    {
                                        NroDocLetra = n.v_Serie + "-" + n.v_Correlativo,
                                        d_Importe = n.d_Importe.Value,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdProveedor,
                                        v_IdLetras = n.v_IdLetrasPagar,
                                        v_IdLetrasDetalle = n.v_IdLetrasPagarDetalle,
                                        v_Serie = n.v_Serie,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Eliminado = n.i_Eliminado,
                                        i_IdTipoDocumento = n.i_IdTipoDocumento,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_Pagada = n.i_Pagada,
                                        i_TotalDias = n.i_TotalDias,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaEmision = n.t_FechaEmision,
                                        t_FechaVencimiento = n.t_FechaVencimiento,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        Moneda = n.i_IdMoneda.Value == 1 ? "SOLES" : "DÓLARES"
                                    }
                        ).ToList().Select(n =>
                        {
                            var ubicacion = ObtenerUltimaUbicacionLetra(n.v_IdLetrasDetalle, tempDs,
                                TipoMantenimiento.Pago);
                            return new letraspagardetalleDto
                            {
                                NroDocLetra = n.NroDocLetra,
                                d_Importe = n.d_Importe,
                                v_Correlativo = n.v_Correlativo,
                                v_IdProveedor = n.v_IdCliente,
                                v_IdLetrasPagar = n.v_IdLetras,
                                v_IdLetrasPagarDetalle = n.v_IdLetrasDetalle,
                                v_Serie = n.v_Serie,
                                i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                i_Eliminado = n.i_Eliminado,
                                i_IdTipoDocumento = n.i_IdTipoDocumento,
                                i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                i_Pagada = n.i_Pagada,
                                i_TotalDias = n.i_TotalDias,
                                t_ActualizaFecha = n.t_ActualizaFecha,
                                t_FechaEmision = n.t_FechaEmision,
                                t_FechaVencimiento = n.t_FechaVencimiento,
                                t_InsertaFecha = n.t_InsertaFecha,
                                Moneda = n.Moneda,
                                Estado = ubicacion.Estado,
                                Ubicacion = ubicacion.Ubicacion,
                                NroUnico = ubicacion.v_NroUnico
                            };
                        }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasConsutaPorCliente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string InsertarMantenimientoLetras(ref OperationResult pobjOperationResult,
            letrasmantenimientoDto pobjletrasmantenimientoDto, List<letrasmantenimientodetalleDto> pTemp_Insertar,
            List<string> ClientSession, TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Cobranza)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBl = new SecuentialBL();
                        var objEntityLetras = pobjletrasmantenimientoDto.ToEntity();

                        int SecuentialId = 0;
                        string newIdLetras = string.Empty;
                        string newIdLetrasDetalle = string.Empty;

                        #region Inserta Cabecera

                        objEntityLetras.t_InsertaFecha = DateTime.Now;
                        objEntityLetras.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityLetras.i_Eliminado = 0;
                        objEntityLetras.i_IdTipoMantenimiento = (int)oTipoMantenimiento;

                        var intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 77);
                        newIdLetras = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LM");
                        objEntityLetras.v_IdLetrasMantenimiento = newIdLetras;
                        dbContext.AddToletrasmantenimiento(objEntityLetras);

                        #endregion

                        #region Inserta Detalles

                        foreach (var Canje in pTemp_Insertar)
                        {
                            letrasmantenimientodetalle _letrasmantenimientodetalle = new letrasmantenimientodetalle();
                            _letrasmantenimientodetalle = Canje.ToEntity();
                            SecuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 78);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LN");
                            _letrasmantenimientodetalle.v_IdLetrasMantenimientoDetalle = newIdLetrasDetalle;
                            _letrasmantenimientodetalle.v_IdLetrasMantenimiento = newIdLetras;
                            _letrasmantenimientodetalle.t_InsertaFecha = DateTime.Now;
                            _letrasmantenimientodetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrasmantenimientodetalle.i_Eliminado = 0;
                            dbContext.AddToletrasmantenimientodetalle(_letrasmantenimientodetalle);
                        }

                        #endregion

                        dbContext.SaveChanges();

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return newIdLetras;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.InsertarMantenimientoLetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string ActualizarMantenimientoLetras(ref OperationResult pobjOperationResult,
            letrasmantenimientoDto pobjletrasmantenimientoDto, List<letrasmantenimientodetalleDto> pTemp_Insertar,
            List<letrasmantenimientodetalleDto> pTemp_Modificar, List<letrasmantenimientodetalleDto> pTemp_Eliminar,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        string newIdLetrasDetalle = string.Empty;
                        var intNodeId = int.Parse(ClientSession[0]);

                        #region Actualiza Cabecera

                        var EntidadOriginal = dbContext.letrasmantenimiento
                            .FirstOrDefault(p => p.v_IdLetrasMantenimiento == pobjletrasmantenimientoDto.v_IdLetrasMantenimiento);

                        letrasmantenimiento objEntidad = new letrasmantenimiento();
                        objEntidad = letrasmantenimientoAssembler.ToEntity(pobjletrasmantenimientoDto);
                        objEntidad.t_ActualizaFecha = DateTime.Now;
                        objEntidad.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.letrasmantenimiento.ApplyCurrentValues(objEntidad);

                        #endregion

                        #region Inserta Detalles

                        foreach (var Canje in pTemp_Insertar)
                        {
                            letrasmantenimientodetalle _letrasmantenimientodetalle = new letrasmantenimientodetalle();
                            _letrasmantenimientodetalle = letrasmantenimientodetalleAssembler.ToEntity(Canje);
                            var SecuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, 78);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LN");
                            _letrasmantenimientodetalle.v_IdLetrasMantenimientoDetalle = newIdLetrasDetalle;
                            _letrasmantenimientodetalle.v_IdLetrasMantenimiento = objEntidad.v_IdLetrasMantenimiento;
                            _letrasmantenimientodetalle.t_InsertaFecha = DateTime.Now;
                            _letrasmantenimientodetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrasmantenimientodetalle.i_Eliminado = 0;
                            dbContext.AddToletrasmantenimientodetalle(_letrasmantenimientodetalle);
                        }

                        #endregion

                        #region Modifica Detalles

                        foreach (var Canje in pTemp_Modificar)
                        {
                            var EntidadDetalleOriginal = dbContext.letrasmantenimientodetalle
                                .FirstOrDefault(p => p.v_IdLetrasMantenimientoDetalle == Canje.v_IdLetrasMantenimientoDetalle);

                            var _letrasmantenimientodetalle = new letrasmantenimientodetalle();
                            _letrasmantenimientodetalle = Canje.ToEntity();
                            _letrasmantenimientodetalle.t_ActualizaFecha = DateTime.Now;
                            _letrasmantenimientodetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.letrasmantenimientodetalle.ApplyCurrentValues(_letrasmantenimientodetalle);
                        }

                        #endregion

                        #region Elimina Detalles

                        foreach (var Canje in pTemp_Eliminar)
                        {
                            var EntidadDetalleOriginal = dbContext.letrasmantenimientodetalle
                                .Where(p => p.v_IdLetrasMantenimientoDetalle == Canje.v_IdLetrasMantenimientoDetalle)
                                .FirstOrDefault();

                            letrasmantenimientodetalle _letrasmantenimientodetalle = new letrasmantenimientodetalle();
                            _letrasmantenimientodetalle = letrasmantenimientodetalleAssembler.ToEntity(Canje);
                            _letrasmantenimientodetalle.t_ActualizaFecha = DateTime.Now;
                            _letrasmantenimientodetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrasmantenimientodetalle.i_Eliminado = 1;
                            dbContext.letrasmantenimientodetalle.ApplyCurrentValues(_letrasmantenimientodetalle);
                        }

                        #endregion

                        dbContext.SaveChanges();

                        pobjOperationResult.Success = 1;
                        ts.Complete();

                        return objEntidad.v_IdLetrasMantenimiento;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ActualizarMantenimientoLetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void EliminaMantenimientoLetras(ref OperationResult pobjOperationResult, string pstrIdMantenimientoLetras,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objEntidadOriginal = dbContext.letrasmantenimiento
                            .FirstOrDefault(p => p.v_IdLetrasMantenimiento == pstrIdMantenimientoLetras);

                        objEntidadOriginal.t_ActualizaFecha = DateTime.Now;
                        objEntidadOriginal.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntidadOriginal.i_Eliminado = 1;
                        dbContext.letrasmantenimiento.ApplyCurrentValues(objEntidadOriginal);

                        foreach (var Detalle in objEntidadOriginal.letrasmantenimientodetalle)
                        {
                            Detalle.t_ActualizaFecha = DateTime.Now;
                            Detalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            Detalle.i_Eliminado = 1;
                            dbContext.letrasmantenimientodetalle.ApplyCurrentValues(Detalle);
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
                pobjOperationResult.AdditionalInformation = "LetrasBL.EliminaMantenimientoLetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static letrasmantenimientodetalleDto ObtenerUltimaUbicacionLetra(string pstrIdLetraDetalle, IEnumerable<letrasmantenimientodetalle> ptempDs,
            TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Cobranza)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = new letrasmantenimientodetalleDto();
                    var Consulta = new List<letrasmantenimientodetalle>();

                    switch (oTipoMantenimiento)
                    {
                        case TipoMantenimiento.Cobranza:
                            Consulta =
                                ptempDs.Where(
                                    p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                            break;

                        case TipoMantenimiento.Pago:
                            Consulta =
                                ptempDs.Where(
                                    p => p.v_IdLetrasPagarDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                            break;
                    }

                    if (Consulta.Count > 0)
                    {
                        Consulta = Consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();

                        var ultimoRegistro = Consulta.Last();

                        var documento = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == ultimoRegistro.i_IdUbicacion);
                        var estado = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 110 && p.i_ItemId == ultimoRegistro.i_IdEstado);

                        Result = ultimoRegistro.ToDTO();
                        Result.Ubicacion = documento != null ? documento.v_Nombre : "OFICINA";
                        Result.Estado = estado != null ? estado.v_Value1 : "COBRANZA";
                    }
                    else
                    {
                        Result.Ubicacion = "OFICINA";
                        Result.Estado = "COBRANZA";
                    }

                    return Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static letrasmantenimientodetalleDto ObtenerUltimaUbicacionLetra(string pstrIdLetraDetalle,
            TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Cobranza)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = new letrasmantenimientodetalleDto();
                    var Consulta = new List<letrasmantenimientodetalle>();
                    switch (oTipoMantenimiento)
                    {
                        case TipoMantenimiento.Cobranza:
                            Consulta =
                                dbContext.letrasmantenimientodetalle.Where(
                                    p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                            break;

                        case TipoMantenimiento.Pago:
                            Consulta =
                                dbContext.letrasmantenimientodetalle.Where(
                                    p => p.v_IdLetrasPagarDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                            break;
                    }

                    if (Consulta.Count > 0)
                    {
                        Consulta = Consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();

                        var ultimoRegistro = Consulta.Last();

                        var documento = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == ultimoRegistro.i_IdUbicacion);
                        var estado = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 110 && p.i_ItemId == ultimoRegistro.i_IdEstado);

                        Result = ultimoRegistro.ToDTO();
                        Result.Ubicacion = documento != null ? documento.v_Nombre : "OFICINA";
                        Result.Estado = estado != null ? estado.v_Value1 : "COBRANZA";
                    }
                    else
                    {
                        Result.Ubicacion = "OFICINA";
                        Result.Estado = "COBRANZA";
                        Result.i_IdUbicacion = -1;
                        Result.i_IdEstado = -1;
                    }

                    return Result;
                }
            }
            catch (Exception)
            {
                return new letrasmantenimientodetalleDto();
            }
        }

        public string ObtenerSoloLetrasUbicacion(string UbicacionCompleto)
        {
            string cadena = "";
            for (int i = 0; i < UbicacionCompleto.Length; i++)
            {
                char d = Convert.ToChar(UbicacionCompleto.Substring(i, 1));
                {

                    if (!Char.IsDigit(d) && Convert.ToString(d) != "-")
                    {
                        cadena = cadena + d;

                    }

                }

            }

            return cadena;


        }

        public string ObtenerSoloLetrasPagarUbicacion(string UbicacionCompleto)
        {
            string cadena = "";
            for (int i = 0; i < UbicacionCompleto.Length; i++)
            {
                char d = Convert.ToChar(UbicacionCompleto.Substring(i, 1));
                {

                    if (!Char.IsDigit(d) && Convert.ToString(d) != "-")
                    {
                        cadena = cadena + d;

                    }

                }

            }

            return cadena;


        }

        private static letrasmantenimientodetalleDto ObtenerUltimaUbicacionLetraPagar(string pstrIdLetraDetalle, IEnumerable<letrasmantenimientodetalle> ptempDs,
          TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Cobranza)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = new letrasmantenimientodetalleDto();
                    var Consulta = new List<letrasmantenimientodetalle>();

                    switch (oTipoMantenimiento)
                    {
                        case TipoMantenimiento.Cobranza:
                            Consulta =
                                ptempDs.Where(
                                    p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                            break;

                        case TipoMantenimiento.Pago:
                            Consulta =
                                ptempDs.Where(
                                    p => p.v_IdLetrasPagarDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                            break;
                    }

                    if (Consulta.Count > 0)
                    {
                        Consulta = Consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();

                        var ultimoRegistro = Consulta.Last();

                        var documento = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == ultimoRegistro.i_IdUbicacion);
                        var estado = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 110 && p.i_ItemId == ultimoRegistro.i_IdEstado);

                        Result = ultimoRegistro.ToDTO();
                        Result.Ubicacion = documento != null ? documento.v_Nombre : "OFICINA";
                        Result.Estado = estado != null ? estado.v_Value1 : "COBRANZA";
                    }
                    else
                    {
                        Result.Ubicacion = "OFICINA";
                        Result.Estado = "COBRANZA";
                    }

                    return Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }




        public bool CanjeLetrasTieneMantenimiento(string pstrIdCanje)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letrasDetallePorCanje =
                        dbContext.letrasdetalle.Where(p => p.v_IdLetras.Equals(pstrIdCanje) && p.i_Eliminado == 0);
                    foreach (var letrasdetalle in letrasDetallePorCanje)
                    {
                        if (
                            dbContext.letrasmantenimientodetalle.Any(
                                p => p.i_Eliminado == 0 && p.v_IdLetrasDetalle.Equals(letrasdetalle.v_IdLetrasDetalle)))
                            return true;

                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Indica si alguna de las letras en mantenimiento tiene cobranzas activas o hay un mantenimiento más reciente.
        /// </summary>
        /// <param name="pstrIdMantenimiento"></param>
        /// <returns></returns>
        public static bool MantenimientoBloqueado(string pstrIdMantenimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letrasMantenimiento =
                        dbContext.letrasmantenimientodetalle.Where(
                            p => p.v_IdLetrasMantenimiento.Equals(pstrIdMantenimiento) && p.i_Eliminado == 0)
                            .Select(p => p.v_IdLetrasDetalle).ToList();
                    var mant = dbContext.letrasmantenimientodetalle.Where(
                        p => p.v_IdLetrasMantenimiento.Equals(pstrIdMantenimiento) && p.i_Eliminado == 0).ToList();

                    if (!mant.Any()) return false;

                    var dateTime = mant.Max(o => o.t_InsertaFecha);
                    if (dateTime == null) return false;
                    {
                        var fechaMantenimiento = dateTime.Value;

                        var existeAlgunaEnCobranzaActiva = dbContext.cobranzadetalle.Any(
                            p => letrasMantenimiento.Contains(p.v_IdVenta) && p.i_Eliminado == 0);

                        var existeMantenimientoMasActual =
                            dbContext.letrasmantenimientodetalle.Any(
                                p =>
                                    !p.v_IdLetrasMantenimiento.Equals(pstrIdMantenimiento) &&
                                    letrasMantenimiento.Contains(p.v_IdLetrasDetalle) &&
                                    p.t_InsertaFecha > fechaMantenimiento && p.i_Eliminado == 0);

                        var existeLetrasDescuentoMantenimiento =
                            dbContext.letrasdescuentomantenimiento.Any(
                                p => p.i_Eliminado == 0 && letrasMantenimiento.Contains(p.v_IdLetrasDetalle));

                        return existeAlgunaEnCobranzaActiva || existeMantenimientoMasActual || existeLetrasDescuentoMantenimiento;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<letrasdetalleDto> ObtenerTodasLetras()
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    return dbContext.letrasdetalle.Where(p => p.i_Eliminado == 0).ToList().ToDTOs();
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static void ReGenerarTesoreriaLetraDescuento(ref OperationResult pobjOperationResult,
            string pstrIdmantenimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var idRegistroEliminado = new TesoreriaBL().EliminarTesoreriaXDocRef(ref pobjOperationResult, pstrIdmantenimiento, Globals.ClientSession.GetAsList());
                    if (pobjOperationResult.Success == 0) return;
                    GenerarTesoreriaLetraDescuento(ref pobjOperationResult, pstrIdmantenimiento);
                    if (pobjOperationResult.Success == 0) return;

                    ts.Complete();
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ReGenerarTesoreriaLetraDescuento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static void GenerarTesoreriaLetraDescuento(ref OperationResult pobjOperationResult,
            string pstrIdMantenimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var mantenimientoLetra =
                        dbContext.letrasmantenimiento.FirstOrDefault(
                            p => p.v_IdLetrasMantenimiento.Equals(pstrIdMantenimiento));
                    if (mantenimientoLetra == null) return;

                    var manteminientoLetraDetalles = mantenimientoLetra.letrasmantenimientodetalle
                        .Where(p => p.i_IdEstado == 2 && p.i_Eliminado == 0)
                        .ToList();

                    foreach (var letrasDetalle in manteminientoLetraDetalles.GroupBy(o => o.i_IdUbicacion))
                    {

                        var objTesoreriaBl = new TesoreriaBL();
                        var cTesoreriaDto = new tesoreriaDto();
                        var tesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();
                        var tesoreriaDetalleGastos = new List<tesoreriadetalleDto>();
                        var _TesoreriaDetalleIntereses = new List<tesoreriadetalleDto>();

                        var _ListadoTesorerias = objTesoreriaBl.ObtenerListadoTesorerias(ref pobjOperationResult,
                            mantenimientoLetra.t_FechaRegistro.Value.Year.ToString(),
                            mantenimientoLetra.t_FechaRegistro.Value.Month.ToString("00"), letrasDetalle.Key ?? -1);

                        var _MaxMovimiento = _ListadoTesorerias.Any()
                            ? int.Parse(_ListadoTesorerias[_ListadoTesorerias.Count - 1].Value1)
                            : 0;
                        _MaxMovimiento++;
                        cTesoreriaDto.d_TipoCambio = 3;
                        cTesoreriaDto.i_IdMoneda = 1;
                        cTesoreriaDto.v_Mes = mantenimientoLetra.t_FechaRegistro.Value.Month.ToString("00");
                        cTesoreriaDto.v_Periodo = mantenimientoLetra.t_FechaRegistro.Value.Year.ToString();
                        cTesoreriaDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        cTesoreriaDto.i_IdTipoDocumento = letrasDetalle.Key ?? -1;
                        cTesoreriaDto.v_Glosa = "ABONO BANCO POR LETRAS DESCUENTO";
                        cTesoreriaDto.v_NroCuentaCajaBanco =
                            objTesoreriaBl.DevuelveCuentaCajaBanco(ref pobjOperationResult, letrasDetalle.Key ?? -1).NroCuenta;
                        if (pobjOperationResult.Success == 0) return;
                        cTesoreriaDto.i_IdEstado = 1;

                        cTesoreriaDto.i_IdMedioPago = 1;
                        cTesoreriaDto.t_FechaRegistro = mantenimientoLetra.t_FechaRegistro.Value;
                        cTesoreriaDto.v_IdCobranza = mantenimientoLetra.v_IdLetrasMantenimiento;
                        cTesoreriaDto.i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Ingreso;

                        foreach (var fila in letrasDetalle)
                        {
                            var dhTesoreriadetalleDto = new tesoreriadetalleDto();
                            var letra = dbContext.letrasdetalle.FirstOrDefault(v => v.v_IdLetrasDetalle == fila.v_IdLetrasDetalle);
                            if (letra == null) throw new Exception("La letra no existe");
                            var esLetraRenovada = int.Parse(letra.v_Correlativo.Split('-')[1]) > 0;
                            if (esLetraRenovada) continue;
                            var importeLetra = letra.d_Importe ?? 0;
                            dhTesoreriadetalleDto.v_IdTesoreria = cTesoreriaDto.v_IdTesoreria;
                            dhTesoreriadetalleDto.d_Cambio = cTesoreriaDto.i_IdMoneda == 1
                                ? importeLetra / cTesoreriaDto.d_TipoCambio
                                : importeLetra * cTesoreriaDto.d_TipoCambio;
                            dhTesoreriadetalleDto.d_Cambio =
                                Utils.Windows.DevuelveValorRedondeado(dhTesoreriadetalleDto.d_Cambio ?? 0, 2);

                            dhTesoreriadetalleDto.d_Importe = importeLetra;
                            dhTesoreriadetalleDto.v_Naturaleza = "H";
                            dhTesoreriadetalleDto.v_Analisis = string.Empty;

                            var cliente = dbContext.letrasdetalle.Join(dbContext.cliente,
                                v => v.v_IdCliente, j1 => j1.v_IdCliente, (v, j1) => new { v, J1 = j1 })
                                .Where(@t => @t.v.v_IdLetrasDetalle == fila.v_IdLetrasDetalle)
                                .Select(@t => new { @t.J1.v_IdCliente }).FirstOrDefault();

                            if (cliente != null)
                                dhTesoreriadetalleDto.v_IdCliente = cliente.v_IdCliente;

                            if (string.IsNullOrWhiteSpace(Globals.ClientSession.NroCtaObligacionesFinancieros) ||
                                !Utils.Windows.EsCuentaImputable(Globals.ClientSession.NroCtaObligacionesFinancieros))
                                throw new Exception("La cuenta para obligaciones financieras no está especificada!");

                            dhTesoreriadetalleDto.v_NroCuenta = Globals.ClientSession.NroCtaObligacionesFinancieros;
                            dhTesoreriadetalleDto.v_NroDocumento = letra.v_Serie + "-" + letra.v_Correlativo;
                            dhTesoreriadetalleDto.v_OrigenDestino = "";
                            dhTesoreriadetalleDto.v_Pedido = "";
                            dhTesoreriadetalleDto.t_Fecha = letra.t_FechaEmision;
                            dhTesoreriadetalleDto.i_IdTipoDocumento = letra.i_IdTipoDocumento ?? -1;
                            dhTesoreriadetalleDto.i_IdCentroCostos = "0";
                            dhTesoreriadetalleDto.i_IdCaja = 0;
                            tesoreriaDetalleXInsertar.Add(dhTesoreriadetalleDto);
                        }

                        if (!tesoreriaDetalleXInsertar.Any()) return;
                        if (letrasDetalle.Sum(p => p.d_GastosAdministrativos ?? 0) > 0)
                        {
                            var ddTesoreriadetalleDto = new tesoreriadetalleDto();
                            var ctaGasto =
                                Utils.Windows.DevuelveCuentaDatos(Globals.ClientSession.NroCtaGastosFinancieros);
                            if (ctaGasto == null)
                                throw new ArgumentNullException(@"La cuenta de gasto financiero es inválida.");
                            var importe = (letrasDetalle.Sum(p => p.d_GastosAdministrativos ?? 0));

                            ddTesoreriadetalleDto.v_IdTesoreria = cTesoreriaDto.v_IdTesoreria;
                            ddTesoreriadetalleDto.d_Cambio = cTesoreriaDto.i_IdMoneda == 1
                                ? importe / cTesoreriaDto.d_TipoCambio
                                : importe * cTesoreriaDto.d_TipoCambio;
                            ddTesoreriadetalleDto.d_Cambio =
                                Utils.Windows.DevuelveValorRedondeado(ddTesoreriadetalleDto.d_Cambio ?? 0, 2);
                            ddTesoreriadetalleDto.d_Importe = importe;
                            ddTesoreriadetalleDto.i_IdTipoDocumentoRef = -1;
                            ddTesoreriadetalleDto.v_Naturaleza = "D";
                            ddTesoreriadetalleDto.v_Analisis = "GASTO FINANCIERO";
                            ddTesoreriadetalleDto.v_NroCuenta = ctaGasto.v_NroCuenta;
                            ddTesoreriadetalleDto.v_NroDocumento = string.Empty;
                            ddTesoreriadetalleDto.v_NroDocumentoRef = string.Empty;
                            ddTesoreriadetalleDto.v_OrigenDestino = "";
                            ddTesoreriadetalleDto.v_Pedido = "";
                            ddTesoreriadetalleDto.t_Fecha = cTesoreriaDto.t_FechaRegistro;
                            ddTesoreriadetalleDto.i_IdTipoDocumento = 309;
                            ddTesoreriadetalleDto.i_IdCentroCostos = string.Empty;
                            ddTesoreriadetalleDto.i_IdCaja = 0;

                            tesoreriaDetalleGastos.Add(ddTesoreriadetalleDto);
                        }


                        if (letrasDetalle.Count() == 1)
                        {
                            string IdVenta = letrasDetalle.FirstOrDefault().v_IdLetrasDetalle;
                            cTesoreriaDto.v_Nombre = (dbContext.letrasdetalle.GroupJoin(dbContext.cliente,
                                v => v.v_IdCliente, J1 => J1.v_IdCliente, (v, J1_join) => new { v, J1_join })
                                .SelectMany(@t => @t.J1_join.DefaultIfEmpty(), (@t, J1) => new { @t, J1 })
                                .Where(@t => @t.@t.v.v_IdLetrasDetalle == IdVenta)
                                .Select(@t => new
                                {
                                    Nombre =
                                        @t.@t.v.v_IdCliente != "N002-CL000000000"
                                            ? (@t.J1.v_PrimerNombre + " " + @t.J1.v_ApePaterno + " " +
                                               @t.J1.v_ApeMaterno + " " +
                                               @t.J1.v_RazonSocial).Trim()
                                            : "PÚBLICO GENERAL"
                                })
                                ).FirstOrDefault().Nombre;
                        }
                        else
                            cTesoreriaDto.v_Nombre = "COBRANZAS VARIOS";


                        {
                            var gastos = tesoreriaDetalleGastos.Sum(p => p.d_Importe ?? 0);
                            var ddTesoreriadetalleDto = new tesoreriadetalleDto();
                            var cobranzaEfectiva = tesoreriaDetalleXInsertar.Sum(p => p.d_Importe ?? 0);

                            var importe = cobranzaEfectiva - gastos;
                            var idTipoDocRef = tesoreriaDetalleXInsertar.LastOrDefault() != null
                                ? tesoreriaDetalleXInsertar.LastOrDefault().i_IdTipoDocumentoRef ?? -1
                                : -1;
                            ddTesoreriadetalleDto.v_IdTesoreria = cTesoreriaDto.v_IdTesoreria;
                            ddTesoreriadetalleDto.d_Cambio = cTesoreriaDto.i_IdMoneda == 1
                                ? importe / cTesoreriaDto.d_TipoCambio
                                : importe * cTesoreriaDto.d_TipoCambio;
                            ddTesoreriadetalleDto.d_Cambio =
                                Utils.Windows.DevuelveValorRedondeado(ddTesoreriadetalleDto.d_Cambio ?? 0, 2);
                            ddTesoreriadetalleDto.d_Importe = importe;
                            ddTesoreriadetalleDto.i_IdTipoDocumentoRef = tesoreriaDetalleXInsertar.Count == 1
                                ? tesoreriaDetalleXInsertar[0].i_IdTipoDocumento ?? -1
                                : -1;
                            ddTesoreriadetalleDto.v_Naturaleza = "D";
                            ddTesoreriadetalleDto.v_Analisis = "";
                            ddTesoreriadetalleDto.v_NroCuenta = cTesoreriaDto.v_NroCuentaCajaBanco;
                            ddTesoreriadetalleDto.v_NroDocumento = tesoreriaDetalleXInsertar[0].v_NroDocumentoRef;
                            ddTesoreriadetalleDto.v_NroDocumentoRef = tesoreriaDetalleXInsertar.Count == 1
                                ? tesoreriaDetalleXInsertar[0].v_NroDocumento
                                : string.Empty;
                            ddTesoreriadetalleDto.v_OrigenDestino = "";
                            ddTesoreriadetalleDto.v_Pedido = "";
                            ddTesoreriadetalleDto.t_Fecha = cTesoreriaDto.t_FechaRegistro;
                            ddTesoreriadetalleDto.i_IdTipoDocumento = idTipoDocRef;
                            ddTesoreriadetalleDto.i_IdCentroCostos = "0";
                            ddTesoreriadetalleDto.i_IdCaja = 0;

                            tesoreriaDetalleXInsertar.Add(ddTesoreriadetalleDto);
                        }

                        tesoreriaDetalleXInsertar = tesoreriaDetalleXInsertar.Concat(tesoreriaDetalleGastos).Concat(_TesoreriaDetalleIntereses).ToList();
                        var totDebe = tesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe ?? 0);
                        var totHaber = tesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe ?? 0);
                        var totDebeC = tesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio ?? 0);
                        var totHaberC = tesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio ?? 0);
                        cTesoreriaDto.d_TotalDebe_Importe = totDebe;
                        cTesoreriaDto.d_TotalHaber_Importe = totHaber;
                        cTesoreriaDto.d_TotalDebe_Cambio = totDebeC;
                        cTesoreriaDto.d_TotalHaber_Cambio = totHaberC;
                        cTesoreriaDto.d_Diferencia_Importe = totDebe - totHaber;
                        cTesoreriaDto.d_Diferencia_Cambio = totDebeC - totHaberC;

                        if (tesoreriaDetalleXInsertar.Any())
                        {
                            objTesoreriaBl.Insertartesoreria(ref pobjOperationResult, cTesoreriaDto, Globals.ClientSession.GetAsList(), tesoreriaDetalleXInsertar);
                            if (pobjOperationResult.Success == 0) return;
                        }
                    }

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.GenerarTesoreriaLetraDescuento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public enum TipoMantenimiento
        {
            Cobranza = 1,
            Pago = 2
        }

        #endregion

        #region Letras Por Cobrar

        #region Canje Letras

        public void RecalcularEstados(ref OperationResult pobjOperationResult, List<string> idLetras)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letras = (from n in dbContext.letrasdetalle
                                  join c in dbContext.cobranzaletraspendiente on new { id = n.v_IdLetrasDetalle, e = 0 }
                                      equals new { id = c.v_IdLetrasDetalle, e = c.i_Eliminado.Value } into cJOin
                                  from c in cJOin.DefaultIfEmpty()
                                  where idLetras.Contains(n.v_IdLetrasDetalle)
                                  select new
                                  {
                                      letra = n,
                                      pagada = c.d_Saldo == 0
                                  }).ToList();

                    if (letras.Any())
                    {
                        foreach (var letra in letras)
                        {
                            letra.letra.i_Pagada = letra.pagada ? 1 : 0;
                            dbContext.letrasdetalle.ApplyCurrentValues(letra.letra);
                        }

                        dbContext.SaveChanges();
                    }

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RecalcularEstados()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public List<letrasDto> ObtenerLetrasBandeja(ref OperationResult pobjOperationResult, DateTime F_Ini,
            DateTime F_Fin, string pstrIdCliente)
        {
            F_Ini = F_Ini.Date;
            F_Fin =
                DateTime.Parse(F_Fin.Day.ToString() + "/" + F_Fin.Month.ToString() + "/" + F_Fin.Year.ToString() +
                               " 23:59");
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<letrasdetalle> ListaLetrasDetalle = dbContext.letrasdetalle.Where(l => l.i_Eliminado == 0).ToList();
                    int IdTipoDoc = ListaLetrasDetalle.Any() ? ListaLetrasDetalle.Where(l => l.i_IdTipoDocumento != -1).Select(l => l.i_IdTipoDocumento).FirstOrDefault().Value : -1;
                    string documentLetra = dbContext.documento.Where(l => l.i_Eliminado == 0 && l.i_CodigoDocumento == IdTipoDoc).FirstOrDefault().v_Siglas;
                    var Consulta = (from p in dbContext.letras
                                    join J1 in dbContext.cliente on p.v_IdCliente equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.systemuser on new { i_UpdateUserId = p.i_ActualizaIdUsuario.Value }
                                        equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.systemuser on new { i_InsertUserId = p.i_InsertaIdUsuario.Value }
                                        equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()
                                    where p.t_FechaRegistro >= F_Ini.Date && p.t_FechaRegistro <= F_Fin && p.i_Eliminado == 0

                                    && (J1.v_IdCliente == pstrIdCliente || pstrIdCliente == null)
                                    select new
                                    {
                                        v_IdLetras = p.v_IdLetras,
                                        NroRegistro = p.v_Mes.Trim() + "-" + p.v_Correlativo.Trim(),
                                        NombreCliente =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        t_FechaRegistro = p.t_FechaRegistro,
                                        Moneda = p.i_IdMoneda == 1 ? "SOLES" : "DÓLARES",
                                        d_TotalLetras = p.d_TotalLetras,
                                        v_UsuarioCreacion = J3.v_UserName,
                                        v_UsuarioModificacion = J2.v_UserName,
                                        t_ActualizaFecha = p.t_ActualizaFecha,
                                        t_InsertaFecha = p.t_InsertaFecha,
                                        v_IdCliente = p.v_IdCliente,

                                    }).ToList().Select(l => new letrasDto
                                    {
                                        v_IdLetras = l.v_IdLetras,
                                        NroRegistro = l.NroRegistro,
                                        NombreCliente = l.NombreCliente,
                                        t_FechaRegistro = l.t_FechaRegistro,
                                        Moneda = l.Moneda,
                                        d_TotalLetras = l.d_TotalLetras,
                                        v_UsuarioCreacion = l.v_UsuarioCreacion,
                                        v_UsuarioModificacion = l.v_UsuarioModificacion,
                                        t_ActualizaFecha = l.t_ActualizaFecha,
                                        t_InsertaFecha = l.t_InsertaFecha,
                                        v_IdCliente = l.v_IdCliente,
                                        ListaLetrasContiene = (from a in ListaLetrasDetalle

                                                               where a.v_IdLetras == l.v_IdLetras

                                                               select documentLetra + " " + a.v_Serie + " " + a.v_Correlativo).AsEnumerable(),

                                    }).ToList();


                    Consulta.AsParallel().ToList().ForEach(letra =>
                    {

                        letra.LetrasVistaRapida = string.Join(", ", letra.ListaLetrasContiene.Distinct());

                    });

                    pobjOperationResult.Success = 1;
                    return Consulta.ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasBandeja()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public letrasDto ObtenerLetra(ref OperationResult pobjOperationResult, string pstrIdLetra)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                try
                {
                    var Consulta = (from n in dbContext.letras
                                    join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.v_IdLetras == pstrIdLetra && n.i_Eliminado == 0
                                    select new letrasDto
                                    {
                                        d_TipoCambio = n.d_TipoCambio,
                                        d_TotalFacturas = n.d_TotalFacturas,
                                        d_TotalLetras = n.d_TotalLetras,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaRegistro = n.t_FechaRegistro,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdCliente,
                                        NombreCliente =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        v_IdLetras = n.v_IdLetras,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        i_NoSeleccionarFactura = n.i_NoSeleccionarFactura,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Eliminado = n.i_Eliminado,
                                        i_IdMoneda = n.i_IdMoneda,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_NroDias = n.i_NroDias,
                                        i_NroLetras = n.i_NroLetras,
                                        i_IdEstado = n.i_IdEstado
                                    }).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetra()\nLinea:" +
                                                                ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null
                        ? ex.InnerException.Message
                        : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return null;
                }
            }
        }

        public List<letrascanjeDto> ObtenerLetrasCanje(ref OperationResult pobjOperationResult, string pstrIdLetra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Obtener Canjes de Ventas

                    var Consulta = (from n in dbContext.letrascanje
                                    join J1 in dbContext.venta on n.v_IdVenta equals J1.v_IdVenta into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.documento on J1.i_IdTipoDocumento equals J2.i_CodigoDocumento into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.letrasdetalle on n.v_IdLetrasDetalle equals J3.v_IdLetrasDetalle into
                                        J3_join
                                    from J3 in J3_join.DefaultIfEmpty()
                                    join J4 in dbContext.documento on J3.i_IdTipoDocumento equals J4.i_CodigoDocumento into J4_join
                                    from J4 in J4_join.DefaultIfEmpty()
                                    join J5 in dbContext.adelanto on n.v_IdAdelanto equals J5.v_IdAdelanto into J5_join
                                    from J5 in J5_join.DefaultIfEmpty()
                                    join J6 in dbContext.documento on J5.i_IdTipoDocumento equals J6.i_CodigoDocumento into J6_join
                                    from J6 in J6_join.DefaultIfEmpty()
                                    where n.v_IdLetras == pstrIdLetra && n.i_Eliminado == 0
                                    select new letrascanjeDto
                                    {
                                        TipoDocumento = J5 == null ? (J1 != null ? J2.v_Siglas : J4.v_Siglas) : J6.v_Siglas,
                                        NroDocumento = J5 == null ?
                                            (J1 != null
                                                ? J1.v_SerieDocumento + "-" + J1.v_CorrelativoDocumento
                                                : J3.v_Serie + "-" + J3.v_Correlativo) : J5.v_SerieDocumento + "-" + J5.v_CorrelativoDocumento,
                                        FechaEmision = J5 == null ? (J1 != null ? J1.t_FechaRegistro : J3.t_FechaEmision) : J5.t_FechaAdelanto ?? DateTime.Now,
                                        Moneda = J5 == null ?
                                            (J1 != null
                                                ? J1.i_IdMoneda == 1 ? "SOLES" : "DÓLARES"
                                                : J3.i_IdMoneda == 1 ? "SOLES" : "DÓLARES") : J5.i_IdMoneda == 1 ? "SOLES" : "DÓLARES",
                                        Importe = J5 == null ? (J1 != null ? J1.d_Total ?? 0 : J3.d_Importe ?? 0) : J5.d_Importe ?? 0,
                                        TipoCambio = J5 == null ? (J1 != null ? J1.d_TipoCambio ?? 1 : J3.d_TipoCambio ?? 1) : J5.d_TipoCambio ?? 1,
                                        d_MontoCanjeado = n.d_MontoCanjeado.Value
                                    }
                        ).ToList();

                    #endregion

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasCanje()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<letrasdetalleDto> ObtenerLetrasDetalle(ref OperationResult pobjOperationResult, string pstrIdLetra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var cOnsulta =
                        dbContext.letrasdetalle.Where(p => p.v_IdLetras == pstrIdLetra && p.i_Eliminado == 0)
                            .ToList().ToDTOs();

                    pobjOperationResult.Success = 1;
                    return cOnsulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasDetalle()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool VentaFueCanjeadaALetras(ref OperationResult pobjOperationResult, string pstrIdVenta)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var r = dbContext.letrascanje.Count(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0);
                    pobjOperationResult.Success = 1;
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.VentaFueCanjeadaALetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string InsertarCanjeoLetras(ref OperationResult pobjOperationResult, letrasDto pobjletrasDto,
            List<letrascanjeDto> plobjletrascanjeDto, List<letrasdetalleDto> plobjletrasdetalleDto,
            List<string> ClientSession, bool esRenovacion)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBL = new SecuentialBL();
                        var objEntityLetras = letrasAssembler.ToEntity(pobjletrasDto);

                        int SecuentialId = 0;
                        string newIdLetras = string.Empty;
                        string newIdLetrasDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera

                        objEntityLetras.t_InsertaFecha = DateTime.Now;
                        objEntityLetras.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityLetras.i_Eliminado = 0;

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 69);
                        newIdLetras = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LA");
                        objEntityLetras.v_IdLetras = newIdLetras;
                        dbContext.AddToletras(objEntityLetras);

                        #endregion

                        #region Inserta Canjes

                        foreach (var canje in plobjletrascanjeDto)
                        {
                            var _letrascanje = canje.ToEntity();
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 72);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LB");
                            _letrascanje.v_IdLetrasCanje = newIdLetrasDetalle;
                            _letrascanje.v_IdLetras = newIdLetras;
                            _letrascanje.t_InsertaFecha = DateTime.Now;
                            _letrascanje.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrascanje.i_Eliminado = 0;
                            dbContext.AddToletrascanje(_letrascanje);
                            ProcesaDetalleCobranza(ref pobjOperationResult, canje, pobjletrasDto, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }

                        #endregion

                        #region Inserta Letras Detalle

                        foreach (var letraDetalle in plobjletrasdetalleDto)
                        {
                            var serie = letraDetalle.v_Serie.Trim();
                            var corr = letraDetalle.v_Correlativo.Trim();

                            var letraExiste = dbContext.letrasdetalle.Any(p => p.v_Serie.Trim().Equals(serie) && 
                                p.v_Correlativo.Trim().Equals(corr) && p.i_Eliminado == 0);
                            if(letraExiste) 
                                throw new Exception("Una o más de las letras por guardar, acaban de ser guardadas por otro usuario. Por favor regenere las letras e intente de nuevo.");

                            var _letrasdetalle = new letrasdetalle();
                            _letrasdetalle = letraDetalle.ToEntity();
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 73);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LC");
                            _letrasdetalle.v_IdLetrasDetalle = newIdLetrasDetalle;
                            _letrasdetalle.v_IdLetras = newIdLetras;
                            _letrasdetalle.d_TipoCambio = pobjletrasDto.d_TipoCambio ?? 3;
                            _letrasdetalle.t_InsertaFecha = DateTime.Now;
                            _letrasdetalle.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                            _letrasdetalle.i_Eliminado = 0;
                            letraDetalle.v_IdLetrasDetalle = newIdLetrasDetalle;
                            dbContext.AddToletrasdetalle(_letrasdetalle);

                            #region Actualiza Correlativo EmpresaDetalle
                            var Index = letraDetalle.v_Correlativo.IndexOf('-');
                            string SerieSufijo = letraDetalle.v_Correlativo.Substring(Index + 1, 2);

                            if (SerieSufijo == "00")
                            {
                                new DocumentoBL().ActualizarCorrelativoPorSerie(ref pobjOperationResult,
                                    Globals.ClientSession.i_IdEstablecimiento, _letrasdetalle.i_IdTipoDocumento.Value,
                                    _letrasdetalle.v_Serie, int.Parse(_letrasdetalle.v_Correlativo.Substring(0, 5)) + 1);
                                if (pobjOperationResult.Success == 0) return string.Empty;
                            }
                            #endregion

                            dbContext.SaveChanges();
                            if (esRenovacion)
                            {
                                var letraParent = plobjletrascanjeDto.First();
                                if (letraParent == null) return null;
                                var ultimaRenova = ObtenerUltimaUbicacionLetraParaRenovacion(letraParent.v_IdLetrasDetalle);
                                if (ultimaRenova != null)
                                {
                                    var cabeceraParent = dbContext.letrasmantenimiento.SingleOrDefault(p => p.v_IdLetrasMantenimiento.Equals(ultimaRenova.v_IdLetrasMantenimiento));
                                    var cabeceraToInsert = Utils.ObjectCopier.Clone(cabeceraParent);
                                    cabeceraToInsert.v_Periodo = _letrasdetalle.t_FechaEmision.Value.Year.ToString();
                                    cabeceraToInsert.v_Correlativo = Utils.Windows.RetornaCorrelativoPorFecha(ref pobjOperationResult,
                                        ListaProcesos.LetrasMantenimiento, null, _letrasdetalle.t_FechaEmision.Value, "1", 0);
                                    var detalleToInsert = Utils.ObjectCopier.Clone(ultimaRenova);
                                    detalleToInsert.v_IdLetrasDetalle = _letrasdetalle.v_IdLetrasDetalle;
                                    InsertarMantenimientoLetras(ref pobjOperationResult, cabeceraToInsert.ToDTO(),
                                        new List<letrasmantenimientodetalleDto> { detalleToInsert.ToDTO() }, Globals.ClientSession.GetAsList());
                                    if (pobjOperationResult.Success == 0) return null;
                                }
                            }
                        }

                        #endregion

                        #region Genera Libro Diario

                        string IDConcepto = pobjletrasDto.i_IdMoneda == 1 ? "30" : "31";

                        var aa = (from a in dbContext.administracionconceptos
                                  where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                  select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                        if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty &&
                            aa.v_CuentaPVenta.Trim() != string.Empty)
                        {
                            DiarioBL _objDiarioBL = new DiarioBL();
                            diarioDto _diarioDto = new diarioDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                            #region Diario Cabecera
                            var documentoDiario = Globals.ClientSession.i_IdDocumentoContableLEC ?? 335;
                            _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult,
                                pobjletrasDto.t_FechaRegistro.Value.Year.ToString(),
                                pobjletrasDto.t_FechaRegistro.Value.Month.ToString("00"), documentoDiario);

                            var maxMovimiento = _ListadoDiarios.Any()
                                ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count - 1].Value1)
                                : 0;
                            maxMovimiento++;
                            _diarioDto.v_IdDocumentoReferencia = newIdLetras;
                            _diarioDto.v_Periodo = pobjletrasDto.t_FechaRegistro.Value.Year.ToString();
                            _diarioDto.v_Mes =
                                int.Parse(pobjletrasDto.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                            _diarioDto.v_Nombre = pobjletrasDto.NombreCliente;

                            switch (pobjletrasDto.i_IdEstado)
                            {
                                case 14:
                                    _diarioDto.v_Glosa = "CANJE DE FACTURAS POR LETRAS";
                                    break;

                                case 3:
                                    _diarioDto.v_Glosa = "CANJE DE LETRAS POR RENOVACIÓN";
                                    break;

                                case 7:
                                    _diarioDto.v_Glosa = "CANJE DE LETRAS POR REFINANCIACIÓN";
                                    break;

                                case 8:
                                    _diarioDto.v_Glosa = "CANJE DE LETRAS POR RENOVACIÓN DCTO";
                                    break;
                            }

                            _diarioDto.v_Correlativo = maxMovimiento.ToString("00000000");
                            _diarioDto.d_TipoCambio = pobjletrasDto.d_TipoCambio.Value;
                            _diarioDto.i_IdMoneda = pobjletrasDto.i_IdMoneda.Value;
                            _diarioDto.i_IdTipoDocumento = documentoDiario;
                            _diarioDto.t_Fecha = pobjletrasDto.t_FechaRegistro.Value;
                            _diarioDto.i_IdTipoComprobante = 2;

                            #endregion

                            #region Ventas Canjeadas

                            foreach (var cuentaDetalle in plobjletrascanjeDto.Where(p => (p.i_EsAdelanto ?? 0) != 1))
                            {
                                #region lógica de asiento para facturas y letras

                                if (cuentaDetalle.TipoDocumento != "LEC")
                                {
                                    //canje de facturas
                                    var H_SubTotalVenta = new diariodetalleDto();
                                    var VentaOriginal =
                                        dbContext.venta.FirstOrDefault(p => p.v_IdVenta == cuentaDetalle.v_IdVenta);
                                    var IDConceptoVenta = VentaOriginal.i_IdTipoVenta.Value.ToString("00");
                                    decimal SubTotal = cuentaDetalle.d_MontoCanjeado.Value;
                                    H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                    H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal / VentaOriginal.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal * VentaOriginal.d_TipoCambio.Value, 2);
                                    H_SubTotalVenta.i_IdCentroCostos = "0";
                                    H_SubTotalVenta.i_IdTipoDocumento = VentaOriginal.i_IdTipoDocumento.Value;
                                    H_SubTotalVenta.t_Fecha = VentaOriginal.t_FechaRegistro.Value;
                                    H_SubTotalVenta.v_IdCliente = VentaOriginal.v_IdCliente;
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0
                                        ? !_objDocumentoBl.DocumentoEsInverso(VentaOriginal.i_IdTipoDocumento.Value)
                                            ? "H"
                                            : "D"
                                        : !_objDocumentoBl.DocumentoEsInverso(VentaOriginal.i_IdTipoDocumento.Value)
                                            ? "D"
                                            : "H";
                                    H_SubTotalVenta.v_NroDocumento = VentaOriginal.v_SerieDocumento + "-" +
                                                                     VentaOriginal.v_CorrelativoDocumento;

                                    H_SubTotalVenta.v_NroCuenta = (dbContext.administracionconceptos.Where(
                                        a => a.v_Codigo == IDConceptoVenta && a.v_Periodo.Equals(periodo)).Select(a => new { a.v_CuentaPVenta })).First()
                                        .v_CuentaPVenta;

                                    if (_objDocumentoBl.DocumentoEsInverso(VentaOriginal.i_IdTipoDocumento.Value) ||
                                        VentaOriginal.i_IdTipoDocumento.Value.ToString() == "8")
                                    {
                                        H_SubTotalVenta.i_IdTipoDocumentoRef = VentaOriginal.i_IdTipoDocumentoRef.Value;
                                        H_SubTotalVenta.v_NroDocumentoRef = VentaOriginal.v_SerieDocumentoRef + "-" +
                                                                            VentaOriginal.v_CorrelativoDocumentoRef;
                                    }

                                    TempXInsertar.Add(H_SubTotalVenta);
                                    H_SubTotalVenta = new diariodetalleDto();
                                }
                                else
                                {
                                    //canje de letras
                                    var H_SubTotalVenta = new diariodetalleDto();
                                    var VentaOriginal =
                                        dbContext.letrasdetalle
                                            .FirstOrDefault(p => p.v_IdLetrasDetalle == cuentaDetalle.v_IdLetrasDetalle);
                                    string IDConceptoVenta = VentaOriginal.i_IdMoneda == 1 ? "30" : "31";
                                    decimal SubTotal = cuentaDetalle.d_MontoCanjeado.Value;
                                    H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                    H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal / VentaOriginal.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(SubTotal * VentaOriginal.d_Importe.Value,
                                            2);
                                    H_SubTotalVenta.i_IdCentroCostos = "0";
                                    H_SubTotalVenta.i_IdTipoDocumento = VentaOriginal.i_IdTipoDocumento.Value;
                                    H_SubTotalVenta.t_Fecha = VentaOriginal.t_FechaEmision.Value;
                                    H_SubTotalVenta.v_IdCliente = VentaOriginal.v_IdCliente;
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0
                                        ? !_objDocumentoBl.DocumentoEsInverso(VentaOriginal.i_IdTipoDocumento.Value)
                                            ? "H"
                                            : "D"
                                        : !_objDocumentoBl.DocumentoEsInverso(VentaOriginal.i_IdTipoDocumento.Value)
                                            ? "D"
                                            : "H";
                                    H_SubTotalVenta.v_NroDocumento = VentaOriginal.v_Serie + "-" +
                                                                     VentaOriginal.v_Correlativo;
                                    H_SubTotalVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                                   where a.v_Codigo == IDConceptoVenta && a.v_Periodo.Equals(periodo) && a.i_Eliminado == 0
                                                                   select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                                    TempXInsertar.Add(H_SubTotalVenta);
                                    H_SubTotalVenta = new diariodetalleDto();
                                }

                                #endregion
                            }

                            foreach (var cuentaDetalle in plobjletrascanjeDto.Where(p => (p.i_EsAdelanto ?? 0) == 1))
                            {
                                //canje de adelantos
                                var H_SubTotalVenta = new diariodetalleDto();
                                var AdelantoOriginal = dbContext.adelanto.FirstOrDefault(p => p.v_IdAdelanto == cuentaDetalle.v_IdAdelanto);

                                decimal SubTotal = cuentaDetalle.d_MontoCanjeado ?? 0;
                                H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal / AdelantoOriginal.d_TipoCambio.Value, 2)
                                    : Utils.Windows.DevuelveValorRedondeado(SubTotal * AdelantoOriginal.d_Importe.Value,
                                        2);
                                H_SubTotalVenta.i_IdCentroCostos = "0";
                                H_SubTotalVenta.i_IdTipoDocumento = AdelantoOriginal.i_IdTipoDocumento.Value;
                                H_SubTotalVenta.t_Fecha = AdelantoOriginal.t_FechaAdelanto.Value;
                                H_SubTotalVenta.v_IdCliente = AdelantoOriginal.v_IdCliente;
                                H_SubTotalVenta.v_Naturaleza = "H";
                                H_SubTotalVenta.v_NroDocumento = AdelantoOriginal.v_SerieDocumento + "-" +
                                                                 AdelantoOriginal.v_CorrelativoDocumento;
                                H_SubTotalVenta.v_NroCuenta = Globals.ClientSession.NroCtaAdelanto;
                                TempXInsertar.Add(H_SubTotalVenta);
                                H_SubTotalVenta = new diariodetalleDto();
                            }

                            #endregion

                            #region Letras

                            foreach (var letra in plobjletrasdetalleDto)
                            {
                                var SubTotal = letra.d_Importe;
                                var H_SubTotalVenta = new diariodetalleDto();
                                H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal.Value / pobjletrasDto.d_TipoCambio.Value, 2)
                                    : Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal.Value * pobjletrasDto.d_TipoCambio.Value, 2);
                                H_SubTotalVenta.i_IdCentroCostos = "0";
                                H_SubTotalVenta.i_IdTipoDocumento = letra.i_IdTipoDocumento.Value;
                                H_SubTotalVenta.t_Fecha = pobjletrasDto.t_FechaRegistro.Value;
                                H_SubTotalVenta.v_IdCliente = letra.v_IdCliente;
                                H_SubTotalVenta.v_Naturaleza = "D";
                                H_SubTotalVenta.v_NroDocumento = letra.v_Serie + "-" + letra.v_Correlativo;
                                H_SubTotalVenta.v_NroCuenta = (dbContext.administracionconceptos.Where(
                                    a => a.v_Codigo == IDConcepto && a.v_Periodo.Equals(periodo) && a.i_Eliminado == 0).Select(a => new { a.v_CuentaPVenta })).First().v_CuentaPVenta;
                                H_SubTotalVenta.i_IdTipoDocumentoRef = -1;
                                TempXInsertar.Add(H_SubTotalVenta);
                            }

                            #endregion

                            var TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe ?? 0);
                            var TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe ?? 0);
                            var TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio ?? 0);
                            var TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio ?? 0);

                            _diarioDto.d_TotalDebe =
                                decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalHaber =
                                decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalDebeCambio =
                                decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalHaberCambio =
                                decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_DiferenciaDebe =
                                decimal.Parse(
                                    Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_DiferenciaHaber =
                                decimal.Parse(
                                    Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));

                            _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto,
                                Globals.ClientSession.GetAsList(), TempXInsertar, (int)TipoMovimientoTesoreria.Ingreso);
                            if (pobjOperationResult.Success == 0) return string.Empty;
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage =
                                "No existe el concepto 30 (CARTERA MN) o 31 (CARTERA ME) para Letras.";
                            pobjOperationResult.ExceptionMessage =
                                "No se pudo completar la transacción, por favor agrege los conceptos señalados.";
                            pobjOperationResult.AdditionalInformation = "LetrasBL.InsertarCanjeoLetras()";
                            return null;
                        }

                        dbContext.SaveChanges();

                        #endregion

                        #region Genera Pendientes por cobrar en letras

                        foreach (var LetraDetalle in plobjletrasdetalleDto)
                        {
                            RegistraPendienteCobranzaLetra(ref pobjOperationResult, LetraDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }

                        #endregion

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                        return newIdLetras;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.InsertarCanjeoLetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        private static letrasmantenimientodetalle ObtenerUltimaUbicacionLetraParaRenovacion(string pstrIdLetraDetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.letrasmantenimientodetalle.Where(
                            p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();

                    if (consulta.Count > 0)
                    {
                        consulta = consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();

                        var ultimoRegistro = consulta.Last();

                        return ultimoRegistro;
                    }
                    return null;
                }
            }
            catch { return null; }
        }

        public void EliminarCanjeoLetras(ref OperationResult pobjOperationResult, string pstrIdCanjeoLetras,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Elimina Diario

                        new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, pstrIdCanjeoLetras, ClientSession,
                            true);
                        if (pobjOperationResult.Success == 0) return;

                        #endregion

                        #region ELimina Canjeo Cabecera

                        var Cabecera = dbContext.letras.Where(p => p.v_IdLetras == pstrIdCanjeoLetras).FirstOrDefault();
                        Cabecera.t_ActualizaFecha = DateTime.Now;
                        Cabecera.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Cabecera.i_Eliminado = 1;
                        dbContext.letras.ApplyCurrentValues(Cabecera);

                        #endregion

                        #region Elimina Canjeo Facturas

                        foreach (var item in Cabecera.letrascanje)
                        {
                            item.t_ActualizaFecha = DateTime.Now;
                            item.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            item.i_Eliminado = 1;
                            RestauraDetalleCobranza(ref pobjOperationResult, letrascanjeAssembler.ToDTO(item),
                                letrasAssembler.ToDTO(Cabecera), ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                            dbContext.letrascanje.ApplyCurrentValues(item);
                        }

                        #endregion

                        #region Elimina Canjeo Letras Detalle

                        foreach (var item in Cabecera.letrasdetalle)
                        {
                            item.t_ActualizaFecha = DateTime.Now;
                            item.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            item.i_Eliminado = 1;
                            dbContext.letrasdetalle.ApplyCurrentValues(item);
                            EliminaPendienteCobranzaLetra(ref pobjOperationResult, item.v_IdLetrasDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.EliminarCanjeoLetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public int ObtenerUltimoCorrelativoParaRenovacion(ref OperationResult pobjOperationResult,
            string pstrCorrelativo, string pstrSerie)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Separacion = pstrCorrelativo.Split(new char[] { '-' });

                    if (Separacion.Count() == 2)
                    {
                        var Nro = Separacion[0];
                        var Letras = dbContext.letrasdetalle
                            .Where(p => p.v_Correlativo.Contains(Nro) && p.v_Serie == pstrSerie && p.i_Eliminado == 0)
                            .ToList()
                            .Select(o => int.Parse(o.v_Correlativo.Split(new char[] { '-' })[1])).ToList();

                        pobjOperationResult.Success = 1;

                        return Letras.Max();
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasBL.ObtenerUltimoCorrelativoParaRenovacion()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public bool ExisteNroLetra(ref OperationResult pobjOperationResult, string pstrCorrelativo, string pstrSerie, string idLetra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    bool result;
                    if (string.IsNullOrEmpty(idLetra))
                        result = dbContext.letrasdetalle.Any(p => p.v_Serie.Trim() == pstrSerie.Trim()
                                                            && p.v_Correlativo.Trim() == pstrCorrelativo.Trim() && p.i_Eliminado == 0);
                    else
                        result = dbContext.letrasdetalle.Any(p => p.v_Serie.Trim() == pstrSerie.Trim() && p.v_IdLetrasDetalle != idLetra
                                                            && p.v_Correlativo.Trim() == pstrCorrelativo.Trim() && p.i_Eliminado == 0);

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasBL.ExisteNroLetra()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        #endregion

        #region Saldo Inicial Letras

        public List<clienteDto> ObtenerClientesSaldosIniciales(ref OperationResult pobjOperationResult,
            string pstrIdCliente = null)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<clienteDto> Resultado = new List<clienteDto>();
                    var Consulta = (from n in dbContext.letrasdetalle
                                    join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_EsSaldoInicial == 1 && n.i_Eliminado == 0
                                    select new clienteDto
                                    {
                                        v_IdCliente = J1.v_IdCliente,
                                        v_CodCliente = J1.v_CodCliente,
                                        v_NroDocIdentificacion = J1.v_NroDocIdentificacion
                                    }
                        ).ToList();

                    var gConsulta = Consulta.GroupBy(p => p.v_IdCliente).ToList();

                    foreach (var g in gConsulta)
                    {
                        clienteDto c = new clienteDto();
                        c = g.FirstOrDefault();
                        Resultado.Add(c);
                    }

                    if (!string.IsNullOrEmpty(pstrIdCliente))
                        Resultado = Resultado.Where(p => p.v_IdCliente == pstrIdCliente).ToList();

                    pobjOperationResult.Success = 1;
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerClientesSaldosIniciales()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<letrasdetalleDto> ObtenerSaldoInicialLetrasDetalle(ref OperationResult pobjOperationResult,
            string pstrIdCliente)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var COnsulta = letrasdetalleAssembler.ToDTOs(dbContext.letrasdetalle
                        .Where(p => p.v_IdCliente == pstrIdCliente && p.i_Eliminado == 0 && p.i_EsSaldoInicial == 1)
                        .ToList());

                    pobjOperationResult.Success = 1;
                    return new BindingList<letrasdetalleDto>(COnsulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasDetalle()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizarSaldoInicialLetras(ref OperationResult pobjOperationResult,
            List<letrasdetalleDto> pTempInsertarLetras, List<letrasdetalleDto> pTempModificarLetras,
            List<letrasdetalleDto> pTempEliminarLetras, List<string> clientSession)
        {
            var mensajeErrorDetallado = new StringBuilder();
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBl = new SecuentialBL();
                        int intNodeId = int.Parse(clientSession[0]);

                        #region Inserta Letras Detalle

                        foreach (var letraDetalle in pTempInsertarLetras)
                        {
                            var letrasdetalle = letraDetalle.ToEntity();
                            var secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 73);
                            var newIdLetrasDetalle = Utils.GetNewId(int.Parse(clientSession[0]), secuentialId, "LC");
                            letrasdetalle.v_IdLetrasDetalle = newIdLetrasDetalle;
                            letrasdetalle.t_InsertaFecha = DateTime.Now;
                            letrasdetalle.i_InsertaIdUsuario = Int32.Parse(clientSession[2]);
                            letrasdetalle.i_Eliminado = 0;
                            letrasdetalle.i_EsSaldoInicial = 1;
                            letraDetalle.v_IdLetrasDetalle = newIdLetrasDetalle;
                            dbContext.AddToletrasdetalle(letrasdetalle);
                            mensajeErrorDetallado = new StringBuilder();
                            mensajeErrorDetallado.AppendLine(letrasdetalle.v_IdCliente);
                            mensajeErrorDetallado.AppendLine(newIdLetrasDetalle);
                            mensajeErrorDetallado.AppendLine(letrasdetalle.d_Importe.ToString());
                            dbContext.SaveChanges();
                            RegistraPendienteCobranzaLetra(ref pobjOperationResult,
                                letrasdetalle.ToDTO(), clientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Modificar Letras Detalle

                        foreach (var letrasDetalle in pTempModificarLetras)
                        {
                            var query = dbContext.letrasdetalle
                                .FirstOrDefault(p => p.v_IdLetrasDetalle == letrasDetalle.v_IdLetrasDetalle);

                            var letrasdetalle = letrasDetalle.ToEntity();
                            letrasdetalle.t_ActualizaFecha = DateTime.Now;
                            letrasdetalle.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                            dbContext.letrasdetalle.ApplyCurrentValues(letrasdetalle);

                            EliminaPendienteCobranzaLetra(ref pobjOperationResult, letrasdetalle.v_IdLetrasDetalle, clientSession);
                            if (pobjOperationResult.Success == 0) return;
                            RegistraPendienteCobranzaLetra(ref pobjOperationResult, letrasdetalle.ToDTO(), clientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Eliminar Letras Detalle

                        foreach (var letrasDetalle in pTempEliminarLetras)
                        {
                            var query = dbContext.letrasdetalle
                                .FirstOrDefault(p => p.v_IdLetrasDetalle == letrasDetalle.v_IdLetrasDetalle);

                            var letrasdetalle = letrasDetalle.ToEntity();
                            letrasdetalle.t_ActualizaFecha = DateTime.Now;
                            letrasdetalle.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                            letrasdetalle.i_Eliminado = 1;
                            dbContext.letrasdetalle.ApplyCurrentValues(letrasdetalle);
                            EliminaPendienteCobranzaLetra(ref pobjOperationResult, letrasdetalle.v_IdLetrasDetalle, clientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ActualizarSaldoInicialLetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + Environment.NewLine + mensajeErrorDetallado;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarSaldoInicial(ref OperationResult pobjOperationResult, string pstrIdCliente,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var SaldosIniciales = dbContext.letrasdetalle
                            .Where(p => p.v_IdCliente == pstrIdCliente && p.i_Eliminado == 0 && p.i_EsSaldoInicial == 1)
                            .ToList();

                        #region Elimina Canjeo Letras Detalle

                        foreach (var item in SaldosIniciales)
                        {
                            item.t_ActualizaFecha = DateTime.Now;
                            item.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            item.i_Eliminado = 1;
                            dbContext.letrasdetalle.ApplyCurrentValues(item);
                            EliminaPendienteCobranzaLetra(ref pobjOperationResult, item.v_IdLetrasDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.EliminarSaldoInicial()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion

        #region Procesos

        private void RestauraDetalleCobranza(ref OperationResult pobjOperationResult, letrascanjeDto cobranzadetalleDto,
            letrasDto CobranzaEntity, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    if (cobranzadetalleDto.v_IdVenta != null)
                    {
                        #region Actualiza la deuda de una venta

                        var cps =
                            dbContext.cobranzapendiente.Where(
                                p => p.v_IdVenta == cobranzadetalleDto.v_IdVenta && p.i_Eliminado == 0).ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar

                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.venta
                                                     where m.v_IdVenta == cp.v_IdVenta && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                switch (CobranzaEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value -
                                                               (cobranzadetalleDto.d_MontoCanjeado /
                                                                VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value +
                                                             (cobranzadetalleDto.d_MontoCanjeado /
                                                              VentaOriginal.d_TipoCambio) >= 0
                                                    ? cp.d_Saldo.Value +
                                                      (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio)
                                                    : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value -
                                                               (cobranzadetalleDto.d_MontoCanjeado *
                                                                VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value +
                                                             (cobranzadetalleDto.d_MontoCanjeado *
                                                              VentaOriginal.d_TipoCambio) >= 0
                                                    ? cp.d_Saldo.Value +
                                                      (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio)
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;
                                        }
                                        break;
                                }

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION,
                                    Globals.ClientSession.v_UserName, "cobranzapendiente", cp.v_IdCobranzaPendiente);
                            }

                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        #region Actualiza la deuda de una letra

                        var cps =
                            dbContext.cobranzaletraspendiente.Where(
                                p => p.v_IdLetrasDetalle == cobranzadetalleDto.v_IdLetrasDetalle && p.i_Eliminado == 0)
                                .ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar

                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.letrasdetalle
                                                     where m.v_IdLetrasDetalle == cp.v_IdLetrasDetalle && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                switch (CobranzaEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value -
                                                               (cobranzadetalleDto.d_MontoCanjeado /
                                                                VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value +
                                                             (cobranzadetalleDto.d_MontoCanjeado /
                                                              VentaOriginal.d_TipoCambio) >= 0
                                                    ? cp.d_Saldo.Value +
                                                      (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio)
                                                    : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value -
                                                               (cobranzadetalleDto.d_MontoCanjeado *
                                                                VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value +
                                                             (cobranzadetalleDto.d_MontoCanjeado *
                                                              VentaOriginal.d_TipoCambio) >= 0
                                                    ? cp.d_Saldo.Value +
                                                      (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio)
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;
                                        }
                                        break;
                                }

                                if (cp.d_Saldo == 0) cp.letrasdetalle.i_Pagada = 1;
                                else cp.letrasdetalle.i_Pagada = 0;

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION,
                                    Globals.ClientSession.v_UserName, "cobranzaletraspendiente",
                                    cp.v_IdCobranzaLetrasPendiente);
                            }

                            #endregion
                        }

                        #endregion
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RestauraDetalleCobranza()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        private void ProcesaDetalleCobranza(ref OperationResult pobjOperationResult, letrascanjeDto cobranzadetalleDto,
            letrasDto CobranzaEntity, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (cobranzadetalleDto.v_IdVenta != null)
                    {
                        #region Actualiza la deuda de una venta

                        var cps =
                            dbContext.cobranzapendiente.Where(
                                p => p.v_IdVenta == cobranzadetalleDto.v_IdVenta && p.i_Eliminado == 0).ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar

                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.venta
                                                     where m.v_IdVenta == cp.v_IdVenta && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                decimal TCambio = VentaOriginal.d_TipoCambio.Value;

                                switch (CobranzaEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value +
                                                               (cobranzadetalleDto.d_MontoCanjeado / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value -
                                                             (cobranzadetalleDto.d_MontoCanjeado / TCambio) >= 0
                                                    ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio)
                                                    : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value +
                                                               (cobranzadetalleDto.d_MontoCanjeado * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value -
                                                             (cobranzadetalleDto.d_MontoCanjeado * TCambio) >= 0
                                                    ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio)
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;
                                        }
                                        break;
                                }

                                dbContext.cobranzapendiente.ApplyCurrentValues(cp);

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION,
                                    Globals.ClientSession.v_UserName, "cobranzapendiente", cp.v_IdCobranzaPendiente);
                            }

                            #endregion
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                            pobjOperationResult.AdditionalInformation = "LetrasBL.ProcesaDetalleCobranza()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }

                        #endregion
                    }
                    else
                    {
                        #region Actualiza la deuda de una letra

                        var cps =
                            dbContext.cobranzaletraspendiente.Where(
                                p => p.v_IdLetrasDetalle == cobranzadetalleDto.v_IdLetrasDetalle && p.i_Eliminado == 0)
                                .ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar

                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.letrasdetalle
                                                     where m.v_IdLetrasDetalle == cp.v_IdLetrasDetalle && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                decimal TCambio = VentaOriginal.d_TipoCambio.Value;

                                switch (CobranzaEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value +
                                                               (cobranzadetalleDto.d_MontoCanjeado / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value -
                                                             (cobranzadetalleDto.d_MontoCanjeado / TCambio) >= 0
                                                    ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio)
                                                    : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value +
                                                               (cobranzadetalleDto.d_MontoCanjeado * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value -
                                                             (cobranzadetalleDto.d_MontoCanjeado * TCambio) >= 0
                                                    ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio)
                                                    : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0
                                                    ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado
                                                    : 0;

                                                break;
                                        }
                                        break;
                                }

                                if (cp.d_Saldo == 0) cp.letrasdetalle.i_Pagada = 1;
                                else cp.letrasdetalle.i_Pagada = 0;

                                dbContext.cobranzaletraspendiente.ApplyCurrentValues(cp);

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION,
                                    Globals.ClientSession.v_UserName, "cobranzaletraspendiente",
                                    cp.v_IdCobranzaLetrasPendiente);
                            }

                            #endregion
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                            pobjOperationResult.AdditionalInformation = "LetrasBL.ProcesaDetalleCobranza()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }

                        #endregion
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ProcesaDetalleCobranza()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private void RegistraPendienteCobranzaLetra(ref OperationResult pobjOperationResult,
            letrasdetalleDto _letrasdetalle, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    cobranzaletraspendiente objEntityLetras = new cobranzaletraspendiente();
                    int SecuentialId = 0;
                    string newIdLetras = string.Empty;
                    int intNodeId;

                    objEntityLetras.t_InsertaFecha = DateTime.Now;
                    objEntityLetras.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityLetras.i_Eliminado = 0;
                    objEntityLetras.v_IdLetrasDetalle = _letrasdetalle.v_IdLetrasDetalle;
                    objEntityLetras.d_Acuenta = 0;
                    objEntityLetras.d_Saldo = _letrasdetalle.d_Importe;

                    intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 76);
                    newIdLetras = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CZ");
                    objEntityLetras.v_IdCobranzaLetrasPendiente = newIdLetras;
                    dbContext.AddTocobranzaletraspendiente(objEntityLetras);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RegistraPendienteCobranzaLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private void EliminaPendienteCobranzaLetra(ref OperationResult pobjOperationResult, string pstrIdLetraDetalle,
            List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var CobranzaPendiente =
                        dbContext.cobranzaletraspendiente.FirstOrDefault(p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0);

                    if (CobranzaPendiente != null)
                    {
                        CobranzaPendiente.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        CobranzaPendiente.t_ActualizaFecha = DateTime.Now;
                        CobranzaPendiente.i_Eliminado = 1;
                        dbContext.cobranzaletraspendiente.ApplyCurrentValues(CobranzaPendiente);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RestauraPendienteCobranzaLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public bool LetraFueCobrada(ref OperationResult pobjOperationResult, string pstrIdLetras)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Letras =
                        dbContext.letrasdetalle.Where(p => p.v_IdLetras == pstrIdLetras && p.i_Eliminado == 0).ToList();
                    pobjOperationResult.Success = 1;

                    foreach (var Letra in Letras)
                    {
                        if (
                            dbContext.cobranzaletraspendiente.Count(
                                p =>
                                    p.v_IdLetrasDetalle == Letra.v_IdLetrasDetalle && p.i_Eliminado == 0 &&
                                    p.d_Acuenta.Value > 0) > 0)
                        {
                            return true;
                        }

                        return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RestauraPendienteCobranzaLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool LetraDetalleFueCobrada(ref OperationResult pobjOperationResult, string pstrIdLetraDetalle)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (
                        dbContext.cobranzaletraspendiente.Count(
                            p =>
                                p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0 && p.d_Acuenta.Value > 0) >
                        0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RestauraPendienteCobranzaLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool SaldoInicialFueCobrado(ref OperationResult pobjOperationResult, string pstrIdCliente)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Letras =
                        dbContext.letrasdetalle.Where(
                            p => p.v_IdCliente == pstrIdCliente && p.i_Eliminado == 0 && p.i_EsSaldoInicial == 1)
                            .ToList();
                    pobjOperationResult.Success = 1;

                    foreach (var Letra in Letras)
                    {
                        if (
                            dbContext.cobranzaletraspendiente.Count(
                                p =>
                                    p.v_IdLetrasDetalle == Letra.v_IdLetrasDetalle && p.i_Eliminado == 0 &&
                                    p.d_Acuenta.Value > 0) > 0)
                        {
                            return true;
                        }

                        return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.SaldoInicialFueCobrado()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        #endregion

        #region Reportes

        public List<ReporteLetrasPendientesCobranzaDto> ReporteLetrasPendientesCobranza(
            ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string IdCliente, string Agrupar, string EstadoLetra, string UbicacionLetra)
        {
            try
            {
                F_Ini = F_Ini.Date;
                F_Fin = DateTime.Parse(F_Fin.Day + "/" + F_Fin.Month + "/" + F_Fin.Year + " 23:59");

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<ReporteLetrasPendientesCobranzaDto> ListaNueva = new List<ReporteLetrasPendientesCobranzaDto>();
                    var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                    var LetrasCanje = dbContext.letrascanje.Where(p => p.i_Eliminado == 0).ToList();
                    var LetrasDetalle = dbContext.letrasdetalle.Where(o => o.i_Eliminado == 0).ToList();
                    var Ventas = dbContext.venta.Where(i => i.i_Eliminado == 0).ToList();
                    var Consulta = (from n in dbContext.letrasdetalle
                                    join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.cobranzaletraspendiente on
                                        new { IdLetrasDetalle = n.v_IdLetrasDetalle, Eliminado = 0 }
                                        equals new { IdLetrasDetalle = J2.v_IdLetrasDetalle, Eliminado = J2.i_Eliminado.Value } into
                                        J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.documento on new { tipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tipoDoc = J3.i_CodigoDocumento, eliminado = J3.i_Eliminado.Value } into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.i_Pagada != 1 && n.t_FechaVencimiento >= F_Ini && n.t_FechaVencimiento <= F_Fin

                                    && J2.d_Saldo > 0
                                    && (n.v_IdCliente == IdCliente || IdCliente == "")

                                    select new
                                    {
                                        NroDocLetra = n.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : n.v_Serie + "-" + n.v_Correlativo,
                                        d_Importe = J2.d_Saldo.Value,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdCliente,
                                        v_IdLetras = n.v_IdLetras,
                                        v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                        t_FechaEmision = n.t_FechaEmision,
                                        v_Serie = n.v_Serie,
                                        t_FechaVencimiento = n.t_FechaVencimiento,
                                        Moneda = n.i_IdMoneda.Value == 1 ? "S" : "D",
                                        Cliente =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        ImporteSoles = n.i_IdMoneda == 1 ? J2.d_Saldo.Value : 0,
                                        ImporteDolares = n.i_IdMoneda == 2 ? J2.d_Saldo.Value : 0,
                                        RucCliente = J1.v_NroDocIdentificacion,
                                        Letra = J3.v_Siglas + " " + n.v_Serie + " " + n.v_Correlativo,

                                    }
                        ).ToList().Select(n =>
                        {

                            var ubicacion = ObtenerUltimaUbicacionLetra(n.v_IdLetrasDetalle, tempDs);
                            var DondeGenero = BuscarLetrasCanjeDocumentoFecha(n.v_IdLetras, LetrasCanje, Ventas, LetrasDetalle);
                            var UbicacionSololetras = ObtenerSoloLetrasUbicacion(ubicacion.Ubicacion);
                            return new ReporteLetrasPendientesCobranzaDto
                            {
                                Moneda = n.Moneda,
                                Estado = ubicacion.Estado,
                                Ubicacion = ubicacion.Ubicacion,
                                NroUnico = ubicacion.v_NroUnico,
                                FechaEmision = n.t_FechaEmision.Value.ToShortDateString(),
                                FechaVencimiento = n.t_FechaVencimiento.Value.ToShortDateString(),
                                Importe = n.d_Importe,
                                //Letra = "LEC " + n.v_Serie + "-" + n.v_Correlativo,
                                Letra = n.Letra,
                                NombreRazonSocial = n.Cliente,
                                v_IdCliente = n.v_IdCliente,
                                ImporteSoles = n.ImporteSoles,
                                ImporteDolares = n.ImporteDolares,
                                Facturas = DondeGenero,
                                UbicacionSoloLetras = UbicacionSololetras,
                                RucCliente = n.RucCliente,
                                DFechaVencimiento = n.t_FechaVencimiento.Value,
                                Agrupamiento = Agrupar == "CLIENTE" ? n.Cliente : Agrupar == "ESTADO" ? ubicacion.Estado : Agrupar == "UBICACIÓN" ? UbicacionLetra : "",
                                AgrupamietoFecha = Agrupar == "FEC. VENCIMIENTO" ? n.t_FechaVencimiento.Value : DateTime.Parse("01/01/1000"),
                            };
                        }
                        ).AsQueryable();

                    if (EstadoLetra != "-1") Consulta = Consulta.Where(p => p.Estado == EstadoLetra);
                    if (UbicacionLetra != "-1") Consulta = Consulta.Where(p => p.Ubicacion == UbicacionLetra);

                    pobjOperationResult.Success = 1;
                    if (Agrupar == "CLIENTE")
                    {

                        return Consulta.ToList().OrderBy(l => l.NombreRazonSocial).ThenBy(l => l.DFechaVencimiento).ToList();
                    }
                    else if (Agrupar == "ESTADO")
                    {
                        return Consulta.ToList().OrderBy(l => l.Estado).ThenBy(l => l.DFechaVencimiento).ToList();
                    }
                    else if (Agrupar == "UBICACIÓN")
                    {
                        return Consulta.ToList().OrderBy(l => l.Ubicacion).ThenBy(l => l.DFechaVencimiento).ToList();
                    }
                    else if (Agrupar == "FEC. VENCIMIENTO")
                    {
                        return Consulta.ToList().OrderBy(l => l.DFechaVencimiento).ToList();
                    }

                    else
                    {
                        // return Consulta.ToList().OrderBy(l => l.FechaVencimiento).ToList();

                        return ListaNueva = Consulta.ToList().OrderBy(l => l.DFechaVencimiento).ToList();
                    }





                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ReporteLetrasPendientesCobranza()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }







        public List<ReporteLetrasPendientesPagoDto> ReporteLetrasPendientesPago(
         ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string IdProveedor, string Agrupar, string EstadoLetra, string UbicacionLetra)
        {
            try
            {
                F_Ini = F_Ini.Date;
                F_Fin = DateTime.Parse(F_Fin.Day + "/" + F_Fin.Month + "/" + F_Fin.Year + " 23:59");

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<ReporteLetrasPendientesPagoDto> ListaNueva = new List<ReporteLetrasPendientesPagoDto>();
                    var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();

                    var LetrasCanje = dbContext.letraspagarcanje.Where(p => p.i_Eliminado == 0).ToList();
                    var LetrasDetalle = dbContext.letraspagardetalle.Where(o => o.i_Eliminado == 0).ToList();
                    var Compras = dbContext.compra.Where(i => i.i_Eliminado == 0).ToList();

                    var Consulta = (from n in dbContext.letraspagardetalle
                                    join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.cobranzaletraspagarpendiente on new { IdLetrasDetalle = n.v_IdLetrasPagarDetalle, Eliminado = 0 } equals new { IdLetrasDetalle = J2.v_IdLetrasPagarDetalle, Eliminado = J2.i_Eliminado.Value } into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.documento on new { tipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tipoDoc = J3.i_CodigoDocumento, eliminado = J3.i_Eliminado.Value } into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.i_Pagada != 1 && n.t_FechaVencimiento >= F_Ini && n.t_FechaVencimiento <= F_Fin
                                    && (n.v_IdProveedor == IdProveedor || IdProveedor == "")

                                    select new
                                    {
                                        NroDocLetra = n.v_Serie + "-" + n.v_Correlativo,
                                        d_Importe = J2.d_Saldo.Value,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdProveedor,
                                        v_IdLetras = n.v_IdLetrasPagar,
                                        v_IdLetrasDetalle = n.v_IdLetrasPagarDetalle,
                                        t_FechaEmision = n.t_FechaEmision,
                                        v_Serie = n.v_Serie,
                                        t_FechaVencimiento = n.t_FechaVencimiento,
                                        Moneda = n.i_IdMoneda.Value == 1 ? "S" : "D",
                                        Cliente =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        ImporteSoles = n.i_IdMoneda == 1 ? J2.d_Saldo.Value : 0,
                                        ImporteDolares = n.i_IdMoneda == 2 ? J2.d_Saldo.Value : 0,
                                        RucCliente = J1.v_NroDocIdentificacion,
                                        Letra = J3.v_Siglas + " " + n.v_Serie + "-" + n.v_Correlativo,


                                    }
                        ).ToList().Select(n =>
                        {

                            var ubicacion = ObtenerUltimaUbicacionLetraPagar(n.v_IdLetrasDetalle, tempDs);
                            var DondeGenero = BuscarLetrasPagarCanjeDocumentoFecha(n.v_IdLetras, LetrasCanje, Compras, LetrasDetalle);
                            var UbicacionSololetras = ObtenerSoloLetrasPagarUbicacion(ubicacion.Ubicacion);
                            return new ReporteLetrasPendientesPagoDto
                            {
                                Moneda = n.Moneda,
                                Estado = ubicacion.Estado,
                                Ubicacion = ubicacion.Ubicacion,
                                NroUnico = ubicacion.v_NroUnico,
                                FechaEmision = n.t_FechaEmision.Value.ToShortDateString(),
                                FechaVencimiento = n.t_FechaVencimiento.Value.ToShortDateString(),
                                Importe = n.d_Importe,
                                //Letra = "LEC " + n.v_Serie + "-" + n.v_Correlativo,
                                Letra = n.Letra,
                                NombreRazonSocial = n.Cliente,
                                v_IdCliente = n.v_IdCliente,
                                ImporteSoles = n.ImporteSoles,
                                ImporteDolares = n.ImporteDolares,
                                Facturas = DondeGenero,
                                UbicacionSoloLetras = UbicacionSololetras,
                                RucCliente = n.RucCliente,
                                DFechaVencimiento = n.t_FechaVencimiento.Value,
                                Agrupamiento = Agrupar == "PROVEEDOR" ? n.Cliente : Agrupar == "ESTADO" ? ubicacion.Estado : Agrupar == "UBICACIÓN" ? UbicacionLetra : "",
                                AgrupamietoFecha = Agrupar == "FEC. VENCIMIENTO" ? n.t_FechaVencimiento.Value : DateTime.Parse("01/01/1000"),
                            };
                        }
                        ).AsQueryable();

                    if (EstadoLetra != "-1") Consulta = Consulta.Where(p => p.Estado == EstadoLetra);
                    if (UbicacionLetra != "-1") Consulta = Consulta.Where(p => p.Ubicacion == UbicacionLetra);

                    pobjOperationResult.Success = 1;
                    if (Agrupar == "CLIENTE")
                    {

                        return Consulta.ToList().OrderBy(l => l.NombreRazonSocial).ThenBy(l => l.DFechaVencimiento).ToList();
                    }
                    else if (Agrupar == "ESTADO")
                    {
                        return Consulta.ToList().OrderBy(l => l.Estado).ThenBy(l => l.DFechaVencimiento).ToList();
                    }
                    else if (Agrupar == "UBICACIÓN")
                    {
                        return Consulta.ToList().OrderBy(l => l.Ubicacion).ThenBy(l => l.DFechaVencimiento).ToList();
                    }
                    else if (Agrupar == "FEC. VENCIMIENTO")
                    {
                        return Consulta.ToList().OrderBy(l => l.DFechaVencimiento).ToList();
                    }

                    else
                    {
                        // return Consulta.ToList().OrderBy(l => l.FechaVencimiento).ToList();

                        return ListaNueva = Consulta.ToList().OrderBy(l => l.DFechaVencimiento).ToList();
                    }





                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ReporteLetrasPendientesCobranza()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }







        public string BuscarLetrasCanjeDocumentoFecha(string IdLetra, IEnumerable<letrascanje> LetrasCanje, IEnumerable<venta> Venta, IEnumerable<letrasdetalle> LetrasDetalle)
        {

            string DocumentoDedondesegenero = string.Empty;

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var DocumentosqueGeneraron = LetrasCanje.Where(o => o.v_IdLetras == IdLetra && o.i_Eliminado == 0).ToList();
                int NumeroRegistros = DocumentosqueGeneraron.Count();
                int Contador = 1;
                decimal ImporteDetalle = 0;
                decimal CanjeadoDetalle = 0;
                foreach (var item in DocumentosqueGeneraron)
                {


                    if (item.v_IdVenta != null)
                    {

                        var docVenta = Venta.Where(o => o.v_IdVenta == item.v_IdVenta && o.i_Eliminado == 0).FirstOrDefault();
                        if (docVenta != null)
                        {
                            var doc = dbContext.documento.Where(o => o.i_CodigoDocumento == docVenta.i_IdTipoDocumento.Value).FirstOrDefault();
                            DocumentoDedondesegenero = DocumentoDedondesegenero + doc.v_Siglas + " " + docVenta.v_SerieDocumento + " " + docVenta.v_CorrelativoDocumento + " FECHA : " + docVenta.t_FechaRegistro.Value.ToShortDateString() + " ";

                        }

                    }
                    if (item.v_IdLetrasDetalle != null)
                    {
                        var docLetras = LetrasDetalle.Where(o => o.v_IdLetrasDetalle == item.v_IdLetrasDetalle && o.i_Eliminado == 0).FirstOrDefault();
                        var doc = dbContext.documento.Where(o => o.i_CodigoDocumento == docLetras.i_IdTipoDocumento.Value).FirstOrDefault();
                        if (docLetras != null)
                        {

                            DocumentoDedondesegenero = DocumentoDedondesegenero + doc.v_Siglas + " " + docLetras.v_Serie + " " + docLetras.v_Correlativo + " FECHA : " + docLetras.t_FechaEmision.Value.ToShortDateString() + " ";

                        }

                    }
                    Contador = Contador + 1;
                }

                return DocumentoDedondesegenero;

            }


        }


        public string BuscarLetrasPagarCanjeDocumentoFecha(string IdLetra, IEnumerable<letraspagarcanje> LetrasCanje, IEnumerable<compra> Compra, IEnumerable<letraspagardetalle> LetrasDetalle)
        {

            string DocumentoDedondesegenero = string.Empty;

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var DocumentosqueGeneraron = LetrasCanje.Where(o => o.v_IdLetrasPagar == IdLetra && o.i_Eliminado == 0).ToList();
                int NumeroRegistros = DocumentosqueGeneraron.Count();
                int Contador = 1;
                decimal ImporteDetalle = 0;
                decimal CanjeadoDetalle = 0;
                foreach (var item in DocumentosqueGeneraron)
                {


                    if (item.v_IdCompra != null)
                    {

                        var docCompra = Compra.Where(o => o.v_IdCompra == item.v_IdCompra && o.i_Eliminado == 0).FirstOrDefault();
                        if (docCompra != null)
                        {
                            var doc = dbContext.documento.Where(o => o.i_CodigoDocumento == docCompra.i_IdTipoDocumento.Value).FirstOrDefault();
                            DocumentoDedondesegenero = DocumentoDedondesegenero + doc.v_Siglas + " " + docCompra.v_SerieDocumento + " " + docCompra.v_CorrelativoDocumento + " FECHA : " + docCompra.t_FechaRegistro.Value.ToShortDateString() + " ";

                        }

                    }
                    if (item.v_IdLetrasPagarDetalle != null)
                    {
                        var docLetras = LetrasDetalle.Where(o => o.v_IdLetrasPagarDetalle == item.v_IdLetrasPagarDetalle && o.i_Eliminado == 0).FirstOrDefault();
                        var doc = dbContext.documento.Where(o => o.i_CodigoDocumento == docLetras.i_IdTipoDocumento.Value).FirstOrDefault();
                        if (docLetras != null)
                        {

                            DocumentoDedondesegenero = DocumentoDedondesegenero + doc.v_Siglas + " " + docLetras.v_Serie + " " + docLetras.v_Correlativo + " FECHA : " + docLetras.t_FechaEmision.Value.ToShortDateString() + " ";

                        }

                    }
                    Contador = Contador + 1;
                }

                return DocumentoDedondesegenero;

            }


        }

        public List<ReporteLetrasPendientesCobranzaDto> ReporteLetrasSaldoInicial(
            ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string pstrFilterExpression)
        {
            try
            {
                F_Ini = F_Ini.Date;
                F_Fin =
                    DateTime.Parse(F_Fin.Day.ToString() + "/" + F_Fin.Month.ToString() + "/" + F_Fin.Year.ToString() +
                                   " 23:59");

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                    var Consulta = (from n in dbContext.letrasdetalle
                                    join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.cobranzaletraspendiente on
                                        new { IdLetrasDetalle = n.v_IdLetrasDetalle, Eliminado = 0 }
                                        equals new { IdLetrasDetalle = J2.v_IdLetrasDetalle, Eliminado = J2.i_Eliminado.Value } into
                                        J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.i_EsSaldoInicial == 1
                                    select new
                                    {
                                        NroDocLetra = n.v_Serie + "-" + n.v_Correlativo,
                                        d_Importe = J2.d_Saldo.Value,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdCliente = n.v_IdCliente,
                                        v_IdLetras = n.v_IdLetras,
                                        v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                        t_FechaEmision = n.t_FechaEmision,
                                        v_Serie = n.v_Serie,
                                        t_FechaVencimiento = n.t_FechaVencimiento,
                                        Moneda = n.i_IdMoneda.Value == 1 ? "S" : "D",
                                        Cliente =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim()
                                    }
                        ).ToList().Select(n =>
                        {
                            var ubicacion = ObtenerUltimaUbicacionLetra(n.v_IdLetrasDetalle, tempDs);
                            return new ReporteLetrasPendientesCobranzaDto
                            {
                                Moneda = n.Moneda,
                                Estado = ubicacion.Estado,
                                Ubicacion = ubicacion.Ubicacion,
                                NroUnico = ubicacion.v_NroUnico,
                                FechaEmision = n.t_FechaEmision.Value.ToShortDateString(),
                                FechaVencimiento = n.t_FechaVencimiento.Value.ToShortDateString(),
                                Importe = n.d_Importe,
                                Letra = "LEC " + n.v_Serie + "-" + n.v_Correlativo,
                                NombreRazonSocial = n.Cliente,
                                v_IdCliente = n.v_IdCliente
                            };
                        }
                        ).AsQueryable();

                    if (!string.IsNullOrEmpty(pstrFilterExpression)) Consulta = Consulta.Where(pstrFilterExpression);

                    pobjOperationResult.Success = 1;
                    return Consulta.ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ReporteLetrasPendientesCobranza()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<historialpagosventaDto> BuscarHistorialPagos(ref OperationResult pobjOperationResult,
            string pstrIdLetra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Query Buscar Cobranzas
                    var queryCobranzas = (from n in dbContext.cobranzadetalle
                                          join J1 in dbContext.cobranza on n.v_IdCobranza equals J1.v_IdCobranza into J1_join
                                          from J1 in J1_join.DefaultIfEmpty()

                                          join J4 in dbContext.documento on new { i_IdTipoDocumento = J1.i_IdTipoDocumento.Value }
                                              equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                          from J4 in J4_join.DefaultIfEmpty()
                                          where n.v_IdVenta == pstrIdLetra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                          select new historialpagosventaDto
                                          {
                                              TipoDocumento = J4.v_Siglas,
                                              NroDocumento = J1.v_Mes.Trim() + "-" + J1.v_Correlativo,
                                              IdDocumento = J1.v_IdCobranza,
                                              Glosa = J1.v_Glosa,
                                              TipoCambio = J1.d_TipoCambio.Value,
                                              Fecha = J1.t_FechaRegistro.Value,
                                              Moneda = J1.i_IdMoneda == 1 ? "Soles" : "Dólares",
                                              EsLetra = false,
                                              Pago = n.d_ImporteSoles.Value,
                                              SaldoLetra = 0,
                                              Estado = J1.i_IdEstado ?? 0,
                                          }).ToList();
                    #endregion

                    #region Query Buscar Canjes a Letras

                    var queryLetras = (from n in dbContext.letrasdetalle
                                       join J1 in dbContext.letrascanje on n.v_IdLetras equals J1.v_IdLetras into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()
                                       join J2 in dbContext.letras on J1.v_IdLetras equals J2.v_IdLetras into J2_join
                                       from J2 in J2_join.DefaultIfEmpty()
                                       join J3 in dbContext.cobranzaletraspendiente on n.v_IdLetrasDetalle equals J3.v_IdLetrasDetalle into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()
                                       join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                           equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()
                                       where J1.v_IdVenta == pstrIdLetra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                       select new historialpagosventaDto
                                       {
                                           TipoDocumento = J4.v_Siglas,
                                           NroDocumento = J2.v_Mes.Trim() + "-" + J2.v_Correlativo,
                                           IdDocumento = n.v_IdLetrasDetalle,
                                           TipoCambio = J2.d_TipoCambio.Value,
                                           Glosa = "*C A N J E  A  L E T R A S*",
                                           Fecha = J2.t_FechaRegistro.Value,
                                           Moneda = J2.i_IdMoneda == 1 ? "Soles" : "Dólares",
                                           EsLetra = true,
                                           Pago = n.d_Importe.Value,
                                           SaldoLetra = J3.d_Saldo.Value,
                                           Estado = 1,
                                       }).ToList();
                    #endregion

                    pobjOperationResult.Success = 1;
                    return queryCobranzas.Concat(queryLetras).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.BuscarHistorialPagos()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }



        public List<historialpagoscompraDto> BuscarHistorialPagosCompra(ref OperationResult pobjOperationResult,
         string pstrIdLetra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Query Buscar Cobranzas
                    var queryCobranzas = (from n in dbContext.pagodetalle
                                          join J1 in dbContext.pago on n.v_IdPago equals J1.v_IdPago into J1_join
                                          from J1 in J1_join.DefaultIfEmpty()

                                          join J4 in dbContext.documento on new { i_IdTipoDocumento = J1.i_IdTipoDocumento.Value }
                                              equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                          from J4 in J4_join.DefaultIfEmpty()
                                          where n.v_IdCompra == pstrIdLetra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                          select new historialpagoscompraDto
                                          {
                                              TipoDocumento = J4.v_Siglas,
                                              NroDocumento = J1.v_Mes.Trim() + "-" + J1.v_Correlativo,
                                              IdDocumento = J1.v_IdPago,
                                              Glosa = J1.v_Glosa,
                                              TipoCambio = J1.d_TipoCambio.Value,
                                              Fecha = J1.t_FechaRegistro.Value,
                                              Moneda = J1.i_IdMoneda == 1 ? "Soles" : "Dólares",
                                              EsLetra = false,
                                              Pago = n.d_ImporteSoles.Value,
                                              SaldoLetra = 0,
                                              Estado = J1.i_IdEstado ?? 0,
                                          }).ToList();
                    #endregion

                    #region Query Buscar Canjes a Letras

                    var queryLetras = (from n in dbContext.letraspagardetalle
                                       join J1 in dbContext.letraspagarcanje on n.v_IdLetrasPagar equals J1.v_IdLetrasPagar into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()
                                       join J2 in dbContext.letraspagar on J1.v_IdLetrasPagar equals J2.v_IdLetrasPagar into J2_join
                                       from J2 in J2_join.DefaultIfEmpty()
                                       join J3 in dbContext.cobranzaletraspagarpendiente on n.v_IdLetrasPagarDetalle equals J3.v_IdLetrasPagarDetalle into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()
                                       join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                           equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()
                                       //where J1.v_IdCompra  == pstrIdLetra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                       where J1.v_IdLetrasPagarDetalle == pstrIdLetra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                       select new historialpagoscompraDto
                                       {
                                           TipoDocumento = J4.v_Siglas,
                                           NroDocumento = J2.v_Mes.Trim() + "-" + J2.v_Correlativo,
                                           IdDocumento = n.v_IdLetrasPagarDetalle,
                                           TipoCambio = J2.d_TipoCambio.Value,
                                           Glosa = "*C A N J E  A  L E T R A S*",
                                           Fecha = J2.t_FechaRegistro.Value,
                                           Moneda = J2.i_IdMoneda == 1 ? "Soles" : "Dólares",
                                           EsLetra = true,
                                           Pago = n.d_Importe.Value,
                                           SaldoLetra = J3.d_Saldo.Value,
                                           Estado = 1,
                                           FechaVencimiento = n.t_FechaVencimiento == null ? DateTime.Now : n.t_FechaVencimiento.Value,
                                       }).ToList();
                    #endregion

                    pobjOperationResult.Success = 1;
                    return queryCobranzas.Concat(queryLetras).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.BuscarHistorialPagosCompra()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }



        public letrasdetalleDto ObtenerLetraDetalleConsulta(ref OperationResult pobjOperationResult,
            string pstrIdLetraDetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letradetalle = (from n in dbContext.letrasdetalle
                                        join J1 in dbContext.letras on n.v_IdLetras equals J1.v_IdLetras into J1_join
                                        from J1 in J1_join.DefaultIfEmpty()

                                        join c in dbContext.cobranzaletraspendiente on new { idletra = n.v_IdLetrasDetalle, eliminado = 0 } equals
                                                                                    new { idletra = c.v_IdLetrasDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                        from c in c_join.DefaultIfEmpty()

                                        where n.v_IdLetrasDetalle == pstrIdLetraDetalle
                                        select new letrasdetalleDto
                                        {
                                            Moneda = J1.i_IdMoneda.Value == 1 ? "Soles" : "Dólares",
                                            t_FechaEmision = J1.t_FechaRegistro.Value,
                                            v_Serie = n.v_Serie,
                                            v_Correlativo = n.v_Correlativo,
                                            t_FechaVencimiento = n.t_FechaVencimiento.Value,
                                            d_Importe = n.d_Importe.Value,
                                            Saldo = c.d_Saldo.Value
                                        }).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return letradetalle;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetraDetalleConsulta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }


        public letraspagardetalleDto ObtenerLetraPagarDetalleConsulta(ref OperationResult pobjOperationResult,
           string pstrIdLetraDetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letradetalle = (from n in dbContext.letraspagardetalle
                                        join J1 in dbContext.letraspagar on n.v_IdLetrasPagar equals J1.v_IdLetrasPagar into J1_join
                                        from J1 in J1_join.DefaultIfEmpty()

                                        join c in dbContext.cobranzaletraspagarpendiente on new { idletra = n.v_IdLetrasPagarDetalle, eliminado = 0 } equals
                                                                                    new { idletra = c.v_IdLetrasPagarDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                        from c in c_join.DefaultIfEmpty()

                                        where n.v_IdLetrasPagarDetalle == pstrIdLetraDetalle
                                        select new letraspagardetalleDto
                                        {
                                            Moneda = J1.i_IdMoneda.Value == 1 ? "Soles" : "Dólares",
                                            t_FechaEmision = J1.t_FechaRegistro.Value,
                                            v_Serie = n.v_Serie,
                                            v_Correlativo = n.v_Correlativo,
                                            t_FechaVencimiento = n.t_FechaVencimiento.Value,
                                            d_Importe = n.d_Importe.Value,
                                            Saldo = c.d_Saldo.Value
                                        }).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return letradetalle;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetraPagarDetalleConsulta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }



        public List<ReporteDocumentoLetra> ReporteDocumentoLetra(ref OperationResult pobjOperationResult,
            string IdLetras, bool ImprimirDni)
        {
            SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {




                    var RucEmpresa = new NodeBL().ReporteEmpresa();
                    var Venta = dbContext.venta.Where(x => x.i_Eliminado == 0);
                    OperationResult objOperationResult = new OperationResult();
                    var ReporteDocumentoLetra = (from a in dbContext.letras

                                                 join c in dbContext.letrasdetalle on new { lc = a.v_IdLetras, eliminado = 0 } equals new { lc = c.v_IdLetras, eliminado = c.i_Eliminado.Value } into c_join
                                                 from c in c_join.DefaultIfEmpty()

                                                 join d in dbContext.documento on new { td = c.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                 from d in d_join.DefaultIfEmpty()

                                                 join e in dbContext.cliente on new { c = a.v_IdCliente, eliminado = 0 } equals new { c = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                                 from e in e_join.DefaultIfEmpty()


                                                 join f in dbContext.avalcliente on new { ac = e.v_IdCliente, eliminado = 0 } equals new { ac = f.v_IdCliente, eliminado = f.i_Eliminado.Value } into f_join
                                                 from f in f_join.DefaultIfEmpty()

                                                 where a.v_IdLetras == IdLetras && a.i_Eliminado == 0


                                                 select new ReporteDocumentoLetra
                                                 {

                                                     NumeroLetra = d.v_Siglas.Trim() + " " + c.v_Serie.Trim() + " " + c.v_Correlativo,
                                                     NroFactura = "",
                                                     FechaEmision = c.t_FechaEmision.Value,
                                                     FechaVencimiento = c.t_FechaVencimiento.Value,
                                                     Importe = c.d_Importe ?? 0,
                                                     Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                                     Empresa = "",
                                                     Cliente = d == null ? "" : (e.v_ApePaterno + " " + e.v_ApeMaterno + " " + e.v_PrimerNombre + " " + e.v_SegundoNombre + " " + e.v_RazonSocial).Trim(),
                                                     DomicilioCliente = d == null ? "" : e.v_DirecPrincipal,
                                                     LocalidadCliente = "",
                                                     NroDocCliente = d == null ? "" : e.v_NroDocIdentificacion == null ? "" : e.v_NroDocIdentificacion,
                                                     TelefonoCliente = d == null ? "" : e.v_TelefonoFijo,
                                                     NombreAval = e == null ? "" : f.v_Nombres,
                                                     DomicilioAval = e == null ? "" : f.v_Direccion,
                                                     LocalidadAval = e == null ? "" : f.v_Localidad,
                                                     NroDocAval = e == null ? "" : f.v_NroDocIdentificacion,
                                                     Grupo = d.v_Siglas.Trim() + " " + c.v_Serie.Trim() + " " + c.v_Correlativo,
                                                     IdMoneda = a.i_IdMoneda.Value,
                                                     TelefonoAval = e == null ? "" : f.v_Telefono,
                                                     IdLetras = a.v_IdLetras,
                                                     IdDepartamento = e.i_IdDepartamento ?? -1,
                                                     IdLetrasDetalle = c.v_IdLetrasDetalle,
                                                     TipoDocumentoPersona = e.i_IdTipoIdentificacion ?? 0,
                                                     TipoPersona = e.i_IdTipoPersona ?? 0,
                                                     NroDocClienteRegistrado = e == null ? "" : e.v_NroDocIdentificacion == null ? "" : e.v_NroDocIdentificacion,
                                                 }).ToList().Select(x =>
                                               {

                                                   letrasDto LetraDto = new letrasDto();

                                                   var IdVenta = (from a in dbContext.letrascanje
                                                                  where a.v_IdLetras == x.IdLetras && a.i_Eliminado == 0

                                                                  select new
                                                                  {
                                                                      IdVenta = a.v_IdVenta
                                                                  }).FirstOrDefault();

                                                   var IdLetrasDetalle = (from a in dbContext.letrascanje
                                                                          where a.i_Eliminado == 0 && a.v_IdLetras == x.IdLetras
                                                                          select new
                                                                          {
                                                                              IdLetrasDetalle = a.v_IdLetrasDetalle
                                                                          }).FirstOrDefault();
                                                   if (IdVenta != null && IdVenta.IdVenta != null)
                                                   {
                                                       LetraDto = (from a in dbContext.venta
                                                                   join b in dbContext.documento on new { v = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { v = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                                                   from b in b_join.DefaultIfEmpty()
                                                                   where a.v_IdVenta == IdVenta.IdVenta && a.i_Eliminado == 0
                                                                   select new letrasDto
                                                                 {
                                                                     NroRegistro = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                                                 }).FirstOrDefault();
                                                   }
                                                   else
                                                   {

                                                       if (IdLetrasDetalle != null && IdLetrasDetalle.IdLetrasDetalle != null)
                                                       {
                                                           LetraDto = (from a in dbContext.letrasdetalle
                                                                       join b in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                                                       from b in b_join.DefaultIfEmpty()
                                                                       where a.v_IdLetrasDetalle == IdLetrasDetalle.IdLetrasDetalle && a.i_Eliminado == 0
                                                                       select new letrasDto
                                                                       {
                                                                           NroRegistro = b.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                       }).FirstOrDefault();
                                                       }

                                                   }

                                                   var Totalletras = x.IdMoneda == (int)Currency.Soles ? Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " SOLES " : SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " SOLES  S.E.U.O" :

                                                   Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " DOLARES AMERICANOS " : SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " DOLARES AMERICANOS S.E.U.O. ";
                                                   var departamento = x.IdDepartamento == -1 ? "" : _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, "").ToList().Where(a => a.Id == x.IdDepartamento.ToString()).FirstOrDefault().Value1.ToString();
                                                   var DocumentoCliente = ImprimirDni ? DevuelveDniPersonaNatural(x.NroDocCliente, x.TipoPersona, x.TipoDocumentoPersona) : x.NroDocCliente;
                                                   return new ReporteDocumentoLetra
                                                   {
                                                       NumeroLetra = x.NumeroLetra ?? "",
                                                       NroFactura = LetraDto != null ? LetraDto.NroRegistro : "",
                                                       FechaEmision = x.FechaEmision,
                                                       FechaVencimiento = x.FechaVencimiento,
                                                       Importe = x.Importe,
                                                       Moneda = x.Moneda ?? "",
                                                       Empresa = RucEmpresa != null ? RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim() : "",
                                                       Cliente = x.Cliente ?? "",
                                                       DomicilioCliente = x.DomicilioCliente ?? "",
                                                       LocalidadCliente = departamento,
                                                       // NroDocCliente = RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucManguifajas || RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucMultimangueras ? x.TipoPersona == (int)TipoPersona.Natural && x.TipoDocumentoPersona == (int)TipoDocumentosPersona.Ruc ? x.NroDocCliente.Substring(2, 8) : x.TipoPersona == (int)TipoPersona.Natural && x.TipoDocumentoPersona == (int)TipoDocumentosPersona.Dni ? x.NroDocCliente : x.NroDocCliente : x.NroDocCliente,
                                                       NroDocCliente = DocumentoCliente,
                                                       TelefonoCliente = x.TelefonoCliente ?? "",
                                                       NombreAval = x.NombreAval ?? "",
                                                       LocalidadAval = x.LocalidadAval ?? "",
                                                       NroDocAval = x.NroDocAval ?? "",
                                                       Grupo = x.Grupo ?? "",
                                                       TotalLetras = Totalletras,
                                                       DomicilioAval = x.DomicilioAval ?? "",
                                                       TelefonoAval = x.TelefonoAval,
                                                       TipoDocumentoPersona = x.TipoDocumentoPersona,
                                                       TipoPersona = x.TipoPersona,
                                                       NroDocClienteRegistrado = x.NroDocClienteRegistrado,

                                                   };

                                               }).ToList();
                    pobjOperationResult.Success = 1;
                    return ReporteDocumentoLetra;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }

        }



        public List<ReporteDocumentoLetra> ReporteDocumentoLetraPagar(ref OperationResult pobjOperationResult,
           string IdLetras, bool ImprimirDni)
        {
            SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {




                    var RucEmpresa = new NodeBL().ReporteEmpresa();
                    var Venta = dbContext.venta.Where(x => x.i_Eliminado == 0);
                    OperationResult objOperationResult = new OperationResult();
                    var ReporteDocumentoLetra = (from a in dbContext.letraspagar

                                                 join c in dbContext.letraspagardetalle on new { lc = a.v_IdLetrasPagar, eliminado = 0 } equals new { lc = c.v_IdLetrasPagar, eliminado = c.i_Eliminado.Value } into c_join
                                                 from c in c_join.DefaultIfEmpty()

                                                 join d in dbContext.documento on new { td = c.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                 from d in d_join.DefaultIfEmpty()

                                                 join e in dbContext.cliente on new { c = a.v_IdProveedor, eliminado = 0 } equals new { c = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                                 from e in e_join.DefaultIfEmpty()


                                                 join f in dbContext.avalcliente on new { ac = e.v_IdCliente, eliminado = 0 } equals new { ac = f.v_IdCliente, eliminado = f.i_Eliminado.Value } into f_join
                                                 from f in f_join.DefaultIfEmpty()

                                                 where a.v_IdLetrasPagar == IdLetras && a.i_Eliminado == 0


                                                 select new ReporteDocumentoLetra
                                                 {

                                                     NumeroLetra = d.v_Siglas.Trim() + " " + c.v_Serie.Trim() + " " + c.v_Correlativo,
                                                     NroFactura = "",
                                                     FechaEmision = c.t_FechaEmision.Value,
                                                     FechaVencimiento = c.t_FechaVencimiento.Value,
                                                     Importe = c.d_Importe ?? 0,
                                                     Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                                     Empresa = "",
                                                     Cliente = d == null ? "" : (e.v_ApePaterno + " " + e.v_ApeMaterno + " " + e.v_PrimerNombre + " " + e.v_SegundoNombre + " " + e.v_RazonSocial).Trim(),
                                                     DomicilioCliente = d == null ? "" : e.v_DirecPrincipal,
                                                     LocalidadCliente = "",
                                                     NroDocCliente = d == null ? "" : e.v_NroDocIdentificacion == null ? "" : e.v_NroDocIdentificacion,
                                                     TelefonoCliente = d == null ? "" : e.v_TelefonoFijo,
                                                     NombreAval = e == null ? "" : f.v_Nombres,
                                                     DomicilioAval = e == null ? "" : f.v_Direccion,
                                                     LocalidadAval = e == null ? "" : f.v_Localidad,
                                                     NroDocAval = e == null ? "" : f.v_NroDocIdentificacion,
                                                     Grupo = d.v_Siglas.Trim() + " " + c.v_Serie.Trim() + " " + c.v_Correlativo,
                                                     IdMoneda = a.i_IdMoneda.Value,
                                                     TelefonoAval = e == null ? "" : f.v_Telefono,
                                                     IdLetras = a.v_IdLetrasPagar,
                                                     IdDepartamento = e.i_IdDepartamento ?? -1,
                                                     IdLetrasDetalle = c.v_IdLetrasPagarDetalle,
                                                     TipoDocumentoPersona = e.i_IdTipoIdentificacion ?? 0,
                                                     TipoPersona = e.i_IdTipoPersona ?? 0,
                                                     NroDocClienteRegistrado = e == null ? "" : e.v_NroDocIdentificacion == null ? "" : e.v_NroDocIdentificacion,
                                                 }).ToList().Select(x =>
                                                 {

                                                     letrasDto LetraDto = new letrasDto();

                                                     var IdVenta = (from a in dbContext.letrascanje
                                                                    where a.v_IdLetras == x.IdLetras && a.i_Eliminado == 0

                                                                    select new
                                                                    {
                                                                        IdVenta = a.v_IdVenta
                                                                    }).FirstOrDefault();

                                                     var IdLetrasDetalle = (from a in dbContext.letrascanje
                                                                            where a.i_Eliminado == 0 && a.v_IdLetras == x.IdLetras
                                                                            select new
                                                                            {
                                                                                IdLetrasDetalle = a.v_IdLetrasDetalle
                                                                            }).FirstOrDefault();
                                                     if (IdVenta != null && IdVenta.IdVenta != null)
                                                     {
                                                         LetraDto = (from a in dbContext.venta
                                                                     join b in dbContext.documento on new { v = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { v = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                                                     from b in b_join.DefaultIfEmpty()
                                                                     where a.v_IdVenta == IdVenta.IdVenta && a.i_Eliminado == 0
                                                                     select new letrasDto
                                                                     {
                                                                         NroRegistro = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                                                     }).FirstOrDefault();
                                                     }
                                                     else
                                                     {

                                                         if (IdLetrasDetalle != null && IdLetrasDetalle.IdLetrasDetalle != null)
                                                         {
                                                             LetraDto = (from a in dbContext.letrasdetalle
                                                                         join b in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                                                         from b in b_join.DefaultIfEmpty()
                                                                         where a.v_IdLetrasDetalle == IdLetrasDetalle.IdLetrasDetalle && a.i_Eliminado == 0
                                                                         select new letrasDto
                                                                         {
                                                                             NroRegistro = b.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                         }).FirstOrDefault();
                                                         }

                                                     }

                                                     var Totalletras = x.IdMoneda == (int)Currency.Soles ? Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " SOLES " : SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " SOLES  S.E.U.O" :

                                                     Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " DOLARES AMERICANOS " : SAMBHS.Common.Resource.Utils.ConvertirenLetras(x.Importe.ToString()) + " DOLARES AMERICANOS S.E.U.O. ";
                                                     var departamento = x.IdDepartamento == -1 ? "" : _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, "").ToList().Where(a => a.Id == x.IdDepartamento.ToString()).FirstOrDefault().Value1.ToString();
                                                     var DocumentoCliente = ImprimirDni ? DevuelveDniPersonaNatural(x.NroDocCliente, x.TipoPersona, x.TipoDocumentoPersona) : x.NroDocCliente;
                                                     return new ReporteDocumentoLetra
                                                     {
                                                         NumeroLetra = x.NumeroLetra ?? "",
                                                         NroFactura = LetraDto != null ? LetraDto.NroRegistro : "",
                                                         FechaEmision = x.FechaEmision,
                                                         FechaVencimiento = x.FechaVencimiento,
                                                         Importe = x.Importe,
                                                         Moneda = x.Moneda ?? "",
                                                         Empresa = RucEmpresa != null ? RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim() : "",
                                                         Cliente = x.Cliente ?? "",
                                                         DomicilioCliente = x.DomicilioCliente ?? "",
                                                         LocalidadCliente = departamento,
                                                         // NroDocCliente = RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucManguifajas || RucEmpresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucMultimangueras ? x.TipoPersona == (int)TipoPersona.Natural && x.TipoDocumentoPersona == (int)TipoDocumentosPersona.Ruc ? x.NroDocCliente.Substring(2, 8) : x.TipoPersona == (int)TipoPersona.Natural && x.TipoDocumentoPersona == (int)TipoDocumentosPersona.Dni ? x.NroDocCliente : x.NroDocCliente : x.NroDocCliente,
                                                         NroDocCliente = DocumentoCliente,
                                                         TelefonoCliente = x.TelefonoCliente ?? "",
                                                         NombreAval = x.NombreAval ?? "",
                                                         LocalidadAval = x.LocalidadAval ?? "",
                                                         NroDocAval = x.NroDocAval ?? "",
                                                         Grupo = x.Grupo ?? "",
                                                         TotalLetras = Totalletras,
                                                         DomicilioAval = x.DomicilioAval ?? "",
                                                         TelefonoAval = x.TelefonoAval,
                                                         TipoDocumentoPersona = x.TipoDocumentoPersona,
                                                         TipoPersona = x.TipoPersona,
                                                         NroDocClienteRegistrado = x.NroDocClienteRegistrado,

                                                     };

                                                 }).ToList();
                    pobjOperationResult.Success = 1;
                    return ReporteDocumentoLetra;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }

        }


        public string DevuelveDniPersonaNatural(string NroDocumentoRegistraodo, int TipPersona, int TipoDocumentoPersona)
        {
            string DniCliente = "";

            DniCliente = TipPersona == (int)TipoPersona.Natural && TipoDocumentoPersona == (int)TipoDocumentosPersona.Ruc ? NroDocumentoRegistraodo.Substring(2, 8) : TipPersona == (int)TipoPersona.Natural && TipoDocumentoPersona == (int)TipoDocumentosPersona.Dni ? NroDocumentoRegistraodo : NroDocumentoRegistraodo;

            return DniCliente;
        }



        public List<ReporteDocumentoLetra> ReporteDocumentoLetraCobrarFormatoVoucher(ref OperationResult pobjOperationResult,
           string IdLetras)
        {
            // SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var Venta = dbContext.venta.ToList();
                    var ListageneroCanje = dbContext.letrascanje.Where(x => x.v_IdLetras == IdLetras && x.i_Eliminado == 0).ToList();
                    var LetrasDetalle = dbContext.letrasdetalle.ToList();
                    var Adelanto = dbContext.adelanto.ToList();
                    var RucEmpresa = new NodeBL().ReporteEmpresa();
                    var LetrasCanje = (from a in dbContext.letras

                                       join b in dbContext.letrascanje on new { lc = a.v_IdLetras, eliminado = 0 } equals new { lc = b.v_IdLetras, eliminado = b.i_Eliminado.Value } into b_join

                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.cliente on new { c = a.v_IdCliente, eliminado = 0 } equals new { c = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.i_Eliminado == 0 && a.v_IdLetras == IdLetras

                                       select new
                                       {
                                           v_IdVenta = b.v_IdVenta,
                                           v_IdLetrasDetalle = b.v_IdLetrasDetalle,
                                           Cliente = c == null ? "" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim(),
                                           FechaCabecera = a.t_FechaRegistro.Value,
                                           Moneda = a.i_IdMoneda == 1 ? "MONEDA : SOLES" : "MONEDA : DÓLARES",
                                           Importe = a.i_IdMoneda == (int)Currency.Soles ? b.d_MontoCanjeado.Value : b.d_MontoCanjeado * a.d_TipoCambio.Value,
                                           MonedaSimboloInicial = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                           ImporteInicial = b.d_MontoCanjeado.Value,
                                           NroRegistro = "VOUCHER N° " + a.v_Mes + " " + a.v_Correlativo,
                                           v_IdLetra = a.v_IdLetras,
                                           v_IdAdelanto = b.v_IdAdelanto,

                                       }).ToList().Select(x =>
                                                        {
                                                            letrasDto LetraDto = new letrasDto();
                                                            if (x.v_IdVenta != null)
                                                            {

                                                                LetraDto = (from a in Venta

                                                                            join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                                                            from b in b_join.DefaultIfEmpty()
                                                                            where a.i_Eliminado == 0 && a.v_IdVenta == x.v_IdVenta
                                                                            select new letrasDto
                                                                            {
                                                                                NroRegistro = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                                                                t_FechaRegistro = a.t_FechaRegistro.Value,
                                                                                EsDocInverso = b.i_UsadoDocumentoInverso ?? 0,

                                                                            }).FirstOrDefault();

                                                            }
                                                            else if (x.v_IdLetrasDetalle != null)
                                                            {
                                                                LetraDto = (from a in LetrasDetalle

                                                                            join b in dbContext.documento on new { ld = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { ld = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                                                            from b in b_join.DefaultIfEmpty()

                                                                            where a.i_Eliminado == 0 && a.v_IdLetrasDetalle == x.v_IdLetrasDetalle
                                                                            select new letrasDto
                                                                            {
                                                                                NroRegistro = b.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                                t_FechaRegistro = a.t_FechaEmision.Value,
                                                                                EsDocInverso = b.i_UsadoDocumentoInverso ?? 0,
                                                                            }).FirstOrDefault();
                                                            }
                                                            else
                                                            {


                                                                LetraDto = (from a in Adelanto
                                                                            join b in dbContext.documento on new { tp = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tp = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into c_join
                                                                            from c in c_join.DefaultIfEmpty()
                                                                            where a.i_Eliminado == 0 && a.v_IdAdelanto == x.v_IdAdelanto
                                                                            select new letrasDto
                                                                            {
                                                                                NroRegistro = c.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                                                                t_FechaRegistro = a.t_FechaAdelanto.Value,
                                                                                EsDocInverso = 0,
                                                                            }).FirstOrDefault();

                                                            }

                                                            return new ReporteDocumentoLetra
                                                            {

                                                                Grupo1 = "FACTURAS SELECCIONADAS",
                                                                Empresa = RucEmpresa != null ? "EMPRESA : " + RucEmpresa.FirstOrDefault().NombreEmpresaPropietaria.Trim() : "",
                                                                Cliente = "CLIENTE : " + x.Cliente,
                                                                FechaEmisionCabecera = x.FechaCabecera,
                                                                Moneda = x.Moneda,
                                                                NroFactura = LetraDto != null ? LetraDto.NroRegistro : "",
                                                                FechaEmision = LetraDto != null ? LetraDto.t_FechaRegistro.Value : DateTime.Now,
                                                                MonedaSimboloInicial = x.MonedaSimboloInicial,
                                                                ImporteInicial = LetraDto != null ? LetraDto.EsDocInverso == 1 ? x.ImporteInicial * -1 : x.ImporteInicial : 0,
                                                                MonedaSimbolo = "S/",
                                                                Importe = LetraDto != null ? LetraDto.EsDocInverso == 1 ? x.Importe.Value * -1 : x.Importe.Value : 0,
                                                                NroRegistro = x.NroRegistro,
                                                                IdLetras = x.v_IdLetra,



                                                            };
                                                        }).ToList();



                    return LetrasCanje.ToList().OrderBy(o => o.FechaEmision).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }


        }


        public List<ReporteDocumentoLetraDetalles> ReporteDocumentoLetraCobrarFormatoVoucherDetalles(ref OperationResult pobjOperationResult,
           string IdLetras)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    pobjOperationResult.Success = 1;

                    var LetrasDetalle = (from a in dbContext.letrasdetalle

                                         join b in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                         from b in b_join.DefaultIfEmpty()

                                         where a.i_Eliminado == 0 && a.v_IdLetras == IdLetras

                                         select new ReporteDocumentoLetraDetalles
                                         {

                                             Grupito = "LETRAS GENERADAS",
                                             NumeroLetra = b.v_Siglas + " " + a.v_Serie + a.v_Correlativo,
                                             FechaEmision = a.t_FechaEmision.Value,
                                             FechaVencimiento = a.t_FechaVencimiento.Value,
                                             MonedaSimboloInicial = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                             ImporteInicial = a.d_Importe.Value,
                                             Importe = a.i_IdMoneda == 1 ? a.d_Importe.Value : a.d_Importe.Value * a.d_TipoCambio.Value,
                                             IdLetras = a.v_IdLetras

                                         }).ToList();

                    return LetrasDetalle.OrderBy(l => l.FechaVencimiento).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;

                return null;
            }
        }




        public List<ReporteDocumentoLetra> ReporteDocumentoLetraPagarFormatoVoucher(ref OperationResult pobjOperationResult,
          string IdLetras)
        {
            // SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ListageneroCanje = dbContext.letrascanje.Where(x => x.v_IdLetras == IdLetras && x.i_Eliminado == 0).ToList();
                    var RucEmpresa = new NodeBL().ReporteEmpresa();
                    var LetrasCanje = (from a in dbContext.letraspagar

                                       join b in dbContext.letraspagarcanje on new { lc = a.v_IdLetrasPagar, eliminado = 0 } equals new { lc = b.v_IdLetrasPagar, eliminado = b.i_Eliminado.Value } into b_join

                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.cliente on new { c = a.v_IdProveedor, eliminado = 0 } equals new { c = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.i_Eliminado == 0 && a.v_IdLetrasPagar == IdLetras

                                       select new
                                       {
                                           v_IdVenta = b.v_IdCompra,
                                           v_IdLetrasDetalle = b.v_IdLetrasPagarDetalle,
                                           Cliente = c == null ? "" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim(),
                                           FechaCabecera = a.t_FechaRegistro.Value,
                                           Moneda = a.i_IdMoneda == 1 ? "MONEDA : SOLES" : "MONEDA : DÓLARES",
                                           Importe = a.i_IdMoneda == (int)Currency.Soles ? b.d_MontoCanjeado.Value : b.d_MontoCanjeado * a.d_TipoCambio.Value,
                                           MonedaSimboloInicial = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                           ImporteInicial = b.d_MontoCanjeado.Value,
                                           NroRegistro = "VOUCHER N° " + a.v_Mes + " " + a.v_Correlativo,
                                           v_IdLetra = a.v_IdLetrasPagar,

                                       }).ToList().Select(x =>
                                       {
                                           letrasDto LetraDto = new letrasDto();
                                           if (x.v_IdVenta != null)
                                           {

                                               LetraDto = (from a in dbContext.compra

                                                           join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                                           from b in b_join.DefaultIfEmpty()
                                                           where a.i_Eliminado == 0 && a.v_IdCompra == x.v_IdVenta
                                                           select new letrasDto
                                                           {
                                                               NroRegistro = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                                               t_FechaRegistro = a.t_FechaRegistro.Value,
                                                               i_IdMoneda = b.i_UsadoDocumentoInverso ?? 0,
                                                           }).FirstOrDefault();

                                           }
                                           else
                                           {
                                               LetraDto = (from a in dbContext.letraspagardetalle

                                                           join b in dbContext.documento on new { ld = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { ld = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                                           from b in b_join.DefaultIfEmpty()

                                                           where a.i_Eliminado == 0 && a.v_IdLetrasPagarDetalle == x.v_IdLetrasDetalle
                                                           select new letrasDto
                                                           {
                                                               NroRegistro = b.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                               t_FechaRegistro = a.t_FechaEmision.Value,
                                                               i_IdMoneda = b.i_UsadoDocumentoInverso ?? 0
                                                           }).FirstOrDefault();
                                           }

                                           return new ReporteDocumentoLetra
                                           {

                                               Grupo1 = "FACTURAS SELECCIONADAS",
                                               Empresa = RucEmpresa != null ? "EMPRESA : " + RucEmpresa.FirstOrDefault().NombreEmpresaPropietaria.Trim() : "",
                                               Cliente = "CLIENTE : " + x.Cliente,
                                               FechaEmisionCabecera = x.FechaCabecera,
                                               Moneda = x.Moneda,
                                               NroFactura = LetraDto.NroRegistro,
                                               FechaEmision = LetraDto.t_FechaRegistro.Value,
                                               MonedaSimboloInicial = x.MonedaSimboloInicial,
                                               ImporteInicial = LetraDto.i_IdMoneda == 1 ? x.ImporteInicial * -1 : x.ImporteInicial,
                                               MonedaSimbolo = "S/",
                                               Importe = LetraDto.i_IdMoneda == 1 ? x.Importe.Value * -1 : x.Importe.Value,
                                               NroRegistro = x.NroRegistro,
                                               IdLetras = x.v_IdLetra,



                                           };
                                       }).ToList();



                    return LetrasCanje.ToList();

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }


        }


        public List<ReporteDocumentoLetraDetalles> ReporteDocumentoLetraPagarFormatoVoucherDetalles(ref OperationResult pobjOperationResult,
           string IdLetras)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    pobjOperationResult.Success = 1;

                    var LetrasDetalle = (from a in dbContext.letraspagardetalle

                                         join b in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                         from b in b_join.DefaultIfEmpty()

                                         where a.i_Eliminado == 0 && a.v_IdLetrasPagar == IdLetras

                                         select new ReporteDocumentoLetraDetalles
                                         {

                                             Grupito = "LETRAS GENERADAS",
                                             NumeroLetra = b.v_Siglas + " " + a.v_Serie + a.v_Correlativo,
                                             FechaEmision = a.t_FechaEmision.Value,
                                             FechaVencimiento = a.t_FechaVencimiento.Value,
                                             MonedaSimboloInicial = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                             ImporteInicial = a.d_Importe.Value,
                                             Importe = a.i_IdMoneda == 1 ? a.d_Importe.Value : a.d_Importe.Value * a.d_TipoCambio.Value,
                                             IdLetras = a.v_IdLetrasPagar,

                                         }).ToList();

                    return LetrasDetalle;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;

                return null;
            }
        }




        public List<ReporteLetrasCobrarEmitidas> ReporteLetrasCobrarEmitidas(ref OperationResult objOperationResult, string CodCliente, DateTime FechaInicio, DateTime FechaFin, string Agrupacion, bool MostrarSoloCanceladas, bool RangoFecha, string EstadoLetra, string UbicacionLetra, string Orden)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    objOperationResult.Success = 1;

                    if (RangoFecha)
                    {

                        #region RangoFecha
                        if (MostrarSoloCanceladas)
                        {
                            var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                            List<ReporteLetrasCobrarEmitidas> letrasdetalle = (from b in dbContext.cobranzadetalle

                                                                               join h in dbContext.cobranza on new { c = b.v_IdCobranza, eliminado = 0 } equals new { c = h.v_IdCobranza, eliminado = h.i_Eliminado.Value } into h_join

                                                                               from h in h_join.DefaultIfEmpty()

                                                                               join a in dbContext.letrasdetalle on new { ld = b.v_IdVenta, eliminado = 0 } equals new { ld = a.v_IdLetrasDetalle, eliminado = a.i_Eliminado.Value } into a_join

                                                                               from a in a_join.DefaultIfEmpty()


                                                                               join d in dbContext.letras on new { IdLetras = a.v_IdLetras, eliminado = 0 } equals new { IdLetras = d.v_IdLetras, eliminado = d.i_Eliminado.Value } into d_join

                                                                               from d in d_join.DefaultIfEmpty()

                                                                               join e in dbContext.cliente on new { cliente = a.v_IdCliente, eliminado = 0 } equals new { cliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                                                               from e in e_join.DefaultIfEmpty()

                                                                               join g in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join
                                                                               from g in g_join.DefaultIfEmpty()



                                                                               where a.t_FechaEmision >= FechaInicio && a.t_FechaEmision <= FechaFin
                                                                               && (e.v_CodCliente == CodCliente || CodCliente == "")

                                                                                && a.i_Eliminado == 0 && b.i_Eliminado == 0 && h.i_IdEstado == 1
                                                                               select new
                                                                               {

                                                                                   Cliente = (e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim(),
                                                                                   NumeroLetra = g.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                                   TotalDias = a.i_TotalDias ?? 0,
                                                                                   FechaEmision = a.t_FechaEmision.Value,
                                                                                   FechaVencimiento = a.t_FechaVencimiento.Value,
                                                                                   Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                                   ImporteSoles = a.d_Importe.Value,
                                                                                   LetrasPagarDetalle = a.v_IdLetrasDetalle,
                                                                                   CodigoCliente = e.v_CodCliente.Trim(),
                                                                                   NroRegistro = a.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Mes.Trim() + " " + d.v_Correlativo.Trim(),
                                                                                   IdLetra = d.v_IdLetras,
                                                                                   NroCobranza = h.v_Mes + "-" + h.v_Correlativo,
                                                                                   FechaPago = h.t_FechaRegistro.Value,
                                                                                   MontoCobrado = b.d_ImporteSoles,
                                                                                   MonedaCobranza = h.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$."



                                                                               }).ToList().Select(l =>
                                                                          {
                                                                              var ubicacion = ObtenerUltimaUbicacionLetra(l.LetrasPagarDetalle, tempDs);

                                                                              return new ReporteLetrasCobrarEmitidas
                                                                              {
                                                                                  Estado = ubicacion.Estado,
                                                                                  Cliente = l.Cliente,
                                                                                  CodigoCliente = l.CodigoCliente,
                                                                                  NumeroLetra = l.NumeroLetra,
                                                                                  TotalDias = l.TotalDias,
                                                                                  Moneda = l.Moneda,
                                                                                  NroRegistro = l.NroRegistro,
                                                                                  FechaVencimiento = l.FechaVencimiento,
                                                                                  FechaEmision = l.FechaEmision,
                                                                                  ImporteSoles = Utils.Windows.DevuelveValorRedondeado(l.ImporteSoles, 2),
                                                                                  sFechaEmision = l.FechaEmision.ToShortDateString(),
                                                                                  sFechaVencimiento = l.FechaVencimiento.ToShortDateString(),
                                                                                  Ubicacion = ubicacion.Ubicacion,
                                                                                  UbicacionNombreCompleto = ubicacion.Ubicacion,
                                                                                  NroUnico = ubicacion.v_NroUnico,
                                                                                  NroCobranza = l.NroCobranza,
                                                                                  FechaPago = l.FechaPago.ToShortDateString(),
                                                                                  MontoCobrado = l.MontoCobrado.Value,
                                                                                  MonedaCobranza = l.MonedaCobranza,
                                                                                  Grupo = Agrupacion == "Cliente"
                                                                                          ? "CLIENTE : " + l.CodigoCliente + " " + l.Cliente
                                                                                          : Agrupacion == "Vencimiento"
                                                                                              ? "FECHA VENCIMIENTO : " + l.FechaVencimiento.ToShortDateString()
                                                                                              : Agrupacion == "Ubicacion" ? "UBICACIÓN :" + ubicacion.Ubicacion : "",

                                                                              };
                                                                          }).ToList();


                            if (EstadoLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Estado == EstadoLetra).ToList();
                            if (UbicacionLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Ubicacion == UbicacionLetra).ToList();
                            if (Orden == "FechaVencimiento")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaVencimiento).ToList();
                            }
                            else if (Orden == "FechaEmision")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaEmision).ToList();
                            }
                            else if (Orden == "NumeroLetra")
                            {
                                return letrasdetalle.OrderBy(l => l.NumeroLetra).ToList();
                            }
                            else if (Orden == "NroRegistro")
                            {
                                return letrasdetalle.OrderBy(l => l.NroRegistro).ToList();
                            }

                            return letrasdetalle;


                        }
                        else
                        {
                            var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                            var Cobranzas = dbContext.cobranzadetalle.Where(o => o.i_Eliminado == 0 && o.cobranza.i_IdEstado == 1).ToList().Select(p => new { fecha = p.cobranza.t_FechaRegistro, IdVenta = p.v_IdVenta, NroCobranza = p.cobranza.v_Mes + " " + p.cobranza.v_Correlativo, cobrado = p.d_ImporteSoles ?? 0, moneda = p.cobranza.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$." }).ToList();
                            List<ReporteLetrasCobrarEmitidas> letrasdetalle = (

                                                                              from a in dbContext.letrasdetalle

                                                                              join d in dbContext.letras on new { IdLetras = a.v_IdLetras, eliminado = 0 } equals new { IdLetras = d.v_IdLetras, eliminado = d.i_Eliminado.Value } into d_join

                                                                              from d in d_join.DefaultIfEmpty()

                                                                              join e in dbContext.cliente on new { cliente = a.v_IdCliente, eliminado = 0 } equals new { cliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                                                              from e in e_join.DefaultIfEmpty()

                                                                              join g in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join
                                                                              from g in g_join.DefaultIfEmpty()



                                                                              where a.t_FechaEmision >= FechaInicio && a.t_FechaEmision <= FechaFin
                                                                              && (e.v_CodCliente == CodCliente || CodCliente == "")

                                                                               && a.i_Eliminado == 0
                                                                              select new
                                                                              {

                                                                                  Cliente = (e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim(),
                                                                                  NumeroLetra = g.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                                  TotalDias = a.i_TotalDias ?? 0,
                                                                                  FechaEmision = a.t_FechaEmision.Value,
                                                                                  FechaVencimiento = a.t_FechaVencimiento.Value,
                                                                                  Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                                  ImporteSoles = a.d_Importe.Value,
                                                                                  LetrasPagarDetalle = a.v_IdLetrasDetalle,
                                                                                  CodigoCliente = e.v_CodCliente.Trim(),
                                                                                  NroRegistro = a.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Mes.Trim() + " " + d.v_Correlativo.Trim(),
                                                                                  IdLetra = d.v_IdLetras,
                                                                              }).ToList().Select(l =>
                                                                               {
                                                                                   var ubicacion = ObtenerUltimaUbicacionLetra(l.LetrasPagarDetalle, tempDs);
                                                                                   var FechaCobranza = Cobranzas.Where(o => o.IdVenta == l.LetrasPagarDetalle).ToList();
                                                                                   return new ReporteLetrasCobrarEmitidas
                                                                               {
                                                                                   Estado = ubicacion.Estado,
                                                                                   Cliente = l.Cliente,
                                                                                   CodigoCliente = l.CodigoCliente,
                                                                                   NumeroLetra = l.NumeroLetra,
                                                                                   TotalDias = l.TotalDias,
                                                                                   Moneda = l.Moneda,
                                                                                   NroRegistro = l.NroRegistro,
                                                                                   FechaVencimiento = l.FechaVencimiento,
                                                                                   FechaEmision = l.FechaEmision,
                                                                                   ImporteSoles = Utils.Windows.DevuelveValorRedondeado(l.ImporteSoles, 2),
                                                                                   sFechaEmision = l.FechaEmision.ToShortDateString(),
                                                                                   sFechaVencimiento = l.FechaVencimiento.ToShortDateString(),
                                                                                   Ubicacion = ubicacion.Ubicacion,
                                                                                   UbicacionNombreCompleto = ubicacion.Ubicacion,
                                                                                   NroUnico = ubicacion.v_NroUnico,
                                                                                   NroCobranza = !FechaCobranza.Any() ? "" : FechaCobranza.FirstOrDefault().NroCobranza,
                                                                                   FechaPago = !FechaCobranza.Any() ? "" : FechaCobranza.FirstOrDefault().fecha.Value.ToShortDateString(),
                                                                                   MontoCobrado = !FechaCobranza.Any() ? 0 : FechaCobranza.FirstOrDefault().cobrado,
                                                                                   MonedaCobranza = !FechaCobranza.Any() ? "" : FechaCobranza.FirstOrDefault().moneda,
                                                                                   Grupo = Agrupacion == "Cliente"
                                                                                           ? "CLIENTE : " + l.CodigoCliente + " " + l.Cliente
                                                                                           : Agrupacion == "Vencimiento"
                                                                                               ? "FECHA VENCIMIENTO : " + l.FechaVencimiento.ToShortDateString()
                                                                                               : Agrupacion == "Ubicacion" ? "UBICACIÓN :" + ubicacion.Ubicacion : "",

                                                                               };
                                                                               }).ToList();


                            if (EstadoLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Estado == EstadoLetra).ToList();
                            if (UbicacionLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Ubicacion == UbicacionLetra).ToList();
                            if (Orden == "FechaVencimiento")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaVencimiento).ToList();
                            }
                            else if (Orden == "FechaEmision")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaEmision).ToList();
                            }
                            else if (Orden == "NumeroLetra")
                            {
                                return letrasdetalle.OrderBy(l => l.NumeroLetra).ToList();
                            }
                            else if (Orden == "NroRegistro")
                            {
                                return letrasdetalle.OrderBy(l => l.NroRegistro).ToList();
                            }
                            return letrasdetalle;
                        }

                        #endregion
                    }
                    else
                    {
                        #region SinRangoFecha

                        if (MostrarSoloCanceladas)
                        {
                            var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                            List<ReporteLetrasCobrarEmitidas> letrasdetalle = (from b in dbContext.cobranzadetalle

                                                                               join h in dbContext.cobranza on new { c = b.v_IdCobranza, eliminado = 0 } equals new { c = h.v_IdCobranza, eliminado = h.i_Eliminado.Value } into h_join

                                                                               from h in h_join.DefaultIfEmpty()

                                                                               join a in dbContext.letrasdetalle on new { ld = b.v_IdVenta, eliminado = 0 } equals new { ld = a.v_IdLetrasDetalle, eliminado = a.i_Eliminado.Value } into a_join

                                                                               from a in a_join.DefaultIfEmpty()


                                                                               join d in dbContext.letras on new { IdLetras = a.v_IdLetras, eliminado = 0 } equals new { IdLetras = d.v_IdLetras, eliminado = d.i_Eliminado.Value } into d_join

                                                                               from d in d_join.DefaultIfEmpty()

                                                                               join e in dbContext.cliente on new { cliente = a.v_IdCliente, eliminado = 0 } equals new { cliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                                                               from e in e_join.DefaultIfEmpty()

                                                                               join g in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join
                                                                               from g in g_join.DefaultIfEmpty()



                                                                               where
                                                                                (e.v_CodCliente == CodCliente || CodCliente == "")

                                                                                && a.i_Eliminado == 0 && b.i_Eliminado == 0 && h.i_IdEstado == 1
                                                                               select new
                                                                               {

                                                                                   Cliente = (e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim(),
                                                                                   NumeroLetra = g.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                                   TotalDias = a.i_TotalDias ?? 0,
                                                                                   FechaEmision = a.t_FechaEmision.Value,
                                                                                   FechaVencimiento = a.t_FechaVencimiento.Value,
                                                                                   Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                                   ImporteSoles = a.d_Importe.Value,
                                                                                   LetrasPagarDetalle = a.v_IdLetrasDetalle,
                                                                                   CodigoCliente = e.v_CodCliente.Trim(),
                                                                                   NroRegistro = a.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Mes.Trim() + " " + d.v_Correlativo.Trim(),
                                                                                   IdLetra = d.v_IdLetras,
                                                                                   NroCobranza = h.v_Mes + "-" + h.v_Correlativo,
                                                                                   FechaPago = h.t_FechaRegistro.Value,
                                                                                   MontoCobrado = b.d_ImporteSoles ?? 0,
                                                                                   MonedaCobranza = h.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",


                                                                               }).ToList().Select(l =>
                                                                          {
                                                                              var ubicacion = ObtenerUltimaUbicacionLetra(l.LetrasPagarDetalle, tempDs);

                                                                              return new ReporteLetrasCobrarEmitidas
                                                                              {
                                                                                  Estado = ubicacion.Estado,
                                                                                  Cliente = l.Cliente,
                                                                                  CodigoCliente = l.CodigoCliente,
                                                                                  NumeroLetra = l.NumeroLetra,
                                                                                  TotalDias = l.TotalDias,
                                                                                  Moneda = l.Moneda,
                                                                                  NroRegistro = l.NroRegistro,
                                                                                  FechaVencimiento = l.FechaVencimiento,
                                                                                  FechaEmision = l.FechaEmision,
                                                                                  ImporteSoles = Utils.Windows.DevuelveValorRedondeado(l.ImporteSoles, 2),
                                                                                  sFechaEmision = l.FechaEmision.ToShortDateString(),
                                                                                  sFechaVencimiento = l.FechaVencimiento.ToShortDateString(),
                                                                                  Ubicacion = ubicacion.Ubicacion,
                                                                                  UbicacionNombreCompleto = ubicacion.Ubicacion,
                                                                                  NroUnico = ubicacion.v_NroUnico,
                                                                                  NroCobranza = l.NroCobranza,
                                                                                  FechaPago = l.FechaPago.ToShortDateString(),
                                                                                  MonedaCobranza = l.MonedaCobranza,
                                                                                  Grupo = Agrupacion == "Cliente"
                                                                                          ? "CLIENTE : " + l.CodigoCliente + " " + l.Cliente
                                                                                          : Agrupacion == "Vencimiento"
                                                                                              ? "FECHA VENCIMIENTO : " + l.FechaVencimiento.ToShortDateString()
                                                                                              : Agrupacion == "Ubicacion" ? "UBICACIÓN :" + ubicacion.Ubicacion : "",
                                                                                  MontoCobrado = l.MontoCobrado,

                                                                              };
                                                                          }).ToList();


                            if (EstadoLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Estado == EstadoLetra).ToList();
                            if (UbicacionLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Ubicacion == UbicacionLetra).ToList();
                            if (Orden == "FechaVencimiento")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaVencimiento).ToList();
                            }
                            else if (Orden == "FechaEmision")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaEmision).ToList();
                            }
                            else if (Orden == "NumeroLetra")
                            {
                                return letrasdetalle.OrderBy(l => l.NumeroLetra).ToList();
                            }
                            else if (Orden == "NroRegistro")
                            {
                                return letrasdetalle.OrderBy(l => l.NroRegistro).ToList();
                            }
                            return letrasdetalle;
                        }
                        else
                        {
                            var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                            var Cobranzas = dbContext.cobranzadetalle.Where(o => o.i_Eliminado == 0 && o.cobranza.i_IdEstado == 1).ToList().Select(p => new { fecha = p.cobranza.t_FechaRegistro, IdVenta = p.v_IdVenta, NroCobranza = p.cobranza.v_Mes + " " + p.cobranza.v_Correlativo, cobrado = p.d_ImporteSoles, moneda = p.cobranza.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$." }).ToList();
                            List<ReporteLetrasCobrarEmitidas> letrasdetalle = (

                                                                              from a in dbContext.letrasdetalle

                                                                              join d in dbContext.letras on new { IdLetras = a.v_IdLetras, eliminado = 0 } equals new { IdLetras = d.v_IdLetras, eliminado = d.i_Eliminado.Value } into d_join

                                                                              from d in d_join.DefaultIfEmpty()

                                                                              join e in dbContext.cliente on new { cliente = a.v_IdCliente, eliminado = 0 } equals new { cliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                                                              from e in e_join.DefaultIfEmpty()

                                                                              join g in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join
                                                                              from g in g_join.DefaultIfEmpty()



                                                                              where
                                                                              (e.v_CodCliente == CodCliente || CodCliente == "")

                                                                               && a.i_Eliminado == 0
                                                                              select new
                                                                              {

                                                                                  Cliente = (e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim(),
                                                                                  NumeroLetra = g.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                                  TotalDias = a.i_TotalDias ?? 0,
                                                                                  FechaEmision = a.t_FechaEmision.Value,
                                                                                  FechaVencimiento = a.t_FechaVencimiento.Value,
                                                                                  Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                                  ImporteSoles = a.d_Importe.Value,
                                                                                  LetrasPagarDetalle = a.v_IdLetrasDetalle,
                                                                                  CodigoCliente = e.v_CodCliente.Trim(),
                                                                                  NroRegistro = a.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Mes.Trim() + " " + d.v_Correlativo.Trim(),
                                                                                  IdLetra = d.v_IdLetras,

                                                                              }).ToList().Select(l =>
                                                                               {
                                                                                   var ubicacion = ObtenerUltimaUbicacionLetra(l.LetrasPagarDetalle, tempDs);
                                                                                   var FechaCobranza = Cobranzas.Where(o => o.IdVenta == l.LetrasPagarDetalle).ToList();
                                                                                   return new ReporteLetrasCobrarEmitidas
                                                                               {
                                                                                   Estado = ubicacion.Estado,
                                                                                   Cliente = l.Cliente,
                                                                                   CodigoCliente = l.CodigoCliente,
                                                                                   NumeroLetra = l.NumeroLetra,
                                                                                   TotalDias = l.TotalDias,
                                                                                   Moneda = l.Moneda,
                                                                                   NroRegistro = l.NroRegistro,
                                                                                   FechaVencimiento = l.FechaVencimiento,
                                                                                   FechaEmision = l.FechaEmision,
                                                                                   ImporteSoles = Utils.Windows.DevuelveValorRedondeado(l.ImporteSoles, 2),
                                                                                   sFechaEmision = l.FechaEmision.ToShortDateString(),
                                                                                   sFechaVencimiento = l.FechaVencimiento.ToShortDateString(),
                                                                                   Ubicacion = ubicacion.Ubicacion,
                                                                                   UbicacionNombreCompleto = ubicacion.Ubicacion,
                                                                                   NroUnico = ubicacion.v_NroUnico,
                                                                                   NroCobranza = !FechaCobranza.Any() ? "" : FechaCobranza.FirstOrDefault().NroCobranza,
                                                                                   FechaPago = !FechaCobranza.Any() ? "" : FechaCobranza.FirstOrDefault().fecha.Value.ToShortDateString(),
                                                                                   MontoCobrado = !FechaCobranza.Any() ? 0 : FechaCobranza.FirstOrDefault().cobrado.Value,
                                                                                   MonedaCobranza = !FechaCobranza.Any() ? "" : FechaCobranza.FirstOrDefault().moneda,
                                                                                   Grupo = Agrupacion == "Cliente"
                                                                                           ? "CLIENTE : " + l.CodigoCliente + " " + l.Cliente
                                                                                           : Agrupacion == "Vencimiento"
                                                                                               ? "FECHA VENCIMIENTO : " + l.FechaVencimiento.ToShortDateString()
                                                                                               : Agrupacion == "Ubicacion" ? "UBICACIÓN :" + ubicacion.Ubicacion : "",

                                                                               };
                                                                               }).ToList();


                            if (EstadoLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Estado == EstadoLetra).ToList();
                            if (UbicacionLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Ubicacion == UbicacionLetra).ToList();
                            if (Orden == "FechaVencimiento")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaVencimiento).ToList();
                            }
                            else if (Orden == "FechaEmision")
                            {
                                return letrasdetalle.OrderBy(l => l.FechaEmision).ToList();
                            }
                            else if (Orden == "NumeroLetra")
                            {
                                return letrasdetalle.OrderBy(l => l.NumeroLetra).ToList();
                            }
                            else if (Orden == "NroRegistro")
                            {
                                return letrasdetalle.OrderBy(l => l.NroRegistro).ToList();
                            }
                            return letrasdetalle;
                        }
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "LetrasBL.ReporteLetrasCobrarEmitidas()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;


            }





        }




        public List<ReporteLetrasCobrarEmitidas> ReporteLetrasSaldoInicial(ref OperationResult objOperationResult, string CodCliente, string Agrupacion, string EstadoLetra, string UbicacionLetra)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    objOperationResult.Success = 1;


                    #region SinRangoFecha
                    string periodo = Globals.ClientSession.i_Periodo.ToString();
                    var tempDs = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0).ToList();
                    List<ReporteLetrasCobrarEmitidas> letrasdetalle = (from a in dbContext.letrasdetalle

                                                                       join e in dbContext.cliente on new { cliente = a.v_IdCliente, eliminado = 0 } equals new { cliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                                                       from e in e_join.DefaultIfEmpty()



                                                                       join g in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join
                                                                       from g in g_join.DefaultIfEmpty()

                                                                       where
                                                                        (e.v_CodCliente == CodCliente || CodCliente == "")

                                                                        && a.i_Eliminado == 0 && a.i_EsSaldoInicial == 1 && a.i_Pagada == 0
                                                                       select new
                                                                       {

                                                                           Cliente = (e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim(),
                                                                           NumeroLetra = g.v_Siglas + " " + a.v_Serie + " " + a.v_Correlativo,
                                                                           TotalDias = a.i_TotalDias ?? 0,
                                                                           FechaEmision = a.t_FechaEmision.Value,
                                                                           FechaVencimiento = a.t_FechaVencimiento.Value,
                                                                           Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                           ImporteSoles = a.d_Importe.Value,
                                                                           LetrasPagarDetalle = a.v_IdLetrasDetalle,
                                                                           CodigoCliente = e.v_CodCliente.Trim(),
                                                                           //NroRegistro = a.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Mes.Trim() + " " + d.v_Correlativo.Trim(),
                                                                           // IdLetra = d.v_IdLetras,


                                                                       }).ToList().Select(l =>
                                                                  {
                                                                      var ubicacion = ObtenerUltimaUbicacionLetra(l.LetrasPagarDetalle, tempDs);

                                                                      return new ReporteLetrasCobrarEmitidas
                                                                      {
                                                                          Estado = ubicacion.Estado,
                                                                          Cliente = l.Cliente,
                                                                          CodigoCliente = l.CodigoCliente,
                                                                          NumeroLetra = l.NumeroLetra,
                                                                          TotalDias = l.TotalDias,
                                                                          Moneda = l.Moneda,
                                                                          //NroRegistro = l.NroRegistro,
                                                                          FechaVencimiento = l.FechaVencimiento,
                                                                          ImporteSoles = Utils.Windows.DevuelveValorRedondeado(l.ImporteSoles, 2),
                                                                          sFechaEmision = l.FechaEmision.ToShortDateString(),
                                                                          sFechaVencimiento = l.FechaVencimiento.ToShortDateString(),
                                                                          Ubicacion = ubicacion.Ubicacion,
                                                                          UbicacionNombreCompleto = ubicacion.Ubicacion,
                                                                          NroUnico = ubicacion.v_NroUnico,

                                                                          Grupo = Agrupacion == "Cliente"
                                                                                  ? "CLIENTE : " + l.CodigoCliente + " " + l.Cliente
                                                                                  : Agrupacion == "Vencimiento"
                                                                                      ? "FECHA VENCIMIENTO : " + l.FechaVencimiento.ToShortDateString()
                                                                                      : Agrupacion == "Ubicacion" ? "UBICACIÓN :" + ubicacion.Ubicacion : "",
                                                                          MontoCobrado = 0,

                                                                      };
                                                                  }).ToList();


                    if (EstadoLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Estado == EstadoLetra).ToList();
                    if (UbicacionLetra != "-1") letrasdetalle = letrasdetalle.Where(p => p.Ubicacion == UbicacionLetra).ToList();
                    return letrasdetalle.OrderBy(l => l.FechaVencimiento).ToList();


                }
                    #endregion
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "LetrasBL.ReporteLetrasCobrarEmitidas()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;


            }
        }


        #endregion

        public static IEnumerable<letrasdetalleDto> ObtenerLetrasParaEdicion(ref OperationResult pobjOperationResult,
            string pstrIdLetras)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letras = dbContext.letrasdetalle.Where(p => p.i_Eliminado == 0 && p.v_IdLetras.Equals(pstrIdLetras)).ToList();
                    pobjOperationResult.Success = 1;
                    return letras.ToDTOs();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasParaEdicion()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void GuardarEdicionLetras(ref OperationResult pobjOperationResult,
            IEnumerable<letrasdetalleDto> lista, out int i)
        {
            i = 0;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    foreach (var letrasdetalleDto in lista)
                    {
                        var letra = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle.
                            Equals(letrasdetalleDto.v_IdLetrasDetalle));
                        if (letra == null) continue;
                        letra.i_TotalDias = letrasdetalleDto.i_TotalDias ?? 0;
                        letra.t_FechaEmision = letrasdetalleDto.t_FechaEmision ?? DateTime.Now;
                        letra.t_FechaVencimiento = letrasdetalleDto.t_FechaVencimiento ?? DateTime.Now;
                        letra.t_ActualizaFecha = DateTime.Now;
                        letra.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        dbContext.letrasdetalle.ApplyCurrentValues(letra);
                    }

                    i = dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.GuardarEdicionLetras()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion

        #region Letras Por Pagar

        #region Canjeo

        public List<letraspagarDto> ObtenerLetrasPagarBandeja(ref OperationResult pobjOperationResult, DateTime F_Ini,
            DateTime F_Fin, string pstrIdProveedor)
        {
            F_Ini = F_Ini.Date;
            F_Fin =
                DateTime.Parse(F_Fin.Day.ToString() + "/" + F_Fin.Month.ToString() + "/" + F_Fin.Year.ToString() +
                               " 23:59");
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<letraspagardetalle> ListaLetrasDetalle = dbContext.letraspagardetalle.Where(l => l.i_Eliminado == 0).ToList();
                    int IdTipoDoc = ListaLetrasDetalle.Any() ? ListaLetrasDetalle.Where(l => l.i_IdTipoDocumento != -1).Select(l => l.i_IdTipoDocumento).FirstOrDefault().Value : -1;
                    string documentLetra = dbContext.documento.Where(l => l.i_Eliminado == 0 && l.i_CodigoDocumento == IdTipoDoc).FirstOrDefault().v_Siglas;

                    var Consulta = (from p in dbContext.letraspagar
                                    join J1 in dbContext.cliente on p.v_IdProveedor equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.systemuser on new { i_UpdateUserId = p.i_ActualizaIdUsuario.Value }
                                        equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.systemuser on new { i_InsertUserId = p.i_InsertaIdUsuario.Value }
                                        equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()
                                    where p.t_FechaRegistro >= F_Ini.Date && p.t_FechaRegistro <= F_Fin && p.i_Eliminado == 0
                                    && (p.v_IdProveedor == pstrIdProveedor || pstrIdProveedor == null)
                                    select new
                                    {
                                        v_IdLetrasPagar = p.v_IdLetrasPagar,
                                        NroRegistro = p.v_Mes.Trim() + "-" + p.v_Correlativo.Trim(),
                                        NombreProveedor =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        t_FechaRegistro = p.t_FechaRegistro,
                                        Moneda = p.i_IdMoneda == 1 ? "SOLES" : "DÓLARES",
                                        d_TotalLetras = p.d_TotalLetras,
                                        v_UsuarioCreacion = J3.v_UserName,
                                        v_UsuarioModificacion = J2.v_UserName,
                                        t_ActualizaFecha = p.t_ActualizaFecha,
                                        t_InsertaFecha = p.t_InsertaFecha,
                                        v_IdProveedor = p.v_IdProveedor
                                    }).ToList().Select(o => new letraspagarDto
                                    {

                                        v_IdLetrasPagar = o.v_IdLetrasPagar,
                                        NroRegistro = o.NroRegistro,
                                        NombreProveedor = o.NombreProveedor,

                                        t_FechaRegistro = o.t_FechaRegistro,
                                        Moneda = o.Moneda,
                                        d_TotalLetras = o.d_TotalLetras,
                                        v_UsuarioCreacion = o.v_UsuarioCreacion,
                                        v_UsuarioModificacion = o.v_UsuarioModificacion,
                                        t_ActualizaFecha = o.t_ActualizaFecha,
                                        t_InsertaFecha = o.t_InsertaFecha,
                                        v_IdProveedor = o.v_IdProveedor,
                                        ListaLetrasContiene = (from a in ListaLetrasDetalle

                                                               where a.v_IdLetrasPagar == o.v_IdLetrasPagar

                                                               select documentLetra + " " + a.v_Serie + " " + a.v_Correlativo).AsEnumerable(),

                                    }).ToList();

                    //if (!string.IsNullOrEmpty(pstrIdProveedor))
                    //{
                    //    Consulta = Consulta.Where(p => p.v_IdProveedor == pstrIdProveedor);
                    //}
                    Consulta.AsParallel().ToList().ForEach(letra =>
                    {

                        letra.LetrasVistaRapida = string.Join(", ", letra.ListaLetrasContiene.Distinct());

                    });

                    pobjOperationResult.Success = 1;
                    return Consulta.ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasPagarBandeja()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public letraspagarDto ObtenerLetraPagar(ref OperationResult pobjOperationResult, string pstrIdLetra)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                try
                {
                    var Consulta = (from n in dbContext.letraspagar
                                    join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.v_IdLetrasPagar == pstrIdLetra && n.i_Eliminado == 0
                                    select new letraspagarDto
                                    {
                                        d_TipoCambio = n.d_TipoCambio,
                                        d_TotalFacturas = n.d_TotalFacturas,
                                        d_TotalLetras = n.d_TotalLetras,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaRegistro = n.t_FechaRegistro,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        v_Correlativo = n.v_Correlativo,
                                        v_IdProveedor = n.v_IdProveedor,
                                        NombreProveedor =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        v_IdLetrasPagar = n.v_IdLetrasPagar,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        i_NoSeleccionarFactura = n.i_NoSeleccionarFactura,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Eliminado = n.i_Eliminado,
                                        i_IdMoneda = n.i_IdMoneda,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_NroDias = n.i_NroDias,
                                        i_NroLetras = n.i_NroLetras,
                                        i_IdEstado = n.i_IdEstado
                                    }).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetraPagar()\nLinea:" +
                                                                ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null
                        ? ex.InnerException.Message
                        : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return null;
                }
            }
        }

        public List<letraspagarcanjeDto> ObtenerLetrasPagarCanje(ref OperationResult pobjOperationResult,
            string pstrIdLetra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Obtener Canjes de Ventas

                    List<letraspagarcanjeDto> Consulta = (from n in dbContext.letraspagarcanje
                                                          join J1 in dbContext.compra on n.v_IdCompra equals J1.v_IdCompra into J1_join
                                                          from J1 in J1_join.DefaultIfEmpty()
                                                          join J2 in dbContext.documento on J1.i_IdTipoDocumento equals J2.i_CodigoDocumento into J2_join
                                                          from J2 in J2_join.DefaultIfEmpty()
                                                          join J3 in dbContext.letraspagardetalle on n.v_IdLetrasPagarDetalle equals
                                                              J3.v_IdLetrasPagarDetalle into J3_join
                                                          from J3 in J3_join.DefaultIfEmpty()
                                                          join J4 in dbContext.documento on J3.i_IdTipoDocumento equals J4.i_CodigoDocumento into J4_join
                                                          from J4 in J4_join.DefaultIfEmpty()
                                                          where n.v_IdLetrasPagar == pstrIdLetra && n.i_Eliminado == 0
                                                          select new letraspagarcanjeDto
                                                          {
                                                              TipoDocumento = J1 != null ? J2.v_Siglas : J4.v_Siglas,
                                                              NroDocumento =
                                                                  J1 != null
                                                                      ? J1.v_SerieDocumento + "-" + J1.v_CorrelativoDocumento
                                                                      : J3.v_Serie + "-" + J3.v_Correlativo,
                                                              FechaEmision = J1 != null ? J1.t_FechaRegistro : J3.t_FechaEmision,
                                                              Moneda =
                                                                  J1 != null
                                                                      ? J1.i_IdMoneda == 1 ? "SOLES" : "DÓLARES"
                                                                      : J3.i_IdMoneda == 1 ? "SOLES" : "DÓLARES",
                                                              Importe = J1 != null ? J1.d_Total.Value : J3.d_Importe.Value,
                                                              TipoCambio = J1 != null ? J1.d_TipoCambio.Value : J3.d_TipoCambio.Value,
                                                              d_MontoCanjeado = n.d_MontoCanjeado.Value
                                                          }
                        ).ToList();

                    #endregion

                    pobjOperationResult.Success = 1;
                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasPagarCanje()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<letraspagardetalleDto> ObtenerLetrasPagarDetalle(ref OperationResult pobjOperationResult,
            string pstrIdLetra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var COnsulta =
                        letraspagardetalleAssembler.ToDTOs(
                            dbContext.letraspagardetalle.Where(
                                p => p.v_IdLetrasPagar == pstrIdLetra && p.i_Eliminado == 0).ToList());

                    pobjOperationResult.Success = 1;
                    return new BindingList<letraspagardetalleDto>(COnsulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerLetrasPagarDetalle()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool CompraFueCanjeadaALetras(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    return dbContext.letraspagarcanje.Count(p => p.v_IdCompra == pstrIdCompra && p.i_Eliminado == 0) > 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.CompraFueCanjeadaALetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string InsertarCanjeoLetrasPagar(ref OperationResult pobjOperationResult, letraspagarDto pobjletrasDto,
            List<letraspagarcanjeDto> plobjletrascanjeDto, List<letraspagardetalleDto> plobjletrasdetalleDto,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        letraspagar objEntityLetras = letraspagarAssembler.ToEntity(pobjletrasDto);

                        int SecuentialId = 0;
                        string newIdLetras = string.Empty;
                        string newIdLetrasDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera

                        objEntityLetras.t_InsertaFecha = DateTime.Now;
                        objEntityLetras.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityLetras.i_Eliminado = 0;

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 82);
                        newIdLetras = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PX");
                        objEntityLetras.v_IdLetrasPagar = newIdLetras;
                        dbContext.AddToletraspagar(objEntityLetras);

                        #endregion

                        #region Inserta Canjes

                        foreach (var Canje in plobjletrascanjeDto)
                        {
                            letraspagarcanje _letrascanje = new letraspagarcanje();
                            _letrascanje = letraspagarcanjeAssembler.ToEntity(Canje);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 83);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PZ");
                            _letrascanje.v_IdLetrasPagarCanje = newIdLetrasDetalle;
                            _letrascanje.v_IdLetrasPagar = newIdLetras;
                            _letrascanje.t_InsertaFecha = DateTime.Now;
                            _letrascanje.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrascanje.i_Eliminado = 0;
                            dbContext.AddToletraspagarcanje(_letrascanje);
                            ProcesaDetallePago(ref pobjOperationResult, Canje, pobjletrasDto, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }

                        #endregion

                        #region Inserta Letras Detalle

                        foreach (var LetraDetalle in plobjletrasdetalleDto)
                        {
                            letraspagardetalle _letrasdetalle = new letraspagardetalle();
                            _letrasdetalle = letraspagardetalleAssembler.ToEntity(LetraDetalle);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 84);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PY");
                            _letrasdetalle.v_IdLetrasPagarDetalle = newIdLetrasDetalle;
                            _letrasdetalle.v_IdLetrasPagar = newIdLetras;
                            _letrasdetalle.t_InsertaFecha = DateTime.Now;
                            _letrasdetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrasdetalle.i_Eliminado = 0;
                            LetraDetalle.v_IdLetrasPagarDetalle = newIdLetrasDetalle;
                            dbContext.AddToletraspagardetalle(_letrasdetalle);
                        }

                        #endregion

                        #region Genera Libro Diario

                        string IDConcepto = pobjletrasDto.i_IdMoneda == 1 ? "32" : "33";

                        var aa = (from a in dbContext.administracionconceptos
                                  where a.v_Codigo == IDConcepto && a.v_Periodo.Equals(periodo) && a.i_Eliminado == 0
                                  select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                        if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty &&
                            aa.v_CuentaPVenta.Trim() != string.Empty)
                        {
                            DiarioBL _objDiarioBL = new DiarioBL();
                            diarioDto _diarioDto = new diarioDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                            #region Diario Cabecera
                            var documentoDiario = Globals.ClientSession.i_IdDocumentoContableLEP ?? 335;
                            _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult,
                                pobjletrasDto.t_FechaRegistro.Value.Year.ToString(),
                                pobjletrasDto.t_FechaRegistro.Value.Month.ToString("00"), documentoDiario);

                            var maxMovimiento = _ListadoDiarios.Any()
                                ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count - 1].Value1)
                                : 0;
                            maxMovimiento++;
                            _diarioDto.v_IdDocumentoReferencia = newIdLetras;
                            _diarioDto.v_Periodo = pobjletrasDto.t_FechaRegistro.Value.Year.ToString();
                            _diarioDto.v_Mes =
                                int.Parse(pobjletrasDto.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                            _diarioDto.v_Nombre = pobjletrasDto.NombreProveedor;

                            switch (pobjletrasDto.i_IdEstado)
                            {
                                case 14:
                                    _diarioDto.v_Glosa = "CANJE DE FACTURAS POR LETRAS";
                                    break;

                                case 3:
                                    _diarioDto.v_Glosa = "CANJE DE LETRAS POR RENOVACIÓN";
                                    break;

                                case 7:
                                    _diarioDto.v_Glosa = "CANJE DE LETRAS POR REFINANCIACIÓN";
                                    break;

                                case 8:
                                    _diarioDto.v_Glosa = "CANJE DE LETRAS POR RENOVACIÓN DCTO";
                                    break;
                            }

                            _diarioDto.v_Correlativo = maxMovimiento.ToString("00000000");
                            _diarioDto.d_TipoCambio = pobjletrasDto.d_TipoCambio.Value;
                            _diarioDto.i_IdMoneda = pobjletrasDto.i_IdMoneda.Value;
                            _diarioDto.i_IdTipoDocumento = documentoDiario;
                            _diarioDto.t_Fecha = pobjletrasDto.t_FechaRegistro.Value;
                            _diarioDto.i_IdTipoComprobante = 2;

                            #endregion

                            #region Ventas Canjeadas

                            foreach (var CuentaDetalle in plobjletrascanjeDto)
                            {
                                if (CuentaDetalle.TipoDocumento != "LEP")
                                {
                                    diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                                    var CompraOriginal =
                                        dbContext.compra.Where(p => p.v_IdCompra == CuentaDetalle.v_IdCompra)
                                            .FirstOrDefault();
                                    var IDConceptoVenta = CompraOriginal.i_IdTipoCompra.Value.ToString("00");
                                    decimal SubTotal = CuentaDetalle.d_MontoCanjeado.Value;
                                    H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                    H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal / CompraOriginal.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal * CompraOriginal.d_TipoCambio.Value, 2);
                                    H_SubTotalVenta.i_IdCentroCostos = "0";
                                    H_SubTotalVenta.i_IdTipoDocumento = CompraOriginal.i_IdTipoDocumento.Value;
                                    H_SubTotalVenta.t_Fecha = CompraOriginal.t_FechaRegistro.Value;
                                    H_SubTotalVenta.v_IdCliente = CompraOriginal.v_IdProveedor;
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0
                                        //? CompraOriginal.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H"
                                        //: CompraOriginal.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                         ? !_objDocumentoBl.DocumentoEsInverso(CompraOriginal.i_IdTipoDocumento.Value) ? "D" : "H"
                                        : !_objDocumentoBl.DocumentoEsInverso(CompraOriginal.i_IdTipoDocumento.Value) ? "H" : "D";
                                    H_SubTotalVenta.v_NroDocumento = CompraOriginal.v_SerieDocumento + "-" +
                                                                     CompraOriginal.v_CorrelativoDocumento;
                                    H_SubTotalVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                                   where a.v_Codigo == IDConceptoVenta && a.v_Periodo.Equals(periodo) && a.i_Eliminado == 0
                                                                   select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;

                                    //if (CompraOriginal.i_IdTipoDocumento.Value.ToString() == "7" ||
                                    //    CompraOriginal.i_IdTipoDocumento.Value.ToString() == "8")
                                    if (_objDocumentoBl.DocumentoEsInverso(CompraOriginal.i_IdTipoDocumento.Value) ||
                                        CompraOriginal.i_IdTipoDocumento.Value.ToString() == "8")
                                    {
                                        H_SubTotalVenta.i_IdTipoDocumentoRef = CompraOriginal.i_IdTipoDocumentoRef.Value;
                                        H_SubTotalVenta.v_NroDocumentoRef = CompraOriginal.v_SerieDocumentoRef + "-" +
                                                                            CompraOriginal.v_CorrelativoDocumentoRef;
                                    }

                                    TempXInsertar.Add(H_SubTotalVenta);
                                    H_SubTotalVenta = new diariodetalleDto();
                                }
                                else
                                {
                                    diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                                    var CompraOriginal =
                                        dbContext.letraspagardetalle.Where(
                                            p => p.v_IdLetrasPagarDetalle == CuentaDetalle.v_IdLetrasPagarDetalle)
                                            .FirstOrDefault();
                                    string IDConceptoVenta = CompraOriginal.i_IdMoneda == 1 ? "32" : "33";
                                    decimal SubTotal = CuentaDetalle.d_MontoCanjeado.Value;
                                    H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                    H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal / CompraOriginal.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            SubTotal * CompraOriginal.d_Importe.Value, 2);
                                    H_SubTotalVenta.i_IdCentroCostos = "0";
                                    H_SubTotalVenta.i_IdTipoDocumento = CompraOriginal.i_IdTipoDocumento.Value;
                                    H_SubTotalVenta.t_Fecha = CompraOriginal.t_FechaEmision.Value;
                                    H_SubTotalVenta.v_IdCliente = CompraOriginal.v_IdProveedor;
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0
                                        //? CompraOriginal.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H"
                                        //: CompraOriginal.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        ? !_objDocumentoBl.DocumentoEsInverso(CompraOriginal.i_IdTipoDocumento.Value) ? "D" : "H"
                                        : !_objDocumentoBl.DocumentoEsInverso(CompraOriginal.i_IdTipoDocumento.Value) ? "H" : "D";

                                    H_SubTotalVenta.v_NroDocumento = CompraOriginal.v_Serie + "-" +
                                                                     CompraOriginal.v_Correlativo;
                                    H_SubTotalVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                                   where a.v_Codigo == IDConceptoVenta && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                                   select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                                    TempXInsertar.Add(H_SubTotalVenta);
                                    H_SubTotalVenta = new diariodetalleDto();
                                }
                            }

                            #endregion

                            #region Letras

                            foreach (var Letra in plobjletrasdetalleDto)
                            {
                                var SubTotal = Letra.d_Importe;
                                diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                                H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                H_SubTotalVenta.d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal.Value / pobjletrasDto.d_TipoCambio.Value, 2)
                                    : Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal.Value * pobjletrasDto.d_TipoCambio.Value, 2);
                                H_SubTotalVenta.i_IdCentroCostos = "0";
                                H_SubTotalVenta.i_IdTipoDocumento = Letra.i_IdTipoDocumento.Value;
                                H_SubTotalVenta.t_Fecha = pobjletrasDto.t_FechaRegistro.Value;
                                H_SubTotalVenta.v_IdCliente = Letra.v_IdProveedor;
                                H_SubTotalVenta.v_Naturaleza = "H";
                                H_SubTotalVenta.v_NroDocumento = Letra.v_Serie + "-" + Letra.v_Correlativo;
                                H_SubTotalVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                               where a.v_Codigo == IDConcepto && a.v_Periodo.Equals(periodo) && a.i_Eliminado == 0
                                                               select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                                H_SubTotalVenta.i_IdTipoDocumentoRef = -1;
                                TempXInsertar.Add(H_SubTotalVenta);
                            }

                            #endregion

                            decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                            TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                            TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                            TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                            TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);

                            _diarioDto.d_TotalDebe =
                                decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalHaber =
                                decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalDebeCambio =
                                decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalHaberCambio =
                                decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_DiferenciaDebe =
                                decimal.Parse(
                                    Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_DiferenciaHaber =
                                decimal.Parse(
                                    Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));

                            _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto,
                                Globals.ClientSession.GetAsList(), TempXInsertar, (int)TipoMovimientoTesoreria.Ingreso);
                            if (pobjOperationResult.Success == 0) return string.Empty;
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage =
                                "No existe el concepto 32 (CARTERA MN) o 33 (CARTERA ME) para Letras Pagar.";
                            pobjOperationResult.ExceptionMessage =
                                "No se pudo completar la transacción, por favor agrege los conceptos señalados.";
                            pobjOperationResult.AdditionalInformation = "LetrasBL.InsertarCanjeoLetrasPagar()";
                            return null;
                        }

                        dbContext.SaveChanges();

                        #endregion

                        #region Genera Pendientes por cobrar en letras

                        foreach (var LetraDetalle in plobjletrasdetalleDto)
                        {
                            RegistraPendientePagoLetra(ref pobjOperationResult, LetraDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }

                        #endregion

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                        return newIdLetras;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.InsertarCanjeoLetrasPagar()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void EliminarCanjeoLetrasPagar(ref OperationResult pobjOperationResult, string pstrIdCanjeoLetras, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Elimina Diario
                        new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, pstrIdCanjeoLetras, ClientSession, true);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        #region ELimina Canjeo Cabecera
                        var Cabecera = dbContext.letraspagar.Where(p => p.v_IdLetrasPagar == pstrIdCanjeoLetras).FirstOrDefault();
                        Cabecera.t_ActualizaFecha = DateTime.Now;
                        Cabecera.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Cabecera.i_Eliminado = 1;
                        dbContext.letraspagar.ApplyCurrentValues(Cabecera);
                        #endregion

                        #region Elimina Canjeo Facturas
                        foreach (var item in Cabecera.letraspagarcanje)
                        {
                            item.t_ActualizaFecha = DateTime.Now;
                            item.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            item.i_Eliminado = 1;
                            RestauraDetallePago(ref pobjOperationResult, letraspagarcanjeAssembler.ToDTO(item), letraspagarAssembler.ToDTO(Cabecera), ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                            dbContext.letraspagarcanje.ApplyCurrentValues(item);
                        }
                        #endregion

                        #region Elimina Canjeo Letras Detalle
                        foreach (var item in Cabecera.letraspagardetalle)
                        {
                            item.t_ActualizaFecha = DateTime.Now;
                            item.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            item.i_Eliminado = 1;
                            dbContext.letraspagardetalle.ApplyCurrentValues(item);
                            EliminaPendientePagoLetra(ref pobjOperationResult, item.v_IdLetrasPagarDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.EliminarCanjeoLetrasPagar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Saldo Inicial Letras Pagar
        public List<clienteDto> ObtenerProveedorSaldosIniciales(ref OperationResult pobjOperationResult, string pstrIdCliente = null)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<clienteDto> Resultado = new List<clienteDto>();
                    var Consulta = (from n in dbContext.letraspagardetalle

                                    join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    where n.i_EsSaldoInicial == 1 && n.i_Eliminado == 0

                                    select new clienteDto
                                    {
                                        v_IdCliente = J1.v_IdCliente,
                                        v_CodCliente = J1.v_CodCliente,
                                        v_NroDocIdentificacion = J1.v_NroDocIdentificacion
                                    }
                                    ).ToList();

                    var gConsulta = Consulta.GroupBy(p => p.v_IdCliente).ToList();

                    foreach (var g in gConsulta)
                    {
                        clienteDto c = new clienteDto();
                        c = g.FirstOrDefault();
                        Resultado.Add(c);
                    }

                    if (!string.IsNullOrEmpty(pstrIdCliente)) Resultado = Resultado.Where(p => p.v_IdCliente == pstrIdCliente).ToList();

                    pobjOperationResult.Success = 1;
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerProveedorSaldosIniciales()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<letraspagardetalleDto> ObtenerSaldoInicialLetrasPagarDetalle(ref OperationResult pobjOperationResult, string pstrIdCliente)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var COnsulta = letraspagardetalleAssembler.ToDTOs(dbContext.letraspagardetalle
                        .Where(p => p.v_IdProveedor == pstrIdCliente && p.i_Eliminado == 0 && p.i_EsSaldoInicial == 1).ToList());

                    pobjOperationResult.Success = 1;
                    return new BindingList<letraspagardetalleDto>(COnsulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ObtenerSaldoInicialLetrasPagarDetalle()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizarSaldoInicialLetrasPagar(ref OperationResult pobjOperationResult, List<letraspagardetalleDto> pTemp_InsertarLetras, List<letraspagardetalleDto> pTemp_ModificarLetras, List<letraspagardetalleDto> pTemp_EliminarLetras, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        int SecuentialId = 0;
                        string newIdLetrasDetalle = string.Empty;
                        int intNodeId = int.Parse(ClientSession[0]);

                        #region Inserta Letras Detalle
                        foreach (var LetraDetalle in pTemp_InsertarLetras)
                        {
                            letraspagardetalle _letrasdetalle = new letraspagardetalle();
                            _letrasdetalle = letraspagardetalleAssembler.ToEntity(LetraDetalle);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 84);
                            newIdLetrasDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PY");
                            _letrasdetalle.v_IdLetrasPagarDetalle = newIdLetrasDetalle;
                            _letrasdetalle.t_InsertaFecha = DateTime.Now;
                            _letrasdetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrasdetalle.i_Eliminado = 0;
                            _letrasdetalle.i_EsSaldoInicial = 1;
                            LetraDetalle.v_IdLetrasPagarDetalle = newIdLetrasDetalle;
                            dbContext.AddToletraspagardetalle(_letrasdetalle);
                            dbContext.SaveChanges();

                            RegistraPendientePagoLetra(ref pobjOperationResult, letraspagardetalleAssembler.ToDTO(_letrasdetalle), ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        #endregion

                        #region Modificar Letras Detalle
                        foreach (var LetrasDetalle in pTemp_ModificarLetras)
                        {
                            var query = dbContext.letraspagardetalle
                                .Where(p => p.v_IdLetrasPagarDetalle == LetrasDetalle.v_IdLetrasPagarDetalle)
                                .FirstOrDefault();

                            letraspagardetalle _letrasdetalle = new letraspagardetalle();
                            _letrasdetalle = letraspagardetalleAssembler.ToEntity(LetrasDetalle);
                            _letrasdetalle.t_ActualizaFecha = DateTime.Now;
                            _letrasdetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.letraspagardetalle.ApplyCurrentValues(_letrasdetalle);

                            EliminaPendientePagoLetra(ref pobjOperationResult, _letrasdetalle.v_IdLetrasPagarDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                            RegistraPendientePagoLetra(ref pobjOperationResult, letraspagardetalleAssembler.ToDTO(_letrasdetalle), ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        #endregion

                        #region Eliminar Letras Detalle
                        foreach (var LetrasDetalle in pTemp_EliminarLetras)
                        {
                            var query = dbContext.letraspagardetalle
                                .Where(p => p.v_IdLetrasPagarDetalle == LetrasDetalle.v_IdLetrasPagarDetalle)
                                .FirstOrDefault();

                            letraspagardetalle _letrasdetalle = new letraspagardetalle();
                            _letrasdetalle = letraspagardetalleAssembler.ToEntity(LetrasDetalle);
                            _letrasdetalle.t_ActualizaFecha = DateTime.Now;
                            _letrasdetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            _letrasdetalle.i_Eliminado = 1;
                            dbContext.letraspagardetalle.ApplyCurrentValues(_letrasdetalle);
                            EliminaPendientePagoLetra(ref pobjOperationResult, _letrasdetalle.v_IdLetrasPagarDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ActualizarSaldoInicialLetrasPagar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarSaldoInicialPagar(ref OperationResult pobjOperationResult, string pstrIdCliente, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var SaldosIniciales = dbContext.letraspagardetalle
                            .Where(p => p.v_IdProveedor == pstrIdCliente && p.i_Eliminado == 0 && p.i_EsSaldoInicial == 1)
                            .ToList();

                        #region Elimina Canjeo Letras Detalle
                        foreach (var item in SaldosIniciales)
                        {
                            item.t_ActualizaFecha = DateTime.Now;
                            item.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            item.i_Eliminado = 1;
                            dbContext.letraspagardetalle.ApplyCurrentValues(item);
                            EliminaPendientePagoLetra(ref pobjOperationResult, item.v_IdLetrasPagarDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        #endregion

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.EliminarSaldoInicialPagar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Procesos
        private void RestauraDetallePago(ref OperationResult pobjOperationResult, letraspagarcanjeDto cobranzadetalleDto, letraspagarDto PagoEntity, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    if (cobranzadetalleDto.v_IdCompra != null)
                    {
                        #region Actualiza la deuda de una compra
                        var cps = dbContext.pagopendiente.Where(p => p.v_IdCompra == cobranzadetalleDto.v_IdCompra && p.i_Eliminado == 0).ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar
                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.compra
                                                     where m.v_IdCompra == cp.v_IdCompra && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                switch (PagoEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;
                                        }
                                        break;
                                }

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pagopendiente", cp.v_IdPagoPendiente);
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region Actualiza la deuda de una letra
                        var cps = dbContext.cobranzaletraspagarpendiente.Where(p => p.v_IdLetrasPagarDetalle == cobranzadetalleDto.v_IdLetrasPagarDetalle && p.i_Eliminado == 0).ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar
                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.letraspagardetalle
                                                     where m.v_IdLetrasPagarDetalle == cp.v_IdLetrasPagarDetalle && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                switch (PagoEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado / VentaOriginal.d_TipoCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_MontoCanjeado * VentaOriginal.d_TipoCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;
                                        }
                                        break;
                                }

                                if (cp.d_Saldo == 0) cp.letraspagardetalle.i_Pagada = 1;
                                else cp.letraspagardetalle.i_Pagada = 0;

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzaletraspagarpendiente", cp.v_IdCobranzaLetrasPagarPendiente);
                            }
                            #endregion
                        }
                        #endregion
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RestauraDetallePago()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        private void ProcesaDetallePago(ref OperationResult pobjOperationResult, letraspagarcanjeDto cobranzadetalleDto, letraspagarDto PagoEntity, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (cobranzadetalleDto.v_IdCompra != null)
                    {
                        #region Actualiza la deuda de una venta
                        var cps = dbContext.pagopendiente.Where(p => p.v_IdCompra == cobranzadetalleDto.v_IdCompra && p.i_Eliminado == 0).ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar
                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.compra
                                                     where m.v_IdCompra == cp.v_IdCompra && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                decimal TCambio = VentaOriginal.d_TipoCambio.Value;

                                switch (PagoEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_MontoCanjeado / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_MontoCanjeado * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;
                                        }
                                        break;
                                }

                                dbContext.pagopendiente.ApplyCurrentValues(cp);

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pagopendiente", cp.v_IdPagoPendiente);
                            }
                            #endregion
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró el pago pendiente";
                            pobjOperationResult.AdditionalInformation = "LetrasBL.ProcesaDetallePago()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }
                        #endregion

                    }
                    else
                    {
                        #region Actualiza la deuda de una letra
                        var cps = dbContext.cobranzaletraspagarpendiente.Where(p => p.v_IdLetrasPagarDetalle == cobranzadetalleDto.v_IdLetrasPagarDetalle && p.i_Eliminado == 0).ToList();

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar
                            foreach (var cp in cps)
                            {
                                var VentaOriginal = (from m in dbContext.letraspagardetalle
                                                     where m.v_IdLetrasPagarDetalle == cp.v_IdLetrasPagarDetalle && m.i_Eliminado == 0
                                                     select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                                int Moneda = VentaOriginal.i_IdMoneda.Value;

                                decimal TCambio = VentaOriginal.d_TipoCambio.Value;

                                switch (PagoEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_MontoCanjeado / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_MontoCanjeado * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                                cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado : 0;

                                                break;
                                        }
                                        break;
                                }

                                if (cp.d_Saldo == 0) cp.letraspagardetalle.i_Pagada = 1;
                                else cp.letraspagardetalle.i_Pagada = 0;

                                dbContext.cobranzaletraspagarpendiente.ApplyCurrentValues(cp);

                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzaletraspagarpendiente", cp.v_IdCobranzaLetrasPagarPendiente);
                            }
                            #endregion
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                            pobjOperationResult.AdditionalInformation = "LetrasBL.ProcesaDetallePago()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }
                        #endregion
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.ProcesaDetallePago()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        private void RegistraPendientePagoLetra(ref OperationResult pobjOperationResult, letraspagardetalleDto _letrasdetalle, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    cobranzaletraspagarpendiente objEntityLetras = new cobranzaletraspagarpendiente();
                    int SecuentialId = 0;
                    string newIdLetras = string.Empty;
                    int intNodeId;

                    objEntityLetras.t_InsertaFecha = DateTime.Now;
                    objEntityLetras.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityLetras.i_Eliminado = 0;
                    objEntityLetras.v_IdLetrasPagarDetalle = _letrasdetalle.v_IdLetrasPagarDetalle;
                    objEntityLetras.d_Acuenta = 0;
                    objEntityLetras.d_Saldo = _letrasdetalle.d_Importe;

                    intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 85);
                    newIdLetras = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PW");
                    objEntityLetras.v_IdCobranzaLetrasPagarPendiente = newIdLetras;
                    dbContext.AddTocobranzaletraspagarpendiente(objEntityLetras);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.RegistraPendientePagoLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private void EliminaPendientePagoLetra(ref OperationResult pobjOperationResult, string pstrIdLetraDetalle, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var CobranzaPendiente = dbContext.cobranzaletraspagarpendiente.Where(p => p.v_IdLetrasPagarDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).FirstOrDefault();

                    if (CobranzaPendiente != null)
                    {
                        CobranzaPendiente.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        CobranzaPendiente.t_ActualizaFecha = DateTime.Now;
                        CobranzaPendiente.i_Eliminado = 1;
                        dbContext.cobranzaletraspagarpendiente.ApplyCurrentValues(CobranzaPendiente);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.EliminaPendientePagoLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public bool LetraFuePagada(ref OperationResult pobjOperationResult, string pstrIdLetras)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Letras = dbContext.letraspagardetalle.Where(p => p.v_IdLetrasPagar == pstrIdLetras && p.i_Eliminado == 0).ToList();
                    pobjOperationResult.Success = 1;

                    foreach (var Letra in Letras)
                    {
                        if (dbContext.cobranzaletraspendiente.Count(p => p.v_IdLetrasDetalle == Letra.v_IdLetrasPagarDetalle && p.i_Eliminado == 0 && p.d_Acuenta.Value > 0) > 0)
                        {
                            return true;
                        }

                        return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.LetraFuePagada()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool LetraDetalleFuePagada(ref OperationResult pobjOperationResult, string pstrIdLetraDetalle)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    if (dbContext.cobranzaletraspagarpendiente.Count(p => p.v_IdLetrasPagarDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0 && p.d_Acuenta.Value > 0) > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.LetraDetalleFuePagada()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool SaldoInicialFuePagada(ref OperationResult pobjOperationResult, string pstrIdCliente)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Letras = dbContext.letraspagardetalle.Where(p => p.v_IdProveedor == pstrIdCliente && p.i_Eliminado == 0 && p.i_EsSaldoInicial == 1).ToList();
                    pobjOperationResult.Success = 1;

                    foreach (var Letra in Letras)
                    {
                        if (dbContext.cobranzaletraspagarpendiente.Count(p => p.v_IdLetrasPagarDetalle == Letra.v_IdLetrasPagarDetalle && p.i_Eliminado == 0 && p.d_Acuenta.Value > 0) > 0)
                        {
                            return true;
                        }

                        return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.SaldoInicialFuePagada()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool LetraPagarRegistradaAnteriormente(ref OperationResult pobjOperationResult, string pstrIdProveedor, string pstrSerie, string pstrCorrelativo)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Count = dbContext.letraspagardetalle
                        .Count(p => p.v_IdProveedor == pstrIdProveedor && p.v_Correlativo == pstrCorrelativo && p.v_Serie == pstrSerie && p.i_Eliminado == 0);

                    pobjOperationResult.Success = 1;
                    return Count > 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "LetrasBL.LetraPagarRegistradaAnteriormente()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }
        #endregion

        #endregion
    }
}
