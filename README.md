ONICPU
---

## 简介

ONICPU 是游戏缺氧的一个CPU模组，支持你建造可编程的代码执行控制单元，用它来实现复杂的自动化功能。

目前原版游戏只有简单的逻辑门自动化，这在实现一些很大型的电路时，会非常复杂，因此设计了这个模组，使用代码来控制自动化。

主要功能：

- CPU运算单元，支持4输入4输出IO，8输入8输出IO
- CPU支持使用汇编代码或者JavaScript代码进行控制
- 拓展线组读取器、线组写入器使其支持传输32位数值
- 拓展一系列传感器，使其可以直接读取数值，传给CPU处理
- 数码管显示

## 使用方法

### CPU

你可以在自动化菜单中找到CPU（需要先解锁自动化科技），目前有两种大小的CPU：

* 4输入4输出IO
* 8输入8输出IO

CPU左侧是输入端口，程序可以读取它的值，右侧是输出端口，可以输出值，控制下游连接的设备。
CPU使用一个控制端口（中下位置）来进行控制，只有在控制端口接收到绿色信号时，单元才会启动，启动状态下，你的程序会以每秒5次的频率执行，在每个执行周期中，你可以读取输入参数，进行自定义计算，然后修改输出端口的值。

因为游戏代码限制，所有CPU端口最大支持32bit的数值（Int32）

每种CPU都有两个版本，有（JavaScript）后缀的表示这个CPU使用JavaScript代码进行控制。JavaScript简单易上手，汇编比较复杂但有趣，你可以选择自己喜欢的。

注意: 在代码中你不能使用while循环来做延迟，这会导致游戏卡住，应该使用变量计数器来实现延时。

### 代码编辑器

你可以在选中CPU单元后，在右侧菜单中点击“编写程序”，来打开编辑器，在编辑器中：

* 顶部显示的是程序当前的状态信息
* 在左侧输入框中编写代码
* 右上角按钮控制程序运行、暂停。
* 右侧显示的是实时的程序IO数值
  * 在汇编模式中，右侧还会显示寄存器、内存数据
  * 在JavaScript模式中，右侧还会显示程序输出日志信息

建议你可以在Visual Studio Code或者其他代码编辑器软件中编写好代码再复制到游戏中。

点击“Compile”可以立即编译程序，编译会临时暂停运行程序，可以再点击上方运行按钮开始运行，点击“Close”可以编译程序然后关闭。

提示：CPU是否运行是根据控制端口是否有绿色信号来判断的，但你在代码编辑器中手动点击运行时不受信号限制，仍然会运行。

### 数字化传感器

本模组拓展了一系列传感器，使其可以直接读取数值，，你可以将其连接至CPU，然后就可以读取数值来进行相关处理了。

### 数码管显示器

本模组添加了数值显示器，你可以用它来显示网络中的数值。分为3档，分别可以显示 8位、16位、32位整数。

### JavaScript

当你选择有（JavaScript）后缀的CPU后，你就可以开始用JavaScript写代码了。你的程序会以下方3个主要函数来组织：

* start：程序启动时执行
* stop：程序停止时执行
* tick：程序在运行中每逻辑帧执行

tick 函数必须存在，否则CPU单元会停止执行，start和stop可选。

```
function start() {
  //代码。。。
}
function stop() {
  //代码。。。
}
function tick() {
  //主要代码。。。
}
```

你的主要程序应该写在 tick 函数中，tick 函数的推荐逻辑应该是，读取数据>处理数据>输出数据。

例如，下方是一个简单的Demo：
* 我将P0连接至一个温度传感器，P1连接至一个压力传感器，P4连接至一个加热设备
* 我希望在温度传感器温度在低于15度或者大于35度，并且压力在1000kg以上时，开启加热设备
则代码可以这样写：
```
function tick() {
  io.P4 = (io.P0 < 15 || io.P0 > 35) && io.P1 > 1000;
}
```
以上代码会每帧执行判断，然后输出结果至P4所连接的加热设备，从而实现需求。

你还可以根据更多条件来实现更多复杂功能，下方是CPU单元一些支持的功能说明：
    
输入和输出: 你可以使用 io.PX 读取或设置引脚值。例如:
  * 读P0输入: let pin0value = io.P0;
  * 设置输出: io.P4 = 1;
  注意：游戏会在名称start、stop、tick执行完毕后将修改后的输出值同步到信号网络。

存储数据：
  注意：你的代码中其他的位置声明变量，在每次编译/上电/游戏重新加载回丢失，需要手动加载。
  系统内置了 storage 对象，在这个对象下的数据会被游戏保存，可以在下次存档加载后读取
  storage.xxx = 1;

