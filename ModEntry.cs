// using System;
// using Microsoft.Xna.Framework;
using System.Linq;
using GenericModConfigMenu;
using Microsoft.Build.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
// using StardewModdingAPI.Utilities;
using StardewValley;
// using System.ComponentModel;
using TeleportToYourLove.Framework;

// https://github.com/Strrato/StardewWrapMultiplayer/blob/master/WarpMultiplayer/ModEntry.cs

namespace TeleportToYourLove
{

  internal class ModEntry : Mod
  {

    // public static Mod Instance { get; private set; }

    public ModConfig Config { get; private set; }

    public override void Entry(IModHelper helper)
    {
      // Instance = this;
      Config = Helper.ReadConfig<ModConfig>();

      helper.Events.Input.ButtonPressed += OnButtonPressed;
      helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }


    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
      // get Generic Mod Config Menu's API (if it's installed)
      var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
      if (configMenu is null)
        return;

      // register mod
      configMenu.Register(
          mod: ModManifest,
          reset: () => Config = new ModConfig(),
          save: () => Helper.WriteConfig(Config)
      );

      // // add some config options
      // configMenu.AddBoolOption(
      //     mod: ModManifest,
      //     name: () => "Example checkbox",
      //     tooltip: () => "An optional description shown as a tooltip to the player.",
      //     getValue: () => Config.ExampleCheckbox,
      //     setValue: value => Config.ExampleCheckbox = value
      // );
      // configMenu.AddTextOption(
      //     mod: ModManifest,
      //     name: () => "Example dropdown",
      //     getValue: () => Config.ExampleDropdown,
      //     setValue: value => Config.ExampleDropdown = value,
      //     allowedValues: new string[] { "choice A", "choice B", "choice C" }
      // );

      configMenu.AddTextOption(
          mod: ModManifest,
          name: () => "Your love name",
          getValue: () => Config.YourLoveName,
          setValue: value => Config.YourLoveName = value
      );


    }

    /*********
    ** Private methods
    *********/
    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      // ignore if player hasn't loaded a save yet
      if (!Context.IsWorldReady)
        return;

      bool isUseToolButton = e.Button.IsUseToolButton();

      if (isUseToolButton && Game1.player.CurrentItem != null)
      {
        bool isWeddingRing = Game1.player.CurrentItem.ParentSheetIndex == 801;

        if (isWeddingRing)
        {
          // Check if another connected player to your world is married to you
          foreach (var player in Game1.getAllFarmers())
          {
            if (player != null && player != Game1.player)
            {
              // Check if the player is the target player
              bool isLocalTargetPlayer = Config.YourLoveName == Game1.player.Name;
              var connectedPlayers = Helper.Multiplayer.GetConnectedPlayers();
              var connectedHostPlayer = connectedPlayers.FirstOrDefault(peer => peer.IsHost);
              var hostPlayer = connectedHostPlayer != null ? Game1.getFarmer(connectedHostPlayer.PlayerID) : Game1.player;
              bool isTargetPlayer = (isLocalTargetPlayer ? hostPlayer.Name : Config.YourLoveName) == player.Name;

              Monitor.Log($"Game1.player.Name: {Game1.player.Name}", LogLevel.Debug);
              Monitor.Log($"Config.YourLoveName: {Config.YourLoveName}", LogLevel.Debug);
              Monitor.Log($"hostPlayer.Name: {hostPlayer.Name}", LogLevel.Debug);

              if (isTargetPlayer)
              {
                // Teleport me to my love
                bool isUniqueNameLocation = !string.IsNullOrEmpty(player.currentLocation.uniqueName.Value);
                string toLocation = isUniqueNameLocation ? player.currentLocation.uniqueName.Value : player.currentLocation.Name;
                Warp destiny = new Warp(0, 0, toLocation, (int)(player.position.X + 16) / Game1.tileSize, (int)(player.position.Y) / Game1.tileSize, false);
                Game1.player.warpFarmer(destiny);
              }
            }
          }
        }
      }
    }
  }
}