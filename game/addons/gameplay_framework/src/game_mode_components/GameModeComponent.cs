namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameModeComponent : Node {
  public virtual IGameState GetGameState() => GameInstance.GetGameState();
}
