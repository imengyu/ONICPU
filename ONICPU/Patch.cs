using HarmonyLib;
using ONICPU.digit;
using ONICPU.screens;
using ONICPU.ui;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static DetailsScreen;

namespace ONICPU
{
  [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
  public class Patch
  {
    private const string SubcategoryID = "fcpu";
    private static void Prefix()
    {
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU2Config.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU4Config.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU8Config.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU2JSConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU4JSConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU8JSConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU2PSASMConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU4PSASMConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU8PSASMConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitSegBaseConfig.ID + 8, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitSegBaseConfig.ID + 16, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitSegBaseConfig.ID + 32, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitTemperatureSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitCritterCountSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitDuplicantSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitLightSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitMassSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitPressureSensorGasConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitPressureSensorLiquidConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitRadiationSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitTimeOfDaySensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitWattageSensorConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", DigitConstantConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Power", DigitBatteryConfig.ID, SubcategoryID);
    }

  }
  [HarmonyPatch(typeof(Db))]
  [HarmonyPatch("Initialize")]
  public class Db_Initialize_Patch
  {
    public static void Postfix()
    {
      if (DlcManager.IsExpansion1Active())
      {
        Utils.AddTechnology("DupeTrafficControl", "DigitSensors", new ResourceTreeNode()
        {
          nodeX = 1950.0f,
          nodeY = -7554.0f,
          width = 250.0f,
          height = 72.0f,
        }, "_Computers");
        Utils.AddTechnology("DigitSensors", "FastCPU", new ResourceTreeNode()
        {
          nodeX = 2305.0f,
          nodeY = -7554.0f,
          width = 250.0f,
          height = 72.0f,
        }, "_Computers");
        Utils.AddTechnologyLine("DupeTrafficControl", "DigitSensors");
        Utils.AddTechnologyLine("DigitSensors", "FastCPU");
      }
      else
      {
        Utils.AddTechnology("DupeTrafficControl", "DigitSensors", new ResourceTreeNode()
        {
          nodeX = 1950.0f,
          nodeY = -6834.80f,
          width = 250.0f,
          height = 72.0f,
        }, "_Computers");
        Utils.AddTechnology("DigitSensors", "FastCPU", new ResourceTreeNode()
        {
          nodeX = 6834.80f,
          nodeY = -6834.80f,
          width = 250.0f,
          height = 72.0f,
        }, "_Computers");
        Utils.AddTechnologyLine("DupeTrafficControl", "DigitSensors");
        Utils.AddTechnologyLine("DigitSensors", "FastCPU");
      }


      Utils.AddBuildingToTechnology("FastCPU", FCPU2Config.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU4Config.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU8Config.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU2JSConfig.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU4JSConfig.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU8JSConfig.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU2PSASMConfig.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU4PSASMConfig.ID);
      Utils.AddBuildingToTechnology("FastCPU", FCPU8PSASMConfig.ID);
      Utils.AddBuildingToTechnology("FastCPU", DigitConstantConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitSegBaseConfig.ID + 8);
      Utils.AddBuildingToTechnology("DigitSensors", DigitSegBaseConfig.ID + 16);
      Utils.AddBuildingToTechnology("DigitSensors", DigitSegBaseConfig.ID + 32);
      Utils.AddBuildingToTechnology("DigitSensors", DigitTemperatureSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitCritterCountSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitDuplicantSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitLightSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitMassSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitPressureSensorGasConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitPressureSensorLiquidConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitRadiationSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitTimeOfDaySensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitWattageSensorConfig.ID);
      Utils.AddBuildingToTechnology("DigitSensors", DigitBatteryConfig.ID);
    }
  }

  [HarmonyPatch(typeof(Localization), "Initialize")]
  public class StringLocalisationPatch
  {
    public static void Postfix()
    {
      Utils.Localize(typeof(ONICPU.STRINGS));
    }
  }

