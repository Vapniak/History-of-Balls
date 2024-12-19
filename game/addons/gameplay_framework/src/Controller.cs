namespace GameplayFramework;

using Godot;

[GlobalClass]
public partial class Controller : Node3D {
  public PlayerState PlayerState { get; private set; }

  public void SetPlayerState(PlayerState playerState) {
    PlayerState = playerState;
  }

  public T GetPlayerState<T>() where T : PlayerState {
    return PlayerState as T;
  }
}
