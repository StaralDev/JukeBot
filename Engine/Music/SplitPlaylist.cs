using YoutubeExplode.Playlists;

namespace JukeBot.Engine.Music;

public class SplitPlaylist()
{
  public List<PlaylistVideo> Videos = [];
  
  private PlaylistVideo last;
  private readonly Random rand = new();
  private const int MAX_RETRY_COUNT = 100;

  public PlaylistVideo ExtractRandom()
  {
    bool found = false;
    int retryc = 0;

    while (!found && retryc < MAX_RETRY_COUNT)
    {
      int i = rand.Next()%Videos.Count;
      PlaylistVideo r = Videos[i];
      
      if (r != last) { last = r; return r; } 
      retryc++;
    }

    return last;
  }
}