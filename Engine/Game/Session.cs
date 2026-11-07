using System.Diagnostics;
using System.Text.Json.Nodes;
using JukeBot.Data;
using JukeBot.Engine.Global;

namespace JukeBot.Engine.Game;

public class Session
{
  private const string AdvancementNether = "minecraft:story/enter_the_nether";
  private const string AdvancementBastion = "minecraft:nether/find_bastion";
  private const string AdvancementFortress = "minecraft:nether/find_fortress";
  private const string RecipeEnderChest = "minecraft:recipes/decorations/ender_chest";
  private const string AdvancementStronghold = "minecraft:story/follow_ender_eye";
  private const string AdvancementEnd = "minecraft:story/enter_the_end";

  public Progress Progress = new();

  public Queue<string> PendingWorlds { get; set; } = new();
  public bool IgnoreNextSingleplayerTitle { get; set; }
  #nullable enable
  public string? ActiveWorld => PendingWorlds.TryPeek(out string? world) ? world : null;
  #nullable disable
  public string ActivePlayer { get; set; }

  private bool Reading = false;

  private Dictionary<string, Advancement> AdvancementProgress = [];
  public bool HasAdvancement(string id) => AdvancementProgress.ContainsKey(id) && (AdvancementProgress[id]?.Done ?? false);

  public void ChangeWorld()
  {
    Progress.Clear();
    AdvancementProgress.Clear();

    CheckProgress();
  }

  public void ClearSingleplayerWorlds()
  {
    Queue<string> rankedWorlds = new(PendingWorlds.Where(Util.IsRankedWorld));
    PendingWorlds = rankedWorlds;
    IgnoreNextSingleplayerTitle = false;
  }

  public void HandleStateOutput(string state)
  {
    if (state != "title") return;

    if (!IgnoreNextSingleplayerTitle &&
        ActiveWorld is string activeWorld && !Util.IsRankedWorld(activeWorld))
    {
      Debug.WriteLine($"Removing completed singleplayer world '{activeWorld}'");
      PendingWorlds.Dequeue();
      ChangeWorld();
    }
    else
      IgnoreNextSingleplayerTitle = false;
  }

  public void HandlePlayerLogin(string player)
  {
    if (player == ActivePlayer) return;

    ActivePlayer = player;
    Debug.WriteLine($"Recognized player: '{ActivePlayer}'");
  }

  public void HandleWorldStart(string startedWorld)
  {
    if (Util.IsRankedWorld(startedWorld))
    {
      ClearSingleplayerWorlds();
      PendingWorlds.Enqueue(startedWorld);
    }
    else
    {
      if (ActiveWorld is string currentWorld && Util.IsRankedWorld(currentWorld))
      {
        Debug.WriteLine($"Ignoring singleplayer load while ranked world '{currentWorld}' is active");
        return;
      }

      if (ActiveWorld is not null)
      {
        if (ActiveWorld == startedWorld)
        {
          Debug.WriteLine($"Ignoring duplicate load for active world '{startedWorld}'");
          return;
        }

        PendingWorlds.Clear();
        PendingWorlds.Enqueue(startedWorld);
        IgnoreNextSingleplayerTitle = true;
        ChangeWorld();
      }
      else
        PendingWorlds.Enqueue(startedWorld);
    }

    Debug.WriteLine($"Added world to queue: '{startedWorld}' at position '{PendingWorlds.Count}'");

    if (PendingWorlds.Count == 1)
    {
      Debug.WriteLine($"Active world set to '{ActiveWorld}'");
      ChangeWorld();
    }
    else
      Debug.WriteLine($"New world halted for '{ActiveWorld}' completion");
  }

  public void HandleWorldStop(string stoppedWorld)
  {
    if (ActiveWorld != stoppedWorld)
    {
      Debug.WriteLine($"Ignoring stop for non-active world '{stoppedWorld}'");
      return;
    }

    Debug.WriteLine($"Removing active world '{stoppedWorld}'; player disconnect/restart...");
    PendingWorlds.Dequeue();
    if (PendingWorlds.Count == 0)
      Debug.WriteLine("World queue is now empty.");
    else
      Debug.WriteLine($"Active world is now '{ActiveWorld}'");

    ChangeWorld();
  }

  public void UpdateAdvancements()
  {
    if (Reading) return;
    Reading = true;

    if (ActiveWorld is null) { Reading = false; return; }

    AdvancementProgress.Clear();

    string dir = Path.Combine(Manager.Config.Root, "saves", ActiveWorld, "advancements");
    if (Directory.Exists(dir))
    { 
      foreach (string f in Directory.GetFiles(dir))
      {
        if (!f.EndsWith(".json")) continue;
        using FileStream a = new(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using StreamReader b = new(a);

        JsonNode root = JsonNode.Parse(b.ReadToEnd());
        foreach (KeyValuePair<string, JsonNode> advraw in root.AsObject())
        {
          if (advraw.Key == "DataVersion") continue;

          Advancement n = new()
          {
            Done = advraw.Value["done"].GetValue<bool>(),
            Criteria = [],
          };

          foreach(KeyValuePair<string, JsonNode> critraw in advraw.Value["criteria"].AsObject())
          {
            n.Criteria.Add(critraw.Key, critraw.Value.GetValue<string>());
          }

          AdvancementProgress.Add(advraw.Key, n);
        }
      }
    }

    CheckProgress();
    Reading = false;
  }

  public void CheckProgress()
  {
    Progress old = new(Progress);
 
    if (ActiveWorld is not null) Progress.Add(RunSplit.Start);
    if (HasAdvancement(AdvancementNether)) Progress.Add(RunSplit.Nether);
    if (HasAdvancement(AdvancementBastion)) Progress.Add(RunSplit.Bastion);
    if (HasAdvancement(AdvancementFortress)) Progress.Add(RunSplit.Fortress);
    if (HasAdvancement(RecipeEnderChest)) Progress.Add(RunSplit.Searching);
    if (HasAdvancement(AdvancementStronghold)) Progress.Add(RunSplit.Stronghold);
    if (HasAdvancement(AdvancementEnd)) Progress.Add(RunSplit.End);

    if (old.Now() != Progress.Now()) ProgressChanged();
  }

  public void ProgressChanged()
  {
    Debug.WriteLine($">>>>>>PLAYER JUST DID SOMETHING: {((Progress.Now() is not null) ? Progress.Now() : "STARTED")}");
  }
}