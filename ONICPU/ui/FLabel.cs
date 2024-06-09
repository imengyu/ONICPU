using UnityEngine;

namespace ONICPU.ui
{
  public class FLabel : MonoBehaviour
  {
    public string LabelKey;
    public string LabelText;
    public LocText Label;

    void Start()
    {
      Label = GetComponent<LocText>();
      UpdateText();
    }

    public void SetText(string text)
    {
      Label.text = text;
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
  }
}
