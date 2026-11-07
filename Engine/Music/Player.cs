using JukeBot.Engine.Music.SongSelector;
using NAudio.Wave;
using YoutubeExplode.Channels;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace JukeBot.Engine.Music;

class Player
{
  private const string TEST_PLAYLIST = "https://youtube.com/playlist?list=PLNHlsqvHbaoGweLQDycbD_7mwyw_v87Yi&si=eJIdgw-bNxrtJ_3f";

  private MemoryStream AudioStream;
  private MediaFoundationReader AudioStreamReader;
  private WaveOut CurrentAudio;
  private IVideo CurrentVideo;
  private Channel CurrentChannel;

  public async Task PlayFromPlaylist()
  {
    
  }

  public async Task PlayFromVideo()
  {
    
  }

  public async Task test()
  {
    if (AudioStreamReader is not null)
    {
      AudioStream.Dispose();
      AudioStreamReader.Dispose();
      CurrentAudio.Dispose();
    }

    SplitPlaylist playlist = new();
    await playlist.FillAsync(TEST_PLAYLIST);
    
    CurrentVideo = playlist.Extract();
    CurrentChannel = await Manager.Fetcher.Channels.GetAsync(CurrentVideo.Author.ChannelId);
    
    StreamManifest manifestContent = await Manager.Fetcher.Videos.Streams.GetManifestAsync(CurrentVideo.Url);
    IStreamInfo strInfo = manifestContent.GetAudioOnlyStreams().GetWithHighestBitrate();
    var audioRaw = await Manager.Fetcher.Videos.Streams.GetAsync(strInfo);
    
    AudioStream = new();
    await audioRaw.CopyToAsync(AudioStream);
    AudioStream.Position = 0;

    AudioStreamReader = new(strInfo.Url);
    CurrentAudio = new();

    CurrentAudio.Init(AudioStreamReader);
    CurrentAudio.Play();
    CurrentAudio.Volume = 0.3f;

    Manager.WidgetLink.UpdateSongPresentation.Invoke(new SongPresentationInfo()
    {
      Title = CurrentVideo.Title,
      Artist = CurrentChannel.Title,
      Cover = CurrentChannel.Thumbnails[0].Url,
      Background = Background.HashBackground(Game.RunSplit.Start, CurrentChannel.Url),
    });

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