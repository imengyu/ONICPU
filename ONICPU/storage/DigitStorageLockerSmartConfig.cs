using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

namespace ONICPU.storage
{
  public class DigitStorageLockerSmartConfig : IBuildingConfig
  {
    public const string ID = "DigitStorageLockerSmart";

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "smartstoragelocker_kanim", 30, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
      obj.Floodable = false;
      obj.AudioCategory = "Metal";
      obj.Overheatable = false;
      obj.ViewMode = OverlayModes.Logic.ID;
      obj.RequiresPowerInput = true;
      obj.AddLogicPowerPort = false;
      obj.EnergyConsumptionWhenActive = 60f;
      obj.ExhaustKilowattsWhenActive = 0.125f;
      obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonOutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.DIGITSTORAGELOCKERSMART.LOGIC_PORT, "", "", show_wire_missing_icon: true) };
      return obj;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
      Prioritizable.AddRef(go);
      Storage storage = go.AddOrGet<Storage>();
      storage.showInUI = true;
      storage.allowItemRemoval = true;
      storage.showDescriptor = true;
      storage.storageFilters = STORAGEFILTERS.STORAGE_LOCKERS_STANDARD;
      storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
      storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
      storage.showCapacityStatusItem = true;
      storage.showCapacityAsMainStatus = true;
      go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
      go.AddOrGet<DigitStorageLockerSmart>();
      go.AddOrGet<UserNameable>();
      go.AddOrGetDef<StorageController.Def>();
      go.AddOrGetDef<RocketUsageRestriction.Def>();
    }
  }
}
