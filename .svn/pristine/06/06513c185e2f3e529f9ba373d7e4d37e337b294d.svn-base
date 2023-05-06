using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Transactions;

namespace SAMBHS.CommonWIN.BL
{
    public class TransportistaBL
    {
        public List<transportistaDto> ObtenerListadoTransportista(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.transportista
                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                                equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                                equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()

                                join J3 in dbContext.systemparameter on new { TipoIdentificacion = A.i_IdTipoIdentificacion.Value }
                                                                equals new { TipoIdentificacion = J3.i_ParameterId } into J3_join
                                from J3 in J3_join.DefaultIfEmpty()

                                where A.i_Eliminado == 0 && J3.i_GroupId == 150
                                select new transportistaDto
                                {
                                    v_Codigo = A.v_Codigo,
                                    v_IdTransportista = A.v_IdTransportista,
                                    v_NombreRazonSocial = A.v_NombreRazonSocial,
                                    v_NombreContacto = A.v_NombreContacto,
                                    v_Direccion = A.v_Direccion,
                                    i_IdTipoPersona = A.i_IdTipoPersona,
                                    i_IdTipoIdentificacion = A.i_IdTipoIdentificacion,
                                    v_NumeroDocumento = A.v_NumeroDocumento,
                                    i_IdPais = A.i_IdPais,
                                    i_IdDepartamento = A.i_IdDepartamento,
                                    i_IdProvincia = A.i_IdProvincia,
                                    i_IdDistrito = A.i_IdDistrito,
                                    v_Telefono = A.v_Telefono,
                                    v_Fax = A.v_Fax,
                                    v_CorreoElectronico = A.v_CorreoElectronico,
                                    i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                    t_InsertaFecha = A.t_InsertaFecha,
                                    i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                    t_ActualizaFecha = A.t_ActualizaFecha,
                                    v_UsuarioCreacion = J1.v_UserName,
                                    v_UsuarioModificacion = J2.v_UserName,
                                    TipoDocumento = J3.v_Value1
                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<transportistaDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public transportistaDto ObtenerTransportista(ref OperationResult pobjOperationResult, string pstringIdTransportista)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    transportistaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.transportista
                                     where a.v_IdTransportista == pstringIdTransportista
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = transportistaAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objDtoEntity; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public transportistaDto ObtenerTransportistaporNroDocumento(ref OperationResult pobjOperationResult, string pstrNroDocumento)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    transportistaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.transportista
                                     where a.v_NumeroDocumento == pstrNroDocumento && a.i_Eliminado == 0
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = objEntity.ToDTO();

                    pobjOperationResult.Success = 1;
                    return objDtoEntity; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public static bool ExisteTransportista(string pstrCodigo, string pstrIdTransportista)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (pstrIdTransportista == null)
                        return dbContext.transportista.Any(p => p.v_Codigo.Trim().Equals(pstrCodigo.Trim()) && p.i_Eliminado == 0);

                    return dbContext.transportista.Any(p => p.v_Codigo.Trim().Equals(pstrCodigo.Trim()) && !p.v_IdTransportista.Equals(pstrIdTransportista) && p.i_Eliminado == 0);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void InsertarTransportista(ref OperationResult pobjOperationResult, transportistaDto pobjDtoEntity, List<string> ClientSession, bool RegistraRapido, transportistachoferDto objTransportistaChofer = null, transportistaunidadtransporteDto objTransportistaUnidadT = null)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    #region Transportista
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    transportista objEntity = transportistaAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId,10);
                    string newIdTransportista = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "TR");
                    objEntity.v_IdTransportista = newIdTransportista;
                    dbContext.AddTotransportista(objEntity);

