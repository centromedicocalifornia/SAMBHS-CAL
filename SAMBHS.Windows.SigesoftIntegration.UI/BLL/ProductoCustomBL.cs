using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class ProductoCustomBL
    {
        public void InsertarProducto(ProductoCustom productoToInsert, int nodeId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    var secuential = Utilidades.GetNextSecuentialId_SAMBHS(nodeId, 24);
                    var newId = Utilidades.GetNewId(nodeId, secuential, "PD");
                    var add = "INSERT INTO producto (d_PrecioVenta, v_IdProducto, v_Descripcion, v_Presentacion, v_Concentracion, v_AccionFarmaco, v_Ubicacion, i_IdProveedor, i_IdTipoProducto, i_IdUsuario, i_EsAfectoDetraccion, i_IdUnidadMedida, d_Empaque, v_CodInterno, " +
                              "i_EsActivo, i_EsActivoFijo, i_EsLote, i_EsServicio, i_SolicitarNroLoteIngreso, i_SolicitarNroLoteSalida, i_NombreEditable, i_ValidarStock, v_IdLinea) " +
                              "VALUES (" + productoToInsert.d_PrecioVenta.Value + ", '" + newId + "', '" + productoToInsert.v_Descripcion + "', '" + productoToInsert.v_Presentacion + "', '" + productoToInsert.v_Concentracion + "', '" + productoToInsert.v_AccionFarmaco + "', '" + productoToInsert.v_Ubicacion + "', " +
                              ""+ productoToInsert.i_IdProveedor.Value +", "+ productoToInsert.i_IdTipoProducto.Value +", "+ productoToInsert.i_IdUsuario.Value +", "+ productoToInsert.i_EsAfectoDetraccion.Value +", " +
                              ""+ productoToInsert.i_IdUnidadMedida.Value +", "+ productoToInsert.d_Empaque.Value +", '"+ productoToInsert.v_CodInterno +"', "+ productoToInsert.i_EsActivo.Value +", " +
                              ""+ productoToInsert.i_EsActivoFijo.Value +", "+ productoToInsert.i_EsLote.Value +", "+ productoToInsert.i_EsServicio.Value +", "+ productoToInsert.i_SolicitarNroLoteIngreso.Value +", " +
                              ""+ productoToInsert.i_SolicitarNroLoteSalida.Value +", "+ productoToInsert.i_NombreEditable.Value +", "+ productoToInsert.i_ValidarStock.Value +", '"+ productoToInsert.v_IdLinea +"')";

                    cnx.Execute(add);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public void ActualizarProducto(ProductoCustom productoUpdate)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    var update = "UPDATE producto " +
                                 "SET v_Descripcion = '"+ productoUpdate.v_Descripcion +"', v_Presentacion = '"+ productoUpdate.v_Presentacion +"', v_Concentracion = '"+ productoUpdate.v_Concentracion +"', v_AccionFarmaco = '"+ productoUpdate.v_AccionFarmaco +"', " +
                                 "v_Ubicacion = '"+ productoUpdate.v_Ubicacion +"', i_IdUnidadMedida = "+ productoUpdate.i_IdUnidadMedida.Value +", d_Empaque = "+ productoUpdate.d_Empaque.Value +", v_CodInterno = '"+ productoUpdate.v_CodInterno +"', i_EsActivo = "+ productoUpdate.i_EsActivo.Value +", " +
                                 "i_EsActivoFijo = "+ productoUpdate.i_EsActivoFijo.Value +", i_EsLote = "+ productoUpdate.i_EsLote.Value +", i_EsServicio = "+ productoUpdate.i_EsServicio.Value +", i_SolicitarNroLoteIngreso = "+ productoUpdate.i_SolicitarNroLoteIngreso.Value +", i_SolicitarNroLoteSalida = "+ productoUpdate.i_SolicitarNroLoteSalida.Value +" " +
                                 "WHERE v_IdProducto = '"+ productoUpdate.v_IdProducto +"'";

                    cnx.Execute(update);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
