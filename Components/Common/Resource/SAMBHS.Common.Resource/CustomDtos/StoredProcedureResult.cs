using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace SAMBHS.Common.Resource
{
    //[DataContract]
    public class StoredProcedureResultDto
    {
        public int? Valor_Retorno { get; set; }
        public int? ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }

}