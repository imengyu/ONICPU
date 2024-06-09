using System;
using UnityEngine;

namespace ONICPU.ui
{
  public class FCheckbox : MonoBehaviour
  {
    public KToggle Toggle;
    public KImage Checkmark;
    public LocText Label;
    public string LabelKey;
    public string LabelText;

    private void Start()
    {
      Toggle = transform.GetChild(0).GetComponent<KToggle>();
      Checkmark = transform.GetChild(0).Find("CheckMark").GetComponent<KImage>();
      Checkmark.enabled = _Checked;
      Label = transform.Find("Label").GetComponent<LocText>();
      Toggle.onClick += Toggle_onClick;
      UpdateText();
    }

    public event Action<bool> onCheckedChanged;

    private bool _Checked = false;

    public bool Checked
    {
      get { return _Checked; }
      set {
        if (_Checked != value)
        {
          _Checked = value;
          if (Checkmark != null)
            Checkmark.enabled = _Checked;
          onCheckedChanged?.Invoke(_Checked);
        }
      }
    }

    public void UpdateText()
    {
      if (!string.IsNullOrEmpty(LabelKey))
      {
        Label.key = LabelKey;
        Label.ApplySettings();
      }
      else if (!string.IsNullOrEmpty(LabelText))
        Label.text = LabelText;
    }

    private void Toggle_onClick()
    {
      Checked = !Checked;
    }
  }
}
