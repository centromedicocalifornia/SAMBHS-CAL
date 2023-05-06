using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Contabilidad.BL
{
    public class CierreMensualBL
    {

        public List<cierremensualDto> ObtenerCierreMensualPeriodo(string pstrPeriodo, int pstridProceso)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            try
            {
                var CierreMensual = (from n in dbContext.cierremensual
                                      
                                     where n.v_Periodo == pstrPeriodo && n.i_IdProceso == pstridProceso && n.i_Cerrado == 1  && ( n.i_Eliminado ==0 || n.i_Eliminado ==null )

                                     select new cierremensualDto
                                     {
                                         v_Mes = n.v_Mes,
                                         v_Periodo = n.v_Periodo,
                                         i_IdProceso = n.i_IdProceso,
                                         i_Cerrado = n.i_Cerrado,
                                         v_IdCierreMensaual = n.v_IdCierreMensaual,

                                     }).ToList();

                return CierreMensual;
            }
            catch (Exception g)
            {

                throw g;
            }


        }

        public void ActualizarCierreMensual(ref OperationResult objOperationResult, List<cierremensualDto> ListaAgregarDto, List<cierremensualDto> ListaModificarDto, List<cierremensualDto> ListaEliminarDto, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                SecuentialBL objSecuentialBL = new SecuentialBL();
                int SecuentialId = 0;
                string newIdCierreMensual = string.Empty;
                string newIdPedidoDetalle = string.Empty;
                int intNodeId;
                foreach (var Fila in ListaAgregarDto)
                {

                    intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 99);
                    Fila.t_InsertaFecha = DateTime.Now;
                    Fila.i_Eliminado = 0;
                    Fila.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    newIdCierreMensual = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XX");
                    Fila.v_IdCierreMensaual = newIdCierreMensual;
                    cierremensual objEntityCierreMensual = cierremensualAssembler.ToEntity(Fila);
                    dbContext.AddTocierremensual(objEntityCierreMensual);
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "cierremensual", newIdCierreMensual);

                }

                foreach (var Fila in ListaModificarDto)
                {
                    cierremensual objEntity = (from a in dbContext.cierremensual
                                               where a.v_IdCierreMensaual == Fila.v_IdCierreMensaual
                                               select a).FirstOrDefault();

                    objEntity.t_ActualizaFecha = DateTime.Now;
                    objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity = cierremensualAssembler.ToEntity(Fila);
                    dbContext.cierremensual.ApplyCurrentValues(objEntity);
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cierremensual", objEntity.v_IdCierreMensaual);

                }


                foreach (var Fila in ListaEliminarDto)
                {

                    var CierreMensual = (from n in dbContext.cierremensual
                                         where n.v_IdCierreMensaual == Fila.v_IdCierreMensaual

                                         select n).FirstOrDefault();
                    if (CierreMensual != null)
                    {

                       // dbContext.cierremensual.DeleteObject(CierreMensual);
                        CierreMensual.i_Eliminado = 1;
                        CierreMensual.t_ActualizaFecha = DateTime.Now;
                        CierreMensual.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.cierremensual.ApplyCurrentValues(CierreMensual);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "cierremensual", CierreMensual.v_IdCierreMensaual);
                    }


                }

                dbContext.SaveChanges();
                objOperationResult.Success = 1;

                //return objCierreMensual.v_IdCierreMensaual;


            }
            catch (Exception g)
            {


                objOperationResult.Success = 0;
                throw g;
            }
        }

        public void EliminarCierreMensual(string pstrIdCierreMensual)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var CierreMensual = (from n in dbContext.cierremensual
                                 where n.v_IdCierreMensaual == pstrIdCierreMensual && ( n.i_Eliminado ==0 || n.i_Eliminado ==null )

                                 select n).FirstOrDefault();
            if (CierreMensual != null)
            {

                dbContext.cierremensual.DeleteObject(CierreMensual);
            }
            dbContext.SaveChanges();

        }
        public bool VerificarMesCerrado(string pstrPeriodo, string pstrMes, int Proceso  )//string pstrProceso)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbContext.cierremensual

                             join m in dbContext.datahierarchy on new { i_Proceso = n.i_IdProceso.Value, grupo = 93 }
                                                                equals new { i_Proceso = m.i_ItemId, grupo = m.i_GroupId } into m_join
                             from m in m_join.DefaultIfEmpty()

                             where n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                             //&& m.v_Value1 == pstrProceso 

                             && n.i_IdProceso == Proceso
                             &&  (n.i_Eliminado ==null || n.i_Eliminado ==0)

                             select n).FirstOrDefault();

                return query != null;
            }

        }


    }
}
