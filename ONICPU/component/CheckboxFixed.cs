using System;
using UnityEngine;

namespace ONICPU.component
{
  public class CheckboxFixed : MonoBehaviour
  {
    public KToggle Toggle;
    public KImage Checkmark;
    public LocText Label;
    public string LabelKey;
    public string LabelText;

    private void Start()
    {
      Toggle = GetComponent<KToggle>();
      Checkmark = transform.Find("CheckMark").GetComponent<KImage>();
      Checkmark.enabled = _Checked;
      Label = transform.parent.Find("Label").GetComponent<LocText>();
      if (!string.IsNullOrEmpty(LabelKey))
      {
        Label.key = LabelKey;
        Label.ApplySettings();
      }
      else if (!string.IsNullOrEmpty(LabelText))
        Label.text = LabelText;
      Toggle.onClick += Toggle_onClick;
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

    private void Toggle_onClick()
    {
      Checked = !Checked;
    }
  }
}
