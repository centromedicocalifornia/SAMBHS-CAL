﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteRegistroVenta
    {

        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string NombreAlmacen { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string NombreCliente { get; set; }
        public string NroDocCliente { get; set; }
        public string NombreVendedor { get; set; }
        public string NroDocVendedor { get; set; }
        public string Moneda { get; set; }
        public decimal ValorVenta { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
        public string DocumentoRef { get; set; }
        public string TipoDocumentoRef { get; set; }
        public DateTime FechaRegistroRef { get; set; }
        public int IdTipoDocumento { get; set; }
        public string IdVendedor { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal ValorVentaD { get; set; }
        public decimal IgvD { get; set; }
        public decimal TotalD { get; set; }
        public int IdMoneda { get; set; }
        public string IgvNombre { get; set; }
        public string Documento { get; set; }
        public int IdEstado { get; set; }
        public int IdAlmacen { get; set; }
        public string v_IdGuiaRemision { get; set; }
        public int i_IdTipoGuia { get; set; }
        public string Establecimiento { get; set; }
        public string CondicioPago { get; set; }
        public string Grupo { get; set; }


    }


    public class CuadreCajaModeloAlternativo :ICloneable 
    {

        public string Fecha { get; set; }
        public string IdVenta { get; set; }
        public string IdCobranza { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Cliente { get; set; }
        public string NroDocCliente { get; set; }

        public string CondicionPago { get; set; }
        public int? iCondicionPago { get; set; }
        public string Vendedor { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public string GrupoLLave { get; set; }
        public string TipoDocumento { get; set; }
        public string GrupoLlave2 { get; set; }
        public decimal TotalOperacion { get; set; }
        public string NombreDocumento { get; set; }
        public string TotalDocumento { get; set; }
        public string TotalGrupoLlave { get; set; }
        public decimal MontoCobrado { get; set; }
        public string DocumentoCobranza { get; set; }
        public string FormasPago { get; set; }
        public decimal Deuda { get; set; }
        public string MonedaCobranza { get; set; }
        public string FechaPago { get; set; }
        public string EstadoCobranza { get; set; }

        public string EstadoVenta { get; set; }
        public string GrupoMoneda { get; set; }
        public int? NotaCreditoCobranzaDetalle { get; set; }

        public int? ClienteEsAgente { get; set; }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
