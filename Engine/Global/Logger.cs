
using System.Diagnostics;
using System.Text.RegularExpressions;
using JukeBot.Engine.Game;

namespace JukeBot.Engine.Global;

// Feinberg has a perfect quote representing what I think
// about this class
// https://youtu.be/A6MRC-osDfk?
public class Logger
{
  public long CurrentLine { get; private set; } = 0;
  public long CurrentLength { get; private set; } = 0;

  public static string RegexStripLogGeneric { get; private set; } = @"\[(.*?)\] \[(.*?)\]:[ ]*(.*)"; 
  public static string RegexStartWorld { get; private set; } = @"Attempting event world load at (.*)";
  public static string RegexPlayerLogin { get; private set; } = @"(.*?)\[(.*?)\] logged in with entity id (.*?) at \(.*?\)";
  public static string RegexEndWorld { get; private set; } = @"\[WorldCreator\] stopping '(.*?)' integrated server\.\.\.";
  public static string RegexStateOutput { get; private set; } = @"StateOutput State: (.*)";

  private readonly Regex FromStripLogGeneric = new(RegexStripLogGeneric);
  private readonly Regex FromStartWorld = new(RegexStartWorld);
  private readonly Regex FromPlayerLogin = new(RegexPlayerLogin);
  private readonly Regex FromEndWorld = new(RegexEndWorld);
  private readonly Regex FromStateOutput = new(RegexStateOutput);

  private bool ActiveReading = false;

  private System.Threading.Timer LogUpdater;

  public void WatchLogs(string path)
  {
    LogUpdater = new(_ => { 
      UpdateLogs(path); ActiveReading = false; 
      Manager.Session.UpdateAdvancements();
    }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
  }

  void UpdateLogs(string path)
  {
    // Prevents overlapping reads if updating takes longer than n specified milliseconds;
    // we undo this at the end
    if (ActiveReading) return;
    ActiveReading = true;

    try
    {
      using FileStream reg = new(path+"\\logs\\latest.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
      if (reg.Length == CurrentLength) return; 
      // Nothing ever happens

      if (reg.Length < CurrentLength)
      {
        CurrentLine = 0;
      }
      
      CurrentLength = reg.Length;
      using StreamReader list = new(reg);

#nullable enable
      string? next;
#nullable disable

      int i = 0;
      while ((next = list.ReadLine()) is not null)
      {
        if (i >= CurrentLine) {  
          CurrentLine++;
          AddLog(next);
        }
        i++;
      }
    }
    catch (Exception a)
    {
      Debug.WriteLine(a);
    }
    finally
    {
      ActiveReading = false;
    }
  }

  private void AddLog(string msg)
  {
    Match match = FromStripLogGeneric.Match(msg);

    if (match.Groups.Count < 4) return;
    TestLog(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
  }

  private void TestLog(string timestamp, string kind, string content)
  {
    Match stateInfo = FromStateOutput.Match(content);
    if (stateInfo.Success) Manager.Session.HandleStateOutput(stateInfo.Groups[1].Value);

    Match loginInfo = FromPlayerLogin.Match(content);
    if (loginInfo.Success) Manager.Session.HandlePlayerLogin(loginInfo.Groups[1].Value);

    Match joinInfo = FromStartWorld.Match(content);
    if (joinInfo.Success) Manager.Session.HandleWorldStart(joinInfo.Groups[1].Value);

    Match leaveInfo = FromEndWorld.Match(content);
    if (leaveInfo.Success) Manager.Session.HandleWorldStop(leaveInfo.Groups[1].Value);
  }
}