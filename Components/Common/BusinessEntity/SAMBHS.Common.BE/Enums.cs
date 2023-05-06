using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class Enums
    {
        public enum TipoBusqueda
        {
            CodigoSegus = 1,
            NombreCategoria = 2,
            NombreSubCategoria = 3,
            NombreComponent = 4,
            ComponentId = 5,
        }

        public enum ActionForm
        {
            None = 0,
            Add = 1,
            Edit = 2,
            Delete = 3,
            Upload = 4,
            Browse = 5,
            Cancel = 6
        }

    }
}
