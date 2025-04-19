namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public abstract partial class HOBAbility : GameplayAbility {
  [Export] public Texture2D? Icon { get; protected set; }
  [Export] public bool ShowInUI { get; protected set; } = true;
  [Export] public int UIOrder { get; protected set; }
}