  [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
  public class DetailsScreenScreenOnPrefabInit_Patch
  {
    static void Postfix(DetailsScreen __instance)
    {
      List<SideScreenRef> sideScreens = typeof(DetailsScreen)
        .GetField("sideScreens", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(__instance) as List<SideScreenRef>;

      UIUtils.GetKleiInternalPrefabs();

      LogicBitSelectorSideScreen logicBitSelectorSideScreen = null;
      foreach (var sideScreen in sideScreens)
      {
        var comp = sideScreen.screenPrefab.GetComponent<LogicBitSelectorSideScreen>();
        if (comp != null)
        {
          logicBitSelectorSideScreen = comp;
          break;
        }
      }

      //Fix LogicBitSelectorSideScreen too high
      if (logicBitSelectorSideScreen)
      {
        var rect = logicBitSelectorSideScreen.rowPrefab.GetComponent<RectTransform>();
        var layout = logicBitSelectorSideScreen.rowPrefab.GetComponent<LayoutElement>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 18);
        layout.minHeight = 18;
        var layout2 = logicBitSelectorSideScreen.transform.Find("Contents/RowContainer/Rows").GetComponent<VerticalLayoutGroup>();
        layout2.spacing = 0;
      }

      //Add custom screens 

      //DigitCritterSensorSideScreen
      //==================================

      UIUtils.AddSideScreen<DigitCritterSensorSideScreen>((parent, self) =>
      {
        self.countCrittersToggle = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.COUNT_CRITTER_LABEL");
        self.countEggsToggle = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.BUILDINGS.PREFABS.LOGICCRITTERCOUNTSENSOR.COUNT_EGG_LABEL");
      }, "STRINGS.BUILDINGS.PREFABS.DIGITCRITTERCOUNTSENSOR.NAME");

      //DigitPressureSensorSideScreen
      //==================================

      UIUtils.AddSideScreen<DigitPressureSensorSideScreen>((parent, self) =>
      {
        self.checkUnitug = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.MICROGRAM");
        self.checkUnitmg = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.MILLIGRAM");
        self.checkUnitg = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.GRAM");
        self.checkUnitkg = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM");
        self.checkUnitt = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.TONNE");
        self.checkUnitpounds = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.MICROGRAM");
        self.checkUnitdr = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.DRACHMA");
        self.checkUnitgr = UIUtils.AddCheckbox(parent, LabelKey: "STRINGS.UI.UNITSUFFIXES.MASS.GRAIN");
      }, "STRINGS.BUILDINGS.PREFABS.DIGITPRESSURESENSORGAS.NAME");

      //DigitCommonSensorSideScreen
      //==================================

      UIUtils.AddSideScreen<DigitCommonSensorSideScreen>((parent, self) =>
      {
        self.currentValue = UIUtils.AddLabel(
          parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 100),
          LabelAlignment: TMPro.TextAlignmentOptions.Center
        );
      });

      //FCPUSideScreen
      //==================================

      UIUtils.AddSideScreen<FCPUSideScreen>((parent, self) =>
      {
        UIUtils.AddLabel(
          parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30),
          "STRINGS.UI.UISIDESCREENS.FCPU.TITLE_EDITOR",
          LabelColor: Color.black, LabelAlignment: TMPro.TextAlignmentOptions.Center
        );

        self.editButton = UIUtils.AddButton(parent,
          new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30),
          LabelKey: "STRINGS.UI.USERMENUACTIONS.FCPUMENU.NAME", 
          TooltipKey: "STRINGS.UI.USERMENUACTIONS.FCPUMENU.TOOLTIP"
        );
        self.manualButton = UIUtils.AddButton(parent,
          new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30),
          LabelKey: "STRINGS.UI.USERMENUACTIONS.FCPUMENU.MANUAL",
          TooltipKey: "STRINGS.UI.USERMENUACTIONS.FCPUMENU.MANUAL_TOOLTIP"
        );

        UIUtils.AddLabel(
          parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30),
          "STRINGS.UI.UISIDESCREENS.FCPU.TITLE_CONTROL_SPEED",
          LabelColor: Color.black, LabelAlignment: TMPro.TextAlignmentOptions.Center
        );

