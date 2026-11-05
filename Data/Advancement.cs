namespace JukeBot.Data;

public class Advancement
{
  public string Id { get; set; }
  public Dictionary<string, string> Criteria { get; set; }
  public bool Done { get; set; }
}