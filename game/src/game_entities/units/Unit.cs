namespace HOB;

using GameEntitySystem;
using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Unit : CharacterBody3D, IGameEntity {
  public IGameEntityData GameEntityData => throw new NotImplementedException();

  public Array<GameEntityComponent> EntityComponents => throw new NotImplementedException();

  public override void _Ready() {

  }
}
