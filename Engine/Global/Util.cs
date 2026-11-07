namespace JukeBot.Engine.Global;

class Util
{
  public static bool IsRankedWorld(string world) =>
    world.StartsWith("mcsrranked #", StringComparison.OrdinalIgnoreCase);
}