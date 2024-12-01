using KSerialization;
using Newtonsoft.Json;
using ONICPU.ui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static ONICPU.FCPUExecutor;

namespace ONICPU
{
  [SerializationConfig(KSerialization.MemberSerialization.OptIn)]
  public class FCPU : LogicGateBase, ILogicEventSender, ILogicNetworkConnection, ISaveLoadable, ISim200ms
  {
    private static KAnimHashedString PORT_SYMBOL_PROG = "dot_prog";
    private static KAnimHashedString PORT_SYMBOL_ERROR = "dot_err";
    private static KAnimHashedString SYMBOL_ASS = "lang_0";
    private static KAnimHashedString SYMBOL_JS = "lang_1";

    public enum FCPUType
    {
      None = 0,
      AssemblyCode,
      JavaScript,
    }

    #region Port value

    [Serialize]
    private bool enableValue = false;
    private List<int> inputValues = new List<int>();
    [Serialize]
    protected int outputValue0;
    [Serialize]
    protected int outputValue1;
    [Serialize]
    protected int outputValue2;
    [Serialize]
    protected int outputValue3;
    [Serialize]
    protected int outputValue4;
    [Serialize]
    protected int outputValue5;
    [Serialize]
    protected int outputValue6;
    [Serialize]
    protected int outputValue7;
    [Serialize]
    protected int outputValue8;
    [Serialize]
    protected int outputValue9;
    [Serialize]
    protected int outputValue10;
    [Serialize]
    protected int outputValue11;
    [Serialize]
    protected int outputValue12;
    [Serialize]
    protected int outputValue13;
    [Serialize]
    protected int outputValue14;
    [Serialize]
    protected int outputValue15;

    #endregion

    #region Settings

    [Serialize]
    private string cpuState = "";//JSON
    [Serialize]
    private string cpuStorageData = "";//JSON
    [Serialize]
    private string programValue;
    [Serialize]
    private string breakpointState;
    [Serialize]
    public int cpuSpeed = 3;
    [Serialize]
    public string saveFileName = "";
    [Serialize]
    public bool requireENCheck = true;

    public static int CPUSpeedArray1xIndex = 4;
    public static List<float> CPUSpeedArray = new List<float>()
    {
      0.1f, 0.2f, 0.5f, 1f, 2f, 5, 10
    };
    private float cpuSpeedReal = 1;

    public int CPUSpeed
    {
      get { return cpuSpeed; }
      set { 
        cpuSpeed = value;
        cpuSpeedReal = CPUSpeedArray[cpuSpeed];
        if (cpuSpeedReal > 1 && CPUType == FCPUType.AssemblyCode)
          (executor as FCPUExecutorAssemblyCode).FrameExecuteTickCount = (short)(8 * cpuSpeedReal);
      }
    }
    public FCPUType CPUType = FCPUType.None;

    public string BreakpointState
    {
      get
      {
        return breakpointState;
      }
      set
      {
        breakpointState = value;
        if (executor != null && executor is FCPUExecutorAssemblyCode)
          (executor as FCPUExecutorAssemblyCode).UpdateBreakPointState(breakpointState);
      }
    }
    public string ProgramValue
    {
      get
      {
        return programValue;
      }
      set
      {
        if (programValue != value)
        {
          programValue = value;
          DoCompileProgram();
        }
        else
        {
          ProgramState = "Compile program ok, file not changed";
        }
      }
    }

    private void OnCopySettings(object data)
    {
      FCPU component = ((GameObject)data).GetComponent<FCPU>();
      if (component != null) 
        ProgramValue = component.ProgramValue;
    }

    #endregion

    private string programError = "";
    private string programState = "";
    private string ProgramState
    {
      get { return programState; }
      set
      {
        programState = value;
        if (showProgramEditor)
          fCPUEditorUI?.SetProgramStatus(programState, programErrorState);
      }
    }
    private FCPUExecutor executor = null;

    private class FCPUIO : FCPUInputOutput
    {
      private readonly FCPU component;
      public FCPUIO(FCPU component)
      {
        this.component = component;
        InputValues = new int[component.inputValues.Count];
        OutputValues = new int[component.inputValues.Count];
      }

