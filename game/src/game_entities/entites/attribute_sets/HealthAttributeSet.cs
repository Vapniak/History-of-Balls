
namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public partial class HealthAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute Health { get; private set; } = new();
}