                    #endregion
                    if (RegistraRapido) //VentaRapida ,se guarda chofer ,marca y tracto
                    {
                        #region TransportistaChofer
                        if (objTransportistaChofer != null)
                        {

                            if (objTransportistaChofer.v_Brevete != string.Empty && objTransportistaChofer.v_NombreCompleto != string.Empty)
                            {

                                objTransportistaChofer.v_IdTransportista = newIdTransportista;
                                transportistachofer objEntityTransportistaChofer = objTransportistaChofer.ToEntity();
                                objEntityTransportistaChofer.t_InsertaFecha = DateTime.Now;
                                objEntityTransportistaChofer.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityTransportistaChofer.i_Eliminado = 0;

                                // Autogeneramos el Pk de la tabla
                                intNodeId = int.Parse(ClientSession[0]);
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 16);
                                string newIdTransportistaChofer = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CF");
                                objEntityTransportistaChofer.v_IdChofer = newIdTransportistaChofer;
                                dbContext.AddTotransportistachofer(objEntityTransportistaChofer);
                            }

                        }
                        #endregion

                        #region UnidadTransporte



                        if (objTransportistaUnidadT != null)
                        {
                            if (!string.IsNullOrEmpty(objTransportistaUnidadT.v_TractoCertificado) || !string.IsNullOrEmpty(objTransportistaUnidadT.v_TractoMarca) || !string.IsNullOrEmpty(objTransportistaUnidadT.v_TractoPlaca))
                            {
                                transportistaunidadtransporte objEntityUnidadT = transportistaunidadtransporteAssembler.ToEntity(objTransportistaUnidadT);
                                objEntityUnidadT.t_InsertaFecha = DateTime.Now;
                                objEntityUnidadT.v_IdTransportista = newIdTransportista;
                                objEntityUnidadT.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityUnidadT.i_Eliminado = 0;
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 17);
                                string newIdTransportistaUnidadT = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "UN");
                                objEntityUnidadT.v_IdUnidadTransporte = newIdTransportistaUnidadT;
                                dbContext.AddTotransportistaunidadtransporte(objEntityUnidadT);
                            }

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
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public void ActualizarTransportista(ref OperationResult pobjOperationResult, transportistaDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.transportista
                                       where a.v_IdTransportista == pobjDtoEntity.v_IdTransportista
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                transportista objEntity = transportistaAssembler.ToEntity(pobjDtoEntity);
                dbContext.transportista.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public void EliminarTransportista(ref OperationResult pobjOperationResult, string pstrIdTransportista, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.transportista
                                       where a.v_IdTransportista == pstrIdTransportista
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;

                //Eliminar choferes del transportista eliminado.
                var objEntitySourceChoferes = (from a in dbContext.transportistachofer
                                               where a.v_IdTransportista == pstrIdTransportista
                                               select a).ToList();

                foreach (var RegistroTransportistaChofer in objEntitySourceChoferes)
                {
                    RegistroTransportistaChofer.t_ActualizaFecha = DateTime.Now;
                    RegistroTransportistaChofer.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    RegistroTransportistaChofer.i_Eliminado = 1;
                }

                //Eliminar unidades de transporte del transportista eliminado.
                var objEntitySourceUnidadesTransporte = (from a in dbContext.transportistaunidadtransporte
                                                         where a.v_IdTransportista == pstrIdTransportista
                                                         select a).ToList();

                foreach (var RegistroTransportistaChofer in objEntitySourceUnidadesTransporte)
                {
                    RegistroTransportistaChofer.t_ActualizaFecha = DateTime.Now;
                    RegistroTransportistaChofer.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    RegistroTransportistaChofer.i_Eliminado = 1;
                }

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public bool TransportistasenGuiasRemision(ref  OperationResult pobjOperationResult, string pstrIdTransportista)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var TransportistasenGuia = (from a in dbContext.guiaremision
                                            where a.i_Eliminado == 0 && a.v_IdTransportista == pstrIdTransportista
                                      select a).ToList();

                return TransportistasenGuia.Any() ? true : false;
            
            
            
            }
        
        
        }
    
    }
}