        self.speedControl = UIUtils.AddDropdown(parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30), (dropdown) =>
        {
          for (var i = 0; i < FCPU.CPUSpeedArray.Count; i++)
          {
            var sp = FCPU.CPUSpeedArray[i];
            dropdown.options.Add(new Dropdown.OptionData() { text = $"{sp}x({sp * 5}hz)" });
          }
        });

        UIUtils.AddSpace(parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 10));
      });

      //DigitConstantSideScreen
      //==================================

      UIUtils.AddSideScreen<DigitConstantSideScreen>((content, DigitConstantSideScreen) =>
      {
        const int ROW_SIZE = 8;

        UIUtils.AddVerticalLayout(content, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 300), (parent, verticalLayoutGroup) =>
        {
          verticalLayoutGroup.childControlHeight = false;
          verticalLayoutGroup.childForceExpandHeight = false;

          for (int i = 0; i < DigitConstantSideScreen.MAX_BIT / ROW_SIZE; i++)
          {
            UIUtils.AddHorzontalLayout(parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30), (groupRectTransform, a) =>
            {
              a.childForceExpandWidth = false;
              a.childControlWidth = false;

              var lineText = UIUtils.AddTextLine(
                $"{i * ROW_SIZE}-{i * ROW_SIZE + 7}",
                "Tip", 50, 30, 0, 0, groupRectTransform
              );
              var lineTextLayoutElement = lineText.GetComponent<LayoutElement>();
              lineTextLayoutElement.minWidth = 30;
              lineTextLayoutElement.preferredWidth = 30;
              lineText.color = Color.black;
              lineText.alignment = TMPro.TextAlignmentOptions.Center;

              for (int j = 0; j < ROW_SIZE; j++)
              {
                var index = i * ROW_SIZE + j;
                DigitConstantSideScreen.valueButtons[index] = UIUtils.AddButton(
                  groupRectTransform, new FLayoutOptions(23, 23),
                  LabelText: "0"
                );
              }
            });
          }

          UIUtils.AddLabel(
            parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30), 
            "STRINGS.BUILDINGS.PREFABS.DIGITCONST.LOGIC_PORT", 
            LabelColor: Color.black, LabelAlignment: TMPro.TextAlignmentOptions.Center
          );
          
          DigitConstantSideScreen.valueInput = UIUtils.AddInputField(parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30)).field;
          DigitConstantSideScreen.valueApplyButton = UIUtils.AddButton(
            parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 30), 
            LabelKey: "STRINGS.UI.UISIDESCREENS.DIGITCONST.APPLY", TooltipKey: "STRINGS.UI.UISIDESCREENS.DIGITCONST.APPLY_INPUT"
          );
          UIUtils.AddSpace(parent, new FLayoutOptions(UIUtils.SIZE_SCREEN_WIDTH, 10));
        }); 
      });
    }
  }

  //Patch LogicRibbonReader/Writer to allow select 32bit

  [HarmonyPatch(typeof(LogicRibbonReader), "OnSpawn")]
  public class LogicRibbonReaderOnSpawn_Patch
  {
    static void Postfix(LogicRibbonReader __instance)
    {
      __instance.bitDepth = 32;
    }
  }

  [HarmonyPatch(typeof(LogicRibbonWriter), "OnSpawn")]
  public class LogicRibbonWriterOnSpawn_Patch
  {
    static void Postfix(LogicRibbonWriter __instance)
    {
      __instance.bitDepth = 32;
    }
  }

  [HarmonyPatch(typeof(SaveLoader), "Save", typeof(string), typeof(bool), typeof(bool))]
  public class SaveLoaderSave_Patch
  {
    public static event System.Action onBeforeSave;

    static void Prefix()
    {
      onBeforeSave?.Invoke();
    }
  }
}
