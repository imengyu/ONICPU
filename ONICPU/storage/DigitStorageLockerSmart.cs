using KSerialization;
using System.Reflection;
using UnityEngine;

namespace ONICPU.storage
{
  public class DigitStorageLockerSmart : StorageLocker, IDigitSmartControlOutPutAmountOrPrecent
  {
    [MyCmpGet]
    private LogicPorts ports;

    [MyCmpGet]
    private Operational operational;

    [Serialize]
    public bool outputPrecent = true;

    public bool OutputPrecent
    {
      get => outputPrecent;
      set {
        outputPrecent = value;
        UpdateLogicAndActiveState();
      }
    }

    private static readonly EventSystem.IntraObjectHandler<DigitStorageLockerSmart> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<DigitStorageLockerSmart>(delegate (DigitStorageLockerSmart component, object data)
    {
      component.UpdateLogicCircuitCB(data);
    });

    public override float UserMaxCapacity
    {
      get
      {
        return base.UserMaxCapacity;
      }
      set
      {
        base.UserMaxCapacity = value;
        UpdateLogicAndActiveState();
      }
    }

    protected override void OnPrefabInit()
    {
      Initialize(use_logic_meter: true);
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();
      ports = base.gameObject.GetComponent<LogicPorts>();
      Subscribe(-1697596308, UpdateLogicCircuitCBDelegate);
      Subscribe(-592767678, UpdateLogicCircuitCBDelegate);
      UpdateLogicAndActiveState();
    }

    private void UpdateLogicCircuitCB(object data)
    {
      UpdateLogicAndActiveState();
    }

    private void UpdateLogicAndActiveState()
    {
      MethodInfo GetMaxCapacityMinusStorageMargin = filteredStorage.GetType()
        .GetMethod("GetMaxCapacityMinusStorageMargin", BindingFlags.NonPublic | BindingFlags.Instance);
      MethodInfo GetAmountStored = filteredStorage.GetType()
        .GetMethod("GetAmountStored", BindingFlags.NonPublic | BindingFlags.Instance);

      float maxCapacityMinusStorageMargin = (float)GetMaxCapacityMinusStorageMargin.Invoke(filteredStorage, new object[0]);
      float amountStored = (float)GetAmountStored.Invoke(filteredStorage, new object[0]);
      float precent = Mathf.Clamp01(amountStored / maxCapacityMinusStorageMargin);

      bool isOperational = operational.IsOperational;
      ports.SendSignal(FilteredStorage.FULL_PORT_ID, isOperational ? 
        (outputPrecent ? Mathf.CeilToInt(precent * 100) : (int)amountStored) : 0
      );
      filteredStorage.SetLogicMeter(isOperational && filteredStorage.IsFull());
      operational.SetActive(isOperational);
    }
  }
}
