using ONICPU.component;
using UnityEngine;

namespace ONICPU.screens
{
  public class DigitCritterSensorSideScreen : SideScreenContent
  {
    public DigitCritterCountSensor targetSensor;

    public CheckboxFixed countCrittersToggle;
    public CheckboxFixed countEggsToggle;

    protected override void OnSpawn()
    {
      base.OnSpawn();
      countCrittersToggle.onCheckedChanged += ToggleCritters;
      countEggsToggle.onCheckedChanged += ToggleEggs;
    }

    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<DigitCritterCountSensor>() != null;
    }
    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      targetSensor = target.GetComponent<DigitCritterCountSensor>();
      countCrittersToggle.Checked = targetSensor.countCritters;
      countEggsToggle.Checked = targetSensor.countEggs;
    }

    private void ToggleCritters(bool checkedValue)
    {
      targetSensor.countCritters = checkedValue;
    }
    private void ToggleEggs(bool checkedValue)
    {
      targetSensor.countEggs = checkedValue;
    }
  }
}