      public override void FlushInputs()
      {
        for (int i = 0; i < InputValues.Length; i++)
          InputValues[i] = component.inputValues[i];
      }
      public override void Reset()
      {
        base.Reset();

        for (int i = 0; i < OutputValues.Length; i++)
          component.SetOutputValue(i, OutputValues[i]);
      }

      public override int Read(int index)
      {
        if (index < InputValues.Length)
        {
          InputValues[index] = component.inputValues[index];
          return InputValues[index];
        }
        else if (index >= InputValues.Length && index < InputValues.Length + OutputValues.Length)
        {
          OutputValues[index - InputValues.Length] = component.GetOutputValue(index - InputValues.Length);
          return OutputValues[index - InputValues.Length];
        }
        throw new Exception($"P{index} is not readable");
      }
      public override void Write(int index, int value)
      {
        if (index >= InputValues.Length && index < InputValues.Length + OutputValues.Length)
        {
          int i = index - InputValues.Length;
          OutputValues[i] = value;
          component.SetOutputValue(i, OutputValues[i]);
        }
        else
        {
          throw new Exception($"P{index} is not writeable");
        }
      }
    }

    private bool programCompileErrorState = false;
    private bool DoCompileProgram()
    {
      var error = executor.CompileProgram(programValue);
      if (string.IsNullOrEmpty(error))
      {
        programCompileErrorState = false;
        ProgramState = "Compile program ok";
        return true;
      }
      else
      {
        programCompileErrorState = true;
        executor.State = FCPUState.HaltByError;
        programErrorState = true;
        programError = ProgramState;
        ProgramState = "Compile program failed: " + error;
      }
      return false;
    }
    private void InitExecutor()
    {
      if (executor != null)
        return;
      switch (CPUType)
      {
        case FCPUType.AssemblyCode: executor = new FCPUExecutorAssemblyCode(); break;
        case FCPUType.JavaScript: executor = new FCPUExecutorJavaScript(); break;
        default: return;
      }
      executor.InputOutput = new FCPUIO(this);
      executor.onBreakpoint = Executor_onBreakpoint;
      executor.onError = Executor_onError;
      executor.onExecute = Executor_onExecute;
      executor.onStateChanged = Executor_onStateChanged;
      executor.onStopped = Executor_onStopped;
      executor.onBeforeStart = Executor_onBeforeStart;
      executor.Init();
      executor.Restore(cpuState);
      executor.LoadStorage(cpuStorageData);
      CPUSpeed = cpuSpeed;
      BreakpointState = breakpointState;
      if (DoCompileProgram() && executor.State == FCPUState.Looping)
        executor.Start();
    }
    private void DestroyExecutor()
    {
      executor = null;
    }
    private void Executor_onExecute(int line)
    {
      ProgramState = line >= 0 ? $"Executing normally at line: {line + 1}" : "Program is running";
      programErrorState = false;
      if (showProgramEditor)
        fCPUEditorUI?.SetActiveLine(line);
      if (programErrorState)
      {
        programErrorState = false;
        RefreshAnimation();
      }
      if (!programSetState)
      {
        programSetState = true;
        RefreshAnimation();
      }
    }
    private void Executor_onError(string error)
    {
      if (!programErrorState)
      {
        programErrorState = true;
        programError = error;
        RefreshAnimation();
      }
    }
    private void Executor_onStateChanged(FCPUState obj)
    {
      switch (obj)
      {
        case FCPUState.Looping:
          ProgramState = $"Program is running";
          programErrorState = false;
          break;
        case FCPUState.NotStart:
          ProgramState = $"Program is not start run";
          programErrorState = false;
          break;
        case FCPUState.HaltByUser:
          ProgramState = $"Program is paused by command or debugger";
          programErrorState = false;
          break;
        case FCPUState.HaltByError:
          programErrorState = true;
          ProgramState = $"Error: {programError}";
          break;
      }
    }
    private void Executor_onBreakpoint(int line)
    {
      ProgramState = $"Program paused at line: {line}";
      if (showProgramEditor)
        fCPUEditorUI?.SetActiveLine(line);
    }
    private void Executor_onStopped()
    {
      cpuStorageData = executor.SaveStorage();
    }
    private void Executor_onBeforeStart()
    {
      executor.LoadStorage(cpuStorageData);
    }
    private void SaveLoaderSave_Patch_onBeforeSave()
    {
      if (executor != null)
      {
        cpuState = executor.Save();
        if (executor.State == FCPUState.Looping)
        {
          cpuStorageData = executor.SaveStorage();
        }
      }
    }

