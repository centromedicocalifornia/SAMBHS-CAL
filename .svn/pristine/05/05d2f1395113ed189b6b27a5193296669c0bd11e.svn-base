using System;
using System.Reflection;

namespace SAMBHS.Common.BE
{
    public partial class planillaporcafpDto
    {
        public object this[string propertyName]
        {
            get
            {
                var myType = typeof(planillaporcafpDto);
                var myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                var myType = typeof(planillaporcafpDto);
                var myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);

            }
        }
    }
}
