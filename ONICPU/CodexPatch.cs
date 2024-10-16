using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ONICPU
{ 
  public class FCPUTips
  {
    public string title;
    public string body;
    public string image;
    public int imageWidth;
    public int imageHeight;

    public FCPUTips(string title, string body, string image = "", int imw = 0, int imh = 0)
    {
      this.title = title;
      this.body = body;
      this.image = image;
      this.imageWidth = imw;
      this.imageHeight = imh;
    }
  }

  [HarmonyPatch(typeof(CodexEntryGenerator), "GenerateTutorialNotificationEntries")]
  public class CodexPatch
  {
    private static List<FCPUTips> FCPUTips = new List<FCPUTips>()
    {
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE1", "STRINGS.UI.CODEX.FCPUTIPS.BODY1"),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE4", "STRINGS.UI.CODEX.FCPUTIPS.BODY4"),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE5", "STRINGS.UI.CODEX.FCPUTIPS.BODY5", "h5.png", 400, 448),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE8", "STRINGS.UI.CODEX.FCPUTIPS.BODY8", "h6.png", 400, 272),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE2", "STRINGS.UI.CODEX.FCPUTIPS.BODY2", "h2.png", 400, 291),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE3", "STRINGS.UI.CODEX.FCPUTIPS.BODY3", "h3.png", 400, 376),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE6", "STRINGS.UI.CODEX.FCPUTIPS.BODY6"),
      new FCPUTips("STRINGS.UI.CODEX.FCPUTIPS.TITLE7", "STRINGS.UI.CODEX.FCPUTIPS.BODY7"),
    };

    private static void GenerateTitleContainers(string name, List<ContentContainer> containers)
    {
      List<ICodexWidget> list = new List<ICodexWidget>();
      list.Add(new CodexText(name, CodexTextStyle.Title));
      list.Add(new CodexDividerLine());
      containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
    }
    private static void AddTutorial(CodexEntry codexEntry, int i, FCPUTips tip)
    {
      List<ContentContainer> list3 = new List<ContentContainer>();
      if (!string.IsNullOrEmpty(tip.title))
      {
        tip.title = Strings.Get(tip.title);
        GenerateTitleContainers(tip.title, list3);
      }

      tip.body = Strings.Get(tip.body);

      if (!string.IsNullOrEmpty(tip.image))
      {
        var sprite = Utils.LoadSpriteFromFile(
          Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/assets/sprite/" + tip.image, 
          tip.imageWidth, tip.imageHeight
        );
        list3.Add(new ContentContainer(new List<ICodexWidget>
        {
          new CodexImage(tip.imageWidth, tip.imageHeight, sprite),
          new CodexText(tip.body, CodexTextStyle.Body, tip.title),
        }, ContentContainer.ContentLayout.Vertical));
      }
      else
      {
        list3.Add(new ContentContainer(new List<ICodexWidget>
        {
          new CodexText(tip.body, CodexTextStyle.Body, tip.title)
        }, ContentContainer.ContentLayout.Vertical));
      }
      list3.Add(new ContentContainer(new List<ICodexWidget>
      {
        new CodexSpacer(),
        new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical));

      SubEntry item = new SubEntry("FCPUTIPS" + i, "FCPUTIPS", list3, tip.title);
      codexEntry.subEntries.Add(item);
    }
    private static void Prefix()
    {
      List<ContentContainer> list = new List<ContentContainer>
      {
        new ContentContainer(new List<ICodexWidget> { new CodexSpacer() }, ContentContainer.ContentLayout.Vertical)
      };
      CodexEntry codexEntry = new CodexEntry("FCPUTIPS", list, Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES.FCPUTIPS"));
      for (int i = 0; i < FCPUTips.Count; i++)
        AddTutorial(codexEntry, i, FCPUTips[i]);
      CodexCache.AddEntry("FCPUTIPS", codexEntry);
    }
  }
}
