using ONICPU.storage;
using ONICPU.ui;
using UnityEngine;

namespace ONICPU.screens
{
  public class DigitSmartReservoirSideScreen : SideScreenContent
  {
    public IDigitSmartControlOutPutAmountOrPrecent targetSensor;

    public FCheckbox checkOutputPrecent;
    public FCheckbox checkOutputAmount;

    protected override void OnSpawn()
    {
      base.OnSpawn();
      checkOutputPrecent.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputPrecent = true;
          UpdateChecks();
        }
      };
      checkOutputAmount.onCheckedChanged += (value) =>
      {
        if (value)
        {
          targetSensor.OutputPrecent = false;
          UpdateChecks();
        }
      };
    }
    public override string GetTitle()
    {
      return STRINGS.UI.UISIDESCREENS.DIGITSMARTRESERVOIR.SIDESCREEN_TITLE;
    }

    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<IDigitSmartControlOutPutAmountOrPrecent>() != null;
    }
    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      targetSensor = target.GetComponent<IDigitSmartControlOutPutAmountOrPrecent>();
      UpdateChecks();
    }

    private void UpdateChecks()
    {
      checkOutputPrecent.Checked = targetSensor.OutputPrecent;
      checkOutputAmount.Checked = !targetSensor.OutputPrecent;
    }
  }
}
