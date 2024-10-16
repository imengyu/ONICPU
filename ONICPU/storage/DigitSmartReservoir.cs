using FMOD;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace ONICPU.storage
{
  public class DigitSmartReservoir : KMonoBehaviour, ISim200ms, IDigitSmartControlOutPutAmountOrPrecent
  {
    [MyCmpGet]
    private Storage storage;

    [MyCmpGet]
    private Operational operational;

    [Serialize]
    private int activateValue;

    [Serialize]
    private int deactivateValue = 100;

    [Serialize]
    private int value;

    [MyCmpGet]
    private LogicPorts logicPorts;

    [MyCmpAdd]
    private CopyBuildingSettings copyBuildingSettings;

    [Serialize]
    public bool outputPrecent = true;

    public bool OutputPrecent
    {
      get => outputPrecent;
      set { 
        outputPrecent = value;
        UpdateLogicCircuit(null);
      }
    }

    private MeterController logicMeter;

    public static readonly HashedString PORT_ID = "SmartReservoirLogicPort";

    private static readonly EventSystem.IntraObjectHandler<DigitSmartReservoir> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DigitSmartReservoir>(delegate (DigitSmartReservoir component, object data)
    {
      component.OnCopySettings(data);
    });

    private static readonly EventSystem.IntraObjectHandler<DigitSmartReservoir> UpdateLogicCircuitDelegate = new EventSystem.IntraObjectHandler<DigitSmartReservoir>(delegate (DigitSmartReservoir component, object data)
    {
      component.UpdateLogicCircuit(data);
    });

    public float PercentFull => storage.MassStored() / storage.Capacity();

    protected override void OnSpawn()
    {
      base.OnSpawn();
      Subscribe(-592767678, UpdateLogicCircuitDelegate);
      SetLogicMeter(true);
    }

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      Subscribe(-905833192, OnCopySettingsDelegate);
    }

    public void Sim200ms(float dt)
    {
      UpdateLogicCircuit(null);
    }

    private void UpdateLogicCircuit(object data)
    {
      value = outputPrecent ? Mathf.CeilToInt(PercentFull * 100) : (int)storage.MassStored();
      logicPorts.SendSignal(PORT_ID, value);
    }

    private void OnCopySettings(object data)
    {
      DigitSmartReservoir component = ((GameObject)data).GetComponent<DigitSmartReservoir>();
      if (component != null)
      {
        outputPrecent = component.OutputPrecent;
      }
    }

    public void SetLogicMeter(bool on)
    {
      if (logicMeter != null)
      {
        logicMeter.SetPositionPercent(on ? 1f : 0f);
      }
    }
  }

  public interface IDigitSmartControlOutPutAmountOrPrecent
  {
    bool OutputPrecent { get; set; }
  }
}
