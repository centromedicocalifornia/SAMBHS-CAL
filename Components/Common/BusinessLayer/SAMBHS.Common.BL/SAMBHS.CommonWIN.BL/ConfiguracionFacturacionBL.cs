using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;

namespace SAMBHS.CommonWIN.BL
{
    public class ConfiguracionFacturacionBL
    {
        /// <summary>
        /// Obtiene la Configuracion para Facturacion Electronica de la Empresa, si no existe la crea.
        /// </summary>
        /// <param name="pobjResult">resultado de la Operacion</param>
        /// <returns>Configuracion de la Empresa</returns>
        public configuracionfacturacionDto GetConfiguracion(out OperationResult pobjResult)
        {
            pobjResult = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var config = (from n in dbContext.configuracionfacturacion
                                  select n).FirstOrDefault();
                    if (config == null)
                    {
                        config = new configuracionfacturacion();
                        config.i_EsEmisor = 0;
                        dbContext.configuracionfacturacion.AddObject(config);
                        dbContext.SaveChanges();
                    }
                    pobjResult.Success = 1;
                    return config.ToDTO();
                }
            }
            catch (Exception ex)
            {
                pobjResult.Success = 0;
                pobjResult.ErrorMessage = ex.Message;
                pobjResult.AdditionalInformation = "ConfiguracionFacturacionBL.getConfiguracion()";
                return null;
            }
        }
       
        /// <summary>
        /// Guarda la Configuracion de Facturacion
        /// </summary>
        /// <param name="pobjResult">Resultado de la Operacion</param>
        /// <param name="objDto">Entidad de configuracion</param>
        public void SaveConfiguracion(out OperationResult pobjResult, configuracionfacturacionDto objDto)
        {
            pobjResult = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var config = (from n in dbContext.configuracionfacturacion
                                  select n).FirstOrDefault();
                    dbContext.configuracionfacturacion.ApplyCurrentValues(objDto.ToEntity());
                    dbContext.SaveChanges();
                    pobjResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjResult.Success = 0;
                pobjResult.ErrorMessage = ex.Message;
                pobjResult.AdditionalInformation = "ConfiguracionFacturacionBL.saveConfiguracion()";
            }
        }
    }

}
