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
  public Progress(Progress from) => Splits = [.. from.Splits];

  private readonly SortedSet<RunSplit> Splits = [];

  public void Clear() => Splits.Clear();
  public void Add(RunSplit a) => Splits.Add(a);
  
  public bool In(RunSplit a) => Splits.Contains(a);

  #nullable enable
  public RunSplit? Now() 
  {
    if (Splits.Count == 0) return null;
    return Splits.Max();
  }
  #nullable disable

  public static bool operator==(Progress a, Progress b) => a.Splits.SetEquals(b.Splits);
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