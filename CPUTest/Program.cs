// See https://aka.ms/new-console-template for more information


using ONICPU;
using static ONICPU.FCPUExecutor;

/*

const string PROGRAM = @"
mov r0,#10
mov r1,#0
mov r2,#0
:loop
inc r2
dec r0
cmp r0,r1
jne :loop
mov P3,r2
hlt
";
const string PROGRAM2 = @"
mov r2,p0
inc r2
mov p4,r2
";

var executor = new FCPUExecutorAssemblyCode();
executor.InputOutput = new FCPUInputOutputBase();
executor.Init();
executor.FrameExecuteTickCount = 200;
executor.onError += Executor_onError;

var error = executor.CompileProgram(PROGRAM2);
if (error != "")
{
  Console.WriteLine("Compile program error: " + error);
  return;
}

executor.InputOutput.InputValues[0] = 999;

executor.Start();
executor.ExecuteTicks();

void Executor_onError(string err)
{
  Console.WriteLine("Executor error: " + err);
}

Console.WriteLine("r0: " + executor.Accessables.Register[0]);
Console.WriteLine("r1: " + executor.Accessables.Register[1]);
Console.WriteLine("r2: " + executor.Accessables.Register[2]);
Console.WriteLine("r3: " + executor.Accessables.Register[3]);
Console.WriteLine("r3: " + executor.InputOutput.OutputValues[0]);
Console.WriteLine("tick: " + executor.TickCount);

*/

var MAX_SEG_VALUE = 16;
var value = 899;
var displayBits = 16;
var maxCount = (displayBits == 8 ? 3 : 5);
short number1 = (displayBits == 8 ? ((short)(char)value) : ((short)value)); //8bit char; 16 bit short
short number = System.Math.Abs(number1);
short index = 0;

if (number1 == 0)
{
  short number2 = 0;
  for (; index < maxCount; index++)
  {
    Update7Seg(index, number2);
  }
  Update7Seg(-1, 1);
}
else
{
  while (number > 0)
  {
    Update7Seg(index, (short)(number % 10));
    number = (short)(number / 10);
    index++;
  }
  if (number1 < 0)
  {
    Update7Seg(index >= maxCount ? (short)-1 : index, -1);
    index++;
  }
  else
  {
    Update7Seg(-1, 1);
  }
  for (; index < maxCount; index++)
    Update7Seg(index, MAX_SEG_VALUE);
}

void Update7Seg(int i, int a)
{
  Console.WriteLine("Update7Seg: " + i + " " + a);
}