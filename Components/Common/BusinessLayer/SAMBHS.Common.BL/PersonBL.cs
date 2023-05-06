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
    class PersonBL
    {


        public List<personDto> GetCustomerPagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from n in dbContext.person
                             //join p in dbContext.person on n.v_CustomerId equals p.i_PersonId

                             join su2 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertUserId.Value }
                                                           equals new { i_InsertUserId = su2.i_SystemUserId } into su2_join
                             from su2 in su2_join.DefaultIfEmpty()

                             join su3 in dbContext.systemuser on new { i_UpdateUserId = n.i_UpdateUserId.Value }
                                                           equals new { i_UpdateUserId = su3.i_SystemUserId } into su3_join
                             from su3 in su3_join.DefaultIfEmpty()
                             where n.i_IsDeleted == 0
                             select new personDto
                             {
                                 i_PersonId = n.i_PersonId,
                                 v_employeeName = n.v_FirstName + " " + n.v_FirstLastName + " " + n.v_SecondLastName,
                                 v_DocNumber = n.v_DocNumber,
                                 v_Mail = n.v_Mail,
                                 v_TelephoneNumber = n.v_TelephoneNumber,
                                 d_InsertDate = n.d_InsertDate,
                                 d_UpdateDate = n.d_UpdateDate
                                 //v_InsertUser = su2.v_UserName,
                                 //v_UpdateUser = su3.v_UserName

                             }
                            );



                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
                {
                    int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                    query = query.Skip(intStartRowIndex);
                }
                if (pintResultsPerPage.HasValue)
                {
                    query = query.Take(pintResultsPerPage.Value);
                }

                List<personDto> objData = query.ToList();
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

    }
}
