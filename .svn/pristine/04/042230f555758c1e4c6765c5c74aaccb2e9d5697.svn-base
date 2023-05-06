using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using System.Data.Objects.SqlClient;

namespace SAMBHS.Cobranza.BL
{
    public class FormaPagoDocumentoBL
    {
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public IQueryable ObtenerRelaciones(ref OperationResult pobjOperationResult)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                IQueryable Listado = (from n in dbContext.relacionformapagodocumento
                                      select n);

                pobjOperationResult.Success = 1;
                return Listado;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormaPagoDocumentoBL.ObtenerRelaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                return null;
            }
        }

        public void ActualizarRelacion(ref OperationResult pobjOperationResult, List<relacionformapagodocumentoDto> pTemp_Insertar, List<relacionformapagodocumentoDto> pTemp_Editar, List<relacionformapagodocumentoDto> pTemp_Eliminar, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar
                    foreach (var Relacion in pTemp_Insertar)
                    {
                        var Entidad = relacionformapagodocumentoAssembler.ToEntity(Relacion);
                        Entidad.t_InsertaFecha = DateTime.Now;
                        Entidad.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.relacionformapagodocumento.AddObject(Entidad);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "relacionformapagodocumento", "");
                    }
                    #endregion

                    #region Modificar
                    foreach (var Relacion in pTemp_Editar)
                    {
                        var Entidad = (from n in dbContext.relacionformapagodocumento
                                       where n.i_IdRelacion == Relacion.i_IdRelacion
                                       select n).FirstOrDefault();

                        if (Entidad != null)
                        {
                            Entidad.i_IdFormaPago = Relacion.i_IdFormaPago;
                            Entidad.i_CodigoDocumento = Relacion.i_CodigoDocumento;
                            Entidad.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            Entidad.t_ActualizaFecha = DateTime.Now;
                            dbContext.relacionformapagodocumento.ApplyCurrentValues(Entidad);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "relacionformapagodocumento", Entidad.i_IdRelacion.ToString());
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.AdditionalInformation = "FormaPagoDocumentoBL.ActualizarRelacion()";
                            pobjOperationResult.ErrorMessage = "No se encontró la entidad relacionformapagodocumento";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                            return;
                        }
                    }

                    #endregion

                    #region Eliminar
                    foreach (var Relacion in pTemp_Eliminar)
                    {
                        var Entidad = (from n in dbContext.relacionformapagodocumento
                                       where n.i_IdRelacion == Relacion.i_IdRelacion
                                       select n).FirstOrDefault();

                        dbContext.relacionformapagodocumento.DeleteObject(Entidad);
                    }
                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormaPagoDocumentoBL.ActualizarRelacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public int DevuelveComprobantePorFormaPago(int IdFormaPago, out bool RequiereNroDocumento, out int idMoneda)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                RequiereNroDocumento = false;
                idMoneda = 1;
                var Documento = (from n in dbContext.relacionformapagodocumento

                    join J2 in dbContext.datahierarchy on new { g = 46, fp = IdFormaPago }
                    equals new { g = J2.i_GroupId, fp = J2.i_ItemId } into J2_join
                    from J2 in J2_join.DefaultIfEmpty()

                    join J4 in dbContext.documento on n.i_CodigoDocumento equals J4.i_CodigoDocumento into J4_join
                    from J4 in J4_join.DefaultIfEmpty()

                    join J3 in dbContext.asientocontable on new { cta = J4.v_NroCuenta, e = 0, p = periodo } 
                                 equals new { cta = J3.v_NroCuenta, e = J3.i_Eliminado ?? 0, p = J3.v_Periodo } into J3_join
                    from J3 in J3_join.DefaultIfEmpty()

                    where n.i_IdFormaPago == IdFormaPago

                    select new
                    {
                        n.i_CodigoDocumento, J2.v_Field,
                        idMoneda = J3 != null ? J3.i_IdMoneda ?? 1 :  1
                    }
                    ).ToList().Select(p => new
                        {
                            p.i_CodigoDocumento,
                            RequiereDocumento = RequiereSerieNumero(p.v_Field), p.idMoneda
                        }
                    ).FirstOrDefault();

                if (Documento != null)
                {
                    RequiereNroDocumento = Documento.RequiereDocumento;
                    idMoneda = Documento.idMoneda;
                    return (int)Documento.i_CodigoDocumento;
                }
                return -1;
            }
        }

        private bool RequiereSerieNumero(string strCodDocumento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                int intCodDocumento;
                intCodDocumento = int.TryParse(strCodDocumento, out intCodDocumento) ? intCodDocumento : 0;

                if (intCodDocumento != 0)
                {
                    var Documento = dbContext.documento.Where(p => p.i_CodigoDocumento == intCodDocumento && p.i_Eliminado == 0)
                        .Select(o => o.i_RequiereSerieNumero).FirstOrDefault();

                    return Documento == 1;
                }
                return false;
            }
        }
    }
}
