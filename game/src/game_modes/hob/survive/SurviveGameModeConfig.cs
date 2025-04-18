namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class SurviveGameModeConfig : HOBGameModeConfig, IGameModeConfig<SurviveGameMode> {
  [Export] public uint RoundsToSurvive { get; private set; }

  public SurviveGameMode CreateGameMode() {
    var gm = ResourceLoader.Load<PackedScene>("uid://dqhwhujtv6m5t").Instantiate<SurviveGameMode>();
    gm.RoundsToSurvive = RoundsToSurvive;
    GD.Print(gm.RoundsToSurvive);
    return gm;
  }
}
