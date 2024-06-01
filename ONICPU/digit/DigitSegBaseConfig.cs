using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ONICPU.digit
{
  public class DigitSegBaseConfig : IBuildingConfig
  {
    public const string ID = "DIGITSEG";

    public virtual int GetBits()
    {
      return 8;
    }
    public virtual LocString GetPortString()
    {
      return STRINGS.BUILDINGS.PREFABS.DIGITSEG8.LOGIC_PORT;
    }

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(
        ID + GetBits(), GetBits() > 16 ? 4 : 2, 1,
        $"dit_seg_{GetBits()}bit_kanim", 10, 3f,
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
      obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonInputPort(DigitSeg.READ_PORT_ID, new CellOffset(0, 0), GetPortString(), global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONBRIDGE.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true) };
      return obj;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      go.AddOrGet<DigitSeg>().displayBits = GetBits();
    }
  }
}
