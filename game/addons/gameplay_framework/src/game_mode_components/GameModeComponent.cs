namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameModeComponent : Node {
  public GameMode OwnerGameMode { get; set; }

  public virtual IGameState GetGameState() => OwnerGameMode.GetGameState();
}
