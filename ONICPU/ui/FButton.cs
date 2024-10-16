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
    public Vector2 Size = default;
    public Vector2 IconSize = default;

    void Start()
    {
      toolTip = gameObject.GetComponent<ToolTip>();
      if (label == null)
        label = transform.Find("Label")?.GetComponent<LocText>();
      if (toolTip == null)
        toolTip = gameObject.AddComponent<ToolTip>();
      if (kButton == null)
        kButton = gameObject.GetComponent<KButton>();

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
        label.ApplySettings();
        label.gameObject.SetActive(true);
      }
      else if (!string.IsNullOrEmpty(LabelText))
      {
        label.text = LabelText;
        label.gameObject.SetActive(true);
      }
      else
      {
        if (label != null)
        {
          if (label.key == "STRINGS.UI.UISIDESCREENS.FCPU.CLEAR_BUTTON")
            throw new System.Exception("Fuck!!!  " + gameObject.name + " LabelKey: " + LabelKey );
          label.gameObject.SetActive(false);
        }
        hasLabel = false;
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


      if (Size.x != 0 && Size.y != 0)
      {
        (transform as RectTransform).sizeDelta = new Vector2(Size.x, Size.y);
        var layout = GetComponent<LayoutElement>();
        if (layout != null)
        {
          layout.minWidth = Size.x;
          layout.minHeight = Size.y;
        }
      }
      if (Icon != null)
      {
        image.gameObject.SetActive(true);
        image.sprite = Icon;
        if (IconSize.x == 0) IconSize.x = 23;
        if (IconSize.y == 0) IconSize.y = 23;
        imageRectTransform.sizeDelta = new Vector2(IconSize.x, IconSize.y);

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
