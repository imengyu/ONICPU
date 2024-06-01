using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DetailsScreen;

namespace ONICPU
{
  public class FCPUEditorUI : MonoBehaviour
  {
    const int MAX_PIN = 8;
    const int MAX_REGUSTER = 8;

    public KInputField ProgramInputField;
    public LocText ProgramStatusText;
    public FCPUExecutor executor;

    private RectTransform ProgramInputLineTexts;
    private RectTransform ProgramStatusLines;
    private KButton StepButton;
    private KButton PlayPauseButton;
    private bool componentGetted = false;
    private int activeLine = -1;
    private bool[] breakpointState = new bool[FCPUExecutor.MAX_LINE];
    private GameObject[] breakpointPoints = new GameObject[FCPUExecutor.MAX_LINE];
    private LocText[] lineTexts = new LocText[FCPUExecutor.MAX_LINE];
    private LocText[] registerValueTexts = null;
    private LocText[] inputValueTexts = null;
    private LocText[] outputValueTexts = null;
    private LocText[] memoryValueTexts = null;
    private LocText[] logTexts = null;

    private static Color TextNormalColor = Color.white;
    private static Color TextErrorColor = new Color(0.7f, 0.3f, 0.3f);
    private static Color TextActiveColor = new Color(0.35f, 0.93f, 0.93f);

    private StringBuilder stringBuilder = new StringBuilder();
    private int tick = 0;
    private bool startNeedFlushInfo = true;

    void Start()
    {
      GetComponentInstances();
    }
    void Update()
    {
      if (executor != null && (executor.State == FCPUState.Looping || startNeedFlushInfo))
      {
        if (tick < 60)
          tick++;
        else
        {
          startNeedFlushInfo = false;
          FlushInfo();
          tick = 0;
        }
      }
    }

    //Public methods
    //=====================================