一些辅助函数
* __getbit: 你可以调用 __getbit(io.P0, 1) 得到输入引脚的1位值。
* __changebit: 你可以调用 io.P3 = __changebit(io.P3, 1, 0) 设置输出引脚的1位值=0。
* __log: 你可以调用' __log('message') '来记录编辑器中的一些消息。
* __reset: 调用 __reset() 来重置所有输出IO
* __shutdown: 你可以调用 __shutdown('error message') 来关闭这个CPU单元。

### 汇编

CPU支持使用汇编代码。但汇编比较难，推荐还是使用 JavaScript，也可以用汇编，因为挺好玩的，但在写代码之前，你需要学习汇编相关知识。

本CPU支持的是类C51汇编（部分子集，和标准不一样），所有计算指令、寄存器大小均为32位。
本文只说明CPU指令相关支持情况和特殊说明，汇编相关知识你需要到互联网上搜索相关资料。

指令格式

[操作码] [操作数] 例如：  MOV A, P0
:label  标签

支持的寻址方式

* 立即数寻址 #20H 例如：MOV A,#20H 
* 寄存器寻址 PSW / A / R1 例如：MOV A,R1 
* 寄存器间接寻址 @R1 / @REG1 例如：MOV A,@R1 
* 跳转寻址 :label 例如：jne :loop

支持的寄存器

* R0-R7 内置通用寄存器
* PSW 程序状态字寄存器 （与8086定义一致）
* A
* B
* C
* ACC 累加寄存器
* SP 堆栈指针寄存器
* PC 程序计数器

支持的指令表

无操作数指令

* NOP
* RST
* HLT
* POP

1操作数指令

* SWAP
* PUSH
* NEG
* JMP
* JE
* JNE
* JZ
* JO
* JNO
* JS
* JNS
* JP
* JNP
* JC
* JNC
* JB
* JNB
* JA
* JNA
* JAE
* JNAE
* JBE
* JNBE

2操作数指令

* MOV
* XCHG
* XCHD
* ADD
* SUB
* INC
* DEC
* MUL
* MOD
* AND
* OR
* XOR
* NOR
* MOD
* SHL
* SHR
* CMP

读取写入IO

* 读取写入IO简化了，无特殊指令，需要内置寄存器一样，直接读取/写入：MOV A, P0 或者 MOV P0, #11

示例程序

mov r0,#10
mov r1,#0
mov r2,#0
:loop
inc r2
dec r0
cmp r0,r1
jne :loop
mov p3,r2
hlt

=================

## Introduction

ONICPU is a CPU mod for OxygenNotIncluded that allows you to build programmable code execution control units to implement complex automation functions.

At present, the original game only has a simple logic gate automation, which will be very complicated when implementing some very large circuits, so i made the this mod, using code to control the automation.

Main functions:

- CPU computing unit, supporting 4 input and 4 output I/OS, 8 input and 8 output I/OS
- The CPU supports assembly code or JavaScript code for control
- Expanded the Logic ribbons /Reader/Writer to support the 32-bit values
- Expanded some sensors that can directly read values and pass them to the CPU for processing
- Digital number display

## How to use

### CPU

You can find the CPU in the Automation menu (need to unlock Automation Tech first), there are currently two sizes of CPU:

* 4 Input 4 Output I/O
* 8 Input 8 Output I/O

The left side of the CPU is the input port, the program can read its value, and the right side is the output port, which can output the value and control the downstream connected devices.
The CPU uses a control port (lower middle position) for control, and the unit will only start when the control port receives a green signal. When the CPU is on, your program will execute at a rate of 5 times per second, and during each execution cycle, you can read the input parameters, perform custom calculations, and then modify the output port value.

Due to game code restrictions, all CPU ports support a maximum of 32bit values (Int32).

There are two versions of each CPU, with the (JavaScript) suffix indicating that the CPU is controlled using JavaScript code. JavaScript is easy to use, assembly is more complex but fun, you can choose what you like.

Note: You can't use a while loop in your code to delay, it will cause the game to get stuck, you should use a variable counter to delay.

### Code editor

You can open the editor by selecting the CPU unit and clicking "Write Program" in the right menu. In the editor:

* The top display is the current status information of the program
* Write the code in the left input box
* Upper right corner button control program run, pause.
* The right side shows the real-time program IO value
* In assembly mode, registers and memory data are also displayed on the right
* In JavaScript mode, program output log information is also displayed on the right

It is recommended that you write Code in Visual Studio Code or other Code Editors and then copy it into your game.

