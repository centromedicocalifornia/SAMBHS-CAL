using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq.Dynamic;

namespace SAMBHS.Contabilidad.BL
{
    public class AdministracionConceptosBL
    {
        string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public List<administracionconceptosDto> ObtenerListadoAdministracionConceptos(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
               

                var query = from A in dbContext.administracionconceptos

                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                        equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                        equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            where A.i_Eliminado == 0 && A.v_Periodo.Equals(periodo)

                            select new administracionconceptosDto
                            {

                                v_IdAdministracionConceptos = A.v_IdAdministracionConceptos,
                                v_Codigo = A.v_Codigo,
                                v_Nombre = A.v_Nombre,
                                v_CuentaPVenta=A.v_CuentaPVenta,
                                v_CuentaIGV=A.v_CuentaIGV,
                                v_CuentaDetraccion=A.v_CuentaDetraccion,
                                i_Eliminado = A.i_Eliminado,
                                i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                t_InsertaFecha = A.t_InsertaFecha,
                                v_UsuarioCreacion = J1.v_UserName,
                                v_UsuarioModificacion = J2.v_UserName,
                                t_ActualizaFecha = A.t_ActualizaFecha
                                
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<administracionconceptosDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public bool ExistenciaAdmConceptosDiversosProcesos(int CodigoConcepto)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var Ventas = (from a in dbContext.venta
                              where a.i_Eliminado == 0 && a.i_IdTipoVenta == CodigoConcepto 

                              select a).ToList();
                var Compras = (from a in dbContext.compra

                               where a.i_Eliminado == 0 && a.i_IdTipoCompra == CodigoConcepto
                               select a).ToList();
                //var Importaciones = (from a in dbContext.importacion

                //                     where a.i_Eliminado == 0

                //                     select a).ToList();


                if (Ventas.Any() || Compras.Any() || ((CodigoConcepto == (int)Concepto.Percepcion || CodigoConcepto == (int)Concepto.CuartaCategoria || CodigoConcepto == (int)Concepto.PorPagarSoles || CodigoConcepto == (int)Concepto.PorPagarDolares || CodigoConcepto == (int)Concepto.ProveedoresAdvalorem || CodigoConcepto == (int)Concepto.ProveedoresFlete || CodigoConcepto == (int)Concepto.ProveedoresSeguro || CodigoConcepto == (int)Concepto.ProveedorIgv || CodigoConcepto == (int)Concepto.ProveedorPercepcion || CodigoConcepto == (int)Concepto.ValorFob || CodigoConcepto == (int)Concepto.Flete || CodigoConcepto == (int)Concepto.Seguro || CodigoConcepto == (int)Concepto.AdValorem) || CodigoConcepto == (int)Concepto.Igv || CodigoConcepto == (int)Concepto.ProveedoresFob))
                { 
                
                 return true ;

                }
                else 
                {
                return false ;
                }
            
            }

        }


        public void EliminarAdministracionConceptos(ref OperationResult pobjOperationResult, string pstrIdAdministracionConceptos, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.administracionconceptos 
                                       where a.v_IdAdministracionConceptos  == pstrIdAdministracionConceptos
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;
                dbContext.SaveChanges();
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "administracionconceptos", objEntitySource.v_IdAdministracionConceptos);
                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdministracionConceptosBL.EliminarAdministracionConceptos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public string InsertarAdministracionConceptos(ref OperationResult pobjOperationResult, administracionconceptosDto pobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                SecuentialBL objSecuentialBL = new SecuentialBL();
                administracionconceptos objEntity = administracionconceptosAssembler.ToEntity(pobjDtoEntity);
                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;
                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 35);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZV");
                objEntity.v_IdAdministracionConceptos  = newId;
                dbContext.AddToadministracionconceptos(objEntity);
                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "administracionconceptos", newId);
                return newId;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdministracionConceptosBL.InsertarAdministracionConceptos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }

        public string  ActualizarAdministracionConceptos(ref OperationResult pobjOperationResult, administracionconceptosDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.administracionconceptos 
                                       where a.v_IdAdministracionConceptos == pobjDtoEntity.v_IdAdministracionConceptos
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                administracionconceptos objEntity = administracionconceptosAssembler.ToEntity(pobjDtoEntity);

                dbContext.administracionconceptos.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "administracionconceptos", objEntitySource.v_IdAdministracionConceptos);
                return pobjDtoEntity.v_IdAdministracionConceptos ;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdministracionConceptosBL.ActualizarAdministracionConceptos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }

        public administracionconceptosDto ObtenerAdministracionConceptos(ref OperationResult pobjOperationResult, string pstrAdministracionConceptosId)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                administracionconceptosDto objDtoEntity = null;

                var objEntity = (from a in dbContext.administracionconceptos
                                 where a.v_IdAdministracionConceptos == pstrAdministracionConceptosId && a.i_Eliminado ==0
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = administracionconceptosAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;
                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdministracionConceptosBL.ObtenerAdministracionConceptos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }

        public administracionconceptosDto ObtenerAdministracionConceptosxCod(ref OperationResult pobjOperationResult, string codigoAdministracion)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                administracionconceptosDto objDtoEntity = null;

                var objEntity = (from a in dbContext.administracionconceptos
                                 where a.v_Codigo == codigoAdministracion && a.i_Eliminado ==0   && a.v_Periodo.Equals(periodo)
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = administracionconceptosAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;
                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdministracionConceptosBL.ObtenerAdministracionConceptosxCod()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }


    }
}
