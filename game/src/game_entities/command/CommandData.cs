namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class CommandData : Resource {
  [Export] public string CommandName { get; private set; } = "Command";
  [Export] public bool ShowInUI { get; private set; }
  [Export] public uint CooldownRounds { get; private set; } = 1;
  [Export] public Texture2D Icon { get; private set; }
}
