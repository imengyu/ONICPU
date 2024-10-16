using ONICPU.wireless;
using UnityEngine;
using UnityEngine.UI;

namespace ONICPU.screens
{
  public class DigitReceiverSideScreen : SideScreenContent
  {
    public DigitReceiver target;

    public Dropdown currentValue;

    protected override void OnSpawn()
    {
      base.OnSpawn();
      currentValue.onValueChanged.AddListener((v) =>
      {
        if (target != null)
          target.Channel = v;
      });
    }
    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<DigitReceiver>() != null;
    }
    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      this.target = target.GetComponent<DigitReceiver>();
      LoadValues();
    }
    private void LoadValues()
    {
      currentValue.value = target.Channel;
    }
  }
}
