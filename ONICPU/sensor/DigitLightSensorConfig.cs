using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DigitLightSensorConfig : IBuildingConfig
{
	public static string ID = "DigitLightSensor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "logiclightsensor_kanim", 30, 30f, new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0]
		}, new string[2] { "RefinedMetal", "Transparent" }, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.AlwaysOperational = true;
		obj.LogicOutputPorts = new List<LogicPorts.Port>();
		obj.LogicOutputPorts.Add(LogicPorts.Port.RibbonOutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), ONICPU.STRINGS.BUILDINGS.PREFABS.DIGITLIGHTSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICLIGHTSENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICLIGHTSENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true));
		SoundEventVolumeCache.instance.AddVolume("logiclightsensor_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("logiclightsensor_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<DigitLightSensor>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
