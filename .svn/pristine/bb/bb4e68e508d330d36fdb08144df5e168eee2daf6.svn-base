using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq.Dynamic;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Common.BE.Custom;
namespace SAMBHS.Contabilidad.BL
{
    public class PendientePorCobrarBL
    {
        public void RecalcularPendientesPorCobrar(ref OperationResult pobjOperationResult,DateTime FechaInicio, DateTime FechaFin, List<string> ClientSession)
        {
             OperationResult objOperationResult = new OperationResult();
             DiarioBL objDiarioBL = new DiarioBL();
             List<diarioDto> objListadiarioDto = new List<diarioDto>();

             TesoreriaBL objTesoreriaBL = new TesoreriaBL();
             List<tesoreriadetalleDto> objListaTesoreriaDto = new List<tesoreriadetalleDto>();

             List<pendientecobrarDto> ListpendientecobrarDto = new List<pendientecobrarDto>();
             pendientecobrarDto objpendientecobrarDto;
             pendientecobrardetalleDto objpendientecobrardetalleDto;

             try
             {
                 //1.- Traer los registros de la tabla Diario y Detalle que estén dentro del mes del rango seleccionado
                 objListadiarioDto = objDiarioBL.DevolverDataJerarquizadaDiario_DiarioDetalle(ref objOperationResult, FechaInicio, FechaFin);

                 //2.- Traer los registros de la tabla Tesorería y Detalle que estén dentro del mes del rango seleccionado
                 objListaTesoreriaDto = objTesoreriaBL.DevolverTesoreriaDetalle(ref objOperationResult, FechaInicio, FechaFin);

                 //3.- Eliminar los registros de la tabla Pendientes por Cobrar que esten dentro del mes seleccionado
                 EliminarPendientesPorCobrarCabeceraDetalle(FechaInicio, FechaFin);

                 //4.- Volver a Insertar data en las tablas PedienteCobrar y PendienteCobrarDetalle

                 //4.1- Crear los registros Base (que viene del Diario) de PendienteCobrar y Detalle
                 foreach (var itemDiario in objListadiarioDto)
                 {
                     //ListpendientecobrarDto = new List<pendientecobrarDto>();
                     objpendientecobrarDto = new pendientecobrarDto();
                     objpendientecobrardetalleDto = new pendientecobrardetalleDto();

                     objpendientecobrarDto.v_NroCuenta = itemDiario.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").v_NroCuenta;
                     objpendientecobrarDto.i_IdTipoDocumento = itemDiario.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").i_IdTipoDocumento;
                     objpendientecobrarDto.v_NroDocumento = itemDiario.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").v_NroDocumento;
                     objpendientecobrarDto.v_IdCliente = itemDiario.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").v_IdCliente;
                     objpendientecobrarDto.t_FechaRegistro = itemDiario.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha;
                     objpendientecobrarDto.t_FechaReferencia = null; // item.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha; Es la Fecha de Vencimiento del Registro de Venta
                     objpendientecobrarDto.i_IdMoneda = itemDiario.i_IdMoneda; // Moneda de la cabecera del Diario

                     var SumaImportes = objListaTesoreriaDto.FindAll(p => p.i_IdTipoDocumento == objpendientecobrarDto.i_IdTipoDocumento && p.v_NroDocumento == objpendientecobrarDto.v_NroDocumento).Sum(s => s.d_Importe);
                     objpendientecobrarDto.d_ImporteSaldo = itemDiario.d_TotalDebe - SumaImportes;

                     objpendientecobrardetalleDto.v_NroCuenta = objpendientecobrarDto.v_NroCuenta;
                     objpendientecobrardetalleDto.i_IdTipoDocumento = objpendientecobrarDto.i_IdTipoDocumento;
                     objpendientecobrardetalleDto.v_NroDocumento = objpendientecobrarDto.v_NroDocumento;
                     objpendientecobrardetalleDto.v_IdCliente = objpendientecobrarDto.v_IdCliente;
                     objpendientecobrardetalleDto.t_FechaRegistro = objpendientecobrarDto.t_FechaRegistro;
                     objpendientecobrardetalleDto.t_FechaReferencia = objpendientecobrarDto.t_FechaReferencia; // item.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha; Es la Fecha de Vencimiento del Registro de Venta
                     objpendientecobrardetalleDto.i_IdMoneda = objpendientecobrarDto.i_IdMoneda; // Moneda de la cabecera del Diario
                     objpendientecobrardetalleDto.d_ImporteSaldo = itemDiario.d_TotalDebe;
                     objpendientecobrardetalleDto.v_IdDiario = itemDiario.v_IdDiario;
                     objpendientecobrardetalleDto.v_Naturaleza = "D";
                     
                     //Grabar Data Base PendienteCobrar y Detalle que viene del diario
                     objpendientecobrarDto.v_IdPendienteCobrar = GrabarPendienteCobrarBase(objpendientecobrarDto, objpendientecobrardetalleDto, ClientSession);

                     //Almacenar la cabecera de pendientes en una tabla temporal
                     ListpendientecobrarDto.Add(objpendientecobrarDto);
                 }

                 //4.2- Crear los registros generados por las tablas de Tesorería y TesoreríaDetalle

                 GrabarPendienteCobrar(objListaTesoreriaDto, ListpendientecobrarDto, ClientSession);
                 pobjOperationResult.Success = 1;
             }
             catch (Exception ex)
             {
                 pobjOperationResult.Success = 0;
                 pobjOperationResult.ExceptionMessage = ex.Message;
                 return;
             }
        }

