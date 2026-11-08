using YoutubeExplode.Channels;
using YoutubeExplode.Videos;

namespace JukeBot.Engine.Music;

public struct SongPresentationInfo
{
  public string Title { get; set; }
  public string Artist { get; set; }
  public string Cover { get; set; }
  public string Background { get; set; }
}

public enum PlayKind
{
  Enabled,
  Last,
  Disabled,
}

public struct LinkInfo(string link, bool single, PlayKind state)
{
  public string Link { get; set; } = link;
  public bool Single { get; set; } = single;
  public PlayKind State { get; set; } = state;
}

public class PreparedSong(Stream audio, IVideo video, Channel channel)
{
  public Stream Audio { get; } = audio;
  public IVideo Video { get; } = video;
  public Channel Channel { get; } = channel;
}