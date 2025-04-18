namespace GameplayFramework;

using Godot;

[GlobalClass]
public abstract partial class GameModeConfig : Resource, IGameModeConfig<GameMode> {
  public abstract void ConfigureGameMode(GameMode gameMode);
  public abstract GameMode CreateGameMode();
}