        private void GrabarPendienteCobrar(List<tesoreriadetalleDto> objListaTesoreriaDto, List<pendientecobrarDto> ListpendientecobrarDto, List<string> ClientSession)
        {
            try
            {
                 pendientecobrarDto objpendientecobrarDto = new pendientecobrarDto();
                 pendientecobrardetalleDto objpendientecobrardetalleDto = new pendientecobrardetalleDto(); 

                 foreach (var itemTesoreriaDetalle in objListaTesoreriaDto)
                 {
                    var Result =  ListpendientecobrarDto.Find(p => p.i_IdTipoDocumento == itemTesoreriaDetalle.i_IdTipoDocumento && p.v_NroDocumento == itemTesoreriaDetalle.v_NroDocumento);
                    if (Result != null)
                    {
                        //Crear Entidad "Pendiente Cobrar Detalle"
                        objpendientecobrardetalleDto.v_IdPendienteCobrar = Result.v_IdPendienteCobrar;
                        objpendientecobrardetalleDto.v_NroCuenta = itemTesoreriaDetalle.v_NroCuenta;
                        objpendientecobrardetalleDto.i_IdTipoDocumento = itemTesoreriaDetalle.i_IdTipoDocumento;
                        objpendientecobrardetalleDto.v_NroDocumento = itemTesoreriaDetalle.v_NroDocumento;
                        objpendientecobrardetalleDto.v_IdCliente = itemTesoreriaDetalle.v_IdCliente;
                        objpendientecobrardetalleDto.t_FechaRegistro = itemTesoreriaDetalle.t_Fecha;
                        objpendientecobrardetalleDto.t_FechaReferencia = null; // item.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha; Es la Fecha de Vencimiento del Registro de Venta
                        objpendientecobrardetalleDto.i_IdMoneda = itemTesoreriaDetalle.i_IdMoneda; // Moneda de la cabecera del Diario
                        objpendientecobrardetalleDto.d_ImporteSaldo = itemTesoreriaDetalle.d_Importe;
                        objpendientecobrardetalleDto.v_IdDiario = null;
                        objpendientecobrardetalleDto.v_IdTesoreria = itemTesoreriaDetalle.v_IdTesoreria;
                        objpendientecobrardetalleDto.v_Naturaleza = "D";
                        

                        //insertar el registro de "Pendiente Cobrar Detalle"
                        GrabarPendienteCobrarDetalle(objpendientecobrardetalleDto, Globals.ClientSession.GetAsList());
                    }
                    else
                    {
                        //Buscar si el documento existe ya en la tabla "Pendiente Cobrar"
                        Result = DevolverPendienteCobrar((int)itemTesoreriaDetalle.i_IdTipoDocumento, itemTesoreriaDetalle.v_NroDocumento); //ListpendientecobrarDto.Find(p => p.i_IdTipoDocumento == itemTesoreriaDetalle.i_IdTipoDocumento && p.NroDocumento == itemTesoreriaDetalle.v_NroDocumento);
                        if (Result != null)
                        {
                            objpendientecobrardetalleDto.v_IdPendienteCobrar = Result.v_IdPendienteCobrar;
                            objpendientecobrardetalleDto.v_IdTesoreria = itemTesoreriaDetalle.v_IdTesoreria;
                            objpendientecobrardetalleDto.v_NroCuenta = itemTesoreriaDetalle.v_NroCuenta;
                            objpendientecobrardetalleDto.i_IdTipoDocumento = itemTesoreriaDetalle.i_IdTipoDocumento;
                            objpendientecobrardetalleDto.v_NroDocumento = itemTesoreriaDetalle.v_NroDocumento;
                            objpendientecobrardetalleDto.v_IdCliente = itemTesoreriaDetalle.v_IdCliente;
                            objpendientecobrardetalleDto.t_FechaRegistro = itemTesoreriaDetalle.t_Fecha;
                            objpendientecobrardetalleDto.t_FechaReferencia = null; // item.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha; Es la Fecha de Vencimiento del Registro de Venta
                            objpendientecobrardetalleDto.i_IdMoneda = itemTesoreriaDetalle.i_IdMoneda; // Moneda de la cabecera del Diario
                            objpendientecobrardetalleDto.d_ImporteSaldo = itemTesoreriaDetalle.d_Importe * (-1);
                            objpendientecobrardetalleDto.v_IdDiario = null;
                            objpendientecobrardetalleDto.v_Naturaleza = "D";

                            //si no existe el registro entonces creo la cabecera de "Pendiente Cobrar" y agrego su detalle "Pendiente Cobrar Detalle"
                            GrabarPendienteCobrarDetalle(objpendientecobrardetalleDto, Globals.ClientSession.GetAsList());

                        }
                        else
                        {
                            objpendientecobrarDto = new pendientecobrarDto();
                            objpendientecobrardetalleDto = new pendientecobrardetalleDto();

                            objpendientecobrarDto.v_NroCuenta = itemTesoreriaDetalle.v_NroCuenta;
                            objpendientecobrarDto.i_IdTipoDocumento = itemTesoreriaDetalle.i_IdTipoDocumento;
                            objpendientecobrarDto.v_NroDocumento = itemTesoreriaDetalle.v_NroDocumento;
                            objpendientecobrarDto.v_IdCliente = itemTesoreriaDetalle.v_IdCliente;
                            objpendientecobrarDto.t_FechaRegistro = itemTesoreriaDetalle.t_Fecha;
                            objpendientecobrarDto.t_FechaReferencia = null; // item.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha; Es la Fecha de Vencimiento del Registro de Venta
                            objpendientecobrarDto.i_IdMoneda = itemTesoreriaDetalle.i_IdMoneda; // Moneda de la cabecera del Diario
                            var SumaImportes = objListaTesoreriaDto.FindAll(p => p.i_IdTipoDocumento == objpendientecobrarDto.i_IdTipoDocumento && p.v_NroDocumento == objpendientecobrarDto.v_NroDocumento).Sum(s => s.d_Importe);
                            objpendientecobrarDto.d_ImporteSaldo = SumaImportes * (-1);

                            objpendientecobrardetalleDto.v_IdTesoreria = itemTesoreriaDetalle.v_IdTesoreria;
                            objpendientecobrardetalleDto.v_NroCuenta = objpendientecobrarDto.v_NroCuenta;
                            objpendientecobrardetalleDto.i_IdTipoDocumento = objpendientecobrarDto.i_IdTipoDocumento;
                            objpendientecobrardetalleDto.v_NroDocumento = objpendientecobrarDto.v_NroDocumento;
                            objpendientecobrardetalleDto.v_IdCliente = objpendientecobrarDto.v_IdCliente;
                            objpendientecobrardetalleDto.t_FechaRegistro = objpendientecobrarDto.t_FechaRegistro;
                            objpendientecobrardetalleDto.t_FechaReferencia = objpendientecobrarDto.t_FechaReferencia; // item.DiarioDetallePersonalizado.Find(p => p.v_Naturaleza == "D").t_Fecha; Es la Fecha de Vencimiento del Registro de Venta
                            objpendientecobrardetalleDto.i_IdMoneda = objpendientecobrarDto.i_IdMoneda; // Moneda de la cabecera del Diario
                            objpendientecobrardetalleDto.d_ImporteSaldo = itemTesoreriaDetalle.d_Importe * (-1);
                            objpendientecobrardetalleDto.v_IdDiario = null;
                            objpendientecobrardetalleDto.v_Naturaleza = "D";

                            //si no existe el registro entonces creo la cabecera de "Pendiente Cobrar" y agrego su detalle "Pendiente Cobrar Detalle"
                            GrabarPendienteCobrarBase(objpendientecobrarDto, objpendientecobrardetalleDto, Globals.ClientSession.GetAsList());
                        }    
                    }
                 }
            }
            catch (Exception)
            {                
                throw;
            }
        }