    public void ToggleBreakpointStateLine(int line)
    {
      SetBreakpointStateLine(line, !breakpointState[line]);
    }
    public void SetBreakpointStateLine(int line, bool state)
    {
      breakpointState[line] = state;
      breakpointPoints[line].SetActive(state);
    }
    public void SetActiveLine(int line)
    {
      if (activeLine == line) return;
      if (activeLine >= 0)
      {
        if (lineTexts[activeLine] != null)
        {
          lineTexts[activeLine].color = TextNormalColor;
          lineTexts[activeLine].text = activeLine.ToString();
        }
      }
      activeLine = line;
      if (lineTexts[activeLine] != null)
      {
        lineTexts[activeLine].color = TextActiveColor;
        lineTexts[activeLine].text = "▶ " + activeLine;
      }
    }
    public void SetProgramStatus(string programStatus, bool error)
    {
      if (programStatus == null)
        programStatus = "";
      ProgramStatusText.text = programStatus;
      ProgramStatusText.color = error ? TextErrorColor : TextNormalColor;
    }
    public void SetValues(string program, string programStatus, string breakpointStateStr)
    {
      if (program == null)
        program = "";
      if (programStatus == null)
        programStatus = "";
      if (breakpointStateStr == null)
        breakpointStateStr = "";

      GetComponentInstances();

      ProgramInputField.SetDisplayValue(program);
      ProgramStatusText.text = programStatus;
      KScreenManager.Instance.RefreshStack();

      var strs = breakpointStateStr.Split(',');
      int i;
      for (i = 0; i < FCPUExecutor.MAX_LINE; i++)
        breakpointState[i] = false;
      i = 0;
      foreach (string str in strs)
      {
        if (i >= breakpointState.Length)
          break;
        breakpointState[i] = str == "1";
        i++;
      }

      for (i = 0; i < FCPUExecutor.MAX_LINE; i++)
        breakpointPoints[i].SetActive(breakpointState[i]);
    }
    public string GetBreakpointStateStr()
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < breakpointState.Length; i++)
        sb.Append(breakpointState[i] ? "1" : "0");
      return sb.ToString();
    }
    public string GetProgram()
    {
      return ProgramInputField.field.text;
    }
    public void Copy()
    {
      GUIUtility.systemCopyBuffer = GetProgram();
    }
    public void Patse()
    {
      ProgramInputField.SetDisplayValue(GUIUtility.systemCopyBuffer);
      KScreenManager.Instance.RefreshStack();
    }
    public void FlushInfo()
    {
      if (executor != null)
      {
        if (executor.GetType() == typeof(FCPUExecutorAssemblyCode))
        {
          var accessable = (executor as FCPUExecutorAssemblyCode).Accessables;
          for (int i = 0; i < accessable.Register.Length; i++)
          {
            registerValueTexts[i].text = $"reg{i} = {accessable.Register[i]}";
            registerValueTexts[i].enabled = true;
          }

          for (int i = 0, j = 0; i < accessable.StackAndHeap.Length / 4; i++)
          {
            stringBuilder.Clear();
            stringBuilder.Append($"{i} ");
            for (int a = 0; a < 4; a++, j++)
            {
              stringBuilder.Append(" ");
              stringBuilder.Append(string.Format("{0:D10}", accessable.StackAndHeap[j]));
            }
            memoryValueTexts[i].text = stringBuilder.ToString();
            memoryValueTexts[i].enabled = true;
          }
        }

        int k = 0;
        for (int i = 0; i < executor.InputOutput.InputValues.Length; i++, k++)
          inputValueTexts[i].text = $"P{k} = {executor.InputOutput.Read(k)}";
        for (int i = 0; i < executor.InputOutput.OutputValues.Length; i++, k++)
          outputValueTexts[i].text = $"P{k} = {executor.InputOutput.Read(k)}";

        if (executor.GetType() == typeof(FCPUExecutorJavaScript))
        {
          for (int i = 0; i < logTexts.Length && i < ((FCPUExecutorJavaScript)executor).Logs.Count; i++)
            logTexts[i].text = ((FCPUExecutorJavaScript)executor).Logs[i];
        }

        PlayPauseButton.fgImage.sprite = executor.State == FCPUState.Looping ? SpritePause : SpritePlay;
      }
    }


    //Get prefab and make ui
    //=====================================

    public static FCPUEditorUI ProgramEditorPanelPrefab = null;
    public static Sprite SpriteStop = null;
    public static Sprite SpriteBreakpoint = null;
    public static Sprite SpriteNext = null;
    public static Sprite SpritePause = null;
    public static Sprite SpritePlay = null;
    public static KInputField KInputFieldPrefab = null;
    public static LocText LocTextPrefab = null;
    public static KButton KButtonPrefab = null;
    public static KScrollRect KScrollRectPrefab = null;
    public static Scrollbar ScrollbarPrefab = null;

    private const int padding = 12;
    private const int width = 700;
    private const int leftWidth = 200;
    private const int leftWidth2 = 500;
    private const int rightWidth2 = width - leftWidth2;
    private const int height = 600;
    private const int buttonBarHeight = 40;
    private const int allHeight = 6400;
    private const int lineCodeWidth = 40;
    private const float lineHeight = 20.74f;

    public static void MakeEditorUI()
    {
      if (ProgramEditorPanelPrefab != null)
        return;

      Debug.Log("Load sprite");

      var assetsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/assets/sprite/";
      SpriteBreakpoint = Utils.LoadSpriteFromFile(assetsPath + "breakpoint.png", 32, 32);
      SpriteNext = Utils.LoadSpriteFromFile(assetsPath + "next.png", 32, 32);
      SpritePause = Utils.LoadSpriteFromFile(assetsPath + "pause.png", 32, 32);
      SpritePlay = Utils.LoadSpriteFromFile(assetsPath + "play.png", 32, 32);
      SpriteStop = Utils.LoadSpriteFromFile(assetsPath + "stop.png", 32, 32);

      var ProgramEditorPanel = new GameObject("ProgramEditorPanel");
      var RectTransform = ProgramEditorPanel.AddComponent<RectTransform>();
      UIAnchorPosUtils.SetUIAnchor(RectTransform, UIAnchor.Stretch, UIAnchor.Top);

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
        Debug.LogError("sideScreens is null, maybe game was updated");
        return;
      }
      SideScreenRef alarmSideScreenRef = null;
      SideScreenRef activeRangeSideScreenRef = null;
      SideScreenRef artableSelectionSideScreenRef = null;
      foreach (var sideScreenRef in sideScreens)
      {
        if (sideScreenRef.screenPrefab is AlarmSideScreen)
        {
          alarmSideScreenRef = sideScreenRef;
          Debug.Log("Found AlarmSideScreen");
        }
        else if (sideScreenRef.screenPrefab is ActiveRangeSideScreen)
        {
          activeRangeSideScreenRef = sideScreenRef;
          Debug.Log("Found ActiveRangeSideScreen");
        }
        else if (sideScreenRef.screenPrefab is ArtableSelectionSideScreen)
        {
          artableSelectionSideScreenRef = sideScreenRef;
          Debug.Log("Found ArtableSelectionSideScreenRef");
        }
      }
      if (alarmSideScreenRef == null)
      {
        Debug.LogError("AlarmSideScreen NOT FOUND, maybe game was updated");
        return;
      }
      if (activeRangeSideScreenRef == null)
      {
        Debug.LogError("activeRangeSideScreen NOT FOUND, maybe game was updated");
        return;
      }
      if (artableSelectionSideScreenRef == null)
      {
        Debug.LogError("artableSelectionSideScreenRef NOT FOUND, maybe game was updated");
        return;
      }
      SkillsScreen skillsScreen = skillsScreenlField.GetValue(ManagementMenu.Instance) as SkillsScreen;
      if (skillsScreen == null)
      {
        Debug.LogError("ManagementMenu.Instance.skillsScreen NOT FOUND, maybe game was updated");
        return;
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
        Debug.LogError("KInputFieldPrefab is null, maybe game was updated");
        return;
      }
      if (LocTextPrefab == null)
      {
        Debug.LogError("LocTextPrefab is null, maybe game was updated");
        return;
      }
      if (KButtonPrefab == null)
      {
        Debug.LogError("ButtonPrefab is null, maybe game was updated");
        return;
      }
      if (KScrollRectPrefab == null)
      {
        Debug.LogError("ScrollRectPrefab is null, maybe game was updated");
        return;
      }
      if (ScrollbarPrefab == null)
      {
        Debug.LogError("ScrollbarPrefab is null, maybe game was updated");
        return;
      }

      Debug.Log("Create UI");

      //Create UI

      //- ProgramStatusText
      //- ButtonBar
      //  - StopButton
      //  - PlayPauseButton
      //  - StepButton
      //  - ResetButton
      //- ScrollRect
      //  - ScrollRectContent
      //    - ProgramInputField
      //    - ProgramInputLineTexts
      //      - Line{x}
      //      - BreakPoint{x}
      //- Scrollbar

      //Top

      var ProgramStatusText = UnityEngine.Object.Instantiate(LocTextPrefab.gameObject, RectTransform);
      var ProgramStatusTextRectTransform = ProgramStatusText.GetComponent<RectTransform>();
      var ProgramStatusTextText = ProgramStatusText.GetComponent<LocText>();
      ProgramStatusText.name = "ProgramStatusText";
      ProgramStatusTextRectTransform.sizeDelta = new Vector2(leftWidth2, 90);
      UIAnchorPosUtils.SetUIPivot(ProgramStatusTextRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ProgramStatusTextRectTransform, UIAnchor.Stretch, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(ProgramStatusTextRectTransform, padding + 40);
      UIAnchorPosUtils.SetUIRight(ProgramStatusTextRectTransform, padding);
      UIAnchorPosUtils.SetUITop(ProgramStatusTextRectTransform, padding);
      ProgramStatusTextText.color = TextNormalColor;
      ProgramStatusTextText.alignment = TextAlignmentOptions.TopLeft;

      //Debug.Log("1");

      var ButtonBar = new GameObject("ButtonBar");
      var ButtonBarRectTransform = ButtonBar.AddComponent<RectTransform>();
      ButtonBarRectTransform.sizeDelta = new Vector2(width, buttonBarHeight);
      ButtonBarRectTransform.SetParent(RectTransform);
      UIAnchorPosUtils.SetUIPivot(ButtonBarRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ButtonBarRectTransform, UIAnchor.Stretch, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(ButtonBarRectTransform, padding);
      UIAnchorPosUtils.SetUIRight(ButtonBarRectTransform, padding);
      UIAnchorPosUtils.SetUITop(ButtonBarRectTransform, padding);

      //Debug.Log("2");

      var EmptyImage = new GameObject("Image");
      var EmptyImageRectTransform = EmptyImage.AddComponent<RectTransform>();
      var EmptyImageImage = EmptyImage.AddComponent<Image>();

      var StopButton = Instantiate(KButtonPrefab.gameObject, ButtonBarRectTransform);
      EmptyImageRectTransform.SetParent(StopButton.transform);
      EmptyImageRectTransform.sizeDelta = new Vector2(23, 23);
      EmptyImageRectTransform.anchoredPosition = new Vector2(0, 0);
      Destroy(StopButton.transform.GetChild(0).gameObject);

      var StopButtonButton = StopButton.GetComponent<KButton>();
      var StopButtonButtonRectTransform = StopButton.GetComponent<RectTransform>();
      StopButtonButton.GetComponent<ToolTip>().FixedStringKey = "STRINGS.UI.UISIDESCREENS.FCPU.STOP_BUTTON_TOOLTIP";
      StopButton.name = "StopButton";
      StopButtonButton.fgImage = EmptyImageImage;
      StopButtonButton.fgImage.sprite = SpriteStop;
      UIAnchorPosUtils.SetUIPivot(StopButtonButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(StopButtonButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      StopButtonButtonRectTransform.sizeDelta = new Vector2(32, 32);
      StopButtonButtonRectTransform.anchoredPosition = new Vector2(width - 84, 0);

      var ResetButton = Instantiate(StopButton, ButtonBarRectTransform);
      var ResetButtonButton = ResetButton.GetComponent<KButton>();
      var ResetButtonRectTransform = ResetButton.GetComponent<RectTransform>();
      ResetButton.GetComponent<ToolTip>().FixedStringKey = "STRINGS.UI.UISIDESCREENS.FCPU.RESET_BUTTON_TOOLTIP";
      ResetButton.name = "ResetButton";
      ResetButtonButton.fgImage.sprite = SpriteBreakpoint;
      UIAnchorPosUtils.SetUIPivot(ResetButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(ResetButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      ResetButtonRectTransform.sizeDelta = new Vector2(32, 32);
      ResetButtonRectTransform.anchoredPosition = new Vector2(width - 120, 0);

      var PlayPauseButton = Instantiate(StopButton, ButtonBarRectTransform);
      var PlayPauseButtonButton = PlayPauseButton.GetComponent<KButton>();
      var PlayPauseButtonRectTransform = PlayPauseButton.GetComponent<RectTransform>();
      PlayPauseButton.GetComponent<ToolTip>().FixedStringKey = "STRINGS.UI.UISIDESCREENS.FCPU.PLAYPAUSE_BUTTON_TOOLTIP";
      PlayPauseButton.name = "PlayPauseButton";
      PlayPauseButtonButton.fgImage.sprite = SpritePlay;
      UIAnchorPosUtils.SetUIPivot(PlayPauseButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(PlayPauseButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      PlayPauseButtonRectTransform.sizeDelta = new Vector2(32, 32);
      PlayPauseButtonRectTransform.anchoredPosition = new Vector2(width - 52, 0);

      var StepButtonButton = Instantiate(StopButton, ButtonBarRectTransform);
      var StepButtonButtonButton = StepButtonButton.GetComponent<KButton>();
      var StepButtonButtonRectTransform = StepButtonButton.GetComponent<RectTransform>();
      StepButtonButton.GetComponent<ToolTip>().FixedStringKey = "STRINGS.UI.UISIDESCREENS.FCPU.STEP_BUTTON_TOOLTIP";
      StepButtonButton.name = "StepButton";
      StepButtonButtonButton.fgImage.sprite = SpriteNext;
      UIAnchorPosUtils.SetUIPivot(StepButtonButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(StepButtonButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      StepButtonButtonRectTransform.sizeDelta = new Vector2(32, 32);
      StepButtonButtonRectTransform.anchoredPosition = new Vector2(width - 20, 0);

      //Debug.Log("3");

      //Editor area

      var ScrollRect = UnityEngine.Object.Instantiate(KScrollRectPrefab.gameObject, RectTransform);
      var ScrollRectTransform = ScrollRect.GetComponent<RectTransform>();
      var ScrollRectScrollRect = ScrollRect.GetComponent<KScrollRect>();
      ScrollRect.name = "ScrollRect";
      UIAnchorPosUtils.SetUIPivot(ScrollRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ScrollRectTransform, UIAnchor.Stretch, UIAnchor.Stretch);
      UIAnchorPosUtils.SetUILeft(ScrollRectTransform, -50);
      UIAnchorPosUtils.SetUIRight(ScrollRectTransform, padding);
      UIAnchorPosUtils.SetUIBottom(ScrollRectTransform, padding);
      UIAnchorPosUtils.SetUITop(ScrollRectTransform, 80);

      ScrollRectScrollRect.vertical = true;
      ScrollRectScrollRect.content.gameObject.name = "ScrollRectContent";

      var ScrollRectContent = new GameObject("Panel");
      var ScrollRectContentRectTransform = ScrollRectContent.AddComponent<RectTransform>();
      UIAnchorPosUtils.SetUIAnchor(ScrollRectContentRectTransform, UIAnchor.Stretch, UIAnchor.Top);
      ScrollRectContentRectTransform.SetParent(ScrollRectScrollRect.content);
      ScrollRectContentRectTransform.sizeDelta = new Vector2(width, allHeight);
      var ScrollRectContentLayoutElement = ScrollRectContent.AddComponent<LayoutElement>();
      ScrollRectContentLayoutElement.minHeight = allHeight;
      ScrollRectContentLayoutElement.minWidth = width;
      ScrollRectContentLayoutElement.preferredWidth = width;
      ScrollRectContentLayoutElement.preferredHeight = allHeight;
      ScrollRectContentLayoutElement.flexibleHeight = 0f;

      //Debug.Log("4");

      var ProgramInputField = Instantiate(KInputFieldPrefab.gameObject, ScrollRectContentRectTransform);
      var ProgramInputFieldInputField = ProgramInputField.GetComponent<KInputField>();
      var ProgramInputFieldRectTransform = ProgramInputField.GetComponent<RectTransform>();
      ProgramInputField.name = "ProgramInputField";
      UIAnchorPosUtils.SetUIPivot(ProgramInputFieldRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ProgramInputFieldRectTransform, UIAnchor.Left, UIAnchor.Stretch);
      ProgramInputFieldRectTransform.sizeDelta = new Vector2(leftWidth, allHeight);
      ProgramInputFieldRectTransform.anchoredPosition = new Vector2(lineCodeWidth, 0);
      ProgramInputFieldInputField.field.lineType = TMP_InputField.LineType.MultiLineNewline;
      ProgramInputFieldInputField.field.characterLimit = 2048;
      ProgramInputFieldInputField.field.textComponent.alignment = TextAlignmentOptions.TopLeft;

      var ProgramInputLineText = new GameObject("ProgramInputLineTexts");
      var ProgramInputLineTextRectTransform = ProgramInputLineText.AddComponent<RectTransform>();
      ProgramInputLineTextRectTransform.SetParent(ScrollRectContentRectTransform);
      UIAnchorPosUtils.SetUIPivot(ProgramInputLineTextRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ProgramInputLineTextRectTransform, UIAnchor.Left, UIAnchor.Stretch);
      ProgramInputLineTextRectTransform.sizeDelta = new Vector2(lineCodeWidth, allHeight);
      ProgramInputLineTextRectTransform.anchoredPosition = new Vector2(0, 0);

      //Debug.Log("5");

      float y = 0;
      for (int i = 0; i < 300; i++)
      {
        AddTextLine((i + 1).ToString(), $"Line{i}", lineCodeWidth, lineHeight, 0, -y, ProgramInputLineTextRectTransform);
        AddImageLine(SpriteBreakpoint, $"BreakPoint{i}", lineHeight, lineHeight, -y, ProgramInputLineTextRectTransform);
        y += lineHeight;
      }

      //Debug.Log("6");

      var ProgramStatusLines = new GameObject("ProgramStatusLines");
      var ProgramStatusLinesRectTransform = ProgramStatusLines.AddComponent<RectTransform>();
      ProgramStatusLinesRectTransform.SetParent(ScrollRectContentRectTransform);
      UIAnchorPosUtils.SetUIPivot(ProgramStatusLinesRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ProgramStatusLinesRectTransform, UIAnchor.Left, UIAnchor.Stretch);
      ProgramStatusLinesRectTransform.sizeDelta = new Vector2(width - leftWidth - 60, allHeight);
      ProgramStatusLinesRectTransform.anchoredPosition = new Vector2(leftWidth + 60, 0);

      ProgramEditorPanelPrefab = ProgramEditorPanel.AddComponent<FCPUEditorUI>();
      var ProgramEditorPanelPrefabLayoutElement = ProgramEditorPanel.AddComponent<LayoutElement>();
      ProgramEditorPanelPrefabLayoutElement.minHeight = height;
      ProgramEditorPanelPrefabLayoutElement.minWidth = width;
      ProgramEditorPanelPrefabLayoutElement.preferredWidth = width;
      ProgramEditorPanelPrefabLayoutElement.preferredHeight = height;
      ProgramEditorPanelPrefabLayoutElement.flexibleHeight = 0f;

      var Scrollbar = Instantiate(ScrollbarPrefab.gameObject, RectTransform);
      var ScrollbarRectTransform = Scrollbar.GetComponent<RectTransform>();
      Scrollbar.name = "Scrollbar";
      UIAnchorPosUtils.SetUIPivot(ScrollbarRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(ScrollbarRectTransform, UIAnchor.Right, UIAnchor.Stretch);
      UIAnchorPosUtils.SetUIRight(ScrollbarRectTransform, -5);
      UIAnchorPosUtils.SetUITop(ScrollbarRectTransform, padding);
      UIAnchorPosUtils.SetUIBottom(ScrollbarRectTransform, padding);
    }

    private void GetComponentInstances()
    {
      if (componentGetted)
        return;

      ProgramInputField = transform.Find("ScrollRect/ScrollRectContent/Panel/ProgramInputField").GetComponent<KInputField>();
      ProgramStatusLines = transform.Find("ScrollRect/ScrollRectContent/Panel/ProgramStatusLines").GetComponent<RectTransform>();
      ProgramInputLineTexts = transform.Find("ScrollRect/ScrollRectContent/Panel/ProgramInputLineTexts").GetComponent<RectTransform>();
      ProgramStatusText = transform.Find("ProgramStatusText").GetComponent<LocText>();
      var ProgramEditorScrollbar = transform.Find("Scrollbar").GetComponent<Scrollbar>();
      var ScrollRect = transform.Find("ScrollRect").GetComponent<ScrollRect>();
      for (int i = 0; i < FCPUExecutor.MAX_LINE; i++)
      {
        var line = i;
        breakpointPoints[line] = ProgramInputLineTexts.Find($"BreakPoint{line}").gameObject;

        var lineText = ProgramInputLineTexts.Find($"Line{line}");
        if (lineText != null)
        {
          lineTexts[line] = lineText.GetComponent<LocText>();
          var button = lineText.gameObject.AddComponent<KButton>();
          button.soundPlayer = KButtonPrefab.soundPlayer;
          button.onPointerUp += () =>
          {
            Debug.Log("ToggleBreakpointStateLine " + line);
            ToggleBreakpointStateLine(line);
          };
        }
      }


      transform.Find("ButtonBar/StopButton").GetComponent<KButton>().onBtnClick += (KKeyCode obj) =>
      {
        onStopButtonClick.Invoke();
        FlushInfo();
      };
      transform.Find("ButtonBar/ResetButton").GetComponent<KButton>().onBtnClick += (KKeyCode obj) =>
      {
        onResetButtonClick.Invoke();
        FlushInfo();
      };
      PlayPauseButton = transform.Find("ButtonBar/PlayPauseButton").GetComponent<KButton>();
      PlayPauseButton.onBtnClick += (KKeyCode obj) =>
      {
        onPlayPauseButtonClick.Invoke();
        FlushInfo();
      };
      StepButton = transform.Find("ButtonBar/StepButton").GetComponent<KButton>();
      StepButton.onBtnClick += (KKeyCode obj) =>
      {
        onStepButtonClick.Invoke();
        FlushInfo();
      };

      ScrollRect.verticalScrollbar = ProgramEditorScrollbar;

      MakeInfoUI();
      componentGetted = true;
    }

    public event System.Action onStopButtonClick;
    public event System.Action onResetButtonClick;
    public event System.Action onPlayPauseButtonClick;
    public event System.Action onStepButtonClick;

    private void MakeInfoUI()
    {
      var y = 0;
      const int STATUS_WIDTH = 400;
      const int STATUS_WIDTH3 = STATUS_WIDTH / 3;

      if (executor != null)
      {
        var isAssemblyCodCpu = executor.GetType() == typeof(FCPUExecutorAssemblyCode);
        if (isAssemblyCodCpu)
        {
          var accessable = (executor as FCPUExecutorAssemblyCode).Accessables;
          registerValueTexts = new LocText[accessable.Register.Length];
          memoryValueTexts = new LocText[accessable.StackAndHeap.Length];
        }
        else
        {
          registerValueTexts = new LocText[0];
          memoryValueTexts = new LocText[0];
          logTexts = new LocText[16];
          ProgramInputField.rectTransform().sizeDelta = new Vector2(leftWidth2, allHeight);
          StepButton.gameObject.SetActive(false);
        }
        inputValueTexts = new LocText[executor.InputOutput.InputValues.Length];
        outputValueTexts = new LocText[executor.InputOutput.OutputValues.Length];


        int startY = y;
        if (isAssemblyCodCpu)
        {
          AddTextLine("Registers", "Text", STATUS_WIDTH3, 20, 0, -y, ProgramStatusLines, 16); y += 20;
          for (int i = 0; i < registerValueTexts.Length; i++)
          {
            registerValueTexts[i] = AddTextLine($"reg{i} = ?", $"RegisterValue{i}", STATUS_WIDTH3, 15, 0, -y, ProgramStatusLines); y += 15;
          }
          y = startY;
          AddTextLine("Inputs", "Text", STATUS_WIDTH3, 20, STATUS_WIDTH3, -y, ProgramStatusLines, 16); y += 20;
          for (int i = 0; i < inputValueTexts.Length; i++)
          {
            inputValueTexts[i] = AddTextLine("", $"InputValue{i}", STATUS_WIDTH3, 15, STATUS_WIDTH3, -y, ProgramStatusLines); y += 15;
          }
          y = startY;
          AddTextLine("Outputs", "Text", STATUS_WIDTH3, 20, STATUS_WIDTH3 * 2, -y, ProgramStatusLines, 16); y += 20;
          for (int i = 0; i < outputValueTexts.Length; i++)
          {
            outputValueTexts[i] = AddTextLine("", $"OutputValue{i}", STATUS_WIDTH3, 15, STATUS_WIDTH3 * 2, -y, ProgramStatusLines); y += 15;
          }
        } 
        else
        {
          AddTextLine("Inputs", "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, 16); y += 20;
          for (int i = 0; i < inputValueTexts.Length; i++)
          {
            inputValueTexts[i] = AddTextLine("", $"InputValue{i}", rightWidth2, 15, leftWidth, -y, ProgramStatusLines); y += 15;
          }
          y += 20;
          AddTextLine("Outputs", "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, 16); y += 20;
          for (int i = 0; i < outputValueTexts.Length; i++)
          {
            outputValueTexts[i] = AddTextLine("", $"OutputValue{i}", rightWidth2, 15, leftWidth, -y, ProgramStatusLines); y += 15;
          }
          y += 20;
          AddTextLine("Logs", "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, 16); y += 20;
          for (int i = 0; i < logTexts.Length; i++)
          {
            logTexts[i] = AddTextLine("", $"LogTexts{i}", rightWidth2, 15, leftWidth, -y, ProgramStatusLines); y += 15;
          }
        }

        if (isAssemblyCodCpu)
        {
          y += 20;
          AddTextLine("Memory", "Text", STATUS_WIDTH, 20, 0, -y, ProgramStatusLines, 16); y += 20;
          AddTextLine("                   0                1                2                3                4", $"MemoryValue-1", STATUS_WIDTH, 15, 0, -y, ProgramStatusLines); y += 15;
          for (int i = 0; i < memoryValueTexts.Length / 4; i++)
          {
            memoryValueTexts[i] = AddTextLine($"{i}     0    0    0    0    0    0    0    0", $"MemoryValue{i}", STATUS_WIDTH, 15, 0, -y, ProgramStatusLines); y += 15;
          }
        }
      }
    }

    private static Image AddImageLine(Sprite sprite, string name, float width, float height, float y, RectTransform parent)
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
    private static LocText AddTextLine(string str, string name, float width, float height, float x, float y, RectTransform parent, float size = 13)
    {
      var LineText = Instantiate(LocTextPrefab.gameObject, parent);
      var LineTextRectTransform = LineText.GetComponent<RectTransform>();
      var LineTextText = LineText.GetComponent<LocText>();
      LineText.name = name;
      UIAnchorPosUtils.SetUIPivot(LineTextRectTransform, UIPivot.TopLeft);
      UIAnchorPosUtils.SetUIAnchor(LineTextRectTransform, UIAnchor.Left, UIAnchor.Top);
      UIAnchorPosUtils.SetUILeft(LineTextRectTransform, x);
      LineTextRectTransform.sizeDelta = new Vector2(width, height);
      LineTextRectTransform.anchoredPosition = new Vector2(LineTextRectTransform.anchoredPosition.x, y);
      LineTextText.color = TextNormalColor;
      LineTextText.alignment = TextAlignmentOptions.MidlineRight;
      LineTextText.text = str;
      LineTextText.fontSize = size;
      return LineTextText;
    }
  }
}
