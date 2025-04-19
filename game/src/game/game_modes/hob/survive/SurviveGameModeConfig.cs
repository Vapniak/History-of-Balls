namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class SurviveGameModeConfig : HOBGameModeConfig {
  [Export] public uint RoundsToSurvive { get; private set; }

  public override SurviveGameMode CreateGameMode() {
    var gm = ResourceLoader.Load<PackedScene>("uid://dqhwhujtv6m5t").Instantiate<SurviveGameMode>();
    return gm;
  }

  public override void ConfigureGameMode(GameMode gameMode) {
    base.ConfigureGameMode(gameMode);
    if (gameMode is SurviveGameMode hob) {
      hob.RoundsToSurvive = RoundsToSurvive;
    }
  }

}
