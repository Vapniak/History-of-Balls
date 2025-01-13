namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameModeComponent : Node, IGetGameState<IGameState> {
  public GameMode OwnerGameMode { get; set; }

  public GameState GameState { get; set; }

  public virtual void Init() { }

  public virtual IGameState GetGameState() => GameState;
}
