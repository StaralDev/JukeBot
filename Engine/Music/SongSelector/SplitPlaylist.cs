using JukeBot.Engine.Music.SongSelector;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace JukeBot.Engine.Music.SongSelector;

public class SplitPlaylist(): ISongSelector
{
  public async Task FillAsync(string playlistId)
  {
    await foreach (var batch in Manager.Fetcher.Playlists.GetVideoBatchesAsync(playlistId))
    {
      foreach (var video in batch.Items)
      {
        Videos.Add(video);
      }
    }
  }

  public List<PlaylistVideo> Videos = [];
  
  private PlaylistVideo last;
  private readonly Random rand = new();
  private const int MAX_RETRY_COUNT = 100;

  public IVideo Extract()
  {
    if (Videos.Count == 0)
    {
      System.Diagnostics.Debug.WriteLine("Cannot extract a song from an empty playlist.");
      return null;
    }

    bool found = false;
    int retryc = 0;

    while (!found && retryc < MAX_RETRY_COUNT)
    {
      int i = rand.Next(Videos.Count);
      PlaylistVideo r = Videos[i];
      
      if (r != last) { last = r; return r; } 
      retryc++;
    }

    return last;
  }
}