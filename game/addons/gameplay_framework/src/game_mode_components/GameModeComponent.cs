namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameModeComponent : Node, IGameModeComponent<IGameState> {
  public GameMode OwnerGameMode { get; set; }

  internal GameState GameState { get; set; }

  public virtual void Init() { }
  public virtual IGameState GetGameState() => GameState;
}
