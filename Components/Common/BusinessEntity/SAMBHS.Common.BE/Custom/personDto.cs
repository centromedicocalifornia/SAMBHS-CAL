using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class personDto
    {
        public string v_employeeName { get; set; }
        public string v_Document { get; set; }
        public string  v_InsertUser { get; set; }
        public string  v_UpdateUser  { get; set; }
        public List<String> ListaEmpresas { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Int32 UserID { get; set; }
    }

    public class EmpresasAsignadas
    {
        public string Ruc { get; set; }
        public string Nombre { get; set; }
    }
}
