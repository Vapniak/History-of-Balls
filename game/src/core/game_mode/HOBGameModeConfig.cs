namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public abstract partial class HOBGameModeConfig : GameModeConfig {
  public MissionData Mission { get; set; } = default!;
  public abstract override HOBGameMode CreateGameMode();
  public override void ConfigureGameMode(GameMode gameMode) {
    if (gameMode is HOBGameMode hob) {
      hob.MissionData = Mission;
    }
  }
}