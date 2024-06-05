using ONICPU.digit;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ONICPU.screens
{
  public class DigitConstantSideScreen : SideScreenContent
  {
    public const int MAX_BIT = 32;

    public DigitConstant target;

    public KButton valueApplyButton = null;
    public KButton[] valueButtons = new KButton[MAX_BIT];
    public LocText[] valueButtonTexs = new LocText[MAX_BIT];
    public KInputTextField valueInput;

    private static Color TextDeactiveColor = new Color(0.7f, 0.3f, 0.3f);
    private static Color TextActiveColor = new Color(0.15f, 0.93f, 0.13f);

    public override bool IsValidForTarget(GameObject target)
    {
      return target.GetComponent<DigitConstant>() != null;
    }
    public override void SetTarget(GameObject target)
    {
      base.SetTarget(target);
      this.target = target.GetComponent<DigitConstant>();
      InitEvents();
      UpdateValues();
    }

    private void UpdateValues()
    {
      valueInput.text = target.Value.ToString();
      for (var i = 0; i < MAX_BIT; i++)
      {
        var on = LogicCircuitNetwork.IsBitActive(i, target.Value);
        valueButtonTexs[i].text = on ? "1" : "0";
        valueButtonTexs[i].color = on ? TextActiveColor : TextDeactiveColor;
      }
    }
    private void UpdateOneBit(int bit)
    {
      var on = LogicCircuitNetwork.IsBitActive(bit, target.Value);
      if (on)
        target.Value &= ~(1 << bit);
      else
        target.Value |= 1 << bit;
      UpdateValues();
    }
    private void UpdateValue()
    {
      if (int.TryParse(valueInput.text, out var v))
      {
        target.Value = v;
        UpdateValues();
      }
      else
      {
        UIUtils.ShowMessageModal(
          Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.DIGITCONST.ERROR_TITLE"),
          Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.DIGITCONST.NOT_A_VALID_NUMBER")
        );
      }
    }

    private bool isInitEvents = false;
    private void InitEvents()
    {
      if (isInitEvents)
        return;

      for (int i = 0; i < MAX_BIT; i++)
      {
        var index = i;
        valueButtons[index].onClick += () => UpdateOneBit(index);
      }

      valueApplyButton.onClick += () => UpdateValue();

      isInitEvents = true;
    }

    public override string GetTitle()
    {
      return STRINGS.UI.UISIDESCREENS.DIGITCONST.SIDESCREEN_TITLE;
    }
  }
}
