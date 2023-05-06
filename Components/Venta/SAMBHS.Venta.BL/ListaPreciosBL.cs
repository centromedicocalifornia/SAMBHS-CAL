﻿using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using System.Transactions;
using System.ComponentModel;
using SAMBHS.Common.BL;
//using SAMBHS.Common.BE;

namespace SAMBHS.Venta.BL
{
    public class ListaPreciosBL
    {

        public bool ObtenerIdListaPrecio(ref OperationResult pobjOperationResult, int idAlmacen, int IdLista)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.listaprecio
                        where n.i_IdAlmacen == idAlmacen && n.i_IdLista == IdLista
                        select n).FirstOrDefault();

                    return query != null;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public listaprecioDto ObtenerCabeceraListaPrecios(ref OperationResult objOperationResult, string IdListaPrecios)
        {

            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    listaprecio Cabecera = (from a in dbContext.listaprecio

                                            where a.i_Eliminado == 0 && a.v_IdListaPrecios == IdListaPrecios

                                            select a).FirstOrDefault();

                    listaprecioDto objCabecera = listaprecioAssembler.ToDTO(Cabecera);
                    objOperationResult.Success = 1;
                    return objCabecera;


                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;

            }
        }


       

        
        public void InsertarListaPrecios(ref OperationResult pobjOperationResult, listaprecioDto pobjDtoEntity, List<string> ClientSession, List<productoalmacenDto> pTemp_Insertar, string tipoCambio, bool CopiarLista = false)
        {
            OperationResult ObjOperationResult = new OperationResult();
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        listaprecio objEntityListaPrecio = listaprecioAssembler.ToEntity(pobjDtoEntity);
                        dbContext.listapreciodetalle.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                        listapreciodetalleDto pobjDtoListaPrecioDetalle = new listapreciodetalleDto();

                        int SecuentialId = 0;
                        string newIdListaPrecio = string.Empty;
                        string newIdListaPrecioDetalle = string.Empty;

                        int intNodeId;

                        #region Inserta Cabecera

                        objEntityListaPrecio.t_InsertaFecha = DateTime.Now;
                        objEntityListaPrecio.i_InsertaIdsuario = Int32.Parse(ClientSession[2]);
                        objEntityListaPrecio.i_Eliminado = 0;

                        // Autogeneramos el Pk de la tabla
                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 48);
                        newIdListaPrecio = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZE");

                        objEntityListaPrecio.v_IdListaPrecios = newIdListaPrecio;
                        dbContext.AddTolistaprecio(objEntityListaPrecio);


                        #endregion

                        #region Inserta Detalle

                        foreach (productoalmacenDto listaproductoAlmacen in pTemp_Insertar)
                        {
                            if (listaproductoAlmacen.v_IdProductoAlmacen != "N002-PA000000000")
                            {
                                listapreciodetalleDto listaPrecioDetalle = new listapreciodetalleDto();
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 49);
                                newIdListaPrecioDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZF");
                                listaPrecioDetalle.v_IdListaPrecios = newIdListaPrecio;
                                listaPrecioDetalle.v_IdProductoDetalle = listaproductoAlmacen.v_ProductoDetalleId;
                                listaPrecioDetalle.v_idListaPrecioDetalle = newIdListaPrecioDetalle;
                                listaPrecioDetalle.t_InsertaFecha = DateTime.Now;
                                listaPrecioDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                listaPrecioDetalle.i_Eliminado = 0;
                                if (!CopiarLista)
                                {

                                    listaPrecioDetalle.d_Descuento = 0;
                                    listaPrecioDetalle.d_Precio = listaproductoAlmacen.Costo; //Se cambio para CMR
                                    listaPrecioDetalle.d_Costo = listaproductoAlmacen.Costo;
                                    listaPrecioDetalle.d_PrecioMinDolares = listaproductoAlmacen.Costo/decimal.Parse(tipoCambio);
                                    listaPrecioDetalle.d_PrecioMinSoles = listaproductoAlmacen.Costo;
                                    listaPrecioDetalle.d_Utilidad = 0;
                                    listaPrecioDetalle.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen.Value;

                                    if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                                    {
                                        listaPrecioDetalle.d_Precio = listaproductoAlmacen.d_Precio;
                                        listaPrecioDetalle.d_Utilidad =listaproductoAlmacen.d_Utilidad ;
                                    
                                    }
                                   

                                }
                                else
                                {

                                    listaPrecioDetalle.d_Descuento = listaproductoAlmacen.d_Descuento;
                                    listaPrecioDetalle.d_Precio = listaproductoAlmacen.d_Precio;
                                    listaPrecioDetalle.d_PrecioMinDolares = listaproductoAlmacen.d_PrecioMinDolares;
                                    listaPrecioDetalle.d_PrecioMinSoles = listaproductoAlmacen.d_PrecioMinSoles;
                                    listaPrecioDetalle.d_Utilidad = listaproductoAlmacen.d_Utilidad;
                                    listaPrecioDetalle.v_IdProductoDetalle = listaproductoAlmacen.v_IdProductoDetalle;
                                    listaPrecioDetalle.d_Costo = listaproductoAlmacen.Costo;
                                    listaPrecioDetalle.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen.Value;

                                    if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                                    {
                                        listaPrecioDetalle.d_Precio = listaproductoAlmacen.d_Precio;
                                        listaPrecioDetalle.d_Utilidad = listaproductoAlmacen.d_Utilidad;

                                    }
                                }

                                listapreciodetalle objEntityListaPrecioDetalle =
                                    listapreciodetalleAssembler.ToEntity(listaPrecioDetalle);
                                dbContext.AddTolistapreciodetalle(objEntityListaPrecioDetalle);
                            }
                        }



                        #endregion

                        pobjOperationResult.Success = 1;
                        dbContext.SaveChanges();
                        ts.Complete();
                    }
                }


            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
            }
        }

        public void ActualizarCabeceraListaPrecios(ref OperationResult pobjOperationResult, listaprecioDto pobjDtoEntity, List<string> ClientSession)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    var objEntitySource = (from a in dbContext.listaprecio
                                           where a.v_IdListaPrecios == pobjDtoEntity.v_IdListaPrecios


                                           select a).FirstOrDefault();


                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    listaprecio objEntity = listaprecioAssembler.ToEntity(pobjDtoEntity);

                    dbContext.listaprecio.ApplyCurrentValues(objEntity);
                    pobjOperationResult.Success = 1;
                    dbContext.SaveChanges();


                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ListaPreciosBL.ActualizarCabeceraListaPrecios()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }




        }

        public void EliminarListaPrecios(ref OperationResult pobjOperationResult, string IdListaPrecios, List<string> ClientSession)
        {

            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Elimina Cabecera

                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.listaprecio
                            where a.v_IdListaPrecios == IdListaPrecios
                            select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;

                        #endregion

                        #region Elimina Detalles

                        //Eliminar detalles del movimiento eliminado.
                        var objEntitySourceDetallesMov = (from a in dbContext.listapreciodetalle
                            where a.v_IdListaPrecios == IdListaPrecios && a.i_Eliminado == 0
                            select a).ToList();

                        foreach (var RegistroMovimientosDetalle in objEntitySourceDetallesMov)
                        {
                            RegistroMovimientosDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistroMovimientosDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistroMovimientosDetalle.i_Eliminado = 1;
                        }


                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ListaPreciosBL.EliminarListaPrecios";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;

            }


        }

        public List<string> ObtenerMinusProductoAlmacen(int idAlmacen, string idListaPrecio)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                string periodo = Globals.ClientSession.i_Periodo.ToString();

                var query = (from n in dbContext.productoalmacen
                             join a in dbContext.productodetalle on new { pd = n.v_ProductoDetalleId, eliminado = 0 } equals new { pd = a.v_IdProductoDetalle, eliminado = a.i_Eliminado.Value } into a_join
                             from a in a_join.DefaultIfEmpty()
                             join b in dbContext.producto on new { p = a.v_IdProducto, eliminado = 0 } equals new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                             from b in b_join.DefaultIfEmpty()
                             where
                             n.i_Eliminado == 0 && n.i_IdAlmacen == idAlmacen &&
                             n.v_Periodo == periodo && b.i_Eliminado == 0 && b.i_EsActivoFijo == 0 && b.v_IdProducto != "N002-PD000000000" && b.i_EsActivo == 1

                             select new productoalmacenDto
                             {

                                 v_ProductoDetalleId = a.v_IdProductoDetalle,
                                 
                                
                             }).ToList();
                List<productoalmacenDto> ListaIdProductoAlmacen = query.ToList();

                var query2 = (from n in dbContext.listapreciodetalle

                              where
                              n.i_Eliminado == 0 && n.v_IdListaPrecios == idListaPrecio && n.i_IdAlmacen == idAlmacen &&
                              n.v_IdProductoDetalle != "N002-PE000000000"
                              select new productoalmacenDto
                              {
                                  v_ProductoDetalleId = n.v_IdProductoDetalle,//ambos se agrego para cuando solo usan una Tarifario
                                 

                              }).ToList();

                //List<string> ListaIdProductoAlmacenLpd = query2.ToList();

                //var FaltantesLpd = ListaIdProductoAlmacen.Except(ListaIdProductoAlmacenLpd).ToList();
                List<productoalmacenDto> ListaIdProductoAlmacenLpd = query2.ToList();

                var FaltantesLpd = ListaIdProductoAlmacen.Select (o=>o.v_ProductoDetalleId).Except(ListaIdProductoAlmacenLpd.Select (o=>o.v_ProductoDetalleId)).ToList();

                return FaltantesLpd;
            }

        }

        public List<productoalmacenDto> ProductosAlmacenInsertarListaPrecios(ref OperationResult pobjOperationResult, int strintIdAlmacen)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodo = Globals.ClientSession.i_Periodo.ToString();
                    var query = (from n in dbContext.productoalmacen

                        join a in dbContext.productodetalle on n.v_ProductoDetalleId equals a.v_IdProductoDetalle into
                        a_join
                        from a in a_join.DefaultIfEmpty()

                        join b in dbContext.producto on a.v_IdProducto equals b.v_IdProducto into b_join
                        from b in b_join.DefaultIfEmpty()

                        where n.i_Eliminado == 0 && n.i_IdAlmacen == strintIdAlmacen
                              && n.v_Periodo == periodo && b.i_EsActivo ==1

                        select new 
                        {
                            v_IdProductoAlmacen = n.v_IdProductoAlmacen,
                            Costo = b.d_PrecioCosto,
                            v_ProductoDetalleId =a.v_IdProductoDetalle ,
                            v_IdProductoDetalle =a.v_IdProductoDetalle ,
                            d_Utilidad =b.d_Utilidad ??0,
                            d_Precio = b.d_PrecioVenta ??0,
                          
                        }).ToList ().Select (o=> new productoalmacenDto
                        {
                        v_IdProductoAlmacen =o.v_IdProductoAlmacen ,
                        Costo =o.Costo ,
                        v_IdProductoDetalle =o.v_IdProductoDetalle ,
                        v_ProductoDetalleId =o.v_ProductoDetalleId ,
                        i_IdAlmacen = strintIdAlmacen,
                        
                        d_Utilidad =o.d_Utilidad ,
                        d_Precio =o.d_Precio ,
                        
                        }).ToList ();

                  

                    List<productoalmacenDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public int VerificarNumeroListaPrecios(ref OperationResult objOperationResult)
        {
            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var listas = (from a in dbContext.listaprecio
                                  where a.i_Eliminado == 0
                                  select a).ToList();
                    return listas.Count();

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return -1;
            }
        
        
        }

        public List<productoalmacenDto> InsertarListaPreciosCopiadoDesdeOtraListaPrecios(ref OperationResult pobjOperationResult, int IdAlmacen, int IdAlmacenAcopiar, int IdListaAcopiar)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Lp = (from a in dbContext.listaprecio

                        join b in dbContext.listapreciodetalle on new {lp = a.v_IdListaPrecios, eliminado = 0} equals
                        new {lp = b.v_IdListaPrecios, eliminado = b.i_Eliminado.Value} into b_join

                        from b in b_join.DefaultIfEmpty()

                        join c in dbContext.productoalmacen on new { pa = b.v_IdProductoDetalle , eliminado = 0 ,almacen =b.i_IdAlmacen.Value  } equals new { pa = c.v_ProductoDetalleId , eliminado = c.i_Eliminado.Value ,almacen =c.i_IdAlmacen } into c_join

                         from c in c_join.DefaultIfEmpty()

                        join d in dbContext.productodetalle on new {pd = c.v_ProductoDetalleId, eliminado = 0} equals
                        new {pd = d.v_IdProductoDetalle, eliminado = d.i_Eliminado.Value} into d_join

                        from d in d_join.DefaultIfEmpty()

                        where
                        a.i_IdAlmacen == IdAlmacenAcopiar && a.i_IdLista == IdListaAcopiar &&
                        c.i_IdAlmacen == IdAlmacenAcopiar

                        select new listapreciodetalleDto
                        {

                            //v_IdProductoAlmacen = c.v_IdProductoAlmacen,
                            d_Descuento = b.d_Descuento,
                            d_Utilidad = b.d_Utilidad,
                            d_Precio = b.d_Precio,
                            d_PrecioMinSoles = b.d_PrecioMinSoles,
                            d_PrecioMinDolares = b.d_PrecioMinDolares,
                            v_IdProductoDetalle =d.v_IdProductoDetalle 


                        }).ToList();

                    var periodo = Globals.ClientSession.i_Periodo.ToString();
                    var query = (from n in dbContext.productoalmacen

                        join a in dbContext.productodetalle on new {pd = n.v_ProductoDetalleId, eliminado = 0} equals
                        new {pd = a.v_IdProductoDetalle, eliminado = a.i_Eliminado.Value} into a_join
                        from a in a_join.DefaultIfEmpty()

                        join b in dbContext.producto on new {p = a.v_IdProducto, eliminado = 0} equals
                        new {p = b.v_IdProducto, eliminado = b.i_Eliminado.Value} into b_join
                        from b in b_join.DefaultIfEmpty()

                        where n.i_Eliminado == 0 && n.i_IdAlmacen == IdAlmacen
                              && n.v_Periodo == periodo
                        select new productoalmacenDto
                        {
                            v_IdProductoAlmacen = n.v_IdProductoAlmacen,
                            Costo = b.d_PrecioCosto,
                            v_ProductoDetalleId = a.v_IdProductoDetalle,
                        }).ToList().Select(x =>
                    {
                        var Pa = Lp.Where(a => a.v_IdProductoDetalle == x.v_ProductoDetalleId).FirstOrDefault();
                        return new productoalmacenDto
                        {

                            v_IdProductoAlmacen = x.v_IdProductoAlmacen,
                            Costo = x.Costo,
                            d_Descuento = Pa != null ? Pa.d_Descuento : 0, //    d == null ? 0 : d.d_Descuento ?? 0,
                            d_Precio = Pa != null ? Pa.d_Precio : 0,
                            d_PrecioMinSoles = Pa != null ? Pa.d_PrecioMinSoles : 0,
                            d_Utilidad = Pa != null ? Pa.d_Utilidad : 0,
                            d_PrecioMinDolares = Pa != null ? Pa.d_PrecioMinDolares : 0,
                            //v_ProductoDetalleId =Pa !=null ? Pa.v_IdProductoDetalle :null ,
                            v_IdProductoDetalle = Pa != null ? Pa.v_IdProductoDetalle : null,
                            

                        };

                    }).ToList().AsQueryable();

                    //foreach (var item in query)
                    //{


                    //    var Pa = Lp.Where(a => a.v_IdProductoAlmacen == item.v_IdProductoAlmacen).FirstOrDefault();


                    //}


                    //} ).ToList ().Select (x=> 
                    //{
                    //    var Pa= Lp.Where (a=>a.v_IdProductoAlmacen == x.v_IdProductoAlmacen).FirstOrDefault ();
                    //    return new productoalmacenDto
                    //    {

                    //        v_IdProductoAlmacen = x.v_IdProductoAlmacen,
                    //        Costo = x.Costo,
                    //        d_Descuento = Pa != null ? Pa.d_Descuento : 0, //    d == null ? 0 : d.d_Descuento ?? 0,
                    //         d_Precio = Pa !=null ? Pa.d_Precio :0,
                    //         d_PrecioMinSoles =Pa!=null ?Pa.d_PrecioMinSoles:0 ,
                    //         d_Utilidad =Pa!=null ?Pa.d_Utilidad:0,
                    //         d_PrecioMinDolares =// d == null ? 0 : d.d_PrecioMinDolares ?? 0,
                    //         d_PrecioMinSoles = //d == null ? 0 : d.d_PrecioMinSoles ?? 0,

                    //    };

                    //}).ToList().AsQueryable () ;

                    List<productoalmacenDto> objData = query.ToList().Where (o=>o.v_IdProductoDetalle !=null ).ToList ();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public string IdProductoAlmacen(ref OperationResult pobjOperationResult, int IdAlmacen, string CodigoProducto)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.producto

                        join a in dbContext.productodetalle on new {p = n.v_IdProducto, eliminado = 0} equals
                        new {p = a.v_IdProducto, eliminado = a.i_Eliminado.Value} into a_join
                        from a in a_join.DefaultIfEmpty()

                        join b in dbContext.productoalmacen on new {pd = a.v_IdProductoDetalle, eliminado = 0} equals
                        new {pd = b.v_ProductoDetalleId, eliminado = b.i_Eliminado.Value} into b_join
                        from b in b_join.DefaultIfEmpty()

                        where n.i_Eliminado == 0 && n.v_CodInterno == CodigoProducto && b.i_IdAlmacen == IdAlmacen

                        select new productoalmacenDto
                        {
                            v_IdProductoAlmacen = b.v_IdProductoAlmacen

                        }
                    ).FirstOrDefault();

                    return query != null ? query.v_IdProductoAlmacen : null;


                    //List<productoalmacenDto> objData = query.ToList();
                    //pobjOperationResult.Success = 1;
                    //return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        public string ObtenerIdListaPreciostr(ref OperationResult objOperationResult, int idAlmacen, int idLista)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var ObjEntity = (from a in dbContext.listaprecio

                                 where a.i_IdAlmacen == idAlmacen && a.i_IdLista == idLista && a.i_Eliminado == 0

                                 select new { a.v_IdListaPrecios }).FirstOrDefault();
                if (ObjEntity != null)
                {
                    objOperationResult.Success = 1;
                    return ObjEntity.v_IdListaPrecios;

                }
                else
                {
                    return null;
                }



            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }


        }
        //public BindingList<BindingListaPrecioDetalleDto> ObtenerListaPreciosDetalles(ref OperationResult pobjOperationResult, string pstrIdListaPrecio, int pIntAlmacen)
        //{
        //    try
        //    {
        //        SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

        //        var query = (from a in dbContext.listaprecio


        //                     join b in dbContext.listapreciodetalle on new { eliminado = 0 } equals new { eliminado = b.i_Eliminado.Value } into b_join

        //                     from b in b_join.DefaultIfEmpty()

        //                     join c in dbContext.productoalmacen on new { pa = b.v_IdProductoAlmacen, eliminado = 0 } equals new { pa = c.v_IdProductoAlmacen, eliminado = c.i_Eliminado.Value } into c_join

        //                     from c in c_join.DefaultIfEmpty()

        //                     join d in dbContext.productodetalle on new { pd = c.v_ProductoDetalleId, eliminado = 0 } equals new { pd = d.v_IdProductoDetalle, eliminado = d.i_Eliminado.Value } into d_join

        //                     from d in d_join.DefaultIfEmpty()

        //                     join e in dbContext.producto on new { p = d.v_IdProducto, eliminado = 0 } equals new { p = e.v_IdProducto, eliminado = e.i_Eliminado.Value } into e_join

        //                     from e in e_join.DefaultIfEmpty()

        //                     where a.i_Eliminado == 0 && a.i_IdAlmacen == pIntAlmacen && a.v_IdListaPrecios == pstrIdListaPrecio && e.i_EsActivoFijo == 0 && c.i_IdAlmacen == pIntAlmacen && b.v_IdListaPrecios == pstrIdListaPrecio
        //                     //  orderby a.v_IdProductoAlmacen ascending

        //                     orderby e.v_CodInterno
        //                     select new BindingListaPrecioDetalleDto
        //                     {

        //                         v_idListaPrecioDetalle = b.v_idListaPrecioDetalle,
        //                         v_IdProductoAlmacen = b.v_IdProductoAlmacen,
        //                         v_IdListaPrecios = b.v_IdListaPrecios,
        //                         d_Descuento = b.d_Descuento ?? 0,
        //                         d_Precio = b.d_Precio ?? 0,
        //                         d_PrecioMinSoles = b.d_PrecioMinSoles ?? 0,
        //                         d_PrecioMinDolares = b.d_PrecioMinDolares ?? 0,
        //                         i_Eliminado = b.i_Eliminado.Value,
        //                         i_InsertaIdUsuario = b.i_InsertaIdUsuario,
        //                         t_InsertaFecha = b.t_InsertaFecha,
        //                         i_ActualizaIdUsuario = b.i_ActualizaIdUsuario,
        //                         t_ActualizaFecha = b.t_ActualizaFecha,
        //                         d_Utilidad = b.d_Utilidad ?? 0,
        //                         v_CodInterno = e==null ?"": e.v_CodInterno,
        //                         v_Descripcion =e==null ?"": e.v_Descripcion,
        //                         Costo = e.d_PrecioCosto ?? 0,

        //                     }
        //                     ).ToList();



        //        pobjOperationResult.Success = 1;

        //        return new BindingList<BindingListaPrecioDetalleDto>(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        pobjOperationResult.Success = 0;
        //        pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
        //        return null;
        //    }
        //}

        public IQueryable ObtenerListaPreciosDetalles(ref OperationResult pobjOperationResult, string pstrIdListaPrecio)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.listapreciodetalle
                             where n.i_Eliminado == 0 && n.v_IdListaPrecios == pstrIdListaPrecio
                             orderby n.v_IdProductoDetalle  ascending
                             select n
                             );


                pobjOperationResult.Success = 1;

                var xx = query.Count();
                return query.AsQueryable();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public BindingList<BindingListaPrecioDetalleDto> ObtenerListaPrecioDetallePrueba(ref OperationResult pobjOperationResult, string pstrIdListaPrecio)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                //var query = (from n in dbContext.listapreciodetalle
                //             join o in dbContext.productoalmacen on new { pa = n.v_IdProductoAlmacen, eliminado = 0 } equals new { pa = o.v_IdProductoAlmacen, eliminado = o.i_Eliminado.Value } into o_join
                //             from o in o_join.DefaultIfEmpty()
                //             join p in dbContext.productodetalle on new { pd = o.v_ProductoDetalleId, eliminado = 0 } equals new { pd = p.v_IdProductoDetalle, eliminado = p.i_Eliminado.Value } into p_join
                //             from p in p_join.DefaultIfEmpty()
                //             join q in dbContext.producto on new { p = p.v_IdProducto, eliminado = 0 } equals new { p = q.v_IdProducto, eliminado = q.i_Eliminado.Value } into q_join
                //             from q in q_join.DefaultIfEmpty()
                //             where n.i_Eliminado == 0 && n.v_IdListaPrecios == pstrIdListaPrecio && q.i_EsActivoFijo == 0
                //             orderby n.v_IdProductoAlmacen ascending
                //             select new BindingListaPrecioDetalleDto
                //            {
                //                v_idListaPrecioDetalle = n.v_idListaPrecioDetalle,
                //                v_IdListaPrecios = n.v_IdListaPrecios,
                //                //v_IdProductoAlmacen = n.v_IdProductoAlmacen,
                //                v_IdProductoAlmacen = o.v_IdProductoAlmacen ,
                //                d_Descuento = n.d_Descuento ?? 0,
                //                d_Precio = n.d_Precio ?? 0,
                //                d_PrecioMinSoles = n.d_PrecioMinSoles ?? 0,
                //                d_PrecioMinDolares = n.d_PrecioMinDolares ?? 0,
                //                i_Eliminado = n.i_Eliminado.Value,
                //                i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                //                t_InsertaFecha = n.t_InsertaFecha,
                //                i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                //                d_Utilidad = n.d_Utilidad ?? 0,
                //                t_ActualizaFecha = n.t_ActualizaFecha.Value

                //            }).ToList();



                var query = (


                             from n in dbContext.listapreciodetalle

                             join p in dbContext.productodetalle on new { pd = n.v_IdProductoDetalle, eliminado = 0 } equals new { pd = p.v_IdProductoDetalle, eliminado = p.i_Eliminado.Value } into p_join
                             from p in p_join.DefaultIfEmpty()
                             join q in dbContext.producto on new { p = p.v_IdProducto, eliminado = 0 } equals new { p = q.v_IdProducto, eliminado = q.i_Eliminado.Value } into q_join
                             from q in q_join.DefaultIfEmpty()
                             join r in dbContext.listaprecio on new { lp = n.v_IdListaPrecios , eliminado = 0 } equals new { lp= r.v_IdListaPrecios ,eliminado= r.i_Eliminado .Value } into r_join
                             from r in r_join.DefaultIfEmpty ()
                             where n.i_Eliminado == 0 && n.v_IdListaPrecios == pstrIdListaPrecio && q.i_EsActivoFijo == 0   //&& n.i_IdAlmacen ==r.i_IdAlmacen 
                             orderby q.v_CodInterno   ascending
                             select new BindingListaPrecioDetalleDto
                             {
                                 v_idListaPrecioDetalle = n.v_idListaPrecioDetalle,
                                 v_IdListaPrecios = n.v_IdListaPrecios,
                                 //v_IdProductoAlmacen = n.v_IdProductoAlmacen,
                                 //v_IdProductoAlmacen = o.v_IdProductoAlmacen,
                                 d_Descuento = n.d_Descuento ?? 0,
                                 d_Precio = n.d_Precio ?? 0,
                                 d_PrecioMinSoles = n.d_PrecioMinSoles ?? 0,
                                 d_PrecioMinDolares = n.d_PrecioMinDolares ?? 0,
                                 i_Eliminado = n.i_Eliminado.Value,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 d_Utilidad = n.d_Utilidad ?? 0,
                                 t_ActualizaFecha = n.t_ActualizaFecha.Value,
                                 v_CodInterno =q.v_CodInterno ,
                                 v_Descripcion =q.v_Descripcion ,
                                 //Costo =q.d_PrecioCosto ??0,
                                 Costo =n.d_Costo ??0,
                                 v_IdProductoDetalle=p.v_IdProductoDetalle ,
                                 i_IdAlmacen =n.i_IdAlmacen ??0,
                                

                             }).ToList();



                var a = query.Count();
                pobjOperationResult.Success = 1;

                return new BindingList<BindingListaPrecioDetalleDto>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<listapreciodetalleDto> ListarBusquedaListaPreciosDetalle(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = (from n in dbContext.listapreciodetalle

                             //join B in dbContext.productoalmacen on new { n.v_IdProductoAlmacen, eliminado = 0 } equals new { B.v_IdProductoAlmacen, eliminado = B.i_Eliminado.Value } into B_join
                             //from B in B_join.DefaultIfEmpty()

                             join B in dbContext.productoalmacen on new { pd= n.v_IdProductoDetalle ,almacen = n.i_IdAlmacen.Value  , eliminado = 0 } equals new {pd=B.v_ProductoDetalleId ,almacen = B.i_IdAlmacen , eliminado = B.i_Eliminado.Value } into B_join
                             from B in B_join.DefaultIfEmpty()

                             join C in dbContext.productodetalle on new { prod = B.v_ProductoDetalleId, eliminado = 0 } equals new { prod = C.v_IdProductoDetalle, eliminado = C.i_Eliminado.Value } into C_join
                             from C in C_join.DefaultIfEmpty()

                             join D in dbContext.producto on new { id = C.v_IdProducto, eliminado = 0 } equals new { id = D.v_IdProducto, eliminado = D.i_Eliminado.Value } into D_join
                             from D in D_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0

                             select new listapreciodetalleDto
                             {
                                 v_CodInterno = D.v_CodInterno,
                                 v_Descripcion = D.v_Descripcion,
                                 //v_IdProductoAlmacen = n.v_IdProductoAlmacen,
                                 d_Descuento = n.d_Descuento,
                                 d_Precio = n.d_Precio,
                                 d_PrecioMinSoles = n.d_PrecioMinSoles,
                                 d_PrecioMinDolares = n.d_PrecioMinDolares
                             });
                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<listapreciodetalleDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        public string ObtenerIdProductoDetalle(string v_IdProductoAlmacen)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var ProductoDetalle = (from a in dbContext.productoalmacen
                                       where a.i_Eliminado == 0 && a.v_IdProductoAlmacen == v_IdProductoAlmacen
                                       select a).FirstOrDefault();

                if (ProductoDetalle != null)
                {

                    return ProductoDetalle.v_ProductoDetalleId;
                }
                else
                {

                    return string.Empty;
                }
            }
        }
        public void ActualizarListaPrecio(ref OperationResult pobjOperationResult, listaprecioDto pobjDtoEntity, List<string> ClientSession, List<listapreciodetalleDto> pTemp_Insertar, List<listapreciodetalleDto> pTemp_Editar, List<listapreciodetalleDto> pTemp_Eliminar, List<productoDto> pTemp_AgregarProducto, List<productoDto> pTemp_ModificarProducto, decimal TipoCambio)
        {

            try
            {
                //using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                //{
                SecuentialBL objSecuentialBL = new SecuentialBL();

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    int SecuentialId = 0;
                    string newIdListaPrecioDetalle = string.Empty;

                    List<listapreciodetalle> ListaEntidades = (from n in dbContext.listapreciodetalle
                        select n).ToList();
                    int intNodeId;

                    #region Actualiza Cabecera

                    intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (from a in dbContext.listaprecio
                        where a.v_IdListaPrecios == pobjDtoEntity.v_IdListaPrecios
                        select a).FirstOrDefault();
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    listaprecio objEntity = listaprecioAssembler.ToEntity(pobjDtoEntity);

                    dbContext.listaprecio.ApplyCurrentValues(objEntitySource);

                    #endregion

                    #region Actualiza Detalle

                    foreach (listapreciodetalleDto listapreciodetalleDto in pTemp_Insertar)
                    {
                        listapreciodetalle objEntityListaPrecioDetalle =
                            listapreciodetalleAssembler.ToEntity(listapreciodetalleDto);

                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 49);

                        newIdListaPrecioDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZF");
                        objEntityListaPrecioDetalle.v_idListaPrecioDetalle = newIdListaPrecioDetalle;
                        objEntityListaPrecioDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityListaPrecioDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityListaPrecioDetalle.i_Eliminado = 0;
                        
                        dbContext.AddTolistapreciodetalle(objEntityListaPrecioDetalle);
                    }

                    foreach (listapreciodetalleDto listapreciodetalleDto in pTemp_Editar)
                    {
                        listapreciodetalle _objEntity =
                            ListaEntidades.Where(
                                    p => p.v_idListaPrecioDetalle == listapreciodetalleDto.v_idListaPrecioDetalle)
                                .FirstOrDefault();
                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        _objEntity.d_Utilidad = listapreciodetalleDto.d_Utilidad;
                        _objEntity.d_Descuento = listapreciodetalleDto.d_Descuento;
                        _objEntity.d_PrecioMinDolares = listapreciodetalleDto.d_PrecioMinDolares;
                        _objEntity.d_PrecioMinSoles = listapreciodetalleDto.d_PrecioMinSoles;
                        _objEntity.d_Precio = listapreciodetalleDto.d_Precio;
                        _objEntity.d_Costo = listapreciodetalleDto.d_Costo;
                        dbContext.listapreciodetalle.ApplyCurrentValues(_objEntity);
                    }

                    foreach (listapreciodetalleDto listapreciodetalleDto in pTemp_Eliminar)
                    {
                        listapreciodetalle _objEntity = listapreciodetalleAssembler.ToEntity(listapreciodetalleDto);
                        var query = (from n in dbContext.listapreciodetalle
                            where n.v_idListaPrecioDetalle == listapreciodetalleDto.v_idListaPrecioDetalle
                            select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.listapreciodetalle.ApplyCurrentValues(query);
                    }

                    #endregion

                    #region Actualiza Costo del Producto

                    List<producto> ListaProductos = (from n in dbContext.producto
                        where n.i_Eliminado == 0 && n.i_EsActivoFijo == 0
                        select n).ToList();

                    foreach (productoDto ProductoDto in pTemp_ModificarProducto)
                    {
                        producto objProducto =
                            ListaProductos.Where(p => p.v_CodInterno == ProductoDto.v_CodInterno).FirstOrDefault();
                        objProducto.t_ActualizaFecha = DateTime.Now;
                        objProducto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objProducto.d_PrecioCosto = ProductoDto.d_PrecioCosto;
                        if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                        {
                            objProducto.d_PrecioVenta = ProductoDto.d_PrecioVenta;
                            objProducto.d_Utilidad = ProductoDto.d_Utilidad;
                        }
                        dbContext.producto.ApplyCurrentValues(objProducto);

                    }
                    dbContext.SaveChanges();

                    #endregion

                    ReplicarCambiosListasPorProducto(pTemp_Editar.Concat (pTemp_Insertar).ToList (), pTemp_ModificarProducto, TipoCambio,
                        int.Parse(pobjDtoEntity.i_IdAlmacen.Value.ToString()));
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    //ts.Complete();
                    return;
                    //}
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }
        public void ReplicarCambiosListasPorProducto(List<listapreciodetalleDto> TempEditar, List<productoDto> TempProducto, decimal TipoCambio, int Almacen)
        {
            try
            {
                if (TempEditar.Count <= 0) return;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var valor = TempEditar[0].v_IdListaPrecios;

                    //var lCosto = (from a in dbContext.productoalmacen

                    //    join b in dbContext.productodetalle on new {p = a.v_ProductoDetalleId, eliminado = 0} equals new {p = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value} into b_join
                    //    from b in b_join.DefaultIfEmpty()

                    //    join c in dbContext.producto on new {prod = b.v_IdProducto, eliminado = 0} equals new {prod = c.v_IdProducto, eliminado = c.i_Eliminado.Value} into c_join
                    //    from c in c_join.DefaultIfEmpty()

                    //    where a.i_Eliminado == 0

                    //    select new {c.d_PrecioCosto, a.v_IdProductoAlmacen}).ToList();


                    var lCosto = (from a in dbContext.productoalmacen

                                  join b in dbContext.productodetalle on new { p = a.v_ProductoDetalleId, eliminado = 0 } equals new { p = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()

                                  join c in dbContext.producto on new { prod = b.v_IdProducto, eliminado = 0 } equals new { prod = c.v_IdProducto, eliminado = c.i_Eliminado.Value } into c_join
                                  from c in c_join.DefaultIfEmpty()

                                  where a.i_Eliminado == 0

                                  select new { c.d_PrecioCosto, a.v_ProductoDetalleId ,a.i_IdAlmacen }).ToList();

                      
                    List<listapreciodetalle> _listapreciodetalleFaltantes = (from l in dbContext.listapreciodetalle
                        where l.v_IdListaPrecios != valor && l.i_Eliminado == 0
                        select l).ToList();
                    foreach (listapreciodetalleDto _listapreciodetalleDto in TempEditar)
                    //Replica para todos los productos solo del mismo almacén
                    {

                        if (Globals.ClientSession.i_CostoListaPreciosDiferentesxAlmacen == 1)
                        {

                            foreach (
                                listapreciodetalle l_listapreciodetalle in
                                _listapreciodetalleFaltantes.Where(
                                    p =>
                                        p.i_IdAlmacen == _listapreciodetalleDto.i_IdAlmacen && p.v_IdProductoDetalle == _listapreciodetalleDto.v_IdProductoDetalle &&
                                        p.i_Eliminado == 0))
                            {
                                //var Costo =
                                //    lCosto.Where(p => p.i_IdAlmacen == l_listapreciodetalle.i_IdAlmacen && p.v_ProductoDetalleId == l_listapreciodetalle.v_IdProductoDetalle)
                                //        .First();

                                var Costo =
                                    TempEditar.Where(p => p.i_IdAlmacen == l_listapreciodetalle.i_IdAlmacen && p.v_IdProductoDetalle  == l_listapreciodetalle.v_IdProductoDetalle)
                                        .First();

                                l_listapreciodetalle.d_Costo = Costo.d_Costo ?? 0;
                                //l_listapreciodetalle.d_Precio = Utils.Windows.DevuelveValorRedondeado((Costo.d_PrecioCosto.Value + ((Costo.d_PrecioCosto.Value * l_listapreciodetalle.d_Utilidad.Value) / 100)), (int)Globals.ClientSession.i_PrecioDecimales);
                                l_listapreciodetalle.d_Precio = l_listapreciodetalle.d_Utilidad ==0 ?0:  Utils.Windows.DevuelveValorRedondeado((Costo.d_Costo.Value + ((Costo.d_Costo.Value * l_listapreciodetalle.d_Utilidad.Value) / 100)), (int)Globals.ClientSession.i_PrecioDecimales);
                                l_listapreciodetalle.d_PrecioMinSoles = Utils.Windows.DevuelveValorRedondeado((l_listapreciodetalle.d_Precio - ((l_listapreciodetalle.d_Precio * l_listapreciodetalle.d_Descuento) / 100)).Value, (int)Globals.ClientSession.i_PrecioDecimales);
                                l_listapreciodetalle.d_PrecioMinDolares = Utils.Windows.DevuelveValorRedondeado((l_listapreciodetalle.d_PrecioMinSoles / TipoCambio).Value, (int)Globals.ClientSession.i_PrecioDecimales);
                                dbContext.listapreciodetalle.ApplyCurrentValues(l_listapreciodetalle);

                            }
                        }
                        else
                        {

                            foreach (
                                    listapreciodetalle l_listapreciodetalle in
                                    _listapreciodetalleFaltantes.Where(
                                        p =>
                                          p.v_IdProductoDetalle == _listapreciodetalleDto.v_IdProductoDetalle &&
                                            p.i_Eliminado == 0))
                            {
                                //var Costo =
                                //    lCosto.Where(p => p.i_IdAlmacen == l_listapreciodetalle.i_IdAlmacen && p.v_ProductoDetalleId == l_listapreciodetalle.v_IdProductoDetalle)
                                //        .First();
                                var Costo =
                                    TempEditar.Where(p => p.i_IdAlmacen == l_listapreciodetalle.i_IdAlmacen && p.v_IdProductoDetalle == l_listapreciodetalle.v_IdProductoDetalle)
                                        .First();

                                l_listapreciodetalle.d_Costo = Costo.d_Costo ?? 0;
                                l_listapreciodetalle.d_Precio = l_listapreciodetalle.d_Utilidad ==0 ?0 : Utils.Windows.DevuelveValorRedondeado((Costo.d_Costo.Value + ((Costo.d_Costo.Value * l_listapreciodetalle.d_Utilidad.Value) / 100)), (int)Globals.ClientSession.i_PrecioDecimales);
                                //l_listapreciodetalle.d_Precio = Utils.Windows.DevuelveValorRedondeado((Costo.d_PrecioCosto.Value + ((Costo.d_PrecioCosto.Value * l_listapreciodetalle.d_Utilidad.Value) / 100)), (int)Globals.ClientSession.i_PrecioDecimales);
                                l_listapreciodetalle.d_PrecioMinSoles = Utils.Windows.DevuelveValorRedondeado((l_listapreciodetalle.d_Precio - ((l_listapreciodetalle.d_Precio * l_listapreciodetalle.d_Descuento) / 100)).Value, (int)Globals.ClientSession.i_PrecioDecimales);
                                l_listapreciodetalle.d_PrecioMinDolares = Utils.Windows.DevuelveValorRedondeado((l_listapreciodetalle.d_PrecioMinSoles / TipoCambio).Value, (int)Globals.ClientSession.i_PrecioDecimales);
                                dbContext.listapreciodetalle.ApplyCurrentValues(l_listapreciodetalle);

                            }
                        }


                    }


                    dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
        }
      
        public listaprecioDto ObtenerListaPrecioCabecera(ref OperationResult objOperationResult, string strIdListaPrecio)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    listaprecioDto objEntity = (from a in dbContext.listaprecio
                        where a.v_IdListaPrecios == strIdListaPrecio

                        select new listaprecioDto
                        {
                            i_IdAlmacen = a.i_IdAlmacen,
                            i_IdLista = a.i_IdLista,
                            v_IdListaPrecios = a.v_IdListaPrecios,
                            i_IdMoneda = a.i_IdMoneda.Value,
                        }).FirstOrDefault();
                     objOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        public List<listaprecioDto> ObtenerListadoListaPrecios(ref OperationResult pobjOperationResult)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbContext.listaprecio
                    where n.i_Eliminado == 0

                    orderby n.v_IdListaPrecios ascending
                    select n).ToList();

                List<listaprecioDto> listaprecio = listaprecioAssembler.ToDTOs(query);
                return listaprecio;
            }
        }
        public string[] DevolverProductos(string IdProductoDetalle) // Productos x almacen
        {
            string success;
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                var EntityProducto = (from n in dbContext.productodetalle

                    join A in dbContext.producto on new {p = n.v_IdProducto, eliminado = 0} equals
                    new {p = A.v_IdProducto, eliminado = A.i_Eliminado.Value} into A_join
                    from A in A_join.DefaultIfEmpty()

                   where  n.i_Eliminado == 0 && n.v_IdProductoDetalle == IdProductoDetalle

                    select new
                    {
                        Nombre = A.v_Descripcion.Trim(),
                        CodigoInterno = A.v_CodInterno.Trim(),
                        Costo = A.d_PrecioCosto,
                        Utilidad= A.d_Utilidad,
                        PrecioVenta =A.d_PrecioVenta,
                    }
                ).FirstOrDefault();


                string[] Cadena = new string[5];
                if (EntityProducto.Nombre != null && EntityProducto.CodigoInterno != null)
                {
                    Cadena[0] = EntityProducto.CodigoInterno != null ? EntityProducto.CodigoInterno : "";
                    Cadena[1] = EntityProducto.Nombre != null ? EntityProducto.Nombre : "";
                    Cadena[2] = EntityProducto.Costo.Value != null ?  EntityProducto.Costo.Value.ToString() : "0";
                    Cadena[3] = EntityProducto.Utilidad != null ? EntityProducto.Utilidad.Value.ToString() : "0";
                    Cadena[4] = EntityProducto.PrecioVenta != null ? EntityProducto.PrecioVenta.Value.ToString() : "0";


                }
                else
                {
                    success = "0";

                }

                return Cadena;
            }
        }
        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                //SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                //var query = (from n in dbContext.tipodecambio
                //             where n.i_IsDeleted == 0 && n.d_FechaTipoC == Fecha
                //             select n
                //             ).FirstOrDefault();

                //pobjOperationResult.Success = 1;

                //if (query != null)
                //{
                //    string TipoCambio = query.d_ValorVenta.ToString();
                //    return TipoCambio;
                //}
                //else
                //{
                //    //return string.Empty;
                //    return "0"; // cambie
                //}
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaVenta(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        public listapreciodetalleDto ObtenerListaPrecioDetalle(ref OperationResult pobjOperationResult, int IdAlmacen, int idLista, string IdProductoDetalle)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    listapreciodetalleDto ListaPrecioDetalle = new listapreciodetalleDto();

                    var ObjEntity = (from a in dbContext.listaprecio

                        join b in dbContext.listapreciodetalle on a.v_IdListaPrecios equals b.v_IdListaPrecios

                        where
                        a.i_IdAlmacen == IdAlmacen && a.i_IdLista == idLista &&
                        b.v_IdProductoDetalle == IdProductoDetalle

                        select new listapreciodetalleDto
                        {
                            d_Descuento = b.d_Descuento,
                            d_Precio = b.d_Precio,



                        }).FirstOrDefault();


                    pobjOperationResult.Success = 1;

                    return ObjEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }


        }


        public List<listapreciodetalleDto> ObtenerListaPrecioDetallesVenta(ref OperationResult pobjOperationResult, string pstrProductoDetalle, int idAlmacen) //,int IdListaCliente
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //if (IdListaCliente != -1)
                    //{
                    var query = (from A in dbContext.listaprecio

                        join E in dbContext.listapreciodetalle on new {IdLista = A.v_IdListaPrecios, eliminado = 0}
                        equals new {IdLista = E.v_IdListaPrecios, eliminado = E.i_Eliminado.Value} into E_join

                        from E in E_join.DefaultIfEmpty()

                        //join B in dbContext.productoalmacen on
                        //new {IdProdAlmacen = E.v_IdProductoAlmacen, eliminado = 0} equals
                        //new {IdProdAlmacen = B.v_IdProductoAlmacen, eliminado = B.i_Eliminado.Value} into B_join
                        //from B in B_join.DefaultIfEmpty()

                                 join B in dbContext.productoalmacen on new { IdProdDetalle = E.v_IdProductoDetalle, eliminado = 0, almacen = A.i_IdAlmacen.Value } equals new { IdProdDetalle = B.v_IdProductoAlmacen, eliminado = B.i_Eliminado.Value, almacen = B.i_IdAlmacen } into B_join
                         from B in B_join.DefaultIfEmpty()
                        join C in dbContext.productodetalle on
                        new {IdProductoDetalle = B.v_ProductoDetalleId, eliminado = 0} equals
                        new {IdProductoDetalle = C.v_IdProductoDetalle, eliminado = C.i_Eliminado.Value} into C_join
                        from C in C_join.DefaultIfEmpty()

                        join D in dbContext.producto on new {IdProducto = C.v_IdProducto, eliminado = 0} equals
                        new {IdProducto = D.v_IdProducto, eliminado = D.i_Eliminado.Value} into D_join
                        from D in D_join.DefaultIfEmpty()

                        join F in dbContext.datahierarchy on
                        new {IdCabecera = A.i_IdLista.Value, eliminado = 0, Grupo = 47} equals
                        new {IdCabecera = F.i_ItemId, eliminado = F.i_IsDeleted.Value, Grupo = F.i_GroupId} into F_join
                        from F in F_join.DefaultIfEmpty()

                        where
                        A.i_Eliminado == 0 && A.i_IdAlmacen == idAlmacen && C.v_IdProductoDetalle == pstrProductoDetalle

                        select new listapreciodetalleDto
                        {

                            v_CodInterno = D.v_CodInterno,
                            v_Descripcion = D.v_Descripcion,
                            //v_IdProductoAlmacen = E.v_IdProductoAlmacen,
                            d_Descuento = E.d_Descuento,
                            d_Precio = E.d_Precio,
                            d_PrecioMinSoles = E.d_PrecioMinSoles,
                            d_PrecioMinDolares = E.d_PrecioMinDolares,
                            v_IdProductoDetalle =C.v_IdProductoDetalle ,
                            NombreLista = F == null ? "" : F.v_Value1,
                            Moneda = A.i_IdMoneda == (int) Currency.Soles ? "SOLES" : "DOLARES",
                            i_IdMoneda = A.i_IdMoneda.Value,


                        }).ToList();
                   
                    List<listapreciodetalleDto> objData = query.ToList().OrderBy(x => x.NombreLista).ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }

        #region Bandeja

        public List<listaprecioDto> ListarBusquedaListaPrecios(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.listaprecio

                        join J1 in dbContext.systemuser on new {i_InsertUserId = n.i_InsertaIdsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = n.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()

                        join J3 in dbContext.almacen on n.i_IdAlmacen equals J3.i_IdAlmacen into J3_join

                        from J3 in J3_join.DefaultIfEmpty()

                        join J4 in dbContext.datahierarchy on new {i_IdLista = n.i_IdLista.Value, b = 47}
                        equals new {i_IdLista = J4.i_ItemId, b = J4.i_GroupId} into J4_join
                        from J4 in J4_join.DefaultIfEmpty()

                        where n.i_Eliminado == 0

                        select new listaprecioDto
                        {

                            v_IdListaPrecios = n.v_IdListaPrecios,
                            i_IdLista = n.i_IdLista,
                            i_IdAlmacen = n.i_IdAlmacen,
                            t_InsertaFecha = n.t_InsertaFecha,
                            t_ActualizaFecha = n.t_ActualizaFecha,
                            v_UsuarioCreacion = J1.v_UserName,
                            v_UsuarioModificacion = J2.v_UserName,
                            Almacen = J3.v_Nombre,
                            Lista = J4.v_Value1,
                            Moneda = n.i_IdMoneda == 1 ? "SOLES" : "DOLARES",

                        });

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }
                    pobjOperationResult.Success = 1;

                    List<listaprecioDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
        #region Reportes
        public List<ImpresionListaPrecios> ImpresionListaPreciosyDetalles(ref OperationResult pobjOperationResult, int idAlmacen, int IdLista) //,int IdListaCliente
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string Periodo = Globals.ClientSession.i_Periodo.ToString();
                    var query = (from A in dbContext.listapreciodetalle

                                 join B in dbContext.listaprecio on new { IdLista = A.v_IdListaPrecios, eliminado = 0 } equals new { IdLista = B.v_IdListaPrecios, eliminado = B.i_Eliminado.Value } into B_join

                                 from B in B_join.DefaultIfEmpty()

                                 //join C in dbContext.productoalmacen on new { IdProdDetalle = A.v_IdProductoDetalle, eliminado = 0, almacen = A.i_IdAlmacen.Value } equals new { IdProdDetalle = C.v_IdProductoAlmacen, eliminado = C.i_Eliminado.Value, almacen = C.i_IdAlmacen } into C_join
                                 //from C in C_join.DefaultIfEmpty()

                                 join D in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle , eliminado = 0 } equals new { IdProductoDetalle = D.v_IdProductoDetalle, eliminado = D.i_Eliminado.Value } into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join E in dbContext.producto on new { IdProducto = D.v_IdProducto, eliminado = 0 } equals new { IdProducto = E.v_IdProducto, eliminado = E.i_Eliminado.Value } into E_join
                                 from E in E_join.DefaultIfEmpty()

                                 join F in dbContext.datahierarchy on new { IdCabecera = B.i_IdLista.Value, eliminado = 0, Grupo = 47 } equals new { IdCabecera = F.i_ItemId, eliminado = F.i_IsDeleted.Value, Grupo = F.i_GroupId } into F_join
                                 from F in F_join.DefaultIfEmpty()

                                 join G in dbContext.almacen on new { a = B.i_IdAlmacen.Value, eliminado = 0 } equals new { a = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join

                                 from G in G_join.DefaultIfEmpty()

                                 where A.i_Eliminado == 0 && B.i_IdAlmacen == idAlmacen && F.i_ItemId == IdLista  && E.i_EsActivoFijo == 0 && A.i_IdAlmacen ==  idAlmacen 

                                 select new ImpresionListaPrecios
                                 {

                                     v_CodInterno = E.v_CodInterno + " " + E.v_Descripcion ,
                                  
                                     d_Descuento = A.d_Descuento ?? 0,
                                     d_Precio = A.d_Precio ?? 0,
                                     d_PrecioMinSoles = A.d_PrecioMinSoles ?? 0,
                                     d_PrecioMinDolares = A.d_PrecioMinDolares ?? 0,
                                     NombreLista = F == null ? "" : F.v_Value1,
                                     Moneda = B.i_IdMoneda == (int)Currency.Soles ?"MONEDA : "+ "SOLES" : "MONEDA : "+ "DOLARES",
                                     Almacen ="ALMACÉN : "+ G.v_Nombre,
                                     d_Utilidad =A.d_Utilidad ??0,
                                     Costo =E.d_PrecioCosto ??0


                                 }).ToList();
                    //if (!string.IsNullOrEmpty(pstrFilterExpression))
                    //{
                    //    query = query.Where(pstrFilterExpression);
                    //}
                    //if (!string.IsNullOrEmpty(pstrSortExpression))
                    //{
                    //    query = query.OrderBy(pstrSortExpression);
                    //}
                    //}
                    List<ImpresionListaPrecios> objData = query.ToList().OrderBy(x => x.NombreLista).ToList().OrderBy(x => x.v_CodInterno).ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        #endregion

    }
}
