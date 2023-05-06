using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class planillaporcleyestrabajadorDto
    {
        public object this[string propertyName]
        {
            get
            {
                var myType = typeof(planillaporcleyestrabajadorDto);
                var myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                var myType = typeof(planillaporcleyestrabajadorDto);
                var myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);

            }
        }
    }
}
