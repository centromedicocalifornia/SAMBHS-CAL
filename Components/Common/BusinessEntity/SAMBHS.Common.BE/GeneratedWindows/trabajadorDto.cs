//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:09:10
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
    public partial class trabajadorDto
    {
        [DataMember()]
        public String v_IdTrabajador { get; set; }

        [DataMember()]
        public String v_IdCliente { get; set; }

        [DataMember()]
        public String v_CodInterno { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdEstadoCivil { get; set; }

        [DataMember()]
        public String v_NroRuc { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdPaisNac { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdDepartamentoNac { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdProvinciaNac { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdDistritoNac { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoVia { get; set; }

        [DataMember()]
        public String v_NombreVia { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoZona { get; set; }

        [DataMember()]
        public String v_NombreZona { get; set; }

        [DataMember()]
        public String v_NumeroDomicilio { get; set; }

        [DataMember()]
        public String v_DepartamentoDomicilio { get; set; }

        [DataMember()]
        public String v_InteriorDomicilio { get; set; }

        [DataMember()]
        public String v_KilometroDomicilio { get; set; }

        [DataMember()]
        public String v_ManzanaDomicilio { get; set; }

        [DataMember()]
        public String v_LoteDomicilio { get; set; }

        [DataMember()]
        public String v_BloqueDomicilio { get; set; }

        [DataMember()]
        public String v_Referencia { get; set; }

        [DataMember()]
        public Byte[] b_Foto { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Domiciliado { get; set; }

        [DataMember()]
        public Nullable<Int32> i_TieneOtrosIngresos5taCat { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Renta5taCatExonerada { get; set; }

        [DataMember()]
        public Nullable<Int32> i_RegimenLaboral { get; set; }

        [DataMember()]
        public Nullable<Int32> i_TipoTrabajador { get; set; }

        [DataMember()]
        public Nullable<Int32> i_NivelEducativo { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_FechaAlta { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_FechaCese { get; set; }

        [DataMember()]
        public Nullable<Int32> i_CategoriaOcupacional { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Ocupacion { get; set; }

        [DataMember()]
        public Nullable<Int32> i_SujetoAcumulativo { get; set; }

        [DataMember()]
        public Nullable<Int32> i_SujetoTrabajoMaximo { get; set; }

        [DataMember()]
        public Nullable<Int32> i_SujetoHorarioNoct { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Sindicalizado { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Situacion { get; set; }

        [DataMember()]
        public Nullable<Int32> i_TipoPago { get; set; }

        [DataMember()]
        public Nullable<Int32> i_PeriodoPago { get; set; }

        [DataMember()]
        public Nullable<Int32> i_SituacionEspecial { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoPensionista { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoModalidadFormativa { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdCentroFormacion { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdSituacionEducativa { get; set; }

        [DataMember()]
        public Nullable<Int32> i_EntidadFinCts { get; set; }

        [DataMember()]
        public String v_NroCuentaCts { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdMonedaCts { get; set; }

        [DataMember()]
        public Nullable<Int32> i_EntidadCuentaAbono { get; set; }

        [DataMember()]
        public String v_NroCuentaAbono { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoCuentaAbono { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdMonedaCuentaAbono { get; set; }

        [DataMember()]
        public Nullable<Int32> i_AportaEsSaludVida { get; set; }

        [DataMember()]
        public Nullable<Int32> i_AportaEsSaludSctr { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdAportaEsSaludSctr { get; set; }

        [DataMember()]
        public Nullable<Int32> i_AportaAseguraPension { get; set; }

        [DataMember()]
        public Nullable<Int32> i_AportaPensionSctr { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdAportaPensionSctr { get; set; }

        [DataMember()]
        public Nullable<Int32> i_TieneDiscapacidad { get; set; }

        [DataMember()]
        public Nullable<Int32> i_AplicaConvenioDobleI { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdRegimenSalud { get; set; }

        [DataMember()]
        public String v_NroEsSalud { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdEpsServicioPropio { get; set; }

        [DataMember()]
        public String v_NroEps { get; set; }

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
        public Byte[] b_HojaVida { get; set; }

        [DataMember()]
        public List<areaslaboratrabajadorDto> areaslaboratrabajador { get; set; }

        [DataMember()]
        public List<derechohabientetrabajadorDto> derechohabientetrabajador { get; set; }

        [DataMember()]
        public List<contratotrabajadorDto> contratotrabajador { get; set; }

        [DataMember()]
        public clienteDto cliente { get; set; }

        [DataMember()]
        public List<planillacalculoDto> planillacalculo { get; set; }

        [DataMember()]
        public List<planillavariablestrabajadorDto> planillavariablestrabajador { get; set; }

        [DataMember()]
        public List<regimenpensionariotrabajadorDto> regimenpensionariotrabajador { get; set; }

        public trabajadorDto()
        {
        }

        public trabajadorDto(String v_IdTrabajador, String v_IdCliente, String v_CodInterno, Nullable<Int32> i_IdEstadoCivil, String v_NroRuc, Nullable<Int32> i_IdPaisNac, Nullable<Int32> i_IdDepartamentoNac, Nullable<Int32> i_IdProvinciaNac, Nullable<Int32> i_IdDistritoNac, Nullable<Int32> i_IdTipoVia, String v_NombreVia, Nullable<Int32> i_IdTipoZona, String v_NombreZona, String v_NumeroDomicilio, String v_DepartamentoDomicilio, String v_InteriorDomicilio, String v_KilometroDomicilio, String v_ManzanaDomicilio, String v_LoteDomicilio, String v_BloqueDomicilio, String v_Referencia, Byte[] b_Foto, Nullable<Int32> i_Domiciliado, Nullable<Int32> i_TieneOtrosIngresos5taCat, Nullable<Int32> i_Renta5taCatExonerada, Nullable<Int32> i_RegimenLaboral, Nullable<Int32> i_TipoTrabajador, Nullable<Int32> i_NivelEducativo, Nullable<DateTime> t_FechaAlta, Nullable<DateTime> t_FechaCese, Nullable<Int32> i_CategoriaOcupacional, Nullable<Int32> i_Ocupacion, Nullable<Int32> i_SujetoAcumulativo, Nullable<Int32> i_SujetoTrabajoMaximo, Nullable<Int32> i_SujetoHorarioNoct, Nullable<Int32> i_Sindicalizado, Nullable<Int32> i_Situacion, Nullable<Int32> i_TipoPago, Nullable<Int32> i_PeriodoPago, Nullable<Int32> i_SituacionEspecial, Nullable<Int32> i_IdTipoPensionista, Nullable<Int32> i_IdTipoModalidadFormativa, Nullable<Int32> i_IdCentroFormacion, Nullable<Int32> i_IdSituacionEducativa, Nullable<Int32> i_EntidadFinCts, String v_NroCuentaCts, Nullable<Int32> i_IdMonedaCts, Nullable<Int32> i_EntidadCuentaAbono, String v_NroCuentaAbono, Nullable<Int32> i_IdTipoCuentaAbono, Nullable<Int32> i_IdMonedaCuentaAbono, Nullable<Int32> i_AportaEsSaludVida, Nullable<Int32> i_AportaEsSaludSctr, Nullable<Int32> i_IdAportaEsSaludSctr, Nullable<Int32> i_AportaAseguraPension, Nullable<Int32> i_AportaPensionSctr, Nullable<Int32> i_IdAportaPensionSctr, Nullable<Int32> i_TieneDiscapacidad, Nullable<Int32> i_AplicaConvenioDobleI, Nullable<Int32> i_IdRegimenSalud, String v_NroEsSalud, Nullable<Int32> i_IdEpsServicioPropio, String v_NroEps, Nullable<Int32> i_Eliminado, Nullable<Int32> i_InsertaIdUsuario, Nullable<DateTime> t_InsertaFecha, Nullable<Int32> i_ActualizaIdUsuario, Nullable<DateTime> t_ActualizaFecha, Byte[] b_HojaVida, List<areaslaboratrabajadorDto> areaslaboratrabajador, List<derechohabientetrabajadorDto> derechohabientetrabajador, List<contratotrabajadorDto> contratotrabajador, clienteDto cliente, List<planillacalculoDto> planillacalculo, List<planillavariablestrabajadorDto> planillavariablestrabajador, List<regimenpensionariotrabajadorDto> regimenpensionariotrabajador)
        {
			this.v_IdTrabajador = v_IdTrabajador;
			this.v_IdCliente = v_IdCliente;
			this.v_CodInterno = v_CodInterno;
			this.i_IdEstadoCivil = i_IdEstadoCivil;
			this.v_NroRuc = v_NroRuc;
			this.i_IdPaisNac = i_IdPaisNac;
			this.i_IdDepartamentoNac = i_IdDepartamentoNac;
			this.i_IdProvinciaNac = i_IdProvinciaNac;
			this.i_IdDistritoNac = i_IdDistritoNac;
			this.i_IdTipoVia = i_IdTipoVia;
			this.v_NombreVia = v_NombreVia;
			this.i_IdTipoZona = i_IdTipoZona;
			this.v_NombreZona = v_NombreZona;
			this.v_NumeroDomicilio = v_NumeroDomicilio;
			this.v_DepartamentoDomicilio = v_DepartamentoDomicilio;
			this.v_InteriorDomicilio = v_InteriorDomicilio;
			this.v_KilometroDomicilio = v_KilometroDomicilio;
			this.v_ManzanaDomicilio = v_ManzanaDomicilio;
			this.v_LoteDomicilio = v_LoteDomicilio;
			this.v_BloqueDomicilio = v_BloqueDomicilio;
			this.v_Referencia = v_Referencia;
			this.b_Foto = b_Foto;
			this.i_Domiciliado = i_Domiciliado;
			this.i_TieneOtrosIngresos5taCat = i_TieneOtrosIngresos5taCat;
			this.i_Renta5taCatExonerada = i_Renta5taCatExonerada;
			this.i_RegimenLaboral = i_RegimenLaboral;
			this.i_TipoTrabajador = i_TipoTrabajador;
			this.i_NivelEducativo = i_NivelEducativo;
			this.t_FechaAlta = t_FechaAlta;
			this.t_FechaCese = t_FechaCese;
			this.i_CategoriaOcupacional = i_CategoriaOcupacional;
			this.i_Ocupacion = i_Ocupacion;
			this.i_SujetoAcumulativo = i_SujetoAcumulativo;
			this.i_SujetoTrabajoMaximo = i_SujetoTrabajoMaximo;
			this.i_SujetoHorarioNoct = i_SujetoHorarioNoct;
			this.i_Sindicalizado = i_Sindicalizado;
			this.i_Situacion = i_Situacion;
			this.i_TipoPago = i_TipoPago;
			this.i_PeriodoPago = i_PeriodoPago;
			this.i_SituacionEspecial = i_SituacionEspecial;
			this.i_IdTipoPensionista = i_IdTipoPensionista;
			this.i_IdTipoModalidadFormativa = i_IdTipoModalidadFormativa;
			this.i_IdCentroFormacion = i_IdCentroFormacion;
			this.i_IdSituacionEducativa = i_IdSituacionEducativa;
			this.i_EntidadFinCts = i_EntidadFinCts;
			this.v_NroCuentaCts = v_NroCuentaCts;
			this.i_IdMonedaCts = i_IdMonedaCts;
			this.i_EntidadCuentaAbono = i_EntidadCuentaAbono;
			this.v_NroCuentaAbono = v_NroCuentaAbono;
			this.i_IdTipoCuentaAbono = i_IdTipoCuentaAbono;
			this.i_IdMonedaCuentaAbono = i_IdMonedaCuentaAbono;
			this.i_AportaEsSaludVida = i_AportaEsSaludVida;
			this.i_AportaEsSaludSctr = i_AportaEsSaludSctr;
			this.i_IdAportaEsSaludSctr = i_IdAportaEsSaludSctr;
			this.i_AportaAseguraPension = i_AportaAseguraPension;
			this.i_AportaPensionSctr = i_AportaPensionSctr;
			this.i_IdAportaPensionSctr = i_IdAportaPensionSctr;
			this.i_TieneDiscapacidad = i_TieneDiscapacidad;
			this.i_AplicaConvenioDobleI = i_AplicaConvenioDobleI;
			this.i_IdRegimenSalud = i_IdRegimenSalud;
			this.v_NroEsSalud = v_NroEsSalud;
			this.i_IdEpsServicioPropio = i_IdEpsServicioPropio;
			this.v_NroEps = v_NroEps;
			this.i_Eliminado = i_Eliminado;
			this.i_InsertaIdUsuario = i_InsertaIdUsuario;
			this.t_InsertaFecha = t_InsertaFecha;
			this.i_ActualizaIdUsuario = i_ActualizaIdUsuario;
			this.t_ActualizaFecha = t_ActualizaFecha;
			this.b_HojaVida = b_HojaVida;
			this.areaslaboratrabajador = areaslaboratrabajador;
			this.derechohabientetrabajador = derechohabientetrabajador;
			this.contratotrabajador = contratotrabajador;
			this.cliente = cliente;
			this.planillacalculo = planillacalculo;
			this.planillavariablestrabajador = planillavariablestrabajador;
			this.regimenpensionariotrabajador = regimenpensionariotrabajador;
        }
    }
}