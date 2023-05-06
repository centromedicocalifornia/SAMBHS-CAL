using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Requerimientos.NBS
{
    public class VentaNbsBl
    {
        public static List<ventaDto> ListarBusquedaVentas(ref OperationResult pobjOperationResult,
           string pstrSortExpression, string pstrIdCliente, DateTime F_Ini, DateTime F_Fin, int idDocumento = -1, string serie = null, string correlativo = null, int IdTipoOperacion = -1, int idEstablecimiento = -1)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.venta
                                 join A in dbContext.cliente on n.v_IdCliente equals A.v_IdCliente into A_join
                                 from A in A_join.DefaultIfEmpty()
                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                     equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()
                                 join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                     equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()
                                 join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento ?? 1 }
                                     equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()
                                 join J5 in dbContext.cobranzapendiente on new { idventa = n.v_IdVenta, eliminado = 0 }
                                     equals new { idventa = J5.v_IdVenta, eliminado = J5.i_Eliminado ?? 0 } into J5_join
                                 from J5 in J5_join.DefaultIfEmpty()
                                 
                                 where
                                     n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Ini && n.t_FechaRegistro <= F_Fin &&
                                     (idEstablecimiento == -1 || n.i_IdEstablecimiento == idEstablecimiento)
                                 orderby n.t_InsertaFecha descending
                                 select new ventaDto
                                 {
                                     v_IdVenta = n.v_IdVenta,
                                     v_Mes = n.v_Mes,
                                     v_Correlativo = n.v_Correlativo,
                                     v_SerieDocumento = n.v_SerieDocumento,
                                     v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                     NroRegistro = n.v_Mes.Trim() + "-" + n.v_Correlativo,
                                     Documento = n.v_SerieDocumento + " - " + n.v_CorrelativoDocumento,
                                     i_IdTipoDocumento = n.i_IdTipoDocumento,
                                     TipoDocumento = J4 != null ? J4.v_Siglas : "",
                                     t_FechaRegistro = n.t_FechaRegistro ?? null,
                                     v_IdCliente = n.v_IdCliente,
                                     CodigoCliente = A != null ? A.v_CodCliente : "",
                                     NombreCliente = A != null ?
                                         n.v_IdCliente != "N002-CL000000000"
                                             ? (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " +
                                                A.v_RazonSocial).Trim()
                                             : (n.v_NombreClienteTemporal != null && n.v_NombreClienteTemporal.Trim() != "") ? n.v_NombreClienteTemporal : "PÚBLICO GENERAL" : "",
                                     d_Total = n.d_Total,
                                     i_IdEstado = n.i_IdEstado,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     v_UsuarioModificacion = J2 != null ? J2.v_UserName : "",
                                     v_UsuarioCreacion = J3 != null ? J3.v_UserName : "",
                                     Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                     Saldo = J5 != null ? J5.d_Saldo ?? 0 : 0,
                                     TieneGRM = !string.IsNullOrEmpty(n.v_NroGuiaRemisionSerie) && !string.IsNullOrEmpty(n.v_NroGuiaRemisionCorrelativo) ? n.v_NroGuiaRemisionSerie.Trim() + "-" + n.v_NroGuiaRemisionCorrelativo.Trim() : "", // n.v_NroGuiaRemisionSerie.Trim() != "",
                                     Origen = "V",
                                     NroDocCliente = A != null ? A.v_NroDocIdentificacion : "",
                                     d_TipoCambio = n.d_TipoCambio ?? 0,
                                     i_EstadoSunat = n.i_EstadoSunat,
                                     i_IdTipoOperacion = n.i_IdTipoOperacion,
                                     DsKardexs = (from a in dbContext.nbs_ventakardex
                                                  where a.v_IdVenta == n.v_IdVenta && a.i_Eliminado == 0
                                                  select a.v_TipoKardex.Trim() + "-" + a.v_NroKardex.Trim()).AsEnumerable()
                                 }
                                );

                    if (!string.IsNullOrWhiteSpace(pstrIdCliente))
                        query = query.Where(n => n.v_IdCliente.Equals(pstrIdCliente));

                    if (idDocumento != -1)
                        query = query.Where(n => n.i_IdTipoDocumento == idDocumento);

                    if (!string.IsNullOrWhiteSpace(serie))
                        query = query.Where(n => n.v_SerieDocumento.Equals(serie));

                    if (!string.IsNullOrWhiteSpace(correlativo))
                        query = query.Where(n => n.v_CorrelativoDocumento.Equals(correlativo));

                    if (IdTipoOperacion != -1)
                    {
                        query = query.Where(n => (n.i_IdTipoOperacion) == IdTipoOperacion);
                    }
                   
                    var result = query.ToList();
                    result.ForEach(p => p.Kardexs = p.DsKardexs.ToList().Any() ? string.Join(", ", p.DsKardexs.ToList()) :  null);
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

    }
}
