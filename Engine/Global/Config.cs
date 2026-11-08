using JukeBot.Engine.Game;
using JukeBot.Engine.Music;

namespace JukeBot.Engine.Global;

public class Config
{
  public string Root { get; set; } = "";
  public bool Looped = true;

  // TODO: TEMP
  private const string TEST_PLAYLIST = "https://youtube.com/playlist?list=PLNHlsqvHbaoGweLQDycbD_7mwyw_v87Yi&si=eJIdgw-bNxrtJ_3f";
  public Dictionary<RunSplit, LinkInfo> SplitPlaylists { get; set; } = new()
  {
    [RunSplit.Start] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
    [RunSplit.Nether] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
    [RunSplit.Bastion] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
    [RunSplit.Fortress] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
    [RunSplit.Searching] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
    [RunSplit.Stronghold] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
    [RunSplit.End] = new(TEST_PLAYLIST, false, PlayKind.Enabled),
  };

  public LinkInfo GetPlaylist(RunSplit split)
  {
    if (SplitPlaylists.TryGetValue(split, out LinkInfo link) &&
        link.State != PlayKind.Last)
      return link;

    for (int i = (int)split - 1; i >= (int)RunSplit.Start; i--)
    {
      LinkInfo previous = SplitPlaylists[(RunSplit)i];
      if (previous.State != PlayKind.Last) return previous;
    }

    return new("", false, PlayKind.Disabled);
  }
}
