namespace GameplayFramework;

using Godot;

/// <summary>
/// Manages game logic.
/// </summary>
[GlobalClass]
public partial class GameMode : Node, IGameMode {
  private IGameState? GameState { get; set; }

  public override void _EnterTree() {
    GameState = CreateGameState();
  }

  public virtual IGameState GetGameState() => GameState!;

  protected virtual IGameState CreateGameState() {
    return new GameState();
  }
}
