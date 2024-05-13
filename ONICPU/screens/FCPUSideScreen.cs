using STRINGS;
using UnityEngine;

namespace ONICPU.screens
{
  public class FCPUSideScreen : SideScreenContent
  {
    public FCPU target;

    public KButton editButton;

    protected override void OnSpawn()
    {
      base.OnSpawn();

      editButton.onBtnClick += EditButton_onBtnClick;
    }

    private void EditButton_onBtnClick(KKeyCode obj)
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
    }

    public override string GetTitle()
    {
      return STRINGS.UI.UISIDESCREENS.FCPU.SIDESCREEN_TITLE;
    }
  }
}
