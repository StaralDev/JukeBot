using JukeBot.Engine.Game;
using JukeBot.Engine.Global;
using JukeBot.Engine.Linkage;
using JukeBot.Engine.Music;

namespace JukeBot.Engine;

class Manager
{
  public static Config Config { get; private set; } = new();
  public static Logger Logger { get; private set; } = new();

  public static Session Session { get; private set; } = new();
  public static FrontEnd FrontEnd { get; private set; }

  public static async Task Start(FrontEnd frontEnd)
  {
    FrontEnd = frontEnd;
    await frontEnd.Ready;

    Config.Root = @"C:\Users\Alynna\AppData\Roaming\PrismLauncher\instances\MCSRRanked-Windows-1.16.1-Basic\minecraft";
    //Config.Root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\.minecraft";
    Logger.WatchLogs(Config.Root);

    // TODO: Temp
    frontEnd.UpdateSongPresentation.Invoke(new SongPresentationInfo()
    {
      Title = "Test Song111..",
      Artist = "Test Artist",
      Cover = @"",
      Background = Background.HashBackground(RunSplit.End, "Test Song111..")
    });
  }
}