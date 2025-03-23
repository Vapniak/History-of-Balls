namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public abstract partial class HOBAbilityResource : GameplayAbilityResource {
  [Export] public Texture2D Icon { get; private set; }
}