    private int GetOutputValue(int index)
    {
      switch (index)
      {
        case 0: return outputValue0;
        case 1: return outputValue1;
        case 2: return outputValue2;
        case 3: return outputValue3;
        case 4: return outputValue4;
        case 5: return outputValue5;
        case 6: return outputValue6;
        case 7: return outputValue7;
        case 8: return outputValue8;
        case 9: return outputValue9;
        case 10: return outputValue10;
        case 11: return outputValue11;
        case 12: return outputValue12;
        case 13: return outputValue13;
        case 14: return outputValue14;
        case 15: return outputValue15;
      }
      return 0;
    }
    private void SetOutputValue(int index, int val)
    {
      switch (index)
      {
        case 0: outputValue0 = val; break;
        case 1: outputValue1 = val; break;
        case 2: outputValue2 = val; break;  
        case 3: outputValue3 = val; break;
        case 4: outputValue4 = val; break;
        case 5: outputValue5 = val; break;
        case 6: outputValue6 = val; break;
        case 7: outputValue7 = val; break;
        case 8: outputValue8 = val; break;
        case 9: outputValue9 = val; break;
        case 10: outputValue10 = val; break;
        case 11: outputValue11 = val; break;
        case 12: outputValue12 = val; break;
        case 13: outputValue13 = val; break;
        case 14: outputValue14 = val; break;
        case 15: outputValue15 = val; break;
      }
      var sender = outputSenders[index];
      if (sender != null)
        sender.SetValue(val);
    }

    private List<LogicEventHandler> inputs = new List<LogicEventHandler>();
    private List<LogicPortVisualizer> outputs = new List<LogicPortVisualizer>();
    private List<LogicEventSender> outputSenders = new List<LogicEventSender>();
    private LogicEventHandler resetOne;
    private LogicEventHandler controlOne;

    public int portCount = 4;

    private static KAnimHashedString[] PORT_SYMBOLS;
    private HashedString[] OUTPUT_IDS = new HashedString[] {
      null, null,
      OUTPUT_TWO_PORT_ID,
      OUTPUT_THREE_PORT_ID,
      OUTPUT_FOUR_PORT_ID,
      new HashedString("LogicGateOutputFive"),
      new HashedString("LogicGateOutputSix"),
      new HashedString("LogicGateOutputSeven"),
      new HashedString("LogicGateOutputEight"),
      new HashedString("LogicGateOutputNine"),
      new HashedString("LogicGateOutputTen"),
      new HashedString("LogicGateOutputEleven"),
      new HashedString("LogicGateOutputTwelve"),
      new HashedString("LogicGateOutputThirteen"),
      new HashedString("LogicGateOutputFourteenn"),
      new HashedString("LogicGateOutputFifteen"),
      new HashedString("LogicGateOutputSixteen"),
    };

