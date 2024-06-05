using System.Collections.Generic;
using System.Reflection;
using static DetailsScreen;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using KMod;

namespace ONICPU
{
  public static class UIUtils
  {
    public static KInputField KInputFieldPrefab = null;
    public static LocText LocTextPrefab = null;
    public static KButton KButtonPrefab = null;
    public static KScrollRect KScrollRectPrefab = null;
    public static Scrollbar ScrollbarPrefab = null;

    private static int getedInternalPrefabs = -1;

    public static bool GetKleiInternalPrefabs()
    {
      if (getedInternalPrefabs != -1)
        return getedInternalPrefabs == 1;

      getedInternalPrefabs = 0;

      Debug.Log("Get Prefabs");

      //Access private prefabs in DetailsScreen
      FieldInfo fieldInfoSideScreens = typeof(DetailsScreen).GetField("sideScreens", BindingFlags.NonPublic | BindingFlags.Instance);
      FieldInfo fieldInfonameInputField = typeof(AlarmSideScreen).GetField("nameInputField", BindingFlags.NonPublic | BindingFlags.Instance);
      FieldInfo activateLabelField = typeof(ActiveRangeSideScreen).GetField("activateLabel", BindingFlags.NonPublic | BindingFlags.Instance);
      FieldInfo skillsScreenlField = typeof(ManagementMenu).GetField("skillsScreen", BindingFlags.NonPublic | BindingFlags.Instance);
      FieldInfo skillsScreenscrollRectField = typeof(SkillsScreen).GetField("scrollRect", BindingFlags.NonPublic | BindingFlags.Instance);

      //Found AlarmSideScreen it contains KInputField prefab
      List<SideScreenRef> sideScreens = fieldInfoSideScreens.GetValue(DetailsScreen.Instance) as List<SideScreenRef>;
      if (sideScreens == null)
      {
        Debug.LogWarning("sideScreens is null, maybe game was updated");
        return false;
      }
      SideScreenRef alarmSideScreenRef = null;
      SideScreenRef activeRangeSideScreenRef = null;
      SideScreenRef artableSelectionSideScreenRef = null;
      foreach (var sideScreenRef in sideScreens)
      {
        if (sideScreenRef.screenPrefab is AlarmSideScreen)
        {
          alarmSideScreenRef = sideScreenRef;
          Debug.LogWarning("Found AlarmSideScreen");
        }
        else if (sideScreenRef.screenPrefab is ActiveRangeSideScreen)
        {
          activeRangeSideScreenRef = sideScreenRef;
          Debug.LogWarning("Found ActiveRangeSideScreen");
        }
        else if (sideScreenRef.screenPrefab is ArtableSelectionSideScreen)
        {
          artableSelectionSideScreenRef = sideScreenRef;
          Debug.LogWarning("Found ArtableSelectionSideScreenRef");
        }
      }
      if (alarmSideScreenRef == null)
      {
        Debug.LogWarning("AlarmSideScreen NOT FOUND, maybe game was updated");
        return false;
      }
      if (activeRangeSideScreenRef == null)
      {
        Debug.LogWarning("activeRangeSideScreen NOT FOUND, maybe game was updated");
        return false;
      }
      if (artableSelectionSideScreenRef == null)
      {
        Debug.LogWarning("artableSelectionSideScreenRef NOT FOUND, maybe game was updated");
        return false;
      }
      SkillsScreen skillsScreen = skillsScreenlField.GetValue(ManagementMenu.Instance) as SkillsScreen;
      if (skillsScreen == null)
      {
        Debug.LogWarning("ManagementMenu.Instance.skillsScreen NOT FOUND, maybe game was updated");
        return false;
      }

      //Get prefabs
      KInputFieldPrefab = fieldInfonameInputField.GetValue(alarmSideScreenRef.screenPrefab) as KInputField;
      LocTextPrefab = activateLabelField.GetValue(activeRangeSideScreenRef.screenPrefab) as LocText;
      KButtonPrefab = (artableSelectionSideScreenRef.screenPrefab as ArtableSelectionSideScreen).applyButton;
      KScrollRectPrefab = skillsScreenscrollRectField.GetValue(skillsScreen) as KScrollRect;
      KScrollRectPrefab = skillsScreenscrollRectField.GetValue(skillsScreen) as KScrollRect;
      ScrollbarPrefab = DetailsScreen.Instance.transform.Find("Body/Scrollbar").gameObject.GetComponent<Scrollbar>();
      if (KInputFieldPrefab == null)
      {
        Debug.Log("KInputFieldPrefab is null, maybe game was updated");
        return false;
      }
      if (LocTextPrefab == null)
      {
        Debug.LogWarning("LocTextPrefab is null, maybe game was updated");
        return false;
      }
      if (KButtonPrefab == null)
      {
        Debug.LogWarning("ButtonPrefab is null, maybe game was updated");
        return false;
      }
      if (KScrollRectPrefab == null)
      {
        Debug.LogWarning("ScrollRectPrefab is null, maybe game was updated");
        return false;
      }
      if (ScrollbarPrefab == null)
      {
        Debug.LogWarning("ScrollbarPrefab is null, maybe game was updated");
        return false;
      }

      getedInternalPrefabs = 1;
      return true;
    }

    public static Image AddImageLine(Sprite sprite, string name, float width, float height, float y, RectTransform parent)
    {
      var BreakPoint = new GameObject(name);
      var BreakPointRectTransform = BreakPoint.AddComponent<RectTransform>();
      var BreakPointImage = BreakPoint.AddComponent<Image>();
      BreakPoint.SetActive(false);
      BreakPointImage.sprite = sprite;
      BreakPointRectTransform.SetParent(parent);
      UIAnchorPosUtils.SetUIPivot(BreakPointRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(BreakPointRectTransform, UIAnchor.Left, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(BreakPointRectTransform, 0);
      BreakPointRectTransform.sizeDelta = new Vector2(width, height);
      BreakPointRectTransform.anchoredPosition = new Vector2(0, y);
      return BreakPointImage;
    }
    public static LocText AddTextLine(string str, string name, float width, float height, float x, float y, RectTransform parent, System.Action click = null, float size = 13)
    {
      var LineText = UnityEngine.Object.Instantiate(LocTextPrefab.gameObject, parent);
      var LineTextRectTransform = LineText.GetComponent<RectTransform>();
      var LineTextText = LineText.GetComponent<LocText>();
      LineText.name = name;
      UIAnchorPosUtils.SetUIPivot(LineTextRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(LineTextRectTransform, UIAnchor.Left, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(LineTextRectTransform, x);
      LineTextRectTransform.sizeDelta = new Vector2(width, height);
      LineTextRectTransform.anchoredPosition = new Vector2(LineTextRectTransform.anchoredPosition.x, y);
      LineTextText.color = Color.white;
      LineTextText.alignment = TextAlignmentOptions.MidlineRight;
      LineTextText.text = str;
      LineTextText.fontSize = size;
      LineTextText.enableWordWrapping = false;
      LineTextText.maxVisibleLines = 1;

      if (click != null)
      {
        var button = LineText.AddComponent<KButton>();
        button.soundPlayer = KButtonPrefab.soundPlayer;
        button.onClick += click;
      }

      return LineTextText;
    }
    public static KButton AddButtonLine(string str, string key, string tooltip, float x, float y, RectTransform parent, System.Action click, float width = 100, float height = 40)
    {
      var StopButton = UnityEngine.Object.Instantiate(KButtonPrefab.gameObject, parent);
      var StopButtonButton = StopButton.GetComponent<KButton>();
      var StopButtonButtonRectTransform = StopButton.GetComponent<RectTransform>();
      StopButton.GetComponent<ToolTip>().SetFixedStringKey(tooltip);
      if (click != null)
        StopButtonButton.onClick += click;
      UIAnchorPosUtils.SetUIPivot(StopButtonButtonRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(StopButtonButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(StopButtonButtonRectTransform, x);
      StopButtonButtonRectTransform.sizeDelta = new Vector2(width, height);
      StopButtonButtonRectTransform.anchoredPosition = new Vector2(StopButtonButtonRectTransform.anchoredPosition.x, y);

      var LineText = StopButtonButtonRectTransform.GetChild(0).gameObject;
      var LineTextText = LineText.GetComponent<LocText>();
      LineTextText.text = str;
      LineTextText.key = key;
      LineTextText.ApplySettings();
      return StopButtonButton;
    }
    public static RectTransform AddSpaceLine(float width, float height, RectTransform parent)
    {
      var group = new GameObject();
      var groupRectTransform = group.AddComponent<RectTransform>();
      var groupLayoutElement = group.AddComponent<LayoutElement>();
      groupLayoutElement.minWidth = width;
      groupLayoutElement.minHeight = height;
      groupRectTransform.SetParent(parent);
      return groupRectTransform;
    }

    public static void ShowMessageModal(string title, string message)
    {
      Util.KInstantiateUI<InfoDialogScreen>(
        ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, force_active: true)
           .SetHeader(title)
           .AddPlainText(message)
           .AddDefaultOK();
    }
  }
}
