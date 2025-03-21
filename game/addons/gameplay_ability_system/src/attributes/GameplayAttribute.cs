namespace GameplayAbilitySystem;

using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class GameplayAttribute : Resource {
  [Export] public string? AttributeName { get; private set; }
}
