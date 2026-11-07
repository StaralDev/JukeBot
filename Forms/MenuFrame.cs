using JukeBot.Engine;
using JukeBot.Engine.Linkage;
using JukeBot.Interface;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace JukeBot;

public partial class MenuFrame : Form
{
  public MenuFrame()
  {
    InitializeComponent();
    new DisplayFrame().Show();

    var services = new ServiceCollection();
    services.AddWindowsFormsBlazorWebView();
    services.AddSingleton<MenuLink>();

    var serviceProvider = services.BuildServiceProvider();

    BlazorWebView webView = new()
    {
      Dock = DockStyle.Fill,
      HostPage = @"Pages\Menu\index.html",
      Services = serviceProvider
    };

    webView.BlazorWebViewInitialized += (_, e) =>
    {
      e.WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
        "appassets.local",
        AppContext.BaseDirectory,
        Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
      e.WebView.DefaultBackgroundColor = Color.Transparent;
      _ = Manager.StartMenu(serviceProvider.GetRequiredService<MenuLink>());
    };

    Controls.Add(webView);
    webView.RootComponents.Add<MenuControl>("#app");
  }
}