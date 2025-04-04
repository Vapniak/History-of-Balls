namespace GameplayAbilitySystem;

using Godot;

[GlobalClass, Tool]
public partial class GameplayAttribute : Resource {
  [Export] public string AttributeName { get; private set; } = "Attribute";
}
