namespace GameplayAbilitySystem;

using System;
using Godot;
using Godot.Collections;

public class GameplayEventData {
  public Array<Node> Targets { get; set; }
  public Array<Vector3> TargetPositions { get; set; }
}
