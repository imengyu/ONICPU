using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace ONICPU
{
  public static class Utils
  {
    public static void AddBuildingToTechnology(string techId, string buildingId)
    {
      Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
    }
    public static void AddTechnology(string preTechId, string techId, ResourceTreeNode node, string category)
    {
      var techs = Db.Get().Techs;
      var tech = new Tech(techId, new List<string>(), techs)
      {
        requiredTech = new List<Tech>() { techs.Get(preTechId) },
      };
      tech.SetNode(node, category);

      tech.tier = Database.Techs.GetTier(tech);

      var TECH_TIERSField = typeof(Database.Techs).GetField("TECH_TIERS", BindingFlags.Instance | BindingFlags.NonPublic);
      var TECH_TIERS = TECH_TIERSField.GetValue(techs) as List<List<Tuple<string, float>>>;

      foreach (Tuple<string, float> item2 in TECH_TIERS[tech.tier])
      {
        if (!tech.costsByResearchTypeID.ContainsKey(item2.first))
          tech.costsByResearchTypeID.Add(item2.first, item2.second);
      }
    }
    public static ResourceTreeNode.Edge AddTechnologyLine(string fromTechId, string toTechId)
    {
      var techs = Db.Get().Techs;
      var nodeField = typeof(Tech).GetField("node", BindingFlags.Instance | BindingFlags.NonPublic);
      var fromTechNode = nodeField.GetValue(techs.Get(fromTechId)) as ResourceTreeNode;
      var toTechNode = nodeField.GetValue(techs.Get(toTechId)) as ResourceTreeNode;
      fromTechNode.references.Add(toTechNode);
      var edgeType = (ResourceTreeNode.Edge.EdgeType)Enum.Parse(typeof(ResourceTreeNode.Edge.EdgeType), "PolyLineEdge");
      var edge = new ResourceTreeNode.Edge(fromTechNode, toTechNode, edgeType);
      edge.sourceOffset = Vector2.zero;
      edge.targetOffset = Vector2.zero;
      fromTechNode.edges.Add(edge);
      return edge;
    }




    /// <summary>
    /// 从文件中加载图片
    /// </summary>
    /// <param name="path">要加载的路径</param>
    /// <returns></returns>
    public static Texture2D LoadTexture2dFromFile(string path, int width, int height)
    {
      Texture2D t2d = new Texture2D(width, height);
      t2d.LoadImage(File.ReadAllBytes(path));
      t2d.Apply();
      return t2d;
    }
    /// <summary>
    /// 从文件中加载图片
    /// </summary>
    /// <param name="path">要加载的路径</param>
    /// <returns></returns>
    public static Sprite LoadSpriteFromFile(string path, int width, int height)
    {
      return Sprite.Create(LoadTexture2dFromFile(path, width, height), new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static AssetBundle LoadAssetBundle(string refPath)
    {
      var assetsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/assets/assetbundle/";
      return AssetBundle.LoadFromFile(assetsPath + refPath);
    }

    public static void ShowSymbolConditionally(bool showAnything, bool active, KBatchedAnimController kbac, KAnimHashedString ifTrue, KAnimHashedString ifFalse)
    {
      if (!showAnything)
      {
        kbac.SetSymbolVisiblity(ifTrue, is_visible: false);
        kbac.SetSymbolVisiblity(ifFalse, is_visible: false);
      }
      else
      {
        kbac.SetSymbolVisiblity(ifTrue, active);
        kbac.SetSymbolVisiblity(ifFalse, !active);
      }
    }
    public static void TintSymbolConditionally(bool tintAnything, bool condition, KBatchedAnimController kbac, KAnimHashedString symbol, Color ifTrue, Color ifFalse)
    {
      if (tintAnything)
      {
        kbac.SetSymbolTint(symbol, condition ? ifTrue : ifFalse);
      }
      else
      {
        kbac.SetSymbolTint(symbol, Color.white);
      }
    }

    public static void Localize(Type root)
    {
      ModUtil.RegisterForTranslation(root);
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      string name = executingAssembly.GetName().Name;
      string path = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "translations");
      Localization.Locale locale = Localization.GetLocale();
      if (locale != null)
      {
        try
        {
          string text = Path.Combine(path, locale.Code + ".po");
          if (File.Exists(text))
          {
            Debug.Log(name + ": Localize file found " + text);
            Localization.OverloadStrings(Localization.LoadStringsFile(text, isTemplate: false));
          }
        }
        catch
        {
          Debug.LogWarning(name + " Failed to load localization.");
        }
      }
      LocString.CreateLocStringKeys(root, "");
    }
    public static string GetLocalizeString(string key)
    {
      return Strings.Get(new StringKey(key));
    }
  }
}
