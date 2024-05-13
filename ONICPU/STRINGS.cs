namespace ONICPU
{
  public class STRINGS
  {
    public class BUILDINGS
    {
      public class PREFABS
      {
        public class FCPU4
        {
          public static LocString NAME = "Fast CPU 4pin";
          public static LocString DESC = "Allow you to use assembly code for more customized control of logical signals.";
          public static LocString EFFECT = "Using assembly code to process and output control signals.";
        }
        public class FCPU8
        {
          public static LocString NAME = "Fast CPU 8pin";
          public static LocString DESC = "Allow you to use assembly code for more customized control of logical signals.";
          public static LocString EFFECT = "Using assembly code to process and output control signals.";
        }
        public class FCPU4JS
        {
          public static LocString NAME = "Fast CPU 4pin (JavaScript)";
          public static LocString DESC = "Allow you to use JavaScript for more customized control of logical signals.";
          public static LocString EFFECT = "Using JavaScript to process and output control signals.";
        }
        public class FCPU8JS
        {
          public static LocString NAME = "Fast CPU 8pin (JavaScript)";
          public static LocString DESC = "Allow you to use JavaScript for more customized control of logical signals.";
          public static LocString EFFECT = "Using JavaScript to process and output control signals.";
        }

        public class DIGITSEG8
        {
          public static LocString NAME = "Digit Display (8bit)";
          public static LocString DESC = "You can display digital number in the logical network.";
          public static LocString EFFECT = "Display a number with 8bit signals (-128-127)";
          public static LocString LOGIC_PORT = "8-Bit Input";
        }
        public class DIGITSEG16
        {
          public static LocString NAME = "Digit Display (16bit)";
          public static LocString DESC = "You can display digital number in the logical network.";
          public static LocString EFFECT = "Display a number with 16bit signals (-32768-32767)";
          public static LocString LOGIC_PORT = "16-Bit Input";
        }
        public class DIGITSEG32
        {
          public static LocString NAME = "Digit Display (32bit)";
          public static LocString DESC = "You can display digital number in the logical network.";
          public static LocString EFFECT = "Display a number with 32bit signals (-2147483648-2147483647)";
          public static LocString LOGIC_PORT = "32-Bit Input";
        }

        public class DIGITTEMPERATURESENSOR
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Thermo Sensor", "DIGITTEMPERATURESENSOR");
          public static LocString DESC = "Thermo sensors can disable buildings when they approach dangerous temperatures.";
          public static LocString EFFECT = "Sends a 16bit digit temperature value to network.";
          public static LocString LOGIC_PORT = "16-Bit intager " + global::STRINGS.UI.FormatAsLink("Temperature", "HEAT") + " value";
        }
        public class DIGITPRESSURESENSORGAS
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Atmo Sensor", "DIGITPRESSURESENSORGAS");
          public static LocString DESC = "Atmo sensors can be used to prevent excess oxygen production and overpressurization.";
          public static LocString EFFECT = "Sends a 16bit digit Gas Pressure value to network.";
          public static LocString LOGIC_PORT = "16-Bit intager " + global::STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " Pressure";
        }
        public class DIGITPRESSURESENSORLIQUID
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Hydro Sensor", "DIGITPRESSURESENSORLIQUID");
          public static LocString DESC = "A hydro sensor can tell a pump to refill its basin as soon as it contains too little liquid.";
          public static LocString EFFECT = "Sends a 16bit digit Liquid Pressure value to network.";
          public static LocString LOGIC_PORT = "16-Bit intager " + global::STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Pressure";
        }
        public class DIGITLIGHTSENSOR 
        {
          public static LocString NAME = global::STRINGS.UI.FormatAsLink("Digit Light Sensor", "DIGITLIGHTSENSOR");
          public static LocString DESC = "Light sensors can tell surface bunker doors above solar panels to open or close based on solar light levels.";
          public static LocString EFFECT = "Sends a 16bit digit Ambient Brightness value to network.";
          public static LocString LOGIC_PORT = "16-Bit intager Ambient " + global::STRINGS.UI.FormatAsLink("Brightness", "LIGHT");
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
        }
      }
      public class USERMENUACTIONS
      {
        public class FCPUMENU
        {
          public static LocString NAME = "Write program";
          public static LocString TOOLTIP = "Write program for this CPU Unit";
        }
      }
    }
  }
}
