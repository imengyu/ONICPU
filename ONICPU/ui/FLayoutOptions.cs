using UnityEngine;
using UnityEngine.UI;

namespace ONICPU.ui
{
  public class FLayoutOptions
  {
    public const float FILL_PARENT = 0;
    public const float INTITAL = -1;

    public float Width = FILL_PARENT;
    public float Height = INTITAL;
    public float MinWidth = 30;
    public float MinHeight = 20;

    public FLayoutOptions() { }
    public FLayoutOptions(float width, float height, float minWidth = 30, float minHeight = 20)
    {
      Width = width;
      Height = height;
      MinWidth = minWidth;
      MinHeight = minHeight;
    }

    public void ApplyRectTaransform(RectTransform rect)
    {
      var parent = rect.parent as RectTransform;
      var groupLayoutElement = rect.gameObject.GetComponent<LayoutElement>();
      if (groupLayoutElement == null) 
        groupLayoutElement = rect.gameObject.AddComponent<LayoutElement>();

      //UIAnchorPosUtils.SetUIPivot(rect, UIPivot.TopLeft);
      //UIAnchorPosUtils.SetUIAnchor(rect, UIAnchor.Left, UIAnchor.Top);
      if (Width == FILL_PARENT)
      {
        UIAnchorPosUtils.SetUILeft(rect, 0);
        UIAnchorPosUtils.SetUIRight(rect, 0);
        rect.sizeDelta = new Vector2(parent.sizeDelta.x, rect.sizeDelta.y);
      } 
      else if (Width > 0)
      {
        rect.sizeDelta = new Vector2(Width, rect.sizeDelta.y);
        groupLayoutElement.minWidth = Width > MinWidth ? MinWidth : Width;
        groupLayoutElement.preferredWidth = Width;
      }

      if (Height == FILL_PARENT)
      {
        UIAnchorPosUtils.SetUITop(rect, 0);
        UIAnchorPosUtils.SetUIBottom(rect, 0);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, parent.sizeDelta.y);
      }
      else if (Height > 0)
      {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Height);
        groupLayoutElement.minHeight = Height > MinHeight ? Height : MinHeight;
        groupLayoutElement.preferredHeight = Height;
      }
    }

    public static FLayoutOptions DEFAULT = new FLayoutOptions();
  }
}
