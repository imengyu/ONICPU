using System.Collections.Generic;
using ONICPU.sensor;
using ONICPU;
using TUNING;
using UnityEngine;

public class DigitPressureSensorGasConfig : IBuildingConfig
{
	public static string ID = "DigitPressureSensorGas";

	public override BuildingDef CreateBuildingDef()
	{
    BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchgaspressure_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
    obj.Overheatable = false;
    obj.Floodable = false;
    obj.Entombable = false;
    obj.ViewMode = OverlayModes.Logic.ID;
    obj.AudioCategory = "Metal";
    obj.SceneLayer = Grid.SceneLayer.Building;
    obj.AlwaysOperational = true;
    obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), ONICPU.STRINGS.BUILDINGS.PREFABS.DIGITPRESSURESENSORGAS.LOGIC_PORT, "", "", show_wire_missing_icon: true) };
    SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
    GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
    return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		var digitPressureSensor = go.AddOrGet<DigitPressureSensor>();
    digitPressureSensor.desiredState = Element.State.Gas;
    go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
