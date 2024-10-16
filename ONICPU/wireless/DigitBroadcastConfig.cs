using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ONICPU.wireless
{
  public class DigitBroadcastConfig : IBuildingConfig
  {
    public const string ID = "DIGITBROADCAST";

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(
        ID, 1, 1,
        $"digit_broadcast_kanim", 10, 3f,
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
      obj.RequiresPowerInput = true;
      obj.PowerInputOffset = new CellOffset(0, 0);
      obj.EnergyConsumptionWhenActive = 100f;
      obj.Floodable = false;
      obj.Overheatable = false;
      obj.Entombable = false;
      obj.AudioCategory = "Metal";
      obj.AudioSize = "small";
      obj.BaseTimeUntilRepair = -1f;
      obj.PermittedRotations = PermittedRotations.Unrotatable;
      obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonInputPort(DigitBroadcast.READ_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.DIGITBROADCAST.LOGIC_PORT, "", "", show_wire_missing_icon: true) };
      return obj;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      go.AddOrGet<DigitBroadcast>().manuallyControlled = true;
    }
  }
}
