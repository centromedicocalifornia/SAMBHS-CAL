using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Venta.BL
{
    public class CarteraClienteBL
    {
        public List<carteraclienteDto> ObtenerListadoCarteraClientePorVendedor(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdVendedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.carteracliente
                        join J1 in dbContext.systemuser on new {i_InsertUserId = A.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = A.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()

                        join J3 in dbContext.cliente on new {v_IdCliente = A.v_IdCliente}
                        equals new {v_IdCliente = J3.v_IdCliente} into J3_join
                        from J3 in J3_join.DefaultIfEmpty()

                        join J4 in dbContext.systemparameter on
                        new {TipoIdentificacion = J3.i_IdTipoIdentificacion.Value}
                        equals new {TipoIdentificacion = J4.i_ParameterId} into J4_join
                        from J4 in J4_join.DefaultIfEmpty()

                        where A.i_Eliminado == 0 && J4.i_GroupId == 150 && A.v_IdVendedor == pstrIdVendedor

                        select new carteraclienteDto
                        {
                            v_IdCarteraCliente = A.v_IdCarteraCliente,
                            v_IdCliente = A.v_IdCliente,
                            v_IdVendedor = A.v_IdVendedor,
                            t_InsertaFecha = A.t_InsertaFecha,
                            t_ActualizaFecha = A.t_ActualizaFecha,
                            v_ClienteNombreRazonSocial =
                            (J3.v_PrimerNombre.Trim() + " " + J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() +
                             " " + J3.v_RazonSocial.Trim()).Trim(),
                            v_CodigoCliente = J3.v_CodCliente.Trim(),
                            v_NroDocumento = J3.v_NroDocIdentificacion,
                            v_TipoDocumento = J4.v_Value1,
                            v_UsuarioCreacion = J1.v_UserName,
                            v_UsuarioModificacion = J2.v_UserName,
                        };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<carteraclienteDto> objData = query.ToList();
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

        public carteraclienteDto ObtenerCarteraCliente(ref OperationResult pobjOperationResult, string pstringIdcarteracliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    carteraclienteDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.carteracliente
                        where a.v_IdCarteraCliente == pstringIdcarteracliente
                        select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = carteraclienteAssembler.ToDTO(objEntity);

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

        public int ConsultarSiExisteCliente(ref OperationResult pobjOperationResult, string pstringIdCliente)
        { 
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.carteracliente
                        join J1 in dbContext.vendedor on new {ID_Vendedor = a.v_IdVendedor}
                        equals new {ID_Vendedor = J1.v_IdVendedor} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        where a.v_IdCliente == pstringIdCliente && a.i_Eliminado == 0 && J1.i_Eliminado == 0
                        select a).FirstOrDefault();

                    return objEntity != null ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return -1;
            }
        }
        
        public void InsertarCarteraCliente(ref OperationResult pobjOperationResult, carteraclienteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    carteracliente objEntity = carteraclienteAssembler.ToEntity(pobjDtoEntity);


                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 25);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CC");
                    objEntity.v_IdCarteraCliente = newId;


                    dbContext.AddTocarteracliente(objEntity);
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public void EliminarCarteraCliente(ref OperationResult pobjOperationResult, string pstrIdcarteracliente, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.carteracliente
                        where a.v_IdCarteraCliente == pstrIdcarteracliente
                        select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public carteraclienteDto ObtenerNombreVendedor(ref OperationResult pobjOperationResult, string IdCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from A in dbContext.carteracliente

                        join J1 in dbContext.vendedor on new {IdVendedor = A.v_IdVendedor}
                        equals new {IdVendedor = J1.v_IdVendedor} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        where A.i_Eliminado == 0 && A.v_IdCliente == IdCliente

                        select new carteraclienteDto
                        {
                            NombreVendedor = J1.v_NombreCompleto
                        }
                    ).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<ReporteClienteVendedorDto> ObtenerReporteCarteraClientePorVendedor(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.carteracliente
                        join J1 in dbContext.systemuser on new {i_InsertUserId = A.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = A.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()

                        join B in dbContext.vendedor on A.v_IdVendedor equals B.v_IdVendedor into B_join
                        from B in B_join.DefaultIfEmpty()

                        join J3 in dbContext.cliente on new {v_IdCliente = A.v_IdCliente}
                        equals new {v_IdCliente = J3.v_IdCliente} into J3_join
                        from J3 in J3_join.DefaultIfEmpty()

                        join C in dbContext.systemparameter on J3.i_IdDepartamento equals C.i_ParameterId into C_join
                        from C in C_join

                        join D in dbContext.systemparameter on J3.i_IdProvincia equals D.i_ParameterId into D_join
                        from D in D_join

                        join E in dbContext.systemparameter on J3.i_IdDistrito equals E.i_ParameterId into E_join
                        from E in E_join

                        join J4 in dbContext.systemparameter on
                        new {TipoIdentificacion = J3.i_IdTipoIdentificacion.Value}
                        equals new {TipoIdentificacion = J4.i_ParameterId} into J4_join
                        from J4 in J4_join.DefaultIfEmpty()

                        where A.i_Eliminado == 0 && J4.i_GroupId == 150

                        select new ReporteClienteVendedorDto
                        {
                            v_IdVendedor = A.v_IdVendedor,
                            NombreVendedor = B.v_NombreCompleto,
                            NombreRazonSocial =
                            (J3.v_PrimerNombre.Trim() + " " + J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() +
                             " " + J3.v_RazonSocial.Trim()).Trim(),
                            FechaCreacion = A.t_InsertaFecha.Value,
                            NroDocumento = J3.v_NroDocIdentificacion,
                            Departamento = C.v_Value1,
                            Provincia = D.v_Value1,
                            Distrito = E.v_Value1,
                        };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<ReporteClienteVendedorDto> objData = query.ToList();
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

    }
}
