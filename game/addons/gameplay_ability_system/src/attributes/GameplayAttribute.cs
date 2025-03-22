namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayAttribute : Resource {
  [Export] public string AttributeName { get; private set; } = "Attribute";
}
