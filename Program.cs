using JukeBot.Engine;

namespace JukeBot;

static class Program
{
  [STAThread]
  static void Main()
  {
    ApplicationConfiguration.Initialize();
    Application.Run(new DisplayFrame());
  }
}