using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ONICPU.storage
{
  public class DigitLiquidReservoirConfig : IBuildingConfig
  {
    public const string ID = "DigitLiquidReservoir";

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "liquidreservoir_kanim", 100, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0);
      buildingDef.InputConduitType = ConduitType.Liquid;
      buildingDef.OutputConduitType = ConduitType.Liquid;
      buildingDef.Floodable = false;
      buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
      buildingDef.AudioCategory = "HollowMetal";
      buildingDef.UtilityInputOffset = new CellOffset(1, 2);
      buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
      buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(DigitSmartReservoir.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.DIGITSMARTRESERVOIR.LOGIC_PORT, "", "") };
      GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
      return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
      go.AddOrGet<Reservoir>();
      Storage storage = BuildingTemplates.CreateDefaultStorage(go);
      storage.showDescriptor = true;
      storage.allowItemRemoval = false;
      storage.storageFilters = STORAGEFILTERS.LIQUIDS;
      storage.capacityKg = 5000f;
      storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
      storage.showCapacityStatusItem = true;
      storage.showCapacityAsMainStatus = true;
      go.AddOrGet<DigitSmartReservoir>();
      ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
      conduitConsumer.conduitType = ConduitType.Liquid;
      conduitConsumer.ignoreMinMassCheck = true;
      conduitConsumer.forceAlwaysSatisfied = true;
      conduitConsumer.alwaysConsume = true;
      conduitConsumer.capacityKG = storage.capacityKg;
      ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
      conduitDispenser.conduitType = ConduitType.Liquid;
      conduitDispenser.elementFilter = null;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      go.AddOrGetDef<StorageController.Def>();
      go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
    }
  }
}
