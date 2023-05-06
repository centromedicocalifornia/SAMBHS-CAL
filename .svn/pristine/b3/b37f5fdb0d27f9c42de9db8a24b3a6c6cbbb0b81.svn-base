using System;
using System.Collections.Generic;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq;
using SAMBHS.Common.BE;
using System.Linq.Dynamic;

namespace SAMBHS.CommonWIN.BL
{
    public class SecuentialBL
    {
        public int GetNextSecuentialId(int pintNodeId, int pintTableId, SAMBHSEntitiesModelWin objContext = null)
        {
            var dbContext = objContext ?? new SAMBHSEntitiesModelWin();
            
                string replicationId = Globals.ClientSession != null ? Globals.ClientSession.ReplicationNodeID : "N";
                secuential objSecuential = (from a in dbContext.secuential
                    where a.i_TableId == pintTableId && a.i_NodeId == pintNodeId && a.v_ReplicationID == replicationId
                    select a).SingleOrDefault();

                // Actualizar el campo con el nuevo valor a efectos de reservar el ID autogenerado para evitar colisiones entre otros nodos
                if (objSecuential != null)
                {
                    objSecuential.i_SecuentialId = objSecuential.i_SecuentialId + 1;
                }
                else
                {
                    objSecuential = new secuential
                    {
                        i_NodeId = pintNodeId,
                        i_TableId = pintTableId,
                        i_SecuentialId = 1,
                        v_ReplicationID = replicationId
                    };
                    dbContext.AddTosecuential(objSecuential);
                }

                dbContext.SaveChanges();

                return objSecuential.i_SecuentialId.Value;
            
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns>Dtos</returns>
        public IEnumerable<secuentialDto> GetAll()
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var q = dbContext.secuential.ToDTOs().OrderBy(o => o.i_TableId).ToList();
                return q;
            }
        }

        public int GetMaxFromTable(out OperationResult pobj, secuentialDto dto, string name)
        {
            pobj = new OperationResult
            {
                Success = 1
            };
            try
            {
                var prop = typeof(SAMBHSEntitiesModelWin).GetProperty(name);
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var table = prop.GetValue(dbContext, null);
                    var nameId = (string)((dynamic)table).EntitySet.ElementType.KeyMembers[0].Name;
                    var ds = ((IQueryable)table)
                        .Where(nameId + ".StartsWith(@0)", dto.v_ReplicationID + dto.i_NodeId.ToString("D3"))
                        .OrderBy(nameId + " desc")
                        .Take(1)
                        .Select(nameId);
                    var res = ds.Cast<string>().ToArray();
                    if (res.Length > 0)
                    {
                        var cor = res[0].Substring(7, 9);
                        return int.Parse(cor);
                    }
                } 
                
            }
            catch (Exception er)
            {
                pobj.Success = 0;
                pobj.ExceptionMessage = er.Message;
            }
            return 0;
        }

        public void Update(out OperationResult pobj, secuentialDto dto)
        {
            pobj = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var e = dbContext.secuential.FirstOrDefault(r => r.v_ReplicationID == dto.v_ReplicationID
                                    && r.i_NodeId == dto.i_NodeId 
                                    && r.i_TableId == dto.i_TableId);
                    if (e != null)
                    {
                        e.i_SecuentialId = dto.i_SecuentialId;
                        dbContext.SaveChanges();
                    }
                }
                pobj.Success = 1;
            }
            catch (Exception er)
            {
                pobj.ExceptionMessage = er.Message;
            }

        } 
    }
}
