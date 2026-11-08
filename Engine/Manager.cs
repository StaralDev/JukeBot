using JukeBot.Engine.Game;
using JukeBot.Engine.Global;
using JukeBot.Engine.Linkage;
using JukeBot.Engine.Music;
using YoutubeExplode;

namespace JukeBot.Engine;

class Manager
{
  public static YoutubeClient Fetcher { get; private set; } = new();

  public static Config Config { get; private set; } = new();
  public static Logger Logger { get; private set; } = new();

  public static Session Session { get; private set; } = new();
  public static Player Player { get; private set; } = new();

  public static WidgetLink WidgetLink { get; private set; }
  public static MenuLink MenuLink { get; private set; }

  public static async Task StartMenu(MenuLink menuLink)
  {
    MenuLink = menuLink;
    // TODO: Setup like StartWidget
  }

  public static async Task StartWidget(WidgetLink widgetLink)
  {
    WidgetLink = widgetLink;
    await widgetLink.Ready;

    Config.Root = @"C:\Users\Alynna\AppData\Roaming\PrismLauncher\instances\MCSRRanked-Windows-1.16.1-Basic\minecraft";
    //Config.Root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\.minecraft";
    Logger.WatchLogs(Config.Root);
  }
}