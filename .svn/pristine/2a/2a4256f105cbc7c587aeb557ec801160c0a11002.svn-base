using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class applicationhierarchyDto
    {
        
    }

    public class ConsultaApplictionhierarchyDto
    {
        public string Descripcion { get; set; }
        public int Id { get; set; }
        public string Form { get; set; }
        public List<ConsultaApplictionhierarchyDto> Hijo { get; set; }
    }

    public class DtvAppHierarchy
    {

        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Description { get; set; }

        public string Description2 { get; set; }

        public int Level { get; set; }

        public bool EnabledSelect { get; set; }


        public override string ToString()
        {
            return String.Format("Id={0} / ParentId={1} / Description={2} / Description2={3}/ Level={4}", Id, ParentId, Description, Description2, Level, true);
        }
    }


    public class DtvForGrwAppHierarchy
    {

        public int i_ApplicationHierarchyId { get; set; }

        public int i_ParentItemId { get; set; }

        public string v_Value1 { get; set; }

        public int Level { get; set; }

        public int i_GroupId { get; set; }

        public string v_ApplicationHierarchyTypeName { get; set; }

        public string v_Form { get; set; }

        public string v_Code { get; set; }

        public int i_ParentId { get; set; }

        public int i_ScopeId { get; set; }

        public string v_ScopeName { get; set; }

        public int i_BusinessRuleId { get; set; }

        public string v_BusinessRuleName { get; set; }

        public int i_IsDeleted { get; set; }

        public int i_InsertUserId { get; set; }

        public DateTime d_InsertDate { get; set; }

        public int i_UpdateUserId { get; set; }

        public DateTime d_UpdateDate { get; set; }

        public override string ToString()
        {
            return String.Format("i_ApplicationHierarchyId={0} / i_ParentItemId={1} / v_Value1={2} / i_GroupId={3} / Level={4}  / v_Form={5} / v_Code={6} / v_ScopeName={7} / v_ApplicationHierarchyTypeName={8}  / v_BusinessRuleName={9}",
    i_ApplicationHierarchyId, i_ParentItemId, v_Value1, i_GroupId, Level, v_Form, v_Code, v_ScopeName, v_ApplicationHierarchyTypeName, v_BusinessRuleName);
        }

    }
}
