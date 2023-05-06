using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class planillaafecleyesempDto
    {
        public string NombreConcepto { get; set; }
        public string CodigoConcepto { get; set; }

        public object this[string propertyName]
        {
            get
            {
                var myType = typeof(planillaafecleyesempDto);
                var myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                var myType = typeof(planillaafecleyesempDto);
                var myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);

            }
        }
    }

    public partial class planillaafecleyestrabDto
    {
        public string NombreConcepto { get; set; }
        public string CodigoConcepto { get; set; }
    }
}
