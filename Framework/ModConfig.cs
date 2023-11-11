
namespace TeleportToYourLove.Framework
{
  public sealed class ModConfig
  {
    // Definition of your config file
    public string YourLoveName { get; set; }

    public ModConfig()
    {
      // Set default values
      YourLoveName = "";
    }
  }
}