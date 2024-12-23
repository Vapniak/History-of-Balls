namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameModeComponent : Node {
  public GameMode OwnerGameMode { get; set; }

  public GameState GameState { get; set; }
  public virtual void Init() { }
}
