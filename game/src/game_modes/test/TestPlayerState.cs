namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;
using HOB.GameEntity;
using System;

[GlobalClass]
public partial class TestPlayerState : PlayerState {
  public Array<EntityDefinition> Entities { get; set; }
}
