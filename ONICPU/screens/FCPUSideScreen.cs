using ONICPU.ui;
using UnityEngine;
using UnityEngine.UI;

namespace ONICPU.screens
{
  public class FCPUSideScreen : SideScreenContent
  {
    public FCPU target;

    public Dropdown speedControl;
    public FButton editButton;
    public FButton manualButton;
    public FCheckbox requireENCheck;

    protected override void OnSpawn()
    {
      base.OnSpawn();

      manualButton.OnClick += ManualButton_onBtnClick;
      editButton.OnClick += EditButton_onBtnClick;
      speedControl.onValueChanged.AddListener((v) =>
      {
        if (target != null)
          target.CPUSpeed = v;
      });
      requireENCheck.onCheckedChanged += (v) =>
      {
        if (target != null)
        {
          target.requireENCheck = v;
          target.OnRequireENCheckChanged();
        }
      };
    }

    private void ManualButton_onBtnClick()
    {
      if (target != null)
        target.OnShowCPUManual();
    }
    private void EditButton_onBtnClick()
    {
      if (target != null)
        target.OnShowProgramEditor();
    }

    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<FCPU>() != null;
    }
    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      this.target = target.GetComponent<FCPU>();
      LoadValues();
    }

    public override string GetTitle()
    {
      return STRINGS.UI.UISIDESCREENS.FCPU.SIDESCREEN_TITLE;
    }

    private void LoadValues()
    {
      speedControl.options.Clear();
      for (var i = 0; i < (target.CPUType == FCPU.FCPUType.JavaScript ? FCPU.CPUSpeedArray1xIndex : FCPU.CPUSpeedArray.Count); i++)
      {
        var sp = FCPU.CPUSpeedArray[i];
        speedControl.options.Add(new Dropdown.OptionData() { text = $"{sp}x ({sp * 5}hz)" });
      }
      speedControl.value = target.CPUSpeed;
      requireENCheck.SetCheckedNoEvent(target.requireENCheck);
    }
  }
}
