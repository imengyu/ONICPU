using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ONICPU.digit
{
  public class DigitConstantConfig : IBuildingConfig
  {
    public const string ID = "DIGITCONST";

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(
        ID, 1, 1,
        $"dit_const_kanim", 10, 3f,
        BUILDINGS.CONSTRUCTION_MASS_KG.TIER0,
        MATERIALS.REFINED_METALS, 1600f,
        BuildLocationRule.Anywhere,
        noise: NOISE_POLLUTION.NONE,
        decor: BUILDINGS.DECOR.PENALTY.TIER0
      );
      obj.ViewMode = OverlayModes.Logic.ID;
      obj.ObjectLayer = ObjectLayer.LogicGate;
      obj.SceneLayer = Grid.SceneLayer.Building;
      obj.ThermalConductivity = 0.05f;
      obj.Floodable = false;
      obj.Overheatable = false;
      obj.Entombable = false;
      obj.AudioCategory = "Metal";
      obj.AudioSize = "small";
      obj.BaseTimeUntilRepair = -1f;
      obj.PermittedRotations = PermittedRotations.R360;
      obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.DIGITCONST.LOGIC_PORT, "", "", show_wire_missing_icon: true) };
      return obj;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      go.AddOrGet<DigitConstant>().manuallyControlled = true;
    }
  }
}
