namespace JukeBot.Components;

public class InvisibleSurface : Panel
{
  protected override CreateParams CreateParams
  {
    get
    {
      var createParams = base.CreateParams;
      createParams.ExStyle |= 0x00000020;
      return createParams;
    }
  }

  public InvisibleSurface()
  {
    SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    BackColor = Color.Transparent;
  }
}