        public void EliminarPendientesPorCobrarCabeceraDetalle(DateTime FechaInicio, DateTime FechaFin)
        {
            using (var dbcontext = new SAMBHSEntitiesModelWin())
            {

                var Query1 = (from a in dbcontext.pendientecobrar
                    where a.t_FechaRegistro > FechaInicio && a.t_FechaRegistro < FechaFin
                    select a).ToList();


                foreach (var item in Query1)
                {
                    dbcontext.pendientecobrar.DeleteObject(item);

                    var Query2 = (from a in dbcontext.pendientecobrardetalle
                        where a.v_IdPendienteCobrar == item.v_IdPendienteCobrar
                        select a).ToList();

                    foreach (var item1 in Query2)
                    {
                        dbcontext.pendientecobrardetalle.DeleteObject(item1);
                    }

                }

                dbcontext.SaveChanges();    
            }
        }

        public string GrabarPendienteCobrarBase(pendientecobrarDto objpendientecobrarDto, pendientecobrardetalleDto objpendientecobrardetalleDto, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;
            string newIdDetail = string.Empty;
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                SecuentialBL objSecuentialBL = new SecuentialBL();

                #region Pendiente Cobrar

                pendientecobrar objEntity = pendientecobrarAssembler.ToEntity(objpendientecobrarDto);

                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);

                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 61);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XK");
                objEntity.v_IdPendienteCobrar = newId;

