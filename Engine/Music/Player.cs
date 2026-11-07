using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace JukeBot.Engine.Music;

class Player
{
  private const string TEST_PLAYLIST = "https://youtube.com/playlist?list=PLNHlsqvHbaoGweLQDycbD_7mwyw_v87Yi&si=eJIdgw-bNxrtJ_3f";

  public async Task test()
  {
    SplitPlaylist playlist = new();
    await playlist.FillAsync(TEST_PLAYLIST);

    Debug.WriteLine(playlist.ExtractRandom().Title);
    Debug.WriteLine(playlist.ExtractRandom().Title);
    Debug.WriteLine(playlist.ExtractRandom().Title);

    /*
    // var n = await Fetcher.Videos.GetAsync(TEST_SONG);
    var usr = await Fetcher.Channels.GetAsync(n.Author.ChannelId);
    Debug.WriteLine($"TEST SONG: {n.Title}");
    Debug.WriteLine($"BY: {usr.Title}");

    // TODO: Cache channel info
    // TODO: Also we can cache audio info while we're at it ig
    string artistImageHash = n.Author.ChannelId;

    // TODO: Temp
    Manager.WidgetLink.UpdateSongPresentation.Invoke(new SongPresentationInfo()
    {
      Title = n.Title,
      Artist = usr.Title,
      Cover = usr.Thumbnails[0].Url,
      Background = Background.HashBackground(Game.RunSplit.Fortress, "Test Song111..")
    });
    */
  }
}