using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ONICPU
{
  public class FCPUExecutorAssemblyCode : FCPUExecutor, FCPUExecutorAssemblyCode.FCPUExecContext
  {
    public FCPUExecutorAssemblyCode()
    {
      SystemRegisters = new FCPUSystemRegisters(this);
      Accessables = new FCPUAccessable();
      State = FCPUState.NotStart;
    }

    #region CPU Data define

    public short FrameExecuteTickCount { get; set; } = 8;
    public short DataBits { get; set; } = 32;
    public long TickCount { get; private set; } = 1;

    public FCPUAccessable Accessables { get; protected set; } //Registers and memory
    public FCPUSystemRegisters SystemRegisters { get; protected set; } //Iternal Registers

    public Dictionary<string, int> ProgramCompiledLabels = new Dictionary<string, int>();
    public List<ProgramCompiledLine> ProgramCompiledLines = new List<ProgramCompiledLine>();
    public bool[] ProgramBreakpointStates = new bool[MAX_LINE];

    public interface FCPUExecContext
    {
      FCPUAccessable Accessables { get; }
      FCPUInputOutput InputOutput { get; }
      FCPUSystemRegisters SystemRegisters { get; }

      void Jump(short address);
      void Call(short address);
      void Ret();
      FCPUData WrapNum(long v);
      void StackPush(ProgramOpParam v);
      void StackPop(ProgramOpParam v);
      FCPUData Read(ProgramOpParam param);
      void Write(ProgramOpParam param, FCPUData v);
    }

    [JsonObject]
    public class FCPURestoreable
    {
      [JsonProperty]
      public FCPUState State = FCPUState.NotStart;
      [JsonProperty]
      public FCPUAccessable Accessables;
      [JsonProperty]
      public FCPUInputOutput InputOutput;
      [JsonProperty]
      public FCPUSystemRegisters SystemRegisters;
    }
    [JsonObject]
    public class FCPUPSW
    {
      private readonly FCPUExecContext context;
      public FCPUPSW(FCPUExecContext context)
      {
        this.context = context;
      }

      [JsonProperty]
      public short CF = 0; //Carry Flag
      [JsonProperty]
      public short PF = 0; //Parity Flag
      [JsonProperty]
      public short AF = 0; //Auxiliary Carry Flag
      [JsonProperty]
      public short ZF = 0; //Zero Flag
      [JsonProperty]
      public short SF = 0; //Sign Flag
      [JsonProperty]
      public short OF = 0; //Overflow Flag
      [JsonProperty]
      public short TF = 0; //TrapFlag

      public void SetAll(FCPUData result)
      {
        SetZF(result)
        .SetCF(result)
        .SetOF(result)
        .SetPF(result)
        .SetSF(result);
      }
      public FCPUPSW SetCF(FCPUData result)
      {
        CF = (short)(result.IsCarry ? 1 : 0);
        return this;
      }
      public FCPUPSW SetOF(FCPUData result)
      {
        OF = (short)(result.IsOverflow ? 1 : 0);
        return this;
      }
      public FCPUPSW SetPF(FCPUData result)
      {
        int c = 0;
        long v = result.Value;
        for (int i = 0; i < 32; i++)
        {
          if ((v & 0x1) == 1)
            c++;
          v >>= 1;
        }
        PF = (short)(((c % 2) == 0) ? 1 : 0);
        return this;
      }
      public FCPUPSW SetSF(FCPUData result)
      {
        SF = (short)(result.Value < 0 ? 1 : 0);
        return this;
      }
      public FCPUPSW SetZF(FCPUData result)
      {
        ZF = (short)(result.Value == 0 ? 1 : 0);
        return this;
      }

      public void Reset()
      {
        CF = 0;
        PF = 0;
        AF = 0;
        ZF = 0;
        SF = 0;
        OF = 0;
      }
      public int Read()
      {
        return OF << 11 | TF << 8 | SF << 7 | ZF << 6 | AF << 4 | PF << 2 | (int)CF;
      }
    }
    [JsonObject]
    public class FCPUAccessable
    {
      [JsonProperty]
      public long[] Register = new long[8];
      [JsonProperty]
      public long[] StackAndHeap = new long[256];
      [JsonProperty]
      public int StackSize = 256;
      public virtual void Reset()
      {
        for (int i = 0; i < Register.Length; i++)
          Register[i] = 0;
        for (int i = 0; i < StackAndHeap.Length; i++)
          StackAndHeap[i] = 0;
      }
    }
    [JsonObject]
    public class FCPUSystemRegisters
    {
      private readonly FCPUExecContext context;
      public FCPUSystemRegisters(FCPUExecContext context)
      {
        this.context = context;
        PSW = new FCPUPSW(context);
      }

      [JsonProperty]
      public FCPUPSW PSW = null;
      [JsonProperty]
      public short PC = 0;
      [JsonProperty]
      public short SP = 0;
      [JsonProperty]
      public short ACC = 0;
      [JsonProperty]
      public short A = 0;
      [JsonProperty]
      public short B = 0;
      [JsonProperty]
      public short C = 0;
      public virtual void Reset()
      {
        PSW.Reset();
        PC = 0;
        SP = 0;
        ACC = 0;
        A = 0;
        B = 0;
        C = 0;
      }
    }

    public class FCPUData
    {
      private long _Value;
      private bool _Overflow = false;
      private bool _Carry = false;

      private const int DATA_POOL_SIZE = 32;
      private static Stack<FCPUData> dataPool = new Stack<FCPUData>();
      private static List<FCPUData> dataUsed = new List<FCPUData>();

      public static FCPUData AllocData(long v, short b)
      {
        var data = dataPool.Pop();
        data.Value = v;
        data.MaxBit = b;
        dataUsed.Add(data);
        return data;
      }
      public static void Initialize()
      {
        for (int i = 0; i < DATA_POOL_SIZE; i++)
          dataPool.Push(new FCPUData());
      }
      public static void CollectUsedData()
      {
        for (int i = 0; i < dataUsed.Count; i++)
          dataPool.Push(dataUsed[i]);
        dataUsed.Clear();
      }

      public short MaxBit;
      public short ShortValue
      {
        get
        {
          return (short)_Value;
        }
      }
      public long Value
      {
        get
        {
          switch (MaxBit)
          {
            case 8: return (char)_Value;
            case 16: return (short)_Value;
            case 32: return (int)_Value;
            case 64: return _Value;
          }
          return _Value;
        }
        set
        {
          _Value = value;
        }
      }
      public long RawValue
      {
        get
        {
          return _Value;
        }
      }
      public bool IsOverflow
      {
        get
        {
          switch (MaxBit)
          {
            case 8: return _Overflow || _Value > char.MaxValue;
            case 16: return _Overflow || _Value > short.MaxValue;
            case 32: return _Overflow || _Value > int.MaxValue;
            case 64: return _Overflow || _Value > long.MaxValue;
          }
          return _Overflow;
        }
      }
      public bool IsCarry
      {
        get
        {
          return _Carry;
        }
      }

      public FCPUData Neg()
      {
        _Value = -_Value;
        return this;
      }

      public static FCPUData operator +(FCPUData a, FCPUData b)
      {
        var result = AllocData(a.Value + b.Value, a.MaxBit);
        if (a.Value > 0 && b.Value > 0 && result.Value < 0)
          result._Overflow = true;
        else if (a.Value < 0 && b.Value < 0 && result.Value > 0)
          result._Overflow = true;
        if ((result.RawValue & (0x1 << a.MaxBit)) == 1)
          result._Carry = true;
        return result;
      }
      public static FCPUData operator -(FCPUData a, FCPUData b)
      {
        b.Neg();
        return a + b;
      }
      public static FCPUData operator *(FCPUData a, FCPUData b)
      {
        return AllocData(a.Value * b.Value, a.MaxBit);
      }
      public static FCPUData operator /(FCPUData a, FCPUData b)
      {
        return AllocData(a.Value / b.Value, a.MaxBit);
      }
      public static FCPUData operator %(FCPUData a, FCPUData b)
      {
        return AllocData(a.Value % b.Value, a.MaxBit);
      }
      public static FCPUData operator |(FCPUData a, FCPUData b)
      {
        return AllocData(a.Value | b.Value, a.MaxBit);
      }
      public static FCPUData operator &(FCPUData a, FCPUData b)
      {
        return AllocData(a.Value & b.Value, a.MaxBit);
      }
      public static FCPUData operator ^(FCPUData a, FCPUData b)
      {
        return AllocData(a.Value ^ b.Value, a.MaxBit);
      }
      public static FCPUData operator ~(FCPUData a)
      {
        return AllocData(~a.Value, a.MaxBit);
      }
    }

    public void Jump(short address)
    {
      SystemRegisters.PC = address;
    }
    public void Call(short address)
    {
      StackPush(SystemRegisters.PC);
      SystemRegisters.PC = address;
    }
    public void Ret()
    {
      SystemRegisters.PC = (short)StackPop();
    }
    public FCPUData MemoryRead(long address)
    {
      return WrapNum(Accessables.StackAndHeap[address]);
    }
    public void MemoryWrite(long address, FCPUData value)
    {
      Accessables.StackAndHeap[address] = value.Value;
    }
    public FCPUData StackRead()
    {
      return WrapNum(Accessables.StackAndHeap[SystemRegisters.SP]);
    }
    public void StackPush(ProgramOpParam param)
    {
      StackPush(Read(param).Value);
    }
    public void StackPush(long value)
    {
      SystemRegisters.SP++;
      if (SystemRegisters.SP >= Accessables.StackSize)
        throw new Exception("Stack overflow");
      Accessables.StackAndHeap[SystemRegisters.SP] = value;
    }
    public long StackPop()
    {
      var val = StackRead().Value;
      SystemRegisters.SP--;
      return val;
    }
    public void StackPop(ProgramOpParam v)
    {
      Write(v, StackRead());
      SystemRegisters.SP--;
    }
    public FCPUData Read(ProgramOpParam param)
    {
      if (param.Type == ProgramLegend.Address || param.Type == ProgramLegend.Data)
        return WrapNum(param.Value);
      if (param.Readable)
      {
        switch (param.Type)
        {
          case ProgramLegend.Register: return WrapNum(Accessables.Register[param.Value]);
          case ProgramLegend.IndirectRegister: return MemoryRead(Accessables.Register[param.Value]);
          case ProgramLegend.Direct: return MemoryRead(param.Value);
          case ProgramLegend.IO: return WrapNum(InputOutput.Read(param.Value));
          case ProgramLegend.Sfr:
            {
              switch (ProgramInternalSfrOpDefines[param.Value])
              {
                case "PSW": return WrapNum(SystemRegisters.PSW.Read());
                case "A": return WrapNum(SystemRegisters.A);
                case "B": return WrapNum(SystemRegisters.B);
                case "C": return WrapNum(SystemRegisters.C);
                case "ACC": return WrapNum(SystemRegisters.ACC);
                case "SP": return WrapNum(SystemRegisters.SP);
                case "PC": return WrapNum(SystemRegisters.PC);
              }
              break;
            }
        }
      }
      throw new Exception($"{param} is not readable");
    }
    public void Write(ProgramOpParam param, FCPUData v)
    {
      if (param.Writeable)
      {
        switch (param.Type)
        {
          case ProgramLegend.Register: Accessables.Register[param.Value] = v.Value; return;
          case ProgramLegend.Direct: MemoryWrite(param.Value, v); return;
          case ProgramLegend.IO: InputOutput.Write(param.Value, (int)v.Value); return;
          case ProgramLegend.Sfr:
            {
              switch (ProgramInternalSfrOpDefines[param.Value])
              {
                case "A": SystemRegisters.A = (short)v.Value; return;
                case "B": SystemRegisters.B = (short)v.Value; return;
                case "C": SystemRegisters.C = (short)v.Value; return;
                case "ACC": SystemRegisters.ACC = (short)v.Value; return;
                case "SP": SystemRegisters.SP = (short)v.Value; return;
                case "PC": SystemRegisters.PC = (short)v.Value; return;
              }
              break;
            }
        }
      }
      throw new Exception($"{param} is not writable");
    }

    #endregion

    #region Program Define

    public enum ProgramLegend
    {
      None,
      Data,    //value: integer constant in range [-2^31..2^31), (-3500)
      Register,//register: (reg1, r3, ..., reg8)
      Direct,  //memory address
      Sfr,     //special function register
      IO,      //IO Port
      Address, //rom address
      IndirectRegister,
    }

    public delegate void ProgramOpExecute(FCPUExecContext context, ProgramCompiledLine program);
    public delegate FCPUData ProgramOpArithmeticTwoExecute(FCPUExecContext context, FCPUData val1, FCPUData val2);
    public delegate bool ProgramOpJumpConditionExecute(FCPUExecContext context);

    public class ProgramOpParam
    {
      public ProgramLegend Type { get; private set; } = ProgramLegend.None;
      public int Value { get; private set; }
      public bool Readable { get; private set; }
      public bool Writeable { get; private set; }

      public ProgramOpParam(ProgramLegend type, int value)
      {
        Value = value;
        Type = type;
        Readable = CheckParamType(ProgramOpParamTypeReadable, type);
        Writeable = CheckParamType(ProgramOpParamTypeWriteable, type) || (type == ProgramLegend.Sfr && CheckSfrWriteable((short)value));
      }

      public override string ToString()
      {
        switch (Type)
        {
          case ProgramLegend.Data: return $"#{Value}";
          case ProgramLegend.Register: return $"R{Value}";
          case ProgramLegend.IndirectRegister: return $"@R{Value}";
          case ProgramLegend.Sfr: return ProgramInternalSfrOpDefines[Value];
          case ProgramLegend.Direct: return $"{Value}";
          case ProgramLegend.IO: return $"P{Value}";
        }
        return "None";
      }
    }
    public class ProgramCompiledLine
    {
      public ProgramOpcode Opcode = ProgramOpcode.Nop;
      public ProgramOpParam OpFirst = null;
      public ProgramOpParam OpSecond = null;
      public ProgramOpDefine Op = null;
      public string OpLabel = null;

      public ProgramCompiledLine(string label)
      {
        Opcode = ProgramOpcode.Label;
        OpLabel = label;
      }
      public ProgramCompiledLine(ProgramOpcode code)
      {
        Opcode = code;
      }
      public ProgramCompiledLine(ProgramOpcode code, ProgramOpParam opFirst)
      {
        OpFirst = opFirst;
        Opcode = code;
      }
      public ProgramCompiledLine(ProgramOpcode code, ProgramOpParam opFirst, ProgramOpParam opSecond)
      {
        OpFirst = opFirst;
        OpSecond = opSecond;
        Opcode = code;
      }

      public override string ToString()
      {
        if (Opcode == ProgramOpcode.Label)
          return ":" + OpLabel;
        return Opcode.ToString().ToUpper()
          + (OpFirst != null ? " " + OpFirst.ToString() : "")
          + (OpSecond != null ? " " + OpSecond.ToString() : "");
      }

      public void Execute(FCPUExecContext context)
      {
        Op?.Execute?.Invoke(context, this);
      }
    }
    public class ProgramOpDefine
    {
      public ProgramOpDefine(ProgramOpcode code)
      {
        Op = code.ToString().ToUpper();
        Opcode = code;
      }

      public string Op;
      public ProgramOpcode Opcode;
      public ProgramOpExecute Execute;
      public int RequiredParam = 0;
      public ProgramLegend[] ParamFirstType;
      public ProgramLegend[] ParamSecondType;

      public bool CheckFirstParamType(ProgramOpParam op)
      {
        return CheckParamType(ParamFirstType, op.Type);
      }
      public bool CheckSecondParamType(ProgramOpParam op)
      {
        return CheckParamType(ParamSecondType, op.Type);
      }
    }
    public class ProgramOpArithmeticTwoDefine : ProgramOpDefine
    {
      public ProgramOpArithmeticTwoDefine(ProgramOpcode code, ProgramOpArithmeticTwoExecute execute, int requiredParam = 2) : base(code)
      {
        RequiredParam = requiredParam;
        ParamFirstType = ProgramOpParamTypeWriteable;
        ParamSecondType = ProgramOpParamTypeReadable;
        Execute = (context, program) =>
        {
          var val1 = context.Read(program.OpFirst);
          var val2 = requiredParam == 2 ? context.Read(program.OpSecond) : null;
          var result = execute(context, val1, val2);
          context.SystemRegisters.PSW.SetAll(result);
          context.Write(program.OpFirst, result);
        };
      }
    }
    public class ProgramOpJumpConditionDefine : ProgramOpDefine
    {
      public ProgramOpJumpConditionDefine(ProgramOpcode code, ProgramOpJumpConditionExecute execute) : base(code)
      {
        RequiredParam = 1;
        ParamFirstType = ProgramOpParamTypeAddress;
        Execute = (context, program) =>
        {
          if (execute(context))
            context.Jump(context.Read(program.OpFirst).ShortValue);
        };
      }
    }

    private static Dictionary<string, ProgramOpDefine> ProgramOpDefines = new Dictionary<string, ProgramOpDefine>();
    private static List<string> ProgramInternalSfrOpDefines = new List<string>() { "PSW", "A", "B", "C", "ACC", "SP", "PC" };
    private static List<string> ProgramInternalSfrOpWriteable = new List<string>() { "A", "B", "C", "ACC", "SP" };
    private static ProgramLegend[] ProgramOpParamTypeAddress = new ProgramLegend[] { ProgramLegend.Address };
    private static ProgramLegend[] ProgramOpParamTypeWriteable = new ProgramLegend[] { ProgramLegend.Register, ProgramLegend.Direct, ProgramLegend.IO };
    private static ProgramLegend[] ProgramOpParamTypeReadable = new ProgramLegend[] { ProgramLegend.Register, ProgramLegend.Sfr, ProgramLegend.IndirectRegister, ProgramLegend.Direct, ProgramLegend.Data, ProgramLegend.IO };

    public static bool CheckParamType(ProgramLegend[] types, ProgramLegend opType)
    {
      if (types == null)
        return true;
      foreach (var item in types)
        if (item == opType)
          return true;
      return false;
    }
    public static bool CheckSfrWriteable(short sfr)
    {
      var sfrName = ProgramInternalSfrOpDefines[sfr];
      return ProgramInternalSfrOpWriteable.Contains(sfrName);
    }

    private void AddProgramOpDefines(ProgramOpDefine define)
    {
      ProgramOpDefines.Add(define.Op, define);
    }

    #endregion

    #region All Op Define

    public enum ProgramOpcode
    {
      Nop = 0,    //nop
      Label,      //:label
      Rst,        //Rst                          - Clear all registers and memory.
      Hlt,        //hlt                          - Halt cpu.
      Push,       //push d                       - Push into stack.
      Pop,        //pop d                        - Pop from stack.
      Int,

      In,
      Out,

      Mov = 10,    //mov dst...[R/O] src[V/I/O/R] - Copy signal from source to destination
      Xchg,        //xch reg1[R] reg2[R]          - Swap signals in memory cells.
      Xchd,
      Swap,

      Setb,        //
      Clr,         //
      Cpl,         //

      Add = 20,    //Calc ops
      Sub,
      Cmp,
      Inc,
      Dec,
      Mul,
      Div,
      Mod,
      Neg,

      And = 40,    //Logic ops
      Or,
      Xor,
      Not,
      Test,
      Shl,
      Sal,
      Shr,
      Sar,
      Rol,
      Ror,
      Rl,
      Rr,
      Rcl,
      Rcr,

      Jmp = 50,   //Jump ops  jmp [A] - Jump to address.
      Call,
      Ret,
      Jz,
      Jnz,
      Je,
      Jne,
      Ja,
      Jna,
      Jb,
      Jnb,
      Jc,
      Jnc,
      Jae,
      Jnae,
      Jbe,
      Jnbe,
      Jg,
      Jge,
      Jng,
      Jnge,
      Jl,
      Jnl,
      Jo,
      Jno,
      Js,
      Jns,
      Jp,
      Jnp,

      Loop,
      Max,
    }

    public FCPUData WrapNum(long v)
    {
      return FCPUData.AllocData(v, DataBits);
    }
    private void InitProgramOpDefines()
    {
      if (ProgramOpDefines.Count == 0)
      {
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Nop));
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Rst)
        {
          Execute = (context, program) => ExecuteReset()
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Hlt)
        {
          Execute = (context, program) => {
            State = FCPUState.HaltByUser;
          }
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Push)
        {
          RequiredParam = 1,
          ParamFirstType = ProgramOpParamTypeReadable,
          Execute = (context, program) => context.StackPush(program.OpFirst)
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Pop)
        {
          RequiredParam = 1,
          ParamFirstType = ProgramOpParamTypeWriteable,
          Execute = (context, program) => context.StackPop(program.OpFirst)
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Mov)
        {
          RequiredParam = 2,
          ParamFirstType = ProgramOpParamTypeWriteable,
          ParamSecondType = ProgramOpParamTypeReadable,
          Execute = (context, program) => context.Write(program.OpFirst, context.Read(program.OpSecond))
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Xchg)
        {
          RequiredParam = 2,
          ParamFirstType = ProgramOpParamTypeWriteable,
          ParamSecondType = ProgramOpParamTypeWriteable,
          Execute = (context, program) =>
          {
            var val1 = context.Read(program.OpFirst);
            var val2 = context.Read(program.OpSecond);
            context.Write(program.OpFirst, val2);
            context.Write(program.OpSecond, val1);
          }
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Xchd)
        {
          RequiredParam = 2,
          ParamFirstType = ProgramOpParamTypeWriteable,
          ParamSecondType = ProgramOpParamTypeWriteable,
          Execute = (context, program) =>
          {
            short val1 = context.Read(program.OpFirst).ShortValue;
            short val2 = context.Read(program.OpSecond).ShortValue;
            short val1low = (short)(val1 & 0xf);
            short val1high = (short)(val1 & 0xf0);
            short val2low = (short)(val1 & 0xf);
            short val2high = (short)(val1 & 0xf0);
            context.Write(program.OpFirst, context.WrapNum(val1high & val2low));
            context.Write(program.OpSecond, context.WrapNum(val2high & val1low));
          }
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Swap)
        {
          RequiredParam = 1,
          ParamFirstType = ProgramOpParamTypeWriteable,
          ParamSecondType = ProgramOpParamTypeWriteable,
          Execute = (context, program) =>
          {
            short val = context.Read(program.OpFirst).ShortValue;
            short low = (short)(val & 0xf);
            short high = (short)(val & 0xf0);
            context.Write(program.OpFirst, context.WrapNum(low & high));
          }
        });

        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Add, (context, val1, val2) => val1 + val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Sub, (context, val1, val2) => val1 - val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Inc, (context, val1, val2) => val1 + context.WrapNum(1), 1));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Dec, (context, val1, val2) => val1 - context.WrapNum(1), 1));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Mul, (context, val1, val2) => val1 * val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Div, (context, val1, val2) => val1 / val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Mod, (context, val1, val2) => val1 % val2));

        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.And, (context, val1, val2) => val1 & val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Or, (context, val1, val2) => val1 | val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Xor, (context, val1, val2) => val1 ^ val2));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Not, (context, val1, val2) => ~val1, 1));

        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Neg)
        {
          RequiredParam = 1,
          ParamFirstType = ProgramOpParamTypeWriteable,
          Execute = (context, program) => Write(program.OpFirst, Read(program.OpFirst).Neg())
        });

        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Shl, (context, val1, val2) => context.WrapNum(val1.Value << val2.ShortValue)));
        AddProgramOpDefines(new ProgramOpArithmeticTwoDefine(ProgramOpcode.Shr, (context, val1, val2) => context.WrapNum(val1.Value >> val2.ShortValue)));

        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Cmp)
        {
          RequiredParam = 2,
          ParamFirstType = ProgramOpParamTypeReadable,
          ParamSecondType = ProgramOpParamTypeReadable,
          Execute = (context, program) =>
          {
            var val1 = context.Read(program.OpFirst);
            var val2 = context.Read(program.OpSecond);
            var result = val1 - val2;
            context.SystemRegisters.PSW.SetAll(result);
          }
        });

        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Jmp)
        {
          RequiredParam = 1,
          ParamFirstType = ProgramOpParamTypeAddress,
          Execute = (context, program) => context.Jump(context.Read(program.OpFirst).ShortValue)
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Call)
        {
          RequiredParam = 1,
          ParamFirstType = ProgramOpParamTypeAddress,
          Execute = (context, program) => context.Call(context.Read(program.OpFirst).ShortValue)
        });
        AddProgramOpDefines(new ProgramOpDefine(ProgramOpcode.Ret)
        {
          RequiredParam = 0,
          Execute = (context, program) => context.Ret()
        });
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Je, (context) => context.SystemRegisters.PSW.ZF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jne, (context) => context.SystemRegisters.PSW.ZF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jz, (context) => context.SystemRegisters.PSW.ZF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jnz, (context) => context.SystemRegisters.PSW.ZF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jo, (context) => context.SystemRegisters.PSW.OF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jno, (context) => context.SystemRegisters.PSW.OF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Js, (context) => context.SystemRegisters.PSW.SF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jns, (context) => context.SystemRegisters.PSW.SF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jp, (context) => context.SystemRegisters.PSW.PF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jnp, (context) => context.SystemRegisters.PSW.PF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jc, (context) => context.SystemRegisters.PSW.CF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jnc, (context) => context.SystemRegisters.PSW.CF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jb, (context) => context.SystemRegisters.PSW.CF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jnb, (context) => context.SystemRegisters.PSW.CF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Ja, (context) => context.SystemRegisters.PSW.CF == 1 && context.SystemRegisters.PSW.ZF == 0));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jna, (context) => !(context.SystemRegisters.PSW.CF == 1 && context.SystemRegisters.PSW.ZF == 0)));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jae, (context) => context.SystemRegisters.PSW.CF == 0 || context.SystemRegisters.PSW.ZF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jnae, (context) => !(context.SystemRegisters.PSW.CF == 0 || context.SystemRegisters.PSW.ZF == 1)));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jbe, (context) => context.SystemRegisters.PSW.CF == 1 || context.SystemRegisters.PSW.ZF == 1));
        AddProgramOpDefines(new ProgramOpJumpConditionDefine(ProgramOpcode.Jnbe, (context) => !(context.SystemRegisters.PSW.CF == 1 || context.SystemRegisters.PSW.ZF == 1)));
      
      
      }
    }

    #endregion

    #region Compile Program

    public override string CompileProgram(string program)
    {
      if (program == null)
        program = "";
      program = program.Replace("\r", "").Trim();

      ExecuteReset();
      ProgramCompiledLabels.Clear();
      ProgramCompiledLines.Clear();

      var lines = program.Split('\n');
      int line = 0, line2 = 0;

      try
      {
        //Label
        foreach (var item in lines)
        {
          line2++;
          if (item.StartsWith(":"))
          {
            var label = item.Substring(1).ToUpper();
            ProgramCompiledLabels.Add(label, line);
            continue;
          }
          line++;
        }

        //Program
        line2 = 0;
        line = 0;
        foreach (var str in lines)
        {
          line2++;
          var item = str;

          //Remove comment
          var commentIndex = item.IndexOf(";");
          if (commentIndex >= 0)
            item = item.Substring(0, commentIndex);

          if (string.IsNullOrWhiteSpace(item))
          {
            ProgramCompiledLines.Add(new ProgramCompiledLine(ProgramOpcode.Nop));
            continue;
          }
          if (item.StartsWith(":"))
          {
            var label = item.Substring(1).ToUpper();
            ProgramCompiledLines.Add(new ProgramCompiledLine(label));
            continue;
          }
          var block = item.Split(' ');
          var op = block[0].ToUpper();

          ProgramOpDefine matchedOp = null;
          if (!ProgramOpDefines.TryGetValue(op, out matchedOp))
            throw new Exception($"Unknown opcode {op}, may not implemented");
          if ((block.Length == 1 || string.IsNullOrEmpty(block[1])) && matchedOp.RequiredParam > 0)
            throw new Exception($"Opcode {op} require {matchedOp.RequiredParam} params");

          ProgramOpParam op1 = null;
          ProgramOpParam op2 = null;

          if (block.Length > 1)
          {
            var param = block[1].Split(',');
            if (param.Length != matchedOp.RequiredParam)
              throw new Exception($"Opcode {op} require {matchedOp.RequiredParam} params but only provided {param.Length}");

            op1 = ParseProgramOpParam(param[0]);
            if (!matchedOp.CheckFirstParamType(op1))
              throw new Exception($"Opcode {op} param 1 incompatible");

            if (param.Length > 1)
            {
              op2 = ParseProgramOpParam(param[1]);
              if (!matchedOp.CheckSecondParamType(op2))
                throw new Exception($"Opcode {op} param 2 incompatible");
            }
          }
          var compiledLine = new ProgramCompiledLine(matchedOp.Opcode, op1, op2);
          compiledLine.Op = matchedOp;
          ProgramCompiledLines.Add(compiledLine);
          line++;
        }

        return "";
      }
      catch (Exception e)
      {
        return e.Message + $", at line {line2}";
      }
    }

    private int ParseProgramNumber(string rawProgram)
    {
      if (int.TryParse(rawProgram, out int a))
        return a;
      throw new Exception($"Unknow number fromat {rawProgram}");
    }
    private ProgramOpParam ParseProgramOpParam(string rawProgram)
    {
      rawProgram = rawProgram.ToUpper().Trim();

      //Data and Label
      if (rawProgram.StartsWith("#"))
        return new ProgramOpParam(ProgramLegend.Data, ParseProgramNumber(rawProgram.Substring(1)));
      else if (rawProgram.StartsWith("@R"))
        return new ProgramOpParam(ProgramLegend.IndirectRegister, ParseProgramNumber(rawProgram.Substring(2)));
      else if (rawProgram.StartsWith("@REG"))
        return new ProgramOpParam(ProgramLegend.IndirectRegister, ParseProgramNumber(rawProgram.Substring(4)));
      else if (rawProgram.StartsWith("P"))
        return new ProgramOpParam(ProgramLegend.IO, ParseProgramNumber(rawProgram.Substring(1)));
      else if (rawProgram.StartsWith(":"))
      {
        var label = rawProgram.Substring(1);
        if (ProgramCompiledLabels.TryGetValue(label, out int address))
          return new ProgramOpParam(ProgramLegend.Address, address);
        else
          throw new Exception($"Unknow label {label}");
      }

      //Sfr
      var index = ProgramInternalSfrOpDefines.FindIndex((p) => p == rawProgram);
      if (index >= 0)
        return new ProgramOpParam(ProgramLegend.Sfr, index);

      //Register
      if (rawProgram.StartsWith("R"))
        return new ProgramOpParam(ProgramLegend.Register, ParseProgramNumber(rawProgram.Substring(1)));
      if (rawProgram.StartsWith("REG"))
        return new ProgramOpParam(ProgramLegend.Register, ParseProgramNumber(rawProgram.Substring(3)));

      throw new Exception($"Unknow param {rawProgram}");
    }

    #endregion

    #region UI Update state

    public void UpdateBreakPointState(string breakPointStateStr)
    {
      int i;
      for (i = 0; i < FCPUExecutor.MAX_LINE; i++)
        ProgramBreakpointStates[i] = false;
      if (!string.IsNullOrEmpty(breakPointStateStr))
      {
        var strs = breakPointStateStr.Split(',');
        i = 0;
        foreach (string str in strs)
        {
          if (i >= ProgramBreakpointStates.Length)
            break;
          ProgramBreakpointStates[i] = str == "1";
          i++;
        }
      }
    }

    #endregion

    private void ResetAll()
    {
      TickCount = 0;
      Accessables.Reset();
      InputOutput.Reset();
      SystemRegisters.Reset();
    }

    public override void Init()
    {
      FCPUData.Initialize();
      InitProgramOpDefines();
    }
    public override void Restore(string json)
    {
      FCPURestoreable restoreData = JsonConvert.DeserializeObject<FCPURestoreable>(json);
      if (restoreData != null)
      {
        if (restoreData.Accessables != null) Accessables = restoreData.Accessables;
        if (restoreData.InputOutput != null) InputOutput.CopyValuesFrom(restoreData.InputOutput);
        if (restoreData.SystemRegisters != null) SystemRegisters = restoreData.SystemRegisters;
        State = restoreData.State;
      }
    }
    public override string Save()
    {
      FCPURestoreable restoreData = new FCPURestoreable();
      restoreData.Accessables = Accessables;
      restoreData.InputOutput = InputOutput;
      restoreData.SystemRegisters = SystemRegisters;
      restoreData.State = State;
      return JsonConvert.SerializeObject(restoreData);
    }
    public override void Start()
    {
      base.Start();
      State = FCPUState.Looping;
    }
    public override void Stop()
    {
      State = FCPUState.HaltByUser;
      base.Stop();
    }
    public override void ExecuteReset()
    {
      ResetAll();
      State = FCPUState.NotStart;
    }
    public override void ExecuteTick()
    {
      if (ProgramCompiledLines.Count == 0)
      {
        HaltAndReportError("Empty Program");
        return;
      }
      if (SystemRegisters.PC > ProgramCompiledLines.Count - 1)
        return;

      FCPUData.CollectUsedData();
      InputOutput.FlushInputs();
      try
      {
        onExecute?.Invoke(SystemRegisters.PC);
        ProgramCompiledLines[SystemRegisters.PC].Execute(this);

        SystemRegisters.PC++;
        TickCount++;

        if (ProgramBreakpointStates[SystemRegisters.PC])
        {
          onBreakpoint.Invoke(SystemRegisters.PC);
          State = FCPUState.HaltByUser;
        }

        onSave?.Invoke();
      }
      catch (Exception e)
      {
        HaltAndReportError(e.Message);
      }
    }
    public override void ExecuteTicks()
    {
      if (State == FCPUState.Looping)
      {
        if (SystemRegisters.PC >= ProgramCompiledLines.Count - 1)
          SystemRegisters.PC = 0;
        int maxTick = FrameExecuteTickCount;
        while (maxTick > 0)
        {
          if (State != FCPUState.Looping)
            return;
          ExecuteTick();
          maxTick--;
        }
      }
    }

  }
}
