using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using SAMBHS.Common.BE.Custom;
namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class MedicamentoBL
    {
        public List<MedicamentoDto> GetListMedicamentos(ref OperationResult pobjOperationResult, string nombre,
            string accionFarmaco)
        {
            try
            {
                nombre = string.IsNullOrWhiteSpace(nombre) ? "null" : "'%" + nombre.Trim().ToLower() + "%'";
                accionFarmaco = string.IsNullOrWhiteSpace(accionFarmaco) ? "null" : "'%" + accionFarmaco.Trim().ToLower() + "%'";
                var periodo = DateTime.Today.Year.ToString();

                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != ConnectionState.Open) cnx.Open();

                    var query =
                        "select \"v_IdProductoDetalle\" as \"IdProductoDetalle\" , \"v_CodInterno\" as \"CodInterno\", \"v_Descripcion\" as \"Nombre\",  " +
                        "\"v_Presentacion\" as \"Presentacion\", \"v_Concentracion\" as \"Concentracion\",  " +
                        "\"v_Ubicacion\" as \"Ubicacion\", " +
                        "p.\"v_IdLinea\" as \"IdLinea\", " +
                        "ISNULL(p.v_AccionFarmaco,'') as AccionFarmaco, ISNULL(p.v_PrincipioActivo,'') as PrincipioActivo, " +
                        "p.\"v_Laboratorio\" as \"Laboratorio\", p.\"d_PrecioVenta\" as \"PrecioVenta\" " +
                        ",pa.\"d_StockActual\" as \"Stock\" " + ", p.\"d_PrecioMayorista\" as \"d_PrecioMayorista\" , p.\"d_PrecioCosto\" as \"d_PrecioCosto\" , P.v_Descripcion2 as  'v_Descripcion2' , ISNULL(p.i_Insumo,0) as  'i_Insumo' " +
                        "from producto p " +
                        "join productodetalle pd on p.\"v_IdProducto\" = pd.\"v_IdProducto\" " +
                        "join productoalmacen pa on pd.\"v_IdProductoDetalle\" = pa.\"v_ProductoDetalleId\" " +
                        "where (" + nombre + " is null or lower(p.\"v_Descripcion\") like " + nombre + ") and p.i_EsActivo =1 and pa.d_StockActual > 0 and " +
                        "(" + accionFarmaco + " is null or lower(p.\"v_Descripcion2\") like " + accionFarmaco + ") " +
                        "and pa.i_IdAlmacen = 1 and p.\"i_Eliminado\" = 0 and pa.v_Periodo=  " + DateTime.Now.ToString().Split('/', ' ')[2] + " ;";

                    var listado = cnx.Query<MedicamentoDto>(query).ToList();

                    pobjOperationResult.Success = 1;
                    return listado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "MedicamentoBl.GetListMedicamentos()";
                return null;
            }
        }


        public void AddUpdateMedicamento(ref OperationResult pobjOperationResult, MedicamentoDto pobjDto, List<string> ClientSession)
        {
            int nodeId = int.Parse(ClientSession[0]);
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    var query = "select * from productodetalle where v_IdProductoDetalle = '" +
                                  pobjDto.IdProductoDetalle + "'";

                    var entidad = cnx.Query<ProductoCustom>(query).FirstOrDefault();

                    var nombre = pobjDto.Nombre.Trim();
                    var presentacion = pobjDto.Presentacion.Trim();
                    var concentracion = pobjDto.Concentracion.Trim();

                    if (entidad == null)
                    {
                        var queryProduct = "select * from producto where v_Descripcion = '"+ nombre +"' and v_Presentacion = '"+ presentacion +"' and v_Concentracion = '"+ concentracion +"'";

                        var yaExiste = cnx.Query<ProductoCustom>(queryProduct).ToList();

                        if (yaExiste.Count > 0)
                        {
                            throw new Exception("El medicamento ya fue registrado anteriormente!");
                        }

                        var productoToInsert = new ProductoCustom
                        {
                            d_PrecioVenta = pobjDto.PrecioVenta,
                            v_Descripcion = nombre,
                            v_Presentacion = presentacion,
                            v_Concentracion = concentracion,
                            v_AccionFarmaco = pobjDto.AccionFarmaco,
                            v_Ubicacion = pobjDto.Ubicacion,
                            i_IdProveedor = -1,
                            i_IdTipoProducto = 1,
                            i_IdUsuario = -1,
                            i_EsAfectoDetraccion = 0,
                            i_IdUnidadMedida = pobjDto.IdUnidadMedida,
                            d_Empaque = 1,
                            v_CodInterno = pobjDto.CodInterno,
                            i_EsActivo = 1,
                            i_EsActivoFijo = 0,
                            i_EsLote = 0,
                            i_EsServicio = 0,
                            i_SolicitarNroLoteIngreso = 1,
                            i_SolicitarNroLoteSalida = 1,
                            i_NombreEditable = 0,
                            i_ValidarStock = 1,
                            v_IdLinea = pobjDto.IdLinea
                        };
                        var productoBl = new ProductoCustomBL();
                        productoBl.InsertarProducto(productoToInsert, nodeId);
                    }
                    else
                    {
                        var prodOriginal = new ProductoCustom
                        {                            
                            v_IdProducto = entidad.v_IdProducto,
                            v_Descripcion = nombre,
                            v_Presentacion = presentacion,
                            v_Concentracion = concentracion,
                            v_AccionFarmaco = pobjDto.AccionFarmaco,
                            v_Ubicacion = pobjDto.Ubicacion,
                            i_IdUnidadMedida = pobjDto.IdUnidadMedida,
                            d_Empaque = 1,
                            v_CodInterno = pobjDto.CodInterno,
                            i_EsActivo = 1,
                            i_EsActivoFijo = 0,
                            i_EsLote = 0,
                            i_EsServicio = 0,
                            i_SolicitarNroLoteIngreso = 1,
                            i_SolicitarNroLoteSalida = 1,
                        }; 
                        var productoBl = new ProductoCustomBL();
                        productoBl.ActualizarProducto(prodOriginal);

                        
                    }
   
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "MedicamentoBl.AddUpdateMedicamento()";
            }
        }

    }
}
