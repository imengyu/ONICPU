using ONICPU.ui;
using UnityEngine;
using static ONICPU.sensor.DigitPressureSensor;

namespace ONICPU.screens
{
  public interface IDigitPressureSensor
  {
    Unit OutputUnit { get; set; }
  }

  public class DigitPressureSensorSideScreen : SideScreenContent
  {
    public IDigitPressureSensor targetSensor;

    public FCheckbox checkUnitug;
    public FCheckbox checkUnitmg;
    public FCheckbox checkUnitg;
    public FCheckbox checkUnitkg;
    public FCheckbox checkUnitt;
    public FCheckbox checkUnitpounds;
    public FCheckbox checkUnitgr;
    public FCheckbox checkUnitdr;

    protected override void OnSpawn()
    {
      base.OnSpawn();
      checkUnitug.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.μg;
          UpdateChecks();
        }
      };
      checkUnitmg.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.mg;
          UpdateChecks();
        }
      };
      checkUnitg.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.g;
          UpdateChecks();
        }
      };
      checkUnitkg.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.kg;
          UpdateChecks();
        }
      };
      checkUnitt.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.t;
          UpdateChecks();
        }
      };
      checkUnitpounds.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.pounds;
          UpdateChecks();
        }
      };
      checkUnitgr.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.gr;
          UpdateChecks();
        }
      };
      checkUnitdr.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputUnit = Unit.dr;
          UpdateChecks();
        }
      };
    }
    public override string GetTitle()
    {
      return STRINGS.UI.UISIDESCREENS.DIGITPRESSURESENSOR.SIDESCREEN_TITLE;
    }

    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<IDigitPressureSensor>() != null;
    }

    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      targetSensor = target.GetComponent<IDigitPressureSensor>();
      UpdateChecks();
    }

    private void UpdateChecks()
    {
      checkUnitug.Checked = targetSensor.OutputUnit == Unit.μg;
      checkUnitmg.Checked = targetSensor.OutputUnit == Unit.mg;
      checkUnitg.Checked = targetSensor.OutputUnit == Unit.g;
      checkUnitkg.Checked = targetSensor.OutputUnit == Unit.kg;
      checkUnitt.Checked = targetSensor.OutputUnit == Unit.t;
      checkUnitpounds.Checked = targetSensor.OutputUnit == Unit.pounds;
      checkUnitgr.Checked = targetSensor.OutputUnit == Unit.gr;
      checkUnitdr.Checked = targetSensor.OutputUnit == Unit.dr;
    }
  }
}