                dbContext.AddTopendientecobrar(objEntity);
                //dbContext.SaveChanges(); 

                #endregion

                #region Pendiente Cobrar Detalle
                pendientecobrardetalle objEntityDetail = pendientecobrardetalleAssembler.ToEntity(objpendientecobrardetalleDto);

                objEntityDetail.v_IdPendienteCobrar = newId;
                objEntityDetail.t_InsertaFecha = DateTime.Now;
                objEntityDetail.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);

                // Autogeneramos el Pk de la tabla
                //int intNodeDId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 62);
                newIdDetail = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                objEntityDetail.v_IdPendienteCobrarDetalle = newIdDetail;

                dbContext.AddTopendientecobrardetalle(objEntityDetail);
                dbContext.SaveChanges();
                #endregion

                return newId; 
            }
        }

        public void GrabarPendienteCobrarDetalle(pendientecobrardetalleDto objpendientecobrardetalleDto, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var objSecuentialBL = new SecuentialBL();

                pendientecobrardetalle objEntityDetail = pendientecobrardetalleAssembler.ToEntity(objpendientecobrardetalleDto);

                objEntityDetail.v_IdPendienteCobrar = objpendientecobrardetalleDto.v_IdPendienteCobrar;
                objEntityDetail.t_InsertaFecha = DateTime.Now;
                objEntityDetail.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);

                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 62);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                objEntityDetail.v_IdPendienteCobrarDetalle = newId;

                dbContext.AddTopendientecobrardetalle(objEntityDetail);
                dbContext.SaveChanges(); 
            }
        }

        public pendientecobrarDto DevolverPendienteCobrar(int pintIdTipoDocumento,  string pstrNroDocumento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                pendientecobrarDto objDtoEntity = null;

                var query = (from A in dbContext.pendientecobrar
                    where A.i_IdTipoDocumento == pintIdTipoDocumento && A.v_NroDocumento == pstrNroDocumento
                    select A).FirstOrDefault();

                if (query != null)
                    objDtoEntity = pendientecobrarAssembler.ToDTO(query);

                return objDtoEntity; 
            }
        }

    }

    //public class RecalcularPendientesBL
    //{
    //    public List<ReporteAnalisisCuentas> ReporteAnalisisCuentas(ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string nroCuenta)
    //    {
    //        try
    //        {
    //            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
    //            {
    //                List<ReporteAnalisisCuentas> Grupos = new List<ReporteAnalisisCuentas>();

    //                #region Recopila las Cuentas Mayores
    //                var CuentasMayores = (from n in dbContext.asientocontable
    //                                      where n.i_LongitudJerarquica == 2 && n.i_Eliminado == 0
    //                                      select n).ToList();

    //                #endregion

    //                #region Recopila Cuentas de Tesorería
    //                var GruposTesoreria = (from n in dbContext.tesoreriadetalle

    //                                       join J1 in dbContext.asientocontable on n.v_NroCuenta.Trim() equals J1.v_NroCuenta.Trim() into J1_join
    //                                       from J1 in J1_join.DefaultIfEmpty()

    //                                       join J2 in dbContext.documento on n.i_IdTipoDocumento.Value equals J2.i_CodigoDocumento into J2_join
    //                                       from J2 in J2_join.DefaultIfEmpty()

    //                                       join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
    //                                       from J3 in J3_join.DefaultIfEmpty()

    //                                       join J4 in dbContext.tesoreria on n.v_IdTesoreria equals J4.v_IdTesoreria into J4_join
    //                                       from J4 in J4_join.DefaultIfEmpty()

    //                                       join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
    //                                       from J5 in J5_join.DefaultIfEmpty()

    //                                       where n.i_Eliminado == 0 && n.t_Fecha >= F_Ini && n.t_Fecha <= F_Fin

    //                                       select new
    //                                       {
    //                                           CuentaMayor = n.v_NroCuenta.Substring(0, 2),
    //                                           CuentaImputable = n.v_NroCuenta,
    //                                           NombreCuenta = J1 != null ? J1.v_NombreCuenta : string.Empty,
    //                                           i_Detalle = J1 != null ? J1.i_Detalle == 1 && J1.i_Analisis == 1 ? true : false : false,
    //                                           FechaDocumento = n.t_Fecha,
    //                                           Detalle = J3.v_CodCliente + " " + n.v_IdCliente != "N002-CL000000000" ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_RazonSocial).Trim() : "PÚBLICO GENERAL",
    //                                           Documento = J2.v_Siglas + " " + n.v_NroDocumento,
    //                                           DocumentoRef = J5 != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : string.Empty,
    //                                           Analisis = n.v_Analisis == null ? !string.IsNullOrEmpty(J4.v_Nombre) ? J4.v_Nombre : string.Empty : n.v_Analisis,
    //                                           IdMoneda = J4.i_IdMoneda,
    //                                           Importe = n.d_Importe,
    //                                           ImporteCambio = n.d_Cambio,
    //                                           Key = n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
    //                                           NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
    //                                           IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
    //                                           NaturalezaRegistro = n.v_Naturaleza
    //                                       }
    //                                        ).ToList().Select(p => new ReporteAnalisisCuentas
    //                                        {
    //                                            CuentaMayor = p.CuentaMayor,
    //                                            CuentaImputable = p.CuentaImputable,
    //                                            NombreCuenta = p.NombreCuenta,
    //                                            Detalle = p.i_Detalle == true ? p.Detalle : string.Empty,
    //                                            Documento = p.i_Detalle == true ? (p.FechaDocumento.Value.ToShortDateString() + " " + p.Documento + " " + p.Analisis).Trim() : string.Empty,
    //                                            DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? p.i_Detalle == true ? (p.FechaDocumento.Value.ToShortDateString() + " " + p.DocumentoRef + " " + p.Analisis).Trim() : string.Empty : string.Empty,
    //                                            FechaDocumento = p.FechaDocumento,
    //                                            ImporteSoles = p.IdMoneda == 1 ? p.Importe : p.ImporteCambio,
    //                                            ImporteDolares = p.IdMoneda == 1 ? p.ImporteCambio : p.Importe,
    //                                            DocumentoKey = p.Key,
    //                                            IdMoneda = (int)p.IdMoneda,
    //                                            IdMonedaCuenta = (int)p.IdMonedaCuenta,
    //                                            NaturalezaCuenta = p.NaturalezaCuenta,
    //                                            NaturalezaRegistro = p.NaturalezaRegistro
    //                                        }
    //                                        ).ToList();

    //                GruposTesoreria = GruposTesoreria.Where(p => !string.IsNullOrEmpty(p.NombreCuenta)).ToList();
    //                #endregion

    //                #region Recopila Cuentas de Diario
    //                var GruposDiario = (from n in dbContext.diariodetalle

    //                                    join J1 in dbContext.asientocontable on n.v_NroCuenta equals J1.v_NroCuenta into J1_join
    //                                    from J1 in J1_join.DefaultIfEmpty()

    //                                    join J2 in dbContext.documento on n.i_IdTipoDocumento.Value equals J2.i_CodigoDocumento into J2_join
    //                                    from J2 in J2_join.DefaultIfEmpty()

    //                                    join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
    //                                    from J3 in J3_join.DefaultIfEmpty()

    //                                    join J4 in dbContext.diario on n.v_IdDiario equals J4.v_IdDiario into J4_join
    //                                    from J4 in J4_join.DefaultIfEmpty()

    //                                    join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
    //                                    from J5 in J5_join.DefaultIfEmpty()

    //                                    where n.i_Eliminado == 0 && n.t_Fecha >= F_Ini && n.t_Fecha <= F_Fin
    //                                    select new
    //                                    {
    //                                        CuentaMayor = n.v_NroCuenta.Substring(0, 2),
    //                                        CuentaImputable = n.v_NroCuenta,
    //                                        NombreCuenta = J1 != null ? J1.v_NombreCuenta : string.Empty,
    //                                        i_Detalle = J1 != null ? J1.i_Detalle == 1 && J1.i_Analisis == 1 ? true : false : false,
    //                                        FechaDocumento = n.t_Fecha,
    //                                        Detalle = J3.v_CodCliente + " " + n.v_IdCliente != "N002-CL000000000" ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_RazonSocial).Trim() : "PÚBLICO GENERAL",
    //                                        Documento = J2.v_Siglas + " " + n.v_NroDocumento,
    //                                        DocumentoRef = J5 != null && n.v_NroDocumentoRef != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : string.Empty,
    //                                        Analisis = n.v_Analisis == null ? !string.IsNullOrEmpty(J4.v_Nombre) ? J4.v_Nombre : string.Empty : n.v_Analisis,
    //                                        IdMoneda = J4.i_IdMoneda,
    //                                        IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
    //                                        Importe = n.d_Importe,
    //                                        ImporteCambio = n.d_Cambio,
    //                                        Key = n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
    //                                        NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
    //                                        NaturalezaRegistro = n.v_Naturaleza
    //                                    }
    //                                    ).ToList().Select(p => new ReporteAnalisisCuentas
    //                                    {
    //                                        CuentaMayor = p.CuentaMayor,
    //                                        CuentaImputable = p.CuentaImputable,
    //                                        NombreCuenta = p.NombreCuenta,
    //                                        Detalle = p.i_Detalle == true ? p.Detalle : string.Empty,
    //                                        Documento = p.i_Detalle == true ? (p.FechaDocumento.Value.ToShortDateString() + " " + p.Documento + " " + p.Analisis).Trim() : string.Empty,
    //                                        DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? p.i_Detalle == true ? (p.DocumentoRef + " " + p.Analisis).Trim() : string.Empty : string.Empty,
    //                                        FechaDocumento = p.FechaDocumento,
    //                                        ImporteSoles = p.IdMoneda == 1 ? p.Importe : p.ImporteCambio,
    //                                        ImporteDolares = p.IdMoneda == 1 ? p.ImporteCambio : p.Importe,
    //                                        DocumentoKey = p.Key,
    //                                        IdMoneda = (int)p.IdMoneda,
    //                                        IdMonedaCuenta = (int)p.IdMonedaCuenta,
    //                                        NaturalezaCuenta = p.NaturalezaCuenta,
    //                                        NaturalezaRegistro = p.NaturalezaRegistro
    //                                    }).ToList();
    //                GruposDiario = GruposDiario.Where(p => !string.IsNullOrEmpty(p.NombreCuenta)).ToList();
    //                #endregion

    //                #region Filtro por cuenta si es requerido
    //                if (!string.IsNullOrEmpty(nroCuenta))
    //                {
    //                    GruposDiario = GruposDiario.Where(p => p.CuentaImputable.StartsWith(nroCuenta)).ToList();
    //                    GruposTesoreria = GruposTesoreria.Where(p => p.CuentaImputable.StartsWith(nroCuenta)).ToList();
    //                }
    //                #endregion

    //                #region Une tesorerias con diarios en una sola entidad
    //                GruposDiario = GruposDiario.Where(n => int.Parse(n.CuentaMayor.Substring(0, 2)) >= 10 && int.Parse(n.CuentaMayor.Substring(0, 2)) <= 59).ToList();

    //                GruposTesoreria = GruposTesoreria.Where(n => int.Parse(n.CuentaMayor.Substring(0, 2)) >= 10 && int.Parse(n.CuentaMayor.Substring(0, 2)) <= 59).ToList();

    //                Grupos = GruposDiario.Concat(GruposTesoreria).ToList().OrderBy(p => p.CuentaMayor).ThenBy(x => x.CuentaImputable).ThenBy(o => o.Detalle).ThenBy(f => f.FechaDocumento).ToList();

    //                Grupos.ForEach(p => p.NombreCuentaMayor = " " + CuentasMayores.Where(o => o.v_NroCuenta == p.CuentaMayor).FirstOrDefault().v_NombreCuenta);

    //                Grupos = Grupos.GroupBy(x => new { x.DocumentoKey, x.NaturalezaRegistro }).Select(o => o.First()).ToList(); //--
    //                #endregion

    //                #region Realiza el cálculo de los saldos
    //                foreach (ReporteAnalisisCuentas RegistroDiario in GruposDiario.Where(p => string.IsNullOrEmpty(p.DocumentoRef)).ToList())
    //                {
    //                    List<ReporteAnalisisCuentas> RegistrosContraCuenta = new List<ReporteAnalisisCuentas>();

    //                    RegistrosContraCuenta = GruposTesoreria.Where(p => p.DocumentoKey == RegistroDiario.DocumentoKey && p.Detalle == RegistroDiario.Detalle).ToList();

    //                    if (RegistrosContraCuenta.Count == 0)
    //                    {
    //                        RegistrosContraCuenta = GruposDiario.Where(p => p.DocumentoKey == RegistroDiario.DocumentoKey && p.NaturalezaRegistro != RegistroDiario.NaturalezaRegistro).ToList();
    //                    }

    //                    var G = Grupos.Where(p => p.DocumentoKey == RegistroDiario.DocumentoKey && p.NaturalezaRegistro == RegistroDiario.NaturalezaRegistro && p.Detalle == RegistroDiario.Detalle).FirstOrDefault();

    //                    if (RegistrosContraCuenta.Count != 0)
    //                    {
    //                        if (G != null)
    //                        {
    //                            if (RegistroDiario.NaturalezaCuenta == 1)
    //                            {
    //                                RegistroDiario.ImporteSoles = RegistroDiario.NaturalezaRegistro == "H" ? (RegistroDiario.ImporteSoles * -1) : RegistroDiario.ImporteSoles;
    //                                RegistroDiario.ImporteDolares = RegistroDiario.NaturalezaRegistro == "H" ? (RegistroDiario.ImporteDolares * -1) : RegistroDiario.ImporteDolares;

    //                                foreach (ReporteAnalisisCuentas RegistroContraCuenta in RegistrosContraCuenta)
    //                                {
    //                                    if (RegistroContraCuenta.NaturalezaRegistro == "D")
    //                                    {
    //                                        G.ImporteSoles = RegistroDiario.ImporteSoles.Value + RegistroContraCuenta.ImporteSoles.Value;
    //                                        G.ImporteDolares = RegistroDiario.ImporteDolares.Value + RegistroContraCuenta.ImporteDolares.Value;
    //                                    }
    //                                    else
    //                                    {
    //                                        G.ImporteSoles = RegistroDiario.ImporteSoles.Value - RegistroContraCuenta.ImporteSoles.Value;
    //                                        G.ImporteDolares = RegistroDiario.ImporteDolares.Value - RegistroContraCuenta.ImporteDolares.Value;
    //                                    }
    //                                    RegistroDiario.ImporteSoles = G.ImporteSoles;
    //                                    RegistroDiario.ImporteDolares = G.ImporteDolares;
    //                                    Grupos.Remove(RegistroContraCuenta);
    //                                }
    //                            }
    //                            else
    //                            {
    //                                RegistroDiario.ImporteSoles = RegistroDiario.NaturalezaRegistro != "H" ? (RegistroDiario.ImporteSoles * -1) : RegistroDiario.ImporteSoles;
    //                                RegistroDiario.ImporteDolares = RegistroDiario.NaturalezaRegistro != "H" ? (RegistroDiario.ImporteDolares * -1) : RegistroDiario.ImporteDolares;

    //                                foreach (ReporteAnalisisCuentas RegistroContraCuenta in RegistrosContraCuenta)
    //                                {
    //                                    if (RegistroContraCuenta.NaturalezaRegistro != "D")
    //                                    {
    //                                        G.ImporteSoles = RegistroDiario.ImporteSoles.Value + RegistroContraCuenta.ImporteSoles.Value;
    //                                        G.ImporteDolares = RegistroDiario.ImporteDolares.Value + RegistroContraCuenta.ImporteDolares.Value;
    //                                    }
    //                                    else
    //                                    {
    //                                        G.ImporteSoles = RegistroDiario.ImporteSoles.Value - RegistroContraCuenta.ImporteSoles.Value;
    //                                        G.ImporteDolares = RegistroDiario.ImporteDolares.Value - RegistroContraCuenta.ImporteDolares.Value;
    //                                    }
    //                                    RegistroDiario.ImporteSoles = G.ImporteSoles;
    //                                    RegistroDiario.ImporteDolares = G.ImporteDolares;
    //                                    Grupos.Remove(RegistroContraCuenta);
    //                                }
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (!string.IsNullOrEmpty(RegistroDiario.DocumentoRef))
    //                        {
    //                            Grupos.Remove(RegistroDiario);
    //                        }
    //                    }
    //                }
    //                #endregion

    //                #region Mostrar solo los que tienen obligación pendiente
    //                Grupos = Grupos.Where(p => p.ImporteSoles.Value != 0).ToList();
    //                #endregion

    //                #region Separa y Calcula los detalles de las cuentas sin detalle ni analisis
    //                var Grupos1 = Grupos.Where(n => string.IsNullOrEmpty(n.Detalle) && string.IsNullOrEmpty(n.Documento)).GroupBy(o => o.CuentaImputable)
    //                                                                       .Select(p => new ReporteAnalisisCuentas
    //                                                                       {
    //                                                                           CuentaMayor = p.FirstOrDefault().CuentaMayor,
    //                                                                           CuentaImputable = p.FirstOrDefault().CuentaImputable,
    //                                                                           NombreCuenta = p.FirstOrDefault().NombreCuenta,
    //                                                                           NombreCuentaMayor = p.FirstOrDefault().NombreCuentaMayor,
    //                                                                           FechaDocumento = p.FirstOrDefault().FechaDocumento,
    //                                                                           IdMoneda = (int)p.FirstOrDefault().IdMonedaCuenta,
    //                                                                           ImporteSoles = CalcularImporte(p, 1),
    //                                                                           ImporteDolares = CalcularImporte(p, 2),
    //                                                                           DocumentoKey = p.FirstOrDefault().DocumentoKey,
    //                                                                           NaturalezaCuenta = p.FirstOrDefault().NaturalezaCuenta,
    //                                                                           NaturalezaRegistro = p.FirstOrDefault().NaturalezaRegistro,
    //                                                                           Detalle = string.Empty,
    //                                                                           Documento = string.Empty,
    //                                                                           DocumentoRef = string.Empty
    //                                                                       }
    //                                                                       ).ToList();
    //                #endregion

    //                #region Separa y filtra los detalles de las cuentas con detalle y analisis
    //                var Grupos2 = Grupos.Where(p => !string.IsNullOrEmpty(p.Detalle) && !string.IsNullOrEmpty(p.Documento)).ToList()
    //                                    .GroupBy(q => new { q.CuentaImputable, q.Detalle, q.Documento }).Select(e => e.First()).ToList();
    //                #endregion

    //                #region Junta los dos grupos procesados
    //                Grupos = Grupos1.Concat(Grupos2).ToList();
    //                #endregion

    //                pobjOperationResult.Success = 1;

    //                return Grupos;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            pobjOperationResult.Success = 0;
    //            pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ReporteAnalisisCuentas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
    //            pobjOperationResult.ErrorMessage = ex.Message;
    //            pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
    //            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
    //            return null;
    //        }
    //    }

    //    decimal CalcularImporte(IEnumerable<ReporteAnalisisCuentas> p, int IdMoneda)
    //    {
    //        decimal Importe = 0;

    //        foreach (var item in p.OrderBy(o => o.NaturalezaRegistro))
    //        {
    //            if (item.NaturalezaRegistro == "D")
    //            {
    //                Importe = Importe + (IdMoneda == 1 ? item.ImporteSoles.Value : item.ImporteDolares.Value);
    //            }
    //            else
    //            {
    //                Importe = Importe - (IdMoneda == 1 ? item.ImporteSoles.Value : item.ImporteDolares.Value);
    //            }
    //        }

    //        return Importe;
    //    }

    //}
}
