namespace GameplayFramework;

using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Manages game logic.
/// </summary>
[GlobalClass]
public partial class GameMode : Node {
  private GameState GameState { get; set; }

  private List<GameModeComponent> GameModeComponents { get; set; } = new();

  public virtual void Init() {
    GameState = CreateGameState();
    GameState.Init();

    foreach (var child in GetChildren()) {
      if (child is GameModeComponent component) {
        GameModeComponents.Add(component);
        component.OwnerGameMode = this;
        component.GameState = GameState;
        component.Init();
      }
    }
  }

  protected virtual GameState CreateGameState() {
    return new GameState();
  }

  public T GetGameState<T>() where T : GameState => GetGameState() as T;
  public GameState GetGameState() => GameState;

  public T GetGameModeComponent<T>() where T : GameModeComponent => GameModeComponents.OfType<T>().First();
}
