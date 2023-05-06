using System;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Requerimientos.NBS
{
    /// <summary>
    /// Clase que sirve para sincronizar los clientes del sistema notarial con el sistema contable.
    /// </summary>
    public class ClientSincroManager
    {
        public delegate void OnError(string error);
        /// <summary>
        /// Evento disparado cuando sucede una excepción.
        /// </summary>
        public event OnError ErrorEvent;

        /// <summary>
        /// Método para consultar si el cliente ya existe, si existe lo retorna en dto, sino lo registra y luego lo retorna en dto.
        /// </summary>
        /// <param name="objKardexCliente"></param>
        /// <returns></returns>
        public clienteDto ObtenerRegistrarCliente(DbfConnector.KardexClienteDto objKardexCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (objKardexCliente == null) return null;

                    if (string.IsNullOrWhiteSpace(objKardexCliente.NroDocumento) ||
                        string.IsNullOrWhiteSpace(objKardexCliente.Codigo) ||
                        string.IsNullOrWhiteSpace(objKardexCliente.NombreApellidosRazonSocial) ||
                        (objKardexCliente.TipoPersona.Equals("J") && string.IsNullOrWhiteSpace(objKardexCliente.Direccion)))
                    {
                        throw new Exception("Este cliente no cuenta con los datos básicos requeridos.");
                    }

                    var clienteEntity = dbContext.cliente.FirstOrDefault(p =>
                        p.v_NroDocIdentificacion.Trim().Equals(objKardexCliente.NroDocumento.Trim()) &&
                        p.i_Eliminado == 0 && p.v_FlagPantalla.Equals("C"));

                    if (clienteEntity != null) return clienteEntity.ToDTO();

                    var clienteToInsert = new clienteDto();
                    if (objKardexCliente.TipoPersona.Equals("N"))
                    {
                        var arrayN = objKardexCliente.NombreApellidosRazonSocial.Split(' ', ',');
                        clienteToInsert.v_ApePaterno = arrayN[0];
                        clienteToInsert.v_ApeMaterno = arrayN[1];
                        clienteToInsert.v_PrimerNombre = arrayN[3];
                        clienteToInsert.v_SegundoNombre = string.Empty;
                        clienteToInsert.v_RazonSocial = string.Empty;
                    }
                    else
                    {
                        clienteToInsert.v_ApePaterno = string.Empty;
                        clienteToInsert.v_ApeMaterno = string.Empty;
                        clienteToInsert.v_PrimerNombre = string.Empty;
                        clienteToInsert.v_SegundoNombre = string.Empty;
                        clienteToInsert.v_RazonSocial = objKardexCliente.NombreApellidosRazonSocial;
                    }
                    clienteToInsert.v_NroDocIdentificacion = objKardexCliente.NroDocumento;
                    clienteToInsert.v_CodCliente = objKardexCliente.Codigo;
                    clienteToInsert.i_IdTipoIdentificacion = objKardexCliente.NroDocumento.Length == 11 ? 6 : 1;
                    clienteToInsert.i_IdTipoPersona = objKardexCliente.TipoPersona.Equals("N") ? 1 : 2;
                    clienteToInsert.v_FlagPantalla = "C";
                    clienteToInsert.i_IdSexo = -1;
                    clienteToInsert.v_DirecPrincipal = objKardexCliente.Direccion;
                    clienteToInsert.i_IdPais = 1;
                    clienteToInsert.i_IdDepartamento = 1391;
                    clienteToInsert.i_IdProvincia = 1392;
                    clienteToInsert.i_IdDistrito = 1393;
                    clienteToInsert.i_Nacionalidad = 0;
                    clienteToInsert.i_IdListaPrecios = 1;
                    clienteToInsert.i_IdZona = 1;
                    var objOperationResult = new OperationResult();
                    var idClienteNuevo = new ClienteBL().InsertarCliente(ref objOperationResult, clienteToInsert,
                        Globals.ClientSession.GetAsList(), null, null, null, null, null, null);

                    if (objOperationResult.Success == 0)
                    {
                        throw new Exception(objOperationResult.ErrorMessage);
                    }
                    return dbContext.cliente.Single(p => p.v_IdCliente.Equals(idClienteNuevo)).ToDTO();
                }
            }
            catch (Exception ex)
            {
                if (ErrorEvent != null)
                    ErrorEvent(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Inicializador del evento Error.
        /// </summary>
        /// <param name="error"></param>
        protected virtual void OnErrorEvent(string error)
        {
            var handler = ErrorEvent;
            if (handler != null) handler(error);
        }
    }
}