using HarmonyLib;
using KMod;
using ONICPU.component;
using ONICPU.digit;
using ONICPU.screens;
using ONICPU.sensor;
using STRINGS;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static DetailsScreen;
using static STRINGS.UI.FRONTEND;

namespace ONICPU
{
  [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
  public class Patch
  {
    private const string SubcategoryID = "fcpu";
    private static void Prefix()
    {
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU4Config.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU8Config.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU4JSConfig.ID, SubcategoryID);
      ModUtil.AddBuildingToPlanScreen("Automation", FCPU8JSConfig.ID, SubcategoryID);
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
      Utils.AddBuildingToTechnology("LogicCircuits", FCPU4Config.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", FCPU8Config.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", FCPU4JSConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", FCPU8JSConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitSegBaseConfig.ID + 8);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitSegBaseConfig.ID + 16);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitSegBaseConfig.ID + 32);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitTemperatureSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitCritterCountSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitDuplicantSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitLightSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitMassSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitPressureSensorGasConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitPressureSensorLiquidConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitRadiationSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitTimeOfDaySensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitWattageSensorConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitBatteryConfig.ID);
      Utils.AddBuildingToTechnology("LogicCircuits", DigitConstantConfig.ID);
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

      CritterSensorSideScreen critterSensorSideScreen = null;
      LogicBitSelectorSideScreen logicBitSelectorSideScreen = null;
      ArtableSelectionSideScreen artableSelectionSideScreen = null;
      foreach (var sideScreen in sideScreens)
      {
        var comp = sideScreen.screenPrefab.GetComponent<CritterSensorSideScreen>();
        if (comp != null)
        {
          critterSensorSideScreen = comp;
          break;
        }
      }
      foreach (var sideScreen in sideScreens)
      {
        var comp = sideScreen.screenPrefab.GetComponent<LogicBitSelectorSideScreen>();
        if (comp != null)
        {
          logicBitSelectorSideScreen = comp;
          break;
        }
      }
      foreach (var sideScreen in sideScreens)
      {
        var comp = sideScreen.screenPrefab.GetComponent<ArtableSelectionSideScreen>();
        if (comp != null)
        {
          artableSelectionSideScreen = comp;
          break;
        }
      }
      //Add custom screens 
      if (critterSensorSideScreen != null)
      {
        //DigitCritterSensorSideScreen
        //==================================

        var digitCritterSensorSideScreen = Object.Instantiate(critterSensorSideScreen.gameObject);
        digitCritterSensorSideScreen.name = "DigitCritterSensorSideScreen";

        var oldScreen = digitCritterSensorSideScreen.GetComponent<CritterSensorSideScreen>();
        var newScreen = digitCritterSensorSideScreen.AddComponent<DigitCritterSensorSideScreen>();
        newScreen.countCrittersToggle = oldScreen.countCrittersToggle.gameObject.AddComponent<CheckboxFixed>();
        newScreen.countEggsToggle = oldScreen.countEggsToggle.gameObject.AddComponent<CheckboxFixed>();
        var titleKeyField = typeof(SideScreenContent).GetField("titleKey", BindingFlags.Instance | BindingFlags.NonPublic);
        titleKeyField.SetValue(newScreen, titleKeyField.GetValue(oldScreen));
        Object.Destroy(oldScreen);

        sideScreens.Add(new SideScreenRef()
        {
          name = digitCritterSensorSideScreen.name,
          screenPrefab = newScreen,
          offset = Vector2.zero,
        });

        //DigitPressureSensorSideScreen
        //==================================

        var digitPressureSensorSideScreenObject = Object.Instantiate(critterSensorSideScreen.gameObject);
        var digitPressureSensorSideScreen = digitPressureSensorSideScreenObject.AddComponent<DigitPressureSensorSideScreen>();
        digitPressureSensorSideScreenObject.name = "DigitPressureSensorSideScreen";

        var content = digitPressureSensorSideScreenObject.transform.Find("Contents") as RectTransform;
        var CheckboxPrefab = content.transform.Find("CheckboxGroup");
        CheckboxPrefab.transform.GetChild(0).gameObject.AddComponent<CheckboxFixed>();
        oldScreen = digitPressureSensorSideScreenObject.GetComponent<CritterSensorSideScreen>();
        Object.Destroy(oldScreen);

        digitPressureSensorSideScreen.checkUnitug = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitmg = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitg = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitkg = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitt = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitpounds = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitdr = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitgr = Object.Instantiate(CheckboxPrefab, content).transform.GetChild(0).gameObject.GetComponent<CheckboxFixed>();
        digitPressureSensorSideScreen.checkUnitgr.LabelText = UI.UNITSUFFIXES.MASS.GRAIN;
        digitPressureSensorSideScreen.checkUnitdr.LabelText = UI.UNITSUFFIXES.MASS.DRACHMA;
        digitPressureSensorSideScreen.checkUnitpounds.LabelText = UI.UNITSUFFIXES.MASS.POUND;
        digitPressureSensorSideScreen.checkUnitug.LabelText = UI.UNITSUFFIXES.MASS.MICROGRAM;
        digitPressureSensorSideScreen.checkUnitmg.LabelText = UI.UNITSUFFIXES.MASS.MILLIGRAM;
        digitPressureSensorSideScreen.checkUnitg.LabelText = UI.UNITSUFFIXES.MASS.GRAM;
        digitPressureSensorSideScreen.checkUnitkg.LabelText = UI.UNITSUFFIXES.MASS.KILOGRAM;
        digitPressureSensorSideScreen.checkUnitt.LabelText = UI.UNITSUFFIXES.MASS.TONNE;

        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
          var item = content.transform.GetChild(i).gameObject;
          if (item.name == "CheckboxGroup")
            item.SetActive(false);
        }

        sideScreens.Add(new SideScreenRef()
        {
          name = digitPressureSensorSideScreenObject.name,
          screenPrefab = digitPressureSensorSideScreen,
          offset = Vector2.zero,
        });

        //DigitCommonSensorSideScreen
        //==================================

        var digitCommonSensorSideScreenObject = Object.Instantiate(critterSensorSideScreen.gameObject);
        var digitCommonSensorSideScreen = digitCommonSensorSideScreenObject.AddComponent<DigitCommonSensorSideScreen>();
        content = digitCommonSensorSideScreenObject.transform.Find("Contents") as RectTransform;
        digitCommonSensorSideScreenObject.name = "DigitCommonSensorSideScreen";

        var LabelPrefab = digitCommonSensorSideScreenObject.transform.Find("Contents/CheckboxGroup/Label");
        oldScreen = digitCommonSensorSideScreenObject.GetComponent<CritterSensorSideScreen>();
        Object.Destroy(oldScreen);

        digitCommonSensorSideScreen.currentValue = Object.Instantiate(LabelPrefab, content).GetComponent<LocText>();
        digitCommonSensorSideScreen.currentValue.alignment = TMPro.TextAlignmentOptions.Center;
        digitCommonSensorSideScreen.currentValue.name = "CurrentValueLabel";

        for(int i = content.transform.childCount - 1; i >= 0; i--)
        {
          var item = content.transform.GetChild(i).gameObject;
          if (item.name == "CheckboxGroup")
            item.SetActive(false);
        }

        sideScreens.Add(new SideScreenRef()
        {
          name = digitCommonSensorSideScreenObject.name,
          screenPrefab = digitCommonSensorSideScreen,
          offset = Vector2.zero,
        });


        if (artableSelectionSideScreen != null)
        {
          //FCPUSideScreen
          //==================================

          var FCPUSideScreenObject = Object.Instantiate(critterSensorSideScreen.gameObject);
          var FCPUSideScreen = FCPUSideScreenObject.AddComponent<FCPUSideScreen>();
          content = FCPUSideScreenObject.transform.Find("Contents") as RectTransform;
          FCPUSideScreenObject.name = "FCPUSideScreen";

          oldScreen = FCPUSideScreen.GetComponent<CritterSensorSideScreen>();
          Object.Destroy(oldScreen);

          FCPUSideScreen.editButton = Object.Instantiate(UIUtils.KButtonPrefab.gameObject, content).GetComponent<KButton>();
          FCPUSideScreen.editButton.name = "EditButton";
          FCPUSideScreen.editButton.transform.Find("Label").GetComponent<LocText>().key = "STRINGS.UI.USERMENUACTIONS.FCPUMENU.NAME";
          FCPUSideScreen.editButton.GetComponent<ToolTip>().FixedStringKey = "STRINGS.UI.USERMENUACTIONS.FCPUMENU.TOOLTIP";
          FCPUSideScreen.editButton.GetComponent<LayoutElement>().minWidth = 260;

          for (int i = content.transform.childCount - 1; i >= 0; i--)
          {
            var item = content.transform.GetChild(i).gameObject;
            if (item.name != "EditButton" && item.name != "BodyBG")
              item.SetActive(false);
          }

          sideScreens.Add(new SideScreenRef()
          {
            name = FCPUSideScreenObject.name,
            screenPrefab = FCPUSideScreen,
            offset = Vector2.zero,
          });

          //DigitConstantSideScreen
          //==================================

          var DigitConstantSideScreenObject = Object.Instantiate(critterSensorSideScreen.gameObject);
          var DigitConstantSideScreen = DigitConstantSideScreenObject.AddComponent<DigitConstantSideScreen>();
          content = DigitConstantSideScreenObject.transform.Find("Contents") as RectTransform;
          DigitConstantSideScreenObject.name = "DigitConstantSideScreen";

          oldScreen = DigitConstantSideScreen.GetComponent<CritterSensorSideScreen>();
          Object.Destroy(oldScreen);

          for (int i = content.transform.childCount - 1; i >= 0; i--)
          {
            var item = content.transform.GetChild(i).gameObject;
            if (item.name != "BodyBG")
              item.SetActive(false);
          }

          const int ROW_SIZE = 8;

          for (int i = 0; i < DigitConstantSideScreen.MAX_BIT / ROW_SIZE; i++)
          {
            var group = new GameObject();
            var groupRectTransform = group.AddComponent<RectTransform>();
            var groupLayoutElement = group.AddComponent<LayoutElement>();
            var groupHorizontalLayoutGroup = group.AddComponent<HorizontalLayoutGroup>();
            groupLayoutElement.minWidth = 260;
            groupLayoutElement.minHeight = 23;
            groupRectTransform.SetParent(content);

            var lineText = UIUtils.AddTextLine(
              $"{i * ROW_SIZE}-{i * ROW_SIZE + 7}",
              "Tip", 30, 23, 0, 0, groupRectTransform
            );
            var lineTextLayoutElement = lineText.GetComponent<LayoutElement>();
            lineTextLayoutElement.minWidth = 30;
            lineTextLayoutElement.preferredWidth = 30;
            lineText.color = Color.black;

            for (int j = 0; j < ROW_SIZE; j++)
            {
              var index = i * ROW_SIZE + j;
              var btn = Object.Instantiate(UIUtils.KButtonPrefab.gameObject, groupRectTransform).GetComponent<KButton>();
              var btnText = btn.transform.GetChild(0).GetComponent<LocText>();
              var btnRectTransform = btn.GetComponent<RectTransform>();
              var btnLayoutElement = btn.GetComponent<LayoutElement>();
              DigitConstantSideScreen.valueButtons[index] = btn;
              DigitConstantSideScreen.valueButtonTexs[index] = btnText;
              Object.Destroy(btn.GetComponent<ToolTip>());
              btnRectTransform.sizeDelta = new Vector2(23, 23);
              btnLayoutElement.minWidth = 23;
              btnLayoutElement.minHeight = 23;
              btnText.text = "";
            }
          }

          var text = UIUtils.AddTextLine(
            Utils.GetLocalizeString("STRINGS.BUILDINGS.PREFABS.DIGITCONST.LOGIC_PORT"),
            "Tip", 260, 15, 0, 0, content
          );
          text.color = Color.black;

          var input = Object.Instantiate(UIUtils.KInputFieldPrefab.gameObject, content).GetComponent<KInputField>();
          DigitConstantSideScreen.valueInput = input.field;
          DigitConstantSideScreen.valueInput.gameObject.AddComponent<LayoutElement>().minWidth = 260;

          DigitConstantSideScreen.valueApplyButton = UIUtils.AddButtonLine(
            "",
            "STRINGS.UI.UISIDESCREENS.DIGITCONST.APPLY",
            "STRINGS.UI.UISIDESCREENS.DIGITCONST.APPLY_INPUT", 0, 0, content,
            null
          );
          UIUtils.AddSpaceLine(260, 10, content);

          sideScreens.Add(new SideScreenRef()
          {
            name = DigitConstantSideScreenObject.name,
            screenPrefab = DigitConstantSideScreen,
            offset = Vector2.zero,
          });

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
