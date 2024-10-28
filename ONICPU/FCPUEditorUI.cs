using ONICPU.ui;
using System.IO;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ONICPU
{
  public class FCPUEditorUI : MonoBehaviour
  {
    private const int MAX_LINE = 300;

    public KInputField ProgramInputField;
    public LocText ProgramStatusText;
    public FCPUExecutor executor;

    private RectTransform ProgramInputLineTexts;
    private RectTransform ProgramStatusLines;
    private FButton StepButton;
    private FButton PlayPauseButton;
    private bool componentGetted = false;
    private int activeLine = -1;
    private bool[] breakpointState = new bool[MAX_LINE];
    private GameObject[] breakpointPoints = new GameObject[MAX_LINE];
    private LocText[] lineTexts = new LocText[MAX_LINE];
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
    private string programPath = "";

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
      if (line >= 0 && line < lineTexts.Length && lineTexts[activeLine] != null)
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
    public void SetProgramPath(string _programPath)
    {
      programPath = _programPath;
      var dir = Path.GetDirectoryName(programPath);
      try
      {
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);
      } 
      catch (System.Exception e)
      {
        Debug.LogWarning(e);
      }
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
      for (i = 0; i < MAX_LINE; i++)
        breakpointState[i] = false;
      i = 0;
      foreach (string str in strs)
      {
        if (i >= breakpointState.Length)
          break;
        breakpointState[i] = str == "1";
        i++;
      }

      for (i = 0; i < MAX_LINE; i++)
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
          bool noEmpty = false;
          for (int i = 0; i < logTexts.Length && i < ((FCPUExecutorJavaScript)executor).Logs.Count; i++)
          {
            var text = ((FCPUExecutorJavaScript)executor).Logs[i];
            logTexts[i].text = text;
            if (!noEmpty && !string.IsNullOrEmpty(text))
              noEmpty = true;
          }
          if (!noEmpty)
            logTexts[0].text = Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.NO_LOGS");
        }

        PlayPauseButton.Icon = executor.State == FCPUState.Looping ? SpritePause : SpritePlay;
        PlayPauseButton.UpdateButton();
      }
    }
    public void Open()
    {
      if (!File.Exists(programPath))
        Save();
      if (File.Exists(programPath))
        Application.OpenURL("file:///" + programPath);
    }
    public void Save()
    {
      try
      {
        File.WriteAllText(programPath, ProgramInputField.field.text);
      }
      catch (System.Exception e)
      {
        UIUtils.ShowMessageModal(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.TITLE_SAVE_ERROR"), "" + e);
        return;
      }
      UIUtils.ShowMessageModal("", Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.TITLE_SAVE_SUCCESS"));
    }
    public void Load()
    {
      if (File.Exists(programPath))
        try 
        { 
          ProgramInputField.SetDisplayValue(File.ReadAllText(programPath));
        } 
        catch (System.Exception e)
        {
          UIUtils.ShowMessageModal(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.TITLE_LOAD_ERROR"), "" + e);
          return;
        }
      UIUtils.ShowMessageModal("", Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.TITLE_LOAD_SUCCESS"));
    }

    private void ShowLogFullText(int index)
    {
      if (executor.GetType() == typeof(FCPUExecutorJavaScript))
      {
        var logs = ((FCPUExecutorJavaScript)executor).Logs;
        if (index < logs.Count)
          onShowFullLog(logs[index]);
      }
    }
    private void ClearLogs()
    {
      if (executor.GetType() == typeof(FCPUExecutorJavaScript))
      {
        var logs = ((FCPUExecutorJavaScript)executor).Logs;
        for (int i = 0; i < logs.Count; i++)
          logs[i] = "";
        FlushInfo();
      }
    }
    private void ShowStorage()
    {
      onShowStorage?.Invoke();
    }
    private void ClearStorage()
    {
      onClearStorage?.Invoke();
    }

    //Get prefab and make ui
    //=====================================

    public static FCPUEditorUI ProgramEditorPanelPrefab = null;
    public static Sprite SpriteStop = null;
    public static Sprite SpriteBreakpoint = null;
    public static Sprite SpriteNext = null;
    public static Sprite SpritePause = null;
    public static Sprite SpritePlay = null;
    public static Sprite SpriteCopy = null;
    public static Sprite SpritePaste = null;
    public static Sprite SpriteLoad = null;
    public static Sprite SpriteSave = null;
    public static Sprite SpriteOpen = null;

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

      SpriteCopy = Utils.LoadSpriteFromFile(assetsPath + "copy.png", 20, 20);
      SpritePaste = Utils.LoadSpriteFromFile(assetsPath + "clipboard.png", 20, 20);
      SpriteLoad = Utils.LoadSpriteFromFile(assetsPath + "download.png", 20, 20);
      SpriteSave = Utils.LoadSpriteFromFile(assetsPath + "export.png", 20, 20);
      SpriteOpen = Utils.LoadSpriteFromFile(assetsPath + "document-download.png", 20, 20);

      var ProgramEditorPanel = new GameObject("ProgramEditorPanel");
      var RectTransform = ProgramEditorPanel.AddComponent<RectTransform>();
      UIAnchorPosUtils.SetUIAnchor(RectTransform, UIAnchor.Stretch, UIAnchor.Top);

      if (!UIUtils.GetKleiInternalPrefabs())
        return;

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

      var ProgramStatusText = UnityEngine.Object.Instantiate(UIUtils.LocTextPrefab.gameObject, RectTransform);
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
      ProgramStatusTextText.overflowMode = TextOverflowModes.Ellipsis;

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

      var StopButton = Instantiate(UIUtils.ButtonPrefab.gameObject, ButtonBarRectTransform);
      var StopButtonButton = StopButton.GetComponent<FButton>();
      var StopButtonButtonRectTransform = StopButton.GetComponent<RectTransform>();
      StopButtonButton.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.STOP_BUTTON_TOOLTIP";
      StopButtonButton.Icon = SpriteStop;
      StopButton.name = "StopButton";
      UIAnchorPosUtils.SetUIPivot(StopButtonButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(StopButtonButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      StopButtonButtonRectTransform.sizeDelta = new Vector2(32, 32);
      StopButtonButtonRectTransform.anchoredPosition = new Vector2(width - 84, 0);

      var ResetButton = Instantiate(UIUtils.ButtonPrefab.gameObject, ButtonBarRectTransform);
      var ResetButtonButton = ResetButton.GetComponent<FButton>();
      var ResetButtonRectTransform = ResetButton.GetComponent<RectTransform>();
      ResetButton.name = "ResetButton";
      ResetButtonButton.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.RESET_BUTTON_TOOLTIP";
      ResetButtonButton.Icon = SpriteBreakpoint;
      UIAnchorPosUtils.SetUIPivot(ResetButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(ResetButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      ResetButtonRectTransform.sizeDelta = new Vector2(32, 32);
      ResetButtonRectTransform.anchoredPosition = new Vector2(width - 120, 0);

      var PlayPauseButton = Instantiate(UIUtils.ButtonPrefab.gameObject, ButtonBarRectTransform);
      var PlayPauseButtonButton = PlayPauseButton.GetComponent<FButton>();
      var PlayPauseButtonRectTransform = PlayPauseButton.GetComponent<RectTransform>();
      PlayPauseButton.name = "PlayPauseButton";
      PlayPauseButtonButton.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.PLAYPAUSE_BUTTON_TOOLTIP";
      PlayPauseButtonButton.Icon = SpritePlay;
      UIAnchorPosUtils.SetUIPivot(PlayPauseButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(PlayPauseButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      PlayPauseButtonRectTransform.sizeDelta = new Vector2(32, 32);
      PlayPauseButtonRectTransform.anchoredPosition = new Vector2(width - 52, 0);

      var StepButtonButton = Instantiate(UIUtils.ButtonPrefab.gameObject, ButtonBarRectTransform);
      var StepButtonButtonButton = StepButtonButton.GetComponent<FButton>();
      var StepButtonButtonRectTransform = StepButtonButton.GetComponent<RectTransform>();
      StepButtonButton.name = "StepButton";
      StepButtonButtonButton.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.STEP_BUTTON_TOOLTIP";
      StepButtonButtonButton.Icon = SpriteNext;
      UIAnchorPosUtils.SetUIPivot(StepButtonButtonRectTransform, UIPivot.TopRight);
      UIAnchorPosUtils.SetUIAnchor(StepButtonButtonRectTransform, UIAnchor.Left, UIAnchor.Top);
      StepButtonButtonRectTransform.sizeDelta = new Vector2(32, 32);
      StepButtonButtonRectTransform.anchoredPosition = new Vector2(width - 20, 0);

      //Debug.Log("3");

      //Editor area

      var ScrollRect = UnityEngine.Object.Instantiate(UIUtils.KScrollRectPrefab.gameObject, RectTransform);
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

      var PrefabSkillWidget = ScrollRect.transform.Find("ScrollRectContent/PrefabSkillWidget")?.gameObject;
      if (PrefabSkillWidget != null)
        Destroy(PrefabSkillWidget);

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

      var ProgramInputField = Instantiate(UIUtils.KInputFieldPrefab.gameObject, ScrollRectContentRectTransform);
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
      for (int i = 0; i < MAX_LINE; i++)
      {
        UIUtils.AddTextLine((i + 1).ToString(), $"Line{i}", lineCodeWidth, lineHeight, 0, -y, ProgramInputLineTextRectTransform);
        UIUtils.AddImageLine(SpriteBreakpoint, $"BreakPoint{i}", lineHeight, lineHeight, -y, ProgramInputLineTextRectTransform);
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

      var Scrollbar = Instantiate(UIUtils.ScrollbarPrefab.gameObject, RectTransform);
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
      for (int i = 0; i < MAX_LINE; i++)
      {
        var line = i;
        var t = ProgramInputLineTexts.Find($"BreakPoint{line}");
        if (t != null)
        {
          breakpointPoints[line] = t.gameObject;

          var lineText = ProgramInputLineTexts.Find($"Line{line}");
          if (lineText != null)
          {
            lineTexts[line] = lineText.GetComponent<LocText>();
            var button = lineText.gameObject.AddComponent<KButton>();
            button.soundPlayer = UIUtils.ButtonPrefab.kButton.soundPlayer;
            button.onPointerUp += () =>
            {
              Debug.Log("ToggleBreakpointStateLine " + line);
              ToggleBreakpointStateLine(line);
            };
          }
        }
      }

      {
        var button = ProgramStatusText.gameObject.AddComponent<KButton>();
        button.soundPlayer = UIUtils.ButtonPrefab.kButton.soundPlayer;
        button.onPointerUp += () =>
        {
          onShowFullStatus(ProgramStatusText.text);
        };
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
      PlayPauseButton = transform.Find("ButtonBar/PlayPauseButton").GetComponent<FButton>();
      PlayPauseButton.OnClick += () =>
      {
        onPlayPauseButtonClick.Invoke();
        FlushInfo();
      };
      StepButton = transform.Find("ButtonBar/StepButton").GetComponent<FButton>();
      StepButton.OnClick += () =>
      {
        onStepButtonClick.Invoke();
        FlushInfo();
      };

      ScrollRect.verticalScrollbar = ProgramEditorScrollbar;

      MakeInfoUI();
      componentGetted = true;
    }

    public System.Action onStopButtonClick;
    public System.Action onResetButtonClick;
    public System.Action onPlayPauseButtonClick;
    public System.Action onStepButtonClick;
    public System.Action<string> onShowFullLog;
    public System.Action<string> onShowFullStatus;
    public System.Action onShowStorage;
    public System.Action onClearStorage;

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
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_REGS"), "Text", STATUS_WIDTH3, 20, 0, -y, ProgramStatusLines, null, 16); y += 20;
          for (int i = 0; i < registerValueTexts.Length; i++)
          {
            registerValueTexts[i] = UIUtils.AddTextLine($"reg{i} = ?", $"RegisterValue{i}", STATUS_WIDTH3, 15, 0, -y, ProgramStatusLines); y += 15;
          }
          y = startY;
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_INPUTS"), "Text", STATUS_WIDTH3, 20, STATUS_WIDTH3, -y, ProgramStatusLines, null, 16); y += 20;
          for (int i = 0; i < inputValueTexts.Length; i++)
          {
            inputValueTexts[i] = UIUtils.AddTextLine("", $"InputValue{i}", STATUS_WIDTH3, 15, STATUS_WIDTH3, -y, ProgramStatusLines); y += 15;
          }
          y = startY;
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_OUTPUTS"), "Text", STATUS_WIDTH3, 20, STATUS_WIDTH3 * 2, -y, ProgramStatusLines, null, 16); y += 20;
          for (int i = 0; i < outputValueTexts.Length; i++)
          {
            outputValueTexts[i] = UIUtils.AddTextLine("", $"OutputValue{i}", STATUS_WIDTH3, 15, STATUS_WIDTH3 * 2, -y, ProgramStatusLines); y += 15;
          }
        } 
        else
        {
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_INPUTS"), "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, null, 16); y += 20;
          for (int i = 0; i < inputValueTexts.Length; i++)
          {
            inputValueTexts[i] = UIUtils.AddTextLine("", $"InputValue{i}", rightWidth2, 15, leftWidth, -y, ProgramStatusLines); y += 15;
          }
          y += 20;
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_OUTPUTS"), "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, null, 16); y += 20;
          for (int i = 0; i < outputValueTexts.Length; i++)
          {
            outputValueTexts[i] = UIUtils.AddTextLine("", $"OutputValue{i}", rightWidth2, 15, leftWidth, -y, ProgramStatusLines); y += 15;
          }
          y += 20;
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_STORAGES"), "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, null, 16); y += 20;

          UIUtils.AddButtonLine("",
            "STRINGS.UI.UISIDESCREENS.FCPU.SHOW_STORAGE_BUTTON",
            "STRINGS.UI.UISIDESCREENS.FCPU.SHOW_STORAGE_BUTTON_TOOLTIP",
            leftWidth + 100, -y, ProgramStatusLines, ShowStorage, 100, 23
          ); y += 23;

          UIUtils.AddButtonLine("",
            "STRINGS.UI.UISIDESCREENS.FCPU.CLEAR_STORAGE_BUTTON",
            "STRINGS.UI.UISIDESCREENS.FCPU.CLEAR_STORAGE_BUTTON_TOOLTIP",
            leftWidth + 100, -y, ProgramStatusLines, ClearStorage, 100, 23
          ); y += 23;

          y += 20;
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_LOGS"), "Text", rightWidth2, 20, leftWidth, -y, ProgramStatusLines, null, 16); y += 20;

          var a = UIUtils.AddButtonLine("",
            "STRINGS.UI.UISIDESCREENS.FCPU.CLEAR_BUTTON",
            "STRINGS.UI.UISIDESCREENS.FCPU.CLEAR_BUTTON_TOOLTIP",
            leftWidth + 100, -y, ProgramStatusLines, ClearLogs, 100, 23
          ); y += 23;

          for (int i = 0; i < logTexts.Length; i++)
          {
            var index = i;
            logTexts[i] = UIUtils.AddTextLine("", $"LogTexts{i}", 100, 15, leftWidth + 100, -y, ProgramStatusLines, () =>
            {
              ShowLogFullText(index);
            }); y += 15;
          }
        }

        if (isAssemblyCodCpu)
        {
          y += 20;
          UIUtils.AddTextLine(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.SUB_TITLE_MEM"), "Text", STATUS_WIDTH, 20, 0, -y, ProgramStatusLines, null, 16); y += 20;
          UIUtils.AddTextLine("                   0                1                2                3                4", $"MemoryValue-1", STATUS_WIDTH, 15, 0, -y, ProgramStatusLines); y += 15;
          for (int i = 0; i < memoryValueTexts.Length / 4; i++)
          {
            memoryValueTexts[i] = UIUtils.AddTextLine($"{i}     0    0    0    0    0    0    0    0", $"MemoryValue{i}", STATUS_WIDTH, 15, 0, -y, ProgramStatusLines); y += 15;
          }
        }
      }
    }
  }
}
