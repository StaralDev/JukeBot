namespace JukeBot.Engine.Game;

public enum RunSplit: uint
{
  Start,
  Nether,
  Bastion,
  Fortress,
  Searching,
  Stronghold,
  End,
}

public class Progress
{
  public Progress() {}
  public Progress(Progress from) => Splits = [.. from.GetSplits()];

  private readonly Lock SplitsLock = new();
  private readonly SortedSet<RunSplit> Splits = [];

  public void Clear()
  {
    lock (SplitsLock) Splits.Clear();
  }

  public void Add(RunSplit a)
  {
    lock (SplitsLock) Splits.Add(a);
  }
  
  public bool In(RunSplit a)
  {
    lock (SplitsLock) return Splits.Contains(a);
  }

  #nullable enable
  public RunSplit? Now() 
  {
    lock (SplitsLock)
    {
      if (Splits.Count == 0) return null;
      return Splits.Max();
    }
  }
  #nullable disable

  private RunSplit[] GetSplits()
  {
    lock (SplitsLock) return [.. Splits];
  }

  public static bool operator==(Progress a, Progress b) => a.GetSplits().SequenceEqual(b.GetSplits());
  public static bool operator!=(Progress a, Progress b) => !(a==b);

  public override bool Equals(object obj)
  {
    if (obj is null) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj is Progress progress) return progress == this;
    return false;
  }

  public override int GetHashCode() => throw new NotImplementedException();
}