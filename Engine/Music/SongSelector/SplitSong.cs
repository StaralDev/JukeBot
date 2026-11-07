using YoutubeExplode.Videos;

namespace JukeBot.Engine.Music.SongSelector;

public class SplitSong(): ISongSelector
{
  private Video Current;

  public async Task FillAsync(string videoId) => 
    Current = await Manager.Fetcher.Videos.GetAsync(videoId);

  public IVideo Extract() => Current;
}