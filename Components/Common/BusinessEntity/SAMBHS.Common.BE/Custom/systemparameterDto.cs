using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
  public partial class systemparameterDto
    {
        public string v_ParentGroupName { get; set; }
        public string v_ParentParameterName { get; set; }
        public int i_ParentGroupId { get; set; }
        public string v_CreationUser { get; set; }
        public string v_UpdateUser { get; set; }
        public DateTime? d_CreationDate { get; set; }

    }
  [Serializable]
  public class DataForTreeView
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
  public class DataForTreeViewSP
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

  public class DataTreeViewForGridViewSP
  {
      public int i_ParameterId { get; set; }
      public int i_ParentItemId { get; set; }
      public string v_Value1 { get; set; }
      public string v_Value2 { get; set; }
      public int Level { get; set; }
      public int i_GroupId { get; set; }
      public override string ToString()
      {
          return String.Format("i_ParameterId={0} / i_ParentItemId={1} / v_Value1={2} / i_GroupId={3} / Level={4} ", i_ParameterId, i_ParentItemId, v_Value1, i_GroupId, Level);
      }
  }
}
