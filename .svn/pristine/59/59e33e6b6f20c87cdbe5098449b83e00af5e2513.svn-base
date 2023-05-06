using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.Resource
{
    public class ParametrosEmpresa
    {
        public int i_IdMoneda
        {
            get { return int.Parse(_objData[0]); }
            set { _objData[0] = value.ToString(); }
        }

        public int i_IdEstablecimiento
        {
            get { return int.Parse(_objData[1]); }
            set { _objData[1] = value.ToString(); }
        }

        public int i_AfectoIgvCompras
        {
            get { return int.Parse(_objData[2]); }
            set { _objData[2] = value.ToString(); }
        }

        public string i_PrecioIncluyeIgvCompras
        {
            get { return _objData[3]; }
            set { _objData[3] = value; }
        }

        public string i_AfectoIgvVentas
        {
            get { return _objData[4]; }
            set { _objData[4] = value; }
        }

        public string i_PrecioIncluyeIgvVentas
        {
            get { return _objData[5]; }
            set { _objData[5] = value; }
        }

        public int? i_IdDestinoCompras
        {
            get { return int.Parse(_objData[6]); }
            set { _objData[6] = value.ToString(); }
        }

        public int i_CantidadDecimales
        {
            get { return int.Parse(_objData[7]); }
            set { _objData[7] = value.ToString(); }
        }

        public string i_PrecioDecimales
        {
            get { return _objData[4]; }
            set { _objData[4] = value; }
        }

        public string d_ValorMaximoBoletas
        {
            get { return _objData[5]; }
            set { _objData[5] = value; }
        }

        public int? v_IdCuentaContableSoles
        {
            get { return int.Parse(_objData[6]); }
            set { _objData[6] = value.ToString(); }
        }

        public int v_IdCuentaContableDolares
        {
            get { return int.Parse(_objData[7]); }
            set { _objData[7] = value.ToString(); }
        }

        public string i_IdPermiteStockNegativo
        {
            get { return _objData[8]; }
            set { _objData[8] = value; }
        }

        private List<string> _objData;

        public ParametrosEmpresa()
        {
            _objData = new List<string>(13);

            for (int i = 0; i < 9; i++)
            {
                _objData.Add(null);
            }
        }

        public List<string> GetAsList()
        {
            return _objData;
        }

        public string[] GetAsArray()
        {
            return _objData.ToArray();
        }
    }
}
