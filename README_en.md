## Introduction

ONICPU is a CPU mod for OxygenNotIncluded that allows you to build programmable code execution control units to implement complex automation functions.

At present, the original game only has a simple logic gate automation, which will be very complicated when implementing some very large circuits, so i made the this mod, using code to control the automation.

Main functions:

-   CPU computing unit, supporting 4 input and 4 output I/OS, 8 input and 8 output I/OS
-   The CPU supports assembly code or JavaScript code for control
-   Expanded the Logic ribbons /Reader/Writer to support the 32-bit values
-   Expanded some sensors that can directly read values and pass them to the CPU for processing
-   Digital number display

## How to use

### CPU

You can find the CPU in the Automation menu (need to unlock Automation Tech first), there are currently two sizes of CPU:

-   4 Input 4 Output I/O
-   8 Input 8 Output I/O

The left side of the CPU is the input port, the program can read its value, and the right side is the output port, which can output the value and control the downstream connected devices. The CPU uses a control port (lower middle position) for control, and the unit will only start when the control port receives a green signal. When the CPU is on, your program will execute at a rate of 5 times per second, and during each execution cycle, you can read the input parameters, perform custom calculations, and then modify the output port value.

Due to game code restrictions, all CPU ports support a maximum of 32bit values (Int32).

There are two versions of each CPU, with the (JavaScript) suffix indicating that the CPU is controlled using JavaScript code. JavaScript is easy to use, assembly is more complex but fun, you can choose what you like.

Note: You can't use a while loop in your code to delay, it will cause the game to get stuck, you should use a variable counter to delay.

### Code editor

You can open the editor by selecting the CPU unit and clicking "Write Program" in the right menu. In the editor:

-   The top display is the current status information of the program
-   Write the code in the left input box
-   Upper right corner button control program run, pause.
-   The right side shows the real-time program IO value
-   In assembly mode, registers and memory data are also displayed on the right
-   In JavaScript mode, program output log information is also displayed on the right

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

-   start: Execute when the program starts
-   stop: Execute when the program stops
-   tick: The program executes every logical frame during runtime

The tick function must exist; otherwise, the CPU unit stops executing. start and stop are optional.

Your main program should be written in the tick function, and the recommended logic for the tick function should be Read input data > Process data > Write output data.

For example, here is a simple Demo:

-   I connected P0 to a temperature sensor, P1 to a pressure sensor, and P4 to a heating device
-   I want to turn on the heating equipment when the temperature sensor temperature is below 15 degrees or greater than 35 degrees and the pressure is above 1000kg The code can be written like this:

```
function tick() {
  io.P4 = (io.P0 < 15 || io.P0 > 35) && io.P1 > 1000;
}
```

The above code performs the judgment each frame and then outputs the result to the heating device connected to P4 to fulfill the requirement.

You can also implement more complex functions according to more conditions, below is a description of some supported functions of the CPU unit:

Input and output: You can use io.PX to read or set pin values. For example:

-   Read P0 Input: let pin0value = io.P0;
-   Set output: io.P4 = 1; Note: The game will synchronize the modified output value to the signal network after the execution of the start, stop and tick.

Store data: Note: Other places in your code declare variables that are lost every time you compile/game reload and need to be loaded manually. The storage object is built into the system, and the data under this object is saved by the game and can be read the next time the game is loaded storage.xxx = 1;

Some helper functions

-   **getbit: You can call **getbit(io.P0, 1) to get the second bit value of the input pin.
-   **changebit: You can call io.P3 = **changebit(io.P3, 1, 0) to set the second bit value of the output pin to 0.
-   **log: You can call '**log('message')' to log some messages in the editor.
-   **reset: Call **reset() to reset all output IO
-   **shutdown: You can call **shutdown('error message') to shutdown the CPU unit.

### Assembly code

The CPU supports assembly code. But assembly is more difficult, Recommend using JavaScript, you can also use assembly, because it's fun, but before you write code, you need to learn about assembly.

This CPU supports class C51 like assembly (partial subset, different from the standard), and all computation instructions and registers are 32-bit. This article only describes the CPU instruction related support and special instructions, assemble the relevant knowledge you need to search for relevant information on the Internet.

Instruction format

[Operation code] [operand] For example, MOV A, P0 :label Label

Supported addressing methods

-   Immediate number addressing: #20H Example: MOV A,#20H
-   Register addressing: PSW/A/R1 Example: MOV A,R1
-   Register indirectly addresses: @R1 / @REG1 For example: MOV A,@R1
-   Skip address: :label For example, jne :loop

Supported register

-   R0-R7 Built-in general purpose register
-   PSW program status word register (as defined in 8086)
-   A
-   B
-   C
-   ACC accumulates register
-   SP stack pointer register
-   PC program counter

Supported instruction list

No operand instruction

-   NOP
-   RST
-   HLT
-   POP

1 Operand instruction

-   SWAP
-   PUSH
-   NEG
-   JMP
-   JE
-   JNE
-   JZ
-   JO
-   JNO
-   JS
-   JNS
-   JP
-   JNP
-   JC
-   JNC
-   JB
-   JNB
-   JA
-   JNA
-   JAE
-   JNAE
-   JBE
-   JNBE

2 Operand instruction

-   MOV
-   XCHG
-   XCHD
-   ADD
-   SUB
-   INC
-   DEC
-   MUL
-   MOD
-   AND
-   OR
-   XOR
-   NOR
-   MOD
-   SHL
-   SHR
-   CMP

Read write IO

-   Read write IO simplified, no special instructions, need built-in registers like, directly read/write: MOV A, P0 or MOV P0, #11

Sample program

mov r0,#10 mov r1,#0 mov r2,#0 :loop inc r2 dec r0 cmp r0,r1 jne :loop mov p3,r2 hlt

[Mod on Steam](https://steamcommunity.com/sharedfiles/filedetails/?id=3173524865)
