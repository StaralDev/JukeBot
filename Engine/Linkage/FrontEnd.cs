using JukeBot.Engine.Music;

namespace JukeBot.Engine.Linkage;

public class FrontEnd
{
  private readonly TaskCompletionSource ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

  public Action<SongPresentationInfo> UpdateSongPresentation { get; set; }
  public Task Ready => ready.Task;

  public void MarkReady() => ready.TrySetResult();
}