    static FCPU()
    {
      PORT_SYMBOLS = new KAnimHashedString[16];
      for (int i = 0; i < 16; i++)
        PORT_SYMBOLS[i] = new KAnimHashedString($"dot{i}");
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();

      if (string.IsNullOrEmpty(saveFileName)) {
        saveFileName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        if (CPUType == FCPUType.JavaScript)
          saveFileName += ".js";
        else if (CPUType == FCPUType.AssemblyCode)
          saveFileName += ".asm";
      }
      if (string.IsNullOrEmpty(programValue) && CPUType == FCPUType.JavaScript)
        programValue = FCPUExecutorJavaScript.DefaultCode;

      controlPortOffsets = new CellOffset[2]
      {
        new CellOffset(0, 0),
        new CellOffset(0, 1)
      };
      if (portCount == 2)
      {
        inputPortOffsets = new CellOffset[2]
        {
          new CellOffset(-1, 1),
          new CellOffset(-1, 0)
        };
        outputPortOffsets = new CellOffset[2]
        {
          new CellOffset(1, 1),
          new CellOffset(1, 0)
        };
      }
      else if (portCount == 4)
      {
        inputPortOffsets = new CellOffset[4]
        {
          new CellOffset(-1, 3),
          new CellOffset(-1, 2),
          new CellOffset(-1, 1),
          new CellOffset(-1, 0)
        };
        outputPortOffsets = new CellOffset[4]
        {
          new CellOffset(1, 3),
          new CellOffset(1, 2),
          new CellOffset(1, 1),
          new CellOffset(1, 0)
        };
        controlPortOffsets[1].y = 3;
      }
      else if (portCount == 8)
      {
        inputPortOffsets = new CellOffset[8]
        {
          new CellOffset(-1, 7),
          new CellOffset(-1, 6),
          new CellOffset(-1, 5),
          new CellOffset(-1, 4),
          new CellOffset(-1, 3),
          new CellOffset(-1, 2),
          new CellOffset(-1, 1),
          new CellOffset(-1, 0)
        };
        outputPortOffsets = new CellOffset[8]
        {
          new CellOffset(1, 7),
          new CellOffset(1, 6),
          new CellOffset(1, 5),
          new CellOffset(1, 4),
          new CellOffset(1, 3),
          new CellOffset(1, 2),
          new CellOffset(1, 1),
          new CellOffset(1, 0)
        };
        controlPortOffsets[1].y = 7;
      }

      for (int i = 0; i < portCount; i++)
      {
        inputs.Add(null);
        outputs.Add(null);
        outputSenders.Add(null);
        inputValues.Add(0);
      }

      RegisterGates();

      for (int i = 0; i < portCount; i++)
      {
        var InputCell = GetActualCell(inputPortOffsets[i]);
        inputs[i] = new LogicEventHandler(InputCell, UpdateState, null, LogicPortSpriteType.RibbonInput);
        inputValues[i] = 0;
      }
      for (int i = 1; i < portCount; i++)
      {
        var OutputCell = GetActualCell(outputPortOffsets[i]);
        var OUTPUT_ID = OUTPUT_IDS[i];
        outputs[i] = new LogicPortVisualizer(OutputCell, LogicPortSpriteType.RibbonOutput);
        outputSenders[i] = new LogicEventSender(OUTPUT_ID, OutputCell, delegate (int new_value, int prev_value)
        {
          if (this != null)
            OnAdditionalOutputsLogicValueChanged(OUTPUT_ID, new_value, prev_value);
        }, null, LogicPortSpriteType.RibbonOutput);
        outputSenders[i].SetValue(GetOutputValue(i));
      }

      controlOne = new LogicEventHandler(ControlCellOne, UpdateStateControl, null, LogicPortSpriteType.ControlInput);
      resetOne = new LogicEventHandler(ControlCellTwo, UpdateStateReset, null, LogicPortSpriteType.Input);

      //Copy from LogicGate, hash may change
      Subscribe(774203113, OnBuildingBrokenDelegate);
      Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
      BuildingHP component = GetComponent<BuildingHP>();
      if (component == null || !component.IsBroken)
        Connect();
      InitExecutor();
      SaveLoaderSave_Patch.onBeforeSave += SaveLoaderSave_Patch_onBeforeSave;
    }


