using Newtonsoft.Json;
using NiL.JS;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using NiL.JS.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONICPU
{
  public class FCPUExecutorJavaScript : FCPUExecutor
  {
    [JsonObject]
    public class FCPURestoreable
    {
      [JsonProperty]
      public FCPUState State = FCPUState.NotStart;
      [JsonProperty]
      public FCPUInputOutput InputOutput;
      [JsonProperty]
      public string StorageData;
    }

    public int LogsMaxCount = 16;
    public List<string> Logs = new List<string>();

    private Module module;
    private Function tick = null;
    private Function start = null;
    private Function stop = null;
    private Function __initStorage = null;
    private Function __saveStorage = null;
    private JSValue io = null;
    private JSValue[] pins = null;
    private JSValue __internalErrorState = null;
    private string StorageData = "";

    public const string DefaultCode = @"//Here is your program
//Note: that you cannot use the while loop for delay, which will cause the game to stall.

function start() {
  /*
    This method called when cpu unit after start running

    * you can use `storage` to load/save data,
    * All fields in storage are saved and can be restored after the next startup:
    
    let lastData = storage.myData;
  */
  __log('Program start');
}
function stop() {
  //This method called when cpu unit before stop
  __log('Program stop');
}
function tick() {
  /*
    This method called every logic tick
    
    * input and output: you can use `io.P{X}` to read or set pin values. eg:
    * Read input: let pin0value = io.P0; 
    * Set output: io.P4 = 1; 
    
    Some helper function
    * __getbit: you can call `__getbit(io.P0, 1)` to get one bit value of a input pin.
    * __changebit: you can call `io.P3 = __changebit(io.P3, 1, 2)` to set one bit value of a output pin.
    * __log: you can call `__log('message')` to log some messages in editor.
    * __reset: call __reset() to reset io
    * __shutdown: you can call `__shutdown('error message')` to shutdown this cpu unit.
  */
  
}
";
    private const string __initCode = @"
var io = {};
var storage = {};
function __initStorage(json) {
  if (json) {
    try { 
      storage = JSON.parse(json); 
    }
    catch (e) { 
      storage = {}; 
    }
   }
}
function __saveStorage() {
  return JSON.stringify(storage);
}
function __shutdown(errorMessage) {
  if (errorMessage)
    __internalErrorState = errorMessage;
  else
    __internalErrorState = 'shutdown by user code';
}
function __getbit(invalue, bit) {
  return (invalue >> bit) & 0x1;
}
function __changebit(invalue, bit, value) {
  return invalue & ((value & 0x1) << bit);
}
";

    public override string CompileProgram(string program)
    {
      State = FCPUState.HaltByUser;
      tick = null;
      start = null;
      stop = null;
      try
      {
        module = new Module(__initCode + program);
      }
      catch (Exception e)
      {
        Debug.Log(e.ToString());
        return "CompileProgram Error: " + e.Message;
      }
      try
      {
        module.Run();

        io = module.Context.GetVariable("io");

        int i = 0, count = InputOutput.InputValues.Length + InputOutput.OutputValues.Length;
        pins = new JSValue[count];
        for (; i < InputOutput.InputValues.Length; i++)
          pins[i] = io.DefineProperty($"P{i}");
        for (; i < count; i++)
          pins[i] = io.DefineProperty($"P{i}");

        __internalErrorState = module.Context.DefineVariable("__internalErrorState");
        var @delegate = new Action<string>(message =>
        {
          Logs.Add(message);
          if (Logs.Count >= LogsMaxCount)
            Logs.RemoveAt(0);
        });
        module.Context.DefineVariable("__log").Assign(module.Context.GlobalContext.ProxyValue(@delegate));
        var @delegate1 = new System.Action(() =>
        {
          ResetAll();
        });
        module.Context.DefineVariable("__reset").Assign(module.Context.GlobalContext.ProxyValue(@delegate1));

        var variable = module.Context.GetVariable("tick");
        tick = variable != null ? variable.As<Function>() : null;
        variable = module.Context.GetVariable("start");
        start = variable != null ? variable.As<Function>() : null;
        variable = module.Context.GetVariable("stop");
        stop = variable != null ? variable.As<Function>() : null;
        variable = module.Context.GetVariable("__initStorage");
        __initStorage = variable != null ? variable.As<Function>() : null;
        variable = module.Context.GetVariable("__saveStorage");
        __saveStorage = variable != null ? variable.As<Function>() : null;
        return "";
      }
      catch (Exception e)
      {
        Debug.Log(e.ToString());
        return "EvalProgram Error: " + e.Message;
      }
    }

    private Arguments emptyArguments = new Arguments();

    private void HandleError(Exception eo, string prefix = "Call Error: ")
    {
      if (eo.GetType() == typeof(JSException))
      {
        var e = eo as JSException;
        StringBuilder err = new StringBuilder();
        try
        {
          err.Append("Tick program Error: ");
          err.Append(e.Message);
          err.Append(" Source: ");
          err.Append(e.SourceCode);
          err.Append(" StackTrace: ");
          err.Append(e.StackTrace);
        }
        catch (Exception e1)
        {
          err.Append(e1.Message);
        }
        Debug.Log(eo.ToString());
        onError.Invoke(err.ToString());
      }
      else
      {
        onError.Invoke(prefix + eo.Message);
        Debug.Log(eo.ToString());
      }
      State = FCPUState.HaltByError;
    }
    private void AssianValues()
    {
      int i = 0, count = InputOutput.InputValues.Length + InputOutput.OutputValues.Length;
      for (; i < InputOutput.InputValues.Length; i++)
        pins[i].Assign(InputOutput.Read(i));
      for (; i < count; i++)
        pins[i].Assign(InputOutput.Read(i));
    }
    private void UpdateValues()
    {
      int i = InputOutput.InputValues.Length, count = InputOutput.InputValues.Length + InputOutput.OutputValues.Length;
      for (int j = 0; i < count; i++, j++)
      {
        int newValue;
        if (pins[i].Is(JSValueType.Boolean))
          newValue = ((bool)pins[i]) ? 1 : 0;
        else
          newValue = pins[i].As<int>();
        if (newValue != InputOutput.OutputValues[j])
          InputOutput.Write(i, newValue);
      }

      if (__internalErrorState != null && !__internalErrorState.IsNull)
      {
        var message = __internalErrorState.As<string>();
        onError.Invoke(message);
        State = FCPUState.HaltByUser;
      }
    }
    private void ResetAll()
    {
      resetCalled = true;
      InputOutput.Reset();
    }

    private bool resetCalled = false;

    public override void Init()
    {
    }
    public override void Restore(string json)
    {
      FCPURestoreable restoreData = JsonConvert.DeserializeObject<FCPURestoreable>(json);
      if (restoreData != null)
      {
        if (restoreData.InputOutput != null)
          InputOutput.CopyValuesFrom(restoreData.InputOutput);
        if (restoreData.StorageData != null) 
          StorageData = restoreData.StorageData;
        State = restoreData.State;
      }
    }
    public override string Save()
    {
      FCPURestoreable restoreData = new FCPURestoreable();
      restoreData.InputOutput = InputOutput;
      restoreData.State = State;
      try
      {
        if (__saveStorage != null)
          StorageData = __saveStorage.Call(new Arguments()).As<string>();
      }
      catch (Exception e)
      {
        HandleError(e, "Call __saveStorage Error: ");
      }
      restoreData.StorageData = StorageData;
      return JsonConvert.SerializeObject(restoreData);
    }
    public override void Start()
    {
      State = FCPUState.Looping;
      try
      {
        if (__initStorage != null)
          __initStorage.Call(new Arguments() { StorageData });
        if (__internalErrorState != null)
          __internalErrorState.Assign(JSValue.Null);
      }
      catch (Exception e)
      {
        HandleError(e, "Start preinit Error: ");
        return;
      }
      if (start != null)
      {
        try
        {
          AssianValues();

          resetCalled = false;
          start.Call(emptyArguments);

          if (!resetCalled)
            UpdateValues();

          onExecute.Invoke(-1);
        }
        catch(Exception e)
        {
          HandleError(e, "Call start Error: ");
        }
      }
    }
    public override void Stop()
    {
      State = FCPUState.HaltByUser;
      if (stop != null)
      {
        try
        {
          resetCalled = false;
          stop.Call(new Arguments());
          if (!resetCalled)
            UpdateValues();
        }
        catch (Exception e)
        {
          HandleError(e, "Call stop Error: ");
        }
      }
    }
    public override void ExecuteReset()
    {
      ResetAll();
      State = FCPUState.NotStart; 
    }
    public override void ExecuteTick()
    {
      if (State == FCPUState.Looping) 
      {
        if (tick == null)
        {
          onError.Invoke("Tick program Error: Your program dose not have tick function");
          State = FCPUState.HaltByError;
          return;
        }

        try
        {
          AssianValues();

          resetCalled = false;
          tick.Call(emptyArguments);

          if (!resetCalled)
            UpdateValues();
        }
        catch (Exception e)
        {
          HandleError(e, "Tick program Error: ");
        }
      }
    }
    public override void ExecuteTicks()
    {
      ExecuteTick();
    }
  }
}
