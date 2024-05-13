using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DigitDuplicantSensorConfig : IBuildingConfig
{
	public const string ID = "DigitDuplicantSensor";

	private const int RANGE = 4;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "presence_sensor_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFoundationRotatable, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true) };
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		AddVisualizer(go, movable: true);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<DigitDuplicantSensor>();
		AddVisualizer(go, movable: false);
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}

	private static void AddVisualizer(GameObject prefab, bool movable)
	{
		RangeVisualizer rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
		rangeVisualizer.OriginOffset = new Vector2I(0, 0);
		rangeVisualizer.RangeMin.x = -2;
		rangeVisualizer.RangeMin.y = 0;
		rangeVisualizer.RangeMax.x = 2;
		rangeVisualizer.RangeMax.y = 4;
		rangeVisualizer.BlockingTileVisible = true;
	}
}
