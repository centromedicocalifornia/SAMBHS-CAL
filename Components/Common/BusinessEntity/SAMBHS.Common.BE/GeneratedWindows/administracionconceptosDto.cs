//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:06:52
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SAMBHS.Common.BE
{
    [DataContract()]
    public partial class administracionconceptosDto
    {
        [DataMember()]
        public String v_IdAdministracionConceptos { get; set; }

        [DataMember()]
        public String v_Codigo { get; set; }

        [DataMember()]
        public String v_Nombre { get; set; }

        [DataMember()]
        public String v_CuentaPVenta { get; set; }

        [DataMember()]
        public String v_CuentaIGV { get; set; }

        [DataMember()]
        public String v_CuentaDetraccion { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Eliminado { get; set; }

        [DataMember()]
        public Nullable<Int32> i_InsertaIdUsuario { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_InsertaFecha { get; set; }

        [DataMember()]
        public Nullable<Int32> i_ActualizaIdUsuario { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_ActualizaFecha { get; set; }

        [DataMember()]
        public String v_Periodo { get; set; }

        public administracionconceptosDto()
        {
        }

        public administracionconceptosDto(String v_IdAdministracionConceptos, String v_Codigo, String v_Nombre, String v_CuentaPVenta, String v_CuentaIGV, String v_CuentaDetraccion, Nullable<Int32> i_Eliminado, Nullable<Int32> i_InsertaIdUsuario, Nullable<DateTime> t_InsertaFecha, Nullable<Int32> i_ActualizaIdUsuario, Nullable<DateTime> t_ActualizaFecha, String v_Periodo)
        {
			this.v_IdAdministracionConceptos = v_IdAdministracionConceptos;
			this.v_Codigo = v_Codigo;
			this.v_Nombre = v_Nombre;
			this.v_CuentaPVenta = v_CuentaPVenta;
			this.v_CuentaIGV = v_CuentaIGV;
			this.v_CuentaDetraccion = v_CuentaDetraccion;
			this.i_Eliminado = i_Eliminado;
			this.i_InsertaIdUsuario = i_InsertaIdUsuario;
			this.t_InsertaFecha = t_InsertaFecha;
			this.i_ActualizaIdUsuario = i_ActualizaIdUsuario;
			this.t_ActualizaFecha = t_ActualizaFecha;
			this.v_Periodo = v_Periodo;
        }
    }
}