using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;


namespace SAMBHS.Common.BL
{
    public class UsuarioBL
    {
        public personDto DevolverUsuario(ref OperationResult pobjOperationResult, int IdUsuario)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    var Persona = (from n in dbContext.systemuser

                                   join J1 in dbContext.person on n.i_PersonId equals J1.i_PersonId into J1_join
                                   from J1 in J1_join.DefaultIfEmpty()
                                   
                                   join J2 in dbContext.systemusernode on n.i_SystemUserId equals J2.i_SystemUserId into J2_join
                                   from J2 in J2_join.DefaultIfEmpty()

                                   where n.i_SystemUserId == IdUsuario

                                   select new personDto
                                   {
                                       v_FirstName = J1.v_FirstName,
                                       v_FirstLastName = J1.v_FirstLastName,
                                       v_SecondLastName = J1.v_SecondLastName,
                                       i_DocTypeId = J1.i_DocTypeId,
                                       v_DocNumber = J1.v_DocNumber,
                                       v_BirthPlace = J1.v_BirthPlace,
                                       d_Birthdate = J1.d_Birthdate,
                                       UserName = n.v_UserName,
                                       Password = n.v_Password
                                   }
                                   ).FirstOrDefault();

                    return Persona;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<EmpresasAsignadas> DevolverEmpresasAsignadas(int IdUsuario)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    var Empresa = (from n in dbContext.systemusernode

                                   join J1 in dbContext.node on n.i_NodeId equals J1.i_NodeId into J1_join
                                   from J1 in J1_join.DefaultIfEmpty()

                                   where n.i_SystemUserId == IdUsuario && n.i_IsDeleted == 0

                                   select new EmpresasAsignadas
                                   {
                                       Ruc = J1.v_RUC,
                                       Nombre = J1.v_RazonSocial
                                   }
                                   ).ToList();

                    return Empresa;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void ActualizarPersona(ref OperationResult pobjOperationResult, personDto PersonaDto)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    var Persona = (from n in dbContext.person
                                   where n.i_PersonId == PersonaDto.i_PersonId
                                   select n).FirstOrDefault();

                    Persona.v_FirstName = PersonaDto.v_FirstName;
                    Persona.v_FirstLastName = PersonaDto.v_FirstLastName;
                    Persona.v_SecondLastName = PersonaDto.v_SecondLastName;
                    Persona.i_DocTypeId = PersonaDto.i_DocTypeId;
                    Persona.v_DocNumber = PersonaDto.v_DocNumber;
                    Persona.v_BirthPlace = PersonaDto.v_BirthPlace;
                    Persona.d_Birthdate = PersonaDto.d_Birthdate;
                    Persona.i_UpdateUserId = Persona.i_PersonId;
                    Persona.d_UpdateDate = DateTime.Now;
                    dbContext.person.ApplyCurrentValues(Persona);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return;
            }
        }

        public void ActualizarContrasena(ref OperationResult pobjOperationResult, string ContrasenaNueva, int IdUsuario)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    var Usuario = (from n in dbContext.systemuser
                                   where n.i_SystemUserId == IdUsuario
                                   select n).FirstOrDefault();

                    Usuario.v_Password = ContrasenaNueva;
                    Usuario.i_UpdateUserId = Usuario.i_SystemUserId;
                    Usuario.d_UpdateDate = DateTime.Now;

                    dbContext.systemuser.ApplyCurrentValues(Usuario);

                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return;
            }
        }
    }
}
