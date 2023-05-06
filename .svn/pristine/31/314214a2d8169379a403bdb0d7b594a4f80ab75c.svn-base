using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class flujoefectivoconceptosDto
    {
        public IEnumerable<string> CtasRelacionadas { get; set; }
        public string CtasRelacionadasString {
            get {
                return CtasRelacionadas.Any() ? string.Join(", ", CtasRelacionadas) : string.Empty;
            }
        }
    }
}