Click "Compile" to compile the program immediately, compile will stop running the CPU unit, click "Close" to compile the program and then close.

Tip: Whether the CPU is running is judged by whether the control port has a green signal, but when you manually click Run in the code editor, it is not limited by the signal and will still run.

### Digital sensor

This mod extends a series of sensors so that it can directly read values, you can connect it to the CPU, and then you can read values for related processing.

### digital tube display

This mod adds a Digital number display, which you can use to display the values in the network. It is divided into three bulidings, which can display 8-bit, 16-bit and 32-bit integers respectively.

### JavaScript

Once you have selected the CPU with the (JavaScript) suffix, you can start writing code in JavaScript. Your program will be like this:

```
function start() {
  // Code...
}
function stop() {
  // Code...
}
function tick() {
  // Main code...
}
```

* start: Execute when the program starts
* stop: Execute when the program stops
* tick: The program executes every logical frame during runtime

The tick function must exist; otherwise, the CPU unit stops executing. start and stop are optional.

Your main program should be written in the tick function, and the recommended logic for the tick function should be Read input data > Process data > Write output data.

For example, here is a simple Demo:
* I connected P0 to a temperature sensor, P1 to a pressure sensor, and P4 to a heating device
* I want to turn on the heating equipment when the temperature sensor temperature is below 15 degrees or greater than 35 degrees and the pressure is above 1000kg
The code can be written like this:
```
function tick() {
  io.P4 = (io.P0 < 15 || io.P0 > 35) && io.P1 > 1000;
}
```
The above code performs the judgment each frame and then outputs the result to the heating device connected to P4 to fulfill the requirement.

You can also implement more complex functions according to more conditions, below is a description of some supported functions of the CPU unit:

Input and output: You can use io.PX to read or set pin values. For example:
  * Read P0 Input: let pin0value = io.P0;
  * Set output: io.P4 = 1;
  Note: The game will synchronize the modified output value to the signal network after the execution of the start, stop and tick.

Store data:
  Note: Other places in your code declare variables that are lost every time you compile/game reload and need to be loaded manually.
  The storage object is built into the system, and the data under this object is saved by the game and can be read the next time the game is loaded
  storage.xxx = 1;

Some helper functions
  * __getbit: You can call __getbit(io.P0, 1) to get the second bit value of the input pin.
  * __changebit: You can call io.P3 = __changebit(io.P3, 1, 0) to set the second bit value of the output pin to 0.
  * __log: You can call '__log('message')' to log some messages in the editor.
  * __reset: Call __reset() to reset all output IO
  * __shutdown: You can call __shutdown('error message') to shutdown the CPU unit.

### Assembly code

The CPU supports assembly code. But assembly is more difficult, Recommend using JavaScript, you can also use assembly, because it's fun, but before you write code, you need to learn about assembly.

This CPU supports class C51 like assembly (partial subset, different from the standard), and all computation instructions and registers are 32-bit.
This article only describes the CPU instruction related support and special instructions, assemble the relevant knowledge you need to search for relevant information on the Internet.

Instruction format

[Operation code] [operand] For example, MOV A, P0
:label Label

Supported addressing methods

* Immediate number addressing: #20H Example: MOV A,#20H
* Register addressing: PSW/A/R1 Example: MOV A,R1
* Register indirectly addresses: @R1 / @REG1 For example: MOV A,@R1
* Skip address: :label For example, jne :loop

Supported register

* R0-R7 Built-in general purpose register
* PSW program status word register (as defined in 8086)
* A
* B
* C
* ACC accumulates register
* SP stack pointer register
* PC program counter

Supported instruction list

No operand instruction

* NOP
* RST
* HLT
* POP

1 Operand instruction

* SWAP
* PUSH
* NEG
* JMP
* JE
* JNE
* JZ
* JO
* JNO
* JS
* JNS
* JP
* JNP
* JC
* JNC
* JB
* JNB
* JA
* JNA
* JAE
* JNAE
* JBE
* JNBE

2 Operand instruction

* MOV
* XCHG
* XCHD
* ADD
* SUB
* INC
* DEC
* MUL
* MOD
* AND
* OR
* XOR
* NOR
* MOD
* SHL
* SHR
* CMP

Read write IO

* Read write IO simplified, no special instructions, need built-in registers like, directly read/write: MOV A, P0 or MOV P0, #11

Sample program

mov r0,#10
mov r1,#0
mov r2,#0
:loop
inc r2
dec r0
cmp r0,r1
jne :loop
mov p3,r2
hlt

[Mod in Steam](https://steamcommunity.com/sharedfiles/filedetails/?id=3173524865)