namespace GameplayFramework;

using Godot.Collections;

public interface IPlayerManagmentGameState : IGameState {
  public Array<PlayerState> PlayerArray { get; set; }
}
