namespace GameplayFramework;

using System.Collections.Generic;
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


  public T GetGameState<T>() where T : GameState => GetGameState() as T;
  public GameState GetGameState() => GameState;

  public T GetGameModeComponent<T>() where T : GameModeComponent {
    foreach (var component in GameModeComponents) {
      if (component is T comp) {
        return comp;
      }
    }

    return null;
  }

  protected virtual GameState CreateGameState() {
    return new GameState();
  }

}
