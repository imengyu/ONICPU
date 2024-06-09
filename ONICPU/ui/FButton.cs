using UnityEngine;
using UnityEngine.UI;

namespace ONICPU.ui
{
  public class FButton : MonoBehaviour
  {
    public event System.Action OnClick;

    public KButton kButton;
    public LocText label;
    public ToolTip toolTip;
    public Image image;
    public RectTransform imageRectTransform;

    public string LabelKey = "";
    public string LabelText = ""; 
    public string TooltipText = "";
    public string TooltipKey = "";
    public Sprite Icon = null;
    public Vector2 IconSize = default(Vector2);

    void Start()
    {
      toolTip = GetComponent<ToolTip>();
      label = transform.Find("Label").GetComponent<LocText>();

      var EmptyImage = new GameObject("Image");
      imageRectTransform = EmptyImage.AddComponent<RectTransform>();
      image = EmptyImage.AddComponent<Image>();
      imageRectTransform.SetParent(transform);
      imageRectTransform.sizeDelta = new Vector2(23, 23);
      imageRectTransform.anchoredPosition = new Vector2(0, 0);

      kButton.onBtnClick += KButton_onBtnClick;
      UpdateButton();
    }

    public void UpdateButton()
    {
      var hasLabel = true;
      if (!string.IsNullOrEmpty(LabelKey))
      {
        label.key = LabelKey;
        label.gameObject.SetActive(true);
        label.ApplySettings();
      }
      else if (!string.IsNullOrEmpty(LabelText))
      {
        label.text = LabelText;
        label.gameObject.SetActive(true);
      }
      else
      {
        hasLabel = false;
        label.gameObject.SetActive(false);
      }

      if (!string.IsNullOrEmpty(TooltipText))
      {
        toolTip.toolTip = TooltipText;
        toolTip.enabled = true;
      }
      else if (!string.IsNullOrEmpty(TooltipKey))
      {
        toolTip.toolTip = Utils.GetLocalizeString(TooltipKey);
        toolTip.enabled = true;
      }
      else
      {
        toolTip.enabled = false;
      }

      if (Icon != null)
      {
        image.gameObject.SetActive(true);
        image.sprite = Icon;
        if (IconSize.x == 0) IconSize.x = 23;
        if (IconSize.y == 0) IconSize.y = 23;
        imageRectTransform.sizeDelta = IconSize;

        if (hasLabel)
        {
          imageRectTransform.anchoredPosition = new Vector2(10, 0);
          UIAnchorPosUtils.SetUIPivot(imageRectTransform, UIPivot.CenterLeft);
        }
        else
        {
          imageRectTransform.anchoredPosition = new Vector2(0, 0);
          UIAnchorPosUtils.SetUIPivot(imageRectTransform, UIPivot.Center);
        }
      }
      else
      {
        image.gameObject.SetActive(false);
      }
    }

    private void KButton_onBtnClick(KKeyCode obj)
    {
      OnClick?.Invoke();
    }
  }
}
