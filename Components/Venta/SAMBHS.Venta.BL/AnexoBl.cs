using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Venta.BL
{
    public class AnexoBl
    {
        public string ObtenerCodigoAnexo(ref OperationResult pobjOperationResult, string flagAnexo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var anexosPorFlag = dbContext.cliente.Where(p => p.v_FlagPantalla.Equals(flagAnexo) && p.i_Eliminado == 0)
                                                         .OrderByDescending(o => o.v_CodCliente).ToList();
                    pobjOperationResult.Success = 1;

                    if (anexosPorFlag.Any())
                    {
                        var ultimo = anexosPorFlag.FirstOrDefault();
                        if (ultimo != null && ultimo.v_CodCliente.Trim().Length == 6)
                        {
                            int i;
                            var numeral = int.TryParse(ultimo.v_CodCliente.Substring(1, 5), out i) ? i : 0;
                            numeral++;
                            return string.Format("{0}{1}", flagAnexo, numeral.ToString("00000"));
                        }
                    }
                }

                return string.Format("{0}{1}", flagAnexo, 1.ToString("00000"));
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AnexoBl.ObtenerCodigoAnexo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public clienteDto ObtenerAnexoPorId(ref OperationResult pobjOperationResult, string idAnexo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var anexo = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(idAnexo));
                    pobjOperationResult.Success = 1;
                    return anexo.ToDTO();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AnexoBl.ObtenerAnexoPorId()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<AnexoConsulta> ObtenerListadoAnexos(ref OperationResult pobjOperationResult, string flagAnexo, string nombre)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    nombre = nombre.Trim();
                    var anexos = (from n in dbContext.cliente
                        where n.v_FlagPantalla.Equals(flagAnexo) && n.i_Eliminado == 0
                        && (nombre == "" || n.v_PrimerNombre.ToUpper().Contains(nombre.ToUpper()))
                        select new AnexoConsulta
                        {
                            Id = n.v_IdCliente,
                            Direccion = n.v_DirecPrincipal,
                            NroDocumento = n.v_NroDocIdentificacion,
                            Nombre = n.v_PrimerNombre
                        }).ToList();

                    pobjOperationResult.Success = 1;

                    return anexos;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AnexoBl.ObtenerAnexoPorId()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool AnexoUsado(ref OperationResult pobjOperationResult, string id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var usadoDiario = dbContext.diariodetalle.Any(p => p.v_IdCliente.Equals(id) && p.i_Eliminado == 0);
                    var usadoTesoreria = dbContext.tesoreriadetalle.Any(p => p.v_IdCliente.Equals(id) && p.i_Eliminado == 0);

                    return usadoDiario || usadoTesoreria;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AnexoBl.AnexoUsado()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public void EliminarAnexo(ref OperationResult pobjOperationResult, string id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(id));
                    if (entity != null)
                    {
                        entity.i_Eliminado = 1;
                        entity.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        entity.t_ActualizaFecha = DateTime.Now;
                        dbContext.cliente.ApplyCurrentValues(entity);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        return;
                    }
                    throw new Exception("Anexo no encontrado");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AnexoBl.EliminarAnexo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public class AnexoConsulta
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public string NroDocumento { get; set; }
            public string Direccion { get; set; }
        }
    }
}
