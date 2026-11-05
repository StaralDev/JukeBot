using JukeBot.Engine.Game;

namespace JukeBot.Engine.Music;

public class Background
{
  private static int SimpleHash(string to)
  {
    int n = 0;
    foreach (char c in to) { n += c; }
    return n;
  }

  public static string HashBackground(RunSplit kind, string name)
  {
    #nullable enable
    List<string>? selection = kind switch
    {
      RunSplit.Start => ["golem", "portal"],
      RunSplit.Nether => ["crimson", "enderman", "hills"],
      RunSplit.Bastion => ["approach", "far", "shiny"],
      RunSplit.Fortress => ["assault", "front"],
      RunSplit.Searching => ["famillear", "measure"],
      RunSplit.Stronghold => ["blind", "gone", "spawner"],
      RunSplit.End => ["distance", "faith", "observation"],
      _ => null,
    };
    #nullable disable

    if (selection is null) return @"../../Resources/Default/album.webp";
    int i = SimpleHash(name) % selection.Count;

    return $"../../Resources/{kind}/{selection[i]}.png";
  }
}