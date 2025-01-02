namespace GameplayFramework;

using Godot.Collections;

public interface IPlayerManagmentGameState : IGameState {
  Array<PlayerState> PlayerArray { get; set; }
}
