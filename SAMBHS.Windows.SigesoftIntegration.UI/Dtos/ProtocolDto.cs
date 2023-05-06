using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class ProtocolDto
    {
        public string v_ProtocolId { get; set; }
        public string v_Name { get; set; }
        public string Geso { get; set; }
        public string TipoEso { get; set; }
        public string EmpresaCliente { get; set; }
        public string EmpresaEmpleadora { get; set; }
        public string EmpresaTrabajo { get; set; }
        public int i_EsoTypeId { get; set; }
        public string v_EmployerOrganizationId { get; set; }
        public string v_EmployerLocationId { get; set; }
        public string v_GroupOccupationId { get; set; }
        public string v_CustomerOrganizationId { get; set; }
        public string v_CustomerLocationId { get; set; }

        public string v_WorkingOrganizationId { get; set; }
        public string v_WorkingLocationId { get; set; }
        public int i_MasterServiceId { get; set; }
        public string v_CostCenter { get; set; }
        public int i_MasterServiceTypeId { get; set; }
        public int i_HasVigency { get; set; }
        public int? i_ValidInDays { get; set; }
        public int i_IsActive { get; set; }
        public string v_NombreVendedor { get; set; }
        public int i_OrganizationTypeId { get; set; }
    }

    public class ServiceComponentUpdatePrice
    {
        public string v_ServiceComponentId { get; set; }
        public string v_ServiceId { get; set; }
        public decimal r_Price { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Descuento { get; set; }
        public decimal SaldoPaciente { get; set; }
        public decimal SaldoSeguro { get; set; }
        public int PlanSeguro { get; set; }
        public decimal Deducible { get; set; }
        public decimal Coaseguro { get; set; }
    }

    public class ProtocoloDescuentosDto
    {
        public string v_ServiceId { get; set; }
        public string v_ProtocolId { get; set; }
        public decimal d_PrecioConsulta { get; set; }
        public decimal d_DescuentoLaboratorio { get; set; }
        public decimal d_DescuentoRayosX { get; set; }
        public decimal d_DescuentoEcografias { get; set; }
        public decimal d_DescuentoFarmacia { get; set; }
        public decimal d_DescuentoOdontologia { get; set; }
        public decimal d_CamaHosp { get; set; }
        public decimal d_SalaOperaciones { get; set; }
        public decimal d_PrecioAmbulancia { get; set; }
    }

    public class CategoriaExamen
    {
        public string v_ComponentId { get; set; }
        public string v_Name { get; set; }
        public int i_CategoryId { get; set; }
        public string v_Value1 { get; set; }
    }
}
