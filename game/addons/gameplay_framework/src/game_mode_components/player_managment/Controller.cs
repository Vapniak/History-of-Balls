namespace GameplayFramework;

using Godot;

/// <summary>
/// Works like bridge between game, player state and character. Controls logic of character.
/// </summary>
[GlobalClass]
public partial class Controller : Node3D, IController {
  private PlayerState PlayerState { get; set; }

  public void SetPlayerState(PlayerState playerState) {
    PlayerState = playerState;
  }

  public PlayerState GetPlayerState() => PlayerState;
  public T GetPlayerState<T>() where T : class, IPlayerState {
    return GetPlayerState() as T;
  }

  public virtual IGameState GetGameState() => GameInstance.GetGameState<IGameState>();
}
