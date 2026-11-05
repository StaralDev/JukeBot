using System.Diagnostics;
using System.Drawing.Drawing2D;
using JukeBot.Engine;
using JukeBot.Engine.Linkage;
using JukeBot.Interface;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace JukeBot;

public partial class DisplayFrame : Form
{
  private const int HT_CAPTION = 0x2;
  private const int WM_NCHITTEST = 0x0084;
  private const int WM_EXITSIZEMOVE = 0x0232;

  public DisplayFrame()
  {
    InitializeComponent();

    var services = new ServiceCollection();
    services.AddWindowsFormsBlazorWebView();
    services.AddSingleton<FrontEnd>();

    var serviceProvider = services.BuildServiceProvider();

    BlazorWebView webView = new()
    {
      Dock = DockStyle.Fill,
      HostPage = @"Pages\Root\index.html",
      Services = serviceProvider
    };

    webView.BlazorWebViewInitialized += (_, e) =>
    {
      e.WebView.DefaultBackgroundColor = Color.Transparent;
      _ = Manager.Start(serviceProvider.GetRequiredService<FrontEnd>());
    };

    Controls.Add(webView);
    webView.RootComponents.Add<IndexRazor>("#app");
    webView.Enabled = false;
  }

  protected override void OnResize(EventArgs e)
  {
    base.OnResize(e);

    using GraphicsPath path = new();
    const int radius = 16;
    int diameter = radius * 2;
    Rectangle bounds = ClientRectangle;
    bounds.Inflate(-1, -1);
    path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
    path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
    path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
    path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
    path.CloseFigure();
    Region?.Dispose();
    Region = new Region(path);
  }

  protected override void WndProc(ref Message message)
  {
    Screen monitor = Screen.FromControl(this);
    switch (message.Msg)
    {
      // Makes sure you don't move the window off of your screen;
      // updates when we exit a size/move message since
      // 1. It's probably less expensive
      // 2. You may want to drag the app onto a seperate monitor
      case WM_EXITSIZEMOVE:
        Location = new(
          Math.Clamp(Location.X, 0, monitor.WorkingArea.Width-Width), 
          Math.Clamp(Location.Y, 0, monitor.WorkingArea.Height-Height)
        );
        break;

      // Manually telling the OS we hit the window when trying to interact
      case WM_NCHITTEST:
        message.Result = HT_CAPTION;
        return;
    }

    base.WndProc(ref message);
  }
}
