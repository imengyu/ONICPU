using STRINGS;
using UnityEngine;

namespace ONICPU.sensor
{
  public class DigitWattageSensor : DigitCommonSensor
  {
    protected override float simValue(out bool skip)
    {
      float currentWattage = Game.Instance.circuitManager.GetWattsUsedByCircuit(Game.Instance.circuitManager.GetCircuitID(Grid.PosToCell(this)));
      currentWattage = Mathf.Max(0f, currentWattage);
      skip = false;
      return currentWattage;
    }
    public override LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE;
    public override string Format(float value, bool units)
    {
      return GameUtil.GetFormattedWattage(value, GameUtil.WattageFormatterUnit.Watts, units);
    }
  }
}
