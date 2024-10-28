namespace ONICPU
{
  public class STRINGS
  {
    public class BUILDINGS
    {
      public class PREFABS
      {
        public static LocString FCPUDESC = "Allow you to use assembly code for more customized control of logical signals.";
        public static LocString FCPUJSDESC = "Allow you to use JavaScript for more customized control of logical signals.";

        public class FCPU2JS
        {
          public static LocString NAME = "Easy JavaScript CPU (2pin)";
          public static LocString DESC = FCPUJSDESC;
          public static LocString EFFECT = "Using JavaScript to process and output control signals.";
        }
        public class FCPU4JS
        {
          public static LocString NAME = "Easy JavaScript CPU (4pin)";
          public static LocString DESC = FCPUJSDESC;
          public static LocString EFFECT = "Using JavaScript to process and output control signals.";
        }
        public class FCPU8JS
        {
          public static LocString NAME = "Easy JavaScript CPU (8pin)";
          public static LocString DESC = FCPUJSDESC;
          public static LocString EFFECT = "Using JavaScript to process and output control signals.";
        }

        public class FCPU2
        {
          public static LocString NAME = "Advanced CPU (2pin)";
          public static LocString DESC = FCPUDESC;
          public static LocString EFFECT = "Using assembly code to process and output control signals.";
        }
        public class FCPU4
        {
          public static LocString NAME = "Advanced CPU (4pin)";
          public static LocString DESC = FCPUDESC;
          public static LocString EFFECT = "Using assembly code to process and output control signals.";
        }
        public class FCPU8
        {
          public static LocString NAME = "Advanced CPU (8pin)";
          public static LocString DESC = FCPUDESC;
          public static LocString EFFECT = "Using assembly code to process and output control signals.";
        }

        public class DIGITSEG8
        {
          public static LocString NAME = "Digit Display (8bit)";
          public static LocString DESC = "You can display digital number in the logical network.";
          public static LocString EFFECT = "Display a number with 8bit signals (-128-127). Exceeding the range will display overflow (OF).";
          public static LocString LOGIC_PORT = "8-Bit Input";
        }
        public class DIGITSEG16
        {
          public static LocString NAME = "Digit Display (16bit)";
          public static LocString DESC = "You can display digital number in the logical network.";
          public static LocString EFFECT = "Display a number with 16bit signals (-32768-32767). Exceeding the range will display overflow (OF).";
          public static LocString LOGIC_PORT = "16-Bit Input";
        }
        public class DIGITSEG32
        {
          public static LocString NAME = "Digit Display (32bit)";
          public static LocString DESC = "You can display digital number in the logical network.";
          public static LocString EFFECT = "Display a number with 32bit signals (-2147483648-2147483647). Exceeding the range will display overflow (OF).";
          public static LocString LOGIC_PORT = "32-Bit Input";
        }

        public class DIGITTEMPERATURESENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Thermo Sensor", "DIGITTEMPERATURESENSOR");
          public static LocString DESC = "Thermo sensors can disable buildings when they approach dangerous temperatures.";
          public static LocString EFFECT = "Sends a 16bit digit temperature value to network.";
          public static LocString LOGIC_PORT = "16-Bit integer " + global::STRINGS.UI.FormatAsLink("Temperature", "HEAT") + " value";
        }
        public class DIGITPRESSURESENSORGAS
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Atmo Sensor", "DIGITPRESSURESENSORGAS");
          public static LocString DESC = "Atmo sensors can be used to prevent excess oxygen production and overpressurization.";
          public static LocString EFFECT = "Sends a 16bit digit Gas Pressure value to network.";
          public static LocString LOGIC_PORT = "16-Bit integer " + global::STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " Pressure";
        }
        public class DIGITPRESSURESENSORLIQUID
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Hydro Sensor", "DIGITPRESSURESENSORLIQUID");
          public static LocString DESC = "A hydro sensor can tell a pump to refill its basin as soon as it contains too little liquid.";
          public static LocString EFFECT = "Sends a 16bit digit Liquid Pressure value to network.";
          public static LocString LOGIC_PORT = "16-Bit integer " + global::STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Pressure";
        }
        public class DIGITLIGHTSENSOR 
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Light Sensor", "DIGITLIGHTSENSOR");
          public static LocString DESC = "Light sensors can tell surface bunker doors above solar panels to open or close based on solar light levels.";
          public static LocString EFFECT = "Sends a 16bit digit Ambient Brightness value to network.";
          public static LocString LOGIC_PORT = "16-Bit integer Ambient " + global::STRINGS.UI.FormatAsLink("Brightness", "LIGHT");
        }
        public class DIGITWATTAGESENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Wattage Sensor", "DIGITWATTAGESENSOR");
          public static LocString DESC = "Wattage sensors can send a signal when a building has switched on or off.";
          public static LocString EFFECT = "Sends a 16 bit integer value of current " + global::STRINGS.UI.FormatAsLink("Wattage", "POWER") + " .";
          public static LocString LOGIC_PORT = "Consumed " + global::STRINGS.UI.FormatAsLink("Wattage", "POWER");
        }
        public class DIGITRADIATIONSENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Radiation Sensor", "DIGITRADIATIONSENSOR");
          public static LocString DESC = "Radiation sensors can disable buildings when they detect dangerous levels of radiation.";
          public static LocString EFFECT = "Sends a 16 bit integer value of current ambient " + global::STRINGS.UI.FormatAsLink("Radiation", "RADIATION") + " .";
        }
        public class DIGITTIMEOFDAYSENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Cycle Sensor", "DIGITTIMEOFDAYSENSOR");
          public static LocString DESC = "Cycle sensors ensure systems always turn on at the same time, day or night, every cycle.";
          public static LocString EFFECT = "Sets an 8bit precent value of day-night cycle (0-100).";
        }
        public class DIGITMASSSENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Weight Plate", "FLOORSWITCH");
          public static LocString DESC = "Weight plates can be used to turn on amenities only when Duplicants pass by.";
          public static LocString EFFECT = "Sends a integer weight value (kg).";
        }
        public class DIGITDUPLICANTSENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Duplicant Motion Sensor", "DIGITDUPLICANTSENSOR");
          public static LocString DESC = "Motion sensors save power by only enabling buildings when Duplicants are nearby.";
          public static LocString EFFECT = "Sends a integer Motion count value.";
        }
        public class DIGITCRITTERCOUNTSENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Critter Sensor", "DIGITCRITTERCOUNTSENSOR");
          public static LocString DESC = "Detecting critter populations can help adjust their automated feeding and care regimens.";
          public static LocString EFFECT = "Sends a digit number of eggs and critters in a room.";
        }
        public class DIGITBATTERY
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Battery", "DIGITBATTERY");
          public static LocString DESC = "Smart batteries send a charge percentage value (0-100) to network.";
          public static LocString EFFECT = "Stores " + global::STRINGS.UI.FormatAsLink("Power", "POWER") + " from generators, then provides that power to buildings.\n\nSends a charge percentage value (0-100) to network.\n\nVery slightly loses charge over time.";
        }
        public class DIGITCONST
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit constant", "DIGITCONST");
          public static LocString DESC = "Output constant signals to the network.";
          public static LocString EFFECT = "A configurable constant outputer.";
          public static LocString LOGIC_PORT = "32-Bit integer number (signed)";
        }
        public class DIGITBROADCAST
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit wireless boradcast", "DIGITBROADCAST");
          public static LocString DESC = "You can use a wireless boradcast to send a signal to the world and receive it anywhere with a receiver.";
          public static LocString EFFECT = "Send a wireless digit signal to world. Need power so it can send signal.";
          public static LocString LOGIC_PORT = "32-Bit integer number";
        }
        public class DIGITRECEIVER
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit wireless receiver", "DIGITRECEIVER");
          public static LocString DESC = "Receive a wireless digit signal.";
          public static LocString EFFECT = "Need power so it can receive signal.";
          public static LocString LOGIC_PORT = "32-Bit integer number";
        }

        public class DIGITSTORAGELOCKERSMART
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit storage Bin", "DIGITSTORAGELOCKERSMART");
          public static LocString DESC = "Smart storage bins can automate resource organization based on type and mass. (Digit version)";
          public static LocString EFFECT = "Stores the " + global::STRINGS.UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.\n\nSends a mass value or precent of stored value.";
          public static LocString LOGIC_PORT = "32-Bit integer number";
        }
        public class DIGITSMARTRESERVOIR
        {
          public static LocString NAME = "Smart reservoir";
          public static LocString LOGIC_PORT = "32-Bit integer number";
        }
        public class DIGITLIQUIDRESERVOIR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Liquid Reservoir", "DIGITLIQUIDRESERVOIR");
          public static LocString DESC = "Reservoirs cannot receive manually delivered resources. (Digit version)";
          public static LocString EFFECT = "Stores any " + global::STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it, and send a mass value or precent of stored value.";
        }
        public class DIGITGASRESERVOIR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Gas Reservoir", "DIGITGASRESERVOIR");
          public static LocString DESC = "Reservoirs cannot receive manually delivered resources. (Digit version)";
          public static LocString EFFECT = "Stores any " + global::STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it, and send a mass value or precent of stored value.";
        }
      }
    }
    public class UI
    {
      public class UISIDESCREENS
      {
        public class DIGITPRESSURESENSOR
        {
          public static LocString SIDESCREEN_TITLE = "Choose output pressure unit";
        }
        public class DIGITTIMEOFDAYSENSOR
        {
          public static LocString PRECENT = "Percentage of a day";
        }
        public class DIGITCONST
        {
          public static LocString SIDESCREEN_TITLE = "Set output value";
          public static LocString ERROR_TITLE = "Error";
          public static LocString NOT_A_VALID_NUMBER = "The value entered is not a valid integer";
          public static LocString APPLY = "Apply";
          public static LocString APPLY_INPUT = "Apply the value entered in the input box";          
        }
        public class COMMONSENSOR
        {
          public static LocString SIDESCREEN_TITLE = "Current value";
        }
        public class FCPU
        {
          public static LocString SIDESCREEN_TITLE = "CPU Config";
          public static LocString STOP_BUTTON_TOOLTIP = "Stop";
          public static LocString RESET_BUTTON_TOOLTIP = "Reset\n\nFor JavaScript CPU, only reset io values,\nFor AssimbleCode CPU, reset all cpu data.";
          public static LocString PLAYPAUSE_BUTTON_TOOLTIP = "Start/Pause";
          public static LocString STEP_BUTTON_TOOLTIP = "Step over";
          public static LocString CLEAR_BUTTON = "Clear";
          public static LocString CLEAR_BUTTON_TOOLTIP = "Clear all logs";
          public static LocString SHOW_STORAGE_BUTTON = "Storage";
          public static LocString SHOW_STORAGE_BUTTON_TOOLTIP = "Display a snapshot of the data currently stored in the CPU unit";
          public static LocString CLEAR_STORAGE_BUTTON = "Clear";
          public static LocString CLEAR_STORAGE_BUTTON_TOOLTIP = "Clear the data currently stored in the CPU unit";

          public static LocString EDITOR_TITLE = "Edit program for this CPU Unit";
          public static LocString EDITOR_LOG_TITLE = "Log details";
          public static LocString EDITOR_STATUS_TITLE = "Status details";
          
          public static LocString EDITOR_CLOSE = "Close";
          public static LocString EDITOR_COMPILE = "Compile";
          public static LocString EDITOR_COPY = "Copy to clipboard";
          public static LocString EDITOR_PASTE = "Paste from clipboard";
          public static LocString EDITOR_LOAD = "Load from script file";
          public static LocString EDITOR_SAVE = "Save to script file (will override!)";
          public static LocString EDITOR_OPEN = "Open script file with system application";

          public static LocString SUB_TITLE_LOGS = "Logs";
          public static LocString SUB_TITLE_INPUTS = "Inputs";
          public static LocString SUB_TITLE_OUTPUTS = "Outputs";
          public static LocString SUB_TITLE_REGS = "Registers";
          public static LocString SUB_TITLE_MEM = "Memory";
          public static LocString SUB_TITLE_STORAGES = "Storage";

          public static LocString NO_LOGS = "No log output";

          public static LocString TITLE_EDITOR = "Program editor";
          public static LocString TITLE_CONTROL_SPEED = "Speed control";
          public static LocString TITLE_LANG = "CPU Language";
          public static LocString TITLE_LOAD_ERROR = "Save script file filed";
          public static LocString TITLE_SAVE_ERROR = "Load script file filed";
          public static LocString TITLE_LOAD_SUCCESS = "Save script file success";
          public static LocString TITLE_SAVE_SUCCESS = "Load script file success";

          public static LocString REQUIRE_EN = "EN Control Enable";
          public static LocString REQUIRE_EN_TOOLTIP = "Set whether the current unit runs when receiving a green signal on the EN pin, otherwise it can be manually controlled by the button in the editor.";
        }
        public class DIGITBROADCAST
        {
          public static LocString SIDESCREEN_TITLE = "Choose channel";
          public static LocString CHANNEL = "Channel";
          public static LocString CHANNEL_OVERRIDE = "Note: A channel can transmit only one digit, when multiple broadcaster are connected to the channel, the later broadcaster will overwrite the previous broadcaster.";
        }
        public class DIGITSMARTRESERVOIR
        {
          public static LocString SIDESCREEN_TITLE = "Choose channel"; 
          public static LocString OUTPUT_PRECENT = "Output percentage of stored";
          public static LocString OUTPUT_AMOUNT = "Output amount of stored";
        }
      }
      public class USERMENUACTIONS
      {
        public class FCPUMENU
        {
          public static LocString NAME = "Write program";
          public static LocString TOOLTIP = "Write program for this CPU Unit";
          public static LocString MANUAL = "Show manual";
          public static LocString MANUAL_TOOLTIP = "Display CPU programming manual";
        }
      }
      public class CODEX
      {
        public class CATEGORYNAMES
        {
          public static LocString FCPUTIPS = "Digit CPU Manual";
        }
        public class FCPUTIPS
        {
          public static LocString TITLE1 = "Introduction";
          public static LocString TITLE2 = "CPU";
          public static LocString TITLE3 = "Pprogram editor";
          public static LocString TITLE4 = "Digit display";
          public static LocString TITLE5 = "Digit sensor";
          public static LocString TITLE6 = "Easy JavaScript";
          public static LocString TITLE7 = "Advanced Assembly Code";
          public static LocString TITLE8 = "Digit wireless singal";
          public static LocString BODY1 = "Digit CPU is a CPU mod for OxygenNotIncluded that allows you to build programmable code execution control units to implement complex automation functions." +
            "\n" +
            "\nAt present, the original game only has a simple logic gate automation, which will be very complicated when implementing some very large circuits, so i made the this mod, using code to control the automation.";
          public static LocString BODY2 = "You can find the CPU in the Automation menu (need to unlock Automation Tech first), there are currently 3 sizes of CPU:" +
            "\n" +
            "\n- 2 Input 2 Output I/O" +
            "\n- 4 Input 4 Output I/O" +
            "\n- 8 Input 8 Output I/O" +
            "\n" +
            "\n- The left side of the CPU is the input port, the program can read its value, and the right side is the output port, which can output the value and control the downstream connected devices." +
            "\n- The CPU uses a control port (lower middle position) for control, and the unit will only start when the control port receives a green signal. When the CPU is on, your program will execute at a rate of 5 times per second, and during each execution cycle, you can read the input parameters, perform custom calculations, and then modify the output port value." +
            "\n- There is a reset port on the CPU, and when the reset port receives a green signal, then reset the program." +
            "\n" +
            "\nDue to game code restrictions, all CPU ports support a maximum of 32bit values (Int32)." +
            "\n" +
            "\nThere are two versions of each CPU, with the (JavaScript) suffix indicating that the CPU is controlled using JavaScript code. JavaScript is easy to use, assembly is more complex but fun, you can choose what you like." +
            "\n" +
            "\nSpeed: Each CPU can control the running speed, from 0.1 times speed to 1 times speed (JS) /10 times speed (assembly), click the CPU, you can choose the speed in the side panel." +
            "\nNote: If you use JavaScript, you can't use a while loop in your code to delay, it will cause the game to get stuck, you should use a variable counter or timer to delay.";
          public static LocString BODY3 = "You can open the editor by selecting the CPU unit and clicking \"Write Program\" in the right menu. In the editor:" +
            "\n" +
            "\n- The top display is the current running status information of the cpu." +
            "\n- Write the code in the left input area." +
            "\n- Upper right corner button control program run/pause/step." +
            "\n- The right side shows the real-time program IO value." +
            "\n- In assembly mode, registers and memory data are also displayed on the right." +
            "\n- In JavaScript mode, program output log information is also displayed on the right." +
            "\n" +
            "\nIt is recommended that you write Code in Visual Studio Code or other Code Editors and then copy it into your game." +
            "\n" +
            "\nBottom buttons:" +
            "\n- Copy: Copy the code in the current editor to the system clipboard." +
            "\n- Paste: Pasting code from the system clipboard into the current editor, note: will overwrite the original content." +
            "\n- Load: Loading code from a file into the current editor, note: will overwrite the original content." +
            "\n- Save: Save the code in the current editor to the file, note: will overwrite the original content of the file." +
            "\n- Open: Open the code file with the system default editor." +
            "\n" +
            "\nClick \"Compile\" to compile the program immediately, compile will stop running the CPU unit, click \"Close\" to compile the program and then close." +
            "\n" +
            "\nTip: Whether the CPU is running is judged by whether the control port has a green signal, but when you manually click Run in the code editor, it is not limited by the signal and will still run.";
          public static LocString BODY4 = "This mod adds a Digital number display, which you can use to display the values in the network. It is divided into three levels, which can display 8-bit, 16-bit and 32-bit integers respectively.\n" +
            "\n" +
            "Attention: Please select a monitor within the numerical range. If the input value exceeds the value that the monitor can display, overflow (OF) will be displayed";
          public static LocString BODY5 = "This mod extends a series of sensors so that it can directly read values, you can connect it to the CPU, and then you can read values for related processing.";
          public static LocString BODY6 = "Once you have selected the CPU with the (JavaScript) suffix, you can start writing code in JavaScript. Your program will be like this:" +
            "\n\nfunction start() {" +
            "\n  // Code..." +
            "\n  __log('Hello world! My program start!');" +
            "\n}\nfunction stop() {" +
            "\n  // Code..." +
            "\n  __log('My program stop!');" +
            "\n}\nfunction tick() {" +
            "\n  // Main code..." +
            "\n}" +
            "\n" +
            "\n- start: Execute when the program starts" +
            "\n- stop: Execute when the program stops" +
            "\n- tick: The program executes every logical frame during runtime" +
            "\n" +
            "\nThe tick function must exist; otherwise, the CPU unit stops executing. start and stop are optional." +
            "\n" +
            "\nYour main program should be written in the tick function, and the recommended logic for the tick function should be Read input data > Process data > Write output data." +
            "\n" +
            "\nFor example, here is a simple Demo:" +
            "\n- I connected P0 to a temperature sensor, P1 to a pressure sensor, and P4 to a heating device" +
            "\n- I want to turn on the heating equipment when the temperature sensor temperature is below 15 degrees or greater than 35 degrees and the pressure is above 1000kg" +
            "\nThe code can be written like this:" +
            "\n" +
            "\nfunction tick() {" +
            "\n  io.P4 = (io.P0 < 15 || io.P0 > 35) && io.P1 > 1000;" +
            "\n}\n\nThe above code performs the judgment each frame and then outputs the result to the heating device connected to P4 to fulfill the requirement." +
            "\n\nYou can also implement more complex functions according to more conditions, below is a description of some supported functions of the CPU unit:" +
            "\n" +
            "\nInput and output: You can use io.PX to read or set pin values. For example:" +
            "\n  - Read P0 Input: let pin0value = io.P0;" +
            "\n  - Set output: io.P4 = 1;" +
            "\n  Note: The game will synchronize the modified output value to the signal network after the execution of the start, stop and tick." +
            "\n" +
            "\nStore data:" +
            "\n  Note: Other places in your code declare variables that are lost every time you compile/game reload and need to be loaded manually." +
            "\n  The storage object is built into the system, and the data under this object is saved by the game and can be read the next time the game is loaded" +
            "\n  storage.xxx = 1;" +
            "\n" +
            "\nTimer:" +
            "\n" +
            "\nJs CPU supports the timers, this timer timing time matches the game speed, you can achieve periodic tasks through timers, such as:" +
            "\n" +
            "\nfunction start() {" +
            "\n  io.P4 = 0;" +
            "\n  //Set a timer to change the P4 output every 4 seconds." +
            "\n  __timer(4, () => {" +
            "\n    io.P4 = io.P4 == 0 ? 1 : 0;" +
            "\n  });" +
            "\n}" +
            "\nfunction stop() {" +
            "\n  __stopAllTimer();//Stop all timers after cpu stop" +
            "\n}" +
            "\n" +
            "\nSystem functions:" +
            "\n" +
            "\n- __getbit: You can call __getbit(io.P0, 1) to get the second bit value of the input pin." +
            "\n- __changebit: You can call io.P3 = __changebit(io.P3, 1, 0) to set the second bit value of the output pin to 0." +
            "\n- __log: You can call '__log('message')' to log some messages in the editor." +
            "\n- __reset: Call __reset() to reset all output IO" +
            "\n- __shutdown: You can call __shutdown('error message') to shutdown the CPU unit." +
            "\n- __timer(second,callbackFn,once = false): Set timer and return timerID." +
            "\n- __stopTimer(timerID): Stop timer with ID." +
            "\n- __stopAllTimer(): Stop all.";
          public static LocString BODY7 = "The CPU supports assembly code. But assembly is more difficult, Recommend using JavaScript, you can also use assembly, because it's fun, but before you write code, you need to learn about assembly." +
            "\n\nNote: The CPU's assembly language is designed for games only and is not completely consistent with reality. " +
            "\n" +
            "\nThis CPU supports class C51 like assembly (partial subset, different from the standard), and all computation instructions and registers are 32-bit." +
            "\n- Number of registers: 8" +
            "\n- Code segment size: 300 instructions" +
            "\n- Support stack size: 256 data" +
            "\nThis article only describes the CPU instruction related support and special instructions, assemble the relevant knowledge you need to search for relevant information on the Internet." +
            "\n" +
            "\nInstruction format" +
            "\n" +
            "\nOperation code operand For example, MOV A, P0" +
            "\n:label Label" +
            "\n" +
            "\nSupported addressing methods" +
            "\n" +
            "\n- Immediate number addressing: #20H Example: MOV A,#20H" +
            "\n- Register addressing: PSW/A/R1 Example: MOV A,R1" +
            "\n- Register indirectly addresses: @R1 / @REG1 For example: MOV A,@R1" +
            "\n- Skip address: :label For example, jne :loop" +
            "\n" +
            "\nSupported register" +
            "\n" +
            "\n- R0-R7 Built-in general purpose register\n- PSW program status word register (as defined in 8086)" +
            "\n- A/B/C" +
            "\n- ACC accumulates register" +
            "\n- SP stack pointer register" +
            "\n- PC program counter" +
            "\n" +
            "\nSupported instruction list" +
            "\n" +
            "\nNote: The following parameter shorthand indicates the parameters accepted by the instruction:" +
            "\n - W: Writeable register or mempry: Register, Direct, IO." +
            "\n - R: Readable register or mempry: Register, Sfr, IndirectRegister, Direct, Data, IO." +
            "\n - A: Label or address." +
            "\n" +
            "\n- Miscellaneous Instructions: " +
            "\n - NOP: No operation." +
            "\n - RST: Reset this cpu unit." +
            "\n - HLT: Halt this cpu unit." +
            "\n" +
            "\n- Data transfer instruction: " +
            "\n - PUSH [R]: Push onto stack." +
            "\n - POP [W]: Pop off of stack." +
            "\n - SWAP [W]: Swap data high 4 bit and low 4bit." +
            "\n - MOV [W] [R]: Move data between general-purpose registers; move data between memory and generalpurpose or segment registers; move immediates to general-purpose registers." +
            "\n - XCHG [W] [W]: Exchange." +
            "\n - XCHD [W] [W]: Exchange half byte." +
            "\n" +
            "\n- Shift and Rotate Instructions:" +
            "\n - SHL [W] [R]: Shift logical left." +
            "\n - SHR [W] [R]: Shift logical right." +
            "\n" +
            "\n- Binary Arithmetic Instructions:" +
            "\n - ADD [W] [R]: Integer add." +
            "\n - SUB [W] [R]: Subtract." +
            "\n - MUL [W] [R]: Unsigned multiply." +
            "\n - DIV [W] [R]: Unsigned divide." +
            "\n - MOD [W] [R]: Modulo." +
            "\n - CMP [W] [R]: Compare." +
            "\n - INC [W]: Increment." +
            "\n - DEC [W]: Decrement." +
            "\n - NEG [W]: Negate." +
            "\n" +
            "\n- Logical Instructions:" +
            "\n - AND [W] [R]: Perform bitwise logical AND." +
            "\n - OR [W] [R]:  Perform bitwise logical OR." +
            "\n - XOR [W] [R]: Perform bitwise logical exclusive OR." +
            "\n - NOT [W]: Perform bitwise logical NOT" +
            "\n" +
            "\n- Control Transfer Instructions:" +
            "\n - CALL [A] : Call procedure." +
            "\n - RET: Return." +
            "\n - JMP [A]: Jump." +
            "\n - JE/JZ [A]: Jump if equal/Jump if zero." +
            "\n - JNE/JNZ [A]: Jump if not equal/Jump if not zero." +
            "\n - JA/JNBE [A]: Jump if above/Jump if not below or equal." +
            "\n - JAE/JNB [A]: Jump if above or equal/Jump if not below." +
            "\n - JB/JNAE [A]: Jump if below/Jump if not above or equal." +
            "\n - JBE/JNA [A]:  Jump if below or equal/Jump if not above." +
            "\n - JO [A]: Jump if overflow." +
            "\n - JNO [A]: Jump if not overflow." +
            "\n - JS [A]:  Jump if sign (negative)." +
            "\n - JNS [A]: Jump if not sign (non-negative)." +
            "\n - JP [A]: Jump if parity even." +
            "\n - JNP [A]: Jump if not parity even." +
            "\n - JC [A]: Jump if carry." +
            "\n - JNC [A]: Jump if not carry。" +
            "\n" +
            "\nRead write IO" +
            "\n" +
            "\n- Read write IO simplified, no special instructions, need built-in registers like, directly read/write: MOV A, P0 or MOV P0, #11" +
            "\n" +
            "\nSample program" +
            "\n" +
            "\nmov r0,#10" +
            "\nmov r1,#0" +
            "\nmov r2,#0" +
            "\n:loop" +
            "\ninc r2" +
            "\ndec r0" +
            "\ncmp r0,r1" +
            "\njne :loop" +
            "\nmov p3,r2" +
            "\nhlt" +
            "\n";
          public static LocString BODY8 = "The mod also adds a pair of wireless digital signal boradcaster and receivers, which you can use to wirelessly transmit digital signals between any location.";
        }
      }
    }
    public class RESEARCH
    { 
      public class TECHS
      {
        public class FASTCPU
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Fast CPU", "FASTCPU");
          public static LocString DESC = "Allow you to use assembly code for more customized control of logical signals.";
        }
        public class DIGITSENSORS
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Sensors", "DIGITSENSORS");
          public static LocString DESC = "Allow you to use digit sensors and read digit signals.";
        }
      }
    }
  }
}
