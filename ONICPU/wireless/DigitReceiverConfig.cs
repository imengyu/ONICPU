using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ONICPU.wireless
{
  public class DigitReceiverConfig : IBuildingConfig
  {
    public const string ID = "DIGITRECEIVER";

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(
        ID, 1, 1,
        $"digit_receiver_kanim", 10, 3f,
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
      obj.EnergyConsumptionWhenActive = 10f;
      obj.Floodable = false;
      obj.Overheatable = false;
      obj.Entombable = false;
      obj.AudioCategory = "Metal";
      obj.AudioSize = "small";
      obj.BaseTimeUntilRepair = -1f;
      obj.PermittedRotations = PermittedRotations.Unrotatable;
      obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(DigitReceiver.WRITE_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.DIGITRECEIVER.LOGIC_PORT, "", "", show_wire_missing_icon: true) };
      obj.LogicInputPorts = new List<LogicPorts.Port>();
      return obj;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      go.AddOrGet<DigitReceiver>().manuallyControlled = true;
    }
  }
}
