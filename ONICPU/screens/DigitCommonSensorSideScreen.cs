using STRINGS;
using UnityEngine;

namespace ONICPU.screens
{
  public class DigitCommonSensorSideScreen : SideScreenContent, IRender200ms
  {
    public IDigitCommonSensor targetSensor;

    public LocText currentValue;

    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<IDigitCommonSensor>() != null;
    }
    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      targetSensor = target.GetComponent<IDigitCommonSensor>();
      UpdateLabels();
    }

    public void Render200ms(float dt)
    {
      if (targetSensor != null)
      {
        UpdateLabels();
      }
    }
    private void UpdateLabels()
    {
      currentValue.text = string.Format(
        UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CURRENT_VALUE, 
        targetSensor.ThresholdValueName,
        targetSensor.Format(targetSensor.CurrentValue, units: true)
      );
    }
    public override string GetTitle()
    {
      return STRINGS.UI.UISIDESCREENS.COMMONSENSOR.SIDESCREEN_TITLE;
    }
  }

  public interface IDigitCommonSensor
  {
    float CurrentValue { get; }
    LocString ThresholdValueName { get; }
    string Format(float value, bool units);

  }
}
