using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Venta.BL
{
    public class VendedorBL
    {


        ClienteBL _objClienteBL = new ClienteBL();
        clienteDto objClienteDto = new clienteDto();
        lineacreditoempresaDto _lineacreditoempresaDto = new lineacreditoempresaDto();
        public List<vendedorDto> ObtenerListadoVendedor(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from A in dbContext.vendedor
                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value ,eliminado =0 }
                                equals new { i_InsertUserId = J1.i_SystemUserId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value ,eliminado =0 }
                                equals new { i_UpdateUserId = J2.i_SystemUserId ,eliminado= J2.i_IsDeleted.Value  } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()

                                join J3 in dbContext.systemparameter on
                                new { TipoIdentificacion = A.i_IdTipoIdentificacion.Value ,eliminado =0 ,Grupo=150 }
                                equals new { TipoIdentificacion = J3.i_ParameterId ,eliminado = J3.i_IsDeleted.Value,Grupo= J3.i_GroupId } into J3_join
                                from J3 in J3_join.DefaultIfEmpty()

                                where A.i_Eliminado == 0 

                                select new vendedorDto
                                {
                                    v_IdVendedor = A.v_IdVendedor,
                                    v_CodVendedor = A.v_CodVendedor,
                                    v_NombreCompleto = A.v_NombreCompleto,
                                    v_Contacto = A.v_Contacto,
                                    v_Direccion = A.v_Direccion,
                                    i_IdDepartamento = A.i_IdDepartamento,
                                    i_IdPais = A.i_IdPais,
                                    i_IdProvincia = A.i_IdProvincia,
                                    i_IdDistrito = A.i_IdDistrito,
                                    i_IdTipoIdentificacion = A.i_IdTipoIdentificacion,
                                    v_NroDocIdentificacion = A.v_NroDocIdentificacion,
                                    v_Telefono = A.v_Telefono,
                                    v_Fax = A.v_Fax,
                                    v_Correo = A.v_Correo,
                                    i_IdTipoPersona = A.i_IdTipoPersona,
                                    i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                    i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                    t_ActualizaFecha = A.t_ActualizaFecha,
                                    t_InsertaFecha = A.t_InsertaFecha,
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

                    List<vendedorDto> objData = query.ToList();
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

        public vendedorDto ObtenerVendedor(ref OperationResult pobjOperationResult, string pstringIdvendedor)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    vendedorDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.vendedor
                                     where a.v_IdVendedor == pstringIdvendedor
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

        public vendedorDto ObtenerVendeorPorCodigo(ref OperationResult pobjOperationResult, string pstrCodVendedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from n in dbContext.vendedor
                                     where n.v_CodVendedor == pstrCodVendedor
                                     select n).FirstOrDefault();
                    pobjOperationResult.Success = 1;
                    if (objEntity != null)
                        return objEntity.ToDTO();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
            return null;
        }
        public void InsertarVendedor(ref OperationResult pobjOperationResult, vendedorDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;

            string newId = string.Empty;
            try
            {

                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBl = new SecuentialBL();
                        var objEntity = pobjDtoEntity.ToEntity();


                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;

                        // Autogeneramos el Pk de la tabla
                        int intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 15);
                        newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "VE");
                        objEntity.v_IdVendedor = newId;


                        dbContext.AddTovendedor(objEntity);
                        dbContext.SaveChanges();


                        objClienteDto.v_CodCliente = pobjDtoEntity.v_CodVendedor;
                        objClienteDto.v_RazonSocial = pobjDtoEntity.v_NombreCompleto;
                        objClienteDto.v_ApeMaterno = string.Empty;
                        objClienteDto.v_ApePaterno = string.Empty;
                        objClienteDto.v_PrimerNombre = string.Empty;
                        objClienteDto.v_SegundoNombre = string.Empty;
                        objClienteDto.i_IdTipoIdentificacion = pobjDtoEntity.i_IdTipoIdentificacion;
                        objClienteDto.v_NroDocIdentificacion = pobjDtoEntity.v_NroDocIdentificacion;
                        objClienteDto.v_IdVendedor = newId;
                        objClienteDto.i_IdPais = pobjDtoEntity.i_IdPais.Value;
                        objClienteDto.i_IdTipoPersona = pobjDtoEntity.i_IdTipoPersona;
                        objClienteDto.i_IdProvincia = pobjDtoEntity.i_IdProvincia;
                        objClienteDto.i_IdDistrito = pobjDtoEntity.i_IdDistrito;
                        objClienteDto.v_DirecPrincipal = pobjDtoEntity.v_Direccion;
                        objClienteDto.v_FlagPantalla = "E";
                        objClienteDto.v_TelefonoFijo = pobjDtoEntity.v_Telefono;
                        _objClienteBL.InsertarCliente(ref pobjOperationResult, objClienteDto, ClientSession, _lineacreditoempresaDto, null, null, null, null, null, null);
                        if (pobjOperationResult.Success == 0)
                        {
                            return;
                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {



                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VendedorBL.InsertarVendedor()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return;
            }
        }

        public void ActualizarVendedor(ref OperationResult pobjOperationResult, vendedorDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {

                using (var ts = TransactionUtils.CreateTransactionScope())
                {

                    clienteDto objClienteDto = new clienteDto();
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.vendedor
                                               where a.v_IdVendedor == pobjDtoEntity.v_IdVendedor
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                        pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                        vendedor objEntity = pobjDtoEntity.ToEntity();
                        dbContext.vendedor.ApplyCurrentValues(objEntity);

                        // Guardar los cambios


                        objClienteDto = dbContext.cliente.Where(o => o.v_IdVendedor == pobjDtoEntity.v_IdVendedor).FirstOrDefault().ToDTO();
                        if (objClienteDto != null)
                        {
                            objClienteDto.v_CodCliente = pobjDtoEntity.v_CodVendedor;
                            objClienteDto.v_RazonSocial = pobjDtoEntity.v_NombreCompleto;
                            objClienteDto.i_IdTipoIdentificacion = pobjDtoEntity.i_IdTipoIdentificacion;
                            objClienteDto.v_NroDocIdentificacion = pobjDtoEntity.v_NroDocIdentificacion;
                            objClienteDto.v_IdVendedor = pobjDtoEntity.v_IdVendedor;
                            objClienteDto.i_IdPais = pobjDtoEntity.i_IdPais.Value;
                            objClienteDto.i_IdTipoPersona = pobjDtoEntity.i_IdTipoPersona;
                            objClienteDto.i_IdProvincia = pobjDtoEntity.i_IdProvincia;
                            objClienteDto.i_IdDistrito = pobjDtoEntity.i_IdDistrito;
                            objClienteDto.v_DirecPrincipal = pobjDtoEntity.v_Direccion;
                            objClienteDto.v_FlagPantalla = "E";
                            objClienteDto.v_TelefonoFijo = pobjDtoEntity.v_Telefono;
                            _objClienteBL.Actualizarcliente(ref pobjOperationResult, objClienteDto, ClientSession, _lineacreditoempresaDto, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                        }
                        else
                        {
                            objClienteDto = new clienteDto();
                            objClienteDto.v_CodCliente = pobjDtoEntity.v_CodVendedor;
                            objClienteDto.v_RazonSocial = pobjDtoEntity.v_NombreCompleto;
                            objClienteDto.i_IdTipoIdentificacion = pobjDtoEntity.i_IdTipoIdentificacion;
                            objClienteDto.v_NroDocIdentificacion = pobjDtoEntity.v_NroDocIdentificacion;
                            objClienteDto.v_IdVendedor = pobjDtoEntity.v_IdVendedor;
                            objClienteDto.i_IdPais = pobjDtoEntity.i_IdPais.Value;
                            objClienteDto.i_IdTipoPersona = pobjDtoEntity.i_IdTipoPersona;
                            objClienteDto.i_IdProvincia = pobjDtoEntity.i_IdProvincia;
                            objClienteDto.i_IdDistrito = pobjDtoEntity.i_IdDistrito;
                            objClienteDto.v_DirecPrincipal = pobjDtoEntity.v_Direccion;
                            objClienteDto.v_FlagPantalla = "E";
                            objClienteDto.v_TelefonoFijo = pobjDtoEntity.v_Telefono;
                            _objClienteBL.InsertarCliente(ref pobjOperationResult, objClienteDto, ClientSession, _lineacreditoempresaDto, null, null, null, null, null, null);
                        }
                        if (pobjOperationResult.Success == 0)
                        {
                            return;
                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VendedorBL.ActualizarVendedor()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return;
            }
        }

        public void EliminarVendedor(ref OperationResult pobjOperationResult, string pstrIdvendedor, List<string> ClientSession)
        {
            try
            {

                 using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Eliminar Vendedor
                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.vendedor
                                               where a.v_IdVendedor == pstrIdvendedor
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;



                        //Eliminar cartera de clientes
                        var objEntitySourceCarteraClientes = (from a in dbContext.carteracliente
                                                              where a.v_IdVendedor == pstrIdvendedor
                                                              select a).ToList();

                        foreach (var RegistroCarteraCliente in objEntitySourceCarteraClientes)
                        {
                            RegistroCarteraCliente.t_ActualizaFecha = DateTime.Now;
                            RegistroCarteraCliente.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistroCarteraCliente.i_Eliminado = 1;
                        }

                        #endregion
                        // Guardar los cambios


                        #region Eliminar Cliente

                        var objEntityVendedor = (from a in dbContext.cliente
                                                 where a.v_IdVendedor == pstrIdvendedor
                                                 select a).FirstOrDefault();

                        if (objEntityVendedor != null)
                        {
                            // Crear la entidad con los datos actualizados
                            objEntityVendedor.t_ActualizaFecha = DateTime.Now;
                            objEntityVendedor.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityVendedor.i_Eliminado = 1;
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
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public List<KeyValueDTO> ObtenerListadoVendedorParaCombo(ref OperationResult pobjOperationResult, string pstrSortExression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from a in dbContext.vendedor
                                where a.i_Eliminado == 0 && a.i_EsActivo ==1
                                select new
                                {
                                    a.v_IdVendedor,
                                    a.v_NombreCompleto,
                                    a.i_EsActivo
                                };

                    query = query.OrderBy("v_NombreCompleto");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.v_IdVendedor,
                            Value1 = x.v_NombreCompleto
                        }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<KeyValueDTO> ObtenerUsuariosParaCombo(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from a in dbContext.systemuser
                                where a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_UserName");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_SystemUserId.ToString(),
                            Value1 = x.v_UserName
                        }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<KeyValueDTO> DevuelveVendedores()
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<KeyValueDTO> EntityCliente = (from n in dbContext.vendedor
                                                       select new KeyValueDTO { Id = n.v_IdVendedor, Value1 = n.v_CodVendedor }).ToList();

                    return EntityCliente;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Reporte
        public List<ReporteVendedor> ReporteVendedor(string pstrt_Orden, int pintIdTipoPersona)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    #region Query

                    var query =
                    (from A in dbContext.vendedor
                     join B in dbContext.datahierarchy on A.i_IdTipoPersona equals B.i_ItemId


                     where
                     A.i_Eliminado == 0 && (B.i_IsDeleted == 0 && B.i_GroupId == 2) &&
                     (A.i_IdTipoPersona == pintIdTipoPersona || pintIdTipoPersona == -1)
                     select new ReporteVendedor
                     {
                         v_CodVendedor = A.v_CodVendedor,
                         Vendedor = A.v_NombreCompleto,
                         v_NroDocIdentificacion = A.v_NroDocIdentificacion,
                         v_Direccion = A.v_Direccion,
                         v_TipoPersona = B.v_Value1,
                         v_Telefono = A.v_Telefono,
                         t_InsertaFecha = A.t_InsertaFecha.Value,
                     });

                    #endregion

                    query = query.OrderBy(pstrt_Orden);


                    return query.ToList();


                    //List<ventaDto> objData = query.ToList();

                    //pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        #endregion
    }
}
