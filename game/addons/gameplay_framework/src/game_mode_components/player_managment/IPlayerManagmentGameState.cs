namespace GameplayFramework;

using Godot.Collections;

public interface IPlayerManagmentGameState : IGameState {
  public Array<PlayerState> PlayerArray { get; set; }

  public virtual PlayerState GetLocalPlayer() {
    return PlayerArray[0];
  }
}
