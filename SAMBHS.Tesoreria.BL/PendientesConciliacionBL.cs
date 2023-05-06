using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using System.Linq.Dynamic;

namespace SAMBHS.Tesoreria.BL
{



    public class PendientesConciliacionBL
    {

      public List<pendientesconciliacionDto> ObtenerListadoPendientesConciliacion(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression,DateTime Fecha)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.pendientesconciliacion

                                join B in dbContext.documento on new { TipoDoc = A.i_IdTipoDoc.Value, eliminado = 0 } equals new { TipoDoc = B.i_CodigoDocumento, eliminado = B.i_Eliminado.Value } into B_join

                                from B in B_join.DefaultIfEmpty()

                                where A.i_Eliminado == 0    //|| A.i_IdTipoDoc ==0

                                //&& A.t_Fecha == Fecha
                                select new pendientesconciliacionDto
                                {

                                    v_NroCuenta = A.v_NroCuenta,
                                    v_Mes = A.v_Mes,
                                    v_IdPendientesConciliacion = A.v_IdPendientesConciliacion,
                                    v_Anio = A.v_Anio,
                                    d_Importe = A.d_Importe,
                                    i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                    i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                    t_ActualizaFecha = A.t_ActualizaFecha,
                                    t_InsertaFecha = A.t_InsertaFecha,
                                    v_Naturaleza = A.v_Naturaleza,
                                    v_Glosa = A.v_Glosa,
                                    v_NumeroDoc = A.v_NumeroDoc,
                                    v_TipoDocumento = B.v_Siglas,
                                    t_Fecha = A.t_Fecha,

                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<pendientesconciliacionDto> objData = query.ToList();
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
      public string ObtenerNombreCuenta(string pstrNumeroCuenta)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var AsientoContable = (from n in dbContext.asientocontable

                where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.i_Imputable == 1
                select new { n.v_NombreCuenta }).FirstOrDefault();

            if (AsientoContable != null)
            {
                return AsientoContable.v_NombreCuenta;

            }
            else
            {
                return string.Empty;
            }
        }
      public string InsertarPendientesConciliacion(ref OperationResult pobjOperationResult, pendientesconciliacionDto  pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();

                    pendientesconciliacion objEntity = pendientesconciliacionAssembler.ToEntity(pobjDtoEntity);

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);

                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 64); //Cambiar
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "YB"); // Cambiarr
                    objEntity.v_IdPendientesConciliacion = newId;

                    dbContext.AddTopendientesconciliacion(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    return newId; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return string.Empty;
            }
        }
      public string  ActualizarPendientesConciliacion(ref OperationResult pobjOperationResult, pendientesconciliacionDto  pobjDtoEntity, List<string> ClientSession)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.pendientesconciliacion 
                                       where a.v_IdPendientesConciliacion == pobjDtoEntity.v_IdPendientesConciliacion 
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha  = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                pendientesconciliacion  objEntity = pendientesconciliacionAssembler.ToEntity(pobjDtoEntity);
                dbContext.pendientesconciliacion.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return pobjDtoEntity.v_IdPendientesConciliacion  ;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return string.Empty ;
            }
        }
      public void EliminarPendientesConciliacion(ref OperationResult pobjOperationResult, string pstrIdPendientesConciliacion, List<string> ClientSession)
      {
          try
          {
              SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

              // Obtener la entidad fuente
              var objEntitySource = (from a in dbContext.pendientesconciliacion 
                                     where a.v_IdPendientesConciliacion  == pstrIdPendientesConciliacion
                                     select a).FirstOrDefault();

              // Crear la entidad con los datos actualizados
              objEntitySource.t_ActualizaFecha = DateTime.Now;
              objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
              objEntitySource.i_Eliminado = 1;


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
      public pendientesconciliacionDto ObtenerPendienteConciliacion(ref OperationResult pobjOperationResult, string pstrIdPendienteConciliacion)
      {
          try
          {
              SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
              pendientesconciliacionDto objDtoEntity = null;
              var objEntity = (from A in dbContext.pendientesconciliacion 
                               where A.v_IdPendientesConciliacion  == pstrIdPendienteConciliacion
                               select A
                               ).FirstOrDefault();
              if (objEntity != null)
                  objDtoEntity = pendientesconciliacionAssembler.ToDTO(objEntity);

              pobjOperationResult.Success = 1;
              return objDtoEntity;
          }
          catch (Exception ex)
          {
              pobjOperationResult.Success = 0;
              pobjOperationResult.ExceptionMessage = ex.Message;
              return null;
          }
      }
      public void GenerarPendientesAnioAnterior(ref  OperationResult pobjOperationResult, string pstrAnio)
      {
         
         
          pendientesconciliacionDto _objPendientesConciliacionDto = new pendientesconciliacionDto();
          //OperationResult objOperationResult= new OperationResult ();
          string mesAnterior = ((int)Mes.Diciembre).ToString();
          string anioAnteior = (int.Parse(pstrAnio) - 1).ToString();
          try
          {
          SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
          var MovimientoEstadoBancario = (from n in dbContext.movimientoestadobancario

                                          where n.i_Eliminado == 0 && n.i_Mes == 0 && n.v_NroCuenta.StartsWith("104") && n.v_Mes == mesAnterior && n.v_Anio  == anioAnteior

                                          select n).ToList();

          foreach (var objMovimientoestaoBancarioDto in MovimientoEstadoBancario)
          {
              _objPendientesConciliacionDto = new pendientesconciliacionDto();
              _objPendientesConciliacionDto.v_NroCuenta = objMovimientoestaoBancarioDto.v_NroCuenta.Trim ();
              _objPendientesConciliacionDto.v_Anio = objMovimientoestaoBancarioDto.v_Anio.Trim();
              _objPendientesConciliacionDto.v_Mes = objMovimientoestaoBancarioDto.v_Mes.Trim();
              _objPendientesConciliacionDto.t_Fecha = objMovimientoestaoBancarioDto.t_Fecha.Value;
              _objPendientesConciliacionDto.v_Naturaleza = objMovimientoestaoBancarioDto.d_Cargo == 0 ? "H" : "D";
              _objPendientesConciliacionDto.d_Importe = objMovimientoestaoBancarioDto.d_Abono ==0? objMovimientoestaoBancarioDto.d_Cargo.Value :objMovimientoestaoBancarioDto.d_Abono.Value ;
              _objPendientesConciliacionDto.i_IdTipoDoc = objMovimientoestaoBancarioDto.i_IdTipoDocumento.Value; 
              _objPendientesConciliacionDto.v_NumeroDoc = objMovimientoestaoBancarioDto.v_NumeroDocumento ;
              _objPendientesConciliacionDto.v_Glosa = string.Empty ;

              InsertarPendientesConciliacion(ref pobjOperationResult, _objPendientesConciliacionDto, Globals.ClientSession.GetAsList());
            
          }
           pobjOperationResult.Success =1 ;
          }
          catch (Exception e)
          {
           pobjOperationResult.Success = 0;
           throw e ;
          
          }
      
      }
      public void EliminarPendientesAnioAnterior(string pstrAnio)
      {
          OperationResult objOperationResult= new OperationResult ();
          string mesAnterior = ((int)Mes.Diciembre).ToString();
          string anioAnterior=(int.Parse(pstrAnio) - 1).ToString();
          SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

          var PendienteAnioAnterior = (from n in dbContext.pendientesconciliacion

                                       where n.v_Mes == mesAnterior && n.i_Eliminado == 0 && n.v_Anio == anioAnterior && n.v_NroCuenta.StartsWith("104")

                                      select n).ToList();

          if (PendienteAnioAnterior.Count() != 0)
            {
                foreach (var pendienteDto in PendienteAnioAnterior)
                {
                   // dbContext.pendientesconciliacion.DeleteObject(pendienteDto);
                   EliminarPendientesConciliacion(ref objOperationResult,pendienteDto.v_IdPendientesConciliacion , Globals.ClientSession.GetAsList());
                        
                }
                dbContext.SaveChanges();
            }
      
      }
    
    }
}
