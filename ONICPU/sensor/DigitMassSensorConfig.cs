using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DigitMassSensorConfig : IBuildingConfig
{
	public static string ID = "DigitMassSensor";

  public override BuildingDef CreateBuildingDef()
  {
    BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "pressureswitch_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
    obj.IsFoundation = true;
    obj.Overheatable = false;
    obj.Floodable = false;
    obj.Entombable = false;
    obj.ViewMode = OverlayModes.Logic.ID;
    obj.TileLayer = ObjectLayer.FoundationTile;
    obj.ReplacementLayer = ObjectLayer.ReplacementTile;
    obj.SceneLayer = Grid.SceneLayer.TileMain;
    obj.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
    obj.AudioCategory = "Metal";
    obj.AudioSize = "small";
    obj.BaseTimeUntilRepair = -1f;
    obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true) };
    obj.AlwaysOperational = true;
    GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
    return obj;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
    SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
    simCellOccupier.doReplaceElement = true;
    simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_2;
    simCellOccupier.notifyOnMelt = true;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<DigitMassSensor>();
    go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
  }
}
