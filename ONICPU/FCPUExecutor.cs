using Newtonsoft.Json;
using System;

namespace ONICPU
{
  public class FCPUExecutor
  {
    public const int MAX_LINE = 400;

    #region Events

    public Action<string> onError;
    public Action<int> onExecute;
    public Action<int> onBreakpoint;
    public System.Action onSave;
    public Action<FCPUState> onStateChanged;

    #endregion

    #region Base
    
    [JsonObject]
    public class FCPUInputOutput
    {
      [JsonProperty]
      public int[] InputValues = new int[4];
      [JsonProperty]
      public int[] OutputValues = new int[4];

      protected int MergePinsTo8bit(int[] arr, int startIndex)
      {
        int v = arr[startIndex];
        for (int i = startIndex + 1, j = 1; i < arr.Length; i++, j++)
          v |= arr[i] << j;
        return v;
      }
      protected void Split8bitToPins(int value, int[] targetArr, int startIndex, InputOutputSplit8bitToPinsCallback callback)
      {
        int mask = 0x1;
        for (int i = startIndex; i < targetArr.Length; i++)
        {
          targetArr[i] = value & mask;
          mask <<= 1;

          if (callback != null)
            callback(i, targetArr[i]);
        }
      }

      public delegate void InputOutputSplit8bitToPinsCallback(int index, int value);
      public virtual void FlushInputs()
      {

      }
      public virtual void Reset()
      {
        for (int i = 0; i < OutputValues.Length; i++)
          OutputValues[i] = 0;
        for (int i = 0; i < InputValues.Length; i++)
          InputValues[i] = 0;
      }
      public virtual int Read(int index)
      {
        throw new NotImplementedException();
      }
      public virtual void Write(int index, int value)
      {
      }
      public void CopyValuesFrom(FCPUInputOutput other)
      {
        InputValues = other.InputValues;
        OutputValues = other.OutputValues;
      }
    }

    public System.Action onStopped;
    public System.Action onBeforeStart;

    public class FCPUInputOutputBase : FCPUInputOutput
    {
      public override int Read(int index)
      {
        if (index < InputValues.Length)
          return InputValues[index];
        else if (index >= InputValues.Length && index < InputValues.Length + OutputValues.Length)
          return OutputValues[index - InputValues.Length];
        throw new Exception($"P{index} is not readable");
      }
      public override void Write(int index, int value)
      {
        if (index >= InputValues.Length && index < InputValues.Length + OutputValues.Length)
        {
          int i = index - InputValues.Length;
          OutputValues[i] = value;
        }
        else
        {
          throw new Exception($"P{index} is not writeable");
        }
      }
    }

    protected FCPUState _FCPUState = FCPUState.NotStart;

    public FCPUInputOutput InputOutput { get; set; } //InputOutput Valuse
    public virtual FCPUState State
    {
      get
      {
        return _FCPUState;
      }
      set
      {
        if (_FCPUState != value)
        {
          _FCPUState = value;
          onStateChanged?.Invoke(_FCPUState);
        }
      }
    }
    protected void HaltAndReportError(string message)
    {
      State = FCPUState.HaltByError;
      onError.Invoke(message);
    }

    #endregion

    #region Interdace

    public virtual string CompileProgram(string program)
    {
      throw new NotImplementedException();
    }
    public virtual void Init()
    {
      throw new NotImplementedException();
    }
    public virtual void Restore(string json)
    {
    }
    public virtual string Save()
    {
      return "";
    }
    public virtual void LoadStorage(string json)
    {
    }
    public virtual void ResetStorage()
    {

    }
    public virtual string SaveStorage()
    {
      return "";
    }
    public virtual void Start()
    {
      onBeforeStart?.Invoke();
    }
    public virtual void Stop()
    {
      onStopped?.Invoke();
    }
    public virtual void ExecuteReset()
    {
      throw new NotImplementedException();
    }
    public virtual void ExecuteTick()
    {
      throw new NotImplementedException();
    }
    public virtual void ExecuteTicks()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
  public enum FCPUState
  {
    Looping,
    NotStart,
    HaltByUser,
    HaltByError,
  }
}