    protected override void OnCleanUp()
    {
      if (executor != null)
        executor.Stop();
      cleaningUp = true;
      SaveLoaderSave_Patch.onBeforeSave -= SaveLoaderSave_Patch_onBeforeSave;
      DestroyExecutor();
      Disconnect();
      Unsubscribe(774203113, OnBuildingBrokenDelegate);
      Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate);
      UnregisterGates();
      base.OnCleanUp();
    }
    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      Subscribe(-905833192, OnCopySettingsDelegate);
      //if (UIAssetbundle == null)
      //  UIAssetbundle = Utils.LoadAssetBundle("test.assetbundle");
    }

    #region Port

    private int GetActualCell(CellOffset offset)
    {
      Rotatable component = GetComponent<Rotatable>();
      if (component != null)
        offset = component.GetRotatedCellOffset(offset);
      return Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), offset);
    }

    private List<IOVisualizer> visChildren = new List<IOVisualizer>();
    private class IOVisualizer : ILogicUIElement, IUniformGridObject
    {
      private int cell;

      private bool input;

      public IOVisualizer(int cell, bool input)
      {
        this.cell = cell;
        this.input = input;
      }

      public int GetLogicUICell()
      {
        return cell;
      }

      public LogicPortSpriteType GetLogicPortSpriteType()
      {
        if (!input)
        {
          return LogicPortSpriteType.Output;
        }
        return LogicPortSpriteType.Input;
      }

      public Vector2 PosMin()
      {
        return Grid.CellToPos2D(cell);
      }

      public Vector2 PosMax()
      {
        return PosMin();
      }
    }
    private void RegisterGates() 
    {
      UnregisterGates();
      for (int i = 0; i < portCount; i++) 
        visChildren.Add(new IOVisualizer(GetActualCell(outputPortOffsets[i]), input: false));
      for (int i = 0; i < portCount; i++)
        visChildren.Add(new IOVisualizer(GetActualCell(inputPortOffsets[i]), input: true));
      LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
      foreach (IOVisualizer visChild in visChildren)
        logicCircuitManager.AddVisElem(visChild);
    }
    private void UnregisterGates()
    {
      LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
      foreach (IOVisualizer visChild in visChildren)
      {
        logicCircuitManager.RemoveVisElem(visChild);
      }
      visChildren.Clear();
    }

    #endregion

    #region Events

    private static readonly EventSystem.IntraObjectHandler<FCPU> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<FCPU>(delegate (FCPU component, object data)
    {
      component.OnBuildingBroken(data);
    });
    private static readonly EventSystem.IntraObjectHandler<FCPU> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<FCPU>(delegate (FCPU component, object data)
    {
      component.OnBuildingFullyRepaired(data);
    });
    private static readonly EventSystem.IntraObjectHandler<FCPU> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<FCPU>(delegate (FCPU component, object data)
    {
      component.OnCopySettings(data);
    });

    private void OnAdditionalOutputsLogicValueChanged(HashedString port_id, int new_value, int prev_value)
    {
      if (gameObject != null)
      {
        //Copy from LogicGate, hash may change
        gameObject.Trigger(-801688580, new LogicValueChanged
        {
          portID = port_id,
          newValue = new_value,
          prevValue = prev_value
        });
      }
    }
    private void OnBuildingBroken(object data)
    {
      Disconnect();
    }
    private void OnBuildingFullyRepaired(object data)
    {
      Connect();
    }

    #endregion

    private bool connected = false;
    private bool cleaningUp = false;

    private void Connect()
    {
      if (!connected)
      {
        LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
        UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
        connected = true;

        outputs[0] = new LogicPortVisualizer(OutputCellOne, LogicPortSpriteType.RibbonOutput);
        logicCircuitSystem.AddToNetworks(OutputCellOne, this, is_endpoint: true);
        logicCircuitManager.AddVisElem(outputs[0]);
        for (int i = 1; i < portCount; i++)
        {
          var OutputCell = GetActualCell(outputPortOffsets[i]);
          outputs[i] = new LogicPortVisualizer(OutputCell, LogicPortSpriteType.RibbonOutput);
          logicCircuitSystem.AddToNetworks(OutputCell, outputSenders[i], is_endpoint: true);
          logicCircuitManager.AddVisElem(outputs[i]);
        }
        for (int i = 0; i < portCount; i++)
        {
          logicCircuitSystem.AddToNetworks(GetActualCell(inputPortOffsets[i]), inputs[i], is_endpoint: true);
          logicCircuitManager.AddVisElem(inputs[i]);
        }

        logicCircuitSystem.AddToNetworks(ControlCellOne, controlOne, is_endpoint: true);
        logicCircuitManager.AddVisElem(controlOne);
        logicCircuitSystem.AddToNetworks(ControlCellTwo, resetOne, is_endpoint: true);
        logicCircuitManager.AddVisElem(resetOne);

        RefreshAnimation();
      }
    }
    private void Disconnect()
    {
      if (connected)
      {
        LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
        UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
        connected = false;

        logicCircuitSystem.RemoveFromNetworks(OutputCellOne, this, is_endpoint: true);
        logicCircuitManager.RemoveVisElem(outputs[0]);
        outputs[0] = null;
        for (int i = 1; i < portCount; i++)
        {
          var outputCell = GetActualCell(outputPortOffsets[i]);
          logicCircuitSystem.RemoveFromNetworks(outputCell, outputSenders[i], is_endpoint: true);
          logicCircuitManager.RemoveVisElem(outputs[i]);
          outputs[i] = null;
        }
        for (int i = 0; i < portCount; i++)
        {
          int inputCell = GetActualCell(inputPortOffsets[i]);
          logicCircuitSystem.RemoveFromNetworks(inputCell, inputs[i], is_endpoint: true);
          logicCircuitManager.RemoveVisElem(inputs[i]);
          inputs[i] = null;
        }

        logicCircuitSystem.RemoveFromNetworks(ControlCellOne, controlOne, is_endpoint: true);
        logicCircuitManager.RemoveVisElem(controlOne);
        logicCircuitSystem.RemoveFromNetworks(ControlCellTwo, resetOne, is_endpoint: true);
        logicCircuitManager.RemoveVisElem(resetOne);
        controlOne = null;
        resetOne = null;

        RefreshAnimation();
      }
    }
    private void UpdateStateControl(int new_value, int prev_value)
    {
      if (cleaningUp)
        return;

      if (requireENCheck && !showProgramEditor)
      {
        enableValue = ((controlOne != null) ? controlOne.Value : 0) != 0;
        if (connected && enableValue)
        {
          if (executor.State != FCPUState.Looping && !programCompileErrorState)
            executor.Start();
        }
        else
        {
          if (executor.State == FCPUState.Looping)
            executor.Stop();
        }
      }

      RefreshAnimation();
    }
    private void UpdateStateReset(int new_value, int prev_value)
    {
      if (cleaningUp)
        return;
      var resetValue = ((resetOne != null) ? resetOne.Value : 0) != 0;
      if (resetValue && executor != null)
        executor.ExecuteReset();
      RefreshAnimation();
    }
    private void UpdateState(int new_value, int prev_value)
    {
      if (cleaningUp)
        return;

      inputValues[0] = inputs[0].Value;
      for (int i = 1; i < portCount; i++)
        inputValues[i] = (inputs[i] != null) ? inputs[i].Value : 0;

      RefreshAnimation();
    }

    #region UI

    private bool showProgramEditor = false;
    private static FCPUEditorUI fCPUEditorUI = null;

    public void OnShowProgramEditor()
    {
      if (!showProgramEditor)
      {
        showProgramEditor = true;

        try
        {
          fCPUEditorUI = null;
          FCPUEditorUI.MakeEditorUI();
        } 
        catch(Exception e)
        {
          Debug.LogError("Failed to MakeEditorUI! " + e);
        }

        if (FCPUEditorUI.ProgramEditorPanelPrefab)
        {
          ScreenPrefabs.Instance.InfoDialogScreen.pause = false;
          var screenInstance = Util.KInstantiateUI<InfoDialogScreen>(
            ScreenPrefabs.Instance.InfoDialogScreen.gameObject, 
            GameScreenManager.Instance.ssOverlayCanvas.gameObject, force_active: true
          );
          screenInstance.SetHeader(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_TITLE"))
              .AddUI(FCPUEditorUI.ProgramEditorPanelPrefab, out fCPUEditorUI)
              .AddOption(false, out var copyButton, out var copyButtonText)
              .AddOption(false, out var pasteButton, out var pasteButtonText)
              .AddOption(false, out var loadButton, out var loadButtonText)
              .AddOption(false, out var saveButton, out var saveButtonText)
              .AddOption(false, out var openButton, out var openButtonText)
              .AddOption(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_COMPILE"), (screen) =>
              {
                ProgramValue = fCPUEditorUI.GetProgram();
                BreakpointState = fCPUEditorUI.GetBreakpointStateStr();
              }, rightSide: true)
              .AddOption(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_CLOSE"), (screen) =>
              {
                var program = fCPUEditorUI.GetProgram();
                if (ProgramValue != program)
                  ProgramValue = program;
                BreakpointState = fCPUEditorUI.GetBreakpointStateStr();
                showProgramEditor = false;
                CameraController.Instance.DisableUserCameraControl = false;
                screen.SetIsEditing(false);
                screen.Show(false);
              }, rightSide: true)
              .SetIsEditing(true);

          var button = copyButton.gameObject.AddComponent<FButton>();
          button.label = copyButtonText;
          button.Icon = FCPUEditorUI.SpriteCopy;
          button.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_COPY";
          button.IconSize = new Vector2(16, 16);
          button.Size = new Vector2(45, 45);
          button.OnClick += () => fCPUEditorUI.Copy();

          button = pasteButton.gameObject.AddComponent<FButton>();
          button.label = pasteButtonText;
          button.Icon = FCPUEditorUI.SpritePaste;
          button.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_PASTE";
          button.IconSize = new Vector2(16, 16);
          button.Size = new Vector2(45, 45);
          button.OnClick += () => fCPUEditorUI.Patse();

          button = loadButton.gameObject.AddComponent<FButton>();
          button.label = loadButtonText;
          button.Icon = FCPUEditorUI.SpriteLoad;
          button.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_LOAD";
          button.IconSize = new Vector2(16, 16);
          button.Size = new Vector2(45, 45);
          button.OnClick += () => fCPUEditorUI.Load();

          button = saveButton.gameObject.AddComponent<FButton>();
          button.label = saveButtonText;
          button.Icon = FCPUEditorUI.SpriteSave;
          button.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_SAVE";
          button.IconSize = new Vector2(16, 16);
          button.Size = new Vector2(45, 45);
          button.OnClick += () => fCPUEditorUI.Save();

          button = openButton.gameObject.AddComponent<FButton>();
          button.label = openButtonText;
          button.Icon = FCPUEditorUI.SpriteOpen;
          button.TooltipKey = "STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_OPEN";
          button.IconSize = new Vector2(16, 16);
          button.Size = new Vector2(45, 45);
          button.OnClick += () => fCPUEditorUI.Open();

          ScreenPrefabs.Instance.InfoDialogScreen.pause = true;

          //Resize modal
          screenInstance.transform.Find("Layout").GetComponent<LayoutElement>().minWidth = 800;

          CameraController.Instance.DisableUserCameraControl = true;
          fCPUEditorUI.executor = executor;
          fCPUEditorUI.SetValues(programValue, ProgramState, breakpointState);
          fCPUEditorUI.SetProgramPath(Path.Combine(Util.RootFolder(), "FCPU", saveFileName));
          fCPUEditorUI.onPlayPauseButtonClick = () =>
          {
            enableValue = true;
            RefreshAnimation();
            if (executor.State == FCPUState.Looping)
            {
              executor.State = FCPUState.HaltByUser;
            }
            else if (!programCompileErrorState)
            {
              executor.Start();
            }
          };
          fCPUEditorUI.onStepButtonClick = () =>
          {
            enableValue = true;
            RefreshAnimation();
            executor.ExecuteTick();
            executor.State = FCPUState.HaltByUser;
          };
          fCPUEditorUI.onResetButtonClick = () =>
          {
            enableValue = false;
            RefreshAnimation();
            executor.ExecuteReset();
          };
          fCPUEditorUI.onStopButtonClick = () =>
          {
            enableValue = false;
            RefreshAnimation();
            executor.Stop();
          };
          fCPUEditorUI.onShowFullLog = (log) =>
          {
            UIUtils.ShowMessageModal(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_LOG_TITLE"), log);
          };
          fCPUEditorUI.onShowFullStatus = (log) =>
          {
            UIUtils.ShowMessageModal(Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_STATUS_TITLE"), log);
          };
          fCPUEditorUI.onClearStorage = () =>
          {
            cpuStorageData = "";
            if (executor != null)
              executor.ResetStorage();
          };
          fCPUEditorUI.onShowStorage = () =>
          {
            if (executor != null && executor.State == FCPUState.Looping)
            {
              cpuStorageData = executor.SaveStorage();
            }
            if (CPUType == FCPUType.JavaScript)
            {
              var sb = new StringBuilder();
              var js = string.IsNullOrEmpty(cpuStorageData) ?
                null :
                JsonConvert.DeserializeObject<Dictionary<string, object>>(cpuStorageData);
              if (js == null || js.Count == 0)
              {
                sb.Append("Empty, No storaged data");
              }
              else
              { 
                var len = 0;
                foreach (var item in js)
                {
                  sb.Append(item.Key);
                  sb.Append(" : ");
                  sb.AppendLine(item.Value.ToString());

                  if (len > 30)
                  {
                    sb.Append("... (");
                    sb.Append(js.Count - len);
                    sb.Append("+)");
                  }

                  len++;
                }
              }

              UIUtils.ShowMessageModal(
                "storage",
                sb.ToString()
              );
            }
          };
        }
        else
        {
          UIUtils.ShowMessageModal(
            Utils.GetLocalizeString("STRINGS.UI.UISIDESCREENS.FCPU.EDITOR_TITLE"), 
            "Failed to make editor UI, mod need update, Exception in console"
          );
        }
      }
    }
    public void OnShowCPUManual()
    {
      ManagementMenu.Instance.OpenCodexToEntry("FCPUTIPS0");
    }
    public void OnRequireENCheckChanged()
    {
      if (requireENCheck)
        UpdateStateControl(1, 1);
      else
        enableValue = true;
    }

    #endregion

    #region Anim

    private bool animState = false;
    private bool programSetState = false;
    private bool programErrorState = false;
    private Color activeTintColor = new Color(1f, 0.4f, 0.4f);
    private Color successTintColor = new Color(0.4f, 1f, 0.4f);
    private Color inactiveTintColor = new Color(40 / 255.0f, 58 / 255.0f, 46 / 255.0f);

    protected void RefreshAnimation()
    {
      if (cleaningUp)
        return;
      var state = connected && enableValue;
      KBatchedAnimController component = GetComponent<KBatchedAnimController>();
      if (state != animState)
      {
        animState = state;
        if (animState)
          component.Play("on", KAnim.PlayMode.Loop);
        else
          component.Play("off");
      }
      Utils.TintSymbolConditionally(enableValue, programErrorState, component, PORT_SYMBOL_ERROR, activeTintColor, inactiveTintColor);
      Utils.TintSymbolConditionally(enableValue, programSetState, component, PORT_SYMBOL_PROG, successTintColor, inactiveTintColor);
      Utils.ShowSymbolConditionally(true, CPUType == FCPUType.AssemblyCode, component, SYMBOL_ASS, SYMBOL_JS);
    }


    #endregion

    #region Code

    private int sleepTick = 0;
    private void ExecCode()
    {
      if (enableValue)
      {
        if (cpuSpeedReal < 1)
        {
          if (sleepTick > 0) {
            sleepTick--;
            return;
          }
          else
          {
            sleepTick = (int)(1.0f / cpuSpeedReal) - 1;
          }
        }

        executor.ExecuteTicks();
      }
    }

    #endregion

    #region Logic

    public int GetLogicCell()
    {
      return OutputCellOne;
    }
    public int GetLogicValue()
    {
      return outputValue0;
    }
    public void OnLogicNetworkConnectionChanged(bool connected)
    {
    }
    public void Sim200ms(float dt)
    {
      ExecCode();
    }
    public void LogicTick()
    {
    }

    #endregion
  }
}
