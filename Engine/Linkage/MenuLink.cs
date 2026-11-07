namespace JukeBot.Engine.Linkage;

public class MenuLink
{
  private readonly TaskCompletionSource ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
  public Task Ready => ready.Task;
  public void MarkReady() => ready.TrySetResult();
}