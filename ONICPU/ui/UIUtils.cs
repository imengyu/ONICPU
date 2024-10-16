using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine;
using ONICPU.ui;
using TMPro;
using static STRINGS.UI.FRONTEND;

namespace ONICPU
{
  public static class UIUtils
  {
    public const float SIZE_SCREEN_WIDTH = 260;

    public static KSlider KSliderPrefab = null;
    public static KScrollRect KScrollRectPrefab = null;
    public static KInputField KInputFieldPrefab = null;
    public static KNumberInputField KNumberInputFieldPrefab = null;
    public static LocText LocTextPrefab = null;
    public static Scrollbar ScrollbarPrefab = null;
    public static Dropdown DropdownPrefab = null;


    public static FLabel LabelPrefab = null;
    public static FButton ButtonPrefab = null;
    public static FCheckbox CheckboxPrefab = null;
    
    private static DetailsScreen.SideScreenRef critterSensorSideScreenRef = null;

    public static bool GetKleiInternalPrefabs()
    {
      Debug.Log("Get Prefabs");

      //Access private prefabs in DetailsScreen
      FieldInfo fieldInfoSideScreens = typeof(DetailsScreen).GetField("sideScreens", BindingFlags.NonPublic | BindingFlags.Instance);
      FieldInfo skillsScreenlField = typeof(ManagementMenu).GetField("skillsScreen", BindingFlags.NonPublic | BindingFlags.Instance);
      FieldInfo skillsScreenscrollRectField = typeof(SkillsScreen).GetField("scrollRect", BindingFlags.NonPublic | BindingFlags.Instance);

      //Found AlarmSideScreen it contains KInputField prefab
      List<DetailsScreen.SideScreenRef> sideScreens = fieldInfoSideScreens.GetValue(DetailsScreen.Instance) as List<DetailsScreen.SideScreenRef>;
      if (sideScreens == null)
      {
        Debug.LogWarning("sideScreens is null, maybe game was updated");
        return false;
      }
      critterSensorSideScreenRef = null;
      DetailsScreen.SideScreenRef alarmSideScreenRef = null;
      DetailsScreen.SideScreenRef activeRangeSideScreenRef = null;
      DetailsScreen.SideScreenRef artableSelectionSideScreenRef = null;
      foreach (var sideScreenRef in sideScreens)
      {
        if (sideScreenRef.screenPrefab is AlarmSideScreen)
          alarmSideScreenRef = sideScreenRef;
        else if (sideScreenRef.screenPrefab is ActiveRangeSideScreen)
          activeRangeSideScreenRef = sideScreenRef;
        else if (sideScreenRef.screenPrefab is ArtableSelectionSideScreen)
          artableSelectionSideScreenRef = sideScreenRef;
        else if (sideScreenRef.screenPrefab is CritterSensorSideScreen)
          critterSensorSideScreenRef = sideScreenRef;
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
      if (critterSensorSideScreenRef == null)
      {
        Debug.LogWarning("critterSensorSideScreenRef NOT FOUND, maybe game was updated");
        return false;
      }
      SkillsScreen skillsScreen = skillsScreenlField.GetValue(ManagementMenu.Instance) as SkillsScreen;
      if (skillsScreen == null)
      {
        Debug.LogWarning("ManagementMenu.Instance.skillsScreen NOT FOUND, maybe game was updated");
        return false;
      }

      var optionsScreen = typeof(PauseScreen)
        .GetField("optionsScreen", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(PauseScreen.Instance) as OptionsMenuScreen;
      if (optionsScreen == null)
      {
        Debug.LogWarning("optionsScreen NOT FOUND, maybe game was updated");
        return false;
      }
      var graphicsOptionsScreenPrefab = typeof(OptionsMenuScreen)
        .GetField("graphicsOptionsScreenPrefab", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(optionsScreen) as KModalScreen;
      if (graphicsOptionsScreenPrefab == null)
      {
        Debug.LogWarning("graphicsOptionsScreenPrefab NOT FOUND, maybe game was updated");
        return false;
      }

      //Get prefabs
      KInputFieldPrefab = typeof(AlarmSideScreen).GetField("nameInputField", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(alarmSideScreenRef.screenPrefab) as KInputField;
      LocTextPrefab = typeof(ActiveRangeSideScreen).GetField("activateLabel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(activeRangeSideScreenRef.screenPrefab) as LocText;
      ButtonPrefab = Object.Instantiate((artableSelectionSideScreenRef.screenPrefab as ArtableSelectionSideScreen).applyButton.gameObject).AddComponent<FButton>();
      KScrollRectPrefab = skillsScreenscrollRectField.GetValue(skillsScreen) as KScrollRect;
      ScrollbarPrefab = DetailsScreen.Instance.transform.Find("Body/Scrollbar")?.gameObject.GetComponent<Scrollbar>();
      CheckboxPrefab = critterSensorSideScreenRef.screenPrefab.transform.Find("Contents/CheckboxGroup")?.gameObject.AddComponent<FCheckbox>();
      LabelPrefab = critterSensorSideScreenRef.screenPrefab.transform.Find("Contents/CheckboxGroup/Label")?.gameObject.AddComponent<FLabel>();
      DropdownPrefab = Assembly.GetAssembly(typeof(AlarmSideScreen)).GetType("GraphicsOptionsScreen")
        .GetField("resolutionDropdown", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(graphicsOptionsScreenPrefab) as Dropdown;
      KSliderPrefab = typeof(ActiveRangeSideScreen).GetField("activateValueSlider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(activeRangeSideScreenRef.screenPrefab) as KSlider;
      KNumberInputFieldPrefab = typeof(ActiveRangeSideScreen).GetField("activateValueLabel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(activeRangeSideScreenRef.screenPrefab) as KNumberInputField;

      if (CheckboxPrefab == null)
      {
        Debug.Log("CheckboxPrefab is null, maybe game was updated");
        return false;
      }
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
      if (ButtonPrefab == null)
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
      if (DropdownPrefab == null)
      {
        Debug.LogWarning("DropdownPrefab is null, maybe game was updated");
        return false;
      }
      if (KSliderPrefab == null)
      {
        Debug.LogWarning("KSliderPrefab is null, maybe game was updated");
        return false;
      }
      if (KNumberInputFieldPrefab == null)
      {
        Debug.LogWarning("KNumberInputFieldPrefab is null, maybe game was updated");
        return false;
      }
      ButtonPrefab.kButton = ButtonPrefab.GetComponent<KButton>();
      return true;
    }

    public delegate void AddLayoutMakeUI<T>(RectTransform parent, T self);
    public delegate void AddLayoutApplyComponent<T>(T self);

    public static T AddSideScreen<T>(AddLayoutMakeUI<T> makeUICb, string titleKey = "") where T : SideScreenContent
    {
      FieldInfo fieldInfoSideScreens = typeof(DetailsScreen).GetField("sideScreens", BindingFlags.NonPublic | BindingFlags.Instance);
      List<DetailsScreen.SideScreenRef> sideScreens = fieldInfoSideScreens.GetValue(DetailsScreen.Instance) as List<DetailsScreen.SideScreenRef>;

      var newScreenObject = Object.Instantiate(critterSensorSideScreenRef.screenPrefab.gameObject);
      newScreenObject.name = typeof(T).Name;

      var oldScreen = newScreenObject.GetComponent<CritterSensorSideScreen>();
      Object.Destroy(oldScreen);

      var newScreen = newScreenObject.AddComponent<T>();

      var content = newScreenObject.transform.Find("Contents") as RectTransform;
      for (int i = content.transform.childCount - 1; i >= 0; i--)
      {
        var item = content.transform.GetChild(i).gameObject;
        if (item.name == "CheckboxGroup")
          item.SetActive(false);
      }

      makeUICb(content, newScreen);

      var titleKeyField = typeof(SideScreenContent).GetField("titleKey", BindingFlags.Instance | BindingFlags.NonPublic);
      titleKeyField.SetValue(newScreen, titleKey);

      sideScreens.Add(new DetailsScreen.SideScreenRef()
      {
        name = newScreenObject.name,
        screenPrefab = newScreen,
        offset = Vector2.zero,
      });

      return newScreen;
    }
    public static void ApplyLayout(RectTransform rect, FLayoutOptions layout)
    {
      if (layout == null)
        FLayoutOptions.DEFAULT.ApplyRectTaransform(rect);
      else
        layout.ApplyRectTaransform(rect);
    }

    public static HorizontalLayoutGroup AddHorzontalLayout(RectTransform parent, FLayoutOptions layout, AddLayoutMakeUI<HorizontalLayoutGroup> makeUICb)
    {
      var group = new GameObject();
      var groupRectTransform = group.AddComponent<RectTransform>();
      var groupHorizontalLayoutGroup = group.AddComponent<HorizontalLayoutGroup>();
      groupRectTransform.SetParent(parent);
      ApplyLayout(group.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(groupRectTransform, groupHorizontalLayoutGroup);
      return groupHorizontalLayoutGroup;
    }
    public static VerticalLayoutGroup AddVerticalLayout(RectTransform parent, FLayoutOptions layout, AddLayoutMakeUI<VerticalLayoutGroup> makeUICb)
    {
      var group = new GameObject();
      var groupRectTransform = group.AddComponent<RectTransform>();
      var groupVerticalLayoutGroup = group.AddComponent<VerticalLayoutGroup>();
      groupRectTransform.SetParent(parent);
      ApplyLayout(group.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(groupRectTransform, groupVerticalLayoutGroup);
      return groupVerticalLayoutGroup;
    }

    public static FCheckbox AddCheckbox(RectTransform parent, FLayoutOptions layout = null, string LabelKey = "", string LabelText = "", string TooltipKey = "", string TooltipText = "", AddLayoutApplyComponent<FCheckbox> makeUICb = null)
    {
      var check = Object.Instantiate(CheckboxPrefab, parent).GetComponent<FCheckbox>();
      check.LabelKey = LabelKey;
      check.LabelText = LabelText;
      ApplyLayout(check.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(check);
      if (TooltipKey != "" || TooltipText != "")
      {
        var toolTip = check.gameObject.GetComponent<ToolTip>();
        if (toolTip == null)
          toolTip = check.gameObject.AddComponent<ToolTip>();
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

      }
      return check;
    }
    public static FLabel AddLabel(RectTransform parent, FLayoutOptions layout = null, string LabelKey = "", string LabelText = "", Color LabelColor = default(Color), TextAlignmentOptions LabelAlignment = TextAlignmentOptions.MidlineLeft, AddLayoutApplyComponent<FLabel> makeUICb = null)
    {
      var label = Object.Instantiate(LabelPrefab, parent).GetComponent<FLabel>();
      label.LabelKey = LabelKey;
      label.LabelText = LabelText;
      label.Label = label.gameObject.GetComponent<LocText>();
      label.Label.alignment = LabelAlignment;
      label.Label.color = LabelColor;
      ApplyLayout(label.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(label);
      return label;
    }
    public static FButton AddButton(
      RectTransform parent, FLayoutOptions layout = null, 
      string LabelKey = "", string LabelText = "", string TooltipText = "", string TooltipKey = "",
      Sprite Icon = null, Vector2 IconSize = default,
      AddLayoutApplyComponent<FButton> makeUICb = null
    )
    {
      var button = Object.Instantiate(ButtonPrefab, parent).GetComponent<FButton>();
      button.LabelKey = LabelKey;
      button.LabelText = LabelText;
      button.TooltipText = TooltipText;
      button.TooltipKey = TooltipKey;
      button.Icon = Icon;
      button.IconSize = IconSize;
      ApplyLayout(button.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(button);
      return button;
    }
    public static KInputField AddInputField(
      RectTransform parent, FLayoutOptions layout = null,
      AddLayoutApplyComponent<KInputField> makeUICb = null
    )
    {
      var input = Object.Instantiate(KInputFieldPrefab.gameObject, parent).GetComponent<KInputField>();
      ApplyLayout(input.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(input);
      return input;
    }

    public static KNumberInputField AddNumberInputField(
      RectTransform parent, FLayoutOptions layout = null,
      float currentValue = 0, float minValue = 0, float maxValue = 100,
      AddLayoutApplyComponent<KNumberInputField> makeUICb = null
    )
    {
      var input = Object.Instantiate(KNumberInputFieldPrefab.gameObject, parent).GetComponent<KNumberInputField>();
      input.minValue = minValue;
      input.maxValue = maxValue;
      input.currentValue = currentValue;
      ApplyLayout(input.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(input);
      return input;
    }
    public static KSlider AddSlider(
      RectTransform parent, FLayoutOptions layout = null,
      float currentValue = 0, float minValue = 0, float maxValue = 100,
      AddLayoutApplyComponent<KSlider> makeUICb = null
    )
    {
      var input = Object.Instantiate(KSliderPrefab.gameObject, parent).GetComponent<KSlider>();
      input.minValue = minValue;
      input.maxValue = maxValue;
      input.value = currentValue;
      ApplyLayout(input.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(input);
      return input;
    }
    public static Dropdown AddDropdown(
      RectTransform parent, FLayoutOptions layout = null,
      AddLayoutApplyComponent<Dropdown> makeUICb = null
    )
    {
      var dropdown = Object.Instantiate(DropdownPrefab.gameObject, parent).GetComponent<Dropdown>();
      dropdown.options.Clear();
      ApplyLayout(dropdown.transform as RectTransform, layout);
      if (makeUICb != null)
        makeUICb(dropdown);
      return dropdown;
    }

    public static RectTransform AddSpace(RectTransform parent, FLayoutOptions layout = null)
    {
      var group = new GameObject();
      var groupRectTransform = group.AddComponent<RectTransform>();
      groupRectTransform.SetParent(parent);
      ApplyLayout(groupRectTransform, layout);
      return groupRectTransform;
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
      var LineText = Object.Instantiate(LocTextPrefab.gameObject, parent);
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
        button.soundPlayer = ButtonPrefab.kButton.soundPlayer;
        button.onClick += click;
      }

      return LineTextText;
    }
    public static FButton AddButtonLine(string str, string key, string tooltip, float x, float y, RectTransform parent, System.Action click, float width = 100, float height = 40)
    {
      var StopButton = Object.Instantiate(ButtonPrefab.gameObject, parent);
      var StopButtonButtonRectTransform = StopButton.GetComponent<RectTransform>();
      var StopButtonButton = StopButton.GetComponent<FButton>();
      StopButtonButton.LabelKey = key;
      StopButtonButton.LabelText = str;
      StopButtonButton.TooltipKey = tooltip;
      StopButton.GetComponent<ToolTip>().SetFixedStringKey(tooltip);
      if (click != null)
        StopButtonButton.OnClick += click;
      UIAnchorPosUtils.SetUIPivot(StopButtonButtonRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(StopButtonButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(StopButtonButtonRectTransform, x);
      StopButtonButtonRectTransform.sizeDelta = new Vector2(width, height);
      StopButtonButtonRectTransform.anchoredPosition = new Vector2(StopButtonButtonRectTransform.anchoredPosition.x, y);
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
