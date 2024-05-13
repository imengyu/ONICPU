using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DigitBatteryConfig : BaseBatteryConfig
{
	public const string ID = "DigitBattery";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = CreateBuildingDef(ID, 2, 2, 30, "smartbattery_kanim", 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, 0f, 0.5f, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
		obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(DigitBattery.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true) };
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DigitBattery batterySmart = go.AddOrGet<DigitBattery>();
		batterySmart.capacity = 20000f;
		batterySmart.joulesLostPerSecond = 2f / 3f;
		batterySmart.powerSortOrder = 1000;
		base.DoPostConfigureComplete(go);
	}
}
