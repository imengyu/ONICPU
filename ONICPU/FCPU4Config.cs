using TUNING;
using UnityEngine;

namespace ONICPU
{
  public class FCPU4Config : IBuildingConfig
  {
    public const string ID = "FCPU4";

    protected virtual string GetID()
    {
      return ID;
    }
    protected virtual string GetAnim()
    {
      return "fcpu_kanim";
    }
    protected virtual int GetW()
    {
      return 3;
    }
    protected virtual int GetH()
    {
      return 4;
    }
    protected virtual void OnFCpuAdded(FCPU component)
    {
      component.CPUType = FCPU.FCPUType.AssemblyCode;
    }

    public override BuildingDef CreateBuildingDef()
    {
      BuildingDef obj = BuildingTemplates.CreateBuildingDef(
        GetID(), GetW(), GetH(),
        GetAnim(), 10, 3f, 
        BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, 
        MATERIALS.REFINED_METALS, 1600f, 
        BuildLocationRule.Anywhere, 
        noise: NOISE_POLLUTION.NONE, 
        decor: BUILDINGS.DECOR.PENALTY.TIER0
      );
      obj.ViewMode = OverlayModes.Logic.ID;
      obj.ObjectLayer = ObjectLayer.LogicGate;
      obj.SceneLayer = Grid.SceneLayer.LogicGates;
      obj.ThermalConductivity = 0.05f;
      obj.Floodable = false;
      obj.Overheatable = false;
      obj.Entombable = false;
      obj.AudioCategory = "Metal";
      obj.AudioSize = "small";
      obj.BaseTimeUntilRepair = -1f;
      obj.PermittedRotations = PermittedRotations.R360;
      obj.DragBuild = false;
      return obj;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
      OnFCpuAdded(go.AddOrGet<FCPU>());
    }
    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
      GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    }
  }
}
