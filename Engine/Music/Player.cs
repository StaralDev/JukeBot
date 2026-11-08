using JukeBot.Engine.Game;
using JukeBot.Engine.Music.SongSelector;
using NAudio.Wave;
using YoutubeExplode.Channels;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace JukeBot.Engine.Music;

class Player
{
  private readonly Dictionary<string, ISongSelector> SelectorCache = [];
  private readonly Dictionary<Stream, PreparedSong> PreparedSongs = [];

  private readonly SemaphoreSlim QueueLock = new(1, 1);
  private readonly SemaphoreSlim PlaybackLock = new(1, 1);
  
  private bool ChangingSong;
  
  private ISongSelector CurrentSelector;
  private RunSplit CurrentSplit = RunSplit.Start;
  private string CurrentAudioPath;
  private WaveOut CurrentAudio;
  private MediaFoundationReader AudioStreamReader;

  public async Task Load(string id, bool single)
  {
    await PlayFromSelector(await GetSelector(id, single));
  }

  public async Task Preload(string id, bool single)
  {
    await QueueSong(await GetSelector(id, single));
  }

  private async Task<ISongSelector> GetSelector(string id, bool single)
  {
    string key = $"{single}:{id}";
    if (!SelectorCache.TryGetValue(key, out ISongSelector selector))
    {
      selector = single ? new SplitSong() : new SplitPlaylist();
      SelectorCache[key] = selector;
      await selector.FillAsync(id);
    }

    return selector;
  }

  private async Task PlayFromSelector(ISongSelector selector)
  {
    await PlaybackLock.WaitAsync();
    try
    {
      CurrentSelector = selector;
      await QueueSong(selector);
      await PlayNext();
      await QueueSong(selector);
    }
    finally
    {
      PlaybackLock.Release();
    }
  }

  private async Task QueueSong(ISongSelector selector)
  {
    await QueueLock.WaitAsync();
    try
    {
      if (Manager.Session.PlaylistNext.Count >= 2) return;

      IVideo video = selector.Extract();
      if (video is null) return;

      Channel channel = await Manager.Fetcher.Channels.GetAsync(video.Author.ChannelId);
      StreamManifest manifest = await Manager.Fetcher.Videos.Streams.GetManifestAsync(video.Url);
      IStreamInfo streamInfo = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
      Stream audio = await Manager.Fetcher.Videos.Streams.GetAsync(streamInfo);

      string path = Path.Combine(Path.GetTempPath(), $"jukebot-{Guid.NewGuid():N}.audio");
      await using (audio)
      await using (FileStream file = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
      {
        await audio.CopyToAsync(file);
      }

      FileStream preparedAudio = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
      Manager.Session.PlaylistNext.Enqueue(preparedAudio);
      PreparedSongs.Add(preparedAudio, new PreparedSong(preparedAudio, video, channel));
    }
    finally
    {
      QueueLock.Release();
    }
  }

  private async Task PlayNext()
  {
    PreparedSong song;
    await QueueLock.WaitAsync();

    try
    {
      if (!Manager.Session.PlaylistNext.TryDequeue(out Stream audio)) return;
      if (!PreparedSongs.Remove(audio, out song)) { audio.Dispose(); return; }
    }
    finally
    {
      QueueLock.Release();
    }

    ChangingSong = true;
    CurrentAudio?.PlaybackStopped -= PlaybackStopped;
    CurrentAudio?.Stop();
    CurrentAudio?.Dispose();
    AudioStreamReader?.Dispose();
    
    if (CurrentAudioPath is not null) File.Delete(CurrentAudioPath);
    
    AudioStreamReader = new(((FileStream)song.Audio).Name);
    CurrentAudioPath = ((FileStream)song.Audio).Name;
    
    CurrentAudio = new();
    CurrentAudio.PlaybackStopped += PlaybackStopped;
    CurrentAudio.Init(AudioStreamReader);
    CurrentAudio.Play();
    song.Audio.Dispose();
    ChangingSong = false;
    
    CurrentAudio.Volume = 0.3f;

    Manager.WidgetLink?.UpdateSongPresentation.Invoke(new SongPresentationInfo()
    {
      Title = song.Video.Title,
      Artist = song.Channel.Title,
      Cover = song.Channel.Thumbnails[0].Url,
      Background = Background.HashBackground(CurrentSplit, song.Channel.Url),
    });
  }

  private async void PlaybackStopped(object sender, StoppedEventArgs e)
  {
    if (ChangingSong || e.Exception is not null) return;

    try
    {
      await PlaybackLock.WaitAsync();
      try
      {
        await PlayNext();
        await QueueSong(CurrentSelector);
      }
      finally
      {
        PlaybackLock.Release();
      }
    }
    catch (Exception exception)
    {
      System.Diagnostics.Debug.WriteLine($"Music playback stopped: {exception}");
    }
  }

  public async void PlayFromSplit(RunSplit current)
  {
    CurrentSplit = current;
    LinkInfo pv = Manager.Config.GetPlaylist(current);
    if (pv.State == PlayKind.Disabled) return;

    _ = Load(pv.Link, pv.Single);
    if (current >= RunSplit.End) return;

    LinkInfo next = Manager.Config.GetPlaylist(current + 1);
    if (next.State == PlayKind.Enabled) _ = Preload(next.Link, next.Single);
  }
}