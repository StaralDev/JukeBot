using YoutubeExplode.Videos;

namespace JukeBot.Engine.Music.SongSelector;

public interface ISongSelector
{
  public IVideo Extract();
}