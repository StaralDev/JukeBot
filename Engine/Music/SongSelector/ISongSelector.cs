using YoutubeExplode.Videos;

namespace JukeBot.Engine.Music.SongSelector;

public interface ISongSelector
{
  public Task FillAsync(string id);
  public IVideo Extract();